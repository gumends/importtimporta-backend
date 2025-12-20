using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.Services
{
    public interface IProdutoService
    {
        Task<Produto> AdicionaProduto(Produto produto, List<IFormFile>? imagens);
        Task<Produto?> ObterProdutoPorId(int id);
        Task<bool> RemoveProduto(int id);
        Task<Produto> AtualizaProduto(
            int id,
            Produto novosDados,
            List<IFormFile>? novasImagens,
            List<string>? imagensExistentes);
        Task<PaginacaoResultado<Produto>> ListaProdutos(int pagina, int tamanhoPagina);
        Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina);
        Task<Produto> AtualizarPreco(decimal valorOriginal, decimal desconto, int id);
        Task<PaginacaoResultado<Produto>> ListaProdutosPromocao(int pagina, int tamanhoPagina);
        Task<PaginacaoResultado<Produto>> ListaProdutosNovaGeracao(int pagina, int tamanhoPagina);
        Task<Produto> DesativarAtivaProduto(int id);
        Task<List<Produto>> BuscaProdutosVariados(int quantidade);
    }
}