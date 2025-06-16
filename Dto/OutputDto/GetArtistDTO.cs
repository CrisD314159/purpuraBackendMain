namespace purpuraMain.Dto.OutputDto;

public class GetArtistDTO
{
  public required Guid Id { get; set; }
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required string PictureUrl { get; set; }
  public List<GetSongDTO>? TopSongs { get; set; }

  public List<GetLibraryAlbumDTO>? Albums {get; set;}
}