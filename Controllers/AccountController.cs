using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Dto.InputDto;
using purpuraMain.Exceptions;
using purpuraMain.Model;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


/// Controlador de autenticación para manejar el inicio de sesión, 
/// la renovación de tokens y el cierre de sesión de los usuarios.
/// Constructor del controlador de autenticación.
/// <param name="dbContext">Contexto de la base de datos de la aplicación.</param>
/// <param name="configuration">Configuración de la aplicación.</param>
[ApiController]
[Route("[controller]")]
public class AccountController(IAuthService authService, SignInManager<User> signInManager, IConfiguration configuration) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    private readonly SignInManager<User> _signInManager = signInManager;

    private readonly IConfiguration _configuration = configuration;


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
    [HttpPut("refreshToken")]
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("checkToken")]
    public IActionResult CheckToken()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("You're not authorized to perform this action");
        return Ok(new { success = true, message = "Token is valid" });

    }
    

    [HttpGet("api/login/google")]
    public IActionResult GoogleSignIn([FromQuery] string returnUrl="/")
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google",
        Url.Action("ExternalLoginCallback", "Account", new { returnUrl }));

        return Challenge(properties, "Google");
    }

    [HttpGet("api/login/google/callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
    {
        var userInfo = await _signInManager.GetExternalLoginInfoAsync();
        var frontUrl = _configuration["Front:Url"];

        if (userInfo == null)
        {
            return Redirect($"{frontUrl}/login?error=login_failed");
        }

        var email = userInfo.Principal.FindFirstValue(ClaimTypes.Email);
        var name = userInfo.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
        {
            return Redirect($"{frontUrl}/login?error=login_failed");
        }

        var tokenSession = await _authService.SignInUsingGoogle(email, name);

        return Redirect($"{frontUrl}/api/auth/google?token={tokenSession.Token}&refresh={tokenSession.RefreshToken}");
    }
}