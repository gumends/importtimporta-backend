using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return GenerateJwt(user.Id.ToString(), user.Name, user.Email, user.Role);
        }

        public string GenerateJwt(string id, string name, string email, Roles role)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role.ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(2),
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
            var handler = new JwtSecurityTokenHandler();

            if (string.IsNullOrEmpty(token))
                return new JwtUserData();

            if (token.StartsWith("Bearer "))
                token = token.Substring(7);

            var jwt = handler.ReadJwtToken(token);

            return new JwtUserData
            {
                Id = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value ?? "",
                Name = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? "",
                Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? ""
            };
        }
    }
}