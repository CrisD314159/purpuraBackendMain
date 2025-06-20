using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.OpenApi.Models;
using purpuraMain.Exceptions.ExceptionFilter;
using purpuraMain.Services.Interfaces;
using purpuraMain.Services.Implementations;
using FluentValidation;
using purpuraMain.Validations;
using purpuraMain.Dto.InputDto;
using purpuraMain.Model;
using Microsoft.AspNetCore.Identity;
using purpuraMain.Mapper;
using purpuraMain.Utils;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Carga variables de entorno desde un archivo .env.
DotNetEnv.Env.Load();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));


// Configuración de la base de datos PostgreSQL usando inyección de dependencias.
builder.Services.AddDbContextPool<PurpuraDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de Identity Framework
builder.Services.AddIdentity<User, IdentityRole<string>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
}
)
.AddEntityFrameworkStores<PurpuraDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

//Injección de los servicios de la aplicación
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IArtistService, ArtistService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IPurpleDaylistService, PurpleDaylistService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IMediaUploadService, MediaUploadService>();


//Inyección de las validaciones de entidades
builder.Services.AddScoped<IValidator<PasswordChangeDTO>,PasswordValidation>();
builder.Services.AddScoped<IValidator<UpdatePlaylistDTO>,PlayListUpdateValidation>();
builder.Services.AddScoped<IValidator<CreatePlayListDTO>,PlayListValidation>();
builder.Services.AddScoped<IValidator<UpdateUserDto>,UserUpdateValidator>();
builder.Services.AddScoped<IValidator<CreateUserDTO>,UserValidator>();
builder.Services.AddScoped<IValidator<CreateAlbumDTO>, CreateAlbumValidation>();
builder.Services.AddScoped<IValidator<CreateArtistDTO>, CreateArtistValidation>();
builder.Services.AddScoped<IValidator<CreateSingleSongDTO>,CreateSongValidation>();
builder.Services.AddScoped<IValidator<UpdateAlbumDTO>, UpdateAlbumValidation>();
builder.Services.AddScoped<IValidator<UpdateArtistDTO>, UpdateArtistValidation>();
builder.Services.AddScoped<IValidator<UpdateSingleSongDTO>, UpdateSongValidation>();
builder.Services.AddScoped<IValidator<CreateGenreDTO>, CreateGenreValidation>();
builder.Services.AddScoped<IValidator<UpdateGenreDTO>, UpdateGenreValidation>();
builder.Services.AddScoped<IValidator<AddSongToAlbumDTO>, AddSongToAlbumValidation>();


// Agrega controladores a la aplicación.
builder.Services.AddControllers();


// Filtro de excepciones, esto evita usar try catch para todo
// Y captura una excepcion que retorna en una respuesta json
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();

});




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



// Fuerza el uso de HTTPS.
app.UseHttpsRedirection();


// Habilita autenticación y autorización.
app.UseAuthentication();
app.UseAuthorization();



// Mapea controladores a las rutas definidas.
app.MapControllers();

// Inicia la aplicación.
app.Run();