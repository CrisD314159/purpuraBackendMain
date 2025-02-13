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
    try
    {
      // Verifica si el usuario existe y si la contraseña es correcta
      var user = await dbContext.Users!.Where(u=> u.Email == email && u.State != UserState.INACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Invalid email or password");
      if(user.State == UserState.UNVERIFIED) throw new NotVerifiedException("User not verified");
      if (new PasswordManipulation().VerifyPassword(user.Password, password) == false)
      {
        throw new EntityNotFoundException("Invalid email or password");
      }

      // genera un token de acceso (30min) y un token de refresco (5 días) y los retorna
      var accessToken = JWTManagement.GenerateAccessToken(user.Id, user.Email, configuration);
      var refreshToken = await JWTManagement.GenerateRefreshToken(user.Id, user.Email, configuration, dbContext);

      return new LoginResponseDTO{
        Token = accessToken,
        RefreshToken = refreshToken
      };
    }
    catch (System.Exception)
    {
      
      throw;
    }

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
    try
    {
      // Verifica si la sesión existe y no ha expirado
      var session = await dbContext.Sessions!.Where(s => s.UserId == userId && s.Id == sessionId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Session not found");
      if (session.ExpiresdAt < DateTime.UtcNow)
      {
        dbContext.Sessions!.Remove(session);
        await dbContext.SaveChangesAsync();
        throw new SessionExpiredException("Session expired, log in again");
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
    catch (System.Exception)
    {
      throw;
    }

  }


  /// <summary>
  /// Realiza una petición de cierre de sesión.
  /// </summary>
  /// <param name="userId">ID del usuario.</param>
  /// <param name="sessionId">ID de la sesión.</param>
  /// <param name="dbContext">Contexto de la base de datos.</param>
  public static async Task<bool> LogoutRequest (string userId, string sessionId, PurpuraDbContext dbContext)
  {
    try
    {
      // Verifica si la sesión existe y no ha expirado, en caso de que exista, la limpia
      var session = await dbContext.Sessions!.Where(s => s.UserId == userId && s.Id == sessionId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Session not found");
      dbContext.Sessions!.Remove(session);
      await dbContext.SaveChangesAsync();
      return true;
    }
    catch (System.Exception)
    {
      
      throw;
    }

  }

}