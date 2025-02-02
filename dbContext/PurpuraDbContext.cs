namespace purpuraMain.DbContext;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Model;


// El dbcontext es el contexto necesario para poder que ef core mapee o estructure la base de datos en el motor
// Esto debe tener las entidades que queremos ver reflejadas en la base de datos y de importa desde el Program.cs para la inyección de dependencias
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
  public DbSet<Session>? Sessions { get; set; }
  public DbSet<Admin>? Admins { get; set; }
  public DbSet<AdminSessions>? AdminSessions { get; set; }

}