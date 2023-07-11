var araziDurum = {
  SatisaAcik: 1,
  Opsiyonlu: 2,
  Satildi: 3,
  Temizleniyor: 4,
  Temizlendi: 5,
  Suruluyor: 6,
  Suruldu: 7,
};

var araziStatu = {
  Imarsiz: 1,
  Imarli: 2,
  SurulmusTarla: 3,
  EkiliTarla: 4,
  HasatSonrasiBos: 5,
};

var araziData = [
  {
    id: 1,
    il: "İstanbul",
    ilce: "Esenyurt",
    buyukluk: 1687,
    deger: 52000,
    durum: 1,
    satici: "@aware",
  },
  {
    id: 2,
    il: "Çorum",
    ilce: "Sungurlu",
    buyukluk: 962,
    deger: 6750,
    durum: 1,
  },
  {
    id: 3,
    il: "Yozgat",
    ilce: "Sorgun",
    buyukluk: 1050,
    deger: 4650,
    durum: 2,
  },
  { id: 4, il: "Kars", ilce: "Merkez", buyukluk: 1450, deger: 3500, durum: 1 },
  { id: 5, il: "Ağrı", ilce: "Patnos", buyukluk: 870, deger: 2200, durum: 1 },
  { id: 6, il: "Ardahan", ilce: "Posof", buyukluk: 960, deger: 1900, durum: 1 },
];

var mazotFiyat = 22.95;

var araziHandlers = {
  showArazilerim: function () {
    var actions = "<div class='actions'>";
    actions += "<button class='mt20 btn-arazi-gor'>Arazi Satın Al</button>";
    actions += "</div>";

    var html = "";
    if (game.araziler.length > 0) {
      html = actions;
      html += "<div class='farm-item-list arazi-list'>";

      html += game.araziler
        .map((m) => {
          var item = "<div class='farm-item arazi'>";
          item += "<img src='./img/land.png'/>";
          item += "<p>" + m.buyukluk + " m2</p>";
          item += "<p>" + m.deger + " <i class='fas fa-turkish-lira-sign'></i></p>";
          item += "<p>" + m.ilce + " / " + m.il + "</p>";
          if (item.durum == araziDurum.Satildi) {
            item += "<p class='etiket etiket-error'>Satıldı</p>";
          }
          else if (item.durum == araziDurum.Opsiyonlu) {
            item += "<p class='etiket etiket-error'>Opsiyonlu</p>";
          } else {
            item += "<button class='btn-text btn-sell' data-type='arazi' data-id='" + m.id + "'>Araziyi Sat</button>";
          }
          item += "</div>";
          return item;
        }).join("");
      html += "</div>";
    } else {
      html += "<p>Henüz bir araziniz yok..</p>";
      html += actions;
    }

    $(".game .header ul.menu li").removeClass("active");
    $(".game .header ul.menu li.arazi").addClass("active");
    $(".story-1").addClass("dn");
    $(".page-content").html(html).removeClass("dn");
  },
  goruntuleAraziler: function (e, container) {
    var html = "<table class='arazi-list'>";
    html += "<tr><th></th><th>Konumu</th><th>Büyüklük</th><th>Değeri</th><th>Satıcı</th><th></th></tr>";
    html += araziData.map((m) => {
      var item = "<tr>";
      item += "<td><img class='cell-image' src='./img/land.png'/></td>";
      item += "<td>" + m.ilce + " / " + m.il + "</td>";
      item += "<td>" + m.buyukluk + " m2</td>";
      item += "<td class='text-bold text-error'>" + m.deger + " <i class='fas fa-turkish-lira-sign'></i></td>";
      item += "<td>" + (m.satici || "") + "</td>";
      item += "<td>" + (m.durum == araziDurum.Satildi ? "<span class='etiket etiket-error'>Satıldı</span>" : "<button class='btn-text btn-purchase' data-type='arazi' data-id='" +
        m.id + "'>Satın Al</button>") + "</td>";
      item += "</tr>";
      return item;
    }).join("");
    html += "</table>";

    container = container || ".page-content";
    $(container).html(html).removeClass("dn");
  },
  onAraziSatinAl: function (id) {
    var item = araziData.find((f) => f.id == id);
    if (item) {
      if (!gameHandlers.kontrolUcret(item.deger)) return;

      var islem = islemTipi.SatinAlArazi + "|" + id;
      var message = item.deger + " <i class='fas fa-turkish-lira-sign'></i> değerindeki bu arazi satın alınacaktır. Devam etmek istiyor musunuz?<br/>Tapu işlemleri 1-2 gün sürmektedir.";
      message += "<div class='mt20 bargain-panel'>Fiyatı düşürmek için pazarlık şansınızı da deneyebilirsiniz;<br/><a href='javascript:void(0)' class='btn-bargain' data-value='" + item.deger + "'>Pazarlık Yap</a></div>"
      helpers.showConfirm(islem, "", message);
    }
  },
  satinAl: function (araziId, teklif) {
    var item = araziData.find((f) => f.id == araziId);
    if (!item) {
      helpers.showPopup(".popup-info", "Arazi Satın Alınamadı", "İşlem yapmak istediğiniz arazi bilgilerine ulaşılamadı!", "error");
      return;
    }

    var odenecekTutar = item.deger;
    if (teklif && parseFloat(teklif) > 0) {
      var oran = Math.random() * 10;
      var kabulEdilebilir = item.deger - item.deger * oran / 100;
      if (kabulEdilebilir <= teklif) {
        odenecekTutar = parseFloat(teklif);
      }
      else {
        helpers.showPopup(".popup-info", "Arazi Satın Alınamadı", "Teklifiniz satıcı tarafından yetersiz bulundu.", "error");
        return;
      }
    }

    if (!gameHandlers.kontrolUcret(item.deger))
      return;

    gameHandlers.ekleIslem(islemTipi.SatinAlArazi, { ...item, statu: araziStatu.Imarsiz });

    game.para -= odenecekTutar;
    item.durum = araziDurum.Opsiyonlu;

    helpers.guncellePara(game.para, true);
    helpers.hidePopup(".popup-confirm");
  },
  satisTamamla: function (arazi) {
    var anaArazi = araziData.find((f) => f.id == arazi.id);
    if (anaArazi)
      anaArazi.durum = araziDurum.Satildi;

    game.araziler = game.araziler || [];
    game.araziler.push(arazi);
    helpers.showPopup(".popup-info", "Satınalma Tamamlandı", "Artık yeni bir araziniz var, tebrikler.", "success");
  },
  temizlikTamamla: function (araziId) {
    var arazi = game.araziler.find((f) => f.id == araziId);
    if (arazi) {
      arazi.durum = araziDurum.Temizlendi;
      arazi.statu = araziStatu.Imarli;
      helpers.showPopup(".popup-info", "Araziniz Temizlendi", "Artık imarlı bir arsanız var. Bu arsayı tarla olrak veya ev, ağıl, ambar inşa etmek için kullanabilirsiniz.", "success");
      gameHandlers.playStory(3);
    } else {
      helpers.showPopup(".popup-info", "Araziniz Temizlenemedi", "Teknik bir sorun sebebiyle işlem gerçekleştirilemedi!", "error");
    }
  },
  araziTemizle: function (araziId) {
    var arazi = game.araziler.find((f) => f.id == araziId);
    var traktorId = $(".farm-item.traktor.active").data("id");
    var traktor = kiralikTraktorList.find((f) => f.id == traktorId);
    if (traktor) {
      var kacSaat = arazi.buyukluk / traktor.verim;
      var ucret = kacSaat * traktor.kiralama + kacSaat * traktor.mazotTuketim * mazotFiyat;
      game.para -= ucret;
      arazi.durum = araziDurum.Temizleniyor;

      gameHandlers.ekleIslem(islemTipi.TemizleArazi, {
        duration: kacSaat,
        araziId: arazi.id,
      });
      gameHandlers.playStory(3);

      helpers.hidePopup(".popup-confirm");
      helpers.showPopup(".popup-info", "Araziniz Temizleniyor", "İşlem tamalandığında size bildireceğiz..", "success");
    } else {
      helpers.showPopup(".popup-info", "İşlem Başarısız", "Lütfen kiralanacak aracı seçtikten sonra tekrar deneyiniz.", "error");
    }
  },
  onTemizle: function () {
    var araziId = $(this).data("arazi-id");
    var arazi = game.araziler.find((f) => f.id == araziId);
    var islem = islemTipi.TemizleArazi + "|" + araziId;
    var traktorList = insaHandlers.getTraktorList(arazi.buyukluk, islemTipi.TemizleArazi);
    helpers.showConfirm(islem, "Araziyi Temizle", traktorList);
  },
  secTraktor: function () {
    $(".farm-item.traktor").removeClass("active");
    $(this).addClass("active");
  },
  getStatuDesc: function (statu) {
    switch (statu) {
      case araziStatu.SurulmusTarla:
        return "Sürülmüş Tarla";
      case araziStatu.EkiliTarla:
        return "Ekili Tarla";
      case araziStatu.HasatSonrasiBos:
        return "Harman Edildi";
      case araziStatu.Imarli:
        return "İmarlı Arazi";
      case araziStatu.Imarsiz:
        return "İmarsız Arazi";
    }
    return "İmarsız Arazi";
  },
  registerEvents: function () {
    $(document).on("click", ".btn-arazi-gor", {}, araziHandlers.goruntuleAraziler);
    $(document).on("click", ".btn-arazilerim", {}, araziHandlers.showArazilerim);
    $(document).on("click", ".btn-temizle-arazi", {}, araziHandlers.onTemizle);
  }
};
