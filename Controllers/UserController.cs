namespace purpuraMain.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;


[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private readonly PurpuraDbContext _dbContext;

    public UserController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserDto>> GetUser(string id)
    {
        try
        {
        var user = await UserService.GetUserById(id, _dbContext);

        return Ok(user);
        }
        catch (ValidationException val)
        {
          return BadRequest(new {Title ="There was an error with the input", Detail = val.Message});
        }
        catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
        catch (System.Exception)
        {
          NotFound();
        }     

        return NotFound();   
    }


      [HttpGet("getByEmail/{email}")]
        public async Task<ActionResult<GetUserDto>> GetUserByEmail(string email)
    {
        try
        {
        var user = await UserService.GetUserByEmail(email, _dbContext);
        return Ok(user); 
        }
         catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
        catch (System.Exception)
        {
          NotFound();
        }     

        return NotFound();   
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDTO user)
    {
      try
      {
        var returnStatement = await UserService.CreateUser(user, _dbContext);

        if(returnStatement == false)
        {
          return BadRequest("An error occured while creating the user");
        }

        return CreatedAtAction(nameof(GetUserByEmail),new {email = user.Email}, user);
      }
      catch(ValidationException val)
      {
        return BadRequest(new {Title ="There was an error with the input", Detail = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(e.Message);
      }        
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UpdateUserDto user )
    {
      try
      {
        var serviceResponse = await UserService.UpdateUser(user, _dbContext);

        if(serviceResponse == false)
        {
          return BadRequest("An error occured while updating the user");
        }

        return Ok("User updated successfully");
        
      }
      catch(ValidationException val)
      {
        return BadRequest(new {Title ="There was an error with the input", Detail = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {Title ="An unexpected error occured", Detail = e.Message});
      }

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
      try
      {
        var serviceResponse = await UserService.DeleteUser(id, _dbContext);

        if(serviceResponse == false)
        {
          return BadRequest("An error occured while deleting the user");
        }

        return Ok("User deleted successfully");
        
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {Title ="An unexpected error occured", Detail = e.Message});
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

        if(serviceResponse == false)
        {
          return BadRequest("An error occured while verifying the account");
        }
        
        return Ok("Account verified successfully");
        
        
      }
      catch(ValidationException val)
      {
        return BadRequest(new {Title ="There was an error with the input", Detail = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {Title ="An unexpected error occured", Detail = e.Message});
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

        if(serviceResponse == false)
        {
          return BadRequest("An error occured while verifying the account");
        }
        
        return Ok("Password updated successfully");
        
        
      }
      catch(ValidationException val)
      {
        return BadRequest(new {Title ="There was an error with the input", Detail = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {Title ="An unexpected error occured", Detail = e.Message});
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

        if(serviceResponse == false)
        {
          return BadRequest("An error occured while sending the verification email");
        }
        
        return Ok("Recovery email sent successfully");
        
        
      }
      catch(ValidationException val)
      {
        return BadRequest(new {Title ="There was an error with the input", Detail = val.Message});
      }
       catch (EntityNotFoundException ex)
        {
          return BadRequest(ex.Message);
        }
      catch (System.Exception e)
      {
        
        return BadRequest(new {Title ="An unexpected error occured", Detail = e.Message});
      }

    }




}