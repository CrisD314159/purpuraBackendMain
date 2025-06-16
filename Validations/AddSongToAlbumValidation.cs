using FluentValidation;
using purpuraMain.Dto.InputDto;

namespace purpuraMain.Validations;


public class AddSongToAlbumValidation : AbstractValidator<AddSongToAlbumDTO>
{
  public AddSongToAlbumValidation()
  {
    RuleFor(album => album.Name).MinimumLength(2).MaximumLength(80);
    RuleFor(album => album.Disclaimer).MinimumLength(5).MaximumLength(100);

  }
}