using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;

/// Controlador para la gestión de usuarios.
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;


    /// Constructor del controlador de usuarios.
    /// <param name="dbContext">Contexto de base de datos.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    /// Obtiene la información del usuario autenticado.
    [HttpGet("getProfile")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser()
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
            var user = await _userService.GetUserById(userId);
            return Ok(user);
   
    }


    /// Crea un nuevo usuario.
    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDTO user)
    {

            await _userService.CreateUser(user);
            return CreatedAtAction("GetUser", new { success = true, message = "User created successfully" });

    }


    /// Actualiza la información del usuario autenticado.
    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(UpdateUserDto user)
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
            await _userService.UpdateUser(userId, user);
            return Ok("User updated successfully");

    }


    /// Elimina la cuenta del usuario autenticado.
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser()
    {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
            await _userService.DeleteUser(userId);
            return Ok("User deleted successfully");

    }


    /// Verifica la cuenta de usuario.
    [HttpPut("verifyAccount")]
    public async Task<ActionResult> VerifyAccount(VerifyAccountDTO verifyAccount)
    {

            if (verifyAccount.Email == null || verifyAccount.Code == 0)
            {
                return BadRequest("Email and code are required");
            }
            await _userService.VerifyAccount(verifyAccount);
            return Ok("Account verified successfully");

    }


    /// <summary>
    /// Envia un correo electrónico con un código de recuperación de contraseña.
    /// </summary>
    /// <param name="sendRecoverEmail"></param>
    /// <returns></returns>
    [HttpPost("sendRecoverEmail")]
    public async Task<ActionResult> SendRecoverEmail(SendRecoverEmailDTO sendRecoverEmail)
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
    public async Task<ActionResult> ChangePassword(PasswordChangeDTO changePassword)
    {

            await _userService.UpdateUserPassword(changePassword);
            return Ok("Password changed successfully");
        

    }
}
