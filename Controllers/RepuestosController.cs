using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepuestosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var db = AppDb.Open();
        return Ok(await db.Repuestos.AsNoTracking().OrderBy(r => r.Descripcion).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var repuesto = await db.Repuestos.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return repuesto is null ? NotFound() : Ok(repuesto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Repuesto repuesto)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(repuesto.Id))
            repuesto.Id = Guid.NewGuid().ToString();

        db.Repuestos.Add(repuesto);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = repuesto.Id }, repuesto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Repuesto datos)
    {
        var db = AppDb.Open();
        var repuesto = await db.Repuestos.FindAsync(id);
        if (repuesto is null)
            return NotFound();

        repuesto.Codigo = datos.Codigo;
        repuesto.Descripcion = datos.Descripcion;
        repuesto.Categoria = datos.Categoria;
        repuesto.Marca = datos.Marca;
        repuesto.Costo = datos.Costo;
        repuesto.PrecioVenta = datos.PrecioVenta;
        repuesto.Proveedor = datos.Proveedor;
        repuesto.StockMinimo = datos.StockMinimo;
        repuesto.Stock = datos.Stock;

        await db.SaveChangesAsync();
        return Ok(repuesto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var db = AppDb.Open();
        var repuesto = await db.Repuestos.FindAsync(id);
        if (repuesto is null)
            return NotFound();

        db.Repuestos.Remove(repuesto);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/stock")]
    public async Task<IActionResult> AjustarStock(string id, AjusteStockRequest request)
    {
        var db = AppDb.Open();
        var repuesto = await db.Repuestos.FindAsync(id);
        if (repuesto is null)
            return NotFound();

        var cantidad = request.Cantidad > 0 ? request.Cantidad : 1;
        repuesto.Stock = request.Tipo.Equals("SALIDA", StringComparison.OrdinalIgnoreCase)
            ? Math.Max(0, repuesto.Stock - cantidad)
            : repuesto.Stock + cantidad;

        await db.SaveChangesAsync();
        return Ok(repuesto);
    }
}