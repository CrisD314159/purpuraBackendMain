using purpuraMain.Model;

namespace purpuraMain.Dto.OutputDto;

public class GetSongDTO
{
  public required Guid Id { get; set; }
  public required string Name { get; set; }
  public required List<GetPlaylistArtistDTO> Artists { get; set; }
  public required Guid AlbumId { get; set; }
  public required string AlbumName { get; set; }
  public required string ImageUrl { get; set; }
  public required string AudioUrl { get; set; }
  public DateTime? ReleaseDate { get; set; }

  public required Genre Genre { get; set; }
  public string Lyrics { get; set; } = "";
  public int Plays {get; set;}
  public bool IsOnLibrary { get; set; } = false;

  public string WriterName { get; set; } = "";
  public string ProducerName { get; set; } = "";
  public string RecordLabel { get; set; } = "";
}