var aware = new $.Aware();

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : 0;
}

function refreshBasketSummary() {
    $(".basket-summary-container").data("loaded", "0");
    $(".btn-basket-summary").removeClass("hover").click();
}

(function ($) {
    var handlers = {
        smartSearch: function () {
            if (elem.isProcessing == 1) {
                elem.isProcessing = 2;
                var query = $(elem.searchBox).val();
                if (query.length < 3) { $(".search-panel .search-results").fadeOut(200); elem.isProcessing = 0; }
                else {
                    $(elem.searchBox).addClass("loading");
                    $.post('/Product/SearchProducts', { keyword: query }, function (data) {
                        $(elem.searchBox).removeClass("loading");
                        $(".search-panel .search-results").html(data.html).slideDown(200);
                        $(".search-panel .search-results span.db.name").each(function () {
                            var $el = $(this);

                            var words = query.replace(new RegExp(",", "gi"), " ").split(" ");
                            for (var i in words) {
                                var word = words[i];
                                if (word.indexOf("*") > -1) {
                                    word = word.replace(new RegExp("*", "gi"), "");
                                }

                                if (word.length > 2 && $(this).html().match(new RegExp(word, "gi"))) {
                                    $el.html($el.html().replace(new RegExp(word, "gi"), "<span class='search-annotate'>" + word + "</span>"));
                                }
                            }
                        });

                        elem.isProcessing = 0;
                        return true;
                    });
                }
            }
        },
        onSearchBoxClicked: function () {
            if ($('.search-panel .search-results').html().length > 0) {
                $('.search-panel .search-results').slideDown(200);
            }
        },
        onSearchKeyDown: function () {
            elem.isProcessing = elem.isProcessing == 2 ? 2 : 0;
        },
        onSearchKeyUp: function () {
            clearTimeout(elem.searchTimeout);
            elem.isProcessing = elem.isProcessing == 2 ? 2 : 1;
            elem.searchTimeout=setTimeout(handlers.smartSearch(), 800);
        },
        onBeforeSearch: function () {
            var keyword = $(elem.searchBox).val().trim();
            if (keyword.length >= 3) {
                window.location.href = "/urunler?q=" + keyword;
            }
            return false;
        },
        onWindowScroll: function () {
            if ($(window).scrollTop() >= 500) {
                $(elem.scrollToTop).fadeIn(400);
            } else {
                $(elem.scrollToTop).fadeOut(400);
            }
        },
        validateContactUsForm: function () {
            return aware.validate('.contact-us');
        },
        onHelpRequested: function () {
            $('.help-box').removeClass('active');
            $(this).addClass('active');

            var index = $(this).data('index');
            $('.help-content').removeClass('active');
            $('.help-content[data-index="' + index + '"]').addClass('active');
        },
        handleLogin: function (wrapper) {
            var url = window.location.href;
            if (aware.validate(wrapper)) {
                $.post("/Account/Login", $(wrapper + ' form').serialize(), function (data) {
                    if (data.success == 1) {
                        window.location.href = url;
                    }
                    else if (data.success == 2) { //Activation Required
                        aware.showError("Hesabınıza erişmek için aktivasyonu tamamlamalısınız!", "Aktivasyon Bekleniyor..");
                        aware.delayedRefresh(2000, '/aktivasyon/2');
                    }
                    else {
                        aware.showError(data.message, "Giriş Başarısız", "fa-minus-circle");
                    }
                });
            }
            return false;
        },
        toggleLoginModal: function (e) {
            if (e.data.isLogin) {
                $('.login-modal .forgot-password-dialog').hide();
                $('.login-modal .login-dialog').show();
            }
            else {
                $('.login-modal .login-dialog').hide();
                $('.login-modal .forgot-password-dialog').show();
            }
            return false;
        },
        handleForgotPassword: function () {
            if (aware.validate(".forgot-password-dialog")) {
                $.post("/Account/ForgotPassword", $('.forgot-password-dialog form').serialize(), function (data) {
                    if (data.success == 1) {
                        aware.clearFields(".forgot-password-dialog");
                        aware.showMessage("İşlem Başarılı", data.message);
                    } else {
                        aware.showError(data.message, "İşlem Başarısız", "fa-minus-circle");
                    }
                });
            }
            return false;
        },
        processFacebookLogin: function () {
            $.post("/Social/GetFacebookLoginUrl", {}, function (data) {
                if (data.success == 1) {
                    document.location = data.url;
                }
                else {
                    aware.showToastr("İşlem gerçekleştirilemiyor..", "error");
                }
            });
            return false;
        },
        arrangeBasketSummary: function () {
            var summaryContainer = ".basket-summary-container";
            var setBasketSummary = function (response) {
                if (response.success == 1) {
                    $(".header .btn-basket-summary .item-count").html(response.itemCount);
                    $(summaryContainer + " .summary").removeClass("hide").removeClass("error").html(response.html);

                    setTimeout(function () {
                        $(summaryContainer + " ul").mCustomScrollbar({
                            theme: "dark-thick",
                            scrollButtons: { enable: true }
                        });
                    }, 300);
                }
                else {
                    $(summaryContainer + " .summary").removeClass("hide").addClass("error").html("Sepet özeti gösterilemiyor!");
                }
                $(summaryContainer + " .loading").addClass("hide");
                return false;
            };

            var getBasketSummary = function () {
                if ($("#isLoggedIn").val() == "true") {
                    var isLoaded = $(summaryContainer).data("loaded");
                    if (isLoaded == 0) {
                        $(summaryContainer).data("loaded", "1");
                        $(summaryContainer + " .loading").removeClass("hide");

                        $.post("/Basket/GetBasketSummary", {}, setBasketSummary);
                    }
                }
                return false;
            };
            getBasketSummary();

            $(document).on("click", elem.btnBasketSummary, {}, function () {
                if ($("#isLoggedIn").val() == "true") {
                    var isActive = $(this).hasClass("hover");
                    if (isActive) {
                        $(this).removeClass("hover");
                        $(summaryContainer).slideUp();
                    } else {
                        $(this).addClass("hover");
                        $(summaryContainer).slideDown();
                        getBasketSummary();
                    }
                }
                else {
                    $("#loginModal").modal("show");
                }
                return false;
            });

            $(document).on("mouseout", summaryContainer, {}, function (event) {
                if (aware.fixMouseOut(this, event)) {
                    $(elem.btnBasketSummary).removeClass("hover");
                    $(summaryContainer).slideUp();
                }
            });

            //Remove summary item
            $(document).on("click", summaryContainer + " .btn-remove-item", {}, function () {
                var parent = $(this).parents("li:eq(0)");
                var itemID = $(parent).data("id");
                var postData = { basketID: $("#BasketID").val(), basketItemID: itemID };

                aware.processRequest("/Basket/DeleteBasketSummaryItem", postData, "Ürün Sepetinizden Kaldırılıyor..", setBasketSummary);
                return false;
            });
        },
        ready: function () {
            $(elem.scrollToTop).click(function () { $("html, body").animate({ scrollTop: 0 }, "slow"); });

            $(document).on("mouseout", ".search-panel", {}, function (event) {
                if (aware.fixMouseOut(this, event)) { $(this).children(".search-results").fadeOut("slow"); }
            });

            $(document).on("click", elem.btnCloseDialog, {}, function () { aware.hideDialog(); });
            $(document).on("click", ".wm-cbx", {}, function () {
                var isActive = $(this).hasClass("active");
                var hidden = $(this).find("input[type='hidden']");
                isActive ? $(this).removeClass("active") : $(this).addClass("active");
                if ($(hidden).length > 0) {
                    $(hidden).val(isActive ? "false" : "true");
                }
            });

            $(document).on('click', '.menu-toggle', {}, function () {
                if ($('.top-menu .navigation').hasClass('closed')) {
                    $('.top-menu .navigation').removeClass('closed');
                } else {
                    $('.top-menu .navigation').addClass('closed');
                }
            });

            $('[data-toggle="tooltip"]').tooltip();

            $(window).load(function () {
                $(".scrollContent").mCustomScrollbar({
                    theme: "dark-thick",
                    scrollButtons: {
                        enable: true
                    }
                });
            });

            $(window).scroll(function () {
                if (($(document).height() - $(window).height()) > 500 && $(window).scrollTop() > 180) {
                    $('.header').addClass('fixed');
                    //$(".body-container").addClass("fixed-padding");
                }
                else {
                    $('.header').removeClass('fixed').removeClass('sticky');
                    //$(".body-container").removeClass("fixed-padding");
                }

                if (($(document).height() - $(window).height()) > 400 && $(window).scrollTop() > 250) {
                    $('.header').addClass('sticky');
                }
                else {
                    $('.header').removeClass('sticky');
                }
            });

            $(".slick-items.lazy-enabled .carousel-control").click(function () {
                var parent = $(this).parent('.slick-items:eq(0)');
                var item = $(parent).find('.item.active');
                item = $(this).hasClass('right') ? $(item).next() : $(item).prev();

                if (item.length == 0 && $(this).hasClass('left')) {
                    item = $(parent).find('.item').last();
                }
                setTimeout(function () {
                    $(item).find('img.lazy').lazy({
                        effect: "fadeIn",
                        effectTime: 500,
                        bind: "event"
                    });
                }, 400);
            });

            //Carousel auto play
            $(".carousel").each(function () {
                $(this).carousel({ interval: $(this).data("interval") });
            });

            $("img").error(function () {
                $(this).attr("src", "/resource/img/Icons/notfound.jpg");
            });
            handlers.arrangeBasketSummary();

            //Ajax unauthorized fix
            $(document).ajaxError(function (e, xhr) {
                aware.hideDialog();
                elem.isProcessing = 0;
                if ($(".btn-add-to-basket").length > 0) { $(".btn-add-to-basket").enable(); }

                if (xhr.status == 401) {
                    $('.login-modal .login-description').show();
                    $('.btn-login-dialog').click();

                    aware.hideDialog();
                    $('.login-modal').modal('show');
                    setTimeout(function () { $('.login-modal .login-description').fadeOut(2000); }, 2000);

                    //var returnUrl = window.location.pathname;
                    //aware.delayedRefresh(500, '/uye-girisi?returnUrl=' + returnUrl);
                }
                else if (xhr.status > 0 && (xhr.status == 403 || xhr.status != 401)) {
                    aware.showError('İstek işlenirken bir hata gerçekleşti!', 'İşlem Başarısız!', 'minus-circle');
                }
            });
        }
    };

    var elem = {
        searchBox: ".search-panel .search-box",
        btnSearch: ".search-panel .btn-search",
        scrollToTop: ".scroll-to-top",
        btnContactUs: ".btn-contact-us",
        btnHelpBox: ".help-box",
        btnCloseDialog: ".btn-close-dialog",
        btnFacobookLogin: ".facebook-login",
        btnBasketSummary: ".btn-basket-summary",
        isProcessing: 0,
        searchTimeout:undefined
    };

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
        { selector: undefined, container: window, event: "scroll", handler: handlers.onWindowScroll, data: {} },
        { selector: elem.searchBox, container: document, event: "keyup", handler: handlers.onSearchKeyUp, data: {} },
        { selector: elem.searchBox, container: document, event: "keydown", handler: handlers.onSearchKeyDown, data: {} },
        { selector: elem.searchBox, container: document, event: "click", handler: handlers.onSearchBoxClicked, data: {} },
        { selector: elem.btnSearch, container: document, event: "click", handler: handlers.onBeforeSearch, data: {} },
        { selector: elem.btnContactUs, container: document, event: "click", handler: handlers.validateContactUsForm, data: {} },
        { selector: elem.btnHelpBox, container: document, event: "click", handler: handlers.onHelpRequested, data: {} },
        { selector: elem.btnFacobookLogin, container: document, event: "click", handler: handlers.processFacebookLogin, data: {} }
    ];

    $(function () {
        for (var it in eventMetaData) {
            var item = eventMetaData[it];
            $(item.container).on(item.event, item.selector, item.data, item.handler);
        }

        $(document).on('click', '.btn-login-dialog', { isLogin: true }, handlers.toggleLoginModal);
        $(document).on('click', '.btn-forgot-password-dialog', { isLogin: false }, handlers.toggleLoginModal);
        $(document).on('click', '.btn-forgot-password', {}, handlers.handleForgotPassword);
        $(document).on('click', '.btn-login', {}, function () { handlers.handleLogin('.login-dialog'); return false; });
        $(document).on('click', '.btn-login-for-page', {}, function () { handlers.handleLogin('.login-dialog-for-page'); return false; });
    });
}(jQuery));