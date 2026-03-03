using Domain.DTO;
using Domain.Entities;
using Domain.Enuns;
using Domain.Models.Topics;
using Domain.Models.UserMenu;

namespace Application.Interfaces.Services
{
    public interface 
        IUserService
    {
        Task<Usuario> CriarUsuario(string name, string email, DateOnly nascimento, string senha, TipoAcesso tipoAcesso);
        Task<Usuario?> ValidUser(string email);
        Task<string> CriarUsuarioELogin(string nome, string email, DateOnly nascimento, string senha);
        Task<Usuario> AtualizarUsuario(Usuario usuario, Guid id);
        Task<Usuario> BuscaUsuario(string email);
        Task<PaginacaoResultado<Usuario>> ListaUsuarios(int pagina, int tamanhoPagina);
        Task ToggleStatusUser(Guid id);
        Task TogglePermissaoUsuario(Guid id);
        Task<List<UserMenu>> GetUserMenus(string email);
        Task<EsqueciMinhaSenhaResponse> EsqueciMinhaSenha(EsqueciSenhaDto email);
        Task<bool> AlterarSenhaUsuario(string token, string senha);
    }
}
