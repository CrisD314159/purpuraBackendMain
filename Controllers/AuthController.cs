using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Dto.InputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;


/// Controlador de autenticación para manejar el inicio de sesión, 
/// la renovación de tokens y el cierre de sesión de los usuarios.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;
    private readonly IConfiguration _config;


    /// Constructor del controlador de autenticación.
    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    public AuthController(PurpuraDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _config = configuration;
    }


    /// Inicia sesión con email y contraseña.
    /// <param name="login">Datos del usuario para autenticación.</param>
    /// <returns>Un token de acceso y un refresh token si la autenticación es exitosa.</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO login)
    {
        try
        {
            if (login.Email == null || login.Password == null)
                throw new BadRequestException("Email or password cannot be null");

            var response = await AuthServices.LoginRequest(login.Email, login.Password, _dbContext, _config);
            return Ok(response);
        }
        catch (NotVerifiedException ex)
        {
            return Unauthorized(new {message = ex.Message, success = false}); // Usuario no ha verificado su cuenta
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new {message = ex.Message, success = false});
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "An unexpected error occurred", success = false});
        }
    }


    /// Renueva el token de autenticación si el refresh token es válido.
    /// <returns>Nuevo token de acceso y refresh token.</returns>
    [HttpPut("refresh/token")]
    [Authorize]
    public async Task<ActionResult<LoginResponseDTO>> RefreshToken()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new SessionExpiredException("Invalid token");
            var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value ?? throw new SessionExpiredException("Invalid token");
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? throw new BadRequestException("Invalid token");

            var token = await AuthServices.RefreshTokenRequest(userId, sessionId, email, _dbContext, _config);
            return Ok(new { success = true, token = token.Token, refreshToken = token.RefreshToken });
        }
        catch (SessionExpiredException ex)
        {
            return Unauthorized(new {message = ex.Message, success = false});
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new {message = ex.Message, success = false});
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new {message = ex.Message, success = false});
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "An unexpected error occurred", success = false});
        }
    }


    /// Cierra la sesión del usuario actual, invalidando el token de sesión.
    /// <returns>Mensaje confirmando el cierre de sesión.</returns>
    [HttpDelete("logout")]
    [Authorize]
    public async Task<ActionResult<object>> Logout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new SessionExpiredException("Invalid token");
            var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value ?? throw new SessionExpiredException("Invalid token");

            var response = await AuthServices.LogoutRequest(userId, sessionId, _dbContext);
            if (!response)
                throw new BadRequestException("An error occurred while logging out");

            return Ok("Logged out successfully");
        }
        catch (SessionExpiredException ex)
        {
            return Unauthorized(new {message = ex.Message, success = false});
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new {message = ex.Message, success = false});
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new {message = ex.Message, success = false});
        }
        catch (System.Exception)
        {
            return BadRequest(new {message = "An unexpected error occurred", success = false});
        }
    }


    /// Verifica si el token del usuario es válido.
    /// <returns>Un mensaje confirmando si el token es válido o no.</returns>
    [HttpPut("checkToken")]
    [Authorize]
    public ActionResult<object> CheckToken()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new SessionExpiredException("Invalid token");
            return Ok(new { success = true, message = "Token is valid" });
        }
        catch (SessionExpiredException ex)
        {
            return Unauthorized(new { message = ex.Message, success = false });
        }
        catch (System.Exception)
        {
            return BadRequest(new { message = "An unexpected error occurred", success = false });
        }
    }
}