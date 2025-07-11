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
using purpuraMain.Dto.InputDto;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public class AuthService(PurpuraDbContext dbContext, SignInManager<User> signInManager, UserManager<User> userManager
, IConfiguration configuration, IThirdPartyUserService thirdPartyUserService
) : IAuthService
{

  private readonly PurpuraDbContext _dbContext = dbContext;
  private readonly SignInManager<User> _signInManager = signInManager;
  private readonly UserManager<User> _userManager = userManager;
  private readonly IThirdPartyUserService _thirdPartyUserService = thirdPartyUserService;

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

    if (user.IsThirdPartyUser) throw new BadRequestException("Use your Google account to log in");

    var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
    if (!result.Succeeded)
    {
      throw new EntityNotFoundException("Invalid email or password");
    }

    if (result.Succeeded && user.Email != null)
    {
      // genera un token de acceso (1 hora) y un token de refresco (7 días) y los retorna
      var accessToken = JWTManagement.GenerateAccessToken(user.Id, user.Email, _configuration, user.Role, false, null);
      var refreshToken = await GenerateSession(user.Id, email);

      return new LoginResponseDTO
      {
        Token = accessToken,
        RefreshToken = refreshToken
      };
    }

    throw new InternalServerException("Cannot login");


  }

  /// <summary>
  /// Realiza una petición de renovación de token.
  /// </summary>
  /// <param name="userId">ID del usuario.</param>
  /// <param name="sessionId">ID de la sesión.</param>
  /// <param name="email">Correo electrónico del usuario.</param>
  /// <param name="_dbContext">Contexto de la base de datos.</param>
  public async Task<LoginResponseDTO> RefreshTokenRequest(RefreshTokenDTO refreshTokenDTO)
  {

    var refreshTokenClaims = JWTManagement.ExtractRefreshTokenInfo(refreshTokenDTO.RefreshToken, _configuration, out SecurityToken securityToken);

    var sessionId = refreshTokenClaims.FindFirst(ClaimTypes.SerialNumber)?.Value
    ?? throw new UnauthorizedAccessException("Invalid refresh token");

    var userId = refreshTokenClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value
    ?? throw new UnauthorizedAccessException("Invalid refresh token");

    var email = refreshTokenClaims.FindFirst(ClaimTypes.Email)?.Value
    ?? throw new UnauthorizedAccessException("Invalid refresh token");

    var role = refreshTokenClaims.FindFirst(ClaimTypes.Role)?.Value
    ?? throw new UnauthorizedAccessException("Invalid refresh token");

    // Verifica si la sesión existe y no ha expirado
    var session = await _dbContext.Sessions.Where(s => s.UserId == userId && s.Id == sessionId)
    .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Session not found");
    if (session.ExpiresdAt < DateTime.UtcNow)
    {
      _dbContext.Sessions.Remove(session);
      await _dbContext.SaveChangesAsync();
      throw new SessionExpiredException("Session expired");
    }

    var userRole = Enum.Parse<UserRole>(role);

    var token = JWTManagement.GenerateAccessToken(session.UserId, email, _configuration, userRole, false, null);

    // Renueva la expiración de la sesión si faltan menos de 2 días
    if (DateTime.UtcNow >= session.ExpiresdAt.AddDays(-2))
    {
      session.ExpiresdAt = DateTime.UtcNow.AddDays(7);
      await _dbContext.SaveChangesAsync();
      var newSessionToken = JWTManagement.GenerateAccessToken(session.UserId, email, _configuration, userRole, false, sessionId);

      return new LoginResponseDTO
      {
        RefreshToken = newSessionToken,
        Token = token
      };
    }
    return new LoginResponseDTO
    {
      Token = token
    };


  }


  /// <summary>
  /// Realiza una petición de cierre de sesión.
  /// </summary>
  /// <param name="userId">ID del usuario.</param>
  /// <param name="sessionId">ID de la sesión.</param>
  /// <param name="_dbContext">Contexto de la base de datos.</param>
  public async Task LogoutRequest(RefreshTokenDTO refreshTokenDTO)
  {

    var refreshTokenClaims = JWTManagement.ExtractRefreshTokenInfo(refreshTokenDTO.RefreshToken, _configuration, out SecurityToken securityToken);

    var sessionId = refreshTokenClaims.FindFirst(ClaimTypes.SerialNumber)?.Value
    ?? throw new UnauthorizedAccessException("Invalid refresh token");
    // Verifica si la sesión existe y no ha expirado, en caso de que exista, la limpia
    var session = await _dbContext.Sessions.FindAsync(sessionId)
    ?? throw new EntityNotFoundException("Invalid email or password");

    _dbContext.Sessions.Remove(session);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<string> GenerateSession(string userId, string email)
  {
    var user = await _userManager.FindByIdAsync(userId)
    ?? throw new EntityNotFoundException("User does no exists");

    var oldSessions = await _dbContext.Sessions.Where(s => s.UserId == userId && s.ExpiresdAt <= DateTime.UtcNow.AddDays(-7))
    .ToListAsync();

    if (oldSessions.Count > 0)
    {
      _dbContext.Sessions.RemoveRange(oldSessions);
    }

    var newSession = new Session
    {
      UserId = userId,
      CreatedAt = DateTime.UtcNow,
      ExpiresdAt = DateTime.UtcNow.AddDays(7)
    };

    var refreshToken = JWTManagement.GenerateAccessToken(userId, email, _configuration, user.Role, true, newSession.Id);

    await _dbContext.Sessions.AddAsync(newSession);
    await _dbContext.SaveChangesAsync();

    return refreshToken;
  }

  /// <summary>
  /// Logs in a user using google Oauth provider
  /// If the user is already on the database, jus returns JWT for access and session 
  /// </summary>
  /// <param name="email"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public async Task<LoginResponseDTO> SignInUsingGoogle(string email, string name)
  {
    var user = await _userManager.FindByEmailAsync(email);

    if (user == null)
    {
      var newUser = await _thirdPartyUserService.CreateThirdPartyUser(email, name);
      return await GenerateThirdPartyTokenAndSession(newUser.Id, newUser.Email!, newUser.Role);
    }

    return await GenerateThirdPartyTokenAndSession(user.Id, user.Email!, user.Role);
  }
  
  private async Task<LoginResponseDTO> GenerateThirdPartyTokenAndSession(string id, string email, UserRole userRole)
  {
    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email)) throw new BadRequestException("id or email not provided");

    var token = JWTManagement.GenerateAccessToken(id, email, _configuration, userRole, false, null);
    var refreshToken = await GenerateSession(id, email);

    return new LoginResponseDTO()
    {
      Token = token,
      RefreshToken = refreshToken
    };
  }
}