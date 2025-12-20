using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using static Domain.Models.Auth.GoogleExchangeService;

namespace Application.Uteis
{
    public class JWT
    {
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
    }
}
