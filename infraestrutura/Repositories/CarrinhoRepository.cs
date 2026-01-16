using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Models.carrinho;
using Domain.Models.Produto;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;
public class CarrinhoRepository : ICarrinhoRepository
{
    private readonly AppDbContext _db;
    private readonly IProdutoRepository _produtoRepository;

    public CarrinhoRepository(AppDbContext db, IProdutoRepository produtoRepository)
    {
        _db = db;
        _produtoRepository = produtoRepository;
    }

    public async Task<Carrinho> inserirProdutoCarrinho(Carrinho carrinho)
    {
        var findCarrinho = await _db.Carrinho.FirstOrDefaultAsync(c => c.IdUsuario == carrinho.IdUsuario && c.IdProduto == carrinho.IdProduto);
        if (findCarrinho != null)
        {
            await SomarItem(carrinho.IdUsuario,  carrinho.IdProduto);
            return findCarrinho;
        }
            
        await _db.Carrinho.AddAsync(carrinho);
        await _db.SaveChangesAsync();
        return carrinho;
    }

    public async Task SomarItem(int usuarioId, int produtoId)
    {
        var carrinho = await _db.Carrinho.FirstOrDefaultAsync(c => c.IdUsuario == usuarioId && c.IdProduto == produtoId);
        if (carrinho == null)
            return;
        carrinho.Quantidade++;
        _db.Carrinho.Update(carrinho);
        await _db.SaveChangesAsync();
    }

    public async Task<CarrinhoDto> ListarCarrinho(int usuarioId)
    {
        var carrinho = new CarrinhoDto();
        var newCarrinho = new List<NewCarrinhoDto>();

        var carrinhoUsuario = await _db.Carrinho
            .AsNoTracking()
            .Where(c => c.IdUsuario == usuarioId)
            .ToListAsync();
        
        decimal subtotal = 0;
        decimal taxa = 0;
        foreach (var c in carrinhoUsuario)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(c.IdProduto);
            subtotal += (produto.Valor ?? 0) * c.Quantidade;

            newCarrinho.Add(new NewCarrinhoDto
            {
                Id = c.Id,
                IdUsuario = c.IdUsuario,
                CriadoEm = c.CriadoEm,
                IdProduto = c.IdProduto,
                Produto = produto,
                Quantidade = c.Quantidade
            });
        }
        
        carrinho.Carrinhos = newCarrinho;
        carrinho.Subtotal = subtotal;
        carrinho.Total = subtotal + taxa;
        carrinho.TaxaEntrega = taxa;

        return carrinho;
    }

    public async Task<bool> ExcluirCarrinho(int carrinhoId)
    {
        var carrinho = await _db.Carrinho.FindAsync(carrinhoId);
        if (carrinho == null)
        {
            return false;
        }
        _db.Carrinho.Remove(carrinho);
        await _db.SaveChangesAsync();
        return true;
    }
}