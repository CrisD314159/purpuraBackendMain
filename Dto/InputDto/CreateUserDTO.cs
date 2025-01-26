

namespace purpuraMain.Dto.InputDto;

public class CreateUserDTO
{
    public required string FirstName { get; set; }
    public required string SurName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }

    public int Country { get; set; }
}