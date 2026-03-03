using Domain.Entities;
using Domain.Models.Auth;

namespace Application.Interfaces.Services
{
    public interface IAuthServices
    {
        string AuthGoogle(string state);
        Task<Callback> GoogleCallback(string code);
        Task<Usuario?> ValidMe(string token);

        Task<string> Login(Login login);
    }
}
