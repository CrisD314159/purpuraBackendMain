using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace purpuraMain.Migrations
{
    /// <inheritdoc />
    public partial class AddedVerificationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VerifyCode",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifyCode",
                table: "Users");
        }
    }
}
