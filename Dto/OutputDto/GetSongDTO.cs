namespace purpuraMain.Dto.OutputDto;

public class GetSongDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required List<GetPlaylistArtistDTO> Artists { get; set; }
  public required string AlbumId { get; set; }
  public required string AlbumName { get; set; }
  public  required double Duration { get; set; }
  public required string? ImageUrl { get; set; }
  public required string? AudioUrl { get; set; }
  public DateTime? ReleaseDate { get; set; }
  public  List<GetGenreDTO>? Genres { get; set; }
  public string? Lyrics { get; set; }
  public int Plays {get; set;}
  public bool? IsOnLibrary {get; set;}

  public string? WriterName { get; set; }
  public string? ProducerName { get; set; }
  public string? RecordLabel { get; set; }
}