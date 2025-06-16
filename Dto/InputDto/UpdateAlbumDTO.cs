using System.ComponentModel.DataAnnotations;

namespace purpuraMain.Dto.InputDto;

public class UpdateAlbumDTO
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public string? Disclaimer { get; set; }
  public string? Description { get; set; }
  [Url]
  public required string ImageUrl { get; set; }
  public required Guid ArtistId { get; set; }
  public DateTime ReleaseDate { get; set; }
  public required Guid GenreId { get; set; }
  public string? WriterName { get; set; }
  public string? ProducerName { get; set; }
  public string? RecordLabel { get; set; }

}