

namespace purpuraMain.Dto.InputDto;

public class CreateUserDTO
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Phone { get; set; }
    public int Country { get; set; }
}