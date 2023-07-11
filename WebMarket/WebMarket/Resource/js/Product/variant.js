; (function ($) {
    var elem = {
        btnVariant: ".variant-options.button-group .variant-option",
        cbxVariant: ".variant-options.checkbox .variant-option",
        sbxVariant: ".variant-options.select",
        rbxVariant: ".variant-options.radio-group .variant-option",

        variantTrackStock: ".variant-container .variant-property[data-track-stock='1']",
        selectedOptions: ".variant-property .variant-option.active",
        availableCombinations: {},
        selectedCombination: undefined,
        maxCombinations: 0
    };

    var handlers = {
        onSbxVariantChange: function () {
            if ($(this).find("option:selected").hasClass("no-stock")) {
                $(this).val(0);
                return false;
            }

            $(this).find("option").removeClass("active");
            $(this).find("option:selected").addClass("active").removeClass("no-stock");

            handlers.arrangeStockSelection($(this));
            return false;
        },
        onBtnVariantClick: function () {
            if ($(this).hasClass("no-stock")) {
                $(this).removeClass("active");
                return false;
            }

            $(this).parent().find(".variant-option").removeClass("active");
            $(this).addClass("active").removeClass("no-stock");

            handlers.arrangeStockSelection($(this));
            return false;
        },
        onRadioVariantClick: function () {
            if ($(this).hasClass("no-stock")) {
                $(this).removeClass("active");
                return false;
            }

            $(this).parent().find(".variant-option").removeClass("active");
            $(this).addClass("active").removeClass("no-stock");

            handlers.arrangeStockSelection($(this));
            return false;
        },
        onCbxVariantClick: function () {
            if ($(this).hasClass("active")) {
                $(this).removeClass("active");
            } else {
                var parent = $(this).parent();
                var activeCount = $(parent).find(".variant-option.active").length;
                var maxSelection = $(parent).data("max-selection");

                if (maxSelection == 0 || activeCount < maxSelection) {
                    $(this).addClass("active");
                } else {
                    aware.showError("En fazla " + maxSelection + " tane seçebilirisiniz.", "Uyarı", "minus-circle");
                }
            }

            handlers.arrangeStockSelection();
            return false;
        },
        getItemVariants: function () {
            $.post("/Product/GetItemVariants", { itemID: $('#itemID').val() }, function (response) {
                if (response.success) {
                    $(".variant-container").html(response.html);
                    elem.maxCombinations = response.data.MaxCombinations;
                    elem.availableCombinations = aware._select(response.data.Selections, function (selection) {
                        return { combination: selection.CombinationInfo, stock: selection.Stock };
                    });
                    handlers.checkStockInfo(elem.availableCombinations);
                }
            });
        },
        arrangeStockSelection: function (item) {
            var parent = $(item).parents(".variant-property:eq(0)");
            if ($(parent).data("track-stock") != 1) {
                return false;
            }

            var itemStock = parseFloat($("#stockInfo").val().replace(",", "."));
            var selectedVariants = aware._clone(elem.availableCombinations);
            var beforeVariants = $(parent).prevAll(".variant-property[data-track-stock='1']");
            beforeVariants.push(parent[0]);

            $(beforeVariants).each(function () {
                var variantID = $(this).data("variant-id");
                var activeOption = $(this).find(".variant-option.active:eq(0)");
                var optionID = $(activeOption).data("option-id") || $(activeOption).val();

                if (optionID > 0) {
                    var tempArray = [];
                    for (var i = 0; i < selectedVariants.length; i++) {
                        var sel = selectedVariants[i];
                        if (aware._containsValue(sel.combination, function (e) { return e.property == variantID && e.option == optionID; })) {
                            tempArray.push(sel);
                        }
                    }
                    selectedVariants = tempArray;
                }
            });

            $(".stock-info-display").html("");
            var nextVariants = $(parent).nextAll(".variant-property[data-track-stock='1']");
            if (nextVariants.length > 0) {
                $(nextVariants).each(function () {
                    var variant = $(this);
                    var propertyID = $(variant).data("variant-id");

                    $(variant).find(".variant-option").each(function () {
                        var opt = $(this);
                        var optionID = $(opt).data("option-id") || $(opt).val();

                        if (optionID > 0) {
                            var combin;
                            for (var i = 0; i < selectedVariants.length; i++) {
                                var sel = selectedVariants[i];
                                if (aware._containsValue(sel.combination, function (e) { return e.property == propertyID && e.option == optionID; })) {
                                    combin = sel;
                                    break;
                                }
                            }

                            if (combin) {
                                if (sel.stock > 0) {
                                    $(opt).removeClass("no-stock").attr("title", opt.text());
                                } else {
                                    $(opt).removeClass("active").addClass("no-stock").attr("title", "Bu kombinasyon mevcut değil!");
                                }
                            }
                            else if (itemStock > 0 || itemStock == -1) {
                                $(opt).removeClass("no-stock").attr("title", opt.text());
                            } else {
                                $(opt).removeClass("active").addClass("no-stock").attr("title", "Bu kombinasyon mevcut değil!");
                            }
                        }
                    });
                });
            }
            handlers.checkStockInfo(selectedVariants);
            return selectedVariants;
        },
        checkPriceInfo: function () {
            var price = 0;
            $(elem.selectedOptions).each(function () {
                price += parseFloat($(this).data("price").replace(",", "."));
            });
            return price;
        },
        checkStockInfo: function (selectionInfo) {
            var variantStock = 0;
            var itemStock = parseFloat($("#stockInfo").val().replace(",", "."));

            for (var i = 0; i < selectionInfo.length; i++) {
                var sel = selectionInfo[i];
                variantStock += sel.stock;
            }

            var hasStock = false;
            if (selectionInfo.length < elem.maxCombinations && (itemStock == -1 || (itemStock + variantStock) > 0)) {
                hasStock = true;
            }
            else {
                hasStock = variantStock > 0;
            }

            $(".product .quantity-row").show();
            if (hasStock) {
                $(".product .btn-no-stock").hide();
                $(".product .quantity").show();
            }
            else {
                $(".product .btn-no-stock").show();
                $(".product .quantity").hide();
            }

            elem.selectedCombination = selectionInfo.length == 1 ? selectionInfo[0] : undefined;
            if (elem.selectedCombination && variantStock < 10 && variantStock > 0) {
                //TODO# : Adet seçim alanında düzenleme yapmaya gerek varmı?
                $(".stock-info-display").html("Son " + variantStock + " ürün!");
            }
        },
        checkVariantSelection: function () {
            var message = "";
            var selection = "";
            $(".variant-container .variant-property").each(function () {
                var variant = $(this);
                var title = $(variant).find(".title").data("name");
                var isRequired = $(variant).data("required"), variantID = $(variant).data("variant-id"), optionID = "";

                var activeOptions = $(variant).find(".variant-option.active");
                $(activeOptions).each(function () {
                    optionID += ($(this).data("option-id") || $(this).val()) + ",";
                });

                if (isRequired && optionID.length <= 0) {
                    message += "<p class='yellow-warn'>" + title + "  için seçim yapmadınız!</p>";
                }
                selection += variantID + ":" + optionID + ";";
            });

            if (message.length > 0) {
                aware.showError(message, "Ürün Seçimi Yapmadınız", "exclamation-circle");
            }

            return {
                message: message,
                valid: (message.length == 0),
                selection: selection,
                price: handlers.checkPriceInfo(),
                combin: elem.selectedCombination
            };
        },
        ready: function () {

        }
    };

    window.getItemVariants = handlers.getItemVariants;
    window.checkVariantSelection = handlers.checkVariantSelection;
    window.getVariantPrice = handlers.checkPriceInfo;

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
        { selector: elem.btnVariant, container: document, event: "click", handler: handlers.onBtnVariantClick, data: {} },
        { selector: elem.cbxVariant, container: document, event: "click", handler: handlers.onCbxVariantClick, data: {} },
        { selector: elem.sbxVariant, container: document, event: "change", handler: handlers.onSbxVariantChange, data: {} },
        { selector: elem.rbxVariant, container: document, event: "click", handler: handlers.onRadioVariantClick, data: {} }
    ];

    $(function () {
        for (var it in eventMetaData) {
            var item = eventMetaData[it];
            $(item.container).on(item.event, item.selector, item.data, item.handler);
        }
    });
}(jQuery));