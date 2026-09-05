using System.Text.Json;
using System.Text.Json.Serialization;
using Backend_TallerGo;
using Backend_TallerGo.Data;

var builder = WebApplication.CreateBuilder(args);

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

// Crear el esquema SQLite y cargar datos de ejemplo (una sola vez).
AppDb.Open();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// CORS: habilitado para cualquier origin/método (solo desarrollo, sin auth ni cookies).
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Auth mínima: los endpoints /api requieren Bearer token válido (24 hs), salvo /api/auth.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    var esApi = path.StartsWith("/api", StringComparison.OrdinalIgnoreCase);
    if (esApi && !path.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
    {
        var header = context.Request.Headers.Authorization.ToString();
        var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header["Bearer ".Length..] : null;
        if (!AuthToken.Validar(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { mensaje = "Sesión inválida o expirada. Iniciá sesión nuevamente." });
            return;
        }
    }
    await next();
});

app.MapControllers();
app.MapGet("/", () => new { nombre = "Backend-TallerGo API", salud = "ok" });

app.Run();