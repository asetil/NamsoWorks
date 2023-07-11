var aware = new $.Aware();

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : 0;
}

(function ($) {
    var handlers = {
        handleLogin: function (wrapper) {
            var url = window.location.href;
            $(wrapper).find(".login-message").removeClass("dn");
            var btn = $(this);
            btn.attr("disabled", "disabled");

            if (aware.validate(wrapper)) {
                $.post("/Account/Login", $(wrapper + ' form').serialize(), function (data) {
                    if (data.ok) {
                        window.location.href = url;
                    }
                    //else if (data.success == 2) { //Activation Required
                    //    aware.showError("Hesabınıza erişmek için aktivasyonu tamamlamalısınız!", "Aktivasyon Bekleniyor..");
                    //    aware.delayedRefresh(2000, '/aktivasyon/2');
                    //}
                    else {
                        $(wrapper).find(".login-message").html(data.message || data.code).removeClass("dn");
                        aware.showError(data.message, "Giriş Başarısız", "minus-circle");
                    }
                    btn.removeAttr("disabled");
                });
            }
            return false;
        },
        toggleLoginModal: function (e) {
            if (e.data.isLogin) {
                $('.login-panel .forgot-password-dialog').hide();
                $('.login-panel .login-dialog').show();
            }
            else {
                $('.login-panel .login-dialog').hide();
                $('.login-panel .forgot-password-dialog').show();
            }
            return false;
        },
        handleForgotPassword: function () {
            if (aware.validate(".forgot-password-dialog")) {
                $.post("/Account/ForgotPassword", $('.forgot-password-dialog form').serialize(), function (result) {
                    if (result.ok) {
                        aware.clearFields(".forgot-password-dialog");
                        $(".login-modal").modal("hide");
                        aware.showMessage("İşlem Başarılı", "Belirttiğiniz e-posta adresine şifrenizi yenileyebilmeniz için bir link gönderdik.", result.code);
                    } else {
                        aware.showError(result.code, "İşlem Başarısız", "fa-minus-circle");
                    }
                });
            }
            return false;
        },
        ready: function () {
            $(document).on("click", ".btn-close-dialog", {}, function () { aware.hideDialog(); });
            //$("img").error(function () {
            //    $(this).attr("src", "/resource/img/notfound.jpg");
            //});

            //Ajax unauthorized fix
            $(document).ajaxError(function (e, xhr) {
                aware.hideDialog();

                if (xhr.status == 401) {
                    $('.login-panel .login-description').show();
                    $('.btn-login-dialog').click();

                    aware.hideDialog();
                    $(".login-modal").modal("show");
                }
                else if (xhr.status > 0 && (xhr.status == 403 || xhr.status != 401)) {
                    aware.showError('İstek işlenirken bir hata gerçekleşti!', 'İşlem Başarısız!', 'minus-circle');
                }
            });
        }
    };

    $(function () {
        $(document).ready(handlers.ready);
        $(document).on('click', '.btn-login-dialog', { isLogin: true }, handlers.toggleLoginModal);
        $(document).on('click', '.btn-forgot-password-dialog', { isLogin: false }, handlers.toggleLoginModal);
        $(document).on('click', '.btn-forgot-password', {}, handlers.handleForgotPassword);
        $(document).on('click', '.btn-login', {}, function () { handlers.handleLogin('.login-dialog'); return false; });
        $(document).on('click', '.btn-login-for-page', {}, function () { handlers.handleLogin('.login-dialog-for-page'); return false; });
    });
}(jQuery));