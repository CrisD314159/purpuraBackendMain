namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;

public static class SongService
{
    public static async Task<GetSongDTO> GetSongById(string id, PurpuraDbContext dbContext)
    {
        try
        {
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
                Genres = s.Genres!.Select(g => new GetGenreDTO{
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),
                ImageUrl = s.ImageUrl!,
                Lyrics = s.Lyrics ?? ""

            }).FirstAsync() ?? throw new EntityNotFoundException("Song not found");

            return song;
        }
        catch (EntityNotFoundException ex)
        {
            throw new EntityNotFoundException(ex.Message);
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<List<GetSongDTO>> GetSongByInput(string input, PurpuraDbContext dbContext)
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
                Lyrics = s.Lyrics ?? ""

            }).ToListAsync() ?? throw new EntityNotFoundException("There are no songs that match the search");

            return songs;
        }
        catch (EntityNotFoundException ex)
        {
            throw new EntityNotFoundException(ex.Message);
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

// Nota: Los metodos de crud de las canciones van en un servicio de canciones hecho en node.js
 
}