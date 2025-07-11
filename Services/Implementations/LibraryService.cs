namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Services.Interfaces;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using AutoMapper;

public class LibraryService(PurpuraDbContext dbContext, IMapper mapper) : ILibraryService
{

    private readonly PurpuraDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    /// <summary>
    /// Obtiene la biblioteca de un usuario por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="dbContext"></param>
    /// <returns>Objeto GetLibraryDTO con la información de la librería de un usuario (playlists y canciones) </returns>
    public async Task<GetLibraryDTO> GetLibraryById(string userId)
    {

        var userLibrary = await _dbContext.Libraries.Where(l => l.UserId == userId && l.User.State == UserState.ACTIVE)
        .ProjectTo<GetLibraryDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

        userLibrary.Playlists = await _dbContext.Playlists.Where(p => p.UserId == userId)
        .ProjectTo<GetLibraryPlaylistDTO>(_mapper.ConfigurationProvider)
        .ToListAsync();

        return userLibrary;
   
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

        var userLibrary = await _dbContext.Libraries.Where(l => l.UserId == userId && l.User.State == UserState.ACTIVE)
        .ProjectTo<GetLibraryDTO>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

        var userSongs = await _dbContext.Libraries.Where(l => l.UserId == userId && l.User.State == UserState.ACTIVE)
        .SelectMany(l => l.Songs)
        .Skip(offset)
        .Take(limit)
        .ProjectTo<GetSongDTO>(_mapper.ConfigurationProvider)
        .ToListAsync();

        await CheckSongsOnLibraryWithUser(userSongs, userId);
        
        userLibrary.Songs = userSongs;

        return userLibrary;

    } 

    /// <summary>
    /// Añade o elimina una canción de la biblioteca de un usuario.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemoveSong"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task AddSongToLibrary(string userId, AddRemoveSongLibraryDTO addRemoveSong)
    {
    
        var library = await _dbContext.Libraries.Include(l => l.Songs)
        .Where(l => l.UserId == userId && l.User.State == UserState.ACTIVE)
        .FirstOrDefaultAsync()
        ?? throw new EntityNotFoundException("Library not found");

        if (library.Songs.Count >= 100)
        {
            throw new BadRequestException("You have reached your maximum ammount of saved songs");    
        }

        var song = await _dbContext.Songs.FindAsync(addRemoveSong.SongId)
        ?? throw new EntityNotFoundException("Song not found");
        if(library.Songs!.Any(s => s.Id == addRemoveSong.SongId))
        {
            // Si ya existe la cancion en la biblioteca, se elimina
            library.Songs.Remove(song);
            await _dbContext.SaveChangesAsync();
            return;
        }
        library.Songs.Add(song);
        await _dbContext.SaveChangesAsync();
    
    }

 
    /// <summary>
    /// Añade o elimina un álbum de la biblioteca de un usuario (Unused by the moment).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemoveAlbum"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
     public async Task AddAlbumToLibrary(string userId, AddRemoveAlbumLibraryDTO addRemoveAlbum)
    {
      
        var library = await _dbContext.Libraries.Include(l => l.Albums).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

        if (library.Albums.Count >= 15)
        {
            throw new BadRequestException("You have reached your maximun ammount of saved albums");
                
        }

        var album = await _dbContext.Albums.Where(a => a.Id == addRemoveAlbum.AlbumId).FirstOrDefaultAsync()
        ?? throw new EntityNotFoundException("Album not found");
        if(library.Albums.Any(a => a.Id == album.Id))
        {
            library.Albums.Remove(album);
            await _dbContext.SaveChangesAsync();
            return;
        }
        library.Albums.Add(album);
        await _dbContext.SaveChangesAsync();
        
    }

    /// <summary>
    /// Añade o elimina una playlist de la biblioteca de un usuario (Unused by the moment).
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="addRemovePlayListDTO"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
      public async Task AddPlayListToLibrary(string userId, AddRemovePlayListDTO addRemovePlayListDTO)
    {

        var library = await _dbContext.Libraries.Include(l => l.Playlists).Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Library not found");

        if (library.Playlists.Count >= 15)
        {
            throw new BadRequestException("You have reached yout maximum ammount of saved playlists");
        }
        var playlist = await _dbContext.Playlists.Where(p => p.Id.Equals(addRemovePlayListDTO.PlaylistId)).FirstOrDefaultAsync()
        ?? throw new EntityNotFoundException("Album not found");
        if(library.Playlists.Any(p => p.Id == playlist.Id))
        {
            library.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync();
            return;
        }
        library.Playlists.Add(playlist);
        await _dbContext.SaveChangesAsync();        
    }
   public async Task CheckSongsOnLibraryWithUser(ICollection<GetSongDTO> songsList, string userId)
    {
        var songsIds = songsList.Select(s => s.Id).ToList();

        var userLibrarySongs = await _dbContext.Libraries!
            .Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE)
            .SelectMany(l => l.Songs)
            .Where(s => songsIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync();

        foreach (var song in songsList)
        {
            song.IsOnLibrary = userLibrarySongs.Contains(song.Id);
        }
        
    }
}