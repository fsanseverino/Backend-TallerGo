using Backend_TallerGo.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Data;

public class DbInitializer
{
    public static DateTime ParseDate(string value) => DateTime.Parse(value);

    public static void Seed(TallerGoDbContext db)
    {
        if (db.Configuraciones.Count() == 0)
        {
            var ahora = DateTime.UtcNow;
            db.Configuraciones.AddRange(
                Config("cfg-nombre", "taller.nombre", "", "taller", ahora),
                Config("cfg-direccion", "taller.direccion", "", "taller", ahora),
                Config("cfg-telefono", "taller.telefono", "", "taller", ahora),
                Config("cfg-email", "taller.email", "", "taller", ahora),
                Config("cfg-cuit", "taller.cuit", "", "taller", ahora),
                Config("cfg-prefijo", "trabajos.prefijo", "T-", "trabajos", ahora),
                Config("cfg-siguiente", "trabajos.siguiente", "1", "trabajos", ahora)
            );
            db.SaveChanges();
        }

        if (db.Clientes.Count() > 0)
            return;

        var clientes = new[]
        {
            Cliente("cli-001", "Juan", "Pérez", TipoDocumento.DNI, "30123456", "11-5555-1234", "juan.perez@mail.com", "Av. Siempre Viva 742", "2026-06-01T10:00:00Z"),
            Cliente("cli-002", "María", "González", TipoDocumento.DNI, "28765432", "11-5555-5678", "maria.gonzalez@mail.com", "Calle Falsa 123", "2026-06-05T10:00:00Z"),
            Cliente("cli-003", "Carlos", "Rodríguez", TipoDocumento.CUIT, "20334567890", "11-5555-9012", "carlos.rodriguez@mail.com", "Belgrano 456", "2026-06-10T10:00:00Z"),
            Cliente("cli-004", "Lucía", "Fernández", TipoDocumento.DNI, "35678901", "11-5555-3456", "lucia.fernandez@mail.com", "San Martín 789", "2026-06-15T10:00:00Z"),
        };
        db.Clientes.AddRange(clientes);

        var empleados = new[]
        {
            Empleado("emp-001", "Diego", "Ramírez", Especialidad.MECANICO, "11-6000-1001", "diego.ramirez@tallergo.com", "Rivadavia 100", "2020-03-01T09:00:00Z", EstadoEmpleado.ACTIVO),
            Empleado("emp-002", "Silvia", "López", Especialidad.CHAPISTA, "11-6000-1002", "silvia.lopez@tallergo.com", "Mitre 200", "2021-07-15T09:00:00Z", EstadoEmpleado.ACTIVO),
            Empleado("emp-003", "Marcos", "Torres", Especialidad.ELECTRICISTA, "11-6000-1003", "marcos.torres@tallergo.com", "Belgrano 300", "2022-01-10T09:00:00Z", EstadoEmpleado.ACTIVO),
        };
        db.Empleados.AddRange(empleados);

        var vehiculos = new[]
        {
            Vehiculo("veh-001", "cli-001", "Toyota", "Corolla", 2020, "AB123CD", "Gris", "8X1AB12C345678901", "2ZR-FE", TipoMotor.NAFTA),
            Vehiculo("veh-002", "cli-002", "Volkswagen", "Gol Trend", 2018, "AC456EF", "Blanco", "8AWG1123GHJ56789", "1.6 MSI", TipoMotor.NAFTA),
            Vehiculo("veh-003", "cli-003", "Ford", "Fiesta KD", 2019, "AD789GH", "Azul", "8AAPK123KL901234", "1.6 Ti-VCT", TipoMotor.NAFTA),
            Vehiculo("veh-004", "cli-004", "Renault", "Kangoo", 2021, "AE012IJ", "Negro", "8AIRE456MN345678", "K9K 656", TipoMotor.DIESEL),
            Vehiculo("veh-005", "cli-001", "Fiat", "Uno", 2016, "AF345KL", "Rojo", "8AFIA789OP567890", "Fire 1.3", TipoMotor.NAFTA),
        };
        db.Vehiculos.AddRange(vehiculos);

        var trabajos = new[]
        {
            Trabajo("tra-001", "veh-001", "cli-001", "emp-001", "Cambio de aceite y filtros", 48250, "2026-06-20", "2026-06-22", EstadoTrabajo.FINALIZADO, 25000, "Aceite sintético y filtro de aire."),
            Trabajo("tra-002", "veh-002", "cli-002", "emp-002", "Reparación de frenos", 51000, "2026-07-05", "2026-07-07", EstadoTrabajo.FINALIZADO, 45000, "Pastillas y discos delanteros."),
            Trabajo("tra-003", "veh-003", "cli-003", null, "Alineación y balanceo", 75300, "2026-07-10", "2026-07-10", EstadoTrabajo.FINALIZADO, 15000, ""),
            Trabajo("tra-004", "veh-004", "cli-004", "emp-003", "Reparación de clutch", 68900, "2026-08-01", "2026-08-04", EstadoTrabajo.FINALIZADO, 80000, "Cambio completo de kit de embrague."),
            Trabajo("tra-005", "veh-005", "cli-001", null, "Cambio de batería", 81200, "2026-08-20", null, EstadoTrabajo.SIN_INICIAR, 35000, "Batería nueva 70Ah."),
            Trabajo("tra-006", "veh-001", "cli-001", "emp-001", "Diagnóstico de motor", 49750, "2026-08-25", null, EstadoTrabajo.EN_CURSO, 10000, "Pendiente de resultado de diagnóstico."),
        };
        db.Trabajos.AddRange(trabajos);

        AddItem(db, "tra-001", "it-1", "Mano de obra cambio de aceite", TipoItem.MANO_OBRA, 1, 12000);
        AddItem(db, "tra-001", "it-2", "Filtro de aceite", TipoItem.REPUESTO, 1, 8000);
        AddItem(db, "tra-001", "it-3", "Aceite sintético", TipoItem.REPUESTO, 1, 5000);
        AddItem(db, "tra-002", "it-4", "Mano de obra frenos", TipoItem.MANO_OBRA, 1, 20000);
        AddItem(db, "tra-002", "it-5", "Juego de pastillas", TipoItem.REPUESTO, 1, 15000);
        AddItem(db, "tra-002", "it-6", "Rectificado de discos", TipoItem.MANO_OBRA, 1, 10000);
        AddItem(db, "tra-003", "it-7", "Alineación", TipoItem.MANO_OBRA, 1, 8000);
        AddItem(db, "tra-003", "it-8", "Balanceo", TipoItem.MANO_OBRA, 1, 7000);
        AddItem(db, "tra-004", "it-9", "Mano de obra clutch", TipoItem.MANO_OBRA, 1, 35000);
        AddItem(db, "tra-004", "it-10", "Kit de embrague", TipoItem.REPUESTO, 1, 45000);
        AddItem(db, "tra-005", "it-11", "Mano de obra", TipoItem.MANO_OBRA, 1, 5000);
        AddItem(db, "tra-005", "it-12", "Batería", TipoItem.REPUESTO, 1, 30000);
        AddItem(db, "tra-006", "it-13", "Diagnóstico por scanner", TipoItem.MANO_OBRA, 1, 10000);

        AddPago(db, "tra-001", "pag-1", 25000, "2026-06-22");
        AddPago(db, "tra-002", "pag-2", 20000, "2026-07-07");
        AddPago(db, "tra-003", "pag-3", 15000, "2026-07-10");
        AddPago(db, "tra-004", "pag-4", 80000, "2026-08-04");
        AddPago(db, "tra-006", "pag-5", 5000, "2026-08-25");

        SeedCaja(db);

        db.SaveChanges();
    }

        private static Cliente Cliente(string id, string nombre, string apellido, TipoDocumento tipo, string documento, string telefono, string email, string direccion, string createdAt) =>
        new Cliente()
        {
            Id = id,
            Nombre = nombre,
            Apellido = apellido,
            TipoDocumento = tipo,
            Documento = documento,
            Telefono = telefono,
            Email = email,
            Direccion = direccion,
            CreatedAt = ParseDate(createdAt),
        };

    private static Empleado Empleado(string id, string nombre, string apellido, Especialidad especialidad, string telefono, string email, string direccion, string fechaIngreso, EstadoEmpleado estado) =>
        new Empleado()
        {
            Id = id,
            Nombre = nombre,
            Apellido = apellido,
            Especialidad = especialidad,
            Telefono = telefono,
            Email = email,
            Direccion = direccion,
            FechaIngreso = ParseDate(fechaIngreso),
            Estado = estado,
            CreatedAt = ParseDate(fechaIngreso),
        };

    private static Vehiculo Vehiculo(string id, string clienteId, string marca, string modelo, int anio, string patente, string color, string chasis, string numeroMotor, TipoMotor tipoMotor) =>
        new Vehiculo()
        {
            Id = id,
            ClienteId = clienteId,
            Marca = marca,
            Modelo = modelo,
            Anio = anio,
            Patente = patente,
            Color = color,
            Chasis = chasis,
            NumeroMotor = numeroMotor,
            TipoMotor = tipoMotor,
        };

    private static Trabajo Trabajo(string id, string vehiculoId, string clienteId, string? empleadoId, string descripcion, int? kilometrajeIngreso, string fechaIngreso, string? fechaRealizacion, EstadoTrabajo estado, decimal monto, string observaciones) =>
        new Trabajo()
        {
            Id = id,
            VehiculoId = vehiculoId,
            ClienteId = clienteId,
            EmpleadoId = empleadoId,
            Descripcion = descripcion,
            KilometrajeIngreso = kilometrajeIngreso,
            FechaIngreso = ParseDate(fechaIngreso),
            FechaRealizacion = fechaRealizacion,
            Estado = estado,
            Monto = monto,
            Observaciones = observaciones,
        };

    private static void AddItem(TallerGoDbContext db, string trabajoId, string id, string descripcion, TipoItem tipo, decimal cantidad, decimal precioUnitario)
    {
        db.TrabajoItems.Add(new TrabajoItem()
        {
            Id = id,
            TrabajoId = trabajoId,
            Descripcion = descripcion,
            Tipo = tipo,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
        });
    }

    private static void AddPago(TallerGoDbContext db, string trabajoId, string id, decimal monto, string fecha)
    {
        db.PagosTrabajo.Add(new PagoTrabajo()
        {
            Id = id,
            TrabajoId = trabajoId,
            Monto = monto,
            Fecha = ParseDate(fecha),
        });
    }

    private static Configuracion Config(string id, string clave, string valor, string categoria, DateTime updatedAt) =>
        new Configuracion()
        {
            Id = id,
            Clave = clave,
            Valor = valor,
            Categoria = categoria,
            UpdatedAt = updatedAt,
        };

    private static void SeedCaja(TallerGoDbContext db)
    {
        var caja1 = new Caja() { Id = "caja-001", FechaApertura = ParseDate("2026-08-01T08:00:00Z"), SaldoInicial = 5000, Estado = EstadoCaja.CERRADA, FechaCierre = ParseDate("2026-08-01T18:00:00Z"), Notas = "Caja del día 1." };
        var caja2 = new Caja() { Id = "caja-002", FechaApertura = ParseDate("2026-08-15T08:00:00Z"), SaldoInicial = 10000, Estado = EstadoCaja.CERRADA, FechaCierre = ParseDate("2026-08-15T18:00:00Z"), Notas = "Caja del día 15." };
        db.Cajas.AddRange(caja1, caja2);

        db.MovimientosCaja.AddRange(
            Mov("mov-001", "caja-001", TipoMovimiento.INGRESO_TRABAJO, "Cambio de aceite y filtros", 25000, "2026-08-01T10:00:00Z", "tra-001"),
            Mov("mov-002", "caja-001", TipoMovimiento.EGRESO, "Compra de repuestos", 8000, "2026-08-01T12:00:00Z", null),
            Mov("mov-003", "caja-002", TipoMovimiento.INGRESO_TRABAJO, "Reparación de clutch", 80000, "2026-08-15T11:00:00Z", "tra-004"),
            Mov("mov-004", "caja-002", TipoMovimiento.INGRESO, "Aporte de fondos", 20000, "2026-08-15T14:00:00Z", null),
            Mov("mov-005", "caja-002", TipoMovimiento.EGRESO, "Pago a proveedor", 15000, "2026-08-15T16:00:00Z", null)
        );
    }

    private static MovimientoCaja Mov(string id, string cajaId, TipoMovimiento tipo, string concepto, decimal monto, string fecha, string? trabajoId) =>
        new MovimientoCaja()
        {
            Id = id,
            CajaId = cajaId,
            Tipo = tipo,
            Concepto = concepto,
            Monto = monto,
            Fecha = ParseDate(fecha),
            TrabajoId = trabajoId,
        };
}