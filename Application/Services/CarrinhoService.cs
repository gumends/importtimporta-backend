using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Models.carrinho;

namespace Application.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly ICarrinhoRepository _carrinhoRepository;

    public CarrinhoService(ICarrinhoRepository carrinhoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
    }

    public async Task<Carrinho> PostCarrinho(CarrinhoRequest carrinhoRequest, int usuarioId)
    {
        var carrinho = new Carrinho
        {
            CriadoEm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
            IdProduto = carrinhoRequest.IdProduto,
            Quantidade = carrinhoRequest.Quantidade,
            IdUsuario = usuarioId
        };
            
        var postCarrinho = _carrinhoRepository.inserirProdutoCarrinho(carrinho);
        return await postCarrinho;
    }

    public async Task<CarrinhoDto> Carrinho(int idUsuario)
    {
        var getCarrinho = _carrinhoRepository.ListarCarrinho(idUsuario);
        return await getCarrinho;
    }

    public async Task<bool> ExcluirItemCarrinho(int carrinhoId)
    {
        var deleteCarrinho = _carrinhoRepository.ExcluirCarrinho(carrinhoId);
        return await deleteCarrinho;
    }
}