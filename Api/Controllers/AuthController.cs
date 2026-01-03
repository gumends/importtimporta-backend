using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServices _authServices;
    private readonly IConfiguration _config;

    public AuthController(
        IAuthServices authServices,
        IConfiguration config
    )
    {
        _authServices = authServices;
        _config = config;
    }

    [HttpGet("google-url")]
    public IActionResult GetGoogleUrl([FromQuery] string state)
    {
        var url = _authServices.AuthGoogle(state);
        return Ok(new { auth = url });
    }

    [HttpGet("callback/google")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return Content("<h2>Erro: código do Google não recebido</h2>", "text/html");
        }

        var result = await _authServices.GoogleCallback(code);

        var frontendUrl = _config["FrontendUrl"];
        var safeToken = HttpUtility.JavaScriptStringEncode(result.Token);

        return Content($@"
        <html>
          <body>
            <script>
              if (window.opener) {{
                window.opener.postMessage(
                  {{ token: '{safeToken}' }},
                  '{frontendUrl}'
                );
                window.close();
              }} else {{
                document.body.innerHTML = 'Erro: opener não encontrado';
              }}
            </script>
          </body>
        </html>
        ", "text/html");
    }

    [HttpPost("me")]
    public async Task<IActionResult> Me([FromBody] MeResponse meResponse)
    {
        if (string.IsNullOrEmpty(meResponse?.SessionToken))
            return Unauthorized();

        var user = await _authServices.ValidMe(meResponse.SessionToken);

        if (user == null)
            return Unauthorized();

        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout realizado" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var token = await _authServices.Login(login);

        if (string.IsNullOrEmpty(token))
            return Unauthorized();
        return Ok(new { token });
    }
}
