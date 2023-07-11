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
            var wrapper = ".change-password-dialog";
            if (aware.validate(wrapper)) {
                var current = $(wrapper + " #CurrentPassword").val();
                if (current == undefined || current.length == 0) { current = ''; }
                var newPassword = $(wrapper + " #Password").val();
                var requestData = $(wrapper + " #RequestData").val();
                var verificationToken = $(wrapper + " input[name='__RequestVerificationToken']").val();
                var postData = { currentpassword: current, password: newPassword, data: requestData, __RequestVerificationToken: verificationToken };

                aware.showLoading(undefined, true);
                $.post("/Account/ChangePassword", postData, function (data) {
                    if (data.ok) {
                        aware.clearFields(wrapper);
                        aware.showLoading("Şifreniz Başarıyla Güncellendi");
                        aware.delayedRefresh(800, "/hesap/uye-girisi");
                    }
                    else {
                        aware.showError(data.code, "İşlem Başarısz", "minus-circle");
                    }
                });
            }
            return false;
        },
        handleRegister: function () {
            aware.validate(".register-dialog")
            if (!$(".register-dialog .cbx-aggreement").is(":checked")) {
                aware.showToastr("Üyelik işlemine devam etmek için üyelik sözleşmesini kabul etmelisiniz!", "error");
                return false;
            }
            return handlers.onSaveUser(".register-dialog");
        },
        onSaveAccount: function () {
            return handlers.onSaveUser(".account-dialog");
        },
        sendNewActivation: function () {
            if (aware.validate(".activation-dialog")) {
                var email = $(".activation-dialog #email").val();
                $.post("/Account/SendActivation", { email }, function (result) {
                        if (result.ok) {
                            window.location.href = '/aktivasyon/5';
                        }
                        else {
                            aware.showError(result.code, 'İşlem Başarısız', 'exclamation-triangle');
                        }
                    });
            }
            return false;
        },
        ready: function () {
            aware.instantValidate(".register-dialog");
            aware.instantValidate(".change-password-dialog");
        }
    };

    $(function () {
        $(document).ready(handlers.ready);
        $(document).on('click', '.btn-change-password', {}, handlers.onPasswordChange);
        $(document).on('click', '.btn-register', {}, handlers.handleRegister);
        $(document).on('click', '.btn-save-account', {}, handlers.onSaveAccount);
        $(document).on('click', '.btn-new-activation', {}, handlers.sendNewActivation);
    });
}(jQuery));