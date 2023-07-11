(function ($) {
    var helpers = {
        setItemPosition: function (item, relativeElement, duration, onComplete) {
            var pos = $(relativeElement).offset();
            var eWidth = $(relativeElement).outerWidth();
            var mWidth = $(item).outerWidth();
            var _left = (pos.left + eWidth - mWidth) + "px";
            var _top = -50 + pos.top + "px";

            $(item).show();
            $(item).animate({
                'left': _left,
                'top': _top
            }, duration, onComplete);
        },
        getItems: function (container) {
            var itemsInfo = $(container).find('.items-info').val();
            elem.selectedItems = {};
            if (itemsInfo != undefined && itemsInfo.length > 0) {
                elem.selectedItems = jQuery.parseJSON(itemsInfo) || {};
            }
            return elem.selectedItems;
        },
        getImages: function (container) {
            var info = $(container).find('.images-info').val();
            if (info != undefined && info.length > 0) {
                return jQuery.parseJSON(info) || {};
            }
            return [];
        },
        arrangeQuantityBox: function (parent, dir) {
            var unit = $(parent).data("unit");
            var factor = unit == 'kg' ? 0.5 : (unit == 'gr' ? 50 : 1);
            var val = factor;

            if (dir != 0) {
                var maxCount = unit == 'gr' ? 1000 : 10;
                val = parseFloat($(parent).find("#SelectedQuantity").val());
                if (dir == -1) {
                    val = val - factor;
                    val = val < factor ? maxCount : val;
                } else {
                    val = val + factor;
                    val = val > maxCount ? factor : val;
                }
            }

            $(parent).find(".txt").html(val + " " + unit).removeClass("f13").addClass(unit == "gr" ? "f13" : "");
            $(parent).find("#SelectedQuantity").val(val);
            return false;
        },
        getCompareItemHtml: function (productID, name, imagePath) {
            var imageCdn = "http://admin.aware.com/resource/img/";
            imagePath = imagePath.indexOf(imageCdn) == -1 ? imageCdn + imagePath : imagePath;

            var html = "<div class='compared-product' data-pid='" + productID + "'>";
            html += "<i class='fa fa-remove btn-remove-compare'></i>";
            html += "<img src='" + imagePath + "'/></div>"; //<span>" + name + "</span>
            return html;
        }
    };

    var handlers = {
        arrangeImages: function () {
            var images = helpers.getImages(elem.selectedProduct);
            if (images.length <= 0) {
                var src = $(elem.selectedProduct).find(".pimage").attr("src") || "";
                if (src.length <= 0 || src.indexOf("data:image") >= 0) {
                    src = $(elem.selectedProduct).find(".pimage").data('src');
                }
                $(".product-view-modal .image img").attr("src", src);
            } else {
                var imageHtml = "";
                var imageCdn = $("#ImageCdn").val();
                for (var j = 0; j < images.length; j++) {
                    var info = images[j];
                    imageHtml += "<div class='swiper-slide'>";
                    imageHtml += "<img class='product-image' src='" + imageCdn + info.path + "' alt='" + name + "' title='" + name + "'/></div>";
                }

                $(".product-view-modal .image .swiper-wrapper").html(imageHtml);
                if (images.length > 1) {
                    $(".product-view-modal .image .swiper-pagination").removeClass("dn");
                } else {
                    $(".product-view-modal .image .swiper-pagination").addClass("dn");
                }

                if (elem.swiper != undefined) {
                    elem.swiper.destroy(true, true);
                    elem.swiper = undefined;
                }
            }

            setTimeout(function () {
                if (images.length > 1) {
                    elem.swiper = new Swiper('.popup-images', {
                        //nextButton: '.swiper-button-next',
                        //prevButton: '.swiper-button-prev',
                        spaceBetween: 10,
                        pagination: '.swiper-pagination',
                        paginationClickable: true,
                        //preloadImages: false,
                        //lazyLoading: true
                    });
                }
            }, 500);
        },
        onProductDetailRequested: function () {
            elem.selectedProduct = $(this);
            var productID = $(this).data("pid");
            var name = $(this).find(".name").attr("title");

            $(".product-view-modal .product-view").data("pid", productID);
            $(".product-view-modal .name").html(name);
            $(".product-view-modal .description").html($(this).find(".description").html());
            $(".product-view-modal .btn-detail").attr("href", "/urun-detay/" + $(this).data("seo-url"));
            $(".product-view-modal .message").hide();
            $(".product-view-modal .btn-no-stock").hide();
            $(".product-view-modal .btn-add-to-basket").enable();
            handlers.arrangeImages();

            var items = helpers.getItems($(this));
            var selectedItem = {};
            if (items.length == 1) {
                selectedItem = items[0];
                $(".product-view-modal .items-info").html("Sadece " + selectedItem.store + " marketinde").addClass("single");
                $(".product-view-modal .items").hide();
            }
            else if (items.length > 1) {
                var storeSelect = "<select class='store-selector'>";
                for (var i in items) {
                    var item = items[i];
                    if (item.isdefault) { selectedItem = item; }
                    storeSelect += "<option value='" + item.id + "'>" + item.store + " (" + item.price + ")</option>";
                }
                storeSelect += "</select>";

                var itemsHtml = "<div class='mb10'><b>Market Seçimi :</b></div>";
                itemsHtml += storeSelect;

                $(".product-view-modal .items-info").html("Bu ürün " + items.length + " markette listeleniyor.");
                $(".product-view-modal .items").html(itemsHtml).show();
                $(".product-view-modal .items .store-selector").selecto();
            }

            var isFavorite = ($("#FavoriteProducts").val() || "").indexOf("," + productID + ",") > -1;
            if (isFavorite) {
                $(".product-view-modal .btn-favorite").addClass("favorite").html("<i class='fa fa-heart'></i> Favorilerimden Kaldır");
            } else {
                $(".product-view-modal .btn-favorite").removeClass("favorite").html("<i class='fa fa-heart'></i> Favorilerime Ekle");
            }

            handlers.arrangeSelectedItem(selectedItem);
            $(".product-view-modal").modal();
            return false;
        },
        arrangeSelectedItem: function (item) {
            if (item != undefined && item.id > 0) {
                $("#SelectedItemID").val(item.id);
                var parent = $(".product .quantity-box");
                helpers.arrangeQuantityBox(parent, 0);

                $(".product .btn-no-stock").hide();
                $(".product .quantity").hide();
                $(".product .quantity-row").hide();

                if (item.forsale && !item.hasstock) {
                    $(".product .quantity-row").show();
                    $(".product .btn-no-stock").show();
                }
                else if (item.forsale) {
                    $(".product .quantity-row").show();
                    $(".product .quantity").show();
                }

                $(".product .formatted-price .lp").html(item.price.split(",")[0]);
                $(".product .formatted-price .rp").html("," + item.price.split(",")[1] + " / " + item.unit);
                $(".product .formatted-price .list-price").html(item.lprice + "/" + item.unit);
                $(".product .formatted-price").show();

                if (item.hasvariant && window.getItemVariants) {
                    window.getItemVariants();
                }
            }
        },
        onProductItemClick: function () {
            var itemID = $(this).data("value");
            $(".btn-add-to-basket").enable();

            for (i in elem.selectedItems) {
                if (elem.selectedItems[i].id == itemID) {
                    var item = elem.selectedItems[i];
                    handlers.arrangeSelectedItem(item);
                    break;
                }
            }
            return false;
        },
        onQuantityClick: function () {
            return helpers.arrangeQuantityBox($(this).parent(), $(this).hasClass('left') ? -1 : 1);
        },
        addItemToBasket: function () {
            var btn = $(this);
            var itemID = $("#SelectedItemID").val().cint();
            if (itemID == 0) {
                aware.showError("Devam etmek için lütfen market seçimi yapın!", "Seçim Yapın", "minus-circle"); return false;
            }

            var definition = aware._first(elem.selectedItems, itemID), variantInfo = {};
            if (definition && definition.hasvariant) {
                if (window.checkVariantSelection) {
                    variantInfo = window.checkVariantSelection();
                    if (!variantInfo.valid) {
                        return false;
                    }
                } else {
                    aware.confirm("Bu ürünü sepetinize eklemek için ürün detay sayfasında varyant seçimi yapmalısınız. Devam etmek istiyor musunuz?", function () {
                        aware.hideDialog();
                        window.open($(btn).parents(".product-view:eq(0)").find(".btn-detail").attr("href"), "_blank");
                        return false;
                    }, "Varyant Seçimi Gerekli");
                    return false;
                }
            }

            var quantity = parseFloat($(this).parent(".quantity:eq(0)").find("#SelectedQuantity").val());
            if (quantity <= 0 || quantity > 10) {
                aware.showError("Ürünü sepete eklemek için seçilen miktar/adet uygun değil! Lütfen kontrol edip tekrar deneyiniz..", "Miktar Seçimi ", 'minus-circle');
                return false;
            }

            $(btn).disable();
            aware.showLoading("Ürün sepete atılıyor..", true);

            $.post("/Basket/AddToBasket", { storeItemID: itemID, quantity: quantity, variantSelection: variantInfo.selection }, function (data) {
                $(btn).enable();
                if (data.success == 1) {
                    aware.showMessage("Ürün Sepetinizde", "Ürün başarıyla sepetinize eklendi.", "", "check-circle");
                    refreshBasketSummary();
                }
                else if (data.resultCode == 2) { //Giriş yapılmamış, show login form
                    aware.hideDialog();
                    $('.modal').modal('hide');
                    setTimeout(function () { $("#loginModal").modal("show"); }, 400);
                }
                else {
                    aware.showError(data.message, "İşlem Başarısız", "minus-circle");
                }
            });
            return false;
        },
        switchProduct: function () {
            if (elem.selectedProduct != undefined) {
                var isNext = $(this).hasClass('next');
                var size = $(elem.pnlProduct).length;
                var index = $(elem.pnlProduct).index($(elem.selectedProduct));
                index += isNext ? 1 : -1;

                if (isNext && index >= size) { index = 0; }
                else if (!isNext && index < 0) { index = size - 1; }

                var sp = $(elem.pnlProduct)[index];
                if (sp != undefined) { $(sp).click(); }
            }
            return false;
        },
        addRemoveFavorite: function (e) {
            e.preventDefault();

            var item = $(this);
            var pid = $(item).parents(".product-view:eq(0)").data("pid");
            var isRemove = $(item).hasClass("favorite");
            var url = "/Product/" + (isRemove ? "RemoveFavorite" : "AddToFavorite");
            var postData = isRemove ? { productIDs: pid } : { productID: pid };
            aware.showLoading(undefined, true);

            $.post(url, postData, function (data) {
                if (data.success == 1) {
                    aware.hideDialog();
                    if (isRemove) {
                        $(item).removeClass("favorite").removeClass("fa-heart").addClass("fa-heart-o");
                        if (!$(item).parents(".product-view:eq(0)").hasClass("item")) {
                            $(item).html("<i class='fa fa-heart'></i> Favorilerime Ekle");
                        }
                        aware.showToastr("Ürün favorilerinizden başarıyla kaldırıldı.", "info");
                    } else {
                        $(item).addClass("favorite").removeClass("fa-heart-o").addClass("fa-heart");
                        if (!$(item).parents(".product-view:eq(0)").hasClass("item")) {
                            $(item).html("<i class='fa fa-heart'></i> Favorilerimden Kaldır");
                        }
                        aware.showToastr("Ürün favorilerinize başarıyla eklendi.", "success");
                    }
                } else {
                    aware.showError(data.message, "İşlem Başarısız", "fa-minus-circle");
                }
            });
            return false;
        },
        addToCompareList: function () {
            var addedValues = handlers.checkComparedProducts(false);
            var parent = $(this).parents("[data-pid]:eq(0)");
            var productID = $(parent).data("pid");

            var isAdded = $.inArray(productID + "", addedValues) > -1;
            if (isAdded) {
                aware.showToastr("Bu ürün zaten karşılaştırma listesinde!", "error");
            }
            else if ($(elem.comparePanel).find(".compare-list .compared-product").length >= 5) {
                aware.showToastr("Karşılaştırma listesine en fazla 5 ürün ekleyebilirsiniz!", "info");
            }
            else {
                addedValues.push(productID);
                aware.createCookie("compare-list", addedValues.join(","), 60, "/");

                var name = $(parent).find(".name").attr("title") || $(parent).find(".name").html();
                var imgSrc = $(parent).find("img").attr("src");

                var html = helpers.getCompareItemHtml(productID, name, imgSrc);
                $(elem.comparePanel).find(".compare-list").append(html);
                $(elem.btnCompareSelected).removeClass("disabled");

            }

            $(elem.comparePanel).slideDown();
            return false;
        },
        removeCompareProduct: function () {
            var parent = $(this).parents(".compared-product:eq(0)");
            var productID = $(parent).data("pid");

            var cookie = aware.readCookie("compare-list");
            var addedValues = (cookie != null && cookie.length > 0 ? cookie.split(",") : []);
            addedValues = $.grep(addedValues, function (i) {
                return i !== productID + "" && i !== "";
            });

            aware.createCookie("compare-list", addedValues.join(","), 60, "/");
            aware.showToastr("Ürün karşılaştırma listesinden kaldırıldı.", "success");
            $(parent).remove();

            if ($(elem.comparePanel).find(".compare-list .compared-product").length > 0) {
                $(elem.btnCompareSelected).removeClass("disabled");
            }
            else {
                $(elem.btnCompareSelected).addClass("disabled");
                $(elem.comparePanel).slideUp();
            }
            return false;
        },
        checkComparedProducts: function (reload) {
            var cookie = aware.readCookie("compare-list");
            var addedValues = (cookie != null && cookie.length > 0 ? cookie.split(",") : []);

            //Clear invalid selections
            $(elem.comparePanel).find(".compare-list .compared-product").each(function () {
                var productID = $(this).data("pid");
                if ($.inArray(productID + "", addedValues) == -1) {
                    $(this).remove();
                }
            });

            if (reload && addedValues.length > 0 && cookie.length > 0) {
                $.post("/Product/GetComparedProductsInfo", { productIDs: addedValues.join(",") }, function (result) {
                    if (result.success == 1) {
                        for (var it in result.items) {
                            var item = result.items[it];
                            var html = helpers.getCompareItemHtml(item.ID, item.Title, item.Url);
                            $(elem.comparePanel).find(".compare-list").append(html);
                            $(elem.btnCompareSelected).removeClass("disabled");
                        }
                    }
                });
            }
            return addedValues;
        },
        ready: function () {
            var items = helpers.getItems(".product-detail");
            if (items.length >= 1) {
                var item = aware._firstItem(items, function (e) { return e.isdefault; });
                handlers.arrangeSelectedItem(item);
            }
            else {
                $('#SelectedItemID').val($(".product-detail #itemID").val());
            }

            $(document).bind('keydown', function (event) {
                if ($('.product-view-modal').hasClass('in')) {
                    if (event.keyCode == 37) {
                        $('.product-view-modal .btn-preview.prev').click();
                    }
                    else if (event.keyCode == 39) {
                        $('.product-view-modal .btn-preview.next').click();
                    }
                }
                return true;
            });

            $(".store-selector").selecto({});
            $(elem.comparePanel).slideUp();
            handlers.checkComparedProducts(true);
        },
        toggleFixedFilter: function () {
            if ($(".product-list .fixed-filter").hasClass("open")) {
                $(".product-list .fixed-filter").removeClass("open");
            } else {
                $(".product-list .fixed-filter").addClass("open");
            }
            return false;
        }
    };

    var elem = {
        pnlProduct: ".product-view:not(.modal-body)",
        pnlProductItem: ".product .items .store-selector ul li",
        btnPreview: ".product .btn-preview",
        btnAddToBasket: ".btn-add-to-basket",
        basketSummary: ".search-area .btn-basket-summary",
        productListItem: ".product-list .products-panel .item",
        btnFavorite: ".btn-favorite",
        btnAddToCompare: ".btn-compare-product",
        btnRemoveCompare: ".btn-remove-compare",
        btnCompareSelected: ".btn-compare-selected",
        pnlFilterIco: ".pnl-filter-ico",
        comparePanel: ".product-compare-panel",
        btnCloseCompare: ".btn-close-compare",

        selectedItems: {},
        selectedProduct: undefined,
        swiper: undefined
    };

    $(function () {
        $(document).on('ready', undefined, {}, handlers.ready);
        $(document).on('click', elem.pnlProduct, {}, handlers.onProductDetailRequested);
        $(document).on("click", elem.pnlProductItem, {}, handlers.onProductItemClick);
        $(document).on('click', '.quantity-box .left,.quantity-box .right', {}, handlers.onQuantityClick);
        $(document).on('click', elem.btnAddToBasket, {}, handlers.addItemToBasket);
        $(document).on('click', elem.btnPreview, {}, handlers.switchProduct);
        $(document).on('click', elem.btnFavorite, {}, handlers.addRemoveFavorite);
        $(document).on('click', elem.pnlFilterIco, {}, handlers.toggleFixedFilter);

        $(document).on('click', elem.btnAddToCompare, {}, handlers.addToCompareList);
        $(document).on('click', elem.btnRemoveCompare, {}, handlers.removeCompareProduct);
        $(document).on('click', elem.btnCloseCompare, {}, function () { $(elem.comparePanel).slideUp(); return false; });
    });
}(jQuery));