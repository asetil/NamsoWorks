(function ($) {
    var elem = {
        btnDisplay3: ".btn-dsp3",
        btnDisplay4: ".btn-dsp4",
        btnDisplayList: ".btn-dsp-list",
        orderSelectBox: "#orderItemsBy",//".select-order > ul > li",
        filterCategory: "#filterByCategory",//".select-category > ul > li",
        filterStore: "#filterByStore",//".select-store > ul > li",
        filterItem: ".filter-container .filter-section .filter-item:not()",
        btnRemoveFilterSelection: ".filter-selections .filter span i",
        txtKeywordFilter: ".filter-section .keyword-filter .text",
        btnFilter: ".btn-filter"
    };

    var helpers = {
        sortByName: function (direction) {
            var mylist = $('.products-panel');
            var listitems = mylist.children('.item').get();
            listitems.sort(function (a, b) {
                var compA = $(a).find('.name').text().toUpperCase();
                var compB = $(b).find('.name').text().toUpperCase();
                return helpers.getOrderDirection(compA, compB, direction);
            });
            $.each(listitems, function (idx, itm) { mylist.append(itm); });

            $(window).scrollTop($(window).scrollTop() + 1); //for lazy load images
            $(window).scrollTop($(window).scrollTop() - 1);
        },
        sortByPrice: function (direction) {
            var mylist = $('.products-panel');
            var listitems = mylist.children('.item').get();
            listitems.sort(function (a, b) {
                var valueA = parseFloat($(a).data('price'));
                var valueB = parseFloat($(b).data('price'));
                return helpers.getOrderDirection(valueA, valueB, direction);
            });
            $.each(listitems, function (idx, itm) { mylist.append(itm); });

            $(window).scrollTop($(window).scrollTop() + 1); //for lazy load images
            $(window).scrollTop($(window).scrollTop() - 1);
        },
        getOrderDirection: function (compA, compB, direction) {
            if (direction == 0) {
                return (compA < compB) ? 1 : (compA > compB) ? -1 : 0;
            }
            else {
                return (compA < compB) ? -1 : (compA > compB) ? 1 : 0;
            }
        },
        arrangeQuantityBox: function (parent, dir) {
            var unit = $(parent).find('.suffix').html();
            var factor = unit == 'kg' ? 0.5 : (unit == 'gr' ? 50 : 1);
            var val = factor;

            if (dir != 0) {
                var maxCount = unit == 'gr' ? 1000 : 10;
                val = parseFloat($(parent).find('.txt').html());
                if (dir == -1) {
                    val = val - factor;
                    val = val < factor ? maxCount : val;
                } else {
                    val = val + factor;
                    val = val > maxCount ? factor : val;
                }
            }

            $(parent).find('.txt').html(val).addClass(unit == "gr" ? "f13" : "").removeClass(unit != "gr" ? "f13" : "");
            $('#SelectedQuantity').val(val);
            return false;
        },
        checkFilterSelections: function (filterType, filterID, isRemove, elem) {
            var filter, filterName;
            var filterContainer = $(".filter-selections .filter[data-filter-type='" + filterType + "']");

            if (isRemove) {
                filter = $(filterContainer).find("span[data-filter-id='" + filterID + "']");
                if (filter.length > 0) {
                    $(filter).remove();
                }

                if ($(filterContainer).find('span').length <= 0) {
                    $(filterContainer).remove();
                }

                if ($(".filter-selections .filter").length <= 0) {
                    $(".filter-selections").hide();
                }
            } else {
                $(".filter-selections").show();
                if ($(filterContainer).length <= 0) {
                    filterName = $(elem).parents('.filter-section:eq(0)').find("h5").html();
                    var filterHtml = "<div class='filter' data-filter-type='" + filterType + "'>" + filterName + " : </div>";
                    $(".filter-selections").append(filterHtml);
                    filterContainer = $(".filter-selections .filter[data-filter-type='" + filterType + "']");
                }

                if (filterType != 'keyword') {
                    filter = $(filterContainer).find("span[data-filter-id='" + filterID + "']");
                    if ($(filter).length <= 0) {
                        filterName = $(elem).data('name');
                        var html = "<span data-filter-id='" + filterID + "'>" + filterName + " <i class='fa fa-remove'></i></span>";
                        $(filterContainer).append(html);
                    }
                }
                else if (filterType == 'keyword') {
                    filterName = $(elem).val();
                    var html = "<span data-filter-id='" + filterID + "'>" + filterName + " <i class='fa fa-remove'></i></span>";
                    var span = $(filterContainer).find('span');
                    if ($(span).length > 0) {
                        $(filterContainer).find('span').replaceWith(html);
                    } else {
                        $(filterContainer).append(html);
                    }
                }
            }
            return false;
        },
        arrangeFilterButton: function () {
            var searchDiv = $('.filter-container');
            if (searchDiv.size() != 1) return;
            var windowHeight = (window.innerHeight > 0) ? window.innerHeight : screen.height;
            var searchDivHeight = searchDiv.height();
            var scrollTop = $(window).scrollTop();
            var top = searchDiv.offset().top;
            var pos1 = scrollTop + windowHeight - top;

            var pos2 = searchDivHeight + top;
            var resultPos;
            if (pos2 >= pos1) {
                resultPos = pos1 - 55;
            } else {
                resultPos = pos2 - 135;
            }

            $(elem.btnFilter).css({ "top": resultPos + "px", "display": "block" });
        }
    };

    var handlers = {
        arrange3DProducts: function () {
            $('.products-panel').removeClass('dsp-list').removeClass('dsp4').addClass('dsp3');
            $('.products-panel .item').removeClass('col-md-3').removeClass('col-sm-4').addClass('col-md-4').addClass('col-sm-6');

            $('.btn-dsp').removeClass('active');
            $('.btn-dsp3').addClass('active');
            $('.dsp3 .product-view .description').addClass('dn');
        },
        arrange4DProducts: function () {
            $('.products-panel').removeClass('dsp-list').removeClass('dsp3').addClass('dsp4');
            $('.products-panel .item').removeClass('col-md-4').removeClass('col-sm-6').addClass('col-md-3').addClass('col-sm-4');

            $('.btn-dsp').removeClass('active');
            $('.btn-dsp4').addClass('active');
            $('.dsp4 .product-view .description').addClass('dn');
        },
        arrangeListProducts: function () {
            $('.products-panel').removeClass('dsp3').removeClass('dsp4').addClass('dsp-list');
            $('.products-panel .item').removeClass('col-md-3').removeClass('col-sm-4').removeClass('col-md-4').removeClass('col-sm-6');

            $('.btn-dsp').removeClass('active');
            $('.btn-dsp-list').addClass('active');
            $('.dsp-list .product-view .description').removeClass('dn');

            $(window).scrollTop($(window).scrollTop() + 1); //for lazy load images
            $(window).scrollTop($(window).scrollTop() - 1);
        },
        sortItemsBy: function () {
            var orderID = $(this).data('value');
            if (orderID <= 2) { helpers.sortByName(orderID == 1 ? 1 : 0); }
            else if (orderID > 2) { helpers.sortByPrice(orderID == 3 ? 1 : 0); }
        },
        dedectScrolling: function () {
            try {
                var scrollDiv = $(".dedect-scroll");
                if ($(scrollDiv).data("dedect-scroll") == 1 && $(scrollDiv).data("loading-complete") == 0) {
                    var delta = $(document).height() / $(window).height();
                    var scrollBarHeight = $(window).height() / delta;
                    var scrollAmount = delta * ($(scrollDiv).offset().top - scrollBarHeight) / (1 + delta);

                    if ($(window).scrollTop() > scrollAmount) {
                        $(scrollDiv).data("dedect-scroll", 0);
                        $(".dedect-scroll img").show();

                        //Filtreleri al sayfa sayısını bir arttır istek yap
                        var page = parseInt($(scrollDiv).data("page"));
                        var filter = $(scrollDiv).data("filter");
                        var url = aware.QS_replace("page", page + 1, "/Product/LoadNextPage?" + filter);
                        $.get(url, {}, function (data) {
                            $(".dedect-scroll").data("dedect-scroll", 1);
                            $(".dedect-scroll img").hide();

                            if (data.success == 1) {
                                $(".product-list .products-panel").append(data.html);
                                $(scrollDiv).data("loading-complete", data.completed);
                                $(scrollDiv).data("filter", url.split("?")[1] || "");
                                $(scrollDiv).data("page", page + 1);

                                $("body .lazy").lazy({
                                    effect: "fadeIn",
                                    effectTime: 500,
                                    threshold: 0,
                                    bind: "event"
                                });

                                if ($(".btn-dsp.active").hasClass("btn-dsp3")) {
                                    handlers.arrange3DProducts();
                                }
                                else if ($(".btn-dsp.active").hasClass("btn-dsp4")) {
                                    handlers.arrange4DProducts();
                                }
                                else if ($(".btn-dsp.active").hasClass("btn-dsp-list")) {
                                    handlers.arrangeListProducts();
                                }
                            }
                            else {
                                $(".dedect-scroll").data("loading-complete", 0);
                            }
                        });
                    }
                }
            }
            catch (e) { }
            return false;
        },
        searchProducts: function (e) {
            var categoryID = parseInt($("#filterByCategory").val());
            var storeID = parseInt($("#filterByStore").val());
            var sortBy = parseInt($("#orderItemsBy").val());
            var isFavorites = parseInt($("#IsFavorites").val()) === 1;

            var url = "";
            if (isFavorites) {
                if (categoryID > 0) { url += "cid=" + categoryID + "&"; }
                if (storeID > 0) { url += "sid=" + storeID + "&"; }
                if (sortBy > 0) { url += "sirala=" + sortBy + "&"; }
                url = "/favorilerim" + (url ? "?" : "") + url.substr(0, url.length - 1);
            }
            else {
                url = "/urunler";
                var cname = $("#filterByCategory").parent().find(".preview").html();
                var sname = $("#filterByStore").parent().find(".preview").html();

                if (categoryID > 0 && storeID > 0) {
                    url = "/" + aware.toSeoUrl(sname) + "-" + aware.toSeoUrl(cname) + "-urunleri-" + storeID + "-" + categoryID;
                }
                else if (categoryID > 0) {
                    url = "/" + aware.toSeoUrl(cname) + "-urunleri-" + categoryID;
                }
                else if (storeID > 0) {
                    url = "/" + aware.toSeoUrl(sname) + "-market-urunleri-" + storeID;
                }
                if (sortBy > 0) { url += "?sirala=" + sortBy; }
            }

            aware.showLoading("Yükleniyor...", true);
            aware.delayedRefresh(400, url);
        },
        onFilterSelection: function () {
            var isRemove = $(this).hasClass('active');
            if (isRemove) {
                $(this).removeClass('active');
                $(this).find("i.cbx").removeClass('fa-check-square').addClass('fa-square');
            } else {
                $(this).find("i.cbx").removeClass('fa-square').addClass('fa-check-square');
                $(this).addClass('active');
            }

            var filterType = $(this).parents('.filter-section:eq(0)').data('filter-type');
            var filterID = $(this).data('filter-id');
            helpers.checkFilterSelections(filterType, filterID, isRemove, $(this));
            return false;
        },
        onKeywordFilterChange: function () {
            var value = $(this).val();
            var isRemove = value.length <= 0;
            helpers.checkFilterSelections('keyword', 'qval', isRemove, $(this));
            return false;
        },
        onFilterSelectionRemoved: function () {
            var filterType = $(this).parents('.filter:eq(0)').data('filter-type');
            var filterID = $(this).parents('span:eq(0)').data('filter-id');
            helpers.checkFilterSelections(filterType, filterID, true);

            if (filterType == "keyword") {
                $(elem.txtKeywordFilter).val("");
            } else {
                var filterItem = $(".filter-container .filter-section[data-filter-type='" + filterType + "'] .filter-item[data-filter-id='" + filterID + "']");
                if ($(filterItem).length > 0) {
                    $(filterItem).removeClass('active');
                    $(filterItem).find('i').removeClass('fa-check-square').addClass('fa-square');
                }
            }
            return false;
        },
        submitFilters: function () {
            var filters = [];
            var keyword = $('.keyword-filter .text').val();
            if (keyword.length > 0) {
                filters.push("q=" + keyword);
            }

            var propertyIDs = [];
            $(".filter-section").each(function () {
                var filter = $(this).data("filter-type");
                var ids = [];

                $(this).find(".filter-item.active").each(function () {
                    if (filter.indexOf("pid_") > -1) {
                        propertyIDs.push($(this).data("filter-id") + "");
                    } else {
                        ids.push($(this).data("filter-id") + "");
                    }
                });

                if (ids.length > 0) {
                    filters.push(filter + "=" + ids.join(","));
                }
            });

            if (propertyIDs.length > 0) {
                filters.push("pid=" + propertyIDs.join(","));
            }

            //console.log(filters.join("&"));
            var url = "/urunler?" + filters.join("&");
            window.location.href = url;

            return false;
        },
        ready: function () {
            $(".select-store").selecto();
            $(".select-category").selecto({ allowSearch: true });
            $(".select-order").selecto();
            $(".product-store").selecto();

            //$(".select-order ul li[data-value='3']").click(); //set order as lower price to higher
            helpers.arrangeFilterButton();
            $(window).scroll(function () {
                helpers.arrangeFilterButton();

                if ($(window).scrollTop() > 250) {
                    $(".product-list .filter-panel").addClass("fixed-filter");
                    if ($(".product-list .filter-panel").length > 0) {
                        $(".header").addClass("item-wrapper");
                    }
                }
                else {
                    $(".product-list .filter-panel").removeClass("fixed-filter");
                    $(".header").removeClass("item-wrapper");
                }
            });

            if ($(".dedect-scroll").length > 0) {
                $(window).on("scroll", undefined, {}, handlers.dedectScrolling);
            }

            //Draw selected filters
            var activeFilters = $(".filter-container .filter-section .filter-item.active");
            $(activeFilters).each(function () {
                var filterType = $(this).parents(".filter-section:eq(0)").data("filter-type");
                var filterID = $(this).data("filter-id");
                helpers.checkFilterSelections(filterType, filterID, false, $(this));
            });
        }
    };

    $(function () {
        $(document).on('ready', undefined, {}, handlers.ready);
        $(document).on('click', elem.btnDisplay3, {}, handlers.arrange3DProducts);
        $(document).on('click', elem.btnDisplay4, {}, handlers.arrange4DProducts);
        $(document).on('click', elem.btnDisplayList, {}, handlers.arrangeListProducts);
        $(document).on('change', elem.filterCategory, {}, handlers.searchProducts);
        $(document).on('change', elem.filterStore, {}, handlers.searchProducts);
        $(document).on('change', elem.orderSelectBox, {}, handlers.searchProducts);

        $(document).on('click', elem.filterItem, {}, handlers.onFilterSelection);
        $(document).on('click', elem.btnRemoveFilterSelection, {}, handlers.onFilterSelectionRemoved);
        $(document).on('textchange keyup cut paste input', elem.txtKeywordFilter, {}, handlers.onKeywordFilterChange);
        $(document).on('click', elem.btnFilter, {}, handlers.submitFilters);
    });
}(jQuery));