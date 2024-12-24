namespace purpuraMain.Dto.OutputDto;

public class GetArtistDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string Description { get; set; }
  public required string ImageUrl { get; set; }
  public List<GetSongDTO>? TopSongs { get; set; }

  public List<GetLibraryAlbumDTO>? Albums {get; set;}
}