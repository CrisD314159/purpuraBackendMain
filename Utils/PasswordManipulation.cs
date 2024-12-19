using Microsoft.AspNetCore.Identity;
namespace purpuraMain.Utils
{

public class PasswordManipulation
{   
     public string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        return passwordHasher.HashPassword(null, password); // Devuelve el hash de la contraseña
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);

        return result == PasswordVerificationResult.Success;
    }
}
}