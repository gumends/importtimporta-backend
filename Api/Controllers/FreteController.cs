using Application.Interfaces.Services;
using Domain.Models.Frete;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("frete")]
public class FreteController : ControllerBase
{
    private readonly IFreteService _freteService;

    public FreteController(IFreteService freteService)
    {
        _freteService = freteService;
    }

    /// <summary>Cota PAC/SEDEX com base no CEP e itens do carrinho (Correios via Melhor Envio ou estimativa).</summary>
    [HttpPost("cotar")]
    public async Task<ActionResult<CotarFreteResponse>> Cotar(
        [FromBody] CotarFreteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _freteService.CotarAsync(request, cancellationToken);
        return Ok(result);
    }
}
