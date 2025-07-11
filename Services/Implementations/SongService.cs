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
using purpuraMain.Dto.InputDto;
using FluentValidation;

public class SongService(PurpuraDbContext dbContext, IMapper mapper, ILibraryService libraryService, UserManager<User> userManager,
IValidator<CreateSingleSongDTO> createSongValidator, IValidator<UpdateSingleSongDTO> updateSongValidator, IValidator<AddSongToAlbumDTO> addSongToAlbumValidator
) : ISongService
{

    private readonly PurpuraDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;
    private readonly ILibraryService _libraryService = libraryService;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IValidator<CreateSingleSongDTO> _createSongValidator= createSongValidator;
    private readonly IValidator<UpdateSingleSongDTO> _updateSingleSongDTO= updateSongValidator;
    private readonly IValidator<AddSongToAlbumDTO> _addSongToAlbumValidator= addSongToAlbumValidator;

    /// <summary>
    /// Obtiene una canción por su ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public async Task<GetSongDTO> GetSongById(string userId, Guid songId)
    {

        var isOnLibrary = await _dbContext.Libraries.Where(l => l.UserId == userId && l.User.State == UserState.ACTIVE)
        .AnyAsync(l => l.Songs.Any(s=> s.Id == songId));

        var song = await _dbContext.Songs!.Where(s => s.Id == songId)
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

        var songs = await _dbContext.Songs.Where(s =>
        EF.Functions.ILike(s.Name, $"%{input}%")
        || s.Artists.Any(a => EF.Functions.ILike(a.Name, $"%{input}%"))
        || EF.Functions.ILike(s.Album.Name, $"%{input}%"))
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

        var songsPlays = await _dbContext.Songs
        .Include(s => s.Album)
        .Include(s => s.Artists)
        .Select(s => new
        {
            Song = s,
            Plays = _dbContext.PlayHistories.Count(ph => ph.SongId == s.Id)
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
            AlbumName = s.Song.Album != null ? s.Song.Album.Name : "",
            ImageUrl = s.Song.ImageUrl ?? "",
            AudioUrl = s.Song.AudioUrl ?? "",
            GenreId = s.Song.GenreId,
            IsOnLibrary = false,
            Lyrics = s.Song.Lyrics ?? "",
            Plays = s.Plays,
            Disclaimer = s.Song.Disclaimer 
                
            }).ToList();

       if(userId != "0") await _libraryService.CheckSongsOnLibraryWithUser(songs, userId);

        return songs;
     
    }

    /// <summary>
    /// Registra un reproducción de una canción cuando en el cliente se reproduce
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="songId"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<bool> AddSongPlay(string userId, Guid songId )
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

    /// <summary>
    /// Creates a song using a data transfer object
    /// </summary>
    /// <param name="createSingleSongDTO"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task CreateSong(CreateSingleSongDTO createSingleSongDTO)
    {
        _createSongValidator.ValidateAndThrow(createSingleSongDTO);

        var artists = await _dbContext.Artists
        .Where(a => createSingleSongDTO.Artists.Contains(a.Id))
        .ToListAsync();

        if (artists.Count == 0) throw new EntityNotFoundException("No artists were found");


        var genre = await _dbContext.Genres.FindAsync(createSingleSongDTO.GenreId)
        ?? throw new EntityNotFoundException("Genre not found");

        var albumDTO = new CreateSingleSongAlbumDTO
        {
            Name = createSingleSongDTO.Name,
            ImageUrl = createSingleSongDTO.ImageUrl,
            Disclaimer = createSingleSongDTO.Disclaimer,
            Artist = artists[0],
            Genre = genre
        };

        var album = await CreateAlbumForSingleSong(albumDTO);

        var song = new Song
        {
            Name = createSingleSongDTO.Name,
            AlbumId = album.Id,
            Album = album,
            AudioUrl = createSingleSongDTO.AudioUrl,
            ImageUrl = createSingleSongDTO.ImageUrl,
            DateAdded = DateTime.UtcNow,
            Artists = artists,
            Disclaimer = createSingleSongDTO.Disclaimer,
            Genre = genre,
            GenreId = genre.Id,
            Lyrics = createSingleSongDTO.Lyrics,
            AlbumTrack = 1
        };

        await _dbContext.Songs.AddAsync(song);
        await _dbContext.SaveChangesAsync();

    }

    /// <summary>
    /// Creates an album to contain a single song release
    /// </summary>
    /// <param name="createSingleSongAlbumDTO"></param>
    /// <returns></returns>
    private async Task<Album> CreateAlbumForSingleSong(CreateSingleSongAlbumDTO createSingleSongAlbumDTO)
    {
        var album = new Album
        {
            Name = createSingleSongAlbumDTO.Name,
            PictureUrl = createSingleSongAlbumDTO.ImageUrl,
            Artist = createSingleSongAlbumDTO.Artist,
            ArtistId = createSingleSongAlbumDTO.Artist.Id,
            DateAdded = DateTime.UtcNow,
            Genre = createSingleSongAlbumDTO.Genre,
            GenreId = createSingleSongAlbumDTO.Genre.Id,
            AlbumType = AlbumType.SINGLE,
            Disclaimer = createSingleSongAlbumDTO.Disclaimer
        };

        await _dbContext.Albums.AddAsync(album);
        await _dbContext.SaveChangesAsync();

        return album;
        
    }

    /// <summary>
    /// Updates a song using a data transfer protocol
    /// </summary>
    /// <param name="updateSingleSongDTO"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task UpdateSong(UpdateSingleSongDTO updateSingleSongDTO)
    {
        _updateSingleSongDTO.ValidateAndThrow(updateSingleSongDTO);

        var artists = await _dbContext.Artists
       .Where(a => updateSingleSongDTO.Artists.Contains(a.Id))
       .ToListAsync();

        if (artists.Count == 0) throw new EntityNotFoundException("No artists were found");

        var genre = await _dbContext.Genres.FindAsync(updateSingleSongDTO.GenreId)
        ?? throw new EntityNotFoundException("Genre not found");

        var song = await _dbContext.Songs.FindAsync(updateSingleSongDTO.Id)
        ?? throw new EntityNotFoundException("Song not found");

        song.Album.Artist = artists[0];
        song.Album.ArtistId = artists[0].Id;
        song.Album.Disclaimer = updateSingleSongDTO.Disclaimer;
        song.Album.Genre = genre;
        song.Album.GenreId = genre.Id;
        song.Album.Name = updateSingleSongDTO.Name;
        song.Album.PictureUrl = updateSingleSongDTO.ImageUrl;

        song.Artists = artists;
        song.Disclaimer = updateSingleSongDTO.Disclaimer;
        song.Genre = genre;
        song.GenreId = genre.Id;
        song.Name = updateSingleSongDTO.Name;
        song.ImageUrl = updateSingleSongDTO.ImageUrl;
        song.AudioUrl = updateSingleSongDTO.AudioUrl;
        song.Lyrics = updateSingleSongDTO.Lyrics;

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a song using its provided id
    /// </summary>
    /// <param name="songId"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteSong(Guid songId)
    {
        var song = await _dbContext.Songs.FindAsync(songId)
        ?? throw new EntityNotFoundException("Song Not found");

        _dbContext.Albums.Remove(song.Album);
    }

    /// <summary>
    /// Adds a song to an existing album
    /// </summary>
    /// <param name="addSongToAlbumDTO"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task AddSongToAlbum(AddSongToAlbumDTO addSongToAlbumDTO)
    {
        _addSongToAlbumValidator.ValidateAndThrow(addSongToAlbumDTO);

        var artists = await _dbContext.Artists
       .Where(a => addSongToAlbumDTO.Artists.Contains(a.Id))
       .ToListAsync();

        if (artists.Count == 0) throw new EntityNotFoundException("No artists were found");

        var album = await _dbContext.Albums.FindAsync(addSongToAlbumDTO.AlbumId)
        ?? throw new EntityNotFoundException("Album not found");

        var song = new Song
        {
            Name = addSongToAlbumDTO.Name,
            AlbumId = album.Id,
            Album = album,
            AudioUrl = addSongToAlbumDTO.AudioUrl,
            ImageUrl = album.PictureUrl,
            DateAdded = DateTime.UtcNow,
            Artists = artists,
            Disclaimer = addSongToAlbumDTO.Disclaimer,
            Genre = album.Genre,
            GenreId = album.GenreId,
            Lyrics = addSongToAlbumDTO.Lyrics,
            AlbumTrack = addSongToAlbumDTO.AlbumTrack
        };

        album.Songs.Add(song);
        await _dbContext.Songs.AddAsync(song);

        await _dbContext.SaveChangesAsync();
    }
}