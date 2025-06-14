namespace purpuraMain.Validations;
using FluentValidation;
using purpuraMain.Dto.InputDto;

public class UserValidator : AbstractValidator<CreateUserDTO>
{
    public UserValidator()
    {
        RuleFor(user => user.Email).EmailAddress().NotEmpty();
        RuleFor(user => user.Password).MinimumLength(8).NotEmpty().Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\-])[A-Za-z\d@$!%*?&\-]{8,}$")
        .WithMessage("Password must be at least 8 characters long, include at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*-?&).");
        RuleFor(user => user.Name).MinimumLength(2).MaximumLength(30).NotEmpty();
    }
}