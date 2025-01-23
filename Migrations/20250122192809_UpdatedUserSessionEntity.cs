using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace purpuraMain.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserSessionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FingerPrint",
                table: "Sessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresdAt",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresdAt",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "FingerPrint",
                table: "Sessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
