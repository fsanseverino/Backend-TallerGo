using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Backend_TallerGo;

public static class AuthToken
{
    private static readonly byte[] Clave = Encoding.UTF8.GetBytes("TallerGo-clave-firma-2026");
    public const string CLAVE_SESION = "SesionActual";

    public static string Generar(DateTime expira, string usuarioId, string rol, IEnumerable<string> permisos)
    {
        var payload = new
        {
            exp = new DateTimeOffset(expira).ToUnixTimeSeconds(),
            uid = usuarioId,
            rol = rol,
            perms = permisos.ToArray(),
        };
        var json = JsonSerializer.Serialize(payload);
        var basePayload = Base64(Encoding.UTF8.GetBytes(json));
        var firma = Firmar(basePayload);
        return $"{basePayload}.{firma}";
    }

    public static SesionActual? Decodificar(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var partes = token.Split('.');
        if (partes.Length != 2)
            return null;

        if (!FixedTimeEquals(partes[1], Firmar(partes[0])))
            return null;

        Payload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<Payload>(Encoding.UTF8.GetString(FromBase64(partes[0])));
        }
        catch (Exception)
        {
            return null;
        }

        if (payload is null || string.IsNullOrEmpty(payload.Rol) || DateTimeOffset.FromUnixTimeSeconds(payload.Exp) <= DateTimeOffset.UtcNow)
            return null;

        return new SesionActual
        {
            UsuarioId = payload.Uid ?? string.Empty,
            Rol = payload.Rol,
            Permisos = new HashSet<string>(payload.Perms ?? Array.Empty<string>(), StringComparer.Ordinal),
        };
    }

    public static bool Validar(string? token) => Decodificar(token) != null;

    private sealed class Payload
    {
        public long Exp { get; set; }
        public string? Uid { get; set; }
        public string? Rol { get; set; }
        public string[]? Perms { get; set; }
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