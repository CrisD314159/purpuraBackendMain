using AutoMapper;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Model;

namespace purpuraMain.Mapper;

public class MapperProfile : Profile
{
  MapperProfile()
  {
    CreateMap<Album, GetAlbumDTO>()
    .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist.Name))
    .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
    .ForMember(dest => dest.Producer, opt => opt.MapFrom(src => src.ProducerName))
    .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.Songs));

    CreateMap<Song, GetSongDTO>()
    .ForMember(dest => dest.AlbumName, opt => opt.MapFrom(src => src.Album.Name))
    .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists))
    .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre));

    CreateMap<Genre, GetGenreDTO>()
    .ForMember(dest => dest.Songs, opt => opt.Ignore());

    CreateMap<Artist, GetPlaylistArtistDTO>();

    CreateMap<Artist, GetArtistDTO>()
    .ForMember(dest => dest.Albums, opt => opt.MapFrom(src => src.Albums))
    .ForMember(dest => dest.TopSongs, opt => opt.Ignore());

    CreateMap<Library, GetLibraryDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
    .ForMember(dest => dest.Playlists, opt => opt.Ignore())
    .ForMember(dest => dest.Songs, opt => opt.Ignore());

    CreateMap<Playlist, GetLibraryPlaylistDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<Playlist, GetPlayListDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<Playlist, GetUserPlayListsDTO>()
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

    CreateMap<User, GetUserDto>()
    .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(src => src.State == UserState.ACTIVE));


  }
}