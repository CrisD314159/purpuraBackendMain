using purpuraMain.Model;

namespace purpuraMain.Dto.OutputDto;


public class GetLibraryAlbumDTO
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Disclaimer { get; set; }
    public required string PictureUrl { get; set; }
    public required Guid ArtistId { get; set; }
    public required string ArtistName { get; set; }
    public DateTime ReleaseDate { get; set; }
}