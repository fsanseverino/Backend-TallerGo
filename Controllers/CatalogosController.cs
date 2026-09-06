using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var db = AppDb.Open();
        var catalogos = await db.Catalogos.AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        return Ok(catalogos);
    }

    [HttpGet("{clave}/valores")]
    public async Task<IActionResult> ValoresPorClave(string clave)
    {
        var db = AppDb.Open();
        var catalogo = await db.Catalogos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Clave == clave);
        if (catalogo is null)
            return NotFound();

        var valores = await db.CatalogoValores.AsNoTracking()
            .Where(v => v.CatalogoId == catalogo.Id)
            .OrderBy(v => v.Orden)
            .ThenBy(v => v.Valor)
            .ToListAsync();
        return Ok(valores);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var db = AppDb.Open();
        var catalogo = await db.Catalogos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        if (catalogo is null)
            return NotFound();
        return Ok(catalogo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CatalogoCrear body)
    {
        if (string.IsNullOrWhiteSpace(body.Nombre) || string.IsNullOrWhiteSpace(body.Clave))
            return BadRequest("Nombre y Clave son requeridos.");

        var db = AppDb.Open();
        var existe = await db.Catalogos.AnyAsync(c => c.Clave == body.Clave);
        if (existe)
            return BadRequest("Ya existe un catálogo con esa clave.");

        var catalogo = new Catalogo
        {
            Id = Guid.NewGuid().ToString(),
            Nombre = body.Nombre.Trim(),
            Clave = body.Clave.Trim().ToLower(),
            Descripcion = body.Descripcion?.Trim(),
        };
        db.Catalogos.Add(catalogo);
        await db.SaveChangesAsync();
        return Ok(catalogo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(string id)
    {
        var db = AppDb.Open();
        var catalogo = await db.Catalogos.FirstOrDefaultAsync(c => c.Id == id);
        if (catalogo is null)
            return NotFound();

        db.Catalogos.Remove(catalogo);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{catalogoId}/valores")]
    public async Task<IActionResult> AgregarValor(string catalogoId, [FromBody] CatalogoValorCrear body)
    {
        if (string.IsNullOrWhiteSpace(body.Valor))
            return BadRequest("El valor es requerido.");

        var db = AppDb.Open();
        var catalogo = await db.Catalogos.FirstOrDefaultAsync(c => c.Id == catalogoId);
        if (catalogo is null)
            return NotFound("Catálogo no encontrado.");

        var valores = await db.CatalogoValores
            .Where(v => v.CatalogoId == catalogoId)
            .Select(v => (int?)v.Orden)
            .MaxAsync();
        var maxOrden = valores ?? 0;

        var valor = new CatalogoValor
        {
            Id = Guid.NewGuid().ToString(),
            CatalogoId = catalogoId,
            Valor = body.Valor.Trim(),
            Orden = maxOrden + 1,
        };
        db.CatalogoValores.Add(valor);
        await db.SaveChangesAsync();
        return Ok(valor);
    }

    [HttpDelete("valores/{id}")]
    public async Task<IActionResult> EliminarValor(string id)
    {
        var db = AppDb.Open();
        var valor = await db.CatalogoValores.FirstOrDefaultAsync(v => v.Id == id);
        if (valor is null)
            return NotFound();

        db.CatalogoValores.Remove(valor);
        await db.SaveChangesAsync();
        return NoContent();
    }
}

public class CatalogoCrear
{
    public string Nombre { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class CatalogoValorCrear
{
    public string Valor { get; set; } = string.Empty;
}
