namespace Backend_TallerGo;

/// Configuración de la aplicación leída una sola vez al arrancar.
/// En producción todo se puede sobreescribir por variables de entorno
/// (ej. Jwt__Secret, SeedDemoData, Data__Ruta, Backups__Ruta).
public static class AppConfig
{
    private static string? _jwtSecret;
    private static string[] _corsOrigins = Array.Empty<string>();
    private static bool _seedDemoData = true;
    private static string _dataRuta = "";
    private static string _backupsRuta = "data/backups";
    private static string? _initialAdminPassword;

    public static string JwtSecret =>
        _jwtSecret ?? throw new InvalidOperationException("AppConfig.Cargar no fue invocado.");

    public static IReadOnlyList<string> CorsOrigins => _corsOrigins;

    public static bool SeedDemoData => _seedDemoData;

    /// Carpeta base de los datos (vacía = carpeta de trabajo actual).
    public static string DataRuta => _dataRuta;

    public static string BackupsRuta => _backupsRuta;

    public static string? InitialAdminPassword => _initialAdminPassword;

    public static string ResolverRuta(string relativa)
    {
        if (string.IsNullOrWhiteSpace(relativa))
            return Directory.GetCurrentDirectory();
        return Path.GetFullPath(relativa, Directory.GetCurrentDirectory());
    }

    public static void Cargar(IConfiguration cfg)
    {
        var secret = cfg["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
            throw new InvalidOperationException("Falta la configuración Jwt:Secret (variable de entorno Jwt__Secret).");
        _jwtSecret = secret;

        _corsOrigins = cfg.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();
        _seedDemoData = bool.TryParse(cfg["SeedDemoData"], out var demo) ? demo : true;
        _dataRuta = cfg["Data:Ruta"] ?? "";
        _backupsRuta = cfg["Backups:Ruta"] ?? "data/backups";
        _initialAdminPassword = cfg["InitialAdminPassword"];
    }
}