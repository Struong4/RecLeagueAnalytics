using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngestionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DefensiveRebounds",
                table: "StatLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OffensiveRebounds",
                table: "StatLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PersonalFouls",
                table: "StatLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThreePointersAttempted",
                table: "StatLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThreePointersMade",
                table: "StatLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "DefensiveRebounds",
                table: "StatLines");

            migrationBuilder.DropColumn(
                name: "OffensiveRebounds",
                table: "StatLines");

            migrationBuilder.DropColumn(
                name: "PersonalFouls",
                table: "StatLines");

            migrationBuilder.DropColumn(
                name: "ThreePointersAttempted",
                table: "StatLines");

            migrationBuilder.DropColumn(
                name: "ThreePointersMade",
                table: "StatLines");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Games");
        }
    }
}
