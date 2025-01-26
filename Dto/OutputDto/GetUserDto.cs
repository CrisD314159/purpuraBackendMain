namespace purpuraMain.Dto.OutputDto;

public class GetUserDto
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; } 
    public required string SurName { get; set; } 
    public  required string Country { get; set; }
    public string? ProfilePicture { get; set; }
    public bool? IsVerified { get; set; }
}