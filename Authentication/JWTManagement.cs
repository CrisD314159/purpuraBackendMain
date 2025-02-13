using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using purpuraMain.DbContext;
using purpuraMain.Model;

namespace purpuraMain.Authentication;

public class JWTManagement{
  
  /// <summary>
  /// Genera un Token de acceso para un usuario.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="email"></param>
  /// <param name="configuration"></param>
  /// <returns></returns>
  public static string GenerateAccessToken(string userId, string email, IConfiguration configuration){
    // Claims del token o información que se quiere guardar en el token
    var claims = new []
    {
      new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
      new Claim(ClaimTypes.Email, email)
    };

    // Clave de seguridad para firmar el token y el algoritmo con que se firma
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Construcción y envío del token
    var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  /// <summary>
  /// Extiende la duración de un token de sesión.
  /// </summary>
  /// <param name="sessionId"></param>
  /// <param name="userId"></param>
  /// <param name="email"></param>
  /// <param name="configuration"></param>
  /// <returns></returns>
  public static string ExtendSessionToken (string sessionId, string userId, string email, IConfiguration configuration){
    // Claims del token o información que se quiere guardar en el token
    var claims = new []
      {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.SerialNumber, sessionId),
        new Claim(ClaimTypes.Email, email)
      };

      // Clave de seguridad para firmar el token y el algoritmo con que se firma
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      // Construcción y envío del token de sesión refrescado
      var token = new JwtSecurityToken(
          issuer: configuration["Jwt:Issuer"],
              audience: configuration["Jwt:Audience"],
              claims: claims,
              expires: DateTime.UtcNow.AddDays(5),
              signingCredentials: credentials
      );

    return new JwtSecurityTokenHandler().WriteToken(token);

  }


  /// <summary>
  /// Genera un token de sesión para un usuario cuando se logue por primera vez. Además, crea la sesión en la base de datos
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="email"></param>
  /// <param name="configuration"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<string> GenerateRefreshToken(string userId, string email, IConfiguration configuration, PurpuraDbContext dbContext){

    try
    {
      // Genera un ID de sesión único
      var sessionId = Guid.NewGuid().ToString();
      // Claims del token o información que se quiere guardar en el token
      var claims = new []
      {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.SerialNumber, sessionId),
        new Claim(ClaimTypes.Email, email)
      };

      // Clave de seguridad para firmar el token y el algoritmo con que se firma
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      // Construcción y envío del token de sesión
      var token = new JwtSecurityToken(
          issuer: configuration["Jwt:Issuer"],
              audience: configuration["Jwt:Audience"],
              claims: claims,
              expires: DateTime.UtcNow.AddDays(5),
              signingCredentials: credentials
      );

      // Guarda la sesión en la base de datos para verificarla en futuras peticiones
      await dbContext.Sessions!.AddAsync(new Session{
        Id = sessionId,
        UserId = userId,
        CreatedAt = DateTime.UtcNow,
        ExpiresdAt = DateTime.UtcNow.AddDays(5)
      });
      await dbContext.SaveChangesAsync();

      return new JwtSecurityTokenHandler().WriteToken(token);
      }

    catch (System.Exception)
    {
      
      throw;
    }
  }
}