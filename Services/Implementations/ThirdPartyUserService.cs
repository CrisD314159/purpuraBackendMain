using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using purpuraMain.DbContext;
using purpuraMain.Exceptions;
using purpuraMain.Model;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Services.Implementations;

public partial class ThirdPartyUserService(UserManager<User> userManager, PurpuraDbContext dbContext) : IThirdPartyUserService
{
  private readonly PurpuraDbContext _dbContext = dbContext;

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

    var cleanName = CleanString(name);

    var uniqueUsername = await GenerateUniqueUsername(cleanName);

    
    User user = new()
    {
      State = UserState.ACTIVE,
      ProfilePicture = $"https://api.dicebear.com/9.x/glass/svg?seed={uniqueUsername}",
      CreatedAt = DateTime.UtcNow,
      Role = UserRole.USER,
      VerificationCode = "",
      UserName = uniqueUsername,
      Email = email,
      EmailConfirmed = true,
      IsThirdPartyUser = true
    };

    var result = await _userManager.CreateAsync(user, "Dummy-User1");
    if (!result.Succeeded)
    {
      throw new InternalServerException(string.Join(";", result.Errors.Select(e => e.Description)));
    }


    await using var transaction = await _dbContext.Database.BeginTransactionAsync();

    try
    {
        // Crear library y playlist
        var playlist = new Playlist
        {
            Name = "Purple Day List",
            Description = "This is your Purple Day List, here you will find daily recommendations based on the music you most like",
            UserId = user.Id,
            User = user,
            CreatedAt = DateTime.UtcNow,
            IsPublic = false,
            Editable = false,
        };

        var library = new Library
        {
            UserId = user.Id,
            User = user,
            Playlists = [playlist]
        };

        await _dbContext.Libraries.AddAsync(library);
        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }

    return user;

  }

  public async Task<string> GenerateUniqueUsername(string name)
  {
    var counter = 1;
    var username = $"{name}_{counter}";

    while (await IsUsernameTaken(username))
    {
      counter++;
      username = $"{name}_{counter}";
    }

    return username;

  }

  public async Task<bool> IsUsernameTaken(string username) {

    var user = await _userManager.FindByNameAsync(username);

    return user is not null;
  }
  
  public string RemoveDiacritics(string text)
  {
    if (string.IsNullOrWhiteSpace(text))
      return string.Empty;

    var normalizedString = text.Normalize(NormalizationForm.FormD);
    var stringBuilder = new StringBuilder();

    foreach (var c in normalizedString)
    {
      var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
      // Skip non-spacing marks (diacritics/accents)
      if (unicodeCategory != UnicodeCategory.NonSpacingMark)
      {
        stringBuilder.Append(c);
      }
    }

    // Normalize back to composed form
    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
  }
  
  public string CleanString(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return string.Empty;

   var text = name.ToLowerInvariant().Replace(" ", "");
    // First remove diacritics
    string withoutDiacritics = RemoveDiacritics(text);

    // Then remove special characters, keeping only alphanumeric and spaces
    string cleaned = MyRegex().Replace(withoutDiacritics, "");

    // Clean up multiple spaces
    cleaned = MyRegex1().Replace(cleaned, " ").Trim();

    return cleaned;
  }

  [GeneratedRegex(@"[^a-zA-Z0-9\s]")]
  private static partial Regex MyRegex();
  [GeneratedRegex(@"\s+")]
  private static partial Regex MyRegex1();
}