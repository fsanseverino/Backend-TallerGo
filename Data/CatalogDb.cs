using Backend_TallerGo.Models;
using Microsoft.Data.Sqlite;

namespace Backend_TallerGo.Data;

/// Registro de talleres (tenants) en el catálogo de la plataforma.
/// Cada tenant tiene su propia base SQLite en data/tenants/{slug}/tallergo.db.
public static class CatalogDb
{
    public static string RutaDb() => Path.Combine(AppConfig.ResolverRuta(AppConfig.DataRuta), "catalog.db");

    public static void AsegurarEsquema()
    {
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Tenants (
                Id TEXT PRIMARY KEY,
                Slug TEXT UNIQUE NOT NULL,
                Nombre TEXT NOT NULL,
                Estado TEXT NOT NULL,
                CreadoEn TEXT NOT NULL,
                Email TEXT NULL,
                Telefono TEXT NULL
            )";
        cmd.ExecuteNonQuery();
    }

    public static Tenant? PorSlug(string slug)
    {
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT Id, Slug, Nombre, Estado, CreadoEn, Email, Telefono FROM Tenants WHERE Slug = $slug";
        cmd.Parameters.AddWithValue("$slug", slug);
        using var r = cmd.ExecuteReader();
        return r.Read() ? Leer(r) : null;
    }

    public static List<Tenant> Listar()
    {
        var lista = new List<Tenant>();
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT Id, Slug, Nombre, Estado, CreadoEn, Email, Telefono FROM Tenants ORDER BY Nombre";
        using var r = cmd.ExecuteReader();
        while (r.Read())
            lista.Add(Leer(r));
        return lista;
    }

    public static bool ExisteSlug(string slug)
    {
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT count(*) FROM Tenants WHERE Slug = $slug";
        cmd.Parameters.AddWithValue("$slug", slug);
        return Convert.ToInt64(cmd.ExecuteScalar() ?? 0) > 0;
    }

    public static void Crear(Tenant t)
    {
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO Tenants (Id, Slug, Nombre, Estado, CreadoEn, Email, Telefono)
                            VALUES ($id, $slug, $nombre, $estado, $creado, $email, $telefono)";
        cmd.Parameters.AddWithValue("$id", t.Id);
        cmd.Parameters.AddWithValue("$slug", t.Slug);
        cmd.Parameters.AddWithValue("$nombre", t.Nombre);
        cmd.Parameters.AddWithValue("$estado", t.Estado);
        cmd.Parameters.AddWithValue("$creado", t.CreadoEn.ToString("O"));
        cmd.Parameters.AddWithValue("$email", (object?)t.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$telefono", (object?)t.Telefono ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public static void Actualizar(string id, string nombre, string? email, string? telefono)
    {
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "UPDATE Tenants SET Nombre = $nombre, Email = $email, Telefono = $telefono WHERE Id = $id OR Slug = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.Parameters.AddWithValue("$nombre", nombre);
        cmd.Parameters.AddWithValue("$email", (object?)email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$telefono", (object?)telefono ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public static void CambiarEstado(string id, string estado)
    {
        using var c = Conectar();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "UPDATE Tenants SET Estado = $estado WHERE Id = $id OR Slug = $id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.Parameters.AddWithValue("$estado", estado);
        cmd.ExecuteNonQuery();
    }

    private static SqliteConnection Conectar()
    {
        var dir = Path.GetDirectoryName(RutaDb());
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);
        var c = new SqliteConnection($"Data Source={RutaDb()}");
        c.Open();
        return c;
    }

    private static Tenant Leer(SqliteDataReader r) => new()
    {
        Id = r.GetString(0),
        Slug = r.GetString(1),
        Nombre = r.GetString(2),
        Estado = r.GetString(3),
        CreadoEn = DateTime.TryParse(r.GetString(4), null, System.Globalization.DateTimeStyles.RoundtripKind, out var creado) ? creado : DateTime.MinValue,
        Email = r.IsDBNull(5) ? null : r.GetString(5),
        Telefono = r.IsDBNull(6) ? null : r.GetString(6),
    };
}