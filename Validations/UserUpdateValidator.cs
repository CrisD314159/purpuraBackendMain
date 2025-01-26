namespace purpuraMain.Validations;
using FluentValidation;
using purpuraMain.Dto.InputDto;

public class UserUpdateValidator : AbstractValidator<UpdateUserDto>
{
  public UserUpdateValidator()
  {
    RuleFor(user =>user.Id).NotEmpty();
    RuleFor(user => user.FirstName).MinimumLength(2).MaximumLength(30).NotEmpty();
    RuleFor(user => user.SurName).MinimumLength(2).MaximumLength(30).NotEmpty();
  }
}