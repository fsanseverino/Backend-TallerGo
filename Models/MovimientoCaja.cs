namespace Backend_TallerGo.Models;

public class MovimientoCaja
{
    public string Id { get; set; } = string.Empty;
    public string CajaId { get; set; } = string.Empty;
    public TipoMovimiento Tipo { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string? TrabajoId { get; set; }
}