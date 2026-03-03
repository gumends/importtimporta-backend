using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Repositories
{
    public interface IProdutoRepository
    {
        Task<Produto> AdicionaProduto(ProdutoDto produto);

        Task<PaginacaoResultado<Produto>> ListaProdutosPaginado(int pagina, int tamanhoPagina, decimal? precoMinimo, decimal? precoMaximo, string? nomeProduto);

        Task<Produto?> ObterProdutoPorId(Guid id);

        Task<Produto> AtualizaProduto(Produto produto);

        Task<bool> RemoveProduto(Guid id);

        Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina);

        Task<PaginacaoResultado<Produto>> ListaProdutosEmPromocao(int pagina, int tamanhoPagina);
        Task<List<Produto>> ListaProdutos();
        Task RemoverImagens(List<Imagem> imagens);
        Task AdicionarImagem(Imagem imagem);
        Task RemoverImagem(Imagem imagem);
    }
}
