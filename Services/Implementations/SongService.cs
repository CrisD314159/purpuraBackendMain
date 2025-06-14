namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;

public class SongService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService, UserManager<User> userManager) : ISongService
{

    private readonly PurpuraDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILibraryService _libraryService = libraryService;
    private readonly UserManager<User> _userManager = userManager;

    /// <summary>
    /// Obtiene una canción por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public async Task<GetSongDTO> GetSongById(string userId, string id)
    {

        var isOnLibrary = await _dbContext.Libraries.Where(l => l.UserId == userId && l.User.State == UserState.ACTIVE)
        .AnyAsync(l => l.Songs.Any(s=> s.Id == id));

        var song = await _dbContext.Songs!.Where(s => s.Id == id)
        .ProjectTo<GetSongDTO>(_mapper.ConfigurationProvider)
        .FirstAsync() ?? throw new EntityNotFoundException("Song not found");

        song.IsOnLibrary = isOnLibrary;

        return song;
     
        
    }

    /// <summary>
    /// Obtiene una lista de canciones que coincidan con el input ingresado.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="input"></param>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<List<GetSongDTO>> GetSongByInput(string userId, string input, int offset, int limit)
    {

        var songs = await _dbContext.Songs.Where(s => s.Name.Contains(input, StringComparison.OrdinalIgnoreCase)
        || s.Artists.Any(a => a.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
        || s.Album.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
        .ProjectTo<GetSongDTO>(_mapper.ConfigurationProvider)
        .Skip(offset).Take(limit).ToListAsync();

        if(userId != "0") await _libraryService.CheckSongsOnLibraryWithUser(songs, userId);

        return songs;
       
    }

    /// <summary>
    /// Obtiene todas las canciones disponibles en la plataforma usando paginación.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<List<GetSongDTO>> GetAllSongs (int offset, int limit)
    {
        var songs = await _dbContext.Songs
        .ProjectTo<GetSongDTO>(_mapper.ConfigurationProvider)
        .Skip(offset).Take(limit).ToListAsync();

        return songs;
    }


    /// <summary>
    /// Obtiene las canciones más populares del momento.
    /// </summary>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<List<GetSongDTO>> GetTopSongs(string userId)
    {

        var songsPlays = await _dbContext.Songs.Select(s => new
        {

            Song = s,
            Plays = _dbContext.PlayHistories.Count(ph => ph.SongId == ph.Id)
        })
        .OrderByDescending(x => x.Plays)
        .Take(10)
        .ToListAsync();

        var songs = songsPlays.Select(s => new GetSongDTO
            {
                Id = s.Song.Id,
                Name = s.Song.Name,
                Artists = [.. s.Song.Artists.Select(a => new GetPlaylistArtistDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description ?? ""
                })],
                AlbumId = s.Song.AlbumId,
                AlbumName = s.Song.Album.Name,
                ImageUrl = s.Song.ImageUrl ?? "",
                AudioUrl = s.Song.AudioUrl ?? "",
                Genres = [.. s.Song.Genres.Select(g => new GetGenreDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description ?? ""
                })],
                IsOnLibrary = false,
                Lyrics = s.Song.Lyrics ?? "",
                Plays = s.Plays
            }).OrderByDescending(s => s.Plays).Take(10).ToList();

        await _libraryService.CheckSongsOnLibraryWithUser(songs, userId);

        return songs;
     
    }

    /// <summary>
    /// Registra un reproducción de una canción cuando en el cliente se reproduce
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="songId"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<bool> AddSongPlay(string userId, string songId )
    {

        var user = await _dbContext.Users.FindAsync(userId)
        ?? throw new EntityNotFoundException("User not found");
        
        var song = await _dbContext.Songs.FindAsync(songId)
        ?? throw new EntityNotFoundException("Song not found");

        var playHistory = new PlayHistory
        {
            Id = Guid.NewGuid().ToString(),
            Song = song,
            User = user,
            PlayedAt = DateTime.UtcNow
        };

        _dbContext.PlayHistories!.Add(playHistory);
        await _dbContext.SaveChangesAsync();

        return true;
     
    }
 
}