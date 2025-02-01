using CloudinaryDotNet;
using DotNetEnv;
using CloudinaryDotNet.Actions;

namespace purpuraMain.Services;

public class ImageService
{
  Cloudinary _cloudinary;

  public ImageService()
  {
    string apiKey = Env.GetString("CLOUDINARY");
    _cloudinary = new Cloudinary(apiKey);
  }


    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, imageStream),
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
            Folder = "purpuraImages"
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.SecureUrl?.ToString() ?? throw new Exception("Image upload failed");
    }

}