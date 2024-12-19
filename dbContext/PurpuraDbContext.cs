namespace purpuraMain.DbContext;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Model;

public class PurpuraDbContext(DbContextOptions<PurpuraDbContext> options) : DbContext(options)
{
  public DbSet<Library>? Libraries { get; set; }
  public DbSet<Artist>? Artists { get; set; }
  public DbSet<Genre>? Genres { get; set; }
  public DbSet<Song>? Songs { get; set; }
  public DbSet<Playlist>? Playlists { get; set; }
  public DbSet<Album>? Albums { get; set; }
  public DbSet<User>? Users { get; set; }
  public DbSet<PlayHistory>? PlayHistories { get; set; }
  public DbSet<Country>? Countries { get; set; }

}