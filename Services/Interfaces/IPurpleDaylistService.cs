using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Model;

namespace purpuraMain.Services.Interfaces;


public interface IPurpleDaylistService
{
  Task<GetPlayListDTO> GetPurpleDaylist(string userId);
  Task<bool> UpdatePurpuraDayList(string playlistId, string userId);
  Task<List<Song>> GetUserRecomendations(string userId);
  Task<bool> ClearPlaylist(string playlistId);
}