namespace purpuraMain.Utils;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using DotNetEnv;

/// <summary>
/// Servicio de correo electrónico para enviar notificaciones y códigos de verificación.
/// </summary>
public static class MailService
{
    /// <summary>
    /// Envía un correo electrónico con un código de verificación.
    /// </summary>
    /// <param name="email">Correo del destinatario.</param>
    /// <param name="verificationCode">Código de verificación.</param>
    /// <param name="name">Nombre del destinatario.</param>
    public static async Task<bool> SendVerificationEmail(string email, string verificationCode, string name)
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
    public static async Task<bool> SendPasswordRecoveryCodeEmail(string email, string verificationCode, string name)
    { 
        var messageMail = new MimeMessage();
        var linkCode = $"https://purpura-music.vercel.app/changePassword?token={verificationCode}";
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
    public static async Task<bool> SendVerifiedAccountMail(string email, string name)
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
    public static async Task<bool> SendPasswordChangeMail(string email, string name)
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
    private static async Task<bool> SendEmail(MimeMessage message)
    {
        using var client = new SmtpClient();
        try
        {
            var apiKey = Env.GetString("GEMAIL");
            var gmail = Env.GetString("GMAIL");

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
