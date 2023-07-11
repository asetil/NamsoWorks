/**
 * @name		jQuery Countdown Plugin
 * @author		Martin Angelov
 * @version 	1.0
 * @url			http://tutorialzine.com/2011/12/countdown-jquery/
 * @license		MIT License
 */

(function ($) {

    // Number of seconds in every time division
    var days = 24 * 60 * 60,
		hours = 60 * 60,
		minutes = 60;

    // Creating the plugin
    $.fn.countdown = function (prop) {

        var options = $.extend({
            callback: function () { },
            timestamp: 0
        }, prop);

        var left, d, h, m, s, positions;
        this.addClass('countdownHolder');
        var elem = this;

        (function tick() {
            left = Math.floor((options.timestamp - (new Date())) / 1000);
            if (left < 0) {
                left = 0;
                elem.addClass('finished');
                elem.html('Bu Kampanya Sona Erdi');
                return false;
            }else if (left < 10800) {
                elem.addClass('warn');
            }

            var info = '';
            d = Math.floor(left / days);
            if(d>0){info+=d + ' gün ';}
            left -= d * days;

            // Number of hours left
            h = Math.floor(left / hours);
            info += h + ' saat ';
            left -= h * hours;

            // Number of minutes left
            m = Math.floor(left / minutes);
            info += m + ' dk';
            left -= m * minutes;

            // Number of seconds left
            s = left;
            //info += s + ' sn kaldı.';
            elem.html(info);

            // Calling an optional user supplied callback
            options.callback(d, h, m, s);

            // Scheduling another call of this function in 1s
            setTimeout(tick, 1000);
        })();

        // This function updates two digit positions at once
        function updateDuo(minor, major, value) {
            switchDigit(positions.eq(minor), Math.floor(value / 10) % 10);
            switchDigit(positions.eq(major), value % 10);
        }

        return this;
    };
})(jQuery);

$(function () {
    $('.countdown').each(function () {
        var ts = (new Date()).getTime() + parseInt($(this).data("remained"));
        var item = $(this);

        $(this).countdown({
            timestamp: ts,
            callback: function (days, hours, minutes, seconds) {
                var message = "Kampanya bitimine ";
                message += days + " gün" + ", ";
                message += hours + " saat" + ", ";
                message += minutes + " dakika kaldı!";
                //message += seconds + " saniye kaldı!";
                $(item).attr('title', message);
            }
        });
    });
});
