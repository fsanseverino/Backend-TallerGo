namespace Backend_TallerGo.Models;

public class PagoTrabajo
{
    public string Id { get; set; } = string.Empty;
    public string TrabajoId { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
}