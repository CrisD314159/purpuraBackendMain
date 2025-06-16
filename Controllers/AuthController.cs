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
/// Constructor del controlador de autenticación.
/// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
/// <param name="configuration">Configuración de la aplicación.</param>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;


  /// Inicia sesión con email y contraseña.
  /// <param name="login">Datos del usuario para autenticación.</param>
  /// <returns>Un token de acceso y un refresh token si la autenticación es exitosa.</returns>
  [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
    {

        if (login.Email == null || login.Password == null)
            throw new BadRequestException("Email or password cannot be null");

        var response = await _authService.LoginRequest(login.Email, login.Password);
        return Ok(response);

    }


    /// Renueva el token de autenticación si el refresh token es válido.
    /// <returns>Nuevo token de acceso y refresh token.</returns>
    [HttpPut("/refreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDTO refreshTokenDTO)
    {

        var token = await _authService.RefreshTokenRequest(refreshTokenDTO);
        return Ok(new { success = true, token = token.Token, refreshToken = token.RefreshToken });

    }


    /// Cierra la sesión del usuario actual, invalidando el token de sesión.
    /// <returns>Mensaje confirmando el cierre de sesión.</returns>
    [HttpDelete("logout")]
  
    public async Task<IActionResult> Logout(RefreshTokenDTO refreshTokenDTO)
    {
        await _authService.LogoutRequest(refreshTokenDTO);
        return Ok("Logged out successfully");
    }


    /// Verifica si el token del usuario es válido.
    /// <returns>Un mensaje confirmando si el token es válido o no.</returns>
    [HttpPut("checkToken")]
    [Authorize]
    public ActionResult<object> CheckToken()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
        return Ok(new { success = true, message = "Token is valid" });

    }
}