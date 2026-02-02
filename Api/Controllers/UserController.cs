using System.Security.Claims;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Endereco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("usuario")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
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
        public async Task<IActionResult> CriarUsuario([FromBody] User user)
        {
            var token = await _userService.CreateUserLogin(user.Name, user.Email, user.Senha, user.Role);

            if (token == null)
                return BadRequest();

            return Ok(new { Token = token });
        }

        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> AtualizarUsuario([FromBody] User user, [FromQuery] int id)
        {
            var newUser = await _userService.AtualizarProduto(user, id);

            return Ok(newUser);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> BuscaUsuario([FromQuery] string email)
        {
            var user = await _userService.BuscaUsuario(email);
            return Ok(user);
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BuscaUsuarios([FromQuery] int pagina, int tamanhoPagina)
        {
            var user = await _userService.ListaUsuarios(pagina, tamanhoPagina);
            return Ok(user);
        }

        [HttpPut("toggle_status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatusUsuario([FromQuery] int id)
        {
            await _userService.ToggleStatusUser(id);
            return Ok(); 
        }

        [HttpPut("toggle_acesso")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleStatusAcesso([FromQuery] int id)
        {
            await _userService.ToggleAcessoUser(id);
            return Ok();
        }

        [HttpGet("menus")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetMenus([FromQuery] string email)
        {
            var menus = await _userService.GetMenus(email);
            return Ok(menus);
        }
        
        [HttpPost("endereco")]
        public async Task<IActionResult> CadastrarEndereco([FromBody] EnderecoRequest enderecoRequest)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var enderecoRespose = await _userService.CadastrarEndereco(enderecoRequest, usuarioId);
            
            return Ok(enderecoRespose);
        }

        [HttpGet("endereco")]
        public async Task<IActionResult> BuscaEndereco([FromQuery] int id)
        {
            var endereco = await _userService.BuscaEndereco(id);
            return Ok(endereco);
        }

        [HttpGet("enderecos")]
        public async Task<IActionResult> BuscaEnderecos()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var endereco = await _userService.BuscaEnderecos(usuarioId);
            return Ok(endereco);
        }

        [HttpPut("endereco")]
        public async Task<IActionResult> AtualizarEndereco([FromBody] EnderecoRequest enderecoRequest,[FromQuery] int id)
        {
            var novoEndereco = await _userService.AtualizarEndereco(enderecoRequest, id);
            return Ok(novoEndereco);
        }

        [HttpDelete("endereco")]
        public async Task<IActionResult> ExcluirEndereco([FromQuery] int id)
        {
            var endereco = await _userService.ExcluirEndereco(id);
            return Ok(endereco);
        }
    }
}
