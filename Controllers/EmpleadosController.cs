using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await db.Empleados.AsNoTracking().OrderBy(e => e.Apellido).ThenBy(e => e.Nombre).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var empleado = await db.Empleados.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return empleado is null ? NotFound() : Ok(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Empleado empleado)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(empleado.Id))
            empleado.Id = Guid.NewGuid().ToString();
        if (empleado.CreatedAt == null)
            empleado.CreatedAt = DateTime.Now;

        db.Empleados.Add(empleado);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = empleado.Id }, empleado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Empleado datos)
    {
        var db = AppDb.Open();
        var empleado = await db.Empleados.FindAsync(id);
        if (empleado is null)
            return NotFound();

        empleado.Nombre = datos.Nombre;
        empleado.Apellido = datos.Apellido;
        empleado.Especialidad = datos.Especialidad;
        empleado.Telefono = datos.Telefono;
        empleado.Email = datos.Email;
        empleado.Direccion = datos.Direccion;
        empleado.FechaIngreso = datos.FechaIngreso;
        empleado.Estado = datos.Estado;

        await db.SaveChangesAsync();
        return Ok(empleado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var empleado = await db.Empleados.FindAsync(id);
        if (empleado is null)
            return NotFound();

        db.Empleados.Remove(empleado);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
