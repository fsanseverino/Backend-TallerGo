namespace Backend_TallerGo.Models;

public class Vehiculo
{
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Patente { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Chasis { get; set; } = string.Empty;
}