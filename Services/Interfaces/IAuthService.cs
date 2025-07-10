using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Services.Interfaces;

public interface IAuthService
{
  Task<LoginResponseDTO> LoginRequest(string email, string password);

  Task<LoginResponseDTO> RefreshTokenRequest(RefreshTokenDTO refreshTokenDTO);

  Task LogoutRequest(RefreshTokenDTO refreshTokenDTO);

  Task<string> GenerateSession(string userId, string email);

  Task<LoginResponseDTO> SignInUsingGoogle(string email, string name);
}