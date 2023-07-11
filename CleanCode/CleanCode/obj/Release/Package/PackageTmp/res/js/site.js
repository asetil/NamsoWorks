var aware = new $.Aware();

(function ($) {
    var handlers = {
        ready: function () {
            var activeMenu = $("#ActiveMenu").val();
            if (activeMenu > 0) {
                $("#topMenu").find("ul li").removeClass("active");
                $("#topMenu").find("ul li[data-id='" + activeMenu + "']").addClass("active");
            }

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
        isProcessing: 0,
        searchTimeout:undefined
    };

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
    ];

    $(function () {
        for (var it in eventMetaData) {
            var item = eventMetaData[it];
            $(item.container).on(item.event, item.selector, item.data, item.handler);
        }
    });
}(jQuery));