using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController : ControllerBase
{
    private readonly PurpuraDbContext _dbContext;
    public CountryController(PurpuraDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }


    [HttpGet("getAll")]
    public async Task<ActionResult<List<GetCountriesDTO>>> GetAllCountries()
    {
        try
        {
            var countries = await CountryService.GetCountries(_dbContext);
            return Ok(countries);
        }
          catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}