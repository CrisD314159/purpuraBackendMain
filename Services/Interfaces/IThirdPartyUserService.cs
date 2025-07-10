using purpuraMain.Model;

namespace purpuraMain.Services.Interfaces;


public interface IThirdPartyUserService
{
  Task<User> CreateThirdPartyUser(string email, string name);
}