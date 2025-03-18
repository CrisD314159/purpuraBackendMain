using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using purpuraMain.Exceptions.ExceptionFilter;
using purpuraMain.Services.Interfaces;
using purpuraMain.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configuración de los servicios de la aplicación.
/// </summary>

// Configuración de la base de datos PostgreSQL usando inyección de dependencias.
builder.Services.AddDbContextPool<PurpuraDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Injección de los servicios de la aplicación
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IPurpleDaylistService, PurpleDaylistService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISearchService, SearchService>();


// Agrega controladores a la aplicación.
builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpResponseExceptionFilter>();
    options.Filters.Add<BadRequestExceptionFilter>();
    options.Filters.Add<EntityNotFoundExceptionFilter>();
    options.Filters.Add<NotVerifiedExceptionFilter>();
    options.Filters.Add<NullFieldExceptionFilter>();
    options.Filters.Add<SessionExpiredExceptionFilter>();
    options.Filters.Add<UnauthorizedExceptionFilter>();

});


// Agregar OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Púrpura Music API",
        Version = "v1",
        Description = "API para la aplicación de streaming de música Púrpura Music.",
        Contact = new OpenApiContact
        {
            Name = "Cristian David Vargas Loaiza",
            Url = new Uri("https://crisdev-pi.vercel.app")
        }
    });

    // Configurar autenticación en Swagger (opcional, si usas JWT)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce el token en formato 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new List<string>() }
    });
});

// ⚠️ **Mueve esto arriba, antes de `builder.Build();`**
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

// Configurar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Púrpura Music API v1");
        options.RoutePrefix = "api-docs"; // Acceder desde /api-docs
    });
}

// Fuerza el uso de HTTPS.
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}else
{
    app.UseExceptionHandler("/error-development");
}

// Habilita autenticación y autorización.
app.UseAuthentication();
app.UseAuthorization();

// Carga variables de entorno desde un archivo .env.
DotNetEnv.Env.Load();

// Mapea controladores a las rutas definidas.
app.MapControllers();

// Inicia la aplicación.
app.Run();