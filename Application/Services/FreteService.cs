using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Frete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class FreteService : IFreteService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FreteService> _logger;

    public FreteService(
        IHttpClientFactory httpClientFactory,
        IProdutoRepository produtoRepository,
        IConfiguration configuration,
        ILogger<FreteService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _produtoRepository = produtoRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<CotarFreteResponse> CotarAsync(
        CotarFreteRequest request,
        CancellationToken cancellationToken = default)
    {
        var cepDestino = NormalizarCep(request.CepDestino);
        if (cepDestino.Length != 8)
            throw new BadRequestException("Informe um CEP válido com 8 dígitos.");

        if (request.Itens == null || request.Itens.Count == 0)
            throw new BadRequestException("Adicione itens ao carrinho para calcular o frete.");

        await ValidarCepExisteAsync(cepDestino, cancellationToken);

        var cepOrigem = NormalizarCep(_configuration["Frete:CepOrigem"] ?? "05711001");
        if (cepOrigem.Length != 8)
            cepOrigem = "05711001";

        var pacote = await MontarPacoteAsync(request.Itens, cancellationToken);
        var freteGratisAcima = decimal.TryParse(
            _configuration["Frete:FreteGratisAcimaDe"],
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var limiteGratis)
            ? limiteGratis
            : 0m;

        List<OpcaoFreteDto> opcoes;
        var estimativa = true;

        var token = _configuration["Frete:MelhorEnvioToken"];
        if (!string.IsNullOrWhiteSpace(token))
        {
            var me = await TentarMelhorEnvioAsync(
                cepOrigem,
                cepDestino,
                pacote,
                token,
                cancellationToken);
            if (me.Count > 0)
            {
                opcoes = me;
                estimativa = false;
            }
            else
            {
                opcoes = EstimarCorreios(cepOrigem, cepDestino, pacote.PesoKg, pacote.ValorDeclarado);
            }
        }
        else
        {
            opcoes = EstimarCorreios(cepOrigem, cepDestino, pacote.PesoKg, pacote.ValorDeclarado);
        }

        if (freteGratisAcima > 0 && pacote.ValorDeclarado >= freteGratisAcima)
        {
            foreach (var o in opcoes.Where(o => o.Codigo == "pac"))
                o.Valor = 0;
        }

        var codigo = string.IsNullOrWhiteSpace(request.OpcaoSelecionada)
            ? opcoes.FirstOrDefault()?.Codigo
            : request.OpcaoSelecionada.ToLowerInvariant();

        var selecionada = opcoes.FirstOrDefault(o => o.Codigo == codigo)
            ?? opcoes.FirstOrDefault();

        return new CotarFreteResponse
        {
            CepDestino = cepDestino,
            Opcoes = opcoes,
            OpcaoSelecionada = selecionada?.Codigo,
            ValorSelecionado = selecionada?.Valor ?? 0,
            PrazoMinDias = selecionada?.PrazoMinDias ?? 0,
            PrazoMaxDias = selecionada?.PrazoMaxDias ?? 0,
            Estimativa = estimativa,
        };
    }

    private async Task ValidarCepExisteAsync(string cep, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("Frete");
        using var response = await client.GetAsync(
            $"https://viacep.com.br/ws/{cep}/json/",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new BadRequestException("Não foi possível validar o CEP.");

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (json.TryGetProperty("erro", out _))
            throw new BadRequestException("CEP não encontrado.");
    }

    private async Task<FretePacote> MontarPacoteAsync(
        List<CotarFreteItemRequest> itens,
        CancellationToken cancellationToken)
    {
        decimal valor = 0;
        double peso = 0;
        var produtosMe = new List<object>();

        foreach (var item in itens)
        {
            var qtd = Math.Max(1, item.Quantidade);
            int tipo = item.TipoProduto ?? 1;
            decimal valorUnit = item.ValorUnitario ?? 0;

            if (item.IdProduto.HasValue && item.IdProduto.Value != Guid.Empty)
            {
                var produto = await _produtoRepository.ObterProdutoPorId(item.IdProduto.Value);
                if (produto != null)
                {
                    tipo = produto.TipoProduto;
                    valorUnit = produto.Valor ?? produto.ValorOriginal;
                    if (valorUnit <= 0)
                        valorUnit = produto.ValorOriginal;
                }
            }

            valor += valorUnit * qtd;
            var dims = DimensoesPorTipo(tipo);
            var pesoItem = PesoPorTipo(tipo);
            peso += pesoItem * qtd;

            produtosMe.Add(new
            {
                id = item.IdProduto?.ToString() ?? Guid.NewGuid().ToString(),
                width = dims.LarguraCm,
                height = dims.AlturaCm,
                length = dims.ComprimentoCm,
                weight = pesoItem,
                insurance_value = (double)(valorUnit * qtd),
                quantity = qtd,
            });
        }

        peso = Math.Max(0.3, peso);

        return new FretePacote
        {
            PesoKg = peso,
            ValorDeclarado = valor,
            ProdutosMelhorEnvio = produtosMe,
        };
    }

    private async Task<List<OpcaoFreteDto>> TentarMelhorEnvioAsync(
        string cepOrigem,
        string cepDestino,
        FretePacote pacote,
        string token,
        CancellationToken cancellationToken)
    {
        var sandbox = _configuration.GetValue("Frete:MelhorEnvioSandbox", true);
        var baseUrl = sandbox
            ? "https://sandbox.melhorenvio.com.br"
            : "https://melhorenvio.com.br";

        var client = _httpClientFactory.CreateClient("Frete");
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"{baseUrl}/api/v2/me/shipment/calculate");
        httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Bearer {token}");
        httpRequest.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequest.Headers.TryAddWithoutValidation("User-Agent", "ImporttImporta (contato@importtimporta.com.br)");

        var body = new
        {
            from = new { postal_code = cepOrigem },
            to = new { postal_code = cepDestino },
            products = pacote.ProdutosMelhorEnvio,
        };

        httpRequest.Content = JsonContent.Create(body);

        try
        {
            using var response = await client.SendAsync(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var detail = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Melhor Envio retornou {Status}: {Detail}", response.StatusCode, detail);
                return new List<OpcaoFreteDto>();
            }

            var raw = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            return ParseMelhorEnvio(raw);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao cotar frete no Melhor Envio");
            return new List<OpcaoFreteDto>();
        }
    }

    private static List<OpcaoFreteDto> ParseMelhorEnvio(JsonElement raw)
    {
        var list = new List<OpcaoFreteDto>();

        if (raw.ValueKind != JsonValueKind.Array)
            return list;

        foreach (var item in raw.EnumerateArray())
        {
            var name = item.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
            var company = item.TryGetProperty("company", out var c) && c.TryGetProperty("name", out var cn)
                ? cn.GetString() ?? ""
                : "";

            if (!company.Contains("Correios", StringComparison.OrdinalIgnoreCase)
                && !name.Contains("PAC", StringComparison.OrdinalIgnoreCase)
                && !name.Contains("SEDEX", StringComparison.OrdinalIgnoreCase))
                continue;

            var codigo = name.Contains("SEDEX", StringComparison.OrdinalIgnoreCase) ? "sedex" : "pac";
            if (list.Any(o => o.Codigo == codigo))
                continue;

            var priceStr = item.TryGetProperty("price", out var p) ? p.GetString() : null;
            if (!decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var price)
                && !decimal.TryParse(priceStr, NumberStyles.Number, new CultureInfo("pt-BR"), out price))
                continue;

            var prazo = item.TryGetProperty("delivery_time", out var d) ? d.GetInt32() : 5;

            list.Add(new OpcaoFreteDto
            {
                Codigo = codigo,
                Nome = codigo == "sedex" ? "SEDEX" : "PAC",
                Valor = Math.Round(price, 2),
                PrazoMinDias = Math.Max(1, prazo - 1),
                PrazoMaxDias = prazo + 2,
            });
        }

        return list.OrderBy(o => o.Valor).ToList();
    }

    private static List<OpcaoFreteDto> EstimarCorreios(
        string cepOrigem,
        string cepDestino,
        double pesoKg,
        decimal valorDeclarado)
    {
        var fator = FatorDistanciaCep(cepOrigem, cepDestino);
        var seguro = (double)Math.Min(valorDeclarado * 0.002m, 80m);

        var pac = 14.90 + pesoKg * 6.5 + fator * 11.0 + seguro;
        var sedex = pac * 1.55 + 12.0 + fator * 4.0;

        pac = Math.Round(pac, 2);
        sedex = Math.Round(sedex, 2);

        var prazoPacMin = 5 + fator * 2;
        var prazoPacMax = 10 + fator * 3;
        var prazoSedexMin = 2 + fator;
        var prazoSedexMax = 5 + fator * 2;

        return new List<OpcaoFreteDto>
        {
            new()
            {
                Codigo = "pac",
                Nome = "PAC (Correios)",
                Valor = (decimal)pac,
                PrazoMinDias = prazoPacMin,
                PrazoMaxDias = prazoPacMax,
            },
            new()
            {
                Codigo = "sedex",
                Nome = "SEDEX (Correios)",
                Valor = (decimal)sedex,
                PrazoMinDias = prazoSedexMin,
                PrazoMaxDias = prazoSedexMax,
            },
        };
    }

    private static int FatorDistanciaCep(string origem, string destino)
    {
        if (!int.TryParse(origem[..5], out var o) || !int.TryParse(destino[..5], out var d))
            return 3;

        var diff = Math.Abs(o - d);
        if (diff < 50) return 1;
        if (diff < 500) return 2;
        if (diff < 2000) return 3;
        return 4;
    }

    private static double PesoPorTipo(int tipoProduto) => tipoProduto switch
    {
        1 => 0.45,
        2 => 2.8,
        3 => 0.12,
        _ => 0.35,
    };

    private static (double ComprimentoCm, double LarguraCm, double AlturaCm) DimensoesPorTipo(int tipoProduto) =>
        tipoProduto switch
        {
            1 => (18, 10, 6),
            2 => (38, 28, 8),
            3 => (16, 12, 4),
            _ => (20, 15, 8),
        };

    private static string NormalizarCep(string cep) =>
        Regex.Replace(cep ?? string.Empty, @"\D", "");

    private sealed class FretePacote
    {
        public double PesoKg { get; set; }
        public decimal ValorDeclarado { get; set; }
        public List<object> ProdutosMelhorEnvio { get; set; } = new();
    }
}
