; (function ($) {
    var elem = {
        quantityElement: '.my-basket .quantity-box .left,.my-basket .quantity-box .right',
        btnSelectList: '.favorite-container .left-part .list-item',
        btnUseCoupon: '.coupon-usage .field-row .btn-use-coupon',
        cbxFavoriteHeader: ".favorite-container .cbx-header",
        cbxFavorite: ".favorite-container .product-wrp.active .cbx-favorite",
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
            $(".my-basket .product-wrp.active .cbx-favorite.active").each(function () {
                var parent = $(this).parents(".product-wrp:eq(0)");
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
        searchFavorites: function () {
            setTimeout(function() {
                var keyword = $(elem.txtSearchFavorite).val();
                var storeID = $(elem.sbxMarketFilter + ".selected").data('value');

                $('.favorite-container .product-wrp').addClass('active');
                $('.favorite-container .product-wrp .store-list .info').removeClass("dn");
                $('.favorite-container .product-wrp').each(function () {
                    if (keyword.length >= 2) {
                        var productName = $(this).find('.product-info').attr('title');
                        if (productName.toLowerCase().indexOf(keyword.toLowerCase()) == -1) {
                            $(this).removeClass('active');
                        }
                    }

                    if (storeID > 0) {
                        var storeInfo = $(this).data('stores');
                        if (storeInfo.indexOf("," + storeID + ",") == -1) {
                            $(this).removeClass("active");
                        } else {
                            $(this).find(".store-list .info").addClass("dn");
                            $(this).find(".store-list .info[data-store-id='" + storeID + "']").removeClass("dn");
                        }
                    }
                });

                if (storeID > 0) {
                    $('.favorite-container .btn-order-favorites').removeClass('dn');
                } else {
                    $('.favorite-container .btn-order-favorites').addClass('dn');

                }
            }, 300);
            return false;
        },
        orderFavorites: function (selectedFavorites, selectedStore) {
            aware.showLoading(undefined, true);
            if (selectedFavorites.length == 0 || selectedStore <= 0) {
                aware.showError("Seçimlerinizi kontrol ettikten sonra tekrar deneyiniz!", "İşleme Devam Edilemiyor", "fa-minus-circle");
                return false;
            }

            $.post("/Basket/AddFavoritesToBasket", { storeID: selectedStore, productIDs: selectedFavorites }, function (data) {
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
                    $(".my-basket .product-wrp[data-product-id='" + data.ids[i] + "']").remove();
                }

                if ($(".my-basket .product-wrp").length < 1) {
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
            var productID = $(this).parents(".product-wrp:eq(0)").data("product-id");
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
            var favoriteID = $(this).parents(".product-wrp:eq(0)").data("product-id");
            var storeID = $(this).parents(".info:eq(0)").data("store-id");

            var message = "<p>Favori ürünü sepetinize eklemek üzeresiniz.</p>";
            message += "<p>Sepetinizde farklı bir markete ait ürünler varsa bu ürünler sepetinizden <u style='color:#dc143c;'>çıkartılacaktır.</u></p>";
            message += "<p>Devam etmek istiyor musunuz?</p>";

            aware.confirm(message, function () {
                handlers.orderFavorites(favoriteID, storeID);
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
                    var storeID = $('.select-store #filterByStore').val();
                    handlers.orderFavorites(favorites, storeID);
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
        toggleAllFavorites: function () {
            var isActive = $(this).hasClass("active");
            if (isActive) {
                $(elem.cbxFavorite).removeClass("active");
            } else {
                $(elem.cbxFavorite).addClass("active");
            }
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
        { selector: elem.sbxMarketFilter, container: document, event: "click", handler: handlers.searchFavorites, data: {} },
        { selector: elem.txtSearchFavorite, container: document, event: "keyup", handler: handlers.searchFavorites, data: {} }
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