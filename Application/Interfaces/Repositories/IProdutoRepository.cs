using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;

namespace Application.Interfaces.Repositories
{
    public interface IProdutoRepository
    {
        Task<Produto> AdicionaProduto(Produto produto);

        Task<PaginacaoResultado<Produto>> ListaProdutosPaginado(int pagina, int tamanhoPagina);

        Task<Produto?> ObterProdutoPorId(int id);

        Task<Produto> AtualizaProduto(Produto produto);

        Task<bool> RemoveProduto(int id);

        Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina);

        Task<PaginacaoResultado<Produto>> ListaProdutosEmPromocao(int pagina, int tamanhoPagina);
        Task SalvarImagens(IEnumerable<Imagem> imagens);
        Task<List<Produto>> ListaProdutos();
        Task RemoverImagens(List<Imagem> imagens);
        Task AdicionarImagem(Imagem imagem);
        Task RemoverImagem(Imagem imagem);
    }
}
