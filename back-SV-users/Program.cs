using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Custom;
using Models;
using back_SV_users.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura el logging para capturar mensajes detallados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();  // Añade el log de depuración

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug);  // Ajusta el nivel mínimo de logs a Debug para más detalles
});

// Add services to the container.
builder.Services.AddControllers();

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

// Get connection string from configuration file
var connectionString = builder.Configuration
    .GetConnectionString("ConnectionString")
    ?? throw new ArgumentNullException("No connection string found");

// Print the connection string to verify that it is being read correctly.
Console.WriteLine($"Connection String: {connectionString}");

// Adding DbContext service with PostgreSQL connection
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging()  // Mostrar datos sensibles en los logs
           .LogTo(Console.WriteLine, LogLevel.Debug); // Mostrar logs detallados en consola con nivel Debug
});

builder.Services.AddSingleton<Utilities>();

// Validate JWT Key
var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT Key is not set correctly in the configuration.");
}

// Si los problemas persisten, comenta la configuración de autenticación JWT para reducir la complejidad temporalmente
 builder.Services.AddAuthentication(config =>
 {
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
     config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
 }).AddJwtBearer(config =>
 {
     config.RequireHttpsMetadata = false;
     config.SaveToken = true;
     config.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         ValidateIssuer = false,
         ValidateAudience = false,
         ValidateLifetime = true,
         ClockSkew = TimeSpan.Zero,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
     };
 });

var app = builder.Build();

// Configure the HTTP request handling pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Implement CORS policy
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
