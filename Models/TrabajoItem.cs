namespace Backend_TallerGo.Models;

public class TrabajoItem
{
    public string Id { get; set; } = string.Empty;
    public string TrabajoId { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public TipoItem Tipo { get; set; } = TipoItem.MANO_OBRA;
    public decimal Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }
}