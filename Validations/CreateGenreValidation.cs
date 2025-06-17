using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class CreateGenreValidation : AbstractValidator<CreateGenreDTO>
{
  public CreateGenreValidation()
  {
    RuleFor(genre => genre.Description).MinimumLength(5).MaximumLength(200);
    RuleFor(genre => genre.Name).MinimumLength(2).MaximumLength(80);
    RuleFor(genre => genre.Color).MinimumLength(2).MaximumLength(10);
  }
}