var basvuruDurum = {
    Iletildi: 1,
    Onaylandi: 2,
    Reddedildi: 3,
    Iptal: 4
};

var basvuruTipi = {
    Hibe: 1,
    BankaKredisi: 2,
};

var krediSureList = [
    { value: undefined, name: "Seçiniz" },
    { value: 3, name: "3 Ay" },
    { value: 6, name: "6 Ay" },
    { value: 9, name: "9 Ay" },
    { value: 12, name: "12 Ay" },
    { value: 18, name: "18 Ay" },
    { value: 24, name: "24 Ay" },
    { value: 30, name: "30 Ay" },
    { value: 36, name: "36 Ay" },
    { value: 42, name: "42 Ay" },
    { value: 48, name: "48 Ay" },
    { value: 54, name: "54 Ay" },
    { value: 60, name: "60 Ay" },
];

var aylikFaiz = 1.09;

var basvuruHandlers = {
    showBasvrularim: function () {
        var actions = "<div class='actions'>";
        actions += "<button class='mr10 mt20 btn-basvur-kredi'>Krediye Başvur</button>";
        actions += "<button class='mt20 btn-basvur-hibe'>Hibeye Başvur</button>";
        actions += "</div>";

        var html = "";
        if (game.basvurular.length > 0) {
            html = actions;
            html += "<div class='farm-item-list basvuru-list'>";

            var durumMap = [
                { id: basvuruDurum.Iletildi, name: "İLETİLDİ", icon: "warning" },
                { id: basvuruDurum.Onaylandi, name: "ONAYLANDI", icon: "success" },
                { id: basvuruDurum.Reddedildi, name: "REDDEDİLDİ", icon: "error" },
                { id: basvuruDurum.Iptal, name: "İPTAL", icon: "error" },
            ];

            html += game.basvurular.map(m => {
                var tipiStr = m.tipi == basvuruTipi.Hibe ? "HİBE" : "KREDİ";
                var item = "<div class='farm-item basvuru'>";
                item += "<img src='./img/apply.jpg'/>";
                item += "<p>" + tipiStr + " #" + m.id + "</p>";

                if (m.krediOdeme) {
                    item += "<p>Kredi Tutarı : " + (m.krediOdeme.krediTutari.toFixed(2)) + " TL</p>";
                    item += "<p>Aylık Ödeme : " + (m.krediOdeme.aylik.toFixed(2)) + " TL</p>";
                    // item += "<p>Faiz Oranı : %"+(m.krediOdeme.aylikFaiz)+"</p>";
                    // item += "<p>Toplam Geri Ödeme : "+(m.krediOdeme.toplam.toFixed(2))+" TL</p>";
                    item += "<p><i class='far fa-calendar'></i> " + helpers.dateToString(m.tarih, "d.m.y H:M") + " </p>";
                }
                else {
                    item += "<p>Başvuru Tarihi : <br/><i class='far fa-calendar'></i> " + helpers.dateToString(m.tarih, "d.m.y H:M") + " </p>";
                }

                var durum = durumMap.find(f => f.id == m.durum);
                item += "<p class='etiket etiket-" + durum.icon + "'>" + durum.name + "</p>";
                if (m.durum == basvuruDurum.Iletildi) {
                    item += "<button class='btn-text btn-iptal-basvuru' data-id='" + m.id + "'>İptal Et</button>";
                }

                item += "</div>";
                return item;
            }).join("");
            html += "</div>";
        }
        else {
            html += "<p>Henüz bir başvurunuz yok..</p>";
            html += actions;
        }

        $(".game .header ul.menu li").removeClass("active");
        $(".game .header ul.menu li.basvurularim").addClass("active");

        helpers.hidePopup();
        $(".story-1").addClass("dn");
        $(".page-content").html(html).removeClass("dn");
    },
    iptalBasvuru: function () {
        var basvuruId = $(this).data("id");
        game.sonIslem = {
            islem: 3, code: "hibe", callback: function () {
                var basvuru = game.basvurular.find(f => f.id == basvuruId);
                if (basvuru) {
                    basvuru.durum = basvuruDurum.Iptal;
                    gameHandlers.save();
                    gameHandlers.arrangeScreen("basvuru");

                    helpers.hidePopup(".popup-confirm");
                    helpers.showPopup(".popup-info", "Başvurunuz İptal Edildi", "Başvurunuz başarıyla iptal edildi.", "success");
                }
                else {
                    helpers.hidePopup(".popup-confirm");
                    helpers.showPopup(".popup-info", "İşlem Başarısız", "Başvuru detayları alınamadı.", "error");
                }
            }
        };

        var message = "Seçtiğiniz başvuru iptal edilecektir. Devam etmek istiyor musunuz?";
        helpers.showPopup(".popup-confirm", "Başvuru İptali", message);
    },
    basvurHibe: function () {
        var devamEden = game.basvurular.find(f => f.tipi == basvuruTipi.Hibe && f.durum == basvuruDurum.Iletildi);
        if (devamEden) {
            helpers.showPopup(".popup-info", "Devam Eden Başvuru Var", "Halen süreci devam edem #" + devamEden.id + " nolu başvurunuz bulunmaktadır!", "error");
            return;
        }

        var kontrolDate = helpers.getDate(game.time, -1, 0, 0, 0, 0, 0);
        var sonBirYilOlumluBasvurular = game.basvurular.filter(f => f.tipi == basvuruTipi.Hibe && f.durum == basvuruDurum.Onaylandi && kontrolDate < new Date(f.tarih));
        if (sonBirYilOlumluBasvurular && sonBirYilOlumluBasvurular.length > 0) {
            helpers.showPopup(".popup-info", "Onaylı Başvuru Var", "Son bir yıl içinde yaptığınız ve olumlu sonuçlanan başvuru var. Üzerinden 1 yıl geçmeden yeni başvuru yapamazsınız!", "error");
            return;
        }

        game.sonIslem = {
            islem: islemTipi.Basvuru, callback: function () {
                var basvuruNo = basvuruHandlers.getBasvuruNo();
                game.basvurular = game.basvurular || [];
                game.basvurular.push({ id: basvuruNo, tipi: basvuruTipi.Hibe, tarih: new Date(game.time), durum: basvuruDurum.Iletildi });
                gameHandlers.save();

                gameHandlers.ekleIslem(islemTipi.BasvuruHibe, { basvuruNo });
                gameHandlers.gosterIslemler();
                gameHandlers.arrangeScreen("basvuru");

                helpers.hidePopup(".popup-confirm");
                helpers.showPopup(".popup-info", "Başvurunuz Alındı", "Hibe başvurunuz alındı. Başvuru numaranız: #" + basvuruNo, "success");
            }
        };
        var message = "Hibe için başvuru yapılacaktır. Devam etmek istiyor musunuz?";
        helpers.showPopup(".popup-confirm", "Hibeye Başvur", message);
        return;
    },
    basvurKredi: function () {
        var devamEden = game.basvurular.find(f => f.tipi == basvuruTipi.BankaKredisi && f.durum == basvuruDurum.Iletildi);
        if (devamEden) {
            helpers.showPopup(".popup-info", "Devam Eden Başvuru Var", "Halen süreci devam edem #" + devamEden.id + " nolu kredi başvurunuz bulunmaktadır!", "error");
            return;
        }

        var kontrolDate = helpers.getDate(game.time, -1, 0, 0, 0, 0, 0);
        var sonBirYilOlumluBasvurular = game.basvurular.filter(f => f.tipi == basvuruTipi.BankaKredisi && f.durum == basvuruDurum.Onaylandi && kontrolDate < new Date(f.tarih));
        if (sonBirYilOlumluBasvurular && sonBirYilOlumluBasvurular.length > 0) {
            helpers.showPopup(".popup-info", "Onaylı Başvuru Var", "Son bir yıl içinde yaptığınız ve olumlu sonuçlanan başvurunuz var. Üzerinden 1 yıl geçmeden yeni kredi başvurusu yapamazsınız!", "error");
            return;
        }

        //Kredi puanı kontrolü
        if (game.krediPuani < 600) {
            helpers.showPopup(".popup-info", "Kredi Puanı Yetersiz", "Kredi puanınız yeterli olmadığı için kredi başvurusunda bulunamazsınız.", "error");
            return;
        }

        //TODO
        //odeyebilme potansiyeli - sure - tutar kontrolü yapilabilir.

        game.sonIslem = {
            islem: islemTipi.BasvuruKredi, callback: function () {
                //Aylik gelir kontrolu
                var krediOdeme = basvuruHandlers.hesaplaGeriOdeme();
                if (krediOdeme.aylik > game.gelir * 0.7) {
                    helpers.showPopup(".popup-info", "Aylık Gelir Yetersiz", "Talep ettiğiniz kredi miktarı aylık gelirinizin %70'inden fazla olamaz!", "error");
                    return;
                }

                var basvuruNo = basvuruHandlers.getBasvuruNo();
                game.basvurular = game.basvurular || [];
                game.basvurular.push({ id: basvuruNo, tipi: basvuruTipi.BankaKredisi, tarih: new Date(game.time), krediOdeme: krediOdeme, durum: basvuruDurum.Iletildi });
                gameHandlers.save();

                gameHandlers.ekleIslem(islemTipi.BasvuruKredi, { basvuruNo });
                gameHandlers.gosterIslemler();
                gameHandlers.arrangeScreen("basvuru");

                helpers.hidePopup(".popup-confirm");
                helpers.showPopup(".popup-info", "Başvurunuz Alındı", "Kredi başvurunuz alındı. Başvuru numaranız: #" + basvuruNo, "success");
            }
        };

        var basvuruFormu = formHelper.build([
            { type: "input", title: "Kredi Tutarı", maxLength: 20, name: "krediTutari" },
            { type: "select", title: "Süre", name: "krediSure", list: krediSureList },
            { type: "label", title: "Aylık Faiz Oranı : %" + aylikFaiz },
        ], "kredi-basvuru-form");

        basvuruFormu += "<div class='form-field'><span class='text-error text-bold'>Toplam Geri Ödeme : <span class='odeme-tutar'>0</span> TL</span></div>";
        helpers.showPopup(".popup-confirm", "Krediye Başvur", basvuruFormu);
        return;
    },
    getBasvuruNo: function () {
        var basvuruNo = 1000;
        if (game.basvurular.length > 0) {
            var last = (game.basvurular.map(m => m.id).sort((a, b) => b - a))[0];
            basvuruNo = parseInt(last) + 1;
        }
        return basvuruNo;
    },
    basvuruTamamla: function (basvuruNo, hibeMi) {
        var basvuru = game.basvurular.find(f => f.id == basvuruNo);
        if (basvuru) {
            if(basvuru.tipi == basvuruTipi.BankaKredisi){
                basvuru.durum = basvuruDurum.Onaylandi;
                game.para += basvuru.krediOdeme.krediTutari;
                helpers.guncellePara(game.para, true);
                helpers.showPopup(".popup-info", "Başvurunuz Onaylandı", "#" + el.id + " nolu hibe başvurunuz onaylandı ve hesabınıza " + basvuru.krediOdeme.krediTutari + " TL transfer edildi.", "success");
            }
            else{
                basvuru.durum = basvuruDurum.Onaylandi;
                var hibeTutari = basvuruHandlers.hibeTutarBelirle();
                game.para += hibeTutari;
                helpers.guncellePara(game.para, true);
                helpers.showPopup(".popup-info", "Başvurunuz Onaylandı", "#" + el.id + " nolu hibe başvurunuz onaylandı ve hesabınıza " + hibeTutari + " TL transfer edildi.", "success");
            }
        }
    },
    hibeTutarBelirle: function () {
        var para = Math.random() * 10000;
        if (para < 1000) {
            para += 618;
        }

        if (para > 5000) {
            para /= 1.618;
        }
        return para;
    },
    hesaplaGeriOdeme: function () {
        var krediTutari = parseFloat($(".kredi-basvuru-form input[name='krediTutari']").val());
        var sure = parseFloat($(".kredi-basvuru-form select[name='krediSure']").val());

        var hesaplama = krediTutari + (krediTutari * aylikFaiz * sure) / 24;
        $(".kredi-basvuru-form").parent().find(".odeme-tutar").html(hesaplama.toFixed(2));
        return { toplam: hesaplama, aylik: hesaplama / sure, krediTutari, sure, aylikFaiz: aylikFaiz };
    }
};