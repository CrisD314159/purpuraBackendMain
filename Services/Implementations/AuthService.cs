namespace purpuraMain.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Authentication;
using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Exceptions;
using purpuraMain.Utils;
using purpuraMain.Model;
using purpuraMain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

public class AuthService(PurpuraDbContext dbContext, SignInManager<User> signInManager, UserManager<User> userManager
, IConfiguration configuration
) : IAuthService
{

  private readonly PurpuraDbContext _dbContext = dbContext;
  private readonly SignInManager<User> _signInManager = signInManager;
  private readonly UserManager<User> _userManager = userManager;

  private readonly IConfiguration _configuration = configuration;
  /// <summary>
  /// Realiza una petición de inicio de sesión.
  /// </summary>
  /// <param name="email">Correo electrónico del usuario.</param>
  /// <param name="password">Contraseña del usuario.</param>
  /// <param name="dbContext">Contexto de la base de datos.</param>
  public async Task<LoginResponseDTO> LoginRequest(string email, string password)
  {

    // Verifica si el usuario existe y si la contraseña es correcta
    var user = await _dbContext.Users.Where(u => u.Email == email && u.State != UserState.INACTIVE).FirstOrDefaultAsync()
    ?? throw new EntityNotFoundException("Invalid email or password");

    if (user.State == UserState.UNVERIFIED) throw new NotVerifiedException("Invalid email or password");

    var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
    if (!result.Succeeded)
    {
      throw new EntityNotFoundException("Invalid email or password");
    }

    if (result.Succeeded && user.Email != null)
    {
      var accessToken = JWTManagement.GenerateAccessToken(user.Id, user.Email, _configuration);
      var refreshToken = await JWTManagement.GenerateRefreshToken(user.Id, user.Email, _configuration, _dbContext);

      return new LoginResponseDTO
      {
        Token = accessToken,
        RefreshToken = refreshToken
      };
    }

    throw new InternalServerException("Cannot login");

    // genera un token de acceso (30min) y un token de refresco (5 días) y los retorn
  }

  /// <summary>
  /// Realiza una petición de renovación de token.
  /// </summary>
  /// <param name="userId">ID del usuario.</param>
  /// <param name="sessionId">ID de la sesión.</param>
  /// <param name="email">Correo electrónico del usuario.</param>
  /// <param name="_dbContext">Contexto de la base de datos.</param>
  public async Task<LoginResponseDTO> RefreshTokenRequest (string userId, string sessionId, string email)
  {

      // Verifica si la sesión existe y no ha expirado
      var session = await _dbContext.Sessions.Where(s => s.UserId == userId && s.Id == sessionId)
      .FirstOrDefaultAsync()
      ?? throw new EntityNotFoundException("Session not found");
      if (session.ExpiresdAt < DateTime.UtcNow)
      {
        _dbContext.Sessions!.Remove(session);
        await _dbContext.SaveChangesAsync();
        throw new SessionExpiredException("Session expired");
      }

      var token = JWTManagement.GenerateAccessToken(session.UserId, email, _configuration);

     // Renueva la expiración de la sesión si faltan menos de 2 días
    if (DateTime.UtcNow >= session.ExpiresdAt.AddDays(-2))
    {
        session.ExpiresdAt = DateTime.UtcNow.AddDays(5);
        await _dbContext.SaveChangesAsync();
        var newSessionId = JWTManagement.ExtendSessionToken(sessionId, userId, email, _configuration);

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
  /// <param name="_dbContext">Contexto de la base de datos.</param>
  public async Task LogoutRequest (string userId, string sessionId)
  {

    // Verifica si la sesión existe y no ha expirado, en caso de que exista, la limpia
    var session = await _dbContext.Sessions.Where(s => s.UserId == userId && s.Id == sessionId)
    .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Invalid email or password");
    _dbContext.Sessions!.Remove(session);
    await _dbContext.SaveChangesAsync();
  }

}