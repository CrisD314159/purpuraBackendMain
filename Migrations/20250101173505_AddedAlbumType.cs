using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace purpuraMain.Migrations
{
    /// <inheritdoc />
    public partial class AddedAlbumType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlbumType",
                table: "Albums",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumType",
                table: "Albums");
        }
    }
}
