namespace purpuraMain.Utils;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;

public static class MailService
{
   public static async Task<bool> SendVerificationEmail(string email, string verificationCode, string name)
{ 
    var messageMail = new MimeMessage();
    var html = File.ReadAllText("./templates/mailVerificationCodeTemplate.html");
    html = html.Replace("{{Name}}", name);
    html = html.Replace("{{VerificationCode}}", verificationCode);
    messageMail.From.Add(new MailboxAddress("Rebeca Hermiston", "rebeca.hermiston53@ethereal.email"));
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
    messageMail.From.Add(new MailboxAddress("Rebeca Hermiston", "rebeca.hermiston53@ethereal.email"));
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
    messageMail.From.Add(new MailboxAddress("Rebeca Hermiston", "rebeca.hermiston53@ethereal.email"));
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
    messageMail.From.Add(new MailboxAddress("Rebeca Hermiston", "rebeca.hermiston53@ethereal.email"));
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
  try
    {
        using var client = new SmtpClient();

        // Conectarse al servidor SMTP usando el puerto correcto y una conexión segura
        await client.ConnectAsync("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);

        // Autenticar usuario
        await client.AuthenticateAsync("rebeca.hermiston53@ethereal.email", "TEPDZHK8PC7eWt3ex3");

        // Enviar el correo
        await client.SendAsync(message);

        // Desconectar y liberar recursos
        await client.DisconnectAsync(true);

        Console.WriteLine("Email sent successfully!");
        return true;
    }
    catch (Exception ex)
    {
        throw new Exception($"Error sending email: {ex.Message}");
    }
}



}