using Aware.Util.Enum;
using Cinescope.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cinescope.Web.Data
{
    public class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>().HasData(
               new Film
               {
                   ID = 1,
                   Adi = "Jumanji 3 Yeni Seviye",
                   ImdbPuani = 6.9M,
                   Status = StatusType.Active,
                   Afis = "https://resim.fullhdfilmizlesene.com/mdsresim_izle/fullhd-jumanji-filmi-izlesene.jpg?0.4029940593148297",
                   Sure = 123M,
                   YapimYili = 2019,
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
               },
               new Film
               {
                   ID = 2,
                   Adi = "The Unhealer",
                   ImdbPuani = 4.5M,
                   Status = StatusType.Active,
                   Afis = "https://resim.fullhdfilmizlesene.com/mdsresim_izle/the-unhealer-33773.jpg",
                   Sure = 94M,
                   YapimYili = 2020,
                   Aciklama = @"2020 Amerikan yapım bilim kurgu ve gerilim filmi olan The Unhealar’ın yazarları Kevin E Moore ve J. Shawn Harris’dir. Yönetmen koltuğunda ise Martin Guigui oturuyor. Başlıca oyuncular, Elijah Nelson, Shelby Janes, Kayla Carlson ve Lance Henriksen’dir. Kelly adında lise öğrencisi, elinde olmayan bir hastalığı dolayısıyla, okulda sürekli zorbalığa uğrar. Kendisinin çöp yeme hastalığı vardır ve bu onu çok hasta yapar. Bu çocuktan hoşlanan Dominique adında kız sürekli onu kollar, diğer çocukların ona yanaşmasına izin vermez.",
               },
               new Film
               {
                   ID = 3,
                   Adi = "Wonder Woman 1984",
                   ImdbPuani = 6.4M,
                   Status = StatusType.Active,
                   Afis = "https://tr.web.img4.acsta.net/pictures/16/11/03/16/33/532910.jpg",
                   Sure = 131M,
                   YapimYili = 2020,
                   Aciklama = @"DC evreninin kahramanı olan Wonder Woman’ı anlatan film Wonder Woman 1984’ün yönetmenliğini Patty Jenkins yapmıştır. Filmin senaryosu Geoff Johns, Dave Callaham, ve Patty Jenkins tarafından William Moulton Marston’un yarattığı çizgi romandan esinlenerek kaleme alınmıştır. Film ülkemizde 15 Ocak 2021’de vizyona girecektir. 4k film izle baş rollerinde Gal Gadot, Robin Wright, Kristen Wiig, Chris Pine ve Connie Nielsen bulunmaktadır.",
               });


            modelBuilder.Entity<Player>().HasData(
                    new Player { ID = 1, Name = "Kadir İnanır", BirthDate = new DateTime(1949, 04, 15), BirthPlace = "Ordu", Status = StatusType.Active, Photo = "images/player/kadir_inanir.jpg", ShortDesc = "Fatsa doğumlu olan Kadir İnanır, ailesinin son çocuğudur. Fatsa'daki ilkokul ve ortaokul eğitimi sırasında sahne yeteneğini çeşitli okul gösterilerinde sergiledi. İnanır, yatılı olarak okuduğu İstanbul Haydarpaşa Lisesi'nin ardından Marmara Üniversitesi İletişim Fakültesi, Radyo-Televizyon Bölümü’nü bitirdi." },
                    new Player { ID = 2, Name = "Serpil Çakmaklı", BirthDate = new DateTime(1962, 05, 19), BirthPlace = "Eskişehir", Status = StatusType.Active, Photo = "images/player/serpil_cakmakli.jpg", ShortDesc = "Serpil Çakmaklı ya da gerçek adıyla Şener Dönmez, Türk sinema oyuncusudur. Eskişehir'de doğdu. Sinemadan önce mankenlik yaptı. 1980'li yılların başında TRT televizyonunda yayınlanan Alçaktan Uçan Güvercin adlı mini dizi filmle üne kavuştu. Birçok sinema filminde rol aldı. İlki 16 yaşında olmak üzere üç evlilik yaptı." }
            );

            modelBuilder.Entity<PlayerFilm>().HasData(
                   new PlayerFilm { ID = 1, PlayerId = 1, FilmId = 6, Status = StatusType.Active },
                   new PlayerFilm { ID = 2, PlayerId = 2, FilmId = 6, Status = StatusType.Active }
           );
        }
    }
}
