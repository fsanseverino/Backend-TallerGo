namespace Backend_TallerGo.Models;

public enum EstadoUsuario
{
    ACTIVO,
    INACTIVO
}

public class Usuario
{
    public string Id { get; set; } = string.Empty;
    public string? EmpleadoId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Sal { get; set; } = string.Empty;
    public string RolId { get; set; } = string.Empty;
    public EstadoUsuario Estado { get; set; } = EstadoUsuario.ACTIVO;
    public DateTime? CreatedAt { get; set; }

}