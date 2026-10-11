using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend_TallerGo;

/// Exige que el token de la petición tenga el permiso <c>modulo:accion</c>.
/// El rol ADMIN pasa siempre. Requiere que el middleware de auth ya haya
/// validado el token y guardado la sesión en HttpContext.Items.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequierePermiso : Attribute, IAuthorizationFilter
{
    private readonly string _codigo;

    public RequierePermiso(string codigo)
    {
        _codigo = codigo;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var sesion = context.HttpContext.Items[AuthToken.CLAVE_SESION] as SesionActual;
        if (sesion is null)
        {
            context.Result = new UnauthorizedObjectResult(new { mensaje = "Sesión inválida o expirada. Iniciá sesión nuevamente." });
            return;
        }

        if (!sesion.TienePermiso(_codigo))
        {
            context.Result = new ObjectResult(new { mensaje = "No tenés permiso para realizar esta acción." })
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };
        }
    }
}