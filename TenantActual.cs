namespace Backend_TallerGo;

/// Slug del tenant asociado a la petición HTTP actual.
/// El middleware de auth lo fija por request; "admin" es la plataforma.
public static class TenantActual
{
    private static readonly AsyncLocal<string?> _slug = new();

    public static string Slug =>
        _slug.Value ?? throw new InvalidOperationException("No hay tenant en el contexto de la petición.");

    public static bool Hay => !string.IsNullOrWhiteSpace(_slug.Value);

    public static void Fijar(string slug) => _slug.Value = slug;

    public static void Limpiar() => _slug.Value = null;
}