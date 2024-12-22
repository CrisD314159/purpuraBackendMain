namespace purpuraMain.Model;

public class Album
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public required string PictureUrl { get; set; }
  public required string ArtistId { get; set; }
  public required Artist Artist { get; set; }
  public DateTime ReleaseDate { get; set; }
  public string? WriterName { get; set; }
  public string? ProducerName { get; set; }
  public string? RecordLabel { get; set; }
   public ICollection<Library> Libraries { get; set; } = [];
  


  
}