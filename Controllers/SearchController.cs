namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{

  private readonly ISearchService _searchService;
  public SearchController(ISearchService searchService)
  {
    _searchService = searchService;
  }

  [HttpGet("input")]
  [Authorize]
  public async Task<ActionResult<GetSearchDTO>> SearchInput (string search)
  {
      if(string.IsNullOrEmpty(search)) throw new BadRequestException("Search is required");
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
      throw new UnauthorizedException(" You're not authorized to perform this action");
      var results = await _searchService.GetSearch(userId, search);
      return Ok(results);
  }
  [HttpGet("input/public")]
  public async Task<ActionResult<GetSearchDTO>> SearchInputPublic (string search)
  {

      if(string.IsNullOrEmpty(search)) throw new BadRequestException("Search is required");
      var results = await _searchService.GetSearch("0", search);
      return Ok(results);


  }

}