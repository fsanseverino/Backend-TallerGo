using System.Security.Cryptography;
using System.Text;

namespace Backend_TallerGo;

public static class AuthToken
{
    private static readonly byte[] Clave = Encoding.UTF8.GetBytes("TallerGo-clave-firma-2026");

    public static string Generar(DateTime expira)
    {
        var exp = new DateTimeOffset(expira).ToUnixTimeSeconds().ToString();
        var payload = Base64(Encoding.UTF8.GetBytes(exp));
        var firma = Firmar(payload);
        return $"{payload}.{firma}";
    }

    public static bool Validar(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var partes = token.Split('.');
        if (partes.Length != 2)
            return false;

        if (!FixedTimeEquals(partes[1], Firmar(partes[0])))
            return false;

        if (!long.TryParse(Encoding.UTF8.GetString(FromBase64(partes[0])), out var exp))
            return false;

        return DateTimeOffset.FromUnixTimeSeconds(exp) > DateTimeOffset.UtcNow;
    }

    private static string Firmar(string payload)
    {
        using var hmac = new HMACSHA256(Clave);
        return Base64(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
    }

    private static string Base64(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=');

    private static byte[] FromBase64(string value)
    {
        var padding = (value.Length % 4) == 0 ? 0 : 4 - (value.Length % 4);
        return Convert.FromBase64String(value.PadRight(value.Length + padding, '='));
    }

    private static bool FixedTimeEquals(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}