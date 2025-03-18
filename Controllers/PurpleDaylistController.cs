using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
public class PurpleDaylistController : ControllerBase
{
  private readonly IPurpleDaylistService _purpleDaylistService;

  public PurpleDaylistController(IPurpleDaylistService purpleDaylistService)
  {
    _purpleDaylistService = purpleDaylistService;
  }

  [HttpGet("gerPurpleDaylist")]
  public async Task<ActionResult<GetPlayListDTO>> GetPurpleDaylist()
  {
    try
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
      throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
      var purpleDaylist = await _purpleDaylistService.GetPurpleDaylist(userId);
      return Ok(purpleDaylist);
    }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
  }



}