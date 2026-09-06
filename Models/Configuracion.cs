namespace Backend_TallerGo.Models;

public class Configuracion
{
    public string Id { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class ConfiguracionUpdate
{
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}