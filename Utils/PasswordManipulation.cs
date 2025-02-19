using Microsoft.AspNetCore.Identity;
namespace purpuraMain.Utils
{

public class PasswordManipulation
{   

    /// <summary>
    /// Método que recibe una contraseña y devuelve su hash.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
     public string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        return passwordHasher.HashPassword(new object(), password); // Devuelve el hash de la contraseña
    }

    /// <summary>
    /// Método que recibe un hash de contraseña y la contraseña proporcionada, y devuelve si son iguales.
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(new object(), hashedPassword, providedPassword);

        return result == PasswordVerificationResult.Success;
    }
}
}