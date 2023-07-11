/*
 * Toastr
 * Copyright 2012-2014 John Papa, Hans Fjällemark, and Tim Ferrell.
 * All Rights Reserved.
 * Use, reproduction, distribution, and modification of this code is subject to the terms and
 * conditions of the MIT license, available at http://www.opensource.org/licenses/mit-license.php
 *
 * Author: John Papa and Hans Fjällemark
 * ARIA Support: Greta Krafsig
 * Project: https://github.com/CodeSeven/toastr
 */
; (function (define) {
    define(['jquery'], function ($) {
        return (function () {
            var $container;
            var listener;
            var toastId = 0;
            var toastType = {
                error: 'error',
                info: 'info',
                success: 'success',
                warning: 'warning'
            };

            var toastr = {
                clear: clear,
                remove: remove,
                error: error,
                getContainer: getContainer,
                info: info,
                options: {},
                subscribe: subscribe,
                success: success,
                version: '2.1.0',
                warning: warning
            };

            var previousToast;

            return toastr;

            //#region Accessible Methods
            function error(message, title, optionsOverride) {
                return notify({
                    type: toastType.error,
                    iconClass: getOptions().iconClasses.error,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function getContainer(options, create) {
                if (!options) { options = getOptions(); }
                $container = $('#' + options.containerId);
                if ($container.length) {
                    return $container;
                }
                if (create) {
                    $container = createContainer(options);
                }
                return $container;
            }

            function info(message, title, optionsOverride) {
                return notify({
                    type: toastType.info,
                    iconClass: getOptions().iconClasses.info,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function subscribe(callback) {
                listener = callback;
            }

            function success(message, title, optionsOverride) {
                return notify({
                    type: toastType.success,
                    iconClass: getOptions().iconClasses.success,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function warning(message, title, optionsOverride) {
                return notify({
                    type: toastType.warning,
                    iconClass: getOptions().iconClasses.warning,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function clear($toastElement) {
                var options = getOptions();
                if (!$container) { getContainer(options); }
                if (!clearToast($toastElement, options)) {
                    clearContainer(options);
                }
            }

            function remove($toastElement) {
                var options = getOptions();
                if (!$container) { getContainer(options); }
                if ($toastElement && $(':focus', $toastElement).length === 0) {
                    removeToast($toastElement);
                    return;
                }
                if ($container.children().length) {
                    $container.remove();
                }
            }
            //#endregion

            //#region Internal Methods

            function clearContainer(options) {
                var toastsToClear = $container.children();
                for (var i = toastsToClear.length - 1; i >= 0; i--) {
                    clearToast($(toastsToClear[i]), options);
                };
            }

            function clearToast($toastElement, options) {
                if ($toastElement && $(':focus', $toastElement).length === 0) {
                    $toastElement[options.hideMethod]({
                        duration: options.hideDuration,
                        easing: options.hideEasing,
                        complete: function () { removeToast($toastElement); }
                    });
                    return true;
                }
                return false;
            }

            function createContainer(options) {
                $container = $('<div/>')
                    .attr('id', options.containerId)
                    .addClass(options.positionClass)
                    .attr('aria-live', 'polite')
                    .attr('role', 'alert');

                $container.appendTo($(options.target));
                return $container;
            }

            function getDefaults() {
                return {
                    tapToDismiss: true,
                    toastClass: 'toast',
                    containerId: 'toast-container',
                    debug: false,

                    showMethod: 'fadeIn', //fadeIn, slideDown, and show are built into jQuery
                    showDuration: 300,
                    showEasing: 'swing', //swing and linear are built into jQuery
                    onShown: undefined,
                    hideMethod: 'fadeOut',
                    hideDuration: 1000,
                    hideEasing: 'swing',
                    onHidden: undefined,

                    extendedTimeOut: 1000,
                    iconClasses: {
                        error: 'toast-error',
                        info: 'toast-info',
                        success: 'toast-success',
                        warning: 'toast-warning'
                    },
                    iconClass: 'toast-info',
                    positionClass: 'toast-top-right',
                    timeOut: 5000, // Set timeOut and extendedTimeOut to 0 to make it sticky
                    titleClass: 'toast-title',
                    messageClass: 'toast-message',
                    target: 'body',
                    closeHtml: '<button>&times;</button>',
                    newestOnTop: true,
                    preventDuplicates: false,
                    progressBar: false
                };
            }

            function publish(args) {
                if (!listener) { return; }
                listener(args);
            }

            function notify(map) {
                var options = getOptions(),
                    iconClass = map.iconClass || options.iconClass;

                if (options.preventDuplicates) {
                    if (map.message === previousToast) {
                        return;
                    }
                    else {
                        previousToast = map.message;
                    }
                }

                if (typeof (map.optionsOverride) !== 'undefined') {
                    options = $.extend(options, map.optionsOverride);
                    iconClass = map.optionsOverride.iconClass || iconClass;
                }

                toastId++;

                $container = getContainer(options, true);
                var intervalId = null,
                    $toastElement = $('<div/>'),
                    $titleElement = $('<div/>'),
                    $messageElement = $('<div/>'),
                    $progressElement = $('<div/>'),
                    $closeElement = $(options.closeHtml),
                    progressBar = {
                        intervalId: null,
                        hideEta: null,
                        maxHideTime: null
                    },
                    response = {
                        toastId: toastId,
                        state: 'visible',
                        startTime: new Date(),
                        options: options,
                        map: map
                    };

                if (map.iconClass) {
                    $toastElement.addClass(options.toastClass).addClass(iconClass);
                }

                if (map.title) {
                    $titleElement.append(map.title).addClass(options.titleClass);
                    $toastElement.append($titleElement);
                }

                if (map.message) {
                    $messageElement.append(map.message).addClass(options.messageClass);
                    $toastElement.append($messageElement);
                }

                if (options.closeButton) {
                    $closeElement.addClass('toast-close-button').attr("role", "button");
                    $toastElement.prepend($closeElement);
                }

                if (options.progressBar) {
                    $progressElement.addClass('toast-progress');
                    $toastElement.prepend($progressElement);
                }

                $toastElement.hide();
                if (options.newestOnTop) {
                    $container.prepend($toastElement);
                } else {
                    $container.append($toastElement);
                }


                $toastElement[options.showMethod](
                    { duration: options.showDuration, easing: options.showEasing, complete: options.onShown }
                );

                if (options.timeOut > 0) {
                    intervalId = setTimeout(hideToast, options.timeOut);
                    progressBar.maxHideTime = parseFloat(options.timeOut);
                    progressBar.hideEta = new Date().getTime() + progressBar.maxHideTime;
                    if (options.progressBar) {
                        progressBar.intervalId = setInterval(updateProgress, 10);
                    }
                }

                $toastElement.hover(stickAround, delayedHideToast);
                if (!options.onclick && options.tapToDismiss) {
                    $toastElement.click(hideToast);
                }

                if (options.closeButton && $closeElement) {
                    $closeElement.click(function (event) {
                        if (event.stopPropagation) {
                            event.stopPropagation();
                        } else if (event.cancelBubble !== undefined && event.cancelBubble !== true) {
                            event.cancelBubble = true;
                        }
                        hideToast(true);
                    });
                }

                if (options.onclick) {
                    $toastElement.click(function () {
                        options.onclick();
                        hideToast();
                    });
                }

                publish(response);

                if (options.debug && console) {
                    console.log(response);
                }

                return $toastElement;

                function hideToast(override) {
                    if ($(':focus', $toastElement).length && !override) {
                        return;
                    }
                    clearTimeout(progressBar.intervalId);
                    return $toastElement[options.hideMethod]({
                        duration: options.hideDuration,
                        easing: options.hideEasing,
                        complete: function () {
                            removeToast($toastElement);
                            if (options.onHidden && response.state !== 'hidden') {
                                options.onHidden();
                            }
                            response.state = 'hidden';
                            response.endTime = new Date();
                            publish(response);
                        }
                    });
                }

                function delayedHideToast() {
                    if (options.timeOut > 0 || options.extendedTimeOut > 0) {
                        intervalId = setTimeout(hideToast, options.extendedTimeOut);
                        progressBar.maxHideTime = parseFloat(options.extendedTimeOut);
                        progressBar.hideEta = new Date().getTime() + progressBar.maxHideTime;
                    }
                }

                function stickAround() {
                    clearTimeout(intervalId);
                    progressBar.hideEta = 0;
                    $toastElement.stop(true, true)[options.showMethod](
                        { duration: options.showDuration, easing: options.showEasing }
                    );
                }

                function updateProgress() {
                    var percentage = ((progressBar.hideEta - (new Date().getTime())) / progressBar.maxHideTime) * 100;
                    $progressElement.width(percentage + '%');
                }
            }

            function getOptions() {
                return $.extend({}, getDefaults(), toastr.options);
            }

            function removeToast($toastElement) {
                if (!$container) { $container = getContainer(); }
                if ($toastElement.is(':visible')) {
                    return;
                }
                $toastElement.remove();
                $toastElement = null;
                if ($container.children().length === 0) {
                    $container.remove();
                }
            }
            //#endregion

        })();
    });
}(typeof define === 'function' && define.amd ? define : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) { //Node
        module.exports = factory(require('jquery'));
    } else {
        window['toastr'] = factory(window['jQuery']);
    }
}));


var text_processing = 'Ödeme işlemi gerçekleşiyor. Lütfen bekleyiniz...',
	text_instalment = 'Taksit',
	text_total = 'Toplam Tutar',
	text_rate = 'Vade Farkı',
	warn_card_owner = 'Lütfen Kart Sahibi bilgisini giriniz!',
	warn_card_number = 'Lütfen geçerli bir kredi kartı numarası giriniz!',
	warn_cvv = 'Lütfen geçerli bir güvenlik kodu giriniz ().',
	warn_select_expiry = 'Lütfen kartınızın son kullanma tarihini seçiniz.',
	warn_expiry = 'Lütfen geçerli bir son kullanım tarihi giriniz.',
	payment_success_header = 'Ödeme başarılı! Bekleyiniz...',
	payment_error_header = 'Ödeme Hatası!',
	payment_td_error_header = '3D Secure Hatası!',
	text_transaction_details = 'İşlem Detayları',
	text_order_number = 'Sipariş No',
	text_response_code = 'Banka İşlem Sonucu Kodu',
	text_response_desc = 'Banka Cevabı',
	text_contact_us = 'LÜTFEN BU BİLGİLERİ NOT ALIP TEKRAR ÖDEME YAPMAYI DENEYİNİZ.<br />HATA ALMAYA DEVAM EDERSENİZ İLETİŞİM BİLGİLERİMİZDEN BİZE ULAŞINIZ.',
	text_td_error_desc = '3D Secure Doğrulaması iptal edildi ya da başarısız.';

function payNow() {
    if (checkForm()) {
        addPreloader(text_processing);
        var t = !1, e = "", a = $("#site_url").val();
        if ("oos" == $("#pos_type").val()
            ? (t = !0, "1" == $("#use_threed").val()
                    ? (t = !0, e = a + "sptr/" + $("#code").val() + "/oos/3d/" + $("#td_model").val() + "/sanalpostrPost.php?action=tdpost")
                    : "0" == $("#use_threed").val() && (t = !0, e = a + "sptr/" + $("#code").val() + "/oos/std/sanalpostrPost.php?action=tdpost")
            )
            : "api" == $("#pos_type").val() && ("1" == $("#use_threed").val()
                ? (t = !0, e = a + "sptr/" + $("#code").val() + "/api/3d/" + $("#td_model").val() + "/sanalpostrPost.php?action=tdpost")
                : "0" == $("#use_threed").val() && (t = !1, e = a + "index.php?route=payment/sptr_" + $("#code").val() + "/dopayment")), t) {
            w = 832, h = 696,
            wleft = (screen.width - w) / 2,
            wtop = (screen.height - h) / 2,
            wleft < 0 && (w = screen.width, wleft = 0),
            wtop < 0 && (h = screen.height, wtop = 0),
            payWindow = window.open("", "formpopup", "width=800, height=600, left=" + wleft + ", top=" + wtop + ", location=no, menubar=no, status=no, toolbar=no, scrollbars=no, resizable=no, titlebar=no"),
            payWindow.resizeTo(w, h),
            payWindow.moveTo(wleft, wtop),
            window.setTimeout("postFormPop('" + e + "')", 500);
            var r = setInterval(function () {
                payWindow.closed && (clearTimeout(r), $("#payWindowFog").remove(),
               removePreloader(),
                $("#result").html(""),
                $("#result").is(":visible") || $("#result").slideDown())
            }, 200); return !1
        }

        "0" == $("#use_threed").val() && $.ajax(
            {
                type: "post",
                url: e,
                data: $("#payform").serialize(),
                dataType: "json",
                success: function (t) {
                    0 == t.result
                        ? paymentOk() : 1 == t.result && paymentError(t.oid, t.errCode, t.errMsg)
                }
            })
    }
}
function postFormPop(t) {
    return document.payform.target = "formpopup",
        document.payform.action = t,
        document.payform.method = "post",
        document.payform.submit(),
        $('<div id="payWindowFog" style="display: none;">&nbsp;</div>').prependTo("body").css({ position: "fixed", top: 0, left: 0, width: $(window).width(), height: $(window).height(), opacity: 0, zIndex: 999 }), $("#payWindowFog").show(), $("#payWindowFog").click(function () { parent_disable() }),
        payWindow.focus(), !1
}
function paymentOk() {
    $("#loading").fadeIn(),
    void 0 !== payWindow && payWindow.close(),
    $("#payWindowFog").length && $("#payWindowFog").remove(),
    removePreloader(),
    err = '<span style="color:#04B404;"><strong>' + payment_success_header + "</strong></span><br><br>", $("#result").html(err),
    $("#result").is(":visible") || $("#result").slideDown(), $("#payresult").val("SUCCESS"),
    $("#payform").find(":input").each(function() {
        switch (this.type) {
            case "select-one":
            case "text":
                $(this).val("");
                break;
            case "checkbox":
            case "radio":
                this.checked = !1
        }
    }),
    $("#payform").attr("action", "index.php?route=payment/sptr_" + $("#code").val() + "/callback"),
    $("#payform").attr("method", "post"), $("#payform").removeAttr("target"),
    setTimeout('$("#payform").submit()', 500)
}
function paymentError(t, e, a) {
    void 0 !== window.payWindow && payWindow.close(),
    $("#payWindowFog").length && $("#payWindowFog").remove(),
    removePreloader(),
    err = '<table style="width:"><tr><td colspan="2"><label class="ccform_label" style="color:red;">' + payment_error_header + '</label></td></tr><tr><td colspan="2"><label class="ccform_label">' + text_transaction_details + '</label></td></tr><tr><td style="width:170px; font-weight:bold; font-size:12px;">' + text_order_number + ' : </td><td style="font-size:12px;">' + t + '</td></tr><tr><td style="width:170px; font-weight:bold; font-size:12px;">' + text_response_code + ' : </td><td style="font-size:12px;">' + e + '</td></tr><tr><td style="width:170px; font-weight:bold; font-size:12px;">' + text_response_desc + ' : </td><td style="font-size:12px;">' + unescape(a) + '</td></tr><tr><td colspan="2"><label class="ccform_label">' + text_contact_us + "</label></td></tr></table>", $("#payresult").val("ERROR"), setTimeout(function () { $("#result").html(err), $("#result").is(":visible") || $("#result").slideDown() }, 500)
}
function tdError() {
    void 0 !== window.payWindow && payWindow.close(),
    $("#payWindowFog").length && $("#payWindowFog").remove(),
    removePreloader(),
    err = '<table style="width:"><tr><td colspan="2"><label class="ccform_label" style="color:red;">' + payment_error_header + '</label></td></tr><tr><td colspan="2"><label class="ccform_label">' + text_transaction_details + '</label></td></tr><tr><td colspan="2"style="width:170px; font-weight:bold; font-size:13px; color:red;">' + text_td_error_desc + '</td></tr><tr><td colspan="2"><label class="ccform_label">' + text_contact_us + "</label></td></tr></table>", $("#payresult").val("ERROR"), setTimeout(function () { $("#result").html(err), $("#result").is(":visible") || $("#result").slideDown() }, 500)
}
function detectCCType(t, e) {
    (4 == t.length || 9 == t.length || 14 == t.length) && $("#cc_ccno").val($("#cc_ccno").val() + " ");
    var a = new Array; a[1] = "^4[0-9]{12}(?:[0-9]{3})?$", a[2] = "^5[1-5][0-9]{14}$", a[3] = "^6(?:011|5[0-9]{2})[0-9]{12}$", a[4] = "^3[47][0-9]{13}$"; var r = t.replace(/[^\d.]/g, ""), l = 0; $.each(a, function (t, e) { var a = new RegExp(e); a.test(r) && t > 0 && (l = t) }), 4 == l ? ($("#cc_ccno").attr("maxlength", "18"), 18 == t.length && document.getElementById(e).select()) : ($("#cc_ccno").attr("maxlength", "19"), 19 == t.length && document.getElementById(e).select()), l > 0 ? 1 == l ? $(".ccform_card_img").attr("src", "sptr/img/visa.png") : 2 == l ? $(".ccform_card_img").attr("src", "sptr/img/mastercard.png") : 4 == l && $(".ccform_card_img").attr("src", "sptr/img/amex.png") : $(".ccform_card_img").attr("src", "sptr/img/card.png"), $("#cc_cctype").val(l)
}
function detectCvv(t) {
     parseFloat($("#cc_cctype").val()) > 0 && 4 == parseFloat($("#cc_cctype").val()) ? $("#cc_cvv2").attr("maxlength", "4") : $("#cc_cvv2").attr("maxlength", "3")
}
function detectDel(t) {
     var e = event.keyCode || event.charCode; (8 == e || 46 == e) && (5 == $(t).val().length || 10 == $(t).val().length || 15 == $(t).val().length) && (value = $(t).val(), value = value.substr(0, value.length - 1), $(t).val(value))
}
function focusIt(t) {
     isNumber(t.value) || ($(t).css("font", ""), $(t).css("letter-spacing", ""), t.value = "")
}
function blurIt(t) {
     isNumber(t.value) || ($(t).css("font", "normal Verdana,sans-serif"), $(t).css("letter-spacing", "2.1pt"), "cc_ccno" == $(t).attr("id") ? $(t).val("•••• •••• •••• ••••") : "cc_cvv2" == $(t).attr("id") && $(t).val("•••"))
}
function isNumber(t) {
     var e = /[0-9]+/; return e.test(t)
}
function isLetter(t) {
     var e = /^[a-zA-Z\u0100-\u017F\u00A0-\u00FF\s]+$/; return e.test(t)
}
function parent_disable() {
     payWindow && !payWindow.closed && payWindow.focus()
}
$(function() {
    $("#cc_ccno").css("font", "normal Verdana,sans-serif"),
   $("#cc_ccno").css("letter-spacing", "2.1pt"),
    $("#cc_cvv2").css("font", "normal Verdana,sans-serif"),
    $("#cc_cvv2").css("letter-spacing", "4.1pt"),
    1 == parseFloat($("#use_instalments").val()) ?
    (instData = $.parseJSON($("#instalments").val()),
    instRow = instData[0], "" == instRow.rate ?
    rt = 0 : rt = parseFloat(instRow.rate).toFixed(2),
    $("#ord_curr").val() == $("#curr").val() ?
    (newTotalText = "", rateAmtText = "", rtAmt = parseFloat($("#rateamt").val()).toFixed(2),
    newTotal = parseFloat($("#orgTotal").val()) + parseFloat(rtAmt), "" != $("#curr_sym_left").val() &&
    (newTotalText += $("#curr_sym_left").val(), rateAmtText += $("#curr_sym_left").val()),
    newTotalText += newTotal.toFixed(2), rateAmtText += rtAmt, "" != $("#curr_sym_right").val() &&
    (newTotalText += $("#curr_sym_right").val(), rateAmtText += $("#curr_sym_right").val()),
    $("#newTotal").val(newTotal.toFixed(2)), $("#TotalAmount").html(newTotalText)) : (newTotalText = "",
    rateAmtText = "", rtAmt = parseFloat($("#rateamt").val()).toFixed(2), newTotal = parseFloat($("#orgTotal").val())
        + parseFloat(rtAmt), "" != $("#curr_sym_left").val() && (newTotalText += $("#curr_sym_left").val(),
    rateAmtText += $("#curr_sym_left").val()), newTotalText += newTotal.toFixed(2), rateAmtText += rtAmt,
    "" != $("#curr_sym_right").val() && (newTotalText += $("#curr_sym_right").val(), rateAmtText += $("#curr_sym_right").val()),
    $("#TotalAmount").html(newTotalText), newTotalText = "",
    rateAmtText = "", rtAmt = parseFloat($("#ordTotal").val()) / 100 * rt,
    rtAmt = rtAmt.toFixed(2), newTotal = parseFloat($("#ordTotal").val()) + parseFloat(rtAmt),
    "" != $("#ord_curr_sym_left").val() && (newTotalText += $("#ord_curr_sym_left").val(),
    rateAmtText += $("#ord_curr_sym_left").val()), newTotalText += newTotal.toFixed(2),
    rateAmtText += rtAmt, "" != $("#ord_curr_sym_right").val() && (newTotalText += $("#ord_curr_sym_right").val(),
    rateAmtText += $("#ord_curr_sym_right").val())), $("#newTotal").val(newTotal.toFixed(2)),
    $("#is_info").html($("#display_name").val() + ", " + $("#rate").val() + "% " + text_rate), $("#is_info").slideDown(),
    $(".checkout-product > table > tfoot > tr:last-child > td:eq(1)").html(newTotalText), "1" == $("#showTotal").val()
    && $(".checkout-product > table > tfoot > tr:last-child").prev("tr").find("td:eq(1)").html(rateAmtText),

    $.ajax({
        type: "post", url: "index.php?route=payment/sptr_" + $("#code").val() + "/settotal",
        data: "oid=" + $("#oid").val() + "&rateamt=" + rtAmt + "&ratetext=" + rateAmtText + "&newtotal=" + newTotal
            + "&newtotaltext=" + newTotalText, dataType: "html", success: function (t) { }
    }),
    prepareInsts()) :
    (rt = parseFloat($("#rate").val()).toFixed(2),
    $("#ord_curr").val() == $("#curr").val() ?
    (newTotalText = "", rateAmtText = "", rtAmt =
        parseFloat($("#rateamt").val()).toFixed(2),
    newTotal = parseFloat($("#orgTotal").val()) + parseFloat(rtAmt), "" != $("#curr_sym_left").val()
    && (newTotalText += $("#curr_sym_left").val(), rateAmtText += $("#curr_sym_left").val()),
    newTotalText += newTotal.toFixed(2), rateAmtText += rtAmt, "" != $("#curr_sym_right").val() &&
    (newTotalText += $("#curr_sym_right").val(), rateAmtText += $("#curr_sym_right").val()),
    $("#newTotal").val(newTotal.toFixed(2)), $("#TotalAmount").html(newTotalText)) :
    (newTotalText = "", rateAmtText = "", rtAmt = parseFloat($("#rateamt").val()).toFixed(2),
    newTotal = parseFloat($("#orgTotal").val()) + parseFloat(rtAmt), "" != $("#curr_sym_left").val()
    && (newTotalText += $("#curr_sym_left").val(), rateAmtText += $("#curr_sym_left").val()),
    newTotalText += newTotal.toFixed(2), rateAmtText += rtAmt, "" != $("#curr_sym_right").val() &&
    (newTotalText += $("#curr_sym_right").val(), rateAmtText += $("#curr_sym_right").val()),
    $("#TotalAmount").html(newTotalText), newTotalText = "", rateAmtText = "",
    rtAmt = parseFloat($("#ordTotal").val()) / 100 * rt, rtAmt = rtAmt.toFixed(2),
    newTotal = parseFloat($("#ordTotal").val()) + parseFloat(rtAmt), "" != $("#ord_curr_sym_left").val() &&
    (newTotalText += $("#ord_curr_sym_left").val(), rateAmtText += $("#ord_curr_sym_left").val()),
    newTotalText += newTotal, rateAmtText += rtAmt, "" != $("#ord_curr_sym_right").val() &&
    (newTotalText += $("#ord_curr_sym_right").val(), rateAmtText += $("#ord_curr_sym_right").val())),
    $("#is_info").html($("#display_name").val() + ", " + $("#rate").val() + "% " + text_rate), $("#is_info").slideDown(),
    $(".checkout-product > table > tfoot > tr:last-child > td:eq(1)").html(newTotalText), "1" == $("#showTotal").val() &&
    $(".checkout-product > table > tfoot > tr:last-child").prev("tr").find("td:eq(1)").html(rateAmtText),
    $("#newTotal").val(newTotal.toFixed(2)), $.ajax({
        type: "post", url: "index.php?route=payment/sptr_" +
            $("#code").val() + "/settotal", data: "oid=" + $("#oid").val() + "&rateamt=" + rtAmt + "&ratetext=" + rateAmtText
                + "&newtotal=" + newTotal + "&newtotaltext=" + newTotalText, dataType: "html", success: function (t) { }
    }))
});