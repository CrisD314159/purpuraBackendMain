namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Dto.InputDto;
using purpuraMain.Validations;
using System.ComponentModel.DataAnnotations;
using CloudinaryDotNet.Actions;
using purpuraMain.Services.Interfaces;

public class PlaylistService(PurpuraDbContext dbContext) : IPlaylistService
{

  private readonly PurpuraDbContext _dbcontext = dbContext;

  /// <summary>
  /// Obtiene la información de una playlist verificando primero si es del usuario que la solicita.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="playListId"></param>
  /// <param name="dbcontext"></param>
  /// <returns></returns>
  public async Task<GetPlayListDTO> GetPlaylist(string userId, string playListId)
  {

      //Esto es para verificar si el usuario que hace la petición puede ver la playlist
      if( await _dbcontext.Playlists!.AnyAsync(p => p.Id == playListId && p.UserId != userId && p.IsPublic == false)) throw new EntityNotFoundException("Playlist not found");
      var playList = await
      _dbcontext.Playlists!.Where(p => p.Id == playListId)
      .Select(p => new GetPlayListDTO
      {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        UserName = p.User!.FirstName!,
        IsPublic = p.IsPublic,
        ImageUrl = p.ImageUrl,
        Songs = p.Songs != null ? p.Songs.Select(s=> new GetSongDTO
        {
          Id = s.Id,
          Name = s.Name,
          Artists = s.Artists != null ? s.Artists.Select(a => new GetPlaylistArtistDTO
          {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description??""
          }).ToList() : new List<GetPlaylistArtistDTO>(),
          AlbumId = s.AlbumId!,
          AlbumName = s.Album!.Name!,
          Duration = s.Duration,
          ImageUrl = s.ImageUrl?? "",
          AudioUrl = s.AudioUrl?? "",
          Genres = s.Genres!.Select(g => new GetGenreDTO
          {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description?? ""
          }).ToList(),
          Lyrics = s.Lyrics ?? "",
          IsOnLibrary = false

        }).ToList() : new List<GetSongDTO>()
      }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");

        foreach (var song in playList.Songs)
        {
          song.IsOnLibrary = await _dbcontext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(so => so.Id == song.Id));
        }
            

      return playList;

 
  }

  /// <summary>
  /// Busca playlists que coincidan con el input del usuario (verifica que sean públicas) (Unused by the moment).
  /// </summary>
  /// <param name="input"></param>
  /// <param name="offset"></param>
  /// <param name="limit"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<List<GetLibraryPlaylistDTO>> SearchPlaylist(string input, int offset, int limit)
  {

      var playList = await
      _dbcontext.Playlists!.Where(p => p.Name.ToLower().Contains(input.ToLower()) && p.IsPublic)
      .Select(p=> new GetLibraryPlaylistDTO
      {
        Id =p.Id,
        ImageUrl = p.ImageUrl,
        IsPublic =p.IsPublic,
        Description = p.Description ?? "",
        Name = p.Name,
        UserId =p.UserId!,
        UserName = p.User!.FirstName!
      }).Skip(offset).Take(limit).ToListAsync() ?? [];

      return playList;
    
  }

  /// <summary>
  /// Añade una canción a una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="addSongDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<bool> AddSong(string userId, AddRemoveSongDTO addSongDTO)
  {

      var playlist = await _dbcontext.Playlists!.FindAsync(addSongDTO.PlaylistId) ?? throw new EntityNotFoundException("Playlist not found");
      if(playlist.UserId != userId || !playlist.Editable) throw new UnauthorizedException("You are not authorized to add songs to this playlist");
      var song = await _dbcontext.Songs!.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException("Song not found");
      playlist.Songs!.Add(song);
      await _dbcontext.SaveChangesAsync();
      return true;
 
  }


  /// <summary>
  /// Elimina una canción de una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="addSongDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<bool> RemoveSong(string userId, AddRemoveSongDTO addSongDTO)
  {

      var playlist = await _dbcontext.Playlists!.Include(p=> p.Songs).Where(p => p.Id == addSongDTO.PlaylistId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");
      if(playlist.UserId != userId) throw new UnauthorizedException("You are not authorized to remove songs from this playlist");
      var song = await _dbcontext.Songs!.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException("Song not found");
      Console.Write(playlist.Songs!.Count);
      playlist.Songs!.Remove(song);
       _dbcontext.SaveChanges();
      return true;

  }

  /// <summary>
  /// Cambia la privacidad de una playlist (Unused by now).
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="changePrivacy"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<bool> ChangePlayListState(string userId, ChangePrivacyPlaylistDto changePrivacy)
  {

      var playList = await _dbcontext.Playlists!.FindAsync(changePrivacy.Id) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != userId) throw new UnauthorizedException("You are not authorized to change the privacy of this playlist");
      playList.IsPublic = !playList.IsPublic;
      await _dbcontext.SaveChangesAsync();
      return true;

  }

  /// <summary>
  /// Obtiene las playlists de un usuario.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<List<GetUserPlayListsDTO>> GetUserPlayLists(string userId)
  {

      if(!await _dbcontext.Users!.AnyAsync(u => u.Id == userId)) throw new EntityNotFoundException("User not found");
      var playLists = await _dbcontext.Playlists!.Where(p => p.UserId== userId && p.Name !="Purple Day List").Select(p=> new GetUserPlayListsDTO{
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        IsPublic = p.IsPublic,
        ImageUrl = p.ImageUrl
      }).ToListAsync() ?? [];

    return playLists;
  }


  /// <summary>
  /// Crea una nueva playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="createPlayListDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<bool> CreatePlayList(string userId, CreatePlayListDTO createPlayListDTO)
  {

      PlayListValidation validator = new();
      if(!validator.Validate(createPlayListDTO).IsValid) throw new ValidationException("Invalid output");
      var library = await _dbcontext.Libraries!.Where(l=> l.UserId == userId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
      if(await _dbcontext.Playlists!.AnyAsync(p => p.Name == createPlayListDTO.Name && p.UserId == userId)) throw new BadRequestException("There is already a playlist with that name on your library");

      var playList = new Playlist
      {
        Id = Guid.NewGuid().ToString(),
        Name = createPlayListDTO.Name,
        Description = createPlayListDTO.Description ?? "",
        UserId = userId,
        ImageUrl = createPlayListDTO.ImageUrl ?? "https://res.cloudinary.com/dw43hgf5p/image/upload/v1735657347/qheqts3xhcejrmwu5hur.jpg",
        IsPublic = true,
        Editable = true,
        CreatedAt = DateTime.UtcNow
      };
      await _dbcontext.Playlists!.AddAsync(playList);
      library.Playlists.Add(playList);
      await _dbcontext.SaveChangesAsync();
      return true;

  }


  /// <summary>
  /// Actualiza una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="updatePlaylistDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<bool> UpdatePlayList(string userId, UpdatePlaylistDTO updatePlaylistDTO)
  {

      PlayListUpdateValidation validator = new();
      if(!validator.Validate(updatePlaylistDTO).IsValid) throw new ValidationException("Invalid output");
      var playList = await _dbcontext.Playlists!.FindAsync(updatePlaylistDTO.Id) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != userId) throw new UnauthorizedException("You're not authorized to update this playlist");
      
      playList.Name = updatePlaylistDTO.Name;
      playList.Description = updatePlaylistDTO.Description;
      if(!string.IsNullOrEmpty(updatePlaylistDTO.ImageUrl))
      {
        playList.ImageUrl = updatePlaylistDTO.ImageUrl;
      }
      await _dbcontext.SaveChangesAsync();
      return true;

  }


  /// <summary>
  /// Elimina una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="deletePlaylist"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task<bool> DeletePlayList(string userId, DeletePlayListDTO deletePlaylist)
  {

      var playList = await _dbcontext.Playlists!.FindAsync(deletePlaylist.Id) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != userId) throw new UnauthorizedException("You're not authorized to delete this playlist");
      _dbcontext.Playlists!.Remove(playList);
      await _dbcontext.SaveChangesAsync();
      return true;
  }

}
