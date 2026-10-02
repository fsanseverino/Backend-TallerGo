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
        var (usuario, slug) = SepararUsuarioTaller(request.Usuario);
        if (slug is null)
            return Unauthorized(new { mensaje = "El usuario debe tener el formato usuario@taller (por ejemplo: admin@tallergo)." });

        var tenant = CatalogDb.PorSlug(slug);
        if (tenant is null || tenant.Estado != Tenant.EstadoActivo)
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

        var db = AppDb.OpenParaSesion(slug);
        var usuarioDb = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario);
        if (usuarioDb is null || !PasswordHasher.Verificar(request.Password, usuarioDb.PasswordHash, usuarioDb.Sal))
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

        if (usuarioDb.Estado != EstadoUsuario.ACTIVO)
            return Unauthorized(new { mensaje = "Tu usuario está desactivado. Contactá al administrador." });

        var rol = db.Roles.Find(usuarioDb.RolId);
        var nombreRol = rol?.Nombre ?? "OPERADOR";
        var esPlataforma = string.Equals(slug, AppDb.SlugPlataforma, StringComparison.OrdinalIgnoreCase);
        var permisos = string.Equals(nombreRol, "ADMIN", StringComparison.OrdinalIgnoreCase)
            ? esPlataforma
                ? PermisosCatalog.Todos.Concat(PermisosCatalog.Plataforma).ToArray()
                : PermisosCatalog.Todos
            : DeserializarPermisos(rol?.Permisos);

        var expira = DateTime.UtcNow.AddHours(24);
        var token = AuthToken.Generar(expira, usuarioDb.Id, nombreRol, permisos, slug);

        return Ok(new
        {
            token,
            expira,
            usuarioId = usuarioDb.Id,
            usuario = usuarioDb.NombreUsuario,
            nombre = ObtenerNombre(db, usuarioDb),
            rol = nombreRol,
            permisos,
            debeCambiarPassword = usuarioDb.DebeCambiarPassword,
            slug,
            taller = tenant.Nombre,
        });
    }

    /// Separa "usuario@taller" en (usuario, taller). Sin '@' o slug inválido → taller null.
    public static (string, string?) SepararUsuarioTaller(string entrada)
    {
        var texto = entrada?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(texto))
            return (texto, null);
        var idx = texto.LastIndexOf('@');
        if (idx <= 0 || idx == texto.Length - 1)
            return (texto, null);
        var usuario = texto[..idx];
        var slug = texto[(idx + 1)..];
        if (slug.Length < 2 || slug.IndexOfAny(new[] { '/', '\\', ':', '.' }) >= 0)
            return (texto, null);
        return (usuario, slug);
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