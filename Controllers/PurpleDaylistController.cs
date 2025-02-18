using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
public class PurpleDaylistController : ControllerBase
{
  private readonly PurpuraDbContext _dbContext;

  public PurpleDaylistController(PurpuraDbContext dbContext)
  {
    _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
  }

  [HttpGet("gerPurpleDaylist")]
  public async Task<ActionResult<GetPlayListDTO>> GetPurpleDaylist()
  {
    try
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      var purpleDaylist = await PurpleDaylistService.GetPurpleDaylist(userId, _dbContext);
      return Ok(purpleDaylist);
    }
    catch (EntityNotFoundException)
    {
      return NotFound(new { message = "Purple Daylist Not found", success = false });
    }
    catch (System.Exception)
    {
      return BadRequest(new { message = "An unexpected error occurred", success = false });
    }
  }



}