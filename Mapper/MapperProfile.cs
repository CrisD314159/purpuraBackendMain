using AutoMapper;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Model;

namespace purpuraMain.Mapper;

public class MapperProfile : Profile
{
  public MapperProfile()
  {
    CreateMap<Album, GetAlbumDTO>()
    .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist.Name))
    .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
    .ForMember(dest => dest.Producer, opt => opt.MapFrom(src => src.ProducerName))
    .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.Songs.OrderBy(s => s.AlbumTrack)));

    CreateMap<Song, GetSongDTO>()
    .ForMember(dest => dest.AlbumName, opt => opt.MapFrom(src => src.Album.Name))
    .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists))
    .ForMember(dest => dest.Disclaimer, opt => opt.MapFrom(src => src.Disclaimer))
    .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre!.Name));

    CreateMap<Genre, GetGenreDTO>()
    .ForMember(dest => dest.Songs, opt => opt.Ignore());

    CreateMap<Artist, GetPlaylistArtistDTO>();

    CreateMap<Artist, GetArtistDTO>()
    .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums))
    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.PictureUrl))
    .ForMember(dest => dest.TopSongs, opt => opt.Ignore());

    CreateMap<Library, GetLibraryDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
    .ForMember(dest => dest.Playlists, opt => opt.Ignore())
    .ForMember(dest => dest.Songs, opt => opt.Ignore());

    CreateMap<Playlist, GetLibraryPlaylistDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<Album, GetLibraryAlbumDTO>()
    .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist.Name));

    CreateMap<Playlist, GetPlayListDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<Playlist, GetUserPlayListsDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<User, GetUserDto>()
    .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.State == UserState.ACTIVE))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName));


  }
}