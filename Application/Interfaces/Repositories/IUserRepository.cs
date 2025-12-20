using Domain.DTO;
using Domain.Entities;
using Domain.Models.Imagem;
using Domain.Models.UserMenu;

namespace Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User?> FindUser(string email);
        Task SalvarImagens(IEnumerable<Imagem> imagens);
        Task<User> UpdateUser(User userUp, int id);
        Task<PaginacaoResultado<User>> GetAllUsersPaginado(int pagina, int tamanhoPagina);
        Task ToggleAcessoUser(int id);
        Task ToggleStatusUser(int id);
        Task<List<UserMenu>> GetUsersMenu(string email);
    }
}
