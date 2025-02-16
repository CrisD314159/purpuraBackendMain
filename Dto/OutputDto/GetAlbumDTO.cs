using purpuraMain.Model;

namespace purpuraMain.Dto.OutputDto;


public class GetAlbumDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string PictureUrl { get; set; }
    public required string ArtistId { get; set; }
    public required string ArtistName { get; set; }
    public string? GenreId { get; set; }
    public string? GenreName { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? RecordLabel {get; set;}
    public string? Writer {get; set;}
    public string? Producer {get; set;}
    public AlbumType? AlbumType { get; set; }
    public List<GetSongDTO>? Songs {get; set;}
    public int TotalPlays { get; set; }
}