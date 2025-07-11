using Microsoft.AspNetCore.Identity;
using purpuraMain.Exceptions;
using purpuraMain.Model;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Services.Implementations;

public class ThirdPartyUserService (UserManager<User> userManager) : IThirdPartyUserService
{

  private readonly UserManager<User> _userManager = userManager;

  /// <summary>
  /// Creates a new user from third party auth services like Google or Facebook
  /// </summary>
  /// <param name="email"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  /// <exception cref="BadRequestException"></exception>
  /// <exception cref="InternalServerException"></exception>
  public async Task<User> CreateThirdPartyUser(string email, string name)
  {
    if (await _userManager.FindByEmailAsync(email) != null)
      throw new BadRequestException("User already exists");

    var cleanName = name.Trim().Replace(" ", "");
    User user = new()
    {
      Id = Guid.NewGuid().ToString(),
      State = UserState.ACTIVE,
      ProfilePicture = $"https://api.dicebear.com/9.x/thumbs/svg?seed={name}",
      CreatedAt = DateTime.UtcNow,
      Role = UserRole.USER,
      VerificationCode = new Random().Next(1000, 9999).ToString(),
      UserName = cleanName,
      Email = email,
      EmailConfirmed = true,
      IsThirdPartyUser = true
    };

    var result = await _userManager.CreateAsync(user, "Dummy-User1");
    if (!result.Succeeded)
    {
      throw new InternalServerException(string.Join(";", result.Errors.Select(e => e.Description)));
    }

    return user;

  }
}