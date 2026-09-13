using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresupuestosController : ControllerBase
{
    private static async Task<Presupuesto?> Cargar(TallerGoDbContext db, string id)
    {
        var presupuesto = await db.Presupuestos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (presupuesto is null)
            return null;

        presupuesto.Items = await db.PresupuestoItems.AsNoTracking().Where(i => i.PresupuestoId == id).ToListAsync();
        return presupuesto;
    }

    private static async Task<List<Presupuesto>> CargarTodos(TallerGoDbContext db)
    {
        var lista = await db.Presupuestos.AsNoTracking().OrderByDescending(p => p.Fecha).ToListAsync();
        foreach (var p in lista)
        {
            p.Items = await db.PresupuestoItems.AsNoTracking().Where(i => i.PresupuestoId == p.Id).ToListAsync();
        }
        return lista;
    }

    private static async Task<string> SiguienteId(TallerGoDbContext db)
    {
        var prefijo = await db.Configuraciones.FirstOrDefaultAsync(c => c.Clave == "presupuestos.prefijo");
        var siguiente = await db.Configuraciones.FirstOrDefaultAsync(c => c.Clave == "presupuestos.siguiente");
        var p = prefijo?.Valor?.Trim();
        var id = string.IsNullOrWhiteSpace(p) ? "PRE-" : p;

        if (siguiente is null)
            return id + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();

        var numero = int.TryParse(siguiente.Valor, out var n) ? n : 1;
        siguiente.Valor = (numero + 1).ToString();
        siguiente.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return id + numero.ToString("D4");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await CargarTodos(db));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var presupuesto = await Cargar(db, id);
        return presupuesto is null ? NotFound() : Ok(presupuesto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Presupuesto presupuesto)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(presupuesto.Id) || presupuesto.Id.StartsWith("PRE-") == false)
            presupuesto.Id = await SiguienteId(db);
        foreach (var item in presupuesto.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = Guid.NewGuid().ToString();
            item.PresupuestoId = presupuesto.Id;
        }

        db.Presupuestos.Add(presupuesto);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = presupuesto.Id }, await Cargar(db, presupuesto.Id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Presupuesto datos)
    {
        var db = AppDb.Open();
        var presupuesto = await db.Presupuestos.FindAsync(id);
        if (presupuesto is null)
            return NotFound();

        presupuesto.ClienteId = datos.ClienteId;
        presupuesto.VehiculoId = datos.VehiculoId;
        presupuesto.EmpleadoId = datos.EmpleadoId;
        presupuesto.Descripcion = datos.Descripcion;
        presupuesto.Fecha = datos.Fecha;
        presupuesto.FechaVencimiento = datos.FechaVencimiento;
        presupuesto.Estado = datos.Estado;
        presupuesto.Monto = datos.Monto;
        presupuesto.Observaciones = datos.Observaciones;

        var viejos = await db.PresupuestoItems.AsNoTracking().Where(i => i.PresupuestoId == id).ToListAsync();
        foreach (var v in viejos)
            db.PresupuestoItems.Remove(v);
        foreach (var item in datos.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = Guid.NewGuid().ToString();
            item.PresupuestoId = id;
            db.PresupuestoItems.Add(item);
        }

        await db.SaveChangesAsync();
        return Ok(await Cargar(db, id));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var presupuesto = await db.Presupuestos.FindAsync(id);
        if (presupuesto is null)
            return NotFound();
        if (presupuesto.Estado == EstadoPresupuesto.CONVERTIDO)
            return Conflict("El presupuesto ya fue convertido en una orden de trabajo.");

        db.Presupuestos.Remove(presupuesto);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/convertir")]
    public async Task<IActionResult> Convertir(string id)
    {
        var db = AppDb.Open();
        var presupuesto = await db.Presupuestos.FindAsync(id);
        if (presupuesto is null)
            return NotFound();
        if (presupuesto.Estado == EstadoPresupuesto.CONVERTIDO)
            return Conflict("El presupuesto ya fue convertido en una orden de trabajo.");

        presupuesto.Items = await db.PresupuestoItems.AsNoTracking().Where(i => i.PresupuestoId == id).ToListAsync();

        var trabajo = new Trabajo
        {
            VehiculoId = presupuesto.VehiculoId,
            ClienteId = presupuesto.ClienteId,
            EmpleadoId = presupuesto.EmpleadoId,
            Descripcion = presupuesto.Descripcion,
            FechaIngreso = DateTime.Now,
            Estado = EstadoTrabajo.SIN_INICIAR,
            Monto = presupuesto.Monto,
            Observaciones = $"Presupuesto {presupuesto.Id}." + (string.IsNullOrWhiteSpace(presupuesto.Observaciones) ? "" : " " + presupuesto.Observaciones),
        };

        // Mismo correlativo que la OT (trabajos.prefijo / trabajos.siguiente).
        var prefijo = await db.Configuraciones.FirstOrDefaultAsync(c => c.Clave == "trabajos.prefijo");
        if (prefijo != null && !string.IsNullOrWhiteSpace(prefijo.Valor))
        {
            var siguiente = await db.Configuraciones.FirstOrDefaultAsync(c => c.Clave == "trabajos.siguiente");
            var numero = int.TryParse(siguiente?.Valor, out var n) ? n : 1;
            trabajo.Id = prefijo.Valor.Trim() + numero.ToString("D4");
            if (siguiente != null)
            {
                siguiente.Valor = (numero + 1).ToString();
                siguiente.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            trabajo.Id = Guid.NewGuid().ToString();
        }

        foreach (var item in presupuesto.Items)
        {
            trabajo.Items.Add(new TrabajoItem
            {
                Id = Guid.NewGuid().ToString(),
                TrabajoId = trabajo.Id,
                Descripcion = item.Descripcion,
                Tipo = item.Tipo,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                RepuestoId = item.RepuestoId,
            });
        }

        db.Trabajos.Add(trabajo);
        await db.SaveChangesAsync();

        presupuesto.Estado = EstadoPresupuesto.CONVERTIDO;
        presupuesto.TrabajoId = trabajo.Id;
        await db.SaveChangesAsync();

        return Ok(new { trabajo = await Cargar(db, trabajo.Id), presupuesto = await Cargar(db, id) });
    }
}