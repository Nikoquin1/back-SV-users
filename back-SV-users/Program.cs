using back_SV_users.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllers(); // En versiones anteriores de .NET, se llamaba ConfigureServices

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Obtener la cadena de conexión desde el archivo de configuración
var connectionString = builder.Configuration
    .GetConnectionString("ConnectionString")
    ?? throw new ArgumentNullException("Dont have connection");

// Imprimir la cadena de conexión para verificar que se esté leyendo correctamente
Console.WriteLine($"Connection String: {connectionString}");

// Añadir el servicio DbContext con la conexión PostgreSQL
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging()  // Activa logging detallado
           .LogTo(Console.WriteLine);     // Muestra los logs en la consola
});


var app = builder.Build();

// Configurar el pipeline de manejo de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar la política de CORS
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
