namespace Backend_TallerGo.Models;

public class Catalogo
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class CatalogoValor
{
    public string Id { get; set; } = string.Empty;
    public string CatalogoId { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public int Orden { get; set; }
}
