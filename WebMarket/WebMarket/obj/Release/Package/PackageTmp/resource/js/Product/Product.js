; (function ($) {
    var elem = {
        categoryHierarchyContainer: ".category-hierarchy",
        similarItems: ".similar-items",
        currentListItem: undefined,
        basketSummary: ".search-area .btn-basket-summary",
        btnNewComment: ".btn-new-comment",
        btnSendComment: ".btn-send-comment",
        btnRating: ".rating-panel span i",
        pnlComments: ".tab-pane#reviews",
        btnShowInstallmets: ".btn-show-installments",

        waitingEvents:[]
    };

    var handlers = {
        getCategoryHierarchy: function () {
            var cid = $(elem.categoryHierarchyContainer).data('category-id');
            $.post("/Product/GetCategoryHierarchy", { parentID: cid }, function (data) {
                if (data.success == 1) {
                    $(elem.categoryHierarchyContainer).html(data.html);
                }
                handlers.processWaitingEvent();
            });
        },
        getSimilarItems: function () {
            var pID = $('#ProductID').val();
            var cID = $('#categoryID').val();
            var sID = $('#storeID').val();
            var postData = { categoryID: cID, storeID: sID, productID: pID };

            $.get("/Product/GetSimilarItems", postData, function (data) {
                if (data.success == 1) {
                    $(elem.similarItems).append(data.html);
                    $(elem.similarItems).fadeIn(300);
                }
                else {
                    $(elem.similarItems).hide();
                }
                handlers.processWaitingEvent();
            });
        },
        getProductCommentStats: function () {
            var productName = $(".product-detail .name").html();
            $.post("/Product/GetProductCommentStats", { productID: $('#ProductID').val(), productName: productName }, function (data) {
                if (data.success) {
                    $('.comment-count').html('('+data.cc+')');
                    $('.rating-star span').attr('style', 'width:' + (data.avg * 20) + '%;');
                    $(elem.pnlComments).html(data.html);
                } else {
                    $(elem.pnlComments).html('<span>Ürün yorumları yüklenemedi. Lütfen daha sonra yekrar deneyiniz.</span>');
                }
                handlers.processWaitingEvent();
            });
        },
        showCommentForm: function () {
            var isLogin = ($('#isLoggedIn').val() == "true");
            if (isLogin) {
                $('.no-comment').hide();
                $(this).removeClass('btn').addClass('block-title mb0');
                $('.comment-form').fadeIn();
            } else {
                $('.login-modal').modal('show');
            }
        },
        onSendComment: function () {
            if (aware.validate('.comment-form', 'top')) {
                var postData = {
                    productID: $('#ProductID').val(),
                    rating: $(elem.btnRating + '.active').data('rating') || 0,
                    title: $('.comment-form #Title').val(),
                    value: $('.comment-form #Value').val()
                };

                $.post("/Product/SaveComment", postData, function (data) {
                    if (data.success == 1) {
                        $(elem.btnNewComment).hide();
                        $('.comment-form').fadeOut();
                        $('.comment-success').removeClass("hide").addClass("in");
                    } else {
                        aware.showError(data.message, 'Yorum Kaydedilemedi', 'fa-remove');
                    }
                });
            }
            return false;
        },
        showInstallments:function() {
            var isLoaded = $(".tabs-wrapper #installments").data("loaded") == 1;
            if (!isLoaded) {
                var total = $(".product-detail .formatted-price").data("price");
                $.post("/Order/GetAllInstallments", { productID: $('#ProductID').val(), total: total,mode:1 }, function (data) {
                    if (data.success) {
                        $(".tabs-wrapper #installments").html(data.html);
                    } else {
                        $(".tabs-wrapper #installments").html("<span>Taksit seçeneği bulunamadı!</span>");
                    }
                    $(this).data("loaded", 1);
                });
            }
            return false;
        },
        processWaitingEvent:function(){
            if (elem.waitingEvents.length > 0) {
                var callback = elem.waitingEvents.pop();
                callback();
            }
        },
        ready: function () {
            if ($(elem.pnlComments).length > 0 && $(elem.pnlComments).data('loaded') == 0) { elem.waitingEvents.push(handlers.getProductCommentStats);}
            if ($(elem.categoryHierarchyContainer).val() != undefined) { elem.waitingEvents.push(handlers.getCategoryHierarchy); }
            elem.waitingEvents.push(handlers.getSimilarItems);

            $(document).on('click', '.rating-summary ', {}, function() {
                $('.review-tab').click();
                $('html, body').animate({
                    scrollTop: $(".review-tab").offset().top-100
                }, 1000);
            });

            $(document).on('click', elem.btnRating, {}, function () {
                $(elem.btnRating).removeClass('active');
                $(this).addClass('active');
            });

            handlers.processWaitingEvent();
        }
    };

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
        { selector: elem.btnNewComment, container: document, event: "click", handler: handlers.showCommentForm, data: {} },
        { selector: elem.btnSendComment, container: document, event: "click", handler: handlers.onSendComment, data: {} },
        { selector: elem.btnShowInstallmets, container: document, event: "click", handler: handlers.showInstallments, data: {} },
    ];

    $(function () {
        for (var it in eventMetaData) {
            var item = eventMetaData[it];
            $(item.container).on(item.event, item.selector, item.data, item.handler);
        }
    });
}(jQuery));