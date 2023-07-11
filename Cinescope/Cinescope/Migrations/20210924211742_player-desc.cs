using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinescope.Web.Migrations
{
    public partial class playerdesc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDesc",
                table: "Player",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "ID",
                keyValue: 1,
                column: "ShortDesc",
                value: "Fatsa doğumlu olan Kadir İnanır, ailesinin son çocuğudur. Fatsa'daki ilkokul ve ortaokul eğitimi sırasında sahne yeteneğini çeşitli okul gösterilerinde sergiledi. İnanır, yatılı olarak okuduğu İstanbul Haydarpaşa Lisesi'nin ardından Marmara Üniversitesi İletişim Fakültesi, Radyo-Televizyon Bölümü’nü bitirdi.");

            migrationBuilder.InsertData(
                table: "Player",
                columns: new[] { "ID", "BirthDate", "BirthPlace", "DateCreated", "DateModified", "DeathDate", "Name", "Photo", "ShortDesc", "Status", "UserCreated", "UserModified" },
                values: new object[] { 2, new DateTime(1962, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Eskişehir", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Serpil Çakmaklı", "images/player/serpil_cakmakli.jpg", "Serpil Çakmaklı ya da gerçek adıyla Şener Dönmez, Türk sinema oyuncusudur. Eskişehir'de doğdu. Sinemadan önce mankenlik yaptı. 1980'li yılların başında TRT televizyonunda yayınlanan Alçaktan Uçan Güvercin adlı mini dizi filmle üne kavuştu. Birçok sinema filminde rol aldı. İlki 16 yaşında olmak üzere üç evlilik yaptı.", 1, 0, 0 });

            migrationBuilder.UpdateData(
                table: "PlayerFilm",
                keyColumn: "ID",
                keyValue: 1,
                column: "FilmId",
                value: 6);

            migrationBuilder.InsertData(
                table: "PlayerFilm",
                columns: new[] { "ID", "DateCreated", "DateModified", "FilmId", "PlayerId", "Status", "UserCreated", "UserModified" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, 2, 1, 0, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Player",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PlayerFilm",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "ShortDesc",
                table: "Player");

            migrationBuilder.UpdateData(
                table: "PlayerFilm",
                keyColumn: "ID",
                keyValue: 1,
                column: "FilmId",
                value: 4);
        }
    }
}
