using purpuraMain.DbContext;
using purpuraMain.Dto;

namespace purpuraMain.Services.Interfaces;

public interface IAuthService
{
  Task<LoginResponseDTO> LoginRequest (string email, string password);

  Task<LoginResponseDTO> RefreshTokenRequest (string userId, string sessionId, string email);

  Task LogoutRequest (string userId, string sessionId);
}