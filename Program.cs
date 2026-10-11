using System.Text.Json;
using System.Text.Json.Serialization;
using Backend_TallerGo;
using Backend_TallerGo.Data;
using Backend_TallerGo.Models;

var builder = WebApplication.CreateBuilder(args);

AppConfig.Cargar(builder.Configuration);

// Servicios necesarios para los controladores y CORS.
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddCors();
builder.Services.AddOpenApi();

var app = builder.Build();

// Crear el esquema SQLite del catálogo y las bases de sistema (una sola vez).
AppDb.Bootstrap();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// CORS: orígenes configurados (por defecto http://localhost:4200 para dev).
var corsOrigins = AppConfig.CorsOrigins;
app.UseCors(policy =>
{
    if (corsOrigins.Count == 0)
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }
    else
    {
        policy.WithOrigins(corsOrigins.ToArray()).AllowAnyMethod().AllowAnyHeader();
    }
});

// Servir el frontend Angular compilado (wwwroot) en el mismo host.
app.UseDefaultFiles();
app.UseStaticFiles();

// Excepciones no controladas → 500 + log en archivo.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Logs.EscribirError(ex);
        if (context.Response.HasStarted)
            throw;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { mensaje = "Error interno del servidor." });
    }
});

// Auth mínima: los endpoints /api requieren Bearer token válido (24 hs), salvo /api/auth.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var esApi = path.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
    if (esApi && !path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
    {
        var header = context.Request.Headers.Authorization.ToString();
        var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header["Bearer ".Length..] : null;
        var sesion = AuthToken.Decodificar(token);
        if (sesion is null || string.IsNullOrWhiteSpace(sesion.Slug))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { mensaje = "Sesión inválida o expirada. Iniciá sesión nuevamente." });
            return;
        }
        var tenant = CatalogDb.PorSlug(sesion.Slug);
        if (tenant is null || tenant.Estado != Tenant.EstadoActivo)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { mensaje = "El taller está suspendido o inactivo. Contactá al administrador de la plataforma." });
            return;
        }
        TenantActual.Fijar(sesion.Slug);
        context.Items[AuthToken.CLAVE_SESION] = sesion;
    }
    try
    {
        await next();
    }
    finally
    {
        TenantActual.Limpiar();
    }
});

app.MapControllers();

// SPA routing: cualquier ruta que no sea /api o archivo, devuelve index.html.
app.MapFallbackToFile("index.html");

app.Run();