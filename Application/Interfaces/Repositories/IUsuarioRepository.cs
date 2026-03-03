using Domain.DTO;
using Domain.Entities;
using Domain.Models.Endereco;
using Domain.Models.Imagem;
using Domain.Models.UserMenu;

namespace Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> CriarUsuario(Usuario usuario);
        Task<Usuario> BuscarUsuarioPorEmail(string email);
        Task<Usuario> BuscarUsuarioPorId(Guid id);
        Task SalvarImagens(IEnumerable<Imagem> imagens);
        Task<Usuario> AtualizarUsuario(Usuario usuario);
        Task<PaginacaoResultado<Usuario>> BuscarUsuariosPaginado(int pagina, int tamanhoPagina);
        Task<List<UserMenu>> BuscarMenuUsuario(string email);
    }
}
