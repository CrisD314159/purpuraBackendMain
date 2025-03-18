namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;

public class SongService(PurpuraDbContext dbContext) : ISongService
{

    private readonly PurpuraDbContext _dbContext = dbContext;

    /// <summary>
    /// Obtiene una canción por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public async Task<GetSongDTO> GetSongById(string userId, string id)
    {

            var isOnLibrary = await _dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(s=> s.Id == id));
            var song = await _dbContext.Songs!.Where(s => s.Id == id).Select(s => new GetSongDTO{
                Id = s.Id,
                Name = s.Name,
                Artists = s.Artists!.Select(a=> new GetPlaylistArtistDTO{
                    Id = a.Id,
                    Name = a.Name,

                }).ToList(),
                AlbumId = s.AlbumId!,
                AlbumName = s.Album!.Name!,
                AudioUrl = s.AudioUrl!,
                Duration = s.Duration,
                ReleaseDate= s.Album!.ReleaseDate,
                Genres = s.Genres!.Select(g => new GetGenreDTO{
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),
                ImageUrl = s.ImageUrl!,
                Lyrics = s.Lyrics ?? "",
                ProducerName = s.Album!.ProducerName ?? "",
                RecordLabel = s.Album!.RecordLabel ?? "",
                WriterName = s.Album!.WriterName ?? "",
                IsOnLibrary = isOnLibrary,


            }).FirstAsync() ?? throw new EntityNotFoundException(404, new {Message = "Song not found", Success = false});

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

            string inputLower = input.ToLower();
            var songs = await _dbContext.Songs!.Where(s => s.Name.ToLower().Contains(inputLower) || s.Artists!.Any(a=>a.Name.ToLower().Contains(inputLower)) || s.Album!.Name.ToLower().Contains(inputLower)).Select(s => new GetSongDTO{
                Id = s.Id,
                Name = s.Name,
                Artists = s.Artists!.Select(a=> new GetPlaylistArtistDTO{
                    Id = a.Id,
                    Name = a.Name,

                }).ToList(),
                AlbumId = s.AlbumId!,
                AlbumName = s.Album!.Name!,
                AudioUrl = s.AudioUrl!,
                Duration = s.Duration,
                Genres = s.Genres!.Select(g => new GetGenreDTO{
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),
                ImageUrl = s.ImageUrl!,
                Lyrics = s.Lyrics ?? "",
                IsOnLibrary = false,

            }).Skip(offset).Take(limit).ToListAsync() ?? throw new EntityNotFoundException(404, new {Message = "There are no sogs that match the searc", Success = false});

            foreach (var song in songs)
            {
                song.IsOnLibrary = await _dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(so => so.Id == song.Id));
            }

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

            var songs = await _dbContext.Songs!.Select(s=> new GetSongDTO{
                Id = s.Id,
                Name = s.Name,
                Artists = s.Artists!.Select(a=> new GetPlaylistArtistDTO{
                    Id = a.Id,
                    Name = a.Name,

                }).ToList(),
                AlbumId = s.AlbumId!,
                AlbumName = s.Album!.Name!,
                AudioUrl = s.AudioUrl!,
                Duration = s.Duration,
                Genres = s.Genres!.Select(g => new GetGenreDTO{
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),
                ImageUrl = s.ImageUrl!,
                Lyrics = s.Lyrics ?? ""
            }).Skip(offset).Take(limit).ToListAsync() ?? [];

            return songs;
     
    }


    /// <summary>
    /// Obtiene las canciones más populares del momento.
    /// </summary>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<List<GetSongDTO>> GetTopSongs(string userId)
    {

            var songs = await _dbContext.Songs!.Select(s => new GetSongDTO
            {
                Id = s.Id,
                Name = s.Name,
                Artists = s.Artists!.Select(a => new GetPlaylistArtistDTO
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
                Genres = s.Genres!.Select(g => new GetGenreDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description ?? ""
                }).ToList(),
                IsOnLibrary = false,
                Lyrics = s.Lyrics ?? "",
                Plays = _dbContext.PlayHistories!.Where(p => p.SongId == s.Id).Count()
            }).OrderByDescending(s => s.Plays).Take(10).ToListAsync() ?? [];

            foreach (var song in songs)
            {
                song.IsOnLibrary = _dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).Any( l=> l.Songs.Any(so => so.Id == song.Id));
                
            }

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

            var song = await _dbContext.Songs!.FindAsync(songId) ?? throw new EntityNotFoundException(404, new {Message = "Song not found", Success = false});
            var user = await _dbContext.Users!.FindAsync(userId) ?? throw new EntityNotFoundException(404, new {Message = "User not found", Success = false});

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