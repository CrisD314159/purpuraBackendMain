using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Model;

namespace purpuraMain.Services;
public static class PurpleDaylistService
{



/// <summary>
/// Obtiene la lista de recomendaciones de un usuario (Purple daylist)
/// </summary>
/// <param name="userId"></param>
/// <param name="dbContext"></param>
/// <returns></returns>
  public static async Task<GetPlayListDTO> GetPurpleDaylist(string userId, PurpuraDbContext dbContext)
  {

      var playlist = await dbContext.Playlists!.Where(p=> p.Name == "Purple Day List" && p.UserId == userId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message = "Playlist not found", Success = false});
      await UpdatePurpuraDayList(playlist.Id, userId, dbContext);
      var entirePlaylist = await dbContext.Playlists!.Where(p => p.Id == playlist.Id).Select(p => new GetPlayListDTO
      {
                Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        UserName = p.User!.FirstName!,
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
          IsOnLibrary = false

        }).ToList() : new List<GetSongDTO>()
      }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message = "Playlist not found", Success = false});

        foreach (var song in entirePlaylist.Songs)
        {
          song.IsOnLibrary = await dbContext.Libraries!.Where(l => l.UserId == userId && l.User!.State == UserState.ACTIVE).AnyAsync(l => l.Songs.Any(so => so.Id == song.Id));
        }

      return entirePlaylist;

  }


/// <summary>
/// Actualiza la lista de recomendaciones de un usuario (Purple daylist)
/// </summary>
/// <param name="playlistId"></param>
/// <param name="userId"></param>
/// <param name="dbContext"></param>
/// <returns></returns>
  public static async Task<bool> UpdatePurpuraDayList(string playlistId, string userId, PurpuraDbContext dbContext)
  {

      var playlist = dbContext.Playlists!.Find(playlistId) ?? throw new EntityNotFoundException(404, new {Message = "Playlist not found", Success = false});
      Console.WriteLine(playlistId);
      if((DateTime.UtcNow.Date - playlist.LastUpdated.Date ) > TimeSpan.FromDays(1))
      {
        await ClearPlaylist(playlistId, dbContext);
        var recomendations =  await GetUserRecomendations(userId, dbContext);
        playlist.Songs = recomendations;
        playlist.LastUpdated = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return true;
      }
      return false;

  }


  /// <summary>
  /// Método de apoyo para obtener las canciones recomendadas de un usuario según su historial de reproducciones.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
 public static async Task<List<Song>> GetUserRecomendations(string userId, PurpuraDbContext dbContext)
{

        // Obtener las canciones reproducidas recientemente y sus géneros
        var recentListenGenres = await dbContext.PlayHistories!
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PlayedAt)
            .Take(10)
            .SelectMany(p => p.Song!.Genres!)  // Aplanamos la lista
            .Select(g => g.Id) // Tomamos solo los IDs de los géneros
            .Distinct()
            .ToListAsync();

        // Buscar canciones que tengan al menos un género en la lista obtenida
        var recomendations = await dbContext.Songs!
            .Where(s => s.Genres!.Any(g => recentListenGenres.Contains(g.Id))) // Comparamos con IDs
            .OrderByDescending(s => s.Name)
            .Take(10)
            .ToListAsync();

        return recomendations;
}


public static async Task<bool> ClearPlaylist(string playlistId, PurpuraDbContext dbContext)
{

        
        var playlist = await dbContext.Playlists!.Include(p => p.Songs).Where(p => p.Id == playlistId).FirstOrDefaultAsync() ?? throw new EntityNotFoundException(404, new {Message = "Playlist not found", Success = false});
        playlist.Songs!.Clear();
        await dbContext.SaveChangesAsync();
        return true;

} 

}