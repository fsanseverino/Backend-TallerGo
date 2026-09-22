using System.Security.Cryptography;

namespace Backend_TallerGo;

public static class PasswordHasher
{
    private const int Iteraciones = 100_000;
    private const int TamanoSal = 16;
    private const int TamanoHash = 32;

    public static (string Hash, string Sal) Hash(string password)
    {
        var sal = RandomNumberGenerator.GetBytes(TamanoSal);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, sal, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(sal));
    }

    public static bool Verificar(string password, string hash, string sal)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(sal))
            return false;

        try
        {
            var hashEsperado = Convert.FromBase64String(hash);
            var hashReal = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(sal), Iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);
            return CryptographicOperations.FixedTimeEquals(hashReal, hashEsperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}