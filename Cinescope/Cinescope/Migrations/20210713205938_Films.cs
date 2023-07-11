using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinescope.Web.Migrations
{
    public partial class Films : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserCreated = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserModified = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Adi = table.Column<string>(nullable: true),
                    Aciklama = table.Column<string>(nullable: true),
                    Afis = table.Column<string>(nullable: true),
                    ImdbPuani = table.Column<decimal>(nullable: false),
                    YapimYili = table.Column<int>(nullable: false),
                    YapimSirketi = table.Column<string>(nullable: true),
                    Sure = table.Column<decimal>(nullable: false),
                    FragmanAdresi = table.Column<string>(nullable: true),
                    IzlemeAdresi = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Lookup",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserCreated = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserModified = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ExtraData = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookup", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserCreated = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    UserModified = table.Column<int>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    LastVisit = table.Column<DateTime>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Films",
                columns: new[] { "ID", "Aciklama", "Adi", "Afis", "DateCreated", "DateModified", "FragmanAdresi", "ImdbPuani", "IzlemeAdresi", "Status", "Sure", "UserCreated", "UserModified", "YapimSirketi", "YapimYili" },
                values: new object[] { 1, @"Next Level, Jake Kasdan tarafından  yönetilen, Jeff Pinkner, Scott Rosenberg ve Jake Kasdan tarafından yazılmış bir Amerikan fantastik macera komedi filmidir. 
2017'deki Jumanji'nin ardından 1995'in Jumanji'sinin ikinci devamı: Ormana Hoş Geldiniz ve genel olarak Jumanji serisinin üçüncü bölümüdür. 
Filmde Dwayne Johnson, Kevin Hart, Jack Black, Karen Gillan, Nick Jonas, Alex Wolff, Morgan Turner, Ser'Darius Blain ve Madison Iseman ve Danny 
DeVito gibi çok ünlü ve yetenekli oyuncular yer almaktadırlar.  Amerika Birleşik Devletleri'nde 13 Aralık 2019 tarihinde Columbia Pictures etiketi
altında Sony Pictures Releasing tarafından piyasaya sürülmesi planlanmaktadır. Spencer, Jumanji video oyununun parçalarını saklamıştır ve bir gün
dedesinin evinin bodrumundaki sistemi tamir ettirir. Spencer'ın arkadaşları Bethany, Fridge ve Martha geldiğinde, Spencer'ı bulamazlar ve oyun çalışır durumdadır.
Onu kurtarmak için Jumanji'ye tekrar girmeye karar verirler. Spencer'ın dedesi Eddie ve arkadaşı Milo Walker bu kargaşayı duyar ve Spencer'in arkadaşları daha
avatarlarını seçemeden  oyunun içine çekilirler. Birinin bu maceradan çıkamayacağını iddia eden Nigel Billingsley tarafından onlara sunulan yeni bir görevle,
genç arkadaşlar Eddie ve Milo'nun oyundaki avatarlarına alışmalarına ve, Spencer'ı bulmalarına yardımcı olmak için son kez Jumanji'den kaçmaya çalışırlar.
Jumanji: The Next Level (2019)", "Jumanji 3 Yeni Seviye", "https://resim.fullhdfilmizlesene.com/mdsresim_izle/fullhd-jumanji-filmi-izlesene.jpg?0.4029940593148297", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 6.9m, null, 1, 123m, 0, 0, null, 2019 });

            migrationBuilder.InsertData(
                table: "Films",
                columns: new[] { "ID", "Aciklama", "Adi", "Afis", "DateCreated", "DateModified", "FragmanAdresi", "ImdbPuani", "IzlemeAdresi", "Status", "Sure", "UserCreated", "UserModified", "YapimSirketi", "YapimYili" },
                values: new object[] { 2, "2020 Amerikan yapım bilim kurgu ve gerilim filmi olan The Unhealar’ın yazarları Kevin E Moore ve J. Shawn Harris’dir. Yönetmen koltuğunda ise Martin Guigui oturuyor. Başlıca oyuncular, Elijah Nelson, Shelby Janes, Kayla Carlson ve Lance Henriksen’dir. Kelly adında lise öğrencisi, elinde olmayan bir hastalığı dolayısıyla, okulda sürekli zorbalığa uğrar. Kendisinin çöp yeme hastalığı vardır ve bu onu çok hasta yapar. Bu çocuktan hoşlanan Dominique adında kız sürekli onu kollar, diğer çocukların ona yanaşmasına izin vermez.", "The Unhealer", "https://resim.fullhdfilmizlesene.com/mdsresim_izle/the-unhealer-33773.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4.5m, null, 1, 94m, 0, 0, null, 2020 });

            migrationBuilder.InsertData(
                table: "Films",
                columns: new[] { "ID", "Aciklama", "Adi", "Afis", "DateCreated", "DateModified", "FragmanAdresi", "ImdbPuani", "IzlemeAdresi", "Status", "Sure", "UserCreated", "UserModified", "YapimSirketi", "YapimYili" },
                values: new object[] { 3, "DC evreninin kahramanı olan Wonder Woman’ı anlatan film Wonder Woman 1984’ün yönetmenliğini Patty Jenkins yapmıştır. Filmin senaryosu Geoff Johns, Dave Callaham, ve Patty Jenkins tarafından William Moulton Marston’un yarattığı çizgi romandan esinlenerek kaleme alınmıştır. Film ülkemizde 15 Ocak 2021’de vizyona girecektir. 4k film izle baş rollerinde Gal Gadot, Robin Wright, Kristen Wiig, Chris Pine ve Connie Nielsen bulunmaktadır.", "Wonder Woman 1984", "https://resim.fullhdfilmizlesene.com/mdsresim_izle/the-unhealer-33773.jpg", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 6.4m, null, 1, 131m, 0, 0, null, 2020 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "Lookup");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
