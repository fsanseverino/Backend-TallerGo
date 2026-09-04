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

// CORS: permitir el servidor de desarrollo de Angular.
app.UseCors(builder => builder
    .WithOrigins("http://localhost:4200", "http://localhost:4301", "http://127.0.0.1:4200")
    .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
    .AllowCredentials());

app.MapControllers();
app.MapGet("/", () => new { nombre = "Backend-TallerGo API", salud = "ok" });

app.Run();