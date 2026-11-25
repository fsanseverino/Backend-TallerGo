using Backend_TallerGo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    [HttpGet("health")]
    [RequierePermiso("sistema:ver")]
    public IActionResult Health()
    {
        return Ok(new { estado = "ok", fecha = DateTime.Now });
    }

    [HttpPost("backup")]
    [RequierePermiso("sistema:backup")]
    public async Task<IActionResult> Backup()
    {
        var db = AppDb.Open();
        var carpeta = AppConfig.ResolverRuta(AppConfig.BackupsRuta);
        Directory.CreateDirectory(carpeta);
        var destino = Path.Combine(carpeta, $"tallergo-{DateTime.Now:yyyyMMdd-HHmmss}.db");

        using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = $"VACUUM INTO '{destino.Replace("'", "''")}'";
        db.Database.OpenConnection();
        try
        {
            await cmd.ExecuteNonQueryAsync();
        }
        finally
        {
            db.Database.CloseConnection();
        }

        return Ok(new { destino });
    }
}