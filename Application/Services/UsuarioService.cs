using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Entities;
using Domain.Models.Topics;
using Domain.Models.UserMenu;
using System.Text.Json;
using Domain;
using Domain.Enuns;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UsuarioService : IUserService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;
        private readonly IQueueService _queueService;
        private readonly string _baseUrlSenha;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUsuarioRepository usuarioRepository, IJwtService jwtService, IQueueService queueService,  IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
            _queueService = queueService;
            _configuration = configuration;
            _baseUrlSenha = _configuration["BaseUrlSenha"] ?? "";
        }

        public async Task<Usuario> CriarUsuario(string name, string email, DateOnly nascimento, string senha, TipoAcesso tipoAcesso)
        {
            var validaUsuario = await _usuarioRepository.BuscarUsuarioPorEmail(email);

            if (validaUsuario != null)
            {
                throw new BadRequestException("Usuario já cadastrado.");
            }

            string senhaHasheada = BCrypt.Net.BCrypt.HashPassword(senha);

            if (senha == string.Empty)
            {
                senhaHasheada = string.Empty;
            }

            var bodyHtmlEsqueciaMinhaSenha = HtmlBemVindo.Gerar(name);

            var usuario = new Usuario(name, email, senhaHasheada, nascimento, tipoAcesso);
            
            var esqueciMinhaSenha = new EsqueciMinhaSenhaResponse()
            {
                To = usuario.Email,
                Subject = "Solicitação para troca de senha.",
                Body =  bodyHtmlEsqueciaMinhaSenha,
            };
            
            var json = JsonSerializer.Serialize(esqueciMinhaSenha);

            await _queueService.SendMessageAsync(json);

            return await _usuarioRepository.CriarUsuario(usuario);
        }

        public async Task<string> CriarUsuarioELogin(string nome, string email, DateOnly nascimento, string senha)
        {
            var res = await CriarUsuario(nome, email, nascimento, senha, TipoAcesso.Padrao);
            var jwt = _jwtService.GenerateJwt(res.Id, res.Name, res.Email, Roles.User);

            if (string.IsNullOrEmpty(jwt))
            {
                throw new Exception();
            }

            return jwt;
        }

        public async Task<Usuario?> ValidUser(string email)
        {
            return await _usuarioRepository.BuscarUsuarioPorEmail(email);
        }

        public async Task<Usuario> AtualizarUsuario(Usuario dados, Guid id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorId(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            usuario.AtualizarDados(
                dados.Name,
                dados.Email,
                dados.Nascimento
            );

            await _usuarioRepository.AtualizarUsuario(usuario);

            return usuario;
        }

        public async Task<Usuario> BuscaUsuario(string email)
        {
            var user = await _usuarioRepository.BuscarUsuarioPorEmail(email);

            return user ?? throw new NotFoundException("Usuario não encontrado");
        }
        public async Task<PaginacaoResultado<Usuario>> ListaUsuarios(int pagina, int tamanhoPagina)
        {
            return await _usuarioRepository.BuscarUsuariosPaginado(pagina, tamanhoPagina);
        }

        public async Task TogglePermissaoUsuario(Guid id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorId(id);
            if (usuario == null)
                throw new NotFoundException("Usuario não encontrado");

            usuario.AlterarRole(usuario.Role == Roles.Admin ? Roles.User : Roles.Admin);
            
            await _usuarioRepository.AtualizarUsuario(usuario);
        }

        public async Task ToggleStatusUser(Guid id)
        {
            var usuario = await _usuarioRepository.BuscarUsuarioPorId(id);
            if (usuario == null)
                throw new NotFoundException("Usuario não encontrado");

            if (usuario.Status)
            {
                usuario.Desativar();
            }
            else
            {
                usuario.Ativar();
            }
            
            await _usuarioRepository.AtualizarUsuario(usuario);
        }

        public async Task<List<UserMenu>> GetUserMenus(string email)
        {
           return await _usuarioRepository.BuscarMenuUsuario(email);
        }

        public async Task<EsqueciMinhaSenhaResponse> EsqueciMinhaSenha(EsqueciSenhaDto email)
        {
            var user = await _usuarioRepository.BuscarUsuarioPorEmail(email.Email);

            if (user == null)
                throw new NotFoundException("Usuario não encontrado.");

            string token = _jwtService.GenerateToken(user);
            string link = _baseUrlSenha + token;
            
            var bodyHtmlEsqueciaMinhaSenha = HTML.Gerar(link);
            
            var esqueciMinhaSenha = new EsqueciMinhaSenhaResponse()
            {
                To = user.Email,
                Subject = "Solicitação para troca de senha.",
                Body =  bodyHtmlEsqueciaMinhaSenha,
            };
            
            var json = JsonSerializer.Serialize(esqueciMinhaSenha);

            await _queueService.SendMessageAsync(json);
            
            return esqueciMinhaSenha;
        }

        public async Task<bool> AlterarSenhaUsuario(string token, string senha)
        {
            var tokenValido = _jwtService.ValidateToken(token);

            string senhaHasheada = BCrypt.Net.BCrypt.HashPassword(senha);
            
            if (tokenValido)
            {
                var dadosToken = _jwtService.ExtraiJwt(token);

                var usuario = await _usuarioRepository.BuscarUsuarioPorId(dadosToken.Id);
                if (usuario == null)
                    throw new NotFoundException("Usuario não encontrado");

                usuario.AlterarSenha(senhaHasheada);
                
                return true; 
            }
            
            return false;
        }
    }
}