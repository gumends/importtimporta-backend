using Domain.DTO;
using Domain.Models.carrinho;

namespace Application.Interfaces.Services;

public interface ICarrinhoService
{
    Task<Carrinho> PostCarrinho(CarrinhoDto carrinhoRequest, Guid usuarioId);
    Task<CarrinhoResponse> Carrinho(Guid idUsuario);
    Task<bool> ExcluirItemCarrinho(Guid carrinhoId);
}