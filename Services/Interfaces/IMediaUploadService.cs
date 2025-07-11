namespace purpuraMain.Services.Interfaces;

public interface IMediaUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UploadAudioAsync(IFormFile file);
}