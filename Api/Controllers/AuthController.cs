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
    public async Task<IActionResult> GoogleCallback(
        [FromQuery] string code,
        [FromQuery] string? state)
    {
        if (string.IsNullOrEmpty(code))
        {
            var googleErr = Request.Query["error"].ToString();
            var msg = string.IsNullOrEmpty(googleErr)
                ? "código do Google não recebido"
                : $"Google cancelou o login ({googleErr})";
            return Content($"<h2>Erro: {msg}</h2>", "text/html");
        }

        Callback result;
        try
        {
            result = await _authServices.GoogleCallback(code);
        }
        catch (BadRequestException ex)
        {
            return GoogleCallbackErrorHtml(ex.Message);
        }
        catch (Exception ex)
        {
            return GoogleCallbackErrorHtml(ex.Message);
        }

        var frontendUrl = GoogleOAuthFrontendOrigin.Resolve(state, _config);
        var safeToken = HttpUtility.JavaScriptStringEncode(result.Token);
        var safeOrigin = HttpUtility.JavaScriptStringEncode(frontendUrl);

        return Content($@"
        <html>
          <body>
            <script>
              if (window.opener) {{
                window.opener.postMessage(
                  {{ token: '{safeToken}' }},
                  '{safeOrigin}'
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

    private static IActionResult GoogleCallbackErrorHtml(string message)
    {
        var safeMsg = HttpUtility.HtmlEncode(message);
        return new ContentResult
        {
            ContentType = "text/html",
            Content =
                $@"<html><body style=""font-family:sans-serif;padding:24px;max-width:520px"">
                <h2>Login com Google</h2><p>{safeMsg}</p>
                <p><small>Feche esta janela e tente novamente.</small></p>
                </body></html>",
        };
    }
}
