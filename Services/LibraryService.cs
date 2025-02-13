namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using Microsoft.EntityFrameworkCore;

public static class LibraryService
{
    /// <summary>
    /// Obtiene la biblioteca de un usuario por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dbContext"></param>
    /// <returns>Objeto GetLibraryDTO con la información de la librería de un usuario (playlists y canciones) </returns>
    public static async Task<GetLibraryDTO> GetLibraryById(string userId, PurpuraDbContext dbContext)
    {
        try
        {   
            var library = await dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Select( l=>
                new GetLibraryDTO{
                   Id= l.Id,
                   UserId = l.UserId!,
                   UserName = l.User!.FirstName!,
                   Albums = l.Albums.Select(a=> new GetLibraryAlbumDTO {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description ?? "",
                        PictureUrl = a.PictureUrl,
                        ArtistId = a.ArtistId,
                        ArtistName = a.Artist.Name,
                        ReleaseDate = a.ReleaseDate,
                   }).ToList(),
                   Playlists = l.Playlists.Select(p=> new GetLibraryPlaylistDTO
                   {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? "",
                    UserName = p.User!.FirstName!,
                    UserId = p.UserId!,
                    IsPublic = p.IsPublic,
                    ImageUrl = p.ImageUrl
                   }).ToList(),
                   Songs= l.Songs.Select(s=> new GetSongDTO
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Artists = s.Artists!.Select(a=> new GetPlaylistArtistDTO
                       {
                           Id = a.Id,
                           Name = a.Name,
                           Description = a.Description ?? ""
                       }).ToList(),
                       AlbumId = s.AlbumId!,
                       AlbumName = s.Album!.Name!,
                       Duration = s.Duration,
                       ImageUrl = s.ImageUrl ?? "",
                       AudioUrl = s.AudioUrl ?? "",
                       Genres = s.Genres!.Select(g=> new GetGenreDTO
                       {
                           Id = g.Id,
                           Name = g.Name,
                           Description = g.Description ?? ""
                       }).ToList(),
                       IsOnLibrary= true,
                       Lyrics = s.Lyrics ?? ""
                   }).ToList()
                   
                }
            ).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

            return library;
        }

        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    /// <summary>
    /// Obtiene las canciones de un usuario por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="dbContext"></param>
    /// <returns>Canciones guardadas por el usuario</returns>
    public static async Task<GetLibraryDTO> GetUserSongs (string userId, int offset, int limit, PurpuraDbContext dbContext)
    {
        try
        {
             var library = await dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Select( l=>
                new GetLibraryDTO{
                   Id= l.Id,
                   UserId = l.UserId!,
                   UserName = l.User!.FirstName!,
                   Songs= l.Songs.Select(s=> new GetSongDTO
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Artists = s.Artists!.Select(a=> new GetPlaylistArtistDTO
                       {
                           Id = a.Id,
                           Name = a.Name,
                           Description = a.Description ?? ""
                       }).ToList(),
                       AlbumId = s.AlbumId!,
                       AlbumName = s.Album!.Name!,
                       Duration = s.Duration,
                       ImageUrl = s.ImageUrl ?? "",
                       AudioUrl = s.AudioUrl ?? "",
                       IsOnLibrary = true,
                       Genres = s.Genres!.Select(g=> new GetGenreDTO
                       {
                           Id = g.Id,
                           Name = g.Name,
                           Description = g.Description ?? ""
                       }).ToList(),
                       Lyrics = s.Lyrics ?? ""
                   }).Skip(offset).Take(limit).ToList()
                   
                }
            ).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

            return library;

        }
        catch (System.Exception)
        {
            
            throw;
        }
    } 

    /// <summary>
    /// Añade o elimina una canción de la biblioteca de un usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemoveSong"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public static async Task<bool> AddSongToLibrary(string userId, AddRemoveSongLibraryDTO addRemoveSong, PurpuraDbContext dbContext)
    {
        try
        {
            var library = await dbContext.Libraries!.Include(l=> l.Songs).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
            var song = await dbContext.Songs!.Where(s => s.Id == addRemoveSong.SongId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Song not found");
            if(library.Songs!.Any(s => s.Id == addRemoveSong.SongId))
            {
                // Si ya existe la cancion en la biblioteca, se elimina
                Console.WriteLine("entra");
                library.Songs.Remove(song);
                await dbContext.SaveChangesAsync();
                return true;
            }
            library.Songs.Add(song);
            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

 
    /// <summary>
    /// Añade o elimina un álbum de la biblioteca de un usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemoveAlbum"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
     public static async Task<bool> AddAlbumToLibrary(string userId, AddRemoveAlbumLibraryDTO addRemoveAlbum, PurpuraDbContext dbContext)
    {
          try
        {
            var library = await dbContext.Libraries!.Include(l=> l.Songs).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
            var album = await dbContext.Albums!.Where(a => a.Id == addRemoveAlbum.AlbumId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Album not found");
            if(library.Albums!.Any(a => a.Id == album.Id))
            {
                library.Albums.Remove(album);
                await dbContext.SaveChangesAsync();
                return true;
            }
            library.Albums.Add(album);
            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    /// <summary>
    /// Añade o elimina una playlist de la biblioteca de un usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemovePlayListDTO"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
      public static async Task<bool> AddPlayListToLibrary(string userId, AddRemovePlayListDTO addRemovePlayListDTO, PurpuraDbContext dbContext)
    {
        try
        {
           var library = await dbContext.Libraries!.Include(l=> l.Songs).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
            var playlist = await dbContext.Playlists!.Where(p => p.Id == addRemovePlayListDTO.PlaylistId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Album not found");
            if(library.Playlists!.Any(p => p.Id == playlist.Id))
            {
                library.Playlists.Remove(playlist);
                await dbContext.SaveChangesAsync();
                return true;
            }
            library.Playlists.Add(playlist);
            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }
}