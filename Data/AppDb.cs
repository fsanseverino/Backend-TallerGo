using Backend_TallerGo.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Data;

/// Base por taller (tenant): cada uno tiene su propio archivo SQLite.
/// El contexto se abre por petición apuntando a la base del tenant de la sesión.
public class AppDb
{
    public const string SlugPlataforma = "admin";
    public const string SlugDemo = "tallergo";

    /// Abre la base del tenant de la petición actual (el middleware fijó TenantActual).
    public static TallerGoDbContext Open()
    {
        if (!TenantActual.Hay)
            throw new InvalidOperationException("No hay tenant activo para abrir la base.");
        return OpenParaSesion(TenantActual.Slug);
    }

    /// Abre la base del superadministrador (plataforma).
    public static TallerGoDbContext OpenPlataforma() => OpenParaSesion(SlugPlataforma);

    public static TallerGoDbContext OpenParaSesion(string slug)
    {
        var db = new TallerGoDbContext(
            new DbContextOptionsBuilder<TallerGoDbContext>()
                .UseSqlite($"Data Source={RutaDb(slug)}")
                .Options
        );
        db.Database.EnsureCreated();
        EsquemaSeguro.Verificar(db);
        DbInitializer.Seed(db, esPlataforma: string.Equals(slug, SlugPlataforma, StringComparison.OrdinalIgnoreCase));
        return db;
    }

    public static string RutaDb(string slug)
    {
        var dir = Path.Combine(AppConfig.ResolverRuta(AppConfig.DataRuta), "tenants", slug.ToLowerInvariant());
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "tallergo.db");
    }

    /// Arranque: crea el catálogo de tenants y las bases de sistema
    /// (plataforma "admin"; taller demo "tallergo" cuando SeedDemoData).
    public static void Bootstrap()
    {
        CatalogDb.AsegurarEsquema();
        AsegurarTenantSistema(SlugPlataforma, "TallerGo Plataforma");
        if (AppConfig.SeedDemoData)
            AsegurarTenantSistema(SlugDemo, "Taller de Prueba");
    }

    private static void AsegurarTenantSistema(string slug, string nombre)
    {
        if (!CatalogDb.ExisteSlug(slug))
            CatalogDb.Crear(new Tenant
            {
                Id = $"ten-{slug}",
                Slug = slug,
                Nombre = nombre,
                Estado = Tenant.EstadoActivo,
                CreadoEn = DateTime.Now,
            });
        OpenParaSesion(slug);
    }
}