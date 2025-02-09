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

/// <summary>
/// Controlador para la gestión de usuarios.
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    /// <summary>
    /// Constructor del controlador de usuarios.
    /// </summary>
    /// <param name="dbContext">Contexto de base de datos.</param>
    public UserController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado.
    /// </summary>
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

    /// <summary>
    /// Crea un nuevo usuario.
    /// </summary>
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

    /// <summary>
    /// Actualiza la información del usuario autenticado.
    /// </summary>
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

    /// <summary>
    /// Elimina la cuenta del usuario autenticado.
    /// </summary>
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

    /// <summary>
    /// Verifica la cuenta de usuario.
    /// </summary>
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
}
