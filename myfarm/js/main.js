(function ($) {
    var stories = [
        {
            no: 1,
            message: "",
        },
    ];

    var handlers = {
        basvuru: basvuruHandlers,
        arazi: araziHandlers,
        game: gameHandlers,

        onPurchase: function () {
            var type = $(this).data("type");
            var id = $(this).data("id");

            if (type == "arazi") {
                araziHandlers.onAraziSatinAl(id);
            }
        },

        onOkClicked: function () {
            var islem = $(this).data("islem") + '';
            var tip = parseInt(islem.split("|")[0]);

            if (tip == islemTipi.SatinAlArazi) {
                var teklif = $(".popup-confirm .bargain-panel #teklif").val();
                araziHandlers.satinAl(islem.split("|")[1], teklif);
            }
            else if (tip == islemTipi.TemizleArazi) {
                araziHandlers.araziTemizle(islem.split("|")[1]);
            }
            else if (tip == islemTipi.OyunuSifirla) {
                gameHandlers.reset();
            }
        },
        runWaitingTasks: function () {
            basvuruHandlers.kontrolBekleyenBasvurular();
        },
        ready: function () {
            gameHandlers.init();

            $(window).on("beforeunload", function (e) {
                e.preventDefault();
                gameHandlers.clear();
                return "--";
            });

            $(window).on("onbeforeunload", function (e) {
                e.preventDefault();
                gameHandlers.clear();
                return "--";
            });

            // var myEvent = window.attachEvent || window.addEventListener;
            // var chkevent = window.attachEvent ? 'onbeforeunload' : 'beforeunload';

            // myEvent(chkevent, function (e) {
            //     e.preventDefault();

            // });
        },
    };

    $(function () {
        $(document).ready(handlers.ready);

        gameHandlers.registerEvents();
        araziHandlers.registerEvents();

        $(document).on("click", ".btn-close-popup", {}, helpers.hidePopup);
        $(document).on("click", ".btn-ok", {}, handlers.onOkClicked);
        $(document).on("click", ".btn-purchase", {}, handlers.onPurchase);


        $(document).on("click", ".btn-basvur-hibe", {}, handlers.basvuru.basvurHibe);
        $(document).on(
            "click",
            ".btn-basvur-kredi",
            {},
            handlers.basvuru.basvurKredi
        );
        $(document).on(
            "click",
            ".btn-basvurularim",
            {},
            handlers.basvuru.showBasvrularim
        );
        $(document).on(
            "click",
            ".btn-iptal-basvuru",
            {},
            handlers.basvuru.iptalBasvuru
        );
        $(document).on(
            "change",
            ".kredi-basvuru-form input",
            {},
            handlers.basvuru.hesaplaGeriOdeme
        );
        $(document).on(
            "change",
            ".kredi-basvuru-form select",
            {},
            handlers.basvuru.hesaplaGeriOdeme
        );


        $(document).on(
            "click",
            ".farm-item.traktor",
            {},
            handlers.arazi.secTraktor
        );

        $(document).on("click", ".btn-insa-et", {}, insaHandlers.showInsaList);
        $(document).on("click", ".btn-uret", {}, insaHandlers.showUretimList);

        insaHandlers.setEvents();
    });
})(jQuery);
