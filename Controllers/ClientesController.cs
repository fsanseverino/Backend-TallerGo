using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await db.Clientes.AsNoTracking().OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var cliente = await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(cliente.Id))
            cliente.Id = Guid.NewGuid().ToString();
        if (cliente.CreatedAt == null)
            cliente.CreatedAt = DateTime.UtcNow;

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Cliente datos)
    {
        var db = AppDb.Open();
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound();

        cliente.Nombre = datos.Nombre;
        cliente.Apellido = datos.Apellido;
        cliente.TipoDocumento = datos.TipoDocumento;
        cliente.Documento = datos.Documento;
        cliente.Telefono = datos.Telefono;
        cliente.Email = datos.Email;
        cliente.Direccion = datos.Direccion;

        await db.SaveChangesAsync();
        return Ok(cliente);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound();

        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync();
        return NoContent();
    }
}