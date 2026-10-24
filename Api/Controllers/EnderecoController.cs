using System.Security.Claims;
using Application.Interfaces.Services;
using Domain.Models.Endereco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("endereco")]
public class EnderecoController : ControllerBase
{
    private readonly IEnderecoService _enderecoService;

    public EnderecoController(IEnderecoService enderecoService)
    {
        _enderecoService = enderecoService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CadastrarEndereco([FromBody] EnderecoRequest enderecoRequest)
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var enderecoRespose = await _enderecoService.CadastrarEndereco(enderecoRequest, usuarioId);
            
        return Ok(enderecoRespose);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var endereco = await _enderecoService.BuscaEndereco(id);

        if (endereco.IdUsuario != usuarioId)
            return Forbid();

        return Ok(endereco);
    }

    [HttpGet]
    public async Task<IActionResult> BuscaEnderecos()
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var endereco = await _enderecoService.BuscaEnderecos(usuarioId);
        return Ok(endereco);
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarEndereco([FromBody] EnderecoRequest enderecoRequest, [FromQuery] Guid id)
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var enderecoAtual = await _enderecoService.BuscaEndereco(id);

        if (enderecoAtual.IdUsuario != usuarioId)
            return Forbid();

        var novoEndereco = await _enderecoService.AtualizarEndereco(enderecoRequest, id);
        return Ok(novoEndereco);
    }

    [HttpDelete]
    public async Task<IActionResult> ExcluirEndereco([FromQuery] Guid id)
    {
        var usuarioId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var enderecoAtual = await _enderecoService.BuscaEndereco(id);

        if (enderecoAtual.IdUsuario != usuarioId)
            return Forbid();

        var endereco = await _enderecoService.ExcluirEndereco(id);
        return Ok(endereco);
    }
}