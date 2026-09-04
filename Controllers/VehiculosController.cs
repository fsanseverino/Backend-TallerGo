using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await db.Vehiculos.AsNoTracking().OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToListAsync());
    }

    [HttpGet("por-cliente/{clienteId}")]
    public async Task<IActionResult> ByCliente(string clienteId)
    {
        var db = AppDb.Open();
        return Ok(await db.Vehiculos.AsNoTracking().Where(v => v.ClienteId == clienteId).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var vehiculo = await db.Vehiculos.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        return vehiculo is null ? NotFound() : Ok(vehiculo);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Vehiculo vehiculo)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(vehiculo.Id))
            vehiculo.Id = Guid.NewGuid().ToString();

        db.Vehiculos.Add(vehiculo);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = vehiculo.Id }, vehiculo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Vehiculo datos)
    {
        var db = AppDb.Open();
        var vehiculo = await db.Vehiculos.FindAsync(id);
        if (vehiculo is null)
            return NotFound();

        vehiculo.ClienteId = datos.ClienteId;
        vehiculo.Marca = datos.Marca;
        vehiculo.Modelo = datos.Modelo;
        vehiculo.Anio = datos.Anio;
        vehiculo.Patente = datos.Patente;
        vehiculo.Color = datos.Color;
        vehiculo.Chasis = datos.Chasis;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var vehiculo = await db.Vehiculos.FindAsync(id);
        if (vehiculo is null)
            return NotFound();

        db.Vehiculos.Remove(vehiculo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}