using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaySecure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Senarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    ScenarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioOptions_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScenarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTranslations_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioOptionTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScenarioOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioOptionTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioOptionTranslations_ScenarioOptions_ScenarioOptionId",
                        column: x => x.ScenarioOptionId,
                        principalTable: "ScenarioOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioOptions_ScenarioId",
                table: "ScenarioOptions",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioOptionTranslations_ScenarioOptionId",
                table: "ScenarioOptionTranslations",
                column: "ScenarioOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTranslations_ScenarioId",
                table: "ScenarioTranslations",
                column: "ScenarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScenarioOptionTranslations");

            migrationBuilder.DropTable(
                name: "ScenarioTranslations");

            migrationBuilder.DropTable(
                name: "ScenarioOptions");

            migrationBuilder.DropTable(
                name: "Scenarios");
        }
    }
}
