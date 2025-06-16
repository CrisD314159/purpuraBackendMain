using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class CreateSongValidation : AbstractValidator<CreateSingleSongDTO>
{
  public CreateSongValidation()
  {
    RuleFor(song => song.Disclaimer).MinimumLength(5).MaximumLength(100);
    RuleFor(song => song.Name).MinimumLength(2).MaximumLength(80);
  }
}