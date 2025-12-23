using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Entities;
using Domain.Models.Produto;
using Domain.Models.UserMenu;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository repo, IJwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        public async Task<User> CreateUser(string name, string email, string senha, Roles role, TipoAcesso tipoAcesso)
        {
            var user = await _repo.FindUser(email);

            if (user != null)
            {
                throw new BadRequestException("Usuario já cadastrado.");
            }

            string senhaHasheada = BCrypt.Net.BCrypt.HashPassword(senha);

            if (senha == string.Empty)
            {
                senhaHasheada = string.Empty;
            }

            var newUser = new User
            {
                Name = name,
                Email = email,
                Senha = senhaHasheada,
                Role = role,
                Acesso = tipoAcesso
            };

            return await _repo.AddAsync(newUser);
        }

        public async Task<string> CreateUserLogin(string name, string email, string senha, Roles role)
        {
            var res = await CreateUser(name, email, senha, role, TipoAcesso.Padrao);
            var jwt = _jwtService.GenerateJwt(res.Id.ToString(), res.Name, res.Email, Roles.User);

            if (string.IsNullOrEmpty(jwt))
            {
                throw new Exception();
            }

            return jwt;
        }

        public async Task<User?> ValidUser(string email)
        {
            return await _repo.FindUser(email);
        }

        public async Task<User> AtualizarProduto(User user, int id)
        {
            return await _repo.UpdateUser(user, id);
        }

        public async Task<User?> BuscaUsuario(string email)
        {
            return await _repo.FindUser(email);
        }
        public async Task<PaginacaoResultado<User>> ListaUsuarios(int pagina, int tamanhoPagina)
        {
            return await _repo.GetAllUsersPaginado(pagina, tamanhoPagina);
        }

        public async Task ToggleAcessoUser(int id)
        {
            await _repo.ToggleAcessoUser(id);
        }

        public async Task ToggleStatusUser(int id)
        {
            await _repo.ToggleStatusUser(id);
        }

        public async Task<List<UserMenu>> GetMenus(string email)
        {
           return await _repo.GetUsersMenu(email);
        }
    }
}