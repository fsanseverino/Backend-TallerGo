namespace Backend_TallerGo.Models;

public class StockMovimiento
{
    public string Id { get; set; } = string.Empty;
    public string RepuestoId { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = "ENTRADA";
    public int Cantidad { get; set; }
    public string? Motivo { get; set; }
    public int StockResultante { get; set; }
}