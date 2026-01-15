using Domain.DTO;
using Domain.Models.carrinho;

namespace Application.Interfaces.Repositories;

public interface ICarrinhoRepository
{
    Task<Carrinho> inserirProdutoCarrinho(Carrinho carrinho);
    Task<CarrinhoDto> ListarCarrinho(int UsuarioId);
    Task<bool> ExcluirCarrinho(int carrinhoId);
}