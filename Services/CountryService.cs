using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services;

public static class CountryService
{


    public static async Task<List<GetCountriesDTO>> GetCountries(PurpuraDbContext dbContext)
    {
        try
        {
            var countries = await dbContext.Countries!.Select(c => new GetCountriesDTO{ Id = c.Id, Name = c.Name}).OrderBy(c=> c.Name).ToListAsync();
            return countries;
        }
        catch (System.Exception)
        {
            throw new Exception("An error occured while fetching the countries");
        }
    }
}