using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infraestrutura.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _db;

        public ProdutoRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Produto> AdicionaProduto(Produto produto)
        {
            ArgumentNullException.ThrowIfNull(produto);
            produto.CalcularValores();
            _db.Produtos.Add(produto);
            await _db.SaveChangesAsync();
            return produto;
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosPaginado(int pagina, int tamanhoPagina)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;

            var query = _db.Produtos
                .Include(p => p.InformacoesAdicionais)
                .Include(p => p.Imagens)
                .AsQueryable();

            int totalItens = await query.CountAsync();

            var itens = await query
                .OrderBy(p => p.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new PaginacaoResultado<Produto>
            {
                Itens = itens,
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalItens = totalItens,
                TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
            };
        }

        public async Task<Produto> AtualizaProduto(Produto produto)
        {
            ArgumentNullException.ThrowIfNull(produto);
            produto.CalcularValores();
            _db.Produtos.Update(produto);
            await _db.SaveChangesAsync();
            return produto;
        }

        public async Task RemoverImagens(List<Imagem> imagens)
        {
            _db.Imagens.RemoveRange(imagens);
            await _db.SaveChangesAsync();
        }
        public async Task RemoverImagem(Imagem imagem)
        {
            _db.Imagens.Remove(imagem);
            await _db.SaveChangesAsync();
        }

        public async Task AdicionarImagem(Imagem imagem)
        {
            await _db.Imagens.AddAsync(imagem);
            await _db.SaveChangesAsync();
        }

        public async Task<Produto?> ObterProdutoPorId(int id)
        {
            var produto = await _db.Produtos
                .Include(p => p.InformacoesAdicionais)
                .Include(p => p.Imagens)
                .FirstOrDefaultAsync(p => p.Id == id);

            return produto;
        }

        public async Task<bool> RemoveProduto(int id)
        {
            var produto = await _db.Produtos.FindAsync(id);
            if (produto == null)
            {
                return false;
            }
            _db.Produtos.Remove(produto);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;
            var query = _db.Produtos
                .Include(p => p.InformacoesAdicionais)
                .Include(p => p.Imagens)
                .Where(p => p.TipoProduto == tipoProduto)
                .AsQueryable();

            int totalItens = await query.CountAsync();

            var itens = await query
                .Where(p => p.Disponivel == true)
                .OrderBy(p => p.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new PaginacaoResultado<Produto>
            {
                Itens = itens,
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalItens = totalItens,
                TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
            };
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosEmPromocao(int pagina, int tamanhoPagina)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;

            var query = _db.Produtos
                .Include(p => p.InformacoesAdicionais)
                .Include(p => p.Imagens)
                .Where(p => p.Desconto > 0)
                .AsQueryable();

            int totalItens = await query.CountAsync();

            var itens = await query
                .Where(p => p.Disponivel == true)
                .OrderBy(p => p.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new PaginacaoResultado<Produto>
            {
                Itens = itens,
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalItens = totalItens,
                TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
            };
        }
        public async Task SalvarImagens(IEnumerable<Imagem> imagens)
        {
            _db.Imagens.AddRange(imagens);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Produto>> ListaProdutos()
        {
            var query = _db.Produtos
                .Include(p => p.InformacoesAdicionais)
                .Include(p => p.Imagens)
                .AsQueryable();

            return await query.ToListAsync();
        }
    }
}
