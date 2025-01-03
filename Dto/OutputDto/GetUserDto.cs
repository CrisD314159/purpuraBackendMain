namespace purpuraMain.Dto.OutputDto;

public class GetUserDto
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; } 
    public required string Phone { get; set; }
    public  required string Country { get; set; }
    public string? ProfilePicture { get; set; }
    public bool? IsVerified { get; set; }
}