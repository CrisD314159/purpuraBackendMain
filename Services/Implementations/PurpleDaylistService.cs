namespace purpuraMain.Services.Implementations;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Exceptions;
using purpuraMain.Model;
using purpuraMain.Services.Interfaces;

public class PurpleDaylistService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService) : IPurpleDaylistService
{

  private readonly PurpuraDbContext _dbContext = dbContext;
  private readonly IMapper _mapper = mapper;

  private readonly ILibraryService _libraryService = libraryService;


/// <summary>
  /// Obtiene la lista de recomendaciones de un usuario (Purple daylist)
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="dbContext"></param>
  /// <returns></returns>
  public async Task<GetPlayListDTO> GetPurpleDaylist(string userId)
  {
    await UpdatePurpuraDayList(userId);

    var purpleDaylist = await _dbContext.Playlists.Where(p => p.Name == "Purple Day List" && p.UserId == userId)
    .ProjectTo<GetPlayListDTO>(_mapper.ConfigurationProvider)
    .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");

    await _libraryService.CheckSongsOnLibraryWithUser(purpleDaylist.Songs, userId);

    return purpleDaylist;
  }


  /// <summary>
  /// Actualiza la lista de recomendaciones de un usuario (Purple daylist)
  /// </summary>
  /// <param name="playlistId"></param>
  /// <param name="userId"></param>
  /// <param name="_dbContext"></param>
  /// <returns></returns>
  public async Task UpdatePurpuraDayList(string userId)
  {

    var playlist = await _dbContext.Playlists!.Where(p => p.Name == "Purple Day List" && p.UserId == userId)
    .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");

    if ((DateTime.UtcNow.Date - playlist.LastUpdated.Date) > TimeSpan.FromDays(3))
    {
      var recomendations = await GetUserRecomendations(userId);
      playlist.Songs = recomendations;
      playlist.LastUpdated = DateTime.UtcNow;
      await _dbContext.SaveChangesAsync();
      return;
    }

    throw new InternalServerException("An error occurred while updating your purple daylist");

  }


  /// <summary>
  /// Método de apoyo para obtener las canciones recomendadas de un usuario según su historial de reproducciones.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="_dbContext"></param>
  /// <returns></returns>
 public async Task<List<Song>> GetUserRecomendations(string userId)
{

  // Obtener las canciones reproducidas recientemente y sus géneros
  var recentListenGenres = await _dbContext.PlayHistories
    .Where(p => p.UserId == userId)
    .OrderByDescending(p => p.PlayedAt)
    .Take(10)
    .SelectMany(p => p.Song!.Genres!)  // Aplanamos la lista
    .Select(g => g.Id) // Tomamos solo los IDs de los géneros
    .Distinct()
    .ToListAsync();

  // Buscar canciones que tengan al menos un género en la lista obtenida
  var recomendations = await _dbContext.Songs
    .Where(s => s.Genres!.Any(g => recentListenGenres.Contains(g.Id))) // Comparamos con IDs
    .OrderByDescending(s => s.Name)
    .Take(10)
    .ToListAsync();

  return recomendations;
}

}