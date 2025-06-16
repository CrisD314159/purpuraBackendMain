using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using purpuraMain.DbContext;
using purpuraMain.Exceptions;
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
  public static string GenerateAccessToken(string userId, string email, IConfiguration configuration, UserRole userRole, bool generateRefresh
  , string? sessionId
  )
  {
    // Claims del token o información que se quiere guardar en el token
    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, userId.ToString()),
      new(ClaimTypes.Email, email),
      new(ClaimTypes.Role, userRole.ToString())
    };

    if (!string.IsNullOrEmpty(sessionId))
    {
      claims.Add(new(ClaimTypes.SerialNumber, sessionId));
    }

    // Clave de seguridad para firmar el token y el algoritmo con que se firma
    var key = generateRefresh ? configuration["Jwt:RefreshKey"] : configuration["Jwt:Key"];

    if (string.IsNullOrEmpty(key))
    {
      throw new BadRequestException("Invalid Access Key");
    }

    var tokenKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);


    // Construcción y envío del token
    var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: generateRefresh ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
  }


  public static ClaimsPrincipal ExtractRefreshTokenInfo(string refreshToken, IConfiguration configuration, out SecurityToken securityToken)
  {
    var handler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(configuration["Jwt:RefreshKey"] ?? throw new BadRequestException("Key not found"));

    var validationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero

    };

    try
    {
      var result = handler.ValidateToken(refreshToken, validationParameters , out securityToken);

      return result;

    }catch(SecurityTokenMalformedException)
    {
      throw new BadRequestException("Invalid token type");
    }
    catch(SecurityTokenExpiredException)
    {
      throw new UnauthorizedException("Expired session");
    }
    catch(SecurityTokenException)
    {
      throw new UnauthorizedException("Token not allowed");
    }
  }
}