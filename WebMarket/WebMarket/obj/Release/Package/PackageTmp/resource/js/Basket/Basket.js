; (function ($) {
    var elem = {
        quantityElement: '.my-basket .quantity-box .left,.my-basket .quantity-box .right',
        btnSelectList: '.favorite-container .left-part .list-item',
        btnUseCoupon: '.coupon-usage .field-row .btn-use-coupon',
        cbxFavoriteHeader: ".favorite-container .cbx-header",
        cbxFavorite: ".favorite-container .items-wrapper .cbx-favorite",
        btnFavorite: '.btn-favorite',
        btnRemoveItem: '.btn-delete-confirm',
        sbxMarketFilter: '.select-store ul li',
        txtSearchFavorite: '.txt-search-favorite',

        btnRemoveFavorite: ".btn-remove-favorite",
        btnRemoveFavorites: ".btn-remove-favorites",
        btnOrderFavorite: ".btn-order-favorite",
        btnOrderFavorites: ".btn-order-favorites",
        isProcess: 0
    };

    var handlers = {
        onQuantityChanged: function () {
            if (elem.isProcess == 1) { return false; }
            elem.isProcess = 1;

            var parent = $(this).parent();
            var unit = $(parent).find('.suffix').html();
            var oldValue = parseFloat($(parent).find('.txt').html().replace(',', '.'));
            var maxCount = unit == 'gr' ? 1000 : 10;
            var factor = unit == 'kg' ? 0.5 : (unit == 'gr' ? 50 : 1);

            var val = oldValue;
            if ($(this).hasClass('left')) {
                val = val - factor;
                val = val < factor ? factor : val;
            } else {
                val = val + factor;
                val = val > maxCount ? maxCount : val;
            }
            if (oldValue == val) { elem.isProcess = 0; return false; }

            var bID = $('#basketID').val();
            var biID = $(this).parents('.item-row:eq(0)').data('item-id');
            var postData = { basketID: bID, basketItemID: biID, quantity: val };

            aware.showLoading('Miktar Güncelleniyor..', true);
            $.post("/Basket/ChangeBasketItemQuantity", postData, function (data) {
                elem.isProcess = 0;
                if (data.success == 1) {
                    $('.basket-container .basket-content').html(data.html);
                    aware.hideDialog();
                }
                else {
                    aware.showError(data.message, 'Miktar Güncellenemedi', 'fa-minus-circle');
                }
                return false;
            });
            return false;
        },
        getSelectedFavorites: function () {
            var favorites = "";
            var storeID = $(".select-store #filterByStore").val();
            $(".my-basket .items-wrapper .item-row:not(.dn) .cbx-favorite.active").each(function () {
                var parent = $(this).parents(".item-row:eq(0)");
                var storeInfo = $(parent).data("stores");
                var hasStore = (storeID == 0 || storeInfo.indexOf("," + storeID + ",") > -1);
                if (hasStore) {
                    favorites += $(parent).data("product-id") + ",";
                }
            });

            if (favorites.length == 0) {
                aware.showError("Devam etmek için lütfen favori ürün seçiniz!", "Seçim Yapmadınız", "fa-minus-circle");
            }
            return favorites;
        },
        orderFavorites: function (selectedFavorites) {
            aware.showLoading(undefined, true);
            var sid = $('.select-store #filterByStore').val();
            if (selectedFavorites.length == 0 || sid <= 0) {
                aware.showError("Seçimlerinizi kontrol ettikten sonra tekrar deneyiniz!", "İşleme Devam Edilemiyor", "fa-minus-circle");
                return false;
            }

            $.post("/Basket/AddFavoritesToBasket", { storeID: sid, productIDs: selectedFavorites }, function (data) {
                if (data.success == 1) {
                    if (data.fails.length > 0) {
                        aware.showMessage('İşlem Kısmen Başarılı', data.message + '<br>' + data.fails, '', 'fa-exclamation-circle');
                    } else {
                        aware.showMessage('İşlem Başarılı', data.message, '', 'fa-check');
                    }

                    refreshBasketSummary();
                }
                else {
                    aware.showError(data.message + '<br>' + data.fails, 'İşlem Başarısız', 'fa-minus-circle');
                }
            });
            return false;
        },
        removeFavorites: function (favorites) {
            if (favorites.length <= 0) {
                return aware.showError("İşleme devam edilemiyor!", "Favori Ürün Seçilmedi", 'fa-minus-circle');
            }

            aware.processRequest("/Favorite/RemoveFavorite", { productIDs: favorites }, undefined, function (data) {
                for (var i in data.ids) {
                    $(".my-basket .item-row[data-product-id='" + data.ids[i] + "']").remove();
                }

                if ($(".my-basket .item-row").length < 1) {
                    aware.delayedRefresh(300);
                }
            });
            return false;
        },
        onDeleteItem: function () {
            var itemID = $(this).data("item-id");
            var itemName = $(this).parents("tr:eq(0)").find(".name a").html();
            var message = "<b>'" + itemName.trim() + "'</b> ürününü sepetinizden çıkarmak istediğinizden emin misiniz?";

            aware.confirm(message, function () {
                var postData = { basketID: $("#basketID").val(), basketItemID: itemID };
                aware.processRequest("/Basket/DeleteBasketItem", postData, "Ürün Sepetinizden Kaldırılıyor..", function (data) {
                    if (data.html.length > 0) {
                        $(".basket-container .basket-content").html(data.html);
                        refreshBasketSummary();
                    } else {
                        aware.delayedRefresh(1000);
                    }
                });
            });
            return false;
        },
        onRemoveFavorite: function () {
            var productID = $(this).parents(".item-row:eq(0)").data("product-id");
            aware.confirm("Bu ürünü favorilerinizden kaldırmak istediğinizden emin misiniz?", function () {
                handlers.removeFavorites(productID);
            });
            return false;
        },
        onRemoveFavorites: function () {
            var favorites = handlers.getSelectedFavorites();
            if (favorites.length > 0) {
                aware.confirm("Seçtiğiniz ürünleri favorilerinizden kaldırmak istediğinizden emin misiniz?", function () {
                    handlers.removeFavorites(favorites);
                });
            }
            return false;
        },
        onOrderFavorite: function () {
            var favoriteID = $(this).parents(".item-row:eq(0)").data("product-id");
            var message = "<p>Favori ürününüzü sepetinize eklemek üzeresiniz.</p>";
            message += "<p>Sepetinizde farklı bir markete ait ürünler varsa bu ürünler sepetinizden <u style='color:#dc143c;'>çıkartılacaktır.</u></p>";
            message += "<p>Devam etmek istiyor musunuz?</p>";

            aware.confirm(message, function () {
                handlers.orderFavorites(favoriteID);
            });
            return false;
        },
        onOrderFavorites: function () {
            var favorites = handlers.getSelectedFavorites();
            if (favorites.length > 0) {
                var storeName = $(".select-store .preview").html();
                var message = "<p>'<b>" + storeName + "</b>' marketi için seçtiğiniz favori ürünlerinizi sepetinize eklemek üzeresiniz.";
                message += " Sepetinizde farklı bir markete ait ürünler varsa bu ürünler sepetinizden <u style='color:red;'>çıkartılacaktır.</u></p>";
                message += "<p>Devam etmek istiyor musunuz?</p>";
                aware.confirm(message, function () {
                    handlers.orderFavorites(favorites);
                });
            }
            return false;
        },
        useCoupon: function () {
            if (aware.validate('.coupon-usage')) {
                aware.showLoading('Kontrol ediliyor..', true);
                var postData = { basketID: $('#basketID').val(), code: $('.coupon-usage .field-box').val() };
                aware.processRequest("/Basket/UseCoupon", postData, "Kontrol Ediliyor..");
            }
            return false;
        },
        onFavoriteSearch: function () {
            var val = $(this).val();
            $('.favorite-container .items-wrapper .item-row').removeClass('filtered');
            if (val.length >= 3) {
                $('.favorite-container .items-wrapper .item-row').each(function () {
                    var productName = $(this).find('.name a').attr('title');
                    if (productName.toLowerCase().indexOf(val.toLowerCase()) == -1) {
                        $(this).addClass('filtered');
                    }
                });
            }
        },
        toggleAllFavorites: function () {
            var isActive = $(this).hasClass("active");
            if (isActive) {
                $(elem.cbxFavorite).removeClass("active");
            } else {
                $(elem.cbxFavorite).addClass("active");
            }
        },
        onFavoriteMarketChange: function () {
            var storeID = $(this).data('value');
            $('.favorite-container .items-wrapper .item-row').removeClass("dn");
            $('.favorite-container .items-wrapper .item-row .store-info').removeClass("dn");
            $('.favorite-container .btn-order-favorite').addClass('dn');
            $(".favorite-container .filter-panel .btn-order-favorites").addClass("dn");

            if (storeID > 0) {
                $(".favorite-container .items-wrapper .item-row").each(function () {
                    var storeInfo = $(this).data('stores');
                    if (storeInfo.indexOf("," + storeID + ",") == -1) {
                        $(this).addClass("dn");
                    }
                    else {
                        $(this).find(".store-info").addClass("dn");
                        $(this).find(".store-info[data-store-id='" + storeID + "']").removeClass("dn");
                        $(this).find(".btn-order-favorite").removeClass("dn");
                    }
                });
                $(".favorite-container .filter-panel .btn-order-favorites").removeClass("dn");
            }
            return false;
        },
        ready: function () {
            $('.quantity-selector').each(function () {
                $(this).selecto();
            });
            $('.select-store').selecto();
            $('.favorite-container .left-part .list-item:eq(0)').click();
        }
    };

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
        { selector: elem.btnUseCoupon, container: document, event: "click", handler: handlers.useCoupon, data: {} },
        { selector: elem.btnRemoveItem, container: document, event: "click", handler: handlers.onDeleteItem, data: {} },
        { selector: elem.sbxMarketFilter, container: document, event: "click", handler: handlers.onFavoriteMarketChange, data: {} },
        { selector: elem.txtSearchFavorite, container: document, event: "keyup", handler: handlers.onFavoriteSearch, data: {} }
    ];

    $(function () {
        for (var it in eventMetaData) {
            if (eventMetaData.hasOwnProperty(it)) {
                var item = eventMetaData[it];
                $(item.container).on(item.event, item.selector, item.data, item.handler);
            }
        }

        $(document).on("click", elem.quantityElement, {}, handlers.onQuantityChanged);
        $(document).on("click", elem.cbxFavoriteHeader, {}, handlers.toggleAllFavorites);
        $(document).on("click", elem.btnOrderFavorite, {}, handlers.onOrderFavorite);
        $(document).on("click", elem.btnOrderFavorites, {}, handlers.onOrderFavorites);
        $(document).on("click", elem.btnRemoveFavorite, {}, handlers.onRemoveFavorite);
        $(document).on("click", elem.btnRemoveFavorites, {}, handlers.onRemoveFavorites);
    });
}(jQuery));