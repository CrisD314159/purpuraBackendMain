using purpuraMain.DbContext;
using purpuraMain.Dto.InputDto;
using purpuraMain.Dto.OutputDto;

namespace purpuraMain.Services.Interfaces;


public interface IPlaylistService
{
  Task<GetPlayListDTO> GetPlaylist(string userId, string playListId);

  Task<List<GetLibraryPlaylistDTO>> SearchPlaylist(string input, int offset, int limit);

  Task<bool> AddSong(string userId, AddRemoveSongDTO addSongDTO);

  Task<bool> RemoveSong(string userId, AddRemoveSongDTO addSongDTO);

  Task<bool> ChangePlayListState(string userId, ChangePrivacyPlaylistDto changePrivacy);

  Task<List<GetUserPlayListsDTO>> GetUserPlayLists(string userId);

  Task<bool> CreatePlayList(string userId, CreatePlayListDTO createPlayListDTO);

  Task<bool> UpdatePlayList(string userId, UpdatePlaylistDTO updatePlaylistDTO);

  Task<bool> DeletePlayList(string userId, DeletePlayListDTO deletePlaylist);
}