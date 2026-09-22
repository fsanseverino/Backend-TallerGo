namespace Backend_TallerGo;

public class SesionActual
{
    public string UsuarioId { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public HashSet<string> Permisos { get; set; } = new(StringComparer.Ordinal);

    public bool TienePermiso(string codigo)
    {
        if (string.Equals(Rol, "ADMIN", StringComparison.OrdinalIgnoreCase))
            return true;
        return Permisos.Contains(codigo);
    }
}