namespace purpuraMain.Dto.InputDto;

public class AddRemoveSongDTO
{
  public required Guid PlaylistId { get; set; }
  public required string SongId { get; set; }
}