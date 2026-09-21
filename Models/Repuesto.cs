namespace Backend_TallerGo.Models;

public class Repuesto
{
    public string Id { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string? Marca { get; set; }
    public decimal Costo { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public string? Proveedor { get; set; }
}

public class AjusteStockRequest
{
    public string Tipo { get; set; } = "ENTRADA";
    public int Cantidad { get; set; } = 1;
    public string? Motivo { get; set; }
}