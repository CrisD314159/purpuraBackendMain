using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace purpuraMain.Migrations
{
    /// <inheritdoc />
    public partial class AddedLibraryAlbumRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlbumLibrary",
                columns: table => new
                {
                    AlbumsId = table.Column<string>(type: "text", nullable: false),
                    LibrariesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumLibrary", x => new { x.AlbumsId, x.LibrariesId });
                    table.ForeignKey(
                        name: "FK_AlbumLibrary_Albums_AlbumsId",
                        column: x => x.AlbumsId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumLibrary_Libraries_LibrariesId",
                        column: x => x.LibrariesId,
                        principalTable: "Libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlbumLibrary_LibrariesId",
                table: "AlbumLibrary",
                column: "LibrariesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlbumLibrary");
        }
    }
}
