using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Dto.InputDto;
using purpuraMain.Validations;
using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Services;

public static class PlaylistServices
{

  private static async Task<bool> CheckIfPlaylistExists(string playListId, PurpuraDbContext dbcontext)
  {
    return await dbcontext.Playlists!.AnyAsync(p => p.Id == playListId);
  }

  public static async Task<GetPlayListDTO> GetPlaylist(string playListId, PurpuraDbContext dbcontext)
  {
    try
    {
      var playList = await 
      dbcontext.Playlists!.Where(p => p.Id == playListId)
      .Select(p => new GetPlayListDTO
      {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        UserName = p.User!.Name!,
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
          
        }).ToList() : new List<GetSongDTO>()
      }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");

      return playList;

    }
    catch(NullFieldException arg)
    {
      throw new NullFieldException(arg.Message);
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
  }
  public static async Task<List<GetLibraryPlaylistDTO>> SearchPlaylist(string input, PurpuraDbContext dbcontext)
  {
    try
    {
      Console.WriteLine("HOLA NUENAS TARDES");
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
        UserName = p.User!.Name!
      }).ToListAsync() ?? [];
     
      return playList;
    }

    catch(NullFieldException arg)
    {
      throw new NullFieldException(arg.Message);
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
  }
  

  public static async Task<bool> AddSong(AddRemoveSongDTO addSongDTO, PurpuraDbContext dbContext)
  {
    try
    {
      var playlist = await dbContext.Playlists!.FindAsync(addSongDTO.PlaylistId) ?? throw new EntityNotFoundException("Playlist not found");
      if(playlist.UserId != addSongDTO.UserId || !playlist.Editable) throw new ValidationException("You are not authorized to add songs to this playlist");
      var song = await dbContext.Songs!.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException("Song not found");  
      playlist.Songs!.Add(song);
      await dbContext.SaveChangesAsync();
      return true;    
    }
    catch(NullFieldException arg)
    {
      throw new NullFieldException(arg.Message);
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
  }


  public static async Task<bool> RemoveSong(AddRemoveSongDTO addSongDTO, PurpuraDbContext dbContext)
  {
    try
    {
      var playlist = await dbContext.Playlists!.FindAsync(addSongDTO.PlaylistId) ?? throw new EntityNotFoundException("Playlist not found");
      if(playlist.UserId != addSongDTO.UserId || !playlist.Editable ) throw new ValidationException("You are not authorized to add songs to this playlist");
      var song = await dbContext.Songs!.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException("Song not found");  
      playlist.Songs!.Remove(song);
      await dbContext.SaveChangesAsync();
      return true;    
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
  }

  public static async Task<bool> ChangePlayListState(ChangePrivacyPlaylistDto changePrivacy, PurpuraDbContext dbContext)
  {
    try
    {
      var playList = await dbContext.Playlists!.FindAsync(changePrivacy.Id) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != changePrivacy.UserId || !playList.Editable) throw new ValidationException("You are not authorized to change the privacy of this playlist");
      playList.IsPublic = !playList.IsPublic;
      await dbContext.SaveChangesAsync();
      return true;
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch(ValidationException arg)
    {
      throw new ValidationException(arg.Message);
    }
    catch (System.Exception)
    {

      
      throw new Exception ("An unexpected error occured");
    }
  }
  public static async Task<List<GetUserPlayListsDTO>> GetUserPlayLists(string userId, PurpuraDbContext dbContext)
  {
    try
    {
      Console.WriteLine("HOLA NUENAS TARDES", userId);
      if(!await dbContext.Users!.AnyAsync(u => u.Id == userId)) throw new EntityNotFoundException("User not found");
      var playLists = await dbContext.Playlists!.Where(p => p.UserId== userId).Select(p=> new GetUserPlayListsDTO{
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        UserId = p.UserId!,
        UserName = p.User!.Name!,
        IsPublic = p.IsPublic,
        ImageUrl = p.ImageUrl
      }).ToListAsync() ?? [];

    return playLists;
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch(ValidationException arg)
    {
      throw new ValidationException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
  }

  public static async Task<bool> CreatePlayList(CreatePlayListDTO createPlayListDTO, PurpuraDbContext dbContext)
  {
    try
    {

      PlayListValidation validator = new();
      if(!validator.Validate(createPlayListDTO).IsValid) throw new ValidationException("Invalid output");
      if(!await dbContext.Users!.AnyAsync(u => u.Id == createPlayListDTO.UserId)) throw new EntityNotFoundException("User not found");
      if(await dbContext.Playlists!.AnyAsync(p => p.Name == createPlayListDTO.Name && p.UserId == createPlayListDTO.UserId)) throw new ValidationException("There is already a playlist with that name on your library");
      
      var playList = new Playlist
      {
        Id = Guid.NewGuid().ToString(),
        Name = createPlayListDTO.Name,
        Description = createPlayListDTO.Description ?? "",
        UserId = createPlayListDTO.UserId,
        ImageUrl = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1735657347/qheqts3xhcejrmwu5hur.jpg",
        IsPublic = true,
        Editable = true,
        CreatedAt = DateTime.UtcNow
      };
      await dbContext.Playlists!.AddAsync(playList);
      await dbContext.SaveChangesAsync();
      await LibraryService.AddPlayListToLibrary(new AddRemovePlayListDTO{PlaylistId = playList.Id, UserId = playList.UserId}, dbContext);
      return true;
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch(ValidationException arg)
    {
      throw new ValidationException(arg.Message);
    }
    catch (System.Exception e)
    {
      Console.WriteLine(e.Message);
      throw new Exception ("An unexpected error occured");
    }
  }

  public static async Task<bool> UpdatePlayList(UpdatePlaylistDTO updatePlaylistDTO, PurpuraDbContext dbContext)
  {
    try
    {
      PlayListUpdateValidation validator = new();
      if(!validator.Validate(updatePlaylistDTO).IsValid) throw new ValidationException("Invalid output");
      var playList = await dbContext.Playlists!.FindAsync(updatePlaylistDTO.Id) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != updatePlaylistDTO.UserId || !playList.Editable) throw new ValidationException("You are not authorized to update this playlist");
      playList.Name = updatePlaylistDTO.Name;
      playList.Description = updatePlaylistDTO.Description;
      await dbContext.SaveChangesAsync();
      return true;
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch(ValidationException arg)
    {
      throw new ValidationException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
    
  }

  public static async Task<bool> DeletePlayList(DeletePlayListDTO deletePlaylist, PurpuraDbContext dbContext)
  {
    try
    {
      var playList = await dbContext.Playlists!.FindAsync(deletePlaylist.Id) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != deletePlaylist.UserId || !playList.Editable) throw new ValidationException("You are not authorized to delete this playlist");
      dbContext.Playlists!.Remove(playList);
      await dbContext.SaveChangesAsync();
      return true;
    }
    catch(EntityNotFoundException arg)
    {
      throw new EntityNotFoundException(arg.Message);
    }
    catch(ValidationException arg)
    {
      throw new ValidationException(arg.Message);
    }
    catch (System.Exception)
    {
      
      throw new Exception ("An unexpected error occured");
    }
  }

  



}