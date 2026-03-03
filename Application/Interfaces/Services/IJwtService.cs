using Domain.Entities;
using Domain.Enuns;
using Domain.Models.Jwt;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
        string GenerateJwt(Guid id, string name, string email, Roles role);
        JwtUserData ExtraiJwt(string token);
        bool ValidateToken(string token);
    }
}