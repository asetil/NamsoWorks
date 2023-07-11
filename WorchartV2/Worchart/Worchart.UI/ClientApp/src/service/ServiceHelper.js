var serviceHelper = {
    baseUrl: "http://api.worchart.com/api",
    apiKey: "38366664383663622d623665662d343235332d396134382d646165343264323632343135",
    clientId: "86fd86cb-b6ef-4253-9a48-dae42d262415",
    accessToken: "",
    customerToken: "",
    tokenRefreshCount: 0,
    invalidAccessTokenMsg: "ERR.TOKEN.INVALIDACCESSTOKEN",
    currentLanguage : "tr-TR",

    getCookie: function (cname) {
        var name = cname + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i].trim();
            if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
        }
        return "";
    },

    addCookie: function (name, value, minutes, path) {
        path = path || "/";
        var expires = "";

        if (minutes) {
            var date = new Date();
            date.setTime(date.getTime() + (minutes * 60 * 1000));
            var expires = "; expires=" + date.toGMTString();
        }
        document.cookie = name + "=" + value + expires + "; path=" + path;
    }
};

export default serviceHelper;
