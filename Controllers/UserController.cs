using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace purpuraMain.Controllers;

/// Controlador para la gestión de usuarios.
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;


    /// Constructor del controlador de usuarios.
    /// <param name="dbContext">Contexto de base de datos.</param>
    public UserController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }


    /// Obtiene la información del usuario autenticado.
    [HttpGet("getProfile")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
            var user = await UserService.GetUserById(userId, _dbContext);
            return Ok(user);
        }
        catch (ValidationException val)
        {
            return BadRequest(new { success = false, message = val.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }


    /// Crea un nuevo usuario.
    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDTO user)
    {
        try
        {
            await UserService.CreateUser(user, _dbContext);
            return CreatedAtAction("GetUser", new { success = true, message = "User created successfully" });
        }
        catch (ValidationException val)
        {
            return BadRequest(new { success = false, message = val.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }


    /// Actualiza la información del usuario autenticado.
    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(UpdateUserDto user)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
            await UserService.UpdateUser(userId, user, _dbContext);
            return Ok("User updated successfully");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ValidationException val)
        {
            return BadRequest(new { success = false, message = val.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }


    /// Elimina la cuenta del usuario autenticado.
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User not found");
            await UserService.DeleteUser(userId, _dbContext);
            return Ok("User deleted successfully");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message, success = false });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }


    /// Verifica la cuenta de usuario.
    [HttpPut("verifyAccount")]
    public async Task<ActionResult> VerifyAccount(VerifyAccountDTO verifyAccount)
    {
        try
        {
            if (verifyAccount.Email == null || verifyAccount.Code == 0)
            {
                return BadRequest("Email and code are required");
            }
            await UserService.VerifyAccount(verifyAccount, _dbContext);
            return Ok("Account verified successfully");
        }
        catch (ValidationException val)
        {
            return BadRequest(new { success = false, message = val.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }


    /// <summary>
    /// Envia un correo electrónico con un código de recuperación de contraseña.
    /// </summary>
    /// <param name="sendRecoverEmail"></param>
    /// <returns></returns>
    [HttpPost("sendRecoverEmail")]
    public async Task<ActionResult> SendRecoverEmail(SendRecoverEmailDTO sendRecoverEmail)
    {
        try
        {
            await UserService.SendPasswordRecoveryCode(sendRecoverEmail.Email, _dbContext);
            return Ok("Recover email sent successfully");
        }
        catch (NotVerifiedException val)
        {
            return BadRequest(new { success = false, message = val.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }

    /// <summary>
    /// Cambia la contraseña de un usuario usando un código de verificación previamente enviado.
    /// </summary>
    /// <param name="changePassword"></param>
    /// <returns></returns>
    [HttpPatch("changePassword")]
    public async Task<ActionResult> ChangePassword(PasswordChangeDTO changePassword)
    {
        try
        {
            await UserService.UpdateUserPassword(changePassword, _dbContext);
            return Ok("Password changed successfully");
        }
        catch (ValidationException val)
        {
            return BadRequest(new { success = false, message = val.Message });
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            return BadRequest(new { success = false, message = e.Message });
        }
    }
}
