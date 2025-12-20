using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthServices _authServices;

    public AuthController(IAuthServices authServices)
    {
        _authServices = authServices;
    }

    [HttpGet("google-url")]
    public IActionResult GetGoogleUrl([FromQuery] string state)
    {
        var url = _authServices.AuthGoogle(state);
        return Ok(new { auth = url });
    }

    [HttpGet("callback/google")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
    {
        var result = await _authServices.GoogleCallback(code);

        Response.Cookies.Append("auth_token", result.Token, new CookieOptions
        {
            Domain = "localhost",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTime.UtcNow.AddHours(2)
        });

        return Redirect(state);
    }

    [HttpPost("me")]
    public async Task<IActionResult> Me([FromBody] MeResponse meResponse)
    {
        var token = Request.Cookies["auth_token"];
        var sessionToken = meResponse?.SessionToken;

        if (string.IsNullOrEmpty(token))
            token = sessionToken;

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var user = await _authServices.ValidMe(token);

        if (user != null)
        {
            return Ok(user);
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        return Ok(new { message = "Logout realizado" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login login)
    {
        var token = await _authServices.Login(login);

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Expires = DateTime.UtcNow.AddHours(2)
        });


        return Ok(new { Token = token });
    }
}
