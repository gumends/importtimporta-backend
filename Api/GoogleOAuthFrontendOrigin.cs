namespace Api;

internal static class GoogleOAuthFrontendOrigin
{
    public static string Resolve(string? state, IConfiguration config)
    {
        var fallback = config["FrontendUrl"]?.Trim().TrimEnd('/')
            ?? "http://localhost:3000";

        if (string.IsNullOrWhiteSpace(state))
            return fallback;

        var candidate = state.Trim().TrimEnd('/');
        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
            return fallback;

        if (IsLocalDevOrigin(uri))
            return $"{uri.Scheme}://{uri.Authority}";

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            fallback,
            "https://importtimporta.com.br",
            "https://www.importtimporta.com.br",
        };

        var origin = $"{uri.Scheme}://{uri.Authority}";
        return allowed.Contains(origin) ? origin : fallback;
    }

    private static bool IsLocalDevOrigin(Uri uri)
    {
        return uri.Scheme is "http" or "https"
            && (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase));
    }
}
