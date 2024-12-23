namespace purpuraMain.Services;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using purpuraMain.Dto.InputDto;
using purpuraMain.Validations;
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

    public static async Task<bool> CreateGenre(CreateGenreDTO genreDTO, PurpuraDbContext dbContext)
    {

        try
        {
            GenreValidation validation = new();
            if(!validation.Validate(genreDTO).IsValid) throw new ValidationException("Check your input and try again");
            if (await dbContext.Genres!.AnyAsync(g=> g.Name.Equals(genreDTO.Name))) throw new ValidationException("Genre already exists");

            var genre = new Genre
            {
                Id = Guid.NewGuid().ToString(),
                Name = genreDTO.Name,
                Color = genreDTO.Color,
                Description = genreDTO.Description ?? "",
            };

            await dbContext.AddAsync(genre);
            return true;

        }
        catch(ValidationException)
        {
            throw;

        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<bool> UpdateGenre(UpdateGenreDTO genreDTO, PurpuraDbContext dbContext)
    {
        try
        {
            UpdateGenreValidation validation = new();
            if(!validation.Validate(genreDTO).IsValid) throw new ValidationException("Check your input and try again");
            var genre = await dbContext.Genres!.FindAsync(genreDTO.Id) ?? throw new EntityNotFoundException("Genre does not exists");

            genre.Description = genreDTO.Description ?? "";
            genre.Name = genreDTO.Name;
            genre.Color = genreDTO.Color;

            await dbContext.SaveChangesAsync();
            return true;            
        }
        catch(ValidationException)
        {
            throw;

        }
        catch(EntityNotFoundException)
        {
            throw;

        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }

    public static async Task<bool> DeleteGenre(string id, PurpuraDbContext dbContext)
    {

        try
        {
            var genre = await dbContext.Genres!.FindAsync(id) ?? throw new EntityNotFoundException("Genre does not exists");
            dbContext.Remove(genre);
            await dbContext.SaveChangesAsync();
            return true;            
        }
        catch(EntityNotFoundException)
        {
            throw;

        }
        catch (System.Exception)
        {
            
            throw;
        }
        
    }
}