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
public class PurpleDaylistController(IPurpleDaylistService purpleDaylistService) : ControllerBase
{
  private readonly IPurpleDaylistService _purpleDaylistService = purpleDaylistService;

  [HttpGet("gerPurpleDaylist")]
  public async Task<IActionResult> GetPurpleDaylist()
  {

      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
      throw new UnauthorizedException("You're not authorized to perform this action");
      var purpleDaylist = await _purpleDaylistService.GetPurpleDaylist(userId);
      return Ok(purpleDaylist);
  }



}