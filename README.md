# Backend-TallerGo

API mínima para el taller (TallerGo), construida con **ASP.NET Core 10** (el más nuevo disponible en la PC) y **EF Core + SQLite** para la base de datos.

## Requisitos

- .NET SDK 10 (probado con 10.0.400)

## Ejecutar

```powershell
dotnet run
```

Al arrancar se crea el archivo SQLite `tallergo.db` (si no existe) y se cargan datos de ejemplo. La API escucha en `http://localhost:8080` por defecto.

## Endpoints

| Método | Ruta | Descripción |
| ------ | ---- | ----------- |
| GET | `/api/clientes` | Lista de clientes |
| GET | `/api/clientes/{id}` | Detalle de un cliente |
| POST | `/api/clientes` | Crear cliente |
| PUT | `/api/clientes/{id}` | Actualizar cliente |
| DELETE | `/api/clientes/{id}` | Eliminar cliente |
| GET | `/api/vehiculos` | Lista de vehículos |
| GET | `/api/vehiculos/por-cliente/{clienteId}` | Vehículos de un cliente |
| POST | `/api/vehiculos` | Crear vehículo |
| PUT | `/api/vehiculos/{id}` | Actualizar vehículo |
| DELETE | `/api/vehiculos/{id}` | Eliminar vehículo |
| GET | `/api/trabajos` | Lista de trabajos (con items y pagos) |
| GET | `/api/trabajos/por-cliente/{clienteId}` | Trabajos de un cliente |
| GET | `/api/trabajos/por-vehiculo/{vehiculoId}` | Trabajos de un vehículo |
| GET | `/api/trabajos/{id}` | Detalle de un trabajo |
| POST | `/api/trabajos` | Crear trabajo (con `items` y `pagos`) |
| PUT | `/api/trabajos/{id}` | Actualizar trabajo (reemplaza `items`) |
| POST | `/api/trabajos/{id}/pagos` | Registrar un pago/adelanto |
| DELETE | `/api/trabajos/{id}` | Eliminar trabajo |
| GET | `/api/cajas` | Lista de cajas |
| GET | `/api/cajas/abierta` | Caja abierta (204 si no hay) |
| POST | `/api/cajas` | Abrir caja |
| POST | `/api/cajas/{id}/cierre` | Cerrar caja |
| GET | `/api/cajas/{id}/movimientos` | Movimientos de una caja |
| POST | `/api/cajas/{id}/movimientos` | Agregar movimiento |
| DELETE | `/api/cajas/movimientos/{id}` | Eliminar movimiento |

## Formato JSON

- Nombres de propiedades en **camelCase** (igual que el frontend Angular).
- Enums como **strings** (`"DNI"`, `"SIN_INICIAR"`, `"MANO_OBRA"`, `"INGRESO_TRABAJO"`).
- Fechas en ISO-8601. Los campos de fecha opcionales aceptan `""` / `null` (p. ej. `fechaRealizacion`).
- El body de POST/PUT con un único parámetro complejo se envía **plano** (p. ej. `{"nombre":"...", "apellido":"..."}`).

## CORS

Permitidos los orígenes del frontend Angular: `http://localhost:4200`, `http://localhost:4301` y `http://127.0.0.1:4200`.

## Estructura

- `Models/` — entidades y enums
- `Data/` — `TallerGoDbContext` (esquema/relaciones), `AppDb` (acceso a SQLite), `DbInitializer` (datos de ejemplo)
- `Controllers/` — `ClientesController`, `VehiculosController`, `TrabajosController`, `CajasController`
- `Program.cs` — bootstrap (servicios, CORS, OpenAPI)