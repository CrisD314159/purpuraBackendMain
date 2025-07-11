using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Model;

namespace purpuraMain.Services.Interfaces;


public interface IUserService
{

  Task<GetUserDto> GetUserById(string id);
  Task CreateUser(CreateUserDTO user);

  Task UpdateUser(string userId, UpdateUserDto user);

  Task DeleteUser(string id);

  Task UpdateUserPassword(PasswordChangeDTO passwordDTO);

  Task VerifyAccount(VerifyAccountDTO verifyAccountDTO);

  Task SendPasswordRecoveryCode(string email);

  Task<User> VerifyAndReturnValidUser(string userId);
}