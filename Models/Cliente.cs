namespace Backend_TallerGo.Models;

public enum TipoDocumento
{
    DNI,
    CUIT,
    CUIL,
    PASAPORTE
}

public class Cliente
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.DNI;
    public string Documento { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}