namespace purpuraMain.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Services.Interfaces;

public class CountryService(PurpuraDbContext dbContext) : ICountryService
{

    private readonly PurpuraDbContext _dbContext = dbContext;


    /// <summary>
    /// Obtiene todos los países disponibles.
    /// </summary>
    public async Task<List<GetCountriesDTO>> GetCountries()
    {
        try
        {
            var countries = await _dbContext.Countries!.Select(c => new GetCountriesDTO{ Id = c.Id, Name = c.Name}).OrderBy(c=> c.Name).ToListAsync();
            return countries;
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}