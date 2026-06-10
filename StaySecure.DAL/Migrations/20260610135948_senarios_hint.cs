using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaySecure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class senarios_hint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Difficulty",
                table: "Scenarios",
                newName: "Level");

            migrationBuilder.AddColumn<bool>(
                name: "HintUsed",
                table: "UserScenarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AgeGroup",
                table: "Scenarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Hint",
                table: "Scenarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HintPenalty",
                table: "Scenarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HintUsed",
                table: "UserScenarios");

            migrationBuilder.DropColumn(
                name: "AgeGroup",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "Hint",
                table: "Scenarios");

            migrationBuilder.DropColumn(
                name: "HintPenalty",
                table: "Scenarios");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Scenarios",
                newName: "Difficulty");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Users",
                type: "int",
                nullable: true);
        }
    }
}
