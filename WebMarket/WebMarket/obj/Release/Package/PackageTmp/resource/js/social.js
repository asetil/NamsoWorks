; (function ($) {
    var elem = {
        btnFacobookLogin: ".facebook-login",
        btnFbClient: ".fb-client-login",
    };

    var handlers = {
        processFacebookLogin: function () {
            $.post("/Social/GetFacebookLoginUrl", {}, function (data) {
                if (data.success == 1) {
                    document.location = data.url;
                }
                else {
                    aware.showToastr('İşlem gerçekleştirilemiyor..', 'error');
                }
            });
            return false;
        },
        getUserData: function (response) {
            var postData = {
                id: response.id,
                email: response.email,
                first_name: response.first_name,
                last_name: response.last_name,
                gender: response.gender,
                birthdate: response.birthday
            };

            $.post("/Social/HandleSocialClient", postData, function (result) {
                if (result.success == 1) {
                    aware.showToastr('Facebook ile başarıyla giriş yaptınız!', 'success');
                    aware.delayedRefresh(600, "/");
                }
                else {
                    aware.showToastr('Facebook ile giriş yapılamadı!', 'error');
                }
            });
            return false;
        },
        handleFacebookLogin: function (response) {
            console.log(response);
            if (response.status === 'connected') {
                FB.api('/me', { fields: 'id,first_name,last_name,email,gender,birthday' }, handlers.getUserData);
            } else {
                aware.showToastr('Facebook ile giriş yapılamadı!', 'error');
            }
            return false;
        },
        processSocialLogin: function () {
            FB.getLoginStatus(function (response) {
                console.log("Status : ", response);

                //logged in
                if (response.status === 'connected') {
                    FB.api('/me', { fields: 'id,first_name,last_name,email,gender,birthday' }, handlers.getUserData);
                } else if (response.status === 'not_authorized') {
                    FB.login(handlers.handleFacebookLogin, { scope: 'public_profile,email' });
                } else {
                    FB.login(handlers.handleFacebookLogin, { scope: 'public_profile,email' });
                }
            });
            return false;
        },
        ready: function () {

        }
    };

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
        { selector: elem.btnFacobookLogin, container: document, event: "click", handler: handlers.processFacebookLogin, data: {} },
        { selector: elem.btnFbClient, container: document, event: "click", handler: handlers.processSocialLogin, data: {} }
    ];

    $(function () {
        for (var it in eventMetaData) {
            var item = eventMetaData[it];
            $(item.container).on(item.event, item.selector, item.data, item.handler);
        }
    });
}(jQuery));