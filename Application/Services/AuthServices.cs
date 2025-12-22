using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Auth;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using static Domain.Models.Auth.GoogleExchangeService;

namespace Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userServices;
        private readonly IJwtService _jwtService;

        public AuthServices(IConfiguration config, IUserService userService, IJwtService jwtService)
        {
            _config = config;
            _userServices = userService;
            _jwtService = jwtService;
        }

        public string AuthGoogle(string state)
        {
            string clientId = _config["Authentication:Google:ClientId"]!;
            string redirectUri = $"{_config["AppBaseUrl"]}/auth/callback/google";

            string url =
                "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={clientId}" +
                $"&redirect_uri={redirectUri}" +
                "&response_type=code" +
                "&scope=openid%20email%20profile" +
                "&access_type=offline" +
                "&prompt=consent" +
                $"&state={Uri.EscapeDataString(state)}";

            return url;
        }

        public async Task<Callback> GoogleCallback(string code)
        {
            var payload = await ExchangeCodeForTokens(code, _config);

            await ValidaUsuario(payload.Email, payload.Name, TipoAcesso.Google);

            var jwt = _jwtService.GenerateJwt(payload.Id, payload.Name, payload.Email, Roles.User);

            return new Callback
            {
                Token = jwt,
                User = payload
            };
        }

        public async Task ValidaUsuario(string email, string name, TipoAcesso tipoAcesso)
        {
            var user = await _userServices.ValidUser(email);
            if (user is null)
            {
                await _userServices.CreateUser(name, email, string.Empty, Roles.User, tipoAcesso);
            }
        }

        public async Task<User?> ValidMe(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var jwtData = _jwtService.ExtraiJwt(token);
            var user = await _userServices.ValidUser(jwtData.Email);

            return user;
        }

        public static async Task<GooglePayload> ExchangeCodeForTokens(string code, IConfiguration config)
        {
            using var http = new HttpClient();

            string clientId = config["Authentication:Google:ClientId"]!;
            string clientSecret = config["Authentication:Google:ClientSecret"]!;
            string redirectUri = $"{config["AppBaseUrl"]}/auth/callback/google";

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            });

            var tokenResponse = await http.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
            var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();

            if (!tokenJson.TryGetProperty("access_token", out var accessTokenProp))
                throw new Exception("Google não retornou access_token.");

            string accessToken = accessTokenProp.GetString()!;

            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var userInfo = await http.GetFromJsonAsync<JsonElement>(
                "https://www.googleapis.com/oauth2/v2/userinfo"
            );

            return new GooglePayload
            {
                Id = userInfo.GetProperty("id").GetString()!,
                Name = userInfo.GetProperty("name").GetString()!,
                Email = userInfo.GetProperty("email").GetString()!
            };
        }

        public async Task<string> Login(Login login)
        {
            if (login.Senha == string.Empty)
                throw new BadRequestException("Senha não pode ser vazia");
            if (login.Email == string.Empty)
                throw new BadRequestException("Email não pode ser vazio");

            var user = await _userServices.ValidUser(login.Email);

            if (user is null)
            {
                throw new NotFoundException("Usuário");
            }

            if (user.Acesso != TipoAcesso.Padrao)
            {
                var tipoConta = user.Acesso == TipoAcesso.Apple ? "Apple" : "Google";

                throw new BadRequestException(
                    $"Este e-mail já está vinculado a uma conta {tipoConta}. Utilize esse método de login."
                );
            }

            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(login.Senha, user.Senha);

            if (!senhaCorreta)
            {
                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            var jwt = _jwtService.GenerateJwt(user.Id.ToString(), user.Name, user.Email, user.Role);

            return jwt;
        }

    }
}
