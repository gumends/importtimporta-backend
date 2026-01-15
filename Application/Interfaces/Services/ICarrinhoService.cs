using Domain.DTO;
using Domain.Models.carrinho;

namespace Application.Interfaces.Services;

public interface ICarrinhoService
{
    Task<Carrinho> PostCarrinho(CarrinhoRequest carrinhoRequest, int usuarioId);
    Task<CarrinhoDto> Carrinho(int idUsuario);
    Task<bool> ExcluirItemCarrinho(int carrinhoId);
}