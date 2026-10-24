namespace Domain.Models.Frete;

public class CotarFreteRequest
{
    public string CepDestino { get; set; } = string.Empty;
    public string? OpcaoSelecionada { get; set; }
    public List<CotarFreteItemRequest> Itens { get; set; } = new();
}

public class CotarFreteItemRequest
{
    public Guid? IdProduto { get; set; }
    public int Quantidade { get; set; } = 1;
    /// <summary>Usado quando o carrinho é local (sem id de produto no banco).</summary>
    public int? TipoProduto { get; set; }
    public decimal? ValorUnitario { get; set; }
}

public class CotarFreteResponse
{
    public string CepDestino { get; set; } = string.Empty;
    public List<OpcaoFreteDto> Opcoes { get; set; } = new();
    public string? OpcaoSelecionada { get; set; }
    public decimal ValorSelecionado { get; set; }
    public int PrazoMinDias { get; set; }
    public int PrazoMaxDias { get; set; }
    public bool Estimativa { get; set; }
}

public class OpcaoFreteDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int PrazoMinDias { get; set; }
    public int PrazoMaxDias { get; set; }
}
