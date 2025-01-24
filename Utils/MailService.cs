namespace purpuraMain.Utils;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using DotNetEnv;

public static class MailService
{
    public static async Task<bool> SendVerificationEmail(string email, string verificationCode, string name)
 { 
     var messageMail = new MimeMessage();
     var html = File.ReadAllText("./templates/mailVerificationCodeTemplate.html");
     html = html.Replace("{{Name}}", name);
     html = html.Replace("{{VerificationCode}}", verificationCode);
     messageMail.From.Add(new MailboxAddress("Purpura Music", "purpuramusic1@gmail.com"));
     messageMail.To.Add(new MailboxAddress(name, email));
     messageMail.Subject = "Aquí está tu código de verificación de Purpura Music";
     messageMail.Body = new TextPart("html")
     {
         Text = html
     };
     try
     {
        var res = await SendEmail(messageMail);
       return res;
     }
     catch (Exception ex)
     {
         throw new Exception($"Error sending email: {ex.Message}");
     }
 }


   public static async Task<bool> SendPasswordRecoveryCodeEmail(string email, string verificationCode, string name)
{ 
    var messageMail = new MimeMessage();
    var html = File.ReadAllText("./templates/paswordChangeCodeTemplate.html");
    html = html.Replace("{{Name}}", name);
    html = html.Replace("{{VerificationCode}}", verificationCode);
    messageMail.From.Add(new MailboxAddress("Purpuramusic", "purpuramusic1@gmail.com"));
    messageMail.To.Add(new MailboxAddress(name, email));
    messageMail.Subject = "Aquí está tu código de recuperación de Purpura Music";
    messageMail.Body = new TextPart("html")
    {
        Text = html
    };

    try
    {
       var res = await SendEmail(messageMail);
      return res;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error sending email: {ex.Message}");
    }
}


public static async Task<bool> SendVerifiedAccountMail(string email, string name)
{ 
    var messageMail = new MimeMessage();
    var html = File.ReadAllText("./templates/accountVerifiedTemplate.html");
    html = html.Replace("{{Name}}", name);
    messageMail.From.Add(new MailboxAddress("Purpuramusic", "purpuramusic1@gmail.com"));
    messageMail.To.Add(new MailboxAddress(name, email));
    messageMail.Subject = "Gracias por verificar tu cuenta en Purpura Music";
    messageMail.Body = new TextPart("html")
    {
        Text = html
    };

    try
    {
       var res = await SendEmail(messageMail);
      return res;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error sending email: {ex.Message}");
    }
}


public static async Task<bool> SendPasswordChangeMail(string email, string name)
{ 
    var messageMail = new MimeMessage();
    var html = File.ReadAllText("./templates/passwordChangeTemplate.html");
    html = html.Replace("{{Name}}", name);
    messageMail.From.Add(new MailboxAddress("Purpuramusic", "purpuramusic1@gmail.com"));
    messageMail.To.Add(new MailboxAddress(name, email));
    messageMail.Subject = "Tu contraseña ha sido cambiada en Purpura Music";
    messageMail.Body = new TextPart("html")
    {
        Text = html
    };

    try
    {
       var res = await SendEmail(messageMail);
      return res;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error sending email: {ex.Message}");
    }
}
public static async Task<bool> SendEmail(MimeMessage message)
{
    using var client = new SmtpClient();
    try
    {
        var apiKey = Env.GetString("GEMAIL");
        var gmail = Env.GetString("GMAIL");


        // Conexión segura al servidor SMTP
        await client.ConnectAsync("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

        // Autenticación con SendGrid
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