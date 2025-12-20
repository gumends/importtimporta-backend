using Domain.DTO;
using Domain.Entities;
using Domain.Models.UserMenu;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(string name, string email, string senha, Roles role, TipoAcesso tipoAcesso);
        Task<User?> ValidUser(string email);
        Task<string> CreateUserLogin(string name, string email, string senha, Roles role);
        Task<User> AtualizarProduto(User user, int id);
        Task<User> BuscaUsuario(string email);
        Task<PaginacaoResultado<User>> ListaUsuarios(int pagina, int tamanhoPagina);
        Task ToggleStatusUser(int id);
        Task ToggleAcessoUser(int id);
        Task<List<UserMenu>> GetMenus(string email);
    }
}
