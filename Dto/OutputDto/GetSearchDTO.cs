namespace purpuraMain.Dto.OutputDto;
public class GetSearchDTO{
  public List<GetSongDTO>? Songs {get; set;}
  public List<GetLibraryPlaylistDTO>? Playlists {get; set;}
  public List<GetArtistDTO>? Artists {get; set;}

}