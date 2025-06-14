using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IPlaylistService
{
  Task<GetPlayListDTO> GetPlaylist(string userId, Guid playListId);

  Task<List<GetLibraryPlaylistDTO>> SearchPlaylist(string input, int offset, int limit);

  Task AddSong(string userId, AddRemoveSongDTO addSongDTO);

  Task RemoveSong(string userId, AddRemoveSongDTO addSongDTO);

  Task ChangePlayListState(string userId, ChangePrivacyPlaylistDto changePrivacy);

  Task<List<GetUserPlayListsDTO>> GetUserPlayLists(string userId);

  Task CreatePlayList(string userId, CreatePlayListDTO createPlayListDTO);

  Task UpdatePlayList(string userId, UpdatePlaylistDTO updatePlaylistDTO);

  Task DeletePlayList(string userId, DeletePlayListDTO deletePlaylist);
}