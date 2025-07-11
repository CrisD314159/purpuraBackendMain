namespace purpuraMain.Model;
public class Session
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string UserId {get; set;}
  public DateTime CreatedAt {get; set;}
  public DateTime ExpiresdAt {get; set;}
}