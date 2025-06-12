using AutoMapper;
using purpuraMain.Dto.OutputDto;
using purpuraMain.Model;

namespace purpuraMain.Mapper;

public class MapperProfile : Profile
{
  MapperProfile()
  {
    CreateMap<Album, GetAlbumDTO>()
    .ForMember(dest => dest.AlbumType, opt => opt.MapFrom(src => src.AlbumType))
    .ForMember(dest => dest.ArtistId, opt => opt.MapFrom(src => src.ArtistId))
    .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist.Name))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.GenreId, opt => opt.MapFrom(src => src.GenreId))
    .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.Name))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl))
    .ForMember(dest => dest.Producer, opt => opt.MapFrom(src => src.ProducerName))
    .ForMember(dest => dest.RecordLabel, opt => opt.MapFrom(src => src.RecordLabel))
    .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate))
    .ForMember(dest => dest.Songs, opt => opt.MapFrom(src => src.Songs));

    CreateMap<Song, GetSongDTO>()
    .ForMember(dest => dest.AlbumId, opt => opt.MapFrom(src => src.AlbumId))
    .ForMember(dest => dest.AlbumName, opt => opt.MapFrom(src => src.Album.Name))
    .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists))
    .ForMember(dest => dest.AudioUrl, opt => opt.MapFrom(src => src.AudioUrl))
    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
    .ForMember(dest => dest.Lyrics, opt => opt.MapFrom(src => src.Lyrics))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

    CreateMap<Genre, GetGenreDTO>()
    .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

    CreateMap<Artist, GetPlaylistArtistDTO>()
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));



  }
}