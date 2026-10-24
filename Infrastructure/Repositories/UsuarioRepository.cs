using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Entities;
using Domain.Models.Imagem;
using Domain.Models.UserMenu;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;

    public UsuarioRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario> CriarUsuario(Usuario usuario)
    {
        await _db.Users.AddAsync(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario?> BuscarUsuarioPorEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> BuscarUsuarioPorId(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task SalvarImagens(IEnumerable<Imagem> imagens)
    {
        await _db.Imagens.AddRangeAsync(imagens);
        await _db.SaveChangesAsync();
    }

    public async Task<Usuario> AtualizarUsuario(Usuario usuario)
    {
        _db.Users.Update(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task<PaginacaoResultado<Usuario>> BuscarUsuariosPaginado(int pagina, int tamanhoPagina)
    {
        if (pagina < 1)
            pagina = 1;
        if (tamanhoPagina < 1)
            tamanhoPagina = 10;

        var query = _db.Users.OrderBy(u => u.Name);
        var total = await query.CountAsync();
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PaginacaoResultado<Usuario>
        {
            Itens = itens,
            PaginaAtual = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = total,
            TotalPaginas = total == 0 ? 0 : (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<List<UserMenu>> BuscarMenuUsuario(string email)
    {
        var usuario = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (usuario == null)
            return new List<UserMenu>();

        return await _db.Menus
            .Where(m => m.Role == usuario.Role)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }
}
