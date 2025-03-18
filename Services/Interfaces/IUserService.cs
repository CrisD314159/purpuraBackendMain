using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IUserService
{

  Task<GetUserDto> GetUserById(string id);
  Task<bool> CreateUser(CreateUserDTO user);

  Task<bool> UpdateUser(string userId, UpdateUserDto user);

  Task<bool> DeleteUser(string id);

  Task<bool> UpdateUserPassword(PasswordChangeDTO passwordDTO);

  Task<bool> VerifyAccount(VerifyAccountDTO verifyAccountDTO);

  Task<bool> SendPasswordRecoveryCode(string email);
}