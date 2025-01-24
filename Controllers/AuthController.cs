using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto;
using purpuraMain.Dto.InputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly PurpuraDbContext _dbContext;
  private readonly IConfiguration _config;

   public AuthController(PurpuraDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _config = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO login)
    {
        try
        {
          if (login.Email == null || login.Password == null) throw new BadRequestException("Email or password cannot be null");
          var response = await AuthServices.LoginRequest(login.Email, login.Password, _dbContext, _config);
          return Ok(response);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (System.Exception)
        {
            return BadRequest("An unexpected error occured");
        }
    }
    [HttpPut("refresh/token")]
    [Authorize]
    public async Task<ActionResult<object>> RefreshToken()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new SessionExpiredException("Invalid token");
            var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value ?? throw new SessionExpiredException("Invalid token");
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? throw new BadRequestException("Invalid token");
            var token = await AuthServices.RefreshTokenRequest(userId, sessionId, email, _dbContext, _config);
            return Ok(new {token});
        }
        catch(SessionExpiredException ex)
        {
            return Unauthorized(ex.Message);

        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (System.Exception)
        {
            return BadRequest("An unexpected error occured");
        }
    }


    [HttpDelete("logout")]
    [Authorize]

    public async Task<ActionResult<object>> Logout()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new SessionExpiredException("Invalid token");
            var sessionId = User.FindFirst(ClaimTypes.SerialNumber)?.Value ?? throw new SessionExpiredException("Invalid token");
            var response = await AuthServices.LogoutRequest(userId, sessionId, _dbContext);
            if(!response) throw new BadRequestException("An error occured while logging out");
            return Ok("Logged out successfully");
        }
        catch(SessionExpiredException ex)
        {
            return Unauthorized(ex.Message);

        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (System.Exception)
        {
            return BadRequest("An unexpected error occured");
        }
    }

}