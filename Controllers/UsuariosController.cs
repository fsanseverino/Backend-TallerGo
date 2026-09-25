using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    [HttpGet]
    [RequierePermiso("usuarios:ver")]
    public IActionResult Listar()
    {
        var db = AppDb.Open();
        var items = db.Usuarios
            .Join(db.Roles, u => u.RolId, r => r.Id, (u, r) => new
            {
                u.Id,
                NombreUsuario = u.NombreUsuario,
                u.EmpleadoId,
                empleado = ObtenerEmpleado(db, u.EmpleadoId),
                u.RolId,
                rol = r.Nombre,
                u.Estado,
                u.CreatedAt,
            })
            .OrderBy(x => x.NombreUsuario)
            .ToList();
        return Ok(items);
    }

    [HttpGet("{id}")]
    [RequierePermiso("usuarios:ver")]
    public IActionResult Obtener(string id)
    {
        var db = AppDb.Open();
        var usuario = db.Usuarios.Find(id);
        if (usuario is null)
            return NotFound(new { mensaje = "Usuario no encontrado." });
        return Ok(new
        {
            usuario.Id,
            usuario.NombreUsuario,
            usuario.EmpleadoId,
            usuario.RolId,
            usuario.Estado,
        });
    }

    [HttpPost]
    [RequierePermiso("usuarios:editar")]
    public IActionResult Crear(CrearUsuarioRequest request)
    {
        var db = AppDb.Open();
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return BadRequest(new { mensaje = "El nombre de usuario es obligatorio." });
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });
        if (db.Roles.Find(request.RolId) is null)
            return BadRequest(new { mensaje = "El rol indicado no existe." });
        if (db.Usuarios.Any(u => u.NombreUsuario == request.Usuario))
            return Conflict(new { mensaje = "Ya existe un usuario con ese nombre." });

        var (hash, sal) = PasswordHasher.Hash(request.Password);
        var usuario = new Usuario
        {
            Id = "usu-" + Guid.NewGuid().ToString("N"),
            NombreUsuario = request.Usuario,
            PasswordHash = hash,
            Sal = sal,
            EmpleadoId = string.IsNullOrWhiteSpace(request.EmpleadoId) ? null : request.EmpleadoId,
            RolId = request.RolId,
            Estado = request.Estado,
            CreatedAt = DateTime.Now,
        };
        db.Usuarios.Add(usuario);
        db.SaveChanges();
        return Created($"/api/Usuarios/{usuario.Id}", new { usuario.Id });
    }

    [HttpPut("{id}")]
    [RequierePermiso("usuarios:editar")]
    public IActionResult Actualizar(string id, ActualizarUsuarioRequest request)
    {
        var db = AppDb.Open();
        var usuario = db.Usuarios.Find(id);
        if (usuario is null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        if (string.Equals(usuario.NombreUsuario, "admin", StringComparison.OrdinalIgnoreCase) &&
            (request.RolId != usuario.RolId || request.Estado != EstadoUsuario.ACTIVO))
            return BadRequest(new { mensaje = "El usuario admin no puede cambiar de rol ni desactivarse." });

        if (!string.IsNullOrWhiteSpace(request.RolId) && request.RolId != usuario.RolId)
        {
            if (db.Roles.Find(request.RolId) is null)
                return BadRequest(new { mensaje = "El rol indicado no existe." });
            usuario.RolId = request.RolId;
        }
        if (request.Estado.HasValue)
            usuario.Estado = request.Estado.Value;
        if (request.EmpleadoId != usuario.EmpleadoId)
            usuario.EmpleadoId = string.IsNullOrWhiteSpace(request.EmpleadoId) ? null : request.EmpleadoId;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 6)
                return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });
            var (hash, sal) = PasswordHasher.Hash(request.Password);
            usuario.PasswordHash = hash;
            usuario.Sal = sal;
        }

        db.SaveChanges();
        return Ok(new { mensaje = "Usuario actualizado." });
    }

    [HttpPost("{id}/resetear-password")]
    [RequierePermiso("usuarios:editar")]
    public IActionResult ResetearPassword(string id)
    {
        var db = AppDb.Open();
        var usuario = db.Usuarios.Find(id);
        if (usuario is null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        var (hash, sal) = PasswordHasher.Hash("123456");
        usuario.PasswordHash = hash;
        usuario.Sal = sal;
        usuario.DebeCambiarPassword = true;
        db.SaveChanges();
        return Ok(new { mensaje = "Contraseña restablecida a 123456. Deberá cambiarla al ingresar." });
    }

    [HttpPost("{id}/cambiar-password")]
    public IActionResult CambiarPassword(string id, CambiarPasswordRequest request)
    {
        var db = AppDb.Open();
        var sesion = HttpContext.Items[AuthToken.CLAVE_SESION] as SesionActual;
        var esPropio = sesion is not null && sesion.UsuarioId == id;
        var esAdmin = sesion is not null && sesion.TienePermiso("usuarios:editar");
        if (!esPropio && !esAdmin)
            return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = "No podés cambiar la contraseña de otro usuario." });

        var usuario = db.Usuarios.Find(id);
        if (usuario is null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { mensaje = "La contraseña debe tener al menos 6 caracteres." });

        var (hash, sal) = PasswordHasher.Hash(request.Password);
        usuario.PasswordHash = hash;
        usuario.Sal = sal;
        usuario.DebeCambiarPassword = false;
        db.SaveChanges();
        return Ok(new { mensaje = "Contraseña actualizada." });
    }

    [HttpDelete("{id}")]
    [RequierePermiso("usuarios:editar")]
    public IActionResult Eliminar(string id)
    {
        var db = AppDb.Open();
        var usuario = db.Usuarios.Find(id);
        if (usuario is null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        if (string.Equals(usuario.NombreUsuario, "admin", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "El usuario admin no puede eliminarse." });

        db.Usuarios.Remove(usuario);
        db.SaveChanges();
        return Ok(new { mensaje = "Usuario eliminado." });
    }

    private static string? ObtenerEmpleado(TallerGoDbContext db, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;
        var empleado = db.Empleados.Find(id);
        return empleado is null ? null : $"{empleado.Nombre} {empleado.Apellido}".Trim();
    }
}