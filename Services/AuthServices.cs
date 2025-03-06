using Microsoft.EntityFrameworkCore;
using purpuraMain.Authentication;
using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Exceptions;
using purpuraMain.Utils;
using purpuraMain.Model;

namespace purpuraMain.Services;

public static class AuthServices 
{

  /// <summary>
  /// Realiza una petición de inicio de sesión.
  /// </summary>
  /// <param name="email">Correo electrónico del usuario.</param>
  /// <param name="password">Contraseña del usuario.</param>
  /// <param name="dbContext">Contexto de la base de datos.</param>
  /// <param name="configuration">Configuración de la aplicación para el JWT.</param>
  public static async Task<LoginResponseDTO> LoginRequest (string email, string password, PurpuraDbContext dbContext, IConfiguration configuration)
  {

      // Verifica si el usuario existe y si la contraseña es correcta
      var user = await dbContext.Users!.Where(u=> u.Email == email && u.State != UserState.INACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Invalid email or password", Success=false});
      if(user.State == UserState.UNVERIFIED) throw new NotVerifiedException(401, new {Message ="You are not verified", Success=false});
      if (new PasswordManipulation().VerifyPassword(user.Password, password) == false)
      {
        throw new EntityNotFoundException(404, new {Message ="Invalid email or password", success=false});
      }

      // genera un token de acceso (30min) y un token de refresco (5 días) y los retorna
      var accessToken = JWTManagement.GenerateAccessToken(user.Id, user.Email, configuration);
      var refreshToken = await JWTManagement.GenerateRefreshToken(user.Id, user.Email, configuration, dbContext);

      return new LoginResponseDTO{
        Token = accessToken,
        RefreshToken = refreshToken
      };


  }

  /// <summary>
  /// Realiza una petición de renovación de token.
  /// </summary>
  /// <param name="userId">ID del usuario.</param>
  /// <param name="sessionId">ID de la sesión.</param>
  /// <param name="email">Correo electrónico del usuario.</param>
  /// <param name="dbContext">Contexto de la base de datos.</param>
  public static async Task<LoginResponseDTO> RefreshTokenRequest (string userId, string sessionId, string email, PurpuraDbContext dbContext, IConfiguration configuration)
  {

      // Verifica si la sesión existe y no ha expirado
      var session = await dbContext.Sessions!.Where(s => s.UserId == userId && s.Id == sessionId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Session not found", Success=false});
      if (session.ExpiresdAt < DateTime.UtcNow)
      {
        dbContext.Sessions!.Remove(session);
        await dbContext.SaveChangesAsync();
        throw new SessionExpiredException(401, new {Message ="Album not found", Success=false});
      }

      var token = JWTManagement.GenerateAccessToken(session.UserId, email, configuration);

     // Renueva la expiración de la sesión si faltan menos de 2 días
    if (DateTime.UtcNow >= session.ExpiresdAt.AddDays(-2))
    {
        session.ExpiresdAt = DateTime.UtcNow.AddDays(5);
        await dbContext.SaveChangesAsync();
        var newSessionId = JWTManagement.ExtendSessionToken(sessionId, userId, email, configuration);

        return new LoginResponseDTO
        {
            RefreshToken = newSessionId,
            Token = token
        };
    }
      return new LoginResponseDTO{
        Token = token
      };
   

  }


  /// <summary>
  /// Realiza una petición de cierre de sesión.
  /// </summary>
  /// <param name="userId">ID del usuario.</param>
  /// <param name="sessionId">ID de la sesión.</param>
  /// <param name="dbContext">Contexto de la base de datos.</param>
  public static async Task<bool> LogoutRequest (string userId, string sessionId, PurpuraDbContext dbContext)
  {

      // Verifica si la sesión existe y no ha expirado, en caso de que exista, la limpia
      var session = await dbContext.Sessions!.Where(s => s.UserId == userId && s.Id == sessionId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Session not found", Success=false});
      dbContext.Sessions!.Remove(session);
      await dbContext.SaveChangesAsync();
      return true;


  }

}