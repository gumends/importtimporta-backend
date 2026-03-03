using System.Security.Claims;
using Application.Interfaces.Services;
using Domain.Models.Endereco;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
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
        var endereco = await _enderecoService.BuscaEndereco(id);

        if (endereco == null)
            return NotFound("Endereco não encontrado");

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
        var novoEndereco = await _enderecoService.AtualizarEndereco(enderecoRequest, id);
        return Ok(novoEndereco);
    }

    [HttpDelete]
    public async Task<IActionResult> ExcluirEndereco([FromQuery] Guid id)
    {
        var endereco = await _enderecoService.ExcluirEndereco(id);
        return Ok(endereco);
    }
}