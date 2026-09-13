using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TurnosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await db.Turnos.AsNoTracking().OrderBy(t => t.Fecha).ThenBy(t => t.Hora).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var turno = await db.Turnos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        return turno is null ? NotFound() : Ok(turno);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Turno turno)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(turno.Id))
            turno.Id = Guid.NewGuid().ToString();

        db.Turnos.Add(turno);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = turno.Id }, turno);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Turno datos)
    {
        var db = AppDb.Open();
        var turno = await db.Turnos.FindAsync(id);
        if (turno is null)
            return NotFound();

        turno.Fecha = datos.Fecha;
        turno.Hora = datos.Hora;
        turno.ClienteId = datos.ClienteId;
        turno.VehiculoId = datos.VehiculoId;
        turno.EmpleadoId = datos.EmpleadoId;
        turno.Motivo = datos.Motivo;
        turno.Notas = datos.Notas;
        turno.Estado = datos.Estado;
        turno.TrabajoId = datos.TrabajoId;

        await db.SaveChangesAsync();
        return Ok(turno);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var turno = await db.Turnos.FindAsync(id);
        if (turno is null)
            return NotFound();

        db.Turnos.Remove(turno);
        await db.SaveChangesAsync();
        return NoContent();
    }
}