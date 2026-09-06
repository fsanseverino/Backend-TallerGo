using Backend_TallerGo.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Data;

public class TallerGoDbContext : DbContext
{
    public TallerGoDbContext(DbContextOptions<TallerGoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Trabajo> Trabajos => Set<Trabajo>();
    public DbSet<TrabajoItem> TrabajoItems => Set<TrabajoItem>();
    public DbSet<PagoTrabajo> PagosTrabajo => Set<PagoTrabajo>();
    public DbSet<Caja> Cajas => Set<Caja>();
    public DbSet<MovimientoCaja> MovimientosCaja => Set<MovimientoCaja>();
    public DbSet<Configuracion> Configuraciones => Set<Configuracion>();
    public DbSet<Catalogo> Catalogos => Set<Catalogo>();
    public DbSet<CatalogoValor> CatalogoValores => Set<CatalogoValor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Use client-generated string ids (keep it drop-in compatible with the Angular app).
        foreach (var type in new[]
        {
            typeof(Cliente), typeof(Vehiculo), typeof(Empleado), typeof(Trabajo),
            typeof(TrabajoItem), typeof(PagoTrabajo), typeof(Caja), typeof(MovimientoCaja),
            typeof(Configuracion), typeof(Catalogo), typeof(CatalogoValor)
        })
        {
            modelBuilder.Entity(type).Property("Id").ValueGeneratedNever();
        }

        var cliente = modelBuilder.Entity<Cliente>();
        cliente.HasKey(c => c.Id);
        cliente.Property(c => c.TipoDocumento).HasConversion<string>().HasMaxLength(20);
        cliente.Property(c => c.Documento).HasMaxLength(50);

        var vehiculo = modelBuilder.Entity<Vehiculo>();
        vehiculo.HasKey(v => v.Id);
        vehiculo.Property(v => v.TipoMotor).HasConversion<string>().HasMaxLength(20);
        vehiculo.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        var empleado = modelBuilder.Entity<Empleado>();
        empleado.HasKey(e => e.Id);
        empleado.Property(e => e.Especialidad).HasConversion<string>().HasMaxLength(20);
        empleado.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);

        var trabajo = modelBuilder.Entity<Trabajo>();
        trabajo.HasKey(t => t.Id);
        trabajo.Property(t => t.Estado).HasConversion<string>().HasMaxLength(30);
        trabajo.HasOne<Vehiculo>()
            .WithMany()
            .HasForeignKey(t => t.VehiculoId)
            .OnDelete(DeleteBehavior.Restrict);
        trabajo.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
        trabajo.HasOne<Empleado>()
            .WithMany()
            .HasForeignKey(t => t.EmpleadoId)
            .OnDelete(DeleteBehavior.SetNull);

        var item = modelBuilder.Entity<TrabajoItem>();
        item.HasKey(i => i.Id);
        item.Property(i => i.Tipo).HasConversion<string>().HasMaxLength(20);
        item.HasOne<Trabajo>()
            .WithMany()
            .HasForeignKey(i => i.TrabajoId)
            .OnDelete(DeleteBehavior.Cascade);

        var pago = modelBuilder.Entity<PagoTrabajo>();
        pago.HasKey(p => p.Id);
        pago.HasOne<Trabajo>()
            .WithMany(t => t.Pagos)
            .HasForeignKey(p => p.TrabajoId)
            .OnDelete(DeleteBehavior.Cascade);

        var caja = modelBuilder.Entity<Caja>();
        caja.HasKey(c => c.Id);
        caja.Property(c => c.Estado).HasConversion<string>().HasMaxLength(20);

        var mov = modelBuilder.Entity<MovimientoCaja>();
        mov.HasKey(m => m.Id);
        mov.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(30);
        mov.HasOne<Caja>()
            .WithMany()
            .HasForeignKey(m => m.CajaId)
            .OnDelete(DeleteBehavior.Cascade);

        var config = modelBuilder.Entity<Configuracion>();
        config.HasKey(c => c.Id);
        config.HasIndex(c => c.Clave).IsUnique();
        config.Property(c => c.Clave).HasMaxLength(100);
        config.Property(c => c.Categoria).HasMaxLength(50);

        var cat = modelBuilder.Entity<Catalogo>();
        cat.HasKey(c => c.Id);
        cat.HasIndex(c => c.Clave).IsUnique();
        cat.Property(c => c.Nombre).HasMaxLength(100);
        cat.Property(c => c.Clave).HasMaxLength(50);

        var catVal = modelBuilder.Entity<CatalogoValor>();
        catVal.HasKey(v => v.Id);
        catVal.HasOne<Catalogo>()
            .WithMany()
            .HasForeignKey(v => v.CatalogoId)
            .OnDelete(DeleteBehavior.Cascade);
        catVal.Property(v => v.Valor).HasMaxLength(200);
    }
}