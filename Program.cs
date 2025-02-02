using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuración de los servicios de la aplicación.
/// </summary>

// Configuración de la base de datos PostgreSQL usando inyección de dependencias.
builder.Services.AddDbContextPool<PurpuraDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agrega controladores a la aplicación.
builder.Services.AddControllers();

// Configuración de autenticación JWT.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Habilita autorización en la aplicación.
builder.Services.AddAuthorization();

var app = builder.Build();

/// <summary>
/// Configuración del pipeline de manejo de solicitudes HTTP.
/// </summary>

// Habilita OpenAPI solo en entornos de desarrollo.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Fuerza el uso de HTTPS.
app.UseHttpsRedirection();

// Habilita autorización.
app.UseAuthorization();

// Carga variables de entorno desde un archivo .env.
DotNetEnv.Env.Load();

// Mapea controladores a las rutas definidas.
app.MapControllers();

// Inicia la aplicación.
app.Run();
