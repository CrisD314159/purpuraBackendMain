using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface ICountryService
{
  Task<List<GetCountriesDTO>> GetCountries();
}