namespace Backend_TallerGo.Models;

public enum Especialidad
{
    MECANICO,
    CHAPISTA,
    ELECTRICISTA,
    PINTOR,
    OTRO
}

public enum EstadoEmpleado
{
    ACTIVO,
    INACTIVO
}

public class Empleado
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public Especialidad Especialidad { get; set; } = Especialidad.OTRO;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public EstadoEmpleado Estado { get; set; } = EstadoEmpleado.ACTIVO;
    public DateTime? CreatedAt { get; set; }
}
