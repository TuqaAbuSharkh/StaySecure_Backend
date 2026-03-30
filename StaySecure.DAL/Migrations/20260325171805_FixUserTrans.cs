using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaySecure.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixUserTrans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationUserTranslations_Users_ApplicationUserId",
                table: "applicationUserTranslations");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "applicationUserTranslations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_applicationUserTranslations_Users_ApplicationUserId",
                table: "applicationUserTranslations",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationUserTranslations_Users_ApplicationUserId",
                table: "applicationUserTranslations");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "applicationUserTranslations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_applicationUserTranslations_Users_ApplicationUserId",
                table: "applicationUserTranslations",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
