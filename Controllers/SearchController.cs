namespace purpuraMain.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Services.Interfaces;

[ApiController]
[Route("[controller]")]
public class SearchController(ISearchService searchService) : ControllerBase
{

  private readonly ISearchService _searchService = searchService;

  [HttpGet("input")]
  [AllowAnonymous]
  public async Task<ActionResult<GetSearchDTO>> SearchInput (string search)
  {
    var result = await HttpContext.AuthenticateAsync("Bearer");

    string userId = "0";
    if (result.Succeeded && result.Principal != null)
    {
        userId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
    }
    var results = await _searchService.GetSearch(userId, search);
    return Ok(results);
  }
}