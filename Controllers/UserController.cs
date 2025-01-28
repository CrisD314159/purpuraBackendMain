namespace purpuraMain.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    public UserController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    [HttpGet("getProfile")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser()
    {
        try
        {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
        var user = await UserService.GetUserById(userId, _dbContext);

        return Ok(user);
        }
        catch (ValidationException val)
        {
          return BadRequest(new  {success =false, message = val.Message});
        }
        catch (EntityNotFoundException ex)
        {
          return NotFound( new {success =false, message = ex.Message});
        }
        catch (System.Exception e)
        {
          return BadRequest(new {success =false, message = e.Message});
        }     
  
    }



    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDTO user)
    {
      try
      {
        var returnStatement = await UserService.CreateUser(user, _dbContext);

        return CreatedAtAction("GetUser", new {success = true, message = "User created successfully"});
      }
      catch(ValidationException val)
      {
        return BadRequest(new  {success =false, message = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(new  {success =false, message = ex.Message});
        }
      catch (System.Exception e)
      {
        
        return BadRequest( new {success =false, message = e.Message});
      }        
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> UpdateUser(UpdateUserDto user )
    {
      try
      {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
        var serviceResponse = await UserService.UpdateUser(userId, user, _dbContext);
        return Ok("User updated successfully");
        
      }
       catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
      catch(ValidationException val)
      {
        return BadRequest(new  {success =false, message = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new  {success =false, message = e.Message});
      }

    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteUser()
    {
      try
      {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
        var serviceResponse = await UserService.DeleteUser(userId, _dbContext);

        return Ok("User deleted successfully");
        
      }
       catch( UnauthorizedAccessException ex){
      return Unauthorized(ex.Message);
    }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(new  {success =false, message = ex.Message});
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new  {success =false, message = e.Message});
      }

    }


    [HttpPut("verifyAccount")]
    public async Task<ActionResult> VerifyAccount(VerifyAccountDTO verifyAccount)
    {
      try
      {
        if(verifyAccount.Email == null || verifyAccount.Code == 0)
        {
          return BadRequest("Email and code are required");
        }
       var serviceResponse =  await UserService.VerifyAccount(verifyAccount, _dbContext);
        
        return Ok("Account verified successfully");
      
      }
      catch(ValidationException val)
      {
        return BadRequest(new {success =false, message = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {success =false, message = e.Message});
      }

    }


   [HttpPut("changePassword")]
    public async Task<ActionResult> ChangePassword(PasswordChangeDTO passwordChange)
    {
      try
      {
        if(passwordChange.Email == null || passwordChange.Code == 0 || passwordChange.Password == null)
        {
          return BadRequest("Email, code and password are required");
        }
       var serviceResponse =  await UserService.UpdateUserPassword(passwordChange, _dbContext);
        
        return Ok("Password updated successfully");
        
        
      }
      catch(ValidationException val)
      {
        return BadRequest(new {success =false, message = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {success =false, message = e.Message});
      }

    }


    [HttpPost("sendRecoveryMail")]
    public async Task<ActionResult> SendVerificationMail([FromQuery] string email)
    {
      try
      {
        if(email == null)
        {
          return BadRequest("Email is required");
        }
       var serviceResponse =  await UserService.SendPasswordRecoveryCode(email, _dbContext);
        
        return Ok("Recovery email sent successfully");
        
        
      }
      catch(ValidationException val)
      {
        return BadRequest(new  {success =false, message = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new  {success =false, message = e.Message});
      }

    }




}