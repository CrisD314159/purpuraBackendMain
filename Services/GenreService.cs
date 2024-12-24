namespace purpuraMain.Services;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;

public static class GenreService
{
    public static async Task<GetGenreDTO> GetTopSongsByGenre(string id, PurpuraDbContext dbContext)
    {
        try
        {
            var songs = await dbContext.Genres!.Where(g => g.Id == id).Select(g=> new GetGenreDTO
            {
                 Id = g.Id,
                Name = g.Name,
                Description = g.Description ?? "",
                Color = g.Color,
                Songs = g.Songs.Select(
                    s=> new GetSongDTO
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
                        Plays = dbContext.PlayHistories!.Where(pl => pl.SongId == s.Id).Count()
                        
                    }).OrderBy(s => s.Plays).ToList()
            }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Genre not found");
            
            return songs;
        }
         catch (EntityNotFoundException)
        {
            
            throw;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }
     public static async Task<List<GetGenreDTO>> GetAllGenres(PurpuraDbContext dbContext)
    {

        try
        {
            var genres = await dbContext.Genres!.Select(g => new GetGenreDTO
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description ?? "",
                Color = g.Color
            }).ToListAsync() ?? [];

            return genres;
        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }
// Crud methods will be implemented y a nodejs api
}