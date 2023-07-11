(function ($) {
    var handlers = {
        onSaveUser: function (selector) {
            var permissions = ""; //Collect permissions
            $(".user-permission.active").each(function () {
                var permissionID = $(this).data("permission-id");
                permissions += permissionID + ",";
            });

            $("#Permissions").val(permissions);
            return aware.validate(selector);
        },
        onPasswordChange: function () {
            var wrapper = '.change-password-dialog';
            if (aware.validate(wrapper)) {
                var _cPassword = $(wrapper + ' #CurrentPassword').val();
                if (_cPassword == undefined || _cPassword.length == 0) { _cPassword = ''; }
                var _newPassword = $(wrapper + ' #Password').val();

                aware.showLoading(undefined, true);
                $.post("/Account/ChangePassword", { currentpassword: _cPassword, password: _newPassword }, function (data) {
                    if (data.success == 1) {
                        aware.clearFields(wrapper);
                        aware.showMessage('Şifreniz Başarıyla Güncellendi', '', '', 'fa-check');
                    }
                    else {
                        aware.showError(data.message, 'İşlem Başarısz', 'fa-minus-circle');
                    }
                });
            }
            return false;
        },
        handleRegister: function () {
            if (!$(".register-dialog .cbx-aggreement").hasClass("active")) {
                aware.showToastr("Üyelik işlemine devam etmek için üyelik sözleşmesini kabul etmelisiniz!", "error");
                return false;
            }
            return handlers.onSaveUser(".register-dialog");
        },
        onSaveAccount: function () {
            return handlers.onSaveUser(".account-dialog");
        },
        sendNewActivation: function () {
            var container = '.activation-dialog';
            if (aware.validate(container)) {
                var _email = $(container + ' #Email').val();
                $.post("/Account/SendActivation", { email: _email }, function (data) {
                    if (data.success == 1) {
                        window.location.href = '/aktivasyon/5';
                    }
                    if (data.success == 2) {
                        aware.showMessage("Hesabınız Zaten Aktif", data.message, '', 'fa-check');
                    }
                    else {
                        aware.showError(data.message, 'İşlem Başarısız', 'fa-minus-circle');
                    }
                });
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
        handleLogin: function () {
            var url = window.location.href;
            if (aware.validate(".login-panel")) {
                aware.showLoading();
                $.post("/User/Login", $(".login-panel form").serialize(), function (data) {
                    if (data.success == 1) {
                        aware.hideDialog();
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
        ready: function () {

        }
    };

    $(function () {
        $(document).on('ready', undefined, {}, handlers.ready);
        $(document).on('click', '.btn-change-password', {}, handlers.onPasswordChange);
        $(document).on('click', '.btn-register', {}, handlers.handleRegister);
        $(document).on('click', '.btn-save-account', {}, handlers.onSaveAccount);
        $(document).on('click', '.btn-new-activation', {}, handlers.sendNewActivation);

        $(document).on('click', '.btn-forgot-password', {}, handlers.handleForgotPassword);
        $(document).on('click', '.btn-login', {}, handlers.handleLogin);
    });
}(jQuery));