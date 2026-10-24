using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Models.carrinho;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CarrinhoRepository : ICarrinhoRepository
{
    private readonly AppDbContext _db;

    public CarrinhoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Carrinho?> ObterPorId(Guid carrinhoId)
    {
        return await _db.Carrinho.FindAsync(carrinhoId);
    }

    public async Task<Carrinho?> ObterItemUsuarioProduto(Guid usuarioId, Guid idProduto)
    {
        return await _db.Carrinho.FirstOrDefaultAsync(c =>
            c.IdUsuario == usuarioId && c.IdProduto == idProduto);
    }

    public async Task<Carrinho> AtualizarItem(Carrinho carrinho)
    {
        _db.Carrinho.Update(carrinho);
        await _db.SaveChangesAsync();
        return carrinho;
    }

    public async Task<Carrinho> CriarProdutoCarrinho(Carrinho carrinho)
    {
        if (carrinho.Id == Guid.Empty)
            carrinho.Id = Guid.NewGuid();

        await _db.Carrinho.AddAsync(carrinho);
        await _db.SaveChangesAsync();
        return carrinho;
    }

    public async Task<CarrinhoResponse> ListarCarrinho(Guid usuarioId)
    {
        var itens = await _db.Carrinho
            .Where(c => c.IdUsuario == usuarioId)
            .Join(
                _db.Produtos.Include(p => p.Imagens),
                c => c.IdProduto,
                p => p.Id,
                (c, p) => new { c, p })
            .ToListAsync();

        var responseItens = itens.Select(x =>
        {
            var valor = x.p.Valor ?? x.p.ValorOriginal;
            return new CarrinhoItemResponse
            {
                Id = x.c.Id,
                IdProduto = x.p.Id,
                NomeProduto = x.p.NomeProduto,
                ValorUnitario = valor,
                Quantidade = x.c.Quantidade,
                TotalItem = valor * x.c.Quantidade,
                ImagemUrl = x.p.Imagens.Select(i => i.Caminho).FirstOrDefault() ?? string.Empty,
                Descricao = x.p.Descricao
            };
        }).ToList();

        var subtotal = responseItens.Sum(i => i.TotalItem);

        return new CarrinhoResponse
        {
            Subtotal = subtotal,
            TaxaEntrega = 0,
            Total = subtotal,
            Itens = responseItens
        };
    }

    public async Task<bool> ExcluirCarrinho(Guid carrinhoId)
    {
        var item = await _db.Carrinho.FindAsync(carrinhoId);
        if (item == null)
            return false;

        _db.Carrinho.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}
