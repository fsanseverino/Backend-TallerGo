using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrabajosController : ControllerBase
{
    private static async Task<Trabajo?> Cargar(TallerGoDbContext db, string id)
    {
        var trabajo = await db.Trabajos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (trabajo is null)
            return null;

        trabajo.Items = await db.TrabajoItems.AsNoTracking().Where(i => i.TrabajoId == id).ToListAsync();
        trabajo.Pagos = await db.PagosTrabajo.AsNoTracking().Where(p => p.TrabajoId == id).ToListAsync();
        return trabajo;
    }

    private static async Task<List<Trabajo>> CargarTodos(TallerGoDbContext db)
    {
        var trabajos = await db.Trabajos.AsNoTracking().OrderBy(t => t.FechaIngreso).ToListAsync();
        foreach (var t in trabajos)
        {
            t.Items = await db.TrabajoItems.AsNoTracking().Where(i => i.TrabajoId == t.Id).ToListAsync();
            t.Pagos = await db.PagosTrabajo.AsNoTracking().Where(p => p.TrabajoId == t.Id).ToListAsync();
        }
        return trabajos;
    }

    private static async Task<List<Trabajo>> CargarPor(TallerGoDbContext db, string clienteId, string vehiculoId)
    {
        var query = db.Trabajos.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(clienteId))
        {
            query = query.Where(t => t.ClienteId == clienteId);
        }
        if (!string.IsNullOrWhiteSpace(vehiculoId))
        {
            query = query.Where(t => t.VehiculoId == vehiculoId);
        }
        var trabajos = await query.ToListAsync();
        foreach (var t in trabajos)
        {
            t.Items = await db.TrabajoItems.AsNoTracking().Where(i => i.TrabajoId == t.Id).ToListAsync();
            t.Pagos = await db.PagosTrabajo.AsNoTracking().Where(p => p.TrabajoId == t.Id).ToListAsync();
        }
        return trabajos;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await CargarTodos(db));
    }

    [HttpGet("por-cliente/{clienteId}")]
    public async Task<IActionResult> ByCliente(string clienteId)
    {
        var db = AppDb.Open();
        return Ok(await CargarPor(db, clienteId, ""));
    }

    [HttpGet("por-vehiculo/{vehiculoId}")]
    public async Task<IActionResult> ByVehiculo(string vehiculoId)
    {
        var db = AppDb.Open();
        return Ok(await CargarPor(db, "", vehiculoId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var trabajo = await Cargar(db, id);
        return trabajo is null ? NotFound() : Ok(trabajo);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Trabajo trabajo)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(trabajo.Id))
            trabajo.Id = Guid.NewGuid().ToString();

        foreach (var item in trabajo.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = Guid.NewGuid().ToString();
            item.TrabajoId = trabajo.Id;
        }
        foreach (var pago in trabajo.Pagos)
        {
            if (string.IsNullOrWhiteSpace(pago.Id))
                pago.Id = Guid.NewGuid().ToString();
            pago.TrabajoId = trabajo.Id;
        }

        db.Trabajos.Add(trabajo);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = trabajo.Id }, await Cargar(db, trabajo.Id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Trabajo datos)
    {
        var db = AppDb.Open();
        var trabajo = await db.Trabajos.FindAsync(id);
        if (trabajo is null)
            return NotFound();

        trabajo.VehiculoId = datos.VehiculoId;
        trabajo.ClienteId = datos.ClienteId;
        trabajo.EmpleadoId = datos.EmpleadoId;
        trabajo.Descripcion = datos.Descripcion;
        trabajo.KilometrajeIngreso = datos.KilometrajeIngreso;
        trabajo.FechaIngreso = datos.FechaIngreso;
        trabajo.FechaRealizacion = datos.FechaRealizacion;
        trabajo.Estado = datos.Estado;
        trabajo.Monto = datos.Monto;
        trabajo.Observaciones = datos.Observaciones;

        // Reemplazar los ítems (los pagos se gestionan por separado).
        var viejos = await db.TrabajoItems.AsNoTracking().Where(i => i.TrabajoId == id).ToListAsync();
        foreach (var v in viejos)
            db.TrabajoItems.Remove(v);

        foreach (var item in datos.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                item.Id = Guid.NewGuid().ToString();
            item.TrabajoId = id;
            db.TrabajoItems.Add(item);
        }

        await db.SaveChangesAsync();
        return Ok(await Cargar(db, id));
    }

    [HttpPost("{id}/pagos")]
    public async Task<IActionResult> RegistrarPago(string id, RegistrarPagoRequest request)
    {
        var db = AppDb.Open();
        var trabajo = await db.Trabajos.FindAsync(id);
        if (trabajo is null)
            return NotFound();

        var pago = new PagoTrabajo
        {
            Id = Guid.NewGuid().ToString(),
            TrabajoId = id,
            Monto = request.Monto,
            Fecha = request.Fecha == default ? DateTime.UtcNow : request.Fecha,
        };
        db.PagosTrabajo.Add(pago);

        MovimientoCaja? movimiento = null;
        if (!string.IsNullOrWhiteSpace(request.CajaId))
        {
            var caja = await db.Cajas.FindAsync(request.CajaId);
            if (caja is null)
                return NotFound("Caja no encontrada.");
            if (caja.Estado != EstadoCaja.ABIERTA)
                return Conflict("No hay una caja abierta para registrar la operación.");

            movimiento = new MovimientoCaja()
            {
                Id = Guid.NewGuid().ToString(),
                CajaId = caja.Id,
                Tipo = TipoMovimiento.INGRESO_TRABAJO,
                Concepto = trabajo.Descripcion,
                Monto = pago.Monto,
                Fecha = pago.Fecha,
                TrabajoId = id,
            };
            pago.MovimientoCajaId = movimiento.Id;
            db.MovimientosCaja.Add(movimiento);
        }

        await db.SaveChangesAsync();
        return Ok(new { trabajo = await Cargar(db, id), movimiento });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var trabajo = await db.Trabajos.FindAsync(id);
        if (trabajo is null)
            return NotFound();

        db.Trabajos.Remove(trabajo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}