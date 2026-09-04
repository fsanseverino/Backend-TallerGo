using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CajasController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await db.Cajas.AsNoTracking().OrderByDescending(c => c.FechaApertura).ToListAsync());
    }

    [HttpGet("abierta")]
    public async Task<IActionResult> Abierta()
    {
        var db = AppDb.Open();
        var caja = await db.Cajas.AsNoTracking().Where(c => c.Estado == EstadoCaja.ABIERTA).OrderByDescending(c => c.FechaApertura).FirstOrDefaultAsync();
        if (caja is null)
            return NoContent();
        return Ok(caja);
    }

    [HttpGet("{id}/movimientos")]
    public async Task<IActionResult> Movimientos(string id)
    {
        var db = AppDb.Open();
        return Ok(await db.MovimientosCaja.AsNoTracking().Where(m => m.CajaId == id).OrderByDescending(m => m.Fecha).ToListAsync());
    }

    [HttpGet("todos/movimientos")]
    public async Task<IActionResult> TodosLosMovimientos()
    {
        var db = AppDb.Open();
        return Ok(await db.MovimientosCaja.AsNoTracking().OrderByDescending(m => m.Fecha).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Abrir(Caja caja)
    {
        var db = AppDb.Open();
        var existente = await db.Cajas.AsNoTracking().Where(c => c.Estado == EstadoCaja.ABIERTA).FirstOrDefaultAsync();
        if (existente is not null)
            return Conflict("Ya existe una caja abierta.");

        if (string.IsNullOrWhiteSpace(caja.Id))
            caja.Id = Guid.NewGuid().ToString();
        if (caja.FechaApertura == default)
            caja.FechaApertura = DateTime.UtcNow;
        caja.Estado = EstadoCaja.ABIERTA;
        db.Cajas.Add(caja);
        await db.SaveChangesAsync();
        return Ok(caja);
    }

    [HttpPost("{id}/cierre")]
    public async Task<IActionResult> Cerrar(string id)
    {
        var db = AppDb.Open();
        var caja = await db.Cajas.FindAsync(id);
        if (caja is null)
            return NotFound();
        if (caja.Estado == EstadoCaja.CERRADA)
            return Conflict("La caja ya está cerrada.");

        caja.Estado = EstadoCaja.CERRADA;
        caja.FechaCierre = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/movimientos")]
    public async Task<IActionResult> AgregarMovimiento(string id, MovimientoCaja movimiento)
    {
        var db = AppDb.Open();
        var caja = await db.Cajas.FindAsync(id);
        if (caja is null)
            return NotFound();
        if (caja.Estado != EstadoCaja.ABIERTA)
            return Conflict("No hay una caja abierta para registrar la operación.");

        if (string.IsNullOrWhiteSpace(movimiento.Id))
            movimiento.Id = Guid.NewGuid().ToString();
        movimiento.CajaId = id;
        if (movimiento.Fecha == default)
            movimiento.Fecha = DateTime.UtcNow;
        db.MovimientosCaja.Add(movimiento);
        await db.SaveChangesAsync();
        return Ok(movimiento);
    }

    [HttpDelete("movimientos/{id}")]
    public async Task<IActionResult> EliminarMovimiento(string id)
    {
        var db = AppDb.Open();
        var movimiento = await db.MovimientosCaja.FindAsync(id);
        if (movimiento is null)
            return NotFound();

        db.MovimientosCaja.Remove(movimiento);
        await db.SaveChangesAsync();
        return NoContent();
    }
}