namespace purpuraMain.Dto.OutputDto;

public class GetLibraryDTO
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public List<GetSongDTO>? Songs { get; set; }
    public List<GetLibraryPlaylistDTO>? Playlists { get; set; }
    public List<GetLibraryAlbumDTO>? Albums { get; set; }
}