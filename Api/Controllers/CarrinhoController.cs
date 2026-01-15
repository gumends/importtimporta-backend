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
    public async Task<IActionResult> PostCarrinho([FromBody] CarrinhoRequest carrinho)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _carrinhoService.PostCarrinho(carrinho, usuarioId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ListarCarrinho()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _carrinhoService.Carrinho(usuarioId);
        return Ok(result);
    }
    
    [HttpDelete("{carrinhoId}")]
    public async Task<IActionResult> ExcluirItemCarrinho([FromRoute] int carrinhoId)
    {
        var result = await _carrinhoService.ExcluirItemCarrinho(carrinhoId);
        return Ok(result);
    }
}