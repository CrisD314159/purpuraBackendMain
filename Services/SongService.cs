namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;

public static class SongService
{
    public static async Task<GetSongDTO> GetSongById(string userId, string id, PurpuraDbContext dbContext)
    {
        try
        {
            var isOnLibrary = await dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(s=> s.Id == id));
            var song = await dbContext.Songs!.Where(s => s.Id == id).Select(s => new GetSongDTO{
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


            }).FirstAsync() ?? throw new EntityNotFoundException("Song not found");

            return song;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<List<GetSongDTO>> GetSongByInput(string userId, string input, int offset, int limit, PurpuraDbContext dbContext)
    {
        try
        {
            string inputLower = input.ToLower();
            var songs = await dbContext.Songs!.Where(s => s.Name.ToLower().Contains(inputLower) || s.Artists!.Any(a=>a.Name.ToLower().Contains(inputLower)) || s.Album!.Name.ToLower().Contains(inputLower)).Select(s => new GetSongDTO{
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

            }).Skip(offset).Take(limit).ToListAsync() ?? throw new EntityNotFoundException("There are no songs that match the search");

            foreach (var song in songs)
            {
                song.IsOnLibrary = await dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(so => so.Id == song.Id));
            }

            return songs;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<List<GetSongDTO>> GetAllSongs (int offset, int limit, PurpuraDbContext dbContext)
    {
        try
        {
            var songs = await dbContext.Songs!.Select(s=> new GetSongDTO{
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
        catch (System.Exception)
        {
            
            throw;
        }
    }

// Nota: Los metodos de crud de las canciones van en un servicio de canciones hecho en node.js
 
}