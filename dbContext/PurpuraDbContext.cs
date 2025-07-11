namespace purpuraMain.DbContext;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Model;


// El dbcontext es el contexto necesario para poder que ef core mapee o estructure la base de datos en el motor
// Esto debe tener las entidades que queremos ver reflejadas en la base de datos y de importa desde el Program.cs para la inyección de dependencias
public class PurpuraDbContext : IdentityDbContext<User>
{

  public PurpuraDbContext(DbContextOptions<PurpuraDbContext> options)
  : base(options)
  {

  }

  // Aquí se definen las tablas que se quieren ver reflejadas en la base de datos
  // Las tablas de muchos a muchos como SongPlaylists las genera ef core automáticamente cuando lee nuestro modelo
  public DbSet<Library> Libraries { get; set; }
  public DbSet<Artist> Artists { get; set; }
  public DbSet<Genre> Genres { get; set; }
  public DbSet<Song> Songs { get; set; }
  public DbSet<Playlist> Playlists { get; set; }
  public DbSet<Album> Albums { get; set; }
  public DbSet<PlayHistory> PlayHistories { get; set; }
  public DbSet<Session> Sessions { get; set; }
  public DbSet<AdminSessions> AdminSessions { get; set; }


  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    // When an album deletes, its songs also are deleted
    builder.Entity<Album>()
    .HasMany(a => a.Songs)
    .WithOne(s => s.Album)
    .HasForeignKey(s => s.AlbumId)
    .OnDelete(DeleteBehavior.Cascade);

  }

}