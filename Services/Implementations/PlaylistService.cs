namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using purpuraMain.Dto.OutputDto;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Exceptions;
using purpuraMain.Dto.InputDto;
using purpuraMain.Validations;
using System.ComponentModel.DataAnnotations;
using CloudinaryDotNet.Actions;
using purpuraMain.Services.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

public class PlaylistService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService
, IValidator<CreatePlayListDTO> createValidator, IValidator<UpdatePlaylistDTO> updateValidator, UserManager<User> userManager,
IUserService userService
) : IPlaylistService
{

  private readonly PurpuraDbContext _dbcontext = dbContext;
  private readonly IMapper _mapper = mapper;
  private readonly ILibraryService _libraryService = libraryService;
  private readonly IUserService _userService = userService;
  private readonly IValidator<CreatePlayListDTO> _createValidator = createValidator;
  private readonly IValidator<UpdatePlaylistDTO> _updatePlaylistDTO = updateValidator;
  private readonly UserManager<User> _userManager = userManager;

  /// <summary>
  /// Obtiene la información de una playlist verificando primero si es del usuario que la solicita.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="playListId"></param>
  /// <param name="dbcontext"></param>
  /// <returns></returns>
  public async Task<GetPlayListDTO> GetPlaylist(string userId, Guid playListId)
  {

    //Esto es para verificar si el usuario que hace la petición puede ver la playlist
    if (await _dbcontext.Playlists.AnyAsync(p => p.Id == playListId && p.UserId != userId && p.IsPublic == false)) throw new EntityNotFoundException("Playlist not found");
    var playList = await
    _dbcontext.Playlists!.Where(p => p.Id == playListId && p.UserId == userId)
    .ProjectTo<GetPlayListDTO>(_mapper.ConfigurationProvider)
    .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");

    await _libraryService.CheckSongsOnLibraryWithUser(playList.Songs, userId);


    return playList;


  }

  /// <summary>
  /// Busca playlists que coincidan con el input del usuario (verifica que sean públicas) (Unused by the moment).
  /// </summary>
  /// <param name="input"></param>
  /// <param name="offset"></param>
  /// <param name="limit"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<List<GetLibraryPlaylistDTO>> SearchPlaylist(string input, int offset, int limit)
  {

    var playList = await _dbcontext.Playlists.Where(p => p.Name.Contains(input, StringComparison.OrdinalIgnoreCase) && p.IsPublic)
    .ProjectTo<GetLibraryPlaylistDTO>(_mapper.ConfigurationProvider)
    .Skip(offset).Take(limit).ToListAsync();

    return playList;
    
  }

  /// <summary>
  /// Añade una canción a una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="addSongDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task AddSong(string userId, AddRemoveSongDTO addSongDTO)
  {

    var playlist = await _dbcontext.Playlists.FindAsync(addSongDTO.PlaylistId) ?? throw new EntityNotFoundException("Playlist not found");
    if(playlist.UserId != userId) throw new UnauthorizedException("You are not authorized to add songs to this playlist");
    var song = await _dbcontext.Songs.FindAsync(addSongDTO.SongId) ?? throw new EntityNotFoundException("Song not found");
    playlist.Songs.Add(song);
    await _dbcontext.SaveChangesAsync();
 
  }


  /// <summary>
  /// Elimina una canción de una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="addSongDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task RemoveSong(string userId, AddRemoveSongDTO addSongDTO)
  {

      var playlist = await _dbcontext.Playlists.Include(p => p.Songs).Where(p => p.Id == addSongDTO.PlaylistId).FirstOrDefaultAsync()
      ?? throw new EntityNotFoundException("Playlist not found");
      
      if (playlist.UserId != userId) throw new UnauthorizedException("You are not authorized to remove songs from this playlist");

      var song = await _dbcontext.Songs.FindAsync(addSongDTO.SongId)
      ?? throw new EntityNotFoundException("Song not found");

      playlist.Songs.Remove(song);
      await _dbcontext.SaveChangesAsync();

  }

  /// <summary>
  /// Cambia la privacidad de una playlist (Unused by now).
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="changePrivacy"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task ChangePlayListState(string userId, ChangePrivacyPlaylistDto changePrivacy)
  {

      var playList = await _dbcontext.Playlists.FindAsync(changePrivacy.PlaylistId) ?? throw new EntityNotFoundException("Playlist not found");
      if(playList.UserId != userId) throw new UnauthorizedException("You are not authorized to change the privacy of this playlist");
      playList.IsPublic = !playList.IsPublic;
      await _dbcontext.SaveChangesAsync();

  }

  /// <summary>
  /// Obtiene las playlists de un usuario.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task<List<GetUserPlayListsDTO>> GetUserPlayLists(string userId)
  {
    var playLists = await _dbcontext.Playlists!.Where(p => p.UserId == userId && p.Name != "Purple Day List")
    .ProjectTo<GetUserPlayListsDTO>(_mapper.ConfigurationProvider)
    .ToListAsync();

    return playLists;
  }


  /// <summary>
  /// Crea una nueva playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="createPlayListDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task CreatePlayList(string userId, CreatePlayListDTO createPlayListDTO)
  {

    _createValidator.ValidateAndThrow(createPlayListDTO);

    if (await _dbcontext.Playlists.AnyAsync(p => p.Name == createPlayListDTO.Name && p.UserId == userId))
      throw new BadRequestException("There is already a playlist with that name on your library");

    var user = await _userService.VerifyAndReturnValidUser(userId);

    var library = await _dbcontext.Libraries.Where(l => l.UserId == userId)
    .FirstOrDefaultAsync()
    ?? throw new EntityNotFoundException("Library not found");


    var playList = new Playlist
    {
      Name = createPlayListDTO.Name,
      Description = createPlayListDTO.Description ?? "",
      UserId = userId,
      User = user,
      ImageUrl = createPlayListDTO.ImageUrl ?? "https://res.cloudinary.com/dw43hgf5p/image/upload/v1735657347/qheqts3xhcejrmwu5hur.jpg",
      IsPublic = true,
      Editable = true,
      CreatedAt = DateTime.UtcNow
    };

    await _dbcontext.Playlists.AddAsync(playList);
    library.Playlists.Add(playList);
    await _dbcontext.SaveChangesAsync();

  }


  /// <summary>
  /// Actualiza una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="updatePlaylistDTO"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  public async Task UpdatePlayList(string userId, UpdatePlaylistDTO updatePlaylistDTO)
  {
    _updatePlaylistDTO.ValidateAndThrow(updatePlaylistDTO);
    var playList = await _dbcontext.Playlists.Where(p => p.Id == updatePlaylistDTO.Id && p.UserId == userId)
    .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");
      
    playList.Name = updatePlaylistDTO.Name;
    playList.Description = updatePlaylistDTO.Description ?? "";
    if(!string.IsNullOrEmpty(updatePlaylistDTO.ImageUrl))
    {
      playList.ImageUrl = updatePlaylistDTO.ImageUrl;
    }
    await _dbcontext.SaveChangesAsync();
  }


  /// <summary>
  /// Elimina una playlist.
  /// </summary>
  /// <param name="userId"></param>
  /// <param name="deletePlaylist"></param>
  /// <param name="_dbcontext"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  public async Task DeletePlayList(string userId, DeletePlayListDTO deletePlaylist)
  {

      var playList = await _dbcontext.Playlists.Where(p => p.Id == deletePlaylist.Id && p.UserId == userId)
      .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("Playlist not found");
      _dbcontext.Playlists.Remove(playList);
      await _dbcontext.SaveChangesAsync();
  }

}
