namespace Backend_TallerGo.Models;

public enum EstadoPresupuesto
{
    BORRADOR,
    ENVIADO,
    APROBADO,
    CONVERTIDO,
    RECHAZADO
}

public class Presupuesto
{
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string VehiculoId { get; set; } = string.Empty;
    public string? EmpleadoId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    public EstadoPresupuesto Estado { get; set; } = EstadoPresupuesto.BORRADOR;
    public string? FechaVencimiento { get; set; }
    public decimal Monto { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public string? TrabajoId { get; set; }
    public List<PresupuestoItem> Items { get; set; } = new();
}

public class PresupuestoItem
{
    public string Id { get; set; } = string.Empty;
    public string PresupuestoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoItem Tipo { get; set; }
    public decimal Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }
    public string? RepuestoId { get; set; }
}