namespace purpuraMain.Dto.OutputDto;

public class GetSongDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required List<GetPlaylistArtistDTO> Artists { get; set; }
  public required string AlbumId { get; set; }
  public required string AlbumName { get; set; }
  public  required double Duration { get; set; }
  public required List<GetGenreDTO> Genres { get; set; }
  public string? Lyrics { get; set; }
}