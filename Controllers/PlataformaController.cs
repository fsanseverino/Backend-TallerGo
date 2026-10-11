using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Backend_TallerGo.Controllers;

/// Gestión de talleres (tenants) desde la plataforma. Solo accesible por el
/// superadministrador (sesión con slug "admin").
[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private static readonly Regex SlugValido = new("^[a-z0-9][a-z0-9-]{1,29}$", RegexOptions.Compiled);

    [HttpGet]
    [RequierePermiso("plataforma:ver")]
    public IActionResult Listar()
    {
        if (!EsPlataforma())
            return NoTieneAcceso();
        return Ok(CatalogDb.Listar());
    }

    [HttpPost]
    [RequierePermiso("plataforma:crear")]
    public IActionResult Crear(CrearTenantRequest request)
    {
        if (!EsPlataforma())
            return NoTieneAcceso();
        var slug = request.Slug?.Trim().ToLowerInvariant() ?? "";
        if (!SlugValido.IsMatch(slug))
            return BadRequest(new { mensaje = "El slug debe tener entre 2 y 30 caracteres (minúsculas, números y guiones)." });
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { mensaje = "El nombre del taller es obligatorio." });
        if (CatalogDb.ExisteSlug(slug))
            return Conflict(new { mensaje = $"Ya existe un taller con el slug '{slug}'." });

        var tenant = new Tenant
        {
            Id = $"ten-{Guid.NewGuid():N}",
            Slug = slug,
            Nombre = request.Nombre.Trim(),
            Estado = Tenant.EstadoActivo,
            CreadoEn = DateTime.Now,
            Email = request.Email?.Trim(),
            Telefono = request.Telefono?.Trim(),
        };
        CatalogDb.Crear(tenant);

        // Crea la base del taller con roles + admin (contraseña inicial del entorno).
        AppDb.OpenParaSesion(slug);

        return CreatedAtAction(nameof(Listar), new { }, tenant);
    }

    [HttpPut("{id}")]
    [RequierePermiso("plataforma:editar")]
    public IActionResult Actualizar(string id, ActualizarTenantRequest request)
    {
        if (!EsPlataforma())
            return NoTieneAcceso();
        var tenant = Buscar(id);
        if (tenant is null)
            return NotFound();
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { mensaje = "El nombre del taller es obligatorio." });
        CatalogDb.Actualizar(id, request.Nombre.Trim(), request.Email?.Trim(), request.Telefono?.Trim());
        return Ok(Buscar(id));
    }

    [HttpPut("{id}/estado")]
    [RequierePermiso("plataforma:editar")]
    public IActionResult CambiarEstado(string id, CambiarEstadoTenantRequest request)
    {
        if (!EsPlataforma())
            return NoTieneAcceso();
        var tenant = Buscar(id);
        if (tenant is null)
            return NotFound();
        if (request.Estado != Tenant.EstadoActivo && request.Estado != Tenant.EstadoSuspendido)
            return BadRequest(new { mensaje = "Estado inválido. Use ACTIVO o SUSPENDIDO." });
        CatalogDb.CambiarEstado(id, request.Estado);
        return Ok(Buscar(id));
    }

    [HttpPost("{id}/reset-password")]
    [RequierePermiso("plataforma:editar")]
    public IActionResult ResetPassword(string id, ResetPasswordRequest request)
    {
        if (!EsPlataforma())
            return NoTieneAcceso();
        var tenant = Buscar(id);
        if (tenant is null)
            return NotFound();
        if (request.Password is null || request.Password.Length < 4)
            return BadRequest(new { mensaje = "La contraseña debe tener al menos 4 caracteres." });

        var db = AppDb.OpenParaSesion(tenant.Slug);
        var admin = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == "admin");
        if (admin is null)
            return NotFound(new { mensaje = "El taller no tiene usuario admin." });
        var (hash, sal) = PasswordHasher.Hash(request.Password);
        admin.PasswordHash = hash;
        admin.Sal = sal;
        admin.DebeCambiarPassword = true;
        db.SaveChanges();

        return Ok(new { mensaje = $"Contraseña de admin en '{tenant.Slug}' actualizada." });
    }

    /// Busca por id o por slug (acepta cualquiera de los dos).
    private Tenant? Buscar(string refId) => CatalogDb.Listar().FirstOrDefault(t => t.Id == refId || t.Slug == refId);

    private bool EsPlataforma()
    {
        var sesion = HttpContext.Items[AuthToken.CLAVE_SESION] as SesionActual;
        return sesion is not null && string.Equals(sesion.Slug, AppDb.SlugPlataforma, StringComparison.OrdinalIgnoreCase);
    }

    private ObjectResult NoTieneAcceso() =>
        new(new { mensaje = "Solo el administrador de la plataforma puede realizar esta acción." })
        {
            StatusCode = StatusCodes.Status403Forbidden,
        };
}

public class CrearTenantRequest
{
    public string? Slug { get; set; }
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}

public class ActualizarTenantRequest
{
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}

public class CambiarEstadoTenantRequest
{
    public string? Estado { get; set; }
}

public class ResetPasswordRequest
{
    public string? Password { get; set; }
}