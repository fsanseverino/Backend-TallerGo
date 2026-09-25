using Backend_TallerGo.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Data;

/// Asegura el esquema de las tablas agregadas después de la creación inicial
/// de la base (EnsureCreated no altera una base existente).
public static class EsquemaSeguro
{
    public static void Verificar(TallerGoDbContext db)
    {
        // Columnas nuevas en la tabla existente Trabajos.
        if (!ExisteColumna(db, "Trabajos", "FechaEntrega"))
        {
            Ejecutar(db, "ALTER TABLE Trabajos ADD COLUMN FechaEntrega TEXT NULL");

                    if (!ExisteColumna(db, "Trabajos", "RepuestoId"))
                    {
                        Ejecutar(db, "ALTER TABLE Trabajos ADD COLUMN RepuestoId TEXT NULL");
                    }

                    if (!ExisteColumna(db, "Trabajos", "PresupuestoId"))
                    {
                        Ejecutar(db, "ALTER TABLE Trabajos ADD COLUMN PresupuestoId TEXT NULL");
                    }
        }

        if (!ExisteColumna(db, "Trabajos", "RepuestoId"))
        {
            Ejecutar(db, "ALTER TABLE Trabajos ADD COLUMN RepuestoId TEXT NULL");
        }

        if (!ExisteColumna(db, "Trabajos", "PresupuestoId"))
        {
            Ejecutar(db, "ALTER TABLE Trabajos ADD COLUMN PresupuestoId TEXT NULL");
        }

        if (!ExisteColumna(db, "Usuarios", "DebeCambiarPassword"))
        {
            Ejecutar(db, "ALTER TABLE Usuarios ADD COLUMN DebeCambiarPassword INTEGER NOT NULL DEFAULT 0");
        }

        var sentencias = new[]
        {
            // phpcs:disable
            @"CREATE TABLE IF NOT EXISTS Turnos (
                Id TEXT NOT NULL PRIMARY KEY,
                Fecha TEXT NOT NULL,
                Hora TEXT NOT NULL,
                ClienteId TEXT NULL,
                VehiculoId TEXT NULL,
                EmpleadoId TEXT NULL,
                Motivo TEXT NULL,
                Notas TEXT NULL,
                Estado TEXT NOT NULL,
                TrabajoId TEXT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS Repuestos (
                Id TEXT NOT NULL PRIMARY KEY,
                Codigo TEXT NULL,
                Descripcion TEXT NOT NULL,
                Categoria TEXT NULL,
                Marca TEXT NULL,
                Costo TEXT NOT NULL,
                PrecioVenta TEXT NOT NULL,
                Stock INTEGER NOT NULL,
                StockMinimo INTEGER NOT NULL,
                Proveedor TEXT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS Presupuestos (
                Id TEXT NOT NULL PRIMARY KEY,
                ClienteId TEXT NOT NULL,
                VehiculoId TEXT NOT NULL,
                EmpleadoId TEXT NULL,
                Descripcion TEXT NOT NULL,
                Fecha TEXT NOT NULL,
                Estado TEXT NOT NULL,
                FechaVencimiento TEXT NULL,
                Monto TEXT NOT NULL,
                Observaciones TEXT NOT NULL,
                TrabajoId TEXT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS PresupuestoItems (
                Id TEXT NOT NULL PRIMARY KEY,
                PresupuestoId TEXT NOT NULL,
                Descripcion TEXT NOT NULL,
                Tipo TEXT NOT NULL,
                Cantidad TEXT NOT NULL,
                PrecioUnitario TEXT NOT NULL,
                RepuestoId TEXT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS Roles (
                Id TEXT NOT NULL PRIMARY KEY,
                Nombre TEXT NOT NULL,
                Descripcion TEXT NOT NULL,
                Permisos TEXT NOT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS Usuarios (
                Id TEXT NOT NULL PRIMARY KEY,
                EmpleadoId TEXT NULL,
                Usuario TEXT NOT NULL,
                PasswordHash TEXT NOT NULL,
                Sal TEXT NOT NULL,
                RolId TEXT NOT NULL,
                Estado TEXT NOT NULL,
                DebeCambiarPassword INTEGER NOT NULL DEFAULT 0,
                CreatedAt TEXT NULL
            )",
        };
        foreach (var sql in sentencias)
        {
            Ejecutar(db, sql);
        }
    }

    private static bool ExisteColumna(TallerGoDbContext db, string tabla, string columna)
    {
        using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = $"SELECT count(*) FROM pragma_table_info('{tabla}') WHERE name = '{columna}'";
        db.Database.OpenConnection();
        object? resultado;
        try
        {
            resultado = cmd.ExecuteScalar();
        }
        finally
        {
            db.Database.CloseConnection();
        }
        return Convert.ToInt64(resultado ?? 0) > 0;
    }

    private static void Ejecutar(TallerGoDbContext db, string sql)
    {
        using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;
        db.Database.OpenConnection();
        try
        {
            cmd.ExecuteNonQuery();
        }
        finally
        {
            db.Database.CloseConnection();
        }
    }
}
