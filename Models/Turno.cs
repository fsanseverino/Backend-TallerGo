namespace Backend_TallerGo.Models;

public enum EstadoTurno
{
    AGENDADO,
    CONFIRMADO,
    ATENDIDO,
    CANCELADO,
    NO_SE_PRESENTO
}

public class Turno
{
    public string Id { get; set; } = string.Empty;
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public string? ClienteId { get; set; }
    public string? VehiculoId { get; set; }
    public string? EmpleadoId { get; set; }
    public string? Motivo { get; set; }
    public string? Notas { get; set; }
    public EstadoTurno Estado { get; set; } = EstadoTurno.AGENDADO;
    public string? TrabajoId { get; set; }
}