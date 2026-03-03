using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Enuns;

namespace Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Usuario usuario)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            return GenerateJwt(usuario.Id, usuario.Name, usuario.Email, usuario.Role);
        }

        public string GenerateJwt(Guid id, string name, string email, Roles role)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(48),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                ),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(tokenDescriptor));
        }

        public JwtUserData ExtraiJwt(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return new JwtUserData();

            if (token.StartsWith("Bearer "))
                token = token.Substring(7);

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return new JwtUserData();

            var jwt = handler.ReadJwtToken(token);
            
            var nameId = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            
            return new JwtUserData
            {
                Id = Guid.TryParse(nameId, out var id) ? id : Guid.Empty,
                Name = name ?? string.Empty,
                Email = email ?? string.Empty
            };
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            if (token.StartsWith("Bearer "))
                token = token.Substring(7);

            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // evita tolerância de tempo
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}