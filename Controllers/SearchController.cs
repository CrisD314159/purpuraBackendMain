namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{

  private readonly PurpuraDbContext _dbcontext;
  public SearchController(PurpuraDbContext dbContext)
  {
    _dbcontext = dbContext;
  }

  [HttpGet("input")]
  [Authorize]
  public async Task<ActionResult<GetSearchDTO>> SearchInput (string search)
  {
    try
    {
      if(string.IsNullOrEmpty(search)) throw new Exception("Search is required");
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
      throw new UnauthorizedException(401 , new {Message = " You're not authorized to perform this action"});
      var results = await SearchServices.GetSearch(userId, search, _dbcontext);
      return Ok(results);
    }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }

  }
  [HttpGet("input/public")]
  public async Task<ActionResult<GetSearchDTO>> SearchInputPublic (string search)
  {
    try
    {
      if(string.IsNullOrEmpty(search)) throw new Exception("Search is required");
      var results = await SearchServices.GetSearch("0", search, _dbcontext);
      return Ok(results);
    }
        catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }

  }

}