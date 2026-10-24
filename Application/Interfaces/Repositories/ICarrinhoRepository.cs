using Domain.DTO;
using Domain.Models.carrinho;

namespace Application.Interfaces.Repositories;

public interface ICarrinhoRepository
{
    Task<Carrinho> CriarProdutoCarrinho(Carrinho carrinho);
    Task<Carrinho?> ObterPorId(Guid carrinhoId);
    Task<Carrinho?> ObterItemUsuarioProduto(Guid usuarioId, Guid idProduto);
    Task<Carrinho> AtualizarItem(Carrinho carrinho);
    Task<CarrinhoResponse> ListarCarrinho(Guid UsuarioId);
    Task<bool> ExcluirCarrinho(Guid carrinhoId);
}