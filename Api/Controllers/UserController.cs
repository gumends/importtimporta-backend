using Application.Interfaces.Services;
using Domain.Entities;
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
        public async Task<IActionResult> CriarUsuario([FromBody] User user, [FromQuery] string state)
        {
            var token = await _userService.CreateUserLogin(user.Name, user.Email, user.Senha, user.Role);

            if (token == null)
                return BadRequest();

            return Ok(new { redirect = state, Token = token });
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AtualizarUsuario([FromBody] User user, [FromQuery] int id)
        {
            var newUser = await _userService.AtualizarProduto(user, id);

            return Ok(newUser);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenus([FromQuery] string email)
        {
            var menus = await _userService.GetMenus(email);
            return Ok(menus);
        }
    }
}
