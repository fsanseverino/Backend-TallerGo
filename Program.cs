using System.Text.Json;
using System.Text.Json.Serialization;
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

app.MapControllers();
app.MapGet("/", () => new { nombre = "Backend-TallerGo API", salud = "ok" });

app.Run();