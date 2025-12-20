using Domain.Entities;
using Domain.Models.Jwt;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateJwt(string id, string name, string email, Roles role);
        JwtUserData ExtraiJwt(string token);
    }
}