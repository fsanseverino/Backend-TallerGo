using Backend_TallerGo.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Data;

/// Recupera una instancia del contexto de base de datos para una petición,
/// asegurando que el esquema y los datos base existan.
public class AppDb
{
    public static TallerGoDbContext Open()
    {
        var db = new TallerGoDbContext(
            new DbContextOptionsBuilder<TallerGoDbContext>()
                .UseSqlite($"Data Source={RutaDb()}")
                .Options
        );
        db.Database.EnsureCreated();
        EsquemaSeguro.Verificar(db);
        DbInitializer.Seed(db);
        return db;
    }

    private static string RutaDb()
    {
        var baseDir = string.IsNullOrWhiteSpace(AppConfig.DataRuta)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(AppConfig.DataRuta, Directory.GetCurrentDirectory());
        Directory.CreateDirectory(baseDir);
        return Path.Combine(baseDir, "tallergo.db");
    }
}