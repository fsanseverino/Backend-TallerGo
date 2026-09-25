using Backend_TallerGo.Data;
using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var db = AppDb.Open();
        var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == request.Usuario);
        if (usuario is null || !PasswordHasher.Verificar(request.Password, usuario.PasswordHash, usuario.Sal))
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

        if (usuario.Estado != EstadoUsuario.ACTIVO)
            return Unauthorized(new { mensaje = "Tu usuario está desactivado. Contactá al administrador." });

        var rol = db.Roles.Find(usuario.RolId);
        var nombreRol = rol?.Nombre ?? "OPERADOR";
        var permisos = string.Equals(nombreRol, "ADMIN", StringComparison.OrdinalIgnoreCase)
            ? PermisosCatalog.Todos
            : DeserializarPermisos(rol?.Permisos);

        var expira = DateTime.UtcNow.AddHours(24);
        var token = AuthToken.Generar(expira, usuario.Id, nombreRol, permisos);

        return Ok(new
        {
            token,
            expira,
            usuarioId = usuario.Id,
            usuario = usuario.NombreUsuario,
            nombre = ObtenerNombre(db, usuario),
            rol = nombreRol,
            permisos,
            debeCambiarPassword = usuario.DebeCambiarPassword,
        });
    }

    private static string[] DeserializarPermisos(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return Array.Empty<string>();
        try
        {
            return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
        }
        catch (Exception)
        {
            return Array.Empty<string>();
        }
    }

    private static string ObtenerNombre(TallerGoDbContext db, Usuario usuario)
    {
        if (!string.IsNullOrWhiteSpace(usuario.EmpleadoId))
        {
            var empleado = db.Empleados.Find(usuario.EmpleadoId);
            if (empleado is not null)
                return $"{empleado.Nombre} {empleado.Apellido}".Trim();
        }
        return usuario.NombreUsuario;
    }
}