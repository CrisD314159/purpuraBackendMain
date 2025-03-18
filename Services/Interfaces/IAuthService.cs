using purpuraMain.DbContext;
using purpuraMain.Dto;

namespace purpuraMain.Services.Interfaces;

public interface IAuthService
{
  Task<LoginResponseDTO> LoginRequest (string email, string password, IConfiguration configuration);

  Task<LoginResponseDTO> RefreshTokenRequest (string userId, string sessionId, string email, IConfiguration configuration);

  Task<bool> LogoutRequest (string userId, string sessionId);
}