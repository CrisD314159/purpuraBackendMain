using MimeKit;

namespace purpuraMain.Services.Interfaces;


public interface IMailService
{
  Task<bool> SendVerificationEmail(string email, string verificationCode, string name);
  Task<bool> SendPasswordRecoveryCodeEmail(string email, string verificationCode, string name);
  Task<bool> SendVerifiedAccountMail(string email, string name);
  Task<bool> SendPasswordChangeMail(string email, string name);
  Task<bool> SendEmail(MimeMessage message);
}