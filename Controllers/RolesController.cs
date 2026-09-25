using Backend_TallerGo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    [HttpGet]
    [RequierePermiso("usuarios:ver")]
    public IActionResult Listar()
    {
        var db = AppDb.Open();
        var roles = db.Roles
            .OrderBy(r => r.Nombre)
            .Select(r => new
            {
                r.Id,
                r.Nombre,
                r.Descripcion,
                r.Permisos,
            })
            .ToList();
        return Ok(roles);
    }

    [HttpPost]
    [RequierePermiso("usuarios:editar")]
    public IActionResult Crear(RolRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { mensaje = "El nombre del rol es obligatorio." });
        if (string.IsNullOrWhiteSpace(request.Permisos))
            return BadRequest(new { mensaje = "Debe indicarse al menos el JSON de permisos." });

        var db = AppDb.Open();
        var nombre = request.Nombre.Trim();
        if (db.Roles.Any(r => r.Nombre == nombre))
            return Conflict(new { mensaje = "Ya existe un rol con ese nombre." });

        var rol = new Models.Rol
        {
            Id = "rol-" + Guid.NewGuid().ToString("N"),
            Nombre = nombre,
            Descripcion = request.Descripcion?.Trim() ?? string.Empty,
            Permisos = request.Permisos,
        };
        db.Roles.Add(rol);
        db.SaveChanges();
        return Created($"/api/Roles/{rol.Id}", new { rol.Id, rol.Nombre, rol.Descripcion, rol.Permisos });
    }

    [HttpPut("{id}")]
    [RequierePermiso("usuarios:editar")]
    public IActionResult Actualizar(string id, RolRequest request)
    {
        var db = AppDb.Open();
        var rol = db.Roles.Find(id);
        if (rol is null)
            return NotFound(new { mensaje = "Rol no encontrado." });

        if (string.Equals(rol.Nombre, "ADMIN", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "El rol ADMIN no puede editarse." });

        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            var nombre = request.Nombre.Trim();
            if (db.Roles.Any(r => r.Nombre == nombre && r.Id != id))
                return Conflict(new { mensaje = "Ya existe un rol con ese nombre." });
            rol.Nombre = nombre;
        }
        if (request.Descripcion != null)
            rol.Descripcion = request.Descripcion.Trim();
        if (!string.IsNullOrWhiteSpace(request.Permisos))
            rol.Permisos = request.Permisos;

        db.SaveChanges();
        return Ok(new { rol.Id, rol.Nombre, rol.Descripcion, rol.Permisos });
    }
}

public class RolRequest
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Permisos { get; set; }
}