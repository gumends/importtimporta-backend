using Application.Interfaces.Repositories;
using Domain.DTO;
using Domain.Entities;
using Domain.Models.Imagem;
using Domain.Models.UserMenu;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> FindUser(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User> UpdateUser(User userUp, int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            user.Name = userUp.Name;
            user.Email = userUp.Email;
            user.Role = userUp.Role;
            user.Senha = BCrypt.Net.BCrypt.HashPassword(userUp.Senha);
            user.Nascimento = userUp.Nascimento;

            _db.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task SalvarImagens(IEnumerable<Imagem> imagens)
        {
            _db.Imagens.AddRange(imagens);
            await _db.SaveChangesAsync();
        }

        public async Task<PaginacaoResultado<User>> GetAllUsersPaginado(int pagina, int tamanhoPagina)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0) tamanhoPagina = 10;

            var query = _db.Users
                .AsQueryable();

            int totalItens = await query.CountAsync();

            var usuarios = await query
                .OrderBy(p => p.Id)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return new PaginacaoResultado<User>
            {
                Itens = usuarios,
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalItens = totalItens,
                TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
            };
        }

        public async Task ToggleAcessoUser(int id)
        {
            var user = await _db.Users.FindAsync(id)
                ?? throw new Exception("Usuário não encontrado");

            user.Role = user.Role == Roles.Admin
                ? Roles.User
                : Roles.Admin;

            await _db.SaveChangesAsync();
        }

        public async Task ToggleStatusUser(int id)
        {
            var user = await _db.Users.FindAsync(id)
                ?? throw new Exception("Usuário não encontrado");

            user.Status = !user.Status;

            await _db.SaveChangesAsync();
        }

        public async Task<List<UserMenu>> GetUsersMenu(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new BadRequestException("Usuário não encontrado");
            }

            var menus = await _db.Menus
                .ToListAsync();

            if (user.Role == Roles.User)
                menus = menus.Where(menu => menu.Role == Roles.User).ToList();

            if (menus.Count < 0)
            {
                throw new BadRequestException("Menus não encontrados para o usuário");
            }

            return menus;
        }
    }
}