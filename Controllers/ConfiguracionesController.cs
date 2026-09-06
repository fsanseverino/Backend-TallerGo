using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfiguracionesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var db = AppDb.Open();
        var configs = await db.Configuraciones.AsNoTracking()
            .OrderBy(c => c.Categoria)
            .ThenBy(c => c.Clave)
            .ToListAsync();
        return Ok(configs);
    }

    [HttpPut]
    public async Task<IActionResult> Upsert([FromBody] List<ConfiguracionUpdate> entradas)
    {
        var db = AppDb.Open();
        var ahora = DateTime.Now;
        foreach (var entrada in entradas)
        {
            if (string.IsNullOrWhiteSpace(entrada.Clave))
                continue;

            var config = await db.Configuraciones.FirstOrDefaultAsync(c => c.Clave == entrada.Clave);
            if (config is null)
            {
                db.Configuraciones.Add(new Configuracion
                {
                    Id = Guid.NewGuid().ToString(),
                    Clave = entrada.Clave,
                    Valor = entrada.Valor,
                    Categoria = "general",
                    UpdatedAt = ahora,
                });
            }
            else if (config.Valor != entrada.Valor)
            {
                config.Valor = entrada.Valor;
                config.UpdatedAt = ahora;
            }
        }
        await db.SaveChangesAsync();

        var actualizadas = await db.Configuraciones.AsNoTracking()
            .OrderBy(c => c.Categoria)
            .ThenBy(c => c.Clave)
            .ToListAsync();
        return Ok(actualizadas);
    }
}