using System.Security.Claims;
using System.Text.Json;

namespace Shared.Auth;

internal static class JwtParser
{
    public static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(
            PadBase64(payload));

        var claims = JsonSerializer
            .Deserialize<Dictionary<string, object>>(jsonBytes)!;

        foreach (var claim in claims)
        {
            if (claim.Value is JsonElement el &&
                el.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in el.EnumerateArray())
                {
                    yield return new Claim(claim.Key, item.ToString()!);
                }
            }
            else
            {
                yield return new Claim(claim.Key, claim.Value.ToString()!);
            }
        }
    }

    private static string PadBase64(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        return base64.PadRight(
            base64.Length + (4 - base64.Length % 4) % 4, '=');
    }
}
