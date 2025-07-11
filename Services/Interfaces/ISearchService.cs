using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;



public interface ISearchService
{

  Task<GetSearchDTO> GetSearch(string userId, string input);
}