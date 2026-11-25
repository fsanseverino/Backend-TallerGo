namespace Backend_TallerGo;

/// Log de errores en archivo (además de la consola). Sin dependencias externas.
public static class Logs
{
    public static void EscribirError(Exception ex)
    {
        Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR {ex}");
        try
        {
            var dir = Path.Combine(AppConfig.ResolverRuta(AppConfig.DataRuta), "logs");
            Directory.CreateDirectory(dir);
            var archivo = Path.Combine(dir, $"errores-{DateTime.Now:yyyyMMdd}.log");
            File.AppendAllText(archivo, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}{Environment.NewLine}");
        }
        catch
        {
            // El log de errores nunca debe tumbar la app.
        }
    }
}