using Microsoft.AspNetCore.Mvc;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Services;
using purpuraMain.Services.Interfaces;

namespace purpuraMain.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController : ControllerBase
{
    private readonly ICountryService _countryService;
    public CountryController(ICountryService countryService)
    {
        _countryService = countryService;
    }


    [HttpGet("getAll")]
    public async Task<ActionResult<List<GetCountriesDTO>>> GetAllCountries()
    {
        try
        {
            var countries = await _countryService.GetCountries();
            return Ok(countries);
        }
          catch (System.Exception)
        {
            throw new HttpResponseException(500, new {Message="An unexpected error occured", Success = false});
        }
    }
}