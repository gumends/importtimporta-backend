using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Services
{
    public interface IProdutoService
    {
        Task<Produto> AdicionaProduto(ProdutoDto produto);
        Task<Produto?> ObterProdutoPorId(Guid id);
        Task<bool> RemoveProduto(Guid id);
        Task<Produto> AtualizaProduto(
            Guid id,
            Produto novosDados,
            List<IFormFile>? novasImagens,
            List<string>? imagensExistentes);
        Task<PaginacaoResultado<Produto>> ListaProdutos(int pagina, int tamanhoPagina, decimal? precoMinimo,
            decimal? precoMaximo, string? nomeProduto);
        Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina);
        Task<Produto> AtualizarPreco(decimal valorOriginal, decimal desconto, int id);
        Task<PaginacaoResultado<Produto>> ListaProdutosPromocao(int pagina, int tamanhoPagina);
        Task<PaginacaoResultado<Produto>> ListaProdutosNovaGeracao(int pagina, int tamanhoPagina);
        Task<Produto> DesativarProduto(Guid id);
        Task<Produto> AtivarProduto(Guid id);
        Task<List<Produto>> BuscaProdutosVariados(int quantidade);
        Task<List<string>> SalvarImagens(List<IFormFile> imagens, Guid idProduto);
    }
}