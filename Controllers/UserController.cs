using Microsoft.AspNetCore.Mvc;
using purpuraMain.Dto.InputDto;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using purpuraMain.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace purpuraMain.Controllers;

/// Controlador para la gestión de usuarios.
/// Constructor del controlador de usuarios.
/// <param name="dbContext">Contexto de base de datos.</param>
[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;


  /// Obtiene la información del usuario autenticado.
    [HttpGet]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetUser()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new UnauthorizedAccessException("User not found");
        var user = await _userService.GetUserById(userId);
        return Ok(user);
   
    }


    /// Crea un nuevo usuario.
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDTO user)
    {

        await _userService.CreateUser(user);
        return CreatedAtAction("GetUser", new { success = true, message = "User created successfully" });

    }


    /// Actualiza la información del usuario autenticado.
    [HttpPut]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateUser(UpdateUserDto user)
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new UnauthorizedAccessException("User not found");
        await _userService.UpdateUser(userId, user);
        return Ok("User updated successfully");

    }


    /// Elimina la cuenta del usuario autenticado.
    [HttpDelete]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteUser()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new UnauthorizedAccessException("User not found");
        await _userService.DeleteUser(userId);
        return Ok("User deleted successfully");

    }


    /// Verifica la cuenta de usuario.
    [HttpPut("verifyAccount")]
    public async Task<IActionResult> VerifyAccount(VerifyAccountDTO verifyAccount)
    {

        await _userService.VerifyAccount(verifyAccount);
        return Ok("Account verified successfully");

    }


    /// <summary>
    /// Envia un correo electrónico con un código de recuperación de contraseña.
    /// </summary>
    /// <param name="sendRecoverEmail"></param>
    /// <returns></returns>
    [HttpPost("sendRecoverEmail")]
    public async Task<IActionResult> SendRecoverEmail(SendRecoverEmailDTO sendRecoverEmail)
    {

        await _userService.SendPasswordRecoveryCode(sendRecoverEmail.Email);
        return Ok("Recover email sent successfully");

    }

    /// <summary>
    /// Cambia la contraseña de un usuario usando un código de verificación previamente enviado.
    /// </summary>
    /// <param name="changePassword"></param>
    /// <returns></returns>
    [HttpPatch("changePassword")]
    public async Task<IActionResult> ChangePassword(PasswordChangeDTO changePassword)
    {

        await _userService.UpdateUserPassword(changePassword);
        return Ok("Password changed successfully");
        

    }
}
