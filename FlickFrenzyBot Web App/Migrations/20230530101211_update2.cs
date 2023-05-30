using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlickFrenzyBot_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currentMessageId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "currentMovieId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "currentMessageId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "currentMovieId",
                table: "Users",
                type: "integer",
                nullable: true);
        }
    }
}
