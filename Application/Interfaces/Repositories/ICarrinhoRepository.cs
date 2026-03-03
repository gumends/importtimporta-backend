using Domain.DTO;
using Domain.Models.carrinho;

namespace Application.Interfaces.Repositories;

public interface ICarrinhoRepository
{
    Task<Carrinho> CriarProdutoCarrinho(Carrinho carrinho);
    Task<CarrinhoResponse> ListarCarrinho(Guid UsuarioId);
    Task<bool> ExcluirCarrinho(Guid carrinhoId);
}