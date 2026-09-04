namespace Backend_TallerGo.Models;

public enum EstadoCaja
{
    ABIERTA,
    CERRADA
}

public enum TipoMovimiento
{
    INGRESO,
    EGRESO,
    INGRESO_TRABAJO
}

public class Caja
{
    public string Id { get; set; } = string.Empty;
    public DateTime FechaApertura { get; set; }
    public decimal SaldoInicial { get; set; }
    public EstadoCaja Estado { get; set; } = EstadoCaja.ABIERTA;
    public DateTime? FechaCierre { get; set; }
    public string Notas { get; set; } = string.Empty;
}