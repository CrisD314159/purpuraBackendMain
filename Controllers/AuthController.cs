using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Dto.InputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


/// Controlador de autenticación para manejar el inicio de sesión, 
/// la renovación de tokens y el cierre de sesión de los usuarios.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;


    /// Constructor del controlador de autenticación.
    /// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
    /// <param name="configuration">Configuración de la aplicación.</param>
    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
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
                throw new BadRequestException(400, new {Message = "Email or password cannot be null", Success = false});

            var response = await _authService.LoginRequest(login.Email, login.Password, _config);
            return Ok(response);
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
            var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});

            var token = await _authService.RefreshTokenRequest(userId, sessionId, email, _config);
            return Ok(new { success = true, token = token.Token, refreshToken = token.RefreshToken });
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
            var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});

            var response = await _authService.LogoutRequest(userId, sessionId);
            if (!response)
                throw new HttpResponseException(500, new{Message = "An error occurred while logging out", Success = false});

            return Ok("Logged out successfully");
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
            return Ok(new { success = true, message = "Token is valid" });
        }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}