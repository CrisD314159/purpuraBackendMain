using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using purpuraMain.DbContext;
using purpuraMain.Model;

namespace purpuraMain.Authentication;

public class JWTManagement{
  
  public static string GenerateAccessToken(string userId, string email, IConfiguration configuration){
    var claims = new []
    {
      new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
      new Claim(ClaimTypes.Email, email)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
  public static async Task<string> GenerateRefreshToken(string userId, string email, IConfiguration configuration, PurpuraDbContext dbContext){

    try
    {
      var sessionId = Guid.NewGuid().ToString();
      var claims = new []
      {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.SerialNumber, sessionId),
        new Claim(ClaimTypes.Email, email)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: configuration["Jwt:Issuer"],
              audience: configuration["Jwt:Audience"],
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(10),
              signingCredentials: credentials
      );

      await dbContext.Sessions!.AddAsync(new Session{
        Id = sessionId,
        UserId = userId,
        CreatedAt = DateTime.UtcNow,
        ExpiresdAt = DateTime.UtcNow.AddMinutes(1)
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