//Creating a Filter
osmanApp.filter('kkma', function () {
    return function (input, uppercase) {
        input = input || '';
        var out = "";
        for (var i = 0; i < input.length; i++) {
            out = input.charAt(i) + out;
        }
        // conditional based on optional argument
        if (uppercase) {
            out = out.toUpperCase();
        }
        return out;
    };
});

osmanApp.filter('reverseText', function () {
    return function (input) {
        return input.split("").reverse().join("");
    };
});

osmanApp.filter('subText', function () {
    return function (input, start, length) {
        return input.substr(start, length);
    };
});