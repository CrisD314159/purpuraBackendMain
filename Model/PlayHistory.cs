namespace purpuraMain.Model;

public class PlayHistory
{
  public required string Id { get; set; }

  public string? UserId { get; set; }
  public User? User { get; set; }

  public Guid? SongId { get; set; }
  public Song? Song { get; set; }
  
  public DateTime PlayedAt { get; set; }
  public int PlayCount { get; set; }


}