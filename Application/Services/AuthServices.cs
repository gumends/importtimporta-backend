using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Models.Auth;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Enuns;
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
            string clientId = _config["Authentication:Google:ClientId"]?.Trim() ?? "";
            string clientSecret = _config["Authentication:Google:ClientSecret"]?.Trim() ?? "";

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new BadRequestException(
                    "Login com Google não está configurado na API. " +
                    "Defina Authentication:Google:ClientId e ClientSecret " +
                    "(appsettings ou user-secrets) e cadastre o redirect URI no Google Cloud."
                );
            }

            if (clientSecret.Contains("COLE_AQUI", StringComparison.OrdinalIgnoreCase)
                || clientSecret.StartsWith("GOOGLE_", StringComparison.OrdinalIgnoreCase))
            {
                throw new BadRequestException(
                    "Substitua o placeholder do ClientSecret no appsettings.Development.json " +
                    "pelo segredo real (GOCSPX-...) copiado do Google Cloud Console."
                );
            }

            string appBase = _config["AppBaseUrl"]?.Trim().TrimEnd('/') ?? "";
            if (string.IsNullOrEmpty(appBase))
            {
                throw new BadRequestException("AppBaseUrl não configurado na API.");
            }

            string redirectUri = $"{appBase}/auth/callback/google";
            string safeState = string.IsNullOrWhiteSpace(state)
                ? _config["FrontendUrl"] ?? "http://localhost:3000"
                : state;

            string url =
                "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={Uri.EscapeDataString(clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                "&response_type=code" +
                "&scope=openid%20email%20profile" +
                "&access_type=offline" +
                "&prompt=select_account" +
                $"&state={Uri.EscapeDataString(safeState)}";

            return url;
        }

        public async Task<Callback> GoogleCallback(string code)
        {
            var payload = await ExchangeCodeForTokens(code, _config);

            await ValidaUsuario(payload.Email, payload.Name, TipoAcesso.Google);

            var user = await _userServices.ValidUser(payload.Email);
            if (user is null)
                throw new NotFoundException("Usuário");

            var jwt = _jwtService.GenerateJwt(user.Id, user.Name, user.Email, user.Role);

            return new Callback
            {
                Token = jwt,
                User = payload
            };
        }

        public async Task ValidaUsuario(string email, string nome, TipoAcesso tipoAcesso)
        {
            var validaUsuario = await _userServices.ValidUser(email);
            if (validaUsuario is null)
            {
                var usuario = new Usuario(nome, email, "", new DateOnly(), tipoAcesso);
                await _userServices.CriarUsuario(usuario.Name,  usuario.Email, usuario.Nascimento, usuario.Senha, usuario.Acesso);
            }
        }
        
        public async Task<Usuario?> ValidMe(string token)
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

            string clientId = config["Authentication:Google:ClientId"]?.Trim() ?? "";
            string clientSecret = config["Authentication:Google:ClientSecret"]?.Trim() ?? "";
            string appBase = config["AppBaseUrl"]?.Trim().TrimEnd('/') ?? "";
            string redirectUri = $"{appBase}/auth/callback/google";

            var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            });

            var tokenResponse = await http.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);
            var rawBody = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenJson = JsonDocument.Parse(
                string.IsNullOrWhiteSpace(rawBody) ? "{}" : rawBody);
            var root = tokenJson.RootElement;

            if (!root.TryGetProperty("access_token", out var accessTokenProp))
            {
                var googleError = root.TryGetProperty("error", out var err)
                    ? err.GetString()
                    : null;
                var googleDesc = root.TryGetProperty("error_description", out var desc)
                    ? desc.GetString()
                    : null;

                var detail = !string.IsNullOrWhiteSpace(googleDesc)
                    ? googleDesc
                    : googleError ?? "resposta inesperada do Google";

                if (googleError == "invalid_client")
                {
                    throw new BadRequestException(
                        "Client Secret do Google inválido. No Google Cloud, abra o cliente OAuth, " +
                        "copie de novo o segredo (ou gere um novo) e atualize Authentication:Google:ClientSecret na API."
                    );
                }

                if (googleError == "redirect_uri_mismatch")
                {
                    throw new BadRequestException(
                        $"Redirect URI não confere. A API usa: {redirectUri}. " +
                        "Cadastre exatamente esse URI em URIs de redirecionamento no Google Cloud."
                    );
                }

                throw new BadRequestException($"Google OAuth falhou: {detail}");
            }

            string accessToken = accessTokenProp.GetString()!;

            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var userInfo = await http.GetFromJsonAsync<JsonElement>(
                "https://www.googleapis.com/oauth2/v2/userinfo"
            );

            var idString = userInfo.GetProperty("id").GetString();

            return new GooglePayload
            {
                Id = Guid.TryParse(idString, out var parsedId) ? parsedId : Guid.Empty,
                Name = userInfo.GetProperty("name").GetString() ?? string.Empty,
                Email = userInfo.GetProperty("email").GetString() ?? string.Empty
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

            var jwt = _jwtService.GenerateJwt(user.Id, user.Name, user.Email, user.Role);

            return jwt;
        }

    }
}
