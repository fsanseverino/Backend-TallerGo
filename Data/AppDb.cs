using Backend_TallerGo.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Data;

/// Recupera una instancia del contexto de base de datos para una petición,
/// asegurando que el esquema y los datos de ejemplo existan.
public class AppDb
{
    public static TallerGoDbContext Open()
    {
        var db = new TallerGoDbContext(
            new DbContextOptionsBuilder<TallerGoDbContext>()
                .UseSqlite("Data Source=tallergo.db")
                .Options
        );
        db.Database.EnsureCreated();
        DbInitializer.Seed(db);
        return db;
    }
}