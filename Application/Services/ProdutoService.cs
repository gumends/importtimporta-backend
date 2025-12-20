using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IS3Service _awsS3Service;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IS3Service s3Service
        )
        {
            _produtoRepository = produtoRepository;
            _awsS3Service = s3Service;
        }

        public async Task<Produto> AdicionaProduto(Produto produto, List<IFormFile>? imagens)
        {
            ArgumentNullException.ThrowIfNull(produto);

            produto.CalcularValores();

            var novoProduto = await _produtoRepository.AdicionaProduto(produto);

            if (imagens != null && imagens.Count > 0)
            {
                novoProduto.Imagens = new List<Imagem>();

                foreach (var imagem in imagens)
                {
                    var caminhoS3 = await _awsS3Service.UploadAsync(imagem);

                    novoProduto.Imagens.Add(new Imagem
                    {
                        Caminho = caminhoS3,
                        Descricao = $"Imagem do produto {novoProduto.NomeProduto}",
                        ProdutoId = novoProduto.Id
                    });
                }

                await _produtoRepository.SalvarImagens(novoProduto.Imagens);
            }

            return novoProduto;
        }

        public async Task<Produto> AtualizaProduto(
        int id,
        Produto novosDados,
        List<IFormFile>? novasImagens,
        List<string>? imagensExistentes
        )
        {
            var produtoExistente = await _produtoRepository.ObterProdutoPorId(id);

            if (produtoExistente == null)
                throw new Exception("Produto não encontrado.");

            produtoExistente.NomeProduto = novosDados.NomeProduto;
            produtoExistente.ValorOriginal = novosDados.ValorOriginal;
            produtoExistente.Desconto = novosDados.Desconto;
            produtoExistente.ValorParcelado = novosDados.ValorParcelado;
            produtoExistente.Descricao = novosDados.Descricao;
            produtoExistente.TipoProduto = novosDados.TipoProduto;
            produtoExistente.NovoLancamento = novosDados.NovoLancamento;
            produtoExistente.NovaGeracao = novosDados.NovaGeracao;
            produtoExistente.Disponivel = novosDados.Disponivel;
            produtoExistente.MesesGarantia = novosDados.MesesGarantia;

            produtoExistente.Color = novosDados.Color;
            produtoExistente.ColorName = novosDados.ColorName;

            produtoExistente.CalcularValores();

            if (novosDados.InformacoesAdicionais != null)
            {
                if (produtoExistente.InformacoesAdicionais == null)
                    produtoExistente.InformacoesAdicionais = new Informacoes();

                produtoExistente.InformacoesAdicionais.Marca = novosDados.InformacoesAdicionais.Marca;
                produtoExistente.InformacoesAdicionais.ArmazenamentoInterno = novosDados.InformacoesAdicionais.ArmazenamentoInterno;
                produtoExistente.InformacoesAdicionais.TipoTela = novosDados.InformacoesAdicionais.TipoTela;
                produtoExistente.InformacoesAdicionais.TamanhoTela = novosDados.InformacoesAdicionais.TamanhoTela;
                produtoExistente.InformacoesAdicionais.ResolucaoTela = novosDados.InformacoesAdicionais.ResolucaoTela;
                produtoExistente.InformacoesAdicionais.Tecnologia = novosDados.InformacoesAdicionais.Tecnologia;
                produtoExistente.InformacoesAdicionais.Processador = novosDados.InformacoesAdicionais.Processador;
                produtoExistente.InformacoesAdicionais.SistemaOperacional = novosDados.InformacoesAdicionais.SistemaOperacional;
                produtoExistente.InformacoesAdicionais.CameraTraseira = novosDados.InformacoesAdicionais.CameraTraseira;
                produtoExistente.InformacoesAdicionais.CameraFrontal = novosDados.InformacoesAdicionais.CameraFrontal;
                produtoExistente.InformacoesAdicionais.Bateria = novosDados.InformacoesAdicionais.Bateria;
                produtoExistente.InformacoesAdicionais.QuantidadeChips = novosDados.InformacoesAdicionais.QuantidadeChips;
                produtoExistente.InformacoesAdicionais.Material = novosDados.InformacoesAdicionais.Material;
            }

            produtoExistente.Imagens ??= new List<Imagem>();
            var imagensAtuais = produtoExistente.Imagens.ToList();

            var caminhosMantidos = imagensExistentes ?? imagensAtuais.Select(i => i.Caminho).ToList();

            var imagensParaRemover = imagensAtuais
                .Where(i => !caminhosMantidos.Contains(i.Caminho))
                .ToList();

            foreach (var img in imagensParaRemover)
            {
                await _awsS3Service.DeleteAsync(img.Caminho);
                await _produtoRepository.RemoverImagem(img);
                produtoExistente.Imagens.Remove(img);
            }

            if (novasImagens != null && novasImagens.Count > 0)
            {
                foreach (var imagem in novasImagens)
                {
                    var caminhoS3 = await _awsS3Service.UploadAsync(imagem);

                    var novaImagem = new Imagem
                    {
                        Caminho = caminhoS3,
                        ProdutoId = produtoExistente.Id,
                        Descricao = $"Imagem do produto {produtoExistente.NomeProduto}"
                    };

                    await _produtoRepository.AdicionarImagem(novaImagem);
                    produtoExistente.Imagens.Add(novaImagem);
                }
            }

            await _produtoRepository.AtualizaProduto(produtoExistente);

            return produtoExistente;
        }

        public async Task<Produto?> ObterProdutoPorId(int id)
        {
            return await _produtoRepository.ObterProdutoPorId(id);
        }


        public async Task<bool> RemoveProduto(int id)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(id);

            if (produto == null)
                return false;

            foreach (var item in produto.Imagens)
            {
                var uri = new Uri(item.Caminho);
                var chave = uri.AbsolutePath.TrimStart('/');

                await _awsS3Service.DeleteAsync(chave);
            }

            return await _produtoRepository.RemoveProduto(id);
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutos(int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosPaginado(pagina, tamanhoPagina);
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosPorTipo(tipoProduto, pagina, tamanhoPagina);
        }

        public Task<Produto> AtualizarPreco(decimal valorOriginal, decimal desconto, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosPromocao(int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosEmPromocao(pagina, tamanhoPagina);

        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosNovaGeracao(int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosPaginado(pagina, tamanhoPagina);
        }

        public async Task<Produto> DesativarAtivaProduto(int id)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(id);

            if (produto is null)
                throw new DirectoryNotFoundException();

            produto.Disponivel = produto.Disponivel == false ? true : false;

            await _produtoRepository.AtualizaProduto(produto);

            return produto;
        }

        public async Task<List<Produto>> BuscaProdutosVariados(int contidade)
        {
            var produtos = await _produtoRepository.ListaProdutos();

            var produtosAleatorios = produtos
                .OrderBy(p => Guid.NewGuid())
                .Take(contidade)
                .ToList();

            return produtosAleatorios;
        }

    }
}
