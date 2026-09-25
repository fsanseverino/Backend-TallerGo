namespace Backend_TallerGo.Models;

public class CrearUsuarioRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? EmpleadoId { get; set; }
    public string RolId { get; set; } = string.Empty;
    public EstadoUsuario Estado { get; set; } = EstadoUsuario.ACTIVO;
}

public class ActualizarUsuarioRequest
{
    public string? RolId { get; set; }
    public EstadoUsuario? Estado { get; set; }
    public string? EmpleadoId { get; set; }
    public string? Password { get; set; }
}

public class CambiarPasswordRequest
{
    public string Password { get; set; } = string.Empty;
}