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
  public static async Task<LoginResponseDTO> LoginRequest (string email, string password, PurpuraDbContext dbContext, IConfiguration configuration)
  {
    try
    {
      var user = await dbContext.Users!.Where(u=> u.Email == email && u.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Invalid email or password");
      if (new PasswordManipulation().VerifyPassword(user.Password, password) == false)
      {
        throw new BadRequestException("Invalid email or password");
      }

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
  public static async Task<string> RefreshTokenRequest (string userId, string sessionId, string email, PurpuraDbContext dbContext, IConfiguration configuration)
  {
    try
    {
      var session = await dbContext.Sessions!.Where(s => s.UserId == userId && s.Id == sessionId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Session not found");
      if (session.ExpiresdAt < DateTime.UtcNow)
      {
        dbContext.Sessions!.Remove(session);
        await dbContext.SaveChangesAsync();
        throw new SessionExpiredException("Session expired, log in again");
      }

      var token = JWTManagement.GenerateAccessToken(session.UserId, email, configuration);

      session.ExpiresdAt = DateTime.UtcNow.AddDays(5);
      await dbContext.SaveChangesAsync();

      return token;
    }
    catch (System.Exception)
    {
      
      throw;
    }

  }


  public static async Task<bool> LogoutRequest (string userId, string sessionId, PurpuraDbContext dbContext)
  {
    try
    {
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