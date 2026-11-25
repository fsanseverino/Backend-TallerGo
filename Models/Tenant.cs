namespace Backend_TallerGo.Models;

public class Tenant
{
    public const string EstadoActivo = "ACTIVO";
    public const string EstadoSuspendido = "SUSPENDIDO";

    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Estado { get; set; } = EstadoActivo;
    public DateTime CreadoEn { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}