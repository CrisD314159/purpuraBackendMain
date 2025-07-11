using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;

public class UpdateSongValidation : AbstractValidator<UpdateSingleSongDTO>
{
  public UpdateSongValidation()
  {
    RuleFor(album => album.Name).MinimumLength(2).MaximumLength(80);
    RuleFor(album => album.Disclaimer).MinimumLength(5).MaximumLength(200);
    RuleFor(song => song.ImageUrl).NotEmpty().NotNull();
    RuleFor(song => song.Artists).NotEmpty();
    RuleFor(song => song.AudioUrl).NotEmpty().NotNull();
    RuleFor(song => song.GenreId).NotEmpty().NotNull();
  }
}