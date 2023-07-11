
var insaList = [
    { id: 1, name: "Ev", img: "icon/ev.png", description: "Kendinize yeni bir ev inşa edin." },
    { id: 2, name: "Ahır", img: "/icon/ahır.png", description: "Büyükbaş hayvanlarınız için yeni bir ahır inşa edin." },
    { id: 3, name: "Ağıl", img: "/icon/ağıl.png", description: "Küçükbaş hayvanlarınız için yeni bir ağıl inşa edin." },
    { id: 4, name: "Kümes", img: "/icon/kumes.png", description: "Tavuk, Kaz, Ördek beslemek için kümes inşa edin." },
    { id: 5, name: "Kulübe", img: "/icon/kulübe.png", description: "Köpek beslemek için kulübe inşa edin." },
];

var uretimList = [
    { id: 1, name: "Buğday", img: "icon/bugday.jpg", description: "Buğday" },
    { id: 2, name: "Nohut", img: "icon/nohut.jpg", description: "Nohut" },
    { id: 3, name: "Mısır", img: "icon/misir.jpg", description: "Mısır" },
    { id: 4, name: "Arpa", img: "icon/arpa.jpg", description: "Arpa" },
    { id: 5, name: "Şeker Pancarı", img: "icon/sekerpancari.jpg", description: "Şeker Pancarı" },
    { id: 6, name: "Domates", img: "icon/domates.jpg", description: "Domates" },
    { id: 7, name: "Çilek", img: "icon/cilek.jpg", description: "Çilek" },
    { id: 8, name: "Kavun", img: "icon/kavun.jpg", description: "Kavun" },
    { id: 9, name: "Karpuz", img: "icon/karpuz.jpg", description: "Karpuz" },
];

var insaHandlers = {
    showInsaList: function () {
        var html = "<div class='insa-list'>";
        insaList.forEach(f => {
            html += "<div class='insa-item' data-id='" + f.id + "'>";
            html += "<img src='./img/" + f.img + "' alt='" + f.name + "'/>";
            html += "<p class='name'>" + f.name + "</p>";
            // html += "<p class='desc'>" + f.description + "</p>";
            html += "</div>";
        });
        html += "</div>";
        $(".eylem-alani").html(html);

        // game.sonIslem = {
        //     islem: islemTipi.InsaEt, callback: function () {
        //         //TODO
        //     }
        // };

        // helpers.showPopup(".popup-confirm", "İnşa Et", html);
    },
    showUretimList: function () {
        var araziId = $(this).data("arazi-id");
        var arazi = game.araziler.find(f => f.id == araziId);
        if (arazi && arazi.id) {
            var kullanilabilir = arazi.kullanilabilir || arazi.buyukluk;
            if (kullanilabilir && kullanilabilir > 100) {
                if (arazi.statu == araziStatu.Imarli) {
                    game.sonIslem = {
                        islem: islemTipi.TarlaSur, callback: function () {
                            var isValid = formHelper.validate(".uretim-form");
                            if (isValid) {
                                var ekimAlani = $(".uretim-form .form-field #ekimAlani").val();
                                if (ekimAlani > kullanilabilir) {
                                    helpers.showPopup(".popup-info", "Yetersiz Ekim Alanı", "Arazinizde ekime uygun kullanılabilir toplam " + kullanilabilir + " m2 alan bulunmaktadır!", "error");
                                    return;
                                }

                                game.sonIslem = {
                                    islem: islemTipi.TarlaSur, callback: function () {
                                        var traktorId = $(".farm-item.traktor.active").data("id");
                                        var traktor = kiralikTraktorList.find(f => f.id == traktorId);
                                        if (traktor) {
                                            var kacSaat = ekimAlani / traktor.verim;
                                            var ucret = kacSaat * traktor.kiralama + kacSaat * traktor.mazotTuketim * mazotFiyat;

                                            if (!gameHandlers.kontrolUcret(ucret)) {
                                                return;
                                            }

                                            game.para -= ucret;
                                            arazi.durum = araziDurum.Suruluyor;

                                            gameHandlers.ekleIslem(islemTipi.TarlaSur, { duration: kacSaat, araziId: arazi.id, ekimAlani: ekimAlani });
                                            gameHandlers.playStory(3);

                                            helpers.hidePopup(".popup-confirm");
                                            helpers.showPopup(".popup-info", "Tarla Sürülüyor", "İşlem tamalandığında size bildireceğiz..", "success");
                                        }
                                        else {
                                            helpers.showPopup(".popup-info", "İşlem Başarısız", "Lütfen kiralanacak aracı seçtikten sonra tekrar deneyiniz.", "error");
                                        }
                                    }
                                };

                                var traktorHtml = insaHandlers.getTraktorList(ekimAlani, islemTipi.TarlaSur);
                                helpers.showPopup(".popup-confirm", "Hizmet Seçin", traktorHtml);
                            }
                        }
                    };

                    var message = "<p class='mb20'>Arazinizde üretime başlamadan önce arazinizin sürülerek tarla hükmüne geçmesi gerekiyor.<br/>İşleme devam etmek istiyor musunuz?</p>";
                    var form = formHelper.build([
                        { type: "input", title: "Ekim Yapılacak Alan", maxLength: 10, name: "ekimAlani", validate: true },
                        { type: "label", title: "Kullanılabilir Alan : " + kullanilabilir + " m2", css: "text-success" },
                    ], "uretim-form");
                    message += form;

                    helpers.showPopup(".popup-confirm", "Sürülmesi Gerekiyor", message);
                }
                else if (arazi.statu == araziStatu.Imarsiz) {
                    helpers.showPopup(".popup-info", "İmarsız Arazi", "Araziniz imarsız statüsünde. Üretim yapabilmek için imarlı bir araziye ihtiyacınız var!", "error");
                }
                else if (arazi.statu == araziStatu.EkiliTarla) {
                    helpers.showPopup(".popup-info", "Ekili Arazi", "Araziniz ekilmiş durumda. Yeniden ekim yapmadan önce mevcut ürünü hasat etmelisiniz.", "error");
                }
                else if (arazi.statu == araziStatu.HasatSonrasiBos) {
                    //TODO
                }
                else if (arazi.statu == araziStatu.SurulmusTarla) {
                    insaHandlers.showUretimSecenek();
                }
            }
            else {
                helpers.showPopup(".popup-info", "Yetersiz Arazi", "Üretime başlamak için arazinizde minimum 100 m2 kullanılabilir alan olması gerekmektedir.", "error");
            }
        }
        else {
            helpers.showPopup(".popup-info", "İşlem Başarısız", "Arazi bilgilerine ulaşılamadı!", "error");
        }
    },
    showUretimSecenek: function () {
        var html = "<div class='insa-list'>";
        uretimList.forEach(f => {
            html += "<div class='insa-item' data-id='" + f.id + "'>";
            html += "<img src='./img/" + f.img + "' alt='" + f.name + "'/>";
            html += "<p class='name'>" + f.name + "</p>";
            html += "<p class='desc'>" + f.description + "</p>";
            html += "</div>";
        });
        html += "</div>";

        $(".eylem-alani").html(html);

        // game.sonIslem = {
        //     islem: islemTipi.InsaEt, callback: function () {
        //         //TODO
        //     }
        // };

        // helpers.showPopup(".popup-confirm", "Ürün Seç", html);
    },
    onTarlaSuruldu: function (araziId, ekimAlani) {
        var arazi = game.araziler.find(f => f.id == araziId);
        if (arazi) {
            arazi.durum = araziDurum.Suruldu;
            arazi.statu = araziStatu.SurulmusTarla;
            helpers.showPopup(".popup-info", "Tarlanız Sürüldü", "Artık ekime başlayabilirsiniz.", "success");
            gameHandlers.playStory(3);
        }
        else {
            helpers.showPopup(".popup-info", "Tarlanız Sürülemedi", "Teknik bir sorun sebebiyle işlem gerçekleştirilemedi!", "error");
        }
    },
    getTraktorList: function (buyukluk, islem) {
        var islemKatsayi = 1;
        if (islem == islemTipi.TemizleArazi) {
            islemKatsayi = 1.18;
        }

        var result = "<div class='farm-item-list traktor-list'>";
        result += kiralikTraktorList.map(m => {
            var item = "<div class='farm-item traktor' data-id='" + m.id + "'>";
            item += "<img src='./img/" + m.img + "'/>";
            item += "<p>" + m.adi + "</p>";
            item += "<p><i class='fas fa-oil-can'></i> " + m.mazotTuketim + " L/saat</p>";
            item += "<p><i class='fas fa-money-bill'></i> " + (m.kiralama * islemKatsayi).toFixed(2) + " TL/saat</p>";
            item += "<p><i class='fas fa-map'></i> " + m.verim + " m2/saat</p>";

            var kacSaat = buyukluk / m.verim;
            var ucret = kacSaat * m.kiralama * islemKatsayi + kacSaat * m.mazotTuketim * mazotFiyat;
            item += "<p class='text-error'>" + ucret.toFixed(2) + " TL</p>";
            item += "<div class='selection text-success'><i class='fas fa-check'></i><p class='text-error'>" + ucret.toFixed(2) + " TL</p></div>";
            item += "</div>"
            return item;
        }).join("");
        result += "</div>";
        return result;
    },
    setEvents: function () {
        $(document).on('click', '.insa-list .insa-item', {}, function () {
            $(".insa-list .insa-item").removeClass("active");
            $(this).addClass("active");
        });
    }
};