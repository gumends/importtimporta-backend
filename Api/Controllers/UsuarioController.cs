using System.Security.Claims;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Entities;
using Domain.Models.Endereco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsuarioController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("valid")]
        public async Task<IActionResult> ValidaUsuario([FromQuery] string email)
        {
            var user = await _userService.ValidUser(email);
            if (user is not null)
            {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] UsuarioDTO usuario)
        {
            var token = await _userService.CriarUsuario(usuario.Name, usuario.Email, usuario.Nascimento, usuario.Senha, usuario.Acesso);
            return Ok(new { Token = token });
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> AtualizarUsuario([FromBody] UsuarioDTO dto, [FromQuery] Guid id)
        {
            Usuario usuario = new Usuario(dto.Name, dto.Email, dto.Senha, dto.Nascimento, dto.Acesso);
            var usuarioAtualizado = await _userService.AtualizarUsuario(usuario, id);

            return Ok(usuarioAtualizado);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> BuscaUsuario([FromQuery] string email)
        {
            var usuario = await _userService.BuscaUsuario(email);
            return Ok(usuario);
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BuscaUsuarios([FromQuery] int pagina, int tamanhoPagina)
        {
            var usuario = await _userService.ListaUsuarios(pagina, tamanhoPagina);
            return Ok(usuario);
        }

        [HttpPut("toggle_status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatusUsuario([FromQuery] Guid id)
        {
            await _userService.ToggleStatusUser(id);
            return Ok(); 
        }

        [HttpPut("toggle_acesso")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TogglePermissao([FromQuery] Guid id)
        {
            await _userService.TogglePermissaoUsuario(id);
            return Ok();
        }

        [HttpGet("menus")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetMenus([FromQuery] string email)
        {
            var menus = await _userService.GetUserMenus(email);
            return Ok(menus);
        }
        
        [HttpPost("esqueci_senha")]
        public async Task<IActionResult> EsqueciMinhaSenha([FromBody] EsqueciSenhaDto email)
        {
             var res = await _userService.EsqueciMinhaSenha(email);
             Console.WriteLine(res);
             return Ok(res);
        }
        
        [HttpPut("alterar_senha")]
        public async Task<IActionResult> AlterarSenhaUsuario([FromBody] AlterarSenhaDTO alterarSenhaDTO)
        {
            if (string.IsNullOrEmpty(alterarSenhaDTO.token))
                return Unauthorized("Token não informado");

            var res = await _userService.AlterarSenhaUsuario(alterarSenhaDTO.token, alterarSenhaDTO.senha);
            return Ok(res);
        }

        [HttpPost("imagens")]
        public async Task<IActionResult> InserirImagens()
        {
            throw new NotImplementedException();
        }
    }
}
