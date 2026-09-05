using Backend_TallerGo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend_TallerGo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        if (request.Usuario != "admin" || request.Password != "123456")
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

        var expira = DateTime.UtcNow.AddHours(24);
        return Ok(new { token = AuthToken.Generar(expira), expira });
    }
}