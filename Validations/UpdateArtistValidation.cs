using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class UpdateArtistValidation : AbstractValidator<UpdateArtistDTO>
{
  public UpdateArtistValidation()
  {
    RuleFor(artist => artist.Description).MinimumLength(5).MaximumLength(100);
    RuleFor(artist => artist.Name).MinimumLength(2).MaximumLength(80);
  }
}