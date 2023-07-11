﻿// <auto-generated />
using System;
using Cinescope.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cinescope.Web.Migrations
{
    [DbContext(typeof(CinescopeDbContext))]
    [Migration("20210924214527_film-player-4")]
    partial class filmplayer4
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Aware.Model.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CompanyID");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Email");

                    b.Property<int>("Gender");

                    b.Property<DateTime>("LastVisit");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("PhoneNumber");

                    b.Property<int>("Role");

                    b.Property<int>("Status");

                    b.Property<int>("UserCreated");

                    b.Property<int>("UserModified");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Aware.Util.Lookup.Lookup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("ExtraData");

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<int>("UserCreated");

                    b.Property<int>("UserModified");

                    b.Property<string>("Value");

                    b.HasKey("ID");

                    b.ToTable("Lookup");
                });

            modelBuilder.Entity("Cinescope.Web.Models.Film", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Aciklama");

                    b.Property<string>("Adi");

                    b.Property<string>("Afis");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("FragmanAdresi");

                    b.Property<decimal>("ImdbPuani");

                    b.Property<string>("IzlemeAdresi");

                    b.Property<int>("Status");

                    b.Property<decimal>("Sure");

                    b.Property<int>("UserCreated");

                    b.Property<int>("UserModified");

                    b.Property<string>("YapimSirketi");

                    b.Property<int>("YapimYili");

                    b.HasKey("ID");

                    b.ToTable("Films");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Aciklama = @"Next Level, Jake Kasdan tarafından  yönetilen, Jeff Pinkner, Scott Rosenberg ve Jake Kasdan tarafından yazılmış bir Amerikan fantastik macera komedi filmidir. 
2017'deki Jumanji'nin ardından 1995'in Jumanji'sinin ikinci devamı: Ormana Hoş Geldiniz ve genel olarak Jumanji serisinin üçüncü bölümüdür. 
Filmde Dwayne Johnson, Kevin Hart, Jack Black, Karen Gillan, Nick Jonas, Alex Wolff, Morgan Turner, Ser'Darius Blain ve Madison Iseman ve Danny 
DeVito gibi çok ünlü ve yetenekli oyuncular yer almaktadırlar.  Amerika Birleşik Devletleri'nde 13 Aralık 2019 tarihinde Columbia Pictures etiketi
altında Sony Pictures Releasing tarafından piyasaya sürülmesi planlanmaktadır. Spencer, Jumanji video oyununun parçalarını saklamıştır ve bir gün
dedesinin evinin bodrumundaki sistemi tamir ettirir. Spencer'ın arkadaşları Bethany, Fridge ve Martha geldiğinde, Spencer'ı bulamazlar ve oyun çalışır durumdadır.
Onu kurtarmak için Jumanji'ye tekrar girmeye karar verirler. Spencer'ın dedesi Eddie ve arkadaşı Milo Walker bu kargaşayı duyar ve Spencer'in arkadaşları daha
avatarlarını seçemeden  oyunun içine çekilirler. Birinin bu maceradan çıkamayacağını iddia eden Nigel Billingsley tarafından onlara sunulan yeni bir görevle,
genç arkadaşlar Eddie ve Milo'nun oyundaki avatarlarına alışmalarına ve, Spencer'ı bulmalarına yardımcı olmak için son kez Jumanji'den kaçmaya çalışırlar.
Jumanji: The Next Level (2019)",
                            Adi = "Jumanji 3 Yeni Seviye",
                            Afis = "https://resim.fullhdfilmizlesene.com/mdsresim_izle/fullhd-jumanji-filmi-izlesene.jpg?0.4029940593148297",
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ImdbPuani = 6.9m,
                            Status = 1,
                            Sure = 123m,
                            UserCreated = 0,
                            UserModified = 0,
                            YapimYili = 2019
                        },
                        new
                        {
                            ID = 2,
                            Aciklama = "2020 Amerikan yapım bilim kurgu ve gerilim filmi olan The Unhealar’ın yazarları Kevin E Moore ve J. Shawn Harris’dir. Yönetmen koltuğunda ise Martin Guigui oturuyor. Başlıca oyuncular, Elijah Nelson, Shelby Janes, Kayla Carlson ve Lance Henriksen’dir. Kelly adında lise öğrencisi, elinde olmayan bir hastalığı dolayısıyla, okulda sürekli zorbalığa uğrar. Kendisinin çöp yeme hastalığı vardır ve bu onu çok hasta yapar. Bu çocuktan hoşlanan Dominique adında kız sürekli onu kollar, diğer çocukların ona yanaşmasına izin vermez.",
                            Adi = "The Unhealer",
                            Afis = "https://resim.fullhdfilmizlesene.com/mdsresim_izle/the-unhealer-33773.jpg",
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ImdbPuani = 4.5m,
                            Status = 1,
                            Sure = 94m,
                            UserCreated = 0,
                            UserModified = 0,
                            YapimYili = 2020
                        },
                        new
                        {
                            ID = 3,
                            Aciklama = "DC evreninin kahramanı olan Wonder Woman’ı anlatan film Wonder Woman 1984’ün yönetmenliğini Patty Jenkins yapmıştır. Filmin senaryosu Geoff Johns, Dave Callaham, ve Patty Jenkins tarafından William Moulton Marston’un yarattığı çizgi romandan esinlenerek kaleme alınmıştır. Film ülkemizde 15 Ocak 2021’de vizyona girecektir. 4k film izle baş rollerinde Gal Gadot, Robin Wright, Kristen Wiig, Chris Pine ve Connie Nielsen bulunmaktadır.",
                            Adi = "Wonder Woman 1984",
                            Afis = "https://tr.web.img4.acsta.net/pictures/16/11/03/16/33/532910.jpg",
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            ImdbPuani = 6.4m,
                            Status = 1,
                            Sure = 131m,
                            UserCreated = 0,
                            UserModified = 0,
                            YapimYili = 2020
                        });
                });

            modelBuilder.Entity("Cinescope.Web.Models.Player", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("BirthPlace");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<DateTime?>("DeathDate");

                    b.Property<string>("Name");

                    b.Property<string>("Photo");

                    b.Property<string>("ShortDesc");

                    b.Property<int>("Status");

                    b.Property<int>("UserCreated");

                    b.Property<int>("UserModified");

                    b.HasKey("ID");

                    b.ToTable("Players");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            BirthDate = new DateTime(1949, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BirthPlace = "Ordu",
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Kadir İnanır",
                            Photo = "images/player/kadir_inanir.jpg",
                            ShortDesc = "Fatsa doğumlu olan Kadir İnanır, ailesinin son çocuğudur. Fatsa'daki ilkokul ve ortaokul eğitimi sırasında sahne yeteneğini çeşitli okul gösterilerinde sergiledi. İnanır, yatılı olarak okuduğu İstanbul Haydarpaşa Lisesi'nin ardından Marmara Üniversitesi İletişim Fakültesi, Radyo-Televizyon Bölümü’nü bitirdi.",
                            Status = 1,
                            UserCreated = 0,
                            UserModified = 0
                        },
                        new
                        {
                            ID = 2,
                            BirthDate = new DateTime(1962, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            BirthPlace = "Eskişehir",
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Serpil Çakmaklı",
                            Photo = "images/player/serpil_cakmakli.jpg",
                            ShortDesc = "Serpil Çakmaklı ya da gerçek adıyla Şener Dönmez, Türk sinema oyuncusudur. Eskişehir'de doğdu. Sinemadan önce mankenlik yaptı. 1980'li yılların başında TRT televizyonunda yayınlanan Alçaktan Uçan Güvercin adlı mini dizi filmle üne kavuştu. Birçok sinema filminde rol aldı. İlki 16 yaşında olmak üzere üç evlilik yaptı.",
                            Status = 1,
                            UserCreated = 0,
                            UserModified = 0
                        });
                });

            modelBuilder.Entity("Cinescope.Web.Models.PlayerFilm", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateModified");

                    b.Property<int>("FilmId");

                    b.Property<int>("PlayerId");

                    b.Property<int>("Status");

                    b.Property<int>("UserCreated");

                    b.Property<int>("UserModified");

                    b.HasKey("ID");

                    b.ToTable("PlayerFilms");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            FilmId = 6,
                            PlayerId = 1,
                            Status = 1,
                            UserCreated = 0,
                            UserModified = 0
                        },
                        new
                        {
                            ID = 2,
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateModified = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            FilmId = 6,
                            PlayerId = 2,
                            Status = 1,
                            UserCreated = 0,
                            UserModified = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
