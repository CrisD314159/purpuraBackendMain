namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
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
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User not found");
      var results = await SearchServices.GetSearch(userId, search, _dbcontext);
      return Ok(results);
    }
    catch(UnauthorizedAccessException e){
      return Unauthorized(new {message = e.Message, success = false});
    }
    catch (System.Exception)
    {
      
      return BadRequest(new {message ="An error occured while searching the data", success = false});
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
    catch(UnauthorizedAccessException e){
      return Unauthorized(new {message = e.Message, success = false});
    }
    catch (System.Exception)
    {
      
      return BadRequest(new {message ="An error occured while searching the data", success = false});
    }

  }

}