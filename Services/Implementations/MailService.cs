using MailKit.Net.Smtp;
using MimeKit;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Services.Implementations;

public class MailService(IConfiguration configuration) : IMailService
{

  private readonly IConfiguration _configuration = configuration;
   /// <summary>
  /// Envía un correo electrónico con un código de verificación.
  /// </summary>
  /// <param name="email">Correo del destinatario.</param>
  /// <param name="verificationCode">Código de verificación.</param>
  /// <param name="name">Nombre del destinatario.</param>
  public async Task<bool> SendVerificationEmail(string email, string verificationCode, string name)
  {
    var messageMail = new MimeMessage();
    var html = File.ReadAllText("./templates/mailVerificationCodeTemplate.html");
    html = html.Replace("{{Name}}", name);
    html = html.Replace("{{VerificationCode}}", verificationCode);
    messageMail.From.Add(new MailboxAddress("Purpura Music", "purpuramusic1@gmail.com"));
    messageMail.To.Add(new MailboxAddress(name, email));
    messageMail.Subject = "Here's your verification code for Purpura Music";
    messageMail.Body = new TextPart("html") { Text = html };

    return await SendEmail(messageMail);
  }

    /// <summary>
    /// Envía un correo electrónico con un código de recuperación de contraseña.
    /// </summary>
    public async Task<bool> SendPasswordRecoveryCodeEmail(string email, string verificationCode, string name)
    { 
        var messageMail = new MimeMessage();
        var encodedCode = Uri.EscapeDataString(verificationCode);
        var linkCode = $"https://purpura-music.vercel.app/changePassword?token={encodedCode}";
        var html = File.ReadAllText("./templates/paswordChangeCodeTemplate.html");
        html = html.Replace("{{Name}}", name);
        html = html.Replace("{{VerificationCode}}", linkCode);
        messageMail.From.Add(new MailboxAddress("Purpuramusic", "purpuramusic1@gmail.com"));
        messageMail.To.Add(new MailboxAddress(name, email));
        messageMail.Subject = "Here's your recovery code for Purpura Music";
        messageMail.Body = new TextPart("html") { Text = html };
        
        return await SendEmail(messageMail);
    }

    /// <summary>
    /// Envía un correo electrónico confirmando la verificación de cuenta.
    /// </summary>
    public async Task<bool> SendVerifiedAccountMail(string email, string name)
    { 
        var messageMail = new MimeMessage();
        var html = File.ReadAllText("./templates/accountVerifiedTemplate.html");
        html = html.Replace("{{Name}}", name);
        messageMail.From.Add(new MailboxAddress("Purpuramusic", "purpuramusic1@gmail.com"));
        messageMail.To.Add(new MailboxAddress(name, email));
        messageMail.Subject = "Thanks for verify your account on Purpura Music";
        messageMail.Body = new TextPart("html") { Text = html };
        
        return await SendEmail(messageMail);
    }

    /// <summary>
    /// Envía un correo electrónico notificando un cambio de contraseña.
    /// </summary>
    public async Task<bool> SendPasswordChangeMail(string email, string name)
    { 
        var messageMail = new MimeMessage();
        var html = File.ReadAllText("./templates/passwordChangeTemplate.html");
        html = html.Replace("{{Name}}", name);
        messageMail.From.Add(new MailboxAddress("Purpuramusic", "purpuramusic1@gmail.com"));
        messageMail.To.Add(new MailboxAddress(name, email));
        messageMail.Subject = "Your password has been changed on Purpura Music";
        messageMail.Body = new TextPart("html") { Text = html };
        
        return await SendEmail(messageMail);
    }

    /// <summary>
    /// Método genérico para enviar correos electrónicos utilizando SMTP.
    /// </summary>
    public async Task<bool> SendEmail(MimeMessage message)
    {
      using var client = new SmtpClient();
      try
      {
        var apiKey = _configuration["Gmail:Key"];
        var gmail = _configuration["Gmail:Mail"];

        // Conexión segura al servidor SMTP
        await client.ConnectAsync("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
        
        // Autenticación con Gmail
        await client.AuthenticateAsync(gmail, apiKey);

        // Envío del mensaje
        await client.SendAsync(message);
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error al enviar el email: {ex.Message}");
        throw new Exception($"Error sending email: {ex.Message}");
      }
      finally
      {
          if (client.IsConnected)
          {
              await client.DisconnectAsync(true);
          }
      }
    }
}