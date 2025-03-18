namespace purpuraMain.Utils;
using CloudinaryDotNet;
using DotNetEnv;
using CloudinaryDotNet.Actions;


public class ImageService
{
  Cloudinary _cloudinary;

  // Constructor donde se inicializa la variable _cloudinary con la API Key de Cloudinary.
  public ImageService()
  {
    string apiKey = Env.GetString("CLOUDINARY");
    _cloudinary = new Cloudinary(apiKey);
  }

    /// <summary>
    /// Sube una imagen a Cloudinary, que en este caso se utilizaría para subir imágenes de perfil de usuario.
    /// </summary>
    /// <param name="imageStream"></param>
    /// <param name="fileName"></param>
    /// <returns>URL de la imagen subuda en cloudinary</returns>
    /// <exception cref="Exception"></exception>
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