using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _db;

    public ProdutoRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Produto> AdicionaProduto(ProdutoDto produto)
    {
        var informacoes = produto.InformacoesProduto ?? new InformacoesProduto();
        if (informacoes.Id == Guid.Empty)
            informacoes.Id = Guid.NewGuid();

        var entity = new Produto(
            produto.NomeProduto,
            produto.ValorOriginal,
            produto.Desconto,
            produto.ValorParcelado,
            produto.Descricao,
            produto.TipoProduto,
            produto.MesesGarantia,
            produto.Quantidade,
            produto.Color,
            produto.ColorName,
            informacoes);

        entity.InformacoesProdutoId = informacoes.Id;

        await _db.Produtos.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<PaginacaoResultado<Produto>> ListaProdutosPaginado(
        int pagina,
        int tamanhoPagina,
        decimal? precoMinimo,
        decimal? precoMaximo,
        string? nomeProduto)
    {
        var query = QueryComRelacionamentos();

        if (!string.IsNullOrWhiteSpace(nomeProduto))
            query = query.Where(p => p.NomeProduto.Contains(nomeProduto));

        if (precoMinimo.HasValue && precoMinimo.Value > 0)
            query = query.Where(p => (p.Valor ?? p.ValorOriginal) >= precoMinimo.Value);

        if (precoMaximo.HasValue && precoMaximo.Value > 0)
            query = query.Where(p => (p.Valor ?? p.ValorOriginal) <= precoMaximo.Value);

        return await Paginar(query.OrderBy(p => p.NomeProduto), pagina, tamanhoPagina);
    }

    public async Task<Produto?> ObterProdutoPorId(Guid id)
    {
        return await QueryComRelacionamentos()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto> AtualizaProduto(Produto produto)
    {
        _db.Produtos.Update(produto);
        await _db.SaveChangesAsync();
        return produto;
    }

    public async Task<bool> RemoveProduto(Guid id)
    {
        var produto = await _db.Produtos.FindAsync(id);
        if (produto == null)
            return false;

        _db.Produtos.Remove(produto);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina)
    {
        var query = QueryComRelacionamentos()
            .Where(p => p.TipoProduto == tipoProduto)
            .OrderBy(p => p.NomeProduto);

        return Paginar(query, pagina, tamanhoPagina);
    }

    public Task<PaginacaoResultado<Produto>> ListaProdutosEmPromocao(int pagina, int tamanhoPagina)
    {
        var query = QueryComRelacionamentos()
            .Where(p => p.Desconto > 0)
            .OrderByDescending(p => p.Desconto);

        return Paginar(query, pagina, tamanhoPagina);
    }

    public async Task<List<Produto>> ListaProdutos()
    {
        return await QueryComRelacionamentos().ToListAsync();
    }

    public async Task RemoverImagens(List<Imagem> imagens)
    {
        _db.Imagens.RemoveRange(imagens);
        await _db.SaveChangesAsync();
    }

    public async Task AdicionarImagem(Imagem imagem)
    {
        if (imagem.Id == Guid.Empty)
            imagem.Id = Guid.NewGuid();

        await _db.Imagens.AddAsync(imagem);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverImagem(Imagem imagem)
    {
        _db.Imagens.Remove(imagem);
        await _db.SaveChangesAsync();
    }

    private IQueryable<Produto> QueryComRelacionamentos()
    {
        return _db.Produtos
            .Include(p => p.Imagens)
            .Include(p => p.InformacoesProduto);
    }

    private static async Task<PaginacaoResultado<Produto>> Paginar(
        IQueryable<Produto> query,
        int pagina,
        int tamanhoPagina)
    {
        if (pagina < 1)
            pagina = 1;
        if (tamanhoPagina < 1)
            tamanhoPagina = 10;

        var total = await query.CountAsync();
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PaginacaoResultado<Produto>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = total,
            TotalPaginas = total == 0 ? 0 : (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }
}
