using System.Security.Claims;
using Application.Interfaces.Repositories;
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
    private readonly ICarrinhoRepository _carrinhoRepository;

    public CarrinhoController(ICarrinhoService carrinhoService, ICarrinhoRepository carrinhoRepository)
    {
        _carrinhoService = carrinhoService;
        _carrinhoRepository = carrinhoRepository;
    }

    [Authorize]
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
    
    [Authorize]
    [HttpDelete("{carrinhoId}")]
    public async Task<IActionResult> ExcluirItemCarrinho([FromRoute] Guid carrinhoId)
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var item = await _carrinhoRepository.ObterPorId(carrinhoId);

        if (item == null)
            return NotFound();

        if (item.IdUsuario != usuarioId)
            return Forbid();

        var result = await _carrinhoService.ExcluirItemCarrinho(carrinhoId);
        return Ok(result);
    }
}