namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Services.Interfaces;

public class LibraryService(PurpuraDbContext dbContext) : ILibraryService
{

    private readonly PurpuraDbContext _dbContext = dbContext;
    /// <summary>
    /// Obtiene la biblioteca de un usuario por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dbContext"></param>
    /// <returns>Objeto GetLibraryDTO con la información de la librería de un usuario (playlists y canciones) </returns>
    public async Task<GetLibraryDTO> GetLibraryById(string userId)
    {
  
            var library = await _dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Select( l=>
                new GetLibraryDTO{
                   Id= l.Id,
                   UserId = l.UserId!,
                   UserName = l.User!.FirstName!,
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
                   
                }
            ).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

            return library;
   
        
    }

    /// <summary>
    /// Obtiene las canciones de un usuario por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="_dbContext"></param>
    /// <returns>Canciones guardadas por el usuario</returns>
    public async Task<GetLibraryDTO> GetUserSongs (string userId, int offset, int limit)
    {
    
             var library = await _dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Select( l=>
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

    /// <summary>
    /// Añade o elimina una canción de la biblioteca de un usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemoveSong"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<bool> AddSongToLibrary(string userId, AddRemoveSongLibraryDTO addRemoveSong)
    {
    
            var library = await _dbContext.Libraries!.Include(l=> l.Songs).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
            var song = await _dbContext.Songs!.Where(s => s.Id == addRemoveSong.SongId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Song not found");
            if(library.Songs!.Any(s => s.Id == addRemoveSong.SongId))
            {
                // Si ya existe la cancion en la biblioteca, se elimina
                library.Songs.Remove(song);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            library.Songs.Add(song);
            await _dbContext.SaveChangesAsync();

            return true;
    
    }

 
    /// <summary>
    /// Añade o elimina un álbum de la biblioteca de un usuario (Unused by the moment).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemoveAlbum"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
     public async Task<bool> AddAlbumToLibrary(string userId, AddRemoveAlbumLibraryDTO addRemoveAlbum)
    {
      
            var library = await _dbContext.Libraries!.Include(l=> l.Songs).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
            var album = await _dbContext.Albums!.Where(a => a.Id == addRemoveAlbum.AlbumId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Album not found");
            if(library.Albums!.Any(a => a.Id == album.Id))
            {
                library.Albums.Remove(album);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            library.Albums.Add(album);
            await _dbContext.SaveChangesAsync();

            return true;
    
        
    }

    /// <summary>
    /// Añade o elimina una playlist de la biblioteca de un usuario (Unused by the moment).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemovePlayListDTO"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
      public async Task<bool> AddPlayListToLibrary(string userId, AddRemovePlayListDTO addRemovePlayListDTO)
    {
    
           var library = await _dbContext.Libraries!.Include(l=> l.Songs).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");
            var playlist = await _dbContext.Playlists!.Where(p => p.Id == addRemovePlayListDTO.PlaylistId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Album not found");
            if(library.Playlists!.Any(p => p.Id == playlist.Id))
            {
                library.Playlists.Remove(playlist);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            library.Playlists.Add(playlist);
            await _dbContext.SaveChangesAsync();

            return true;
    
        
    }
}