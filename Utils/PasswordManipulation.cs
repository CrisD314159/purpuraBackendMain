using Microsoft.AspNetCore.Identity;
namespace purpuraMain.Utils
{

public class PasswordManipulation
{   
     public string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        return passwordHasher.HashPassword(new object(), password); // Devuelve el hash de la contraseña
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(new object(), hashedPassword, providedPassword);

        return result == PasswordVerificationResult.Success;
    }
}
}