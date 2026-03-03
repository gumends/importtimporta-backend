using System.Security.Claims;
using Application.Interfaces.Services;
using Domain.Models.carrinho;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("carrinho")]
public class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoService _carrinhoService;

    public CarrinhoController(ICarrinhoService carrinhoService)
    {
        _carrinhoService = carrinhoService;
    }

    [HttpPost]
    public async Task<IActionResult> PostCarrinho([FromBody] CarrinhoDto carrinho)
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _carrinhoService.PostCarrinho(carrinho, usuarioId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ListarCarrinho()
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _carrinhoService.Carrinho(usuarioId);
        return Ok(result);
    }
    
    [HttpDelete("{carrinhoId}")]
    public async Task<IActionResult> ExcluirItemCarrinho([FromRoute] Guid carrinhoId)
    {
        var result = await _carrinhoService.ExcluirItemCarrinho(carrinhoId);
        return Ok(result);
    }
}