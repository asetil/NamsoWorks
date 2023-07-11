var helpers = {
  getUid: function () {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, (c) =>
      (
        c ^
        (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (c / 4)))
      ).toString(16)
    );
  },
  showMessage: function (title, content, iconCss) {
    helpers.showPopup(".popup-info", title, content, iconCss);
  },
  showPopup: function (selector, title, content, iconCss) {
    $(selector).find(".title").html(title);
    if (content) {
      $(selector).find(".content").html(content);
    }

    if (iconCss) {
      var icss = "check";
      if (iconCss == "error") {
        icss = "remove";
      } else if (iconCss == "warning") {
        icss = "exclamation-triangle";
      } else if (iconCss == "info") {
        icss = "info-circle";
      }

      $(selector)
        .find(".icon")
        .html("<i class='fas fa-" + icss + "'></i>");
      $(selector)
        .find(".icon")
        .attr("class", "icon " + iconCss);
    }

    $(selector).addClass("show");
  },
  showConfirm: function (islem, title, content, iconCss) {
    if ($(".popup-confirm").hasClass("show")) {
      this.showMessage("Uyarı", "Önce mevcut işlemi tamamlayın!", "warning");
      return;
    }

    $(".popup-confirm").find(".btn-ok").data("islem", islem);
    this.showPopup(".popup-confirm", title, content, iconCss);
  },
  hidePopup: function (selector) {
    if (selector) {
      if (typeof selector == "string") $(selector).removeClass("show");
      else if ($(selector.currentTarget)) {
        $(selector.currentTarget).parents(".popup").removeClass("show");
      }
    } else {
      $(".popup").removeClass("show");
    }
  },
  guncellePara: function (para, kaydet) {
    game.para = Math.round(para * 100) / 100;
    $(".game .header span.para").html(game.para);

    if (kaydet) gameHandlers.save();
  },
  arrangeContent: function (selector) {
    var html = $(selector).html();
    $(".page-content").html(html).removeClass("dn");
  },
  checkDate: function (date, dateCompare) {
    return date && dateCompare && new Date(date) > dateCompare;
  },
  getDate: function (date, year, month, day, hour, minute, second) {
    var result = date ? new Date(date) : new Date();
    if (year && year != 0) {
      result.setFullYear(result.getFullYear() + year);
    }

    if (month && month != 0) {
      result.setMonth(result.getMonth() + month);
    }

    if (day && day != 0) {
      result.setDate(result.getDate() + day);
    }

    if (hour && hour != 0) {
      result.setHours(result.getHours() + hour);
    }

    if (minute && minute != 0) {
      result.setMinutes(result.getMinutes() + minute);
    }

    if (second && second != 0) {
      result.setSeconds(result.getSeconds() + second);
    }
    return result;
  },
  dateToString: function (date, format) {
    try {
      var gDate = typeof date == "string" ? new Date(date) : date;
      if (gDate) {
        var mDate = {
          S: gDate.getSeconds(),
          M: gDate.getMinutes(),
          H: gDate.getHours(),
          d: gDate.getDate(),
          m: gDate.getMonth() + 1,
          A: "",
          y: gDate.getFullYear(),
        };

        if (format.indexOf("A") > -1) {
          var monthName = helpers.getMonthName(gDate.getMonth());
          mDate["A"] = monthName;
        }

        // Apply format and add leading zeroes
        return format.replace(/([SMHdmAy])/g, function (key) {
          return (mDate[key] < 10 ? "0" : "") + mDate[key];
        });
      }
    } catch (e) { }
    return "";
  },
  getMonthName: function (month) {
    switch (month) {
      case 0:
        return "Ocak";
      case 1:
        return "Şubat";
      case 2:
        return "Mart";
      case 3:
        return "Nisan";
      case 4:
        return "Mayıs";
      case 5:
        return "Haziran";
      case 6:
        return "Temmuz";
      case 7:
        return "Ağustos";
      case 8:
        return "Eylül";
      case 9:
        return "Ekim";
      case 10:
        return "Kasım";
      case 11:
        return "Aralık";
    }
    return month;
  },
  dateDifference: function (first, second) {
    if (first && second) {
      var diff = (second.getTime() - first.getTime()) / 1000;
      return diff / 60; // minutes
    }
    throw "dateDifference > Hatalı parametre!";
  },
};
