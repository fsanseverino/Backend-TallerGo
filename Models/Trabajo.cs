namespace Backend_TallerGo.Models;

public enum EstadoTrabajo
{
    SIN_INICIAR,
    EN_CURSO,
    FINALIZADO
}

public enum TipoItem
{
    MANO_OBRA,
    REPUESTO,
    OTRO
}

public class Trabajo
{
    public string Id { get; set; } = string.Empty;
    public string VehiculoId { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public string? FechaRealizacion { get; set; }
    public EstadoTrabajo Estado { get; set; } = EstadoTrabajo.SIN_INICIAR;
    public decimal Monto { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public List<TrabajoItem> Items { get; set; } = new();
    public List<PagoTrabajo> Pagos { get; set; } = new();
}