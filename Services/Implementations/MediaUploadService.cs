using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using purpuraMain.Exceptions;
using purpuraMain.Services.Interfaces;
using purpuraMain.Utils;

namespace purpuraMain.Services.Implementations;

public class MediaUploadService: IMediaUploadService
{
   private readonly Cloudinary _cloudinary;

    public MediaUploadService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
        _cloudinary.Api.Secure = true;
    }
  public async Task<string> UploadAudioAsync(IFormFile file)
  {
    await using var stream = file.OpenReadStream();

    var uploadParams = new VideoUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        Folder = "purpuraSongs"
    };

    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    return uploadResult.SecureUrl?.ToString()
    ?? throw new InternalServerException("Cannot upload song");
  }

  public async Task<string> UploadImageAsync(IFormFile file)
  {
    await using var stream = file.OpenReadStream();

    var uploadParams = new ImageUploadParams
    {
        File = new FileDescription(file.FileName, stream),
        Folder = "purpuraImages"
    };

    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    return uploadResult.SecureUrl?.ToString()
    ?? throw new InternalServerException("Cannot upload image");
  }
}