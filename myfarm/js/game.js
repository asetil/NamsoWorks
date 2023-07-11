var islemTipi = {
    BasvuruHibe: 1,
    BasvuruKredi: 2,
    SatinAlArazi: 3,
    TemizleArazi: 4,
    InsaEt: 5,
    TarlaSur: 6,
    OyunuSifirla: 7
};

var game = {
    name: '',
    para: 0,
    gelir: 2000,
    krediPuani: 1000,
    basvurular: [],
    araziler: [],
    islemList: [],
    duyuruList: [],
    story: 0,
    time: new Date(),
};

var gameTimer = undefined;

var gameHandlers = {
    init: function () {
        var saved = localStorage.getItem("myFarm_game");
        if (saved) {
            game = JSON.parse(saved);
        }

        gameHandlers.playStory(game.story);
        gameTimer = setInterval(gameHandlers.setTime, 1000);
    },
    playStory: function (story) {
        if (story == 0) {
            gameHandlers.ekleDuyuru("Oyuna hoşgeldiniz.", 24 * 60);
            helpers.arrangeContent(".story-1");
        }
        else if (story == 1) {
            game.story = 1;
            var para = Math.random() * 10000;
            if (para < 1000) {
                para += 618;
            }

            if (para > 5000) {
                para /= 1.618;
            }
            helpers.guncellePara(para, true);

            var kayitFormu = formHelper.build([
                { type: "input", title: "Adınız", maxLength: 30, name: "adi", validate: true },
                { type: "label", title: "<span class='text-info text-bold'>Başlangıç ücreti olarak <span class='odeme-tutar'>" + (Math.round(para * 100) / 100) + "</span> TL hesabınıza yatırılacaktır.</span>" },
                { type: "button", title: "Kaydet", css: 'btn-register' },
            ], "kayit-form white-panel");

            $(".page-content").html(kayitFormu).removeClass("dn");
        }
        else if (story == 2) {
            var valid = formHelper.validate('.kayit-form');
            if (valid) {
                var adi = $(".kayit-form input[name='adi']").val();
                game.name = adi;
                game.story = 3;
                gameHandlers.save();
                gameHandlers.playStory(3);
            }
            return valid;
        }
        else if (story == 3) {
            if (game.araziler.length > 0) {
                var arazi = game.araziler[0];
                if (arazi && arazi.ilce) {
                    var html = $(".template-arazi").html();
                    html = html.replace("#KONUM#", arazi.ilce + " / " + arazi.il);
                    html = html.replace("#STATU#", araziHandlers.getStatuDesc(arazi.statu));
                    html = html.replace("#ARAZI_ID#", arazi.id);
                    html = html.replace("#M2#", arazi.buyukluk + " m2");
                    $(".page-content").html(html).removeClass("dn");

                    if (arazi.durum == araziDurum.Temizleniyor) {
                        $(".imarsiz-arazi .actions").html("<p>Araziniz temizleniyor..</p>");
                    } else if (
                        arazi.durum == araziDurum.Temizlendi ||
                        arazi.durum == araziDurum.Suruldu
                    ) {
                        // var araziSecenek = "<p>Araziniz temizlendi</p>";
                        // araziSecenek += "<button class='btn-text btn-insa-et' data-arazi-id='" + arazi.id + "'>İnşa et</button>";
                        // araziSecenek += "<button class='btn-text btn-uret' data-arazi-id='" + arazi.id + "'>Üret</button>";
                        $(".imarsiz-arazi .actions").addClass("dn");
                        insaHandlers.showInsaList();
                    } else if (arazi.durum == araziDurum.Suruluyor) {
                        $(".imarsiz-arazi .actions").html("<p>Araziniz sürülüyor..</p>");
                    }
                }
            }
            else {
                araziHandlers.goruntuleAraziler(undefined, ".story-3 .arazi-list");
                helpers.arrangeContent(".story-3");
            }

            $(".game .header ul.menu li").removeClass("active");
            $(".game .header ul.menu li.home").addClass("active");
            $(".game .header span.player-name").html(game.name);
            $(".game .header").removeClass("dn");
            $(".game .header .menu").removeClass("dn");
            $(".game .news-band").removeClass("dn");

            helpers.guncellePara(game.para, false);
            gameHandlers.gosterIslemler();
            gameHandlers.showDuyurular();
        }
    },
    setTime: function () {
        game.time = helpers.getDate(game.time ? new Date(game.time) : new Date(), 0, 0, 0, 0, 1, 0);
        var dateInfo = helpers.dateToString(game.time, "d A y H:M");
        $(".news-band .date-info").html(dateInfo);

        gameHandlers.yonetIslem();
    },
    clear: function () {
        if (gameTimer) {
            clearInterval(gameTimer);
        }
    },
    save: function () {
        localStorage.setItem("myFarm_game", JSON.stringify(game));
    },
    reset: function () {
        game = {
            name: '',
            para: 0,
            gelir: 2000,
            krediPuani: 1000,
            basvurular: [],
            araziler: [],
            islemList: [],
            duyuruList: [],
            story: 0,
            time: new Date(),
        };

        localStorage.setItem("myFarm_game", JSON.stringify(game));
        gameHandlers.clear();
        gameHandlers.init();
        window.location.reload();
    },
    arrangeScreen: function (screen) {
        if (screen == "arazi") {
            araziHandlers.showArazilerim();
        } else if (screen == "basvuru") {
            basvuruHandlers.showBasvrularim();
        }
    },
    gosterIslemler: function () {
        if (game.islemList && game.islemList.length > 0) {
            var html = "";
            html += game.islemList
                .map((m) => {
                    var item = "<div class='farm-item islem'>";
                    if (m.img) item += "<img src='./img/" + m.img + "'/>";
                    item +=
                        "<p class='text-bold'>" + gameHandlers.getIslemAdi(m.tipi) + "</p>";
                    item += "<p>" + helpers.dateToString(m.tarih, "d.m.y H:M") + "</p>";

                    var completion = gameHandlers.getIslemProgress(m);
                    item +=
                        "<div class='progress-bar'><span style='width:" +
                        completion +
                        "%;'>&nbsp;</span></div>";
                    item +=
                        "<div class='btn-islem-iptal' data-id='" +
                        m.uid +
                        "'><i class='fas fa-times'></i></div>";
                    item += "</div>";
                    return item;
                })
                .join("");

            $(".islem-list").html(html);
        } else {
            $(".islem-list").html("");
        }
    },
    ekleIslem: function (islem, data) {
        var duration = 0;
        switch (islem) {
            case islemTipi.BasvuruHibe:
                duration = 24 * (20 + Math.ceil(Math.random() * 12));
                break;
            case islemTipi.BasvuruKredi:
                duration = 24 * (7 + Math.ceil(Math.random() * 7));
                break;
            case islemTipi.SatinAlArazi:
                duration = 6 + Math.ceil(Math.random() * 18);
                break;
            case islemTipi.TemizleArazi:
                duration = data.duration || 3;
                break;
            case islemTipi.TarlaSur:
                duration = data.duration || 3;
                break;
        }

        duration = duration / 10;

        game.islemList = game.islemList || [];
        game.islemList.push({
            uid: helpers.getUid(),
            tipi: islem,
            tarih: new Date(game.time),
            duration: duration,
            data: { ...data },
        });
        gameHandlers.save();
    },
    iptalIslem: function () {
        var islemId = $(this).data("id");
        game.sonIslem = {
            islem: 3,
            code: "islem-iptal",
            callback: function () {
                var islem = game.islemList.find((f) => f.uid == islemId);
                if (islem) {
                    if (
                        islem.tipi == islemTipi.BasvuruHibe ||
                        islem.tipi == islemTipi.BasvuruKredi
                    ) {
                        var basvuru = game.basvurular.find((f) => f.id == islem.basvuruNo);
                        if (basvuru) {
                            basvuru.durum = basvuruDurum.Iptal;
                            gameHandlers.save();
                        }
                    }

                    game.islemList = game.islemList.filter((f) => f.uid != islemId);
                    gameHandlers.save();

                    helpers.hidePopup(".popup-confirm");
                    helpers.showPopup(
                        ".popup-info",
                        "İşlem İptal Edildi",
                        "İşlem başarıyla iptal edildi.",
                        "success"
                    );
                } else {
                    helpers.hidePopup(".popup-confirm");
                    helpers.showPopup(
                        ".popup-info",
                        "Hata",
                        "İşlem bilgilerine ulaşılamadı. İşlem iptali başarısız!",
                        "error"
                    );
                }
            },
        };
        helpers.showPopup(
            ".popup-confirm",
            "İşlem İptali",
            "İşlem iptal edilecektir. Devam etmek istiyor musunuz?"
        );
    },
    yonetIslem: function () {
        var now = game.time;
        var islemList = [...game.islemList];
        for (const islem of islemList) {
            var finish = helpers.getDate(islem.tarih, 0, 0, 0, islem.duration, 0, 0);
            if (finish < now) {
                if (
                    islem.tipi == islemTipi.BasvuruHibe ||
                    islem.tipi == islemTipi.BasvuruKredi
                ) {
                    basvuruHandlers.basvuruTamamla(islem.data.basvuruNo);
                } else if (islem.tipi == islemTipi.SatinAlArazi) {
                    araziHandlers.satisTamamla(islem.data);
                } else if (islem.tipi == islemTipi.TemizleArazi) {
                    araziHandlers.temizlikTamamla(islem.data.araziId);
                } else if (islem.tipi == islemTipi.TarlaSur) {
                    insaHandlers.onTarlaSuruldu(islem.data.araziId, islem.data.ekimAlani);
                }

                game.islemList = game.islemList.filter((f) => f.uid != islem.uid);
                gameHandlers.save();
            }
        }

        gameHandlers.gosterIslemler();
    },
    getIslemAdi: function (value) {
        switch (value) {
            case islemTipi.BasvuruHibe:
                return "Hibe Başvurusu";
            case islemTipi.BasvuruKredi:
                return "Kredi Başvurusu";
            case islemTipi.SatinAlArazi:
                return "Arazi ";
            case islemTipi.TemizleArazi:
                return "Arazi Temizleme";
        }
        return value;
    },
    getIslemProgress: function (islem) {
        var all = islem.duration;
        var completed =
            helpers.dateDifference(new Date(islem.tarih), new Date(game.time)) / 60;
        if (completed > all) {
            completed = all;
        }
        return (100 * completed) / all;
    },
    ekleDuyuru: function (icerik, sure) {
        game.duyuruList = game.duyuruList || [];
        game.duyuruList.push({
            duyuru: icerik,
            bitis: helpers.getDate(0, 0, 0, 0, 0, sure, 0),
        });
    },
    showDuyurular: function () {
        if (game.duyuruList && game.duyuruList.length > 0) {
            var html = "";
            var now = game.time;
            game.duyuruList = game.duyuruList.filter((f) =>
                helpers.checkDate(f.bitis, now)
            );
            html += game.duyuruList.map((m) => {
                var item = "<span><i class='fas fa-info-circle'></i> " + m.duyuru + "</span>";
                return item;
            }).join("");

            $(".news-band .right marquee").html(html);
        } else {
            $(".news-band .right marquee").html("");
        }
    },
    kontrolUcret: function (ucret) {
        if (game.para < ucret) {
            var message = "</p>Bu işlem için yeterli bakiyeniz bulunmamaktadır.<br/><b class='btn-link btn-basvurularim'>Başvurularım</b> alanından banka kredisi yada hibe başvurusunda bulunabilirsiniz.</p>";
            helpers.showMessage("Yetersiz Bakiye", message, "error");
            return false;
        }
        return true;
    },
    showBargainPanel: function () {
        var selector = $(this).parent();
        var panel = formHelper.build([
            { type: "input", title: "Teklifiniz", maxLength: 20, name: "teklif" },
        ], "pazarlik-formu");

        $(selector).append(panel);
    },
    registerEvents: function () {
        $(document).on("click", ".btn-start", {}, () => { return gameHandlers.playStory(1) });
        $(document).on("click", ".btn-register", {}, () => { return gameHandlers.playStory(2) });
        $(document).on("click", ".btn-home", {}, gameHandlers.init);
        $(document).on("click", ".btn-reset", {}, () => {
            helpers.showConfirm(islemTipi.OyunuSifirla, "Oyunu sıfırlamak istediğinizden emin misiniz?", "Mevcut ilerlemeniz silinecek ve oyuna en başından başlayacaksınız!", "warning");
        });
        $(document).on("click", ".btn-islem-iptal", {}, gameHandlers.iptalIslem);
        $(document).on("click", ".btn-bargain", {}, gameHandlers.showBargainPanel);
    }
};
