using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Dto.InputDto;
using purpuraMain.Validations;
using System.ComponentModel.DataAnnotations;
using CloudinaryDotNet.Actions;

namespace purpuraMain.Services;

public static class PlaylistServices
{

  /// <summary>
  /// Obtiene la información de una playlist verificando primero si es del usuario que la solicita.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="playListId"></param>
  /// <param name="dbcontext"></param>
  /// <returns></returns>
  public static async Task<GetPlayListDTO> GetPlaylist(string userId, string playListId, PurpuraDbContext dbcontext)
  {

      //Esto es para verificar si el usuario que hace la petición puede ver la playlist
      if( await dbcontext.Playlists!.AnyAsync(p => p.Id == playListId && p.UserId != userId && p.IsPublic == false)) throw new EntityNotFoundException(404, new {Message ="Playlist not found", success=false});
      var playList = await
      dbcontext.Playlists!.Where(p => p.Id == playListId)
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
      }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Playlist not found", success=false});

        foreach (var song in playList.Songs)
        {
          song.IsOnLibrary = await dbcontext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(so => so.Id == song.Id));
        }
            

      return playList;

 
  }

  /// <summary>
  /// Busca playlists que coincidan con el input del usuario (verifica que sean públicas) (Unused by the moment).
  /// </summary>
  /// <param name="input"></param>
  /// <param name="offset"></param>
  /// <param name="limit"></param>
  /// <param name="dbcontext"></param>
  /// <returns></returns>
  public static async Task<List<GetLibraryPlaylistDTO>> SearchPlaylist(string input, int offset, int limit, PurpuraDbContext dbcontext)
  {

      var playList = await
      dbcontext.Playlists!.Where(p => p.Name.ToLower().Contains(input.ToLower()) && p.IsPublic)
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
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<bool> AddSong(string userId, AddRemoveSongDTO addSongDTO, PurpuraDbContext dbContext)
  {

      var playlist = await dbContext.Playlists!.FindAsync(addSongDTO.PlaylistId) ?? throw new EntityNotFoundException(404, new {Message ="Playlist not found", Success=false});
      if(playlist.UserId != userId || !playlist.Editable) throw new UnauthorizedException(401, new {Message = "You are not authorized to add songs to this playlist", Success = false});
      var song = await dbContext.Songs!.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException(404, new {Message ="Song not found", Success=false});
      playlist.Songs!.Add(song);
      await dbContext.SaveChangesAsync();
      return true;
 
  }


  /// <summary>
  /// Elimina una canción de una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="addSongDTO"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<bool> RemoveSong(string userId, AddRemoveSongDTO addSongDTO, PurpuraDbContext dbContext)
  {

      var playlist = await dbContext.Playlists!.Include(p=> p.Songs).Where(p => p.Id == addSongDTO.PlaylistId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Playlist not found", Success=false});
      if(playlist.UserId != userId) throw new UnauthorizedException(401, new {Message ="You are not authorized to remove songs from this playlist", Success = false});
      var song = await dbContext.Songs!.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException(404, new {Message ="Song not found", Success=false});
      Console.Write(playlist.Songs!.Count);
      playlist.Songs!.Remove(song);
       dbContext.SaveChanges();
      return true;

  }

  /// <summary>
  /// Cambia la privacidad de una playlist (Unused by now).
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="changePrivacy"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<bool> ChangePlayListState(string userId, ChangePrivacyPlaylistDto changePrivacy, PurpuraDbContext dbContext)
  {

      var playList = await dbContext.Playlists!.FindAsync(changePrivacy.Id) ?? throw new EntityNotFoundException(404, new {Message ="Playlist not found", Success=false});
      if(playList.UserId != userId) throw new UnauthorizedException(401, new{Message = "You are not authorized to change the privacy of this playlist", Success = false});
      playList.IsPublic = !playList.IsPublic;
      await dbContext.SaveChangesAsync();
      return true;

  }

  /// <summary>
  /// Obtiene las playlists de un usuario.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<List<GetUserPlayListsDTO>> GetUserPlayLists(string userId, PurpuraDbContext dbContext)
  {

      if(!await dbContext.Users!.AnyAsync(u => u.Id == userId)) throw new EntityNotFoundException(404, new {Message ="User not found", Success=false});
      var playLists = await dbContext.Playlists!.Where(p => p.UserId== userId && p.Name !="Purple Day List").Select(p=> new GetUserPlayListsDTO{
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
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<bool> CreatePlayList(string userId, CreatePlayListDTO createPlayListDTO, PurpuraDbContext dbContext)
  {

      PlayListValidation validator = new();
      if(!validator.Validate(createPlayListDTO).IsValid) throw new ValidationException("Invalid output");
      var library = await dbContext.Libraries!.Where(l=> l.UserId == userId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message ="Library not found", Success=false});
      if(await dbContext.Playlists!.AnyAsync(p => p.Name == createPlayListDTO.Name && p.UserId == userId)) throw new BadRequestException(409, new {Message = "There is already a playlist with that name on your library", Success = false});

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
      await dbContext.Playlists!.AddAsync(playList);
      library.Playlists.Add(playList);
      await dbContext.SaveChangesAsync();
      return true;

  }


  /// <summary>
  /// Actualiza una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="updatePlaylistDTO"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public static async Task<bool> UpdatePlayList(string userId, UpdatePlaylistDTO updatePlaylistDTO, PurpuraDbContext dbContext)
  {

      PlayListUpdateValidation validator = new();
      if(!validator.Validate(updatePlaylistDTO).IsValid) throw new ValidationException("Invalid output");
      var playList = await dbContext.Playlists!.FindAsync(updatePlaylistDTO.Id) ?? throw new EntityNotFoundException(404, new {Message ="Playlist not found", success=false});
      if(playList.UserId != userId) throw new UnauthorizedException(401, new {Message = "You're not authorized to update this playlist", Success= false});
      
      playList.Name = updatePlaylistDTO.Name;
      playList.Description = updatePlaylistDTO.Description;
      if(!string.IsNullOrEmpty(updatePlaylistDTO.ImageUrl))
      {
        playList.ImageUrl = updatePlaylistDTO.ImageUrl;
      }
      await dbContext.SaveChangesAsync();
      return true;

  }


  /// <summary>
  /// Elimina una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="deletePlaylist"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public static async Task<bool> DeletePlayList(string userId, DeletePlayListDTO deletePlaylist, PurpuraDbContext dbContext)
  {

      var playList = await dbContext.Playlists!.FindAsync(deletePlaylist.Id) ?? throw new EntityNotFoundException(404, new {Message ="Playlist not found", success=false});
      if(playList.UserId != userId) throw new UnauthorizedException(401, new {Message = "You're not authorized to delete this playlist", Success= false});
      dbContext.Playlists!.Remove(playList);
      await dbContext.SaveChangesAsync();
      return true;
  }

}
