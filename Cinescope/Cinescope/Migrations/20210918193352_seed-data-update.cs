using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinescope.Web.Migrations
{
    public partial class seeddataupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Films",
                keyColumn: "ID",
                keyValue: 3,
                column: "Afis",
                value: "https://tr.web.img4.acsta.net/pictures/16/11/03/16/33/532910.jpg");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Films",
                keyColumn: "ID",
                keyValue: 3,
                column: "Afis",
                value: "https://resim.fullhdfilmizlesene.com/mdsresim_izle/the-unhealer-33773.jpg");
        }
    }
}
