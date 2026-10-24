using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Entities;
using Domain.Models.carrinho;

namespace Application.Services;

public class CarrinhoService : ICarrinhoService
{
    private readonly ICarrinhoRepository _carrinhoRepository;
    
    public CarrinhoService(ICarrinhoRepository carrinhoRepository)
    {
        _carrinhoRepository = carrinhoRepository;
    }

    public async Task<Carrinho> PostCarrinho(CarrinhoDto carrinhoRequest, Guid usuarioId)
    {
        if (carrinhoRequest.Quantidade <= 0)
            throw new BadRequestException("Quantidade inválida.");

        var existente = await _carrinhoRepository.ObterItemUsuarioProduto(
            usuarioId,
            carrinhoRequest.IdProduto);

        if (existente != null)
        {
            existente.Quantidade += carrinhoRequest.Quantidade;
            return await _carrinhoRepository.AtualizarItem(existente);
        }

        var carrinho = new Carrinho
        {
            CriadoEm = DateTime.UtcNow,
            IdProduto = carrinhoRequest.IdProduto,
            Quantidade = carrinhoRequest.Quantidade,
            IdUsuario = usuarioId
        };

        return await _carrinhoRepository.CriarProdutoCarrinho(carrinho);
    }

    public async Task<CarrinhoResponse> Carrinho(Guid idUsuario)
    {
        var getCarrinho = _carrinhoRepository.ListarCarrinho(idUsuario);
        return await getCarrinho;
    }

    public async Task<bool> ExcluirItemCarrinho(Guid carrinhoId)
    {
        var deleteCarrinho = _carrinhoRepository.ExcluirCarrinho(carrinhoId);
        return await deleteCarrinho;
    }
}