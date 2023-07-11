using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinescope.Web.Migrations
{
    public partial class filmplayer6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerFilms_FilmId",
                table: "PlayerFilms",
                column: "FilmId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerFilms_PlayerId",
                table: "PlayerFilms",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFilms_Films_FilmId",
                table: "PlayerFilms",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerFilms_Players_PlayerId",
                table: "PlayerFilms",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFilms_Films_FilmId",
                table: "PlayerFilms");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerFilms_Players_PlayerId",
                table: "PlayerFilms");

            migrationBuilder.DropIndex(
                name: "IX_PlayerFilms_FilmId",
                table: "PlayerFilms");

            migrationBuilder.DropIndex(
                name: "IX_PlayerFilms_PlayerId",
                table: "PlayerFilms");
        }
    }
}
