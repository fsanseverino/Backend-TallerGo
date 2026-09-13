namespace Backend_TallerGo.Models;

public enum EstadoTrabajo
{
    SIN_INICIAR,
    EN_CURSO,
    ESPERANDO_REPUESTO,
    FINALIZADO,
    ENTREGADO
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
    public string? EmpleadoId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int? KilometrajeIngreso { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string? FechaRealizacion { get; set; }
    public string? FechaEntrega { get; set; }
    public EstadoTrabajo Estado { get; set; } = EstadoTrabajo.SIN_INICIAR;
    public decimal Monto { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public List<TrabajoItem> Items { get; set; } = new();
    public List<PagoTrabajo> Pagos { get; set; } = new();
}

public class TrabajoItem
{
    public string Id { get; set; } = string.Empty;
    public string TrabajoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoItem Tipo { get; set; }
    public decimal Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }
    public string? RepuestoId { get; set; }
}