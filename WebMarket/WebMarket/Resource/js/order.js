; (function ($) {
    var elem = {
        addressModal: "#addressModal",
        btnDeleteAddress: ".btn-delete-address",
        btnEditAddress: "#addressModal .btn-edit-address",
        btnShowAddress: ".btn-show-address",
        addressWrapper: "div.address-container",
        shippingAddressWrapper: "div.address-container .address-type.shipping",
        billingAddressWrapper: "div.address-container .address-type.billing",
        btnContinueOrder: ".btn-continue-order",
        btnApproveOrder: ".btn-approve-order",
        btnCancelOrder: ".btn-cancel-order",
        shippingMethod: ".order-shipments .shipping-method",
        shippingMessage: ".order-shipments .shipping-message",
        rowRemittanceBank: ".table-remittance tr",
        rowInstallment: ".installments-container tr",
        tabPayment: ".payment-type",
        txtCardNumber: "#CardNumber",
        bankPos: ".bank-pos-item",
        btnAllInstallments: ".btn-all-installments",
        lastBinNumber: ""
    };

    var handlers = {

        //ADDRESS
        editAddress: function () {
            if (aware.validate(".address-edit-dialog", "bottom")) {
                var addressID = $("#selectedAddress").val();
                var _name = $("#addressModal #Name").val();
                var _districtID = $("#addressModal #District").val();
                var _body = $("#addressModal #Body").val();
                var _phone = $("#addressModal #Phone").val();
                var postData = { id: addressID, name: _name, districtID: _districtID, description: _body, phone: _phone };

                aware.showLoading(undefined, true);
                $.post("/Address/EditAddress", postData, function (data) {
                    $(elem.addressModal).modal("hide");
                    if (data.success == 1) {
                        var addressID = $("#selectedAddress").val();
                        var isExistingRegion = $(".address-container[data-region-info*='_" + _districtID + "']").length > 0;
                        if (addressID > 0) {
                            $(".address-container[data-address-id='" + addressID + "']").find(".body span:eq(0)").html(data.html);
                            aware.showMessage("Adres Başarıyla Güncellendi", "", "", "check-circle");
                        } else {
                            $(".new-address").parent().before(data.html);
                            aware.showMessage("Adres Başarıyla Oluşturuldu", "", "", "check-circle");
                        }
                        if ($(".address-container").length >= 5) {
                            $(".new-address").hide();
                        }

                        //Refresh shipping methods
                        if (!isExistingRegion && $("#HasShipping").val() == 1) {
                            handlers.refreshShippingMethods(_districtID);
                        }
                    } else {
                        aware.showError(data.message, "İşlem Başarısız", "minus-circle");
                    }
                });
            }
        },
        showAddress: function () {
            var addressID = $(this).parents("div.address-container:eq(0)").data("address-id");
            $("#selectedAddress").val(addressID);

            aware.showLoading(undefined, true);
            $(elem.addressModal).find(".modal-title").html(addressID > 0 ? "Adres Düzenle" : "Yeni Adres Ekle");

            $.post("/Address/LoadAddress", { id: addressID }, function (data) {
                if (data.success == 1) {
                    aware.hideDialog();
                    $(elem.addressModal).find(".modal-body .address-form").html(data.html);
                    $(elem.addressModal).modal();
                    $("#District.district").selecto({ type: "filtered", url: "/Address/LoadDistricts" });
                } else {
                    aware.showError("", "Adres Gösterilemiyor", "fa-minus-circle");
                }
            });
            return false;
        },
        onAddressSelected: function () {
            var regionInfo = "0_0_0";
            if ($(this).find(".address-type.shipping").hasClass("selected")) {
                $(elem.shippingAddressWrapper).removeClass("selected");
            } else {
                $(elem.shippingAddressWrapper).removeClass("selected");
                $(this).find(".address-type.shipping").addClass("selected");
                regionInfo = $(this).data("region-info");
            }
            handlers.toggleShippingMethods(regionInfo);
        },
        onBillingAddressSelected: function () {
            if ($(this).hasClass("selected")) {
                $(elem.billingAddressWrapper).removeClass("selected");
            } else {
                $(elem.billingAddressWrapper).removeClass("selected");
                $(this).addClass("selected");
            }
            return false;
        },
        onDeleteAddress: function () {
            var addressID = $(this).parents("div.address-container:eq(0)").data("address-id");
            aware.confirm("Bu adresi silmek istediğinizden emin misiniz?", function () {
                $.post("/Address/DeleteAddress", { id: addressID }, function (data) {
                    if (data.success == 1) {
                        $(".address-container[data-address-id='" + addressID + "']").parent().remove();
                        if ($(".address-container").length < 5) {
                            $(".new-address").removeClass("dn").show();
                        }
                        aware.showMessage("İşlem Başarılı", "Adresiniz Başarıyla Silindi", "fa-check-circle");
                    } else {
                        aware.showError(data.message, "Adres Silinemedi", "minus-circle");
                    }
                });
            });
        },

        //SHIPPING
        refreshShippingMethods: function (regionID) {
            var found = false;
            $(elem.shippingMethod).each(function () {
                var shippingRegions = ($(this).data("region-info") + "").split(",");
                found = $.inArray(regionID, shippingRegions) > -1;
                if (found) {
                    return false;
                }
            });

            if (!found) {
                $.post("/Order/GetShippingMethods", { regionID: regionID }, function (result) {
                    if (result.success == 1) {
                        var html = "";
                        for (var i in result.data) {
                            var item = result.data[i];
                            if ($(elem.shippingMethod + "[data-shipping-id='" + item.id + "']").length == 0) {
                                html += "<div class='shipping-method dn' data-shipping-id='" + item.id + "' data-shipping-cost='" + item.price + "'";
                                html += "data-region-info='" + item.region + "'>";
                                html += "<i></i> <span>" + item.name + "</span></div>";
                            }
                        }
                        $(".order-shipments .shipping-method-container").append(html);
                    }
                });
            }
            return false;
        },
        toggleShippingMethods: function (regionInfo) {
            var count = 0;
            if (regionInfo != undefined && regionInfo.length > 0) {
                var city = regionInfo.split('_')[0];
                var county = regionInfo.split('_')[1];
                var district = regionInfo.split('_')[2];

                $(elem.shippingMethod).each(function () {
                    $(this).addClass("dn");
                    var shippingRegions = ($(this).data("region-info") + "").split(",");
                    var found = $.inArray("-1", shippingRegions) > -1 || $.inArray(district, shippingRegions) > -1 || $.inArray(county, shippingRegions) > -1 || $.inArray(city, shippingRegions) > -1;
                    if (found) {
                        $(this).removeClass("dn");
                        count++;
                    } else {
                        $(this).removeClass("selected").addClass("dn");
                    }
                });
            }

            if (count > 0) {
                $(elem.shippingMessage).addClass("dn");
            } else {
                $(elem.shippingMessage).html("<i class='fa fa-exclamation-triangle'></i> Seçtiğiniz teslimat adresi için tanımlı bir kargo bulunamadı!");
                $(elem.shippingMessage).removeClass("dn").addClass("warning");
            }
            return false;
        },

        //ORDER
        onBeforeSubmitOrder: function () {
            aware.showLoading("Sipariş Bilgileri Gönderiliyor..", true);
            var selectedAddress = $(".address-container .shipping.selected").parents(".address-container").data("address-id");
            if (selectedAddress == undefined || selectedAddress <= 0) {
                aware.showError("Devam etmek için teslimat adresi seçmelisiniz!", "Teslimat Adresi Seçmediniz", "exclamation-circle");
                return false;
            }

            var billingID = $(elem.billingAddressWrapper + ".selected").parents(".address-container").data("address-id");
            if (billingID == undefined || billingID <= 0) {
                aware.showError("Devam etmek için fatura adresi seçmelisiniz!", "Fatura Adresi Seçmediniz", "exclamation-circle");
                return false;
            }

            var _shippingMethod = 0;
            if ($("#HasShipping").val() == 1) {
                var selectedShippingMethod = $(".shipping-method.selected");
                if ($(selectedShippingMethod).html() == undefined || $(selectedShippingMethod).data("shipping-id") == 0) {
                    aware.showError("Devam etmek için kargo seçmelisiniz!", "Kargo Seçmediniz", "exclamation-circle");
                    return false;
                }
                _shippingMethod = $(selectedShippingMethod).data("shipping-id") || 0;
            }

            var paymentType = 0;
            var subPaymentType = 0;
            var oid = $("#currentOrderID").val();
            var note = $(".order-note").val();
            var order = { ID: oid, ShippingAddressID: selectedAddress, BillingAddressID: billingID, ShippingMethodID: _shippingMethod, PaymentType: paymentType, SubPaymentType: subPaymentType, Note: note };

            $.post("/Order/EditOrder", order, function (response) {
                if (response.success == 1) {
                    aware.showLoading("Ödeme Ekranına Yönlendiriliyorsunuz..", true);
                    aware.delayedRefresh(800, "siparis-odeme-" + response.idinfo);
                } else {
                    aware.showError(response.message, "İşlem Başarısız", "minus-circle");
                }
            });
            return false;
        },
        onBeforeApproveOrder: function () {
            var paymentType = $("#PaymentType").val();
            if (paymentType == undefined || paymentType == 0) {
                aware.showError("Devam etmek için ödeme yöntemi seçmelisiniz!", "Ödeme Yöntemi Seçmediniz", "exclamation-circle");
                return false;
            }

            var subPaymentType = 0;
            var installmant = 0;
            if (paymentType == 3) { //Kredi kartı ile işlem yapılmışsa
                var useOOS = $("#payment_creditcard").data("oos");
                if (!useOOS) {
                    var validated = aware.validate("#payment_creditcard");
                    if (validated && !handlers.isValidCardNumber($("#CardNumber"))) {
                        aware.showToastr("Kredi kartı numarası geçersiz!", "error");
                        validated = false;
                    }
                    if (!validated) { return false; }
                }

                subPaymentType = $("#SelectedPosID").val();
                installmant = $(".installment-options tr.selected").data("installment");
                if (!installmant || installmant <= 0) {
                    aware.showError("Devam etmek için taksit seçimi yapın!", "Taksit Seçilmedi", "exclamation-circle");
                    return false;
                }
            }
            else if (paymentType == 4 && $(".sub-payment").length > 0) { //Havale için alt bilgi seçmeli!
                subPaymentType = $(".sub-payment.selected").data("id");
                if (!subPaymentType || subPaymentType <= 0) {
                    aware.showError("Devam etmek için havale yapacağınız bankayı seçmelisiniz!", "Havale Yapılacak Banka Seçilmedi", "exclamation-circle");
                    return false;
                }
            }

            if (!$(".cbx-sales-aggreement").hasClass("active")) {
                aware.showError("Devam etmek için ön bilgilendirme formunu ve mesafeli satış sözleşmesini okuyup onaylamalısınız!", "Uyarı", "commenting-o");
                return false;
            }

            var orderID = $("#currentOrderID").val();
            aware.showLoading("Lütfen bekleyiniz..", true);
            $.post("/Order/SavePaymentInfo", { orderID: orderID, paymentType: paymentType, subPaymentType: subPaymentType, installment: installmant }, function (response) {
                if (response.success == 1) {
                    var remittanceMessage = $(".new-order-modal .remittance-message");
                    if (response.order.PaymentType == 3 || response.order.PaymentType == 5) { //KREDİ KARTI İLE VEYA GARANTI PAY İLE
                        handlers.processPayment(response.order);
                        return false;
                    }
                    else if (response.order.PaymentType == 4) { //HAVALE İLE
                        $(remittanceMessage).find(".order-no").html(response.order.UniqueID);
                        $(remittanceMessage).find("a.annotate").attr("href", "siparis-detay-" + response.order.UniqueID);
                        $(remittanceMessage).find(".bank-info img.bank-image").attr("src", "/resource/img/Bank/" + response.bank.ImageUrl);

                        var detail = "<p><b>" + response.bank.Name + ", " + response.bank.BranchName + "</b></p>";
                        detail += "<p>IBAN : " + response.bank.IBAN + "</p>";
                        detail += "<p>Hesap No : " + response.bank.AccountNumber + "</p>";

                        $(remittanceMessage).find(".bank-info .detail").html(detail);
                        $(remittanceMessage).removeClass("dn");
                        $(".new-order-modal .redirect-message").hide();
                    } else {
                        $(remittanceMessage).addClass("dn");
                        $(".new-order-modal .redirect-message").show();
                        aware.delayedRefresh(3600, "siparis-detay-" + response.order.UniqueID);
                    }

                    aware.hideDialog();
                    $(".new-order-modal").modal({ backdrop: 'static', keyboard: false });
                } else {
                    aware.showError(response.message, "İşlem Başarısız", "fa-minus-circle");
                }
                return false;
            });
            return false;
        },
        onCancelOrder: function () {
            var orderID = $(this).data("order-id");
            aware.confirm("Siparişinizi iptal etmek istediğinizden emin misiniz?", function () {
                $.post("/Order/CancelOrder", { id: orderID }, function (result) {
                    if (result.success == 1) {
                        aware.showMessage("Siparişiniz İptal Edildi", "", "", "check-circle");
                        aware.delayedRefresh(800);
                    } else {
                        aware.showError(data.message, "İşlem Başarısız", "minus-circle");
                    }
                });
            });
        },
        calculateCost: function (shippingCost, paymentCost) {
            var orderBasketTotal = parseFloat($("#orderBasketTotal").val());
            var currency = $("#currency").val();

            if (paymentCost == 0) {
                $(".order-totals .payment-total-row").addClass("dn");
            } else {
                $(".order-totals .payment-total-row").removeClass("dn");
            }

            $(".order-totals .shipping-cost").html(shippingCost.toFixed(2).replace(".", ",") + " " + currency);
            $(".order-totals .payment-cost").html(paymentCost.toFixed(2).replace(".", ",") + " " + currency);

            var grossTotal = (orderBasketTotal + shippingCost + paymentCost).toFixed(2);
            $(".order-totals .gross-total").html(grossTotal.replace(".", ",") + " " + currency);
        },

        //PAYMENT
        onBankSelected: function () {
            $(elem.rowRemittanceBank).removeClass("selected");
            $(elem.rowRemittanceBank).find("td i").removeClass("fa-check-square").addClass("fa-square-o");

            $(this).addClass("selected");
            $(this).find("td i").addClass("fa-check-square").removeClass("fa-square-o");
            return false;
        },
        onPosSelected: function () {
            if (!$(this).hasClass("selected")) {
                $(elem.bankPos).removeClass("selected");
                $(this).addClass("selected");

                var posID = $(this).data("pos-id");
                $("#SelectedPosID").val(posID);
                handlers.getInstallments("", posID);
            }
            return false;
        },
        onPaymentTypeSelected: function () {
            var btn = $(this);
            if ($(btn).hasClass("tab-cardatdoor")) {
                setTimeout(function () {
                    if ($(btn).hasClass("active")) {
                        $(".panel-cardatdoor").removeClass("dn");
                        $(".panel-atdoor").addClass("dn");
                    } else {
                        $(".panel-cardatdoor").addClass("dn");
                        $(".panel-atdoor").removeClass("dn");
                        $(".tab-at-door").click();
                    }
                }, 200);
            } else if ($(".payment-type.tab-cardatdoor").hasClass("active")) {
                $(".payment-type.tab-cardatdoor").removeClass("active");
                $(".panel-cardatdoor").addClass("dn");
                $(".panel-atdoor").removeClass("dn");
            }

            var paymentType = $(btn).data("payment-type");
            var paymentCost = parseFloat($(btn).data("payment-cost")) || 0;
            var shippingCost = parseFloat($("#shippingCost").val());
            var name = $(btn).data("payment-name");

            $("#PaymentType").val(paymentType);
            $(".order-totals .payment-total-row .payment-name").html(name);
            handlers.calculateCost(shippingCost, paymentCost);
        },
        processPayment: function (order) {
            var cardInfo=undefined;
            if (order.PaymentType != 5) {
                cardInfo = $("#payment_creditcard form").serializeObject();
            }
            cardInfo = cardInfo && cardInfo.CardNumber ? cardInfo : { CardNumber: "", ExpireMonth: "", ExpireYear: "", CVC: "" };

            var postData = {
                orderID: order.ID, posID: order.SubPaymentType, installment: order.Installment,
                CardNumber: cardInfo["CardNumber"].replace(/ /g, ""),
                ExpireMonth: cardInfo["ExpireMonth"],
                ExpireYear: cardInfo["ExpireYear"],
                CVC: cardInfo["CVC"]
            };

            $.post("/Order/ProcessPayment", postData, function (response) {
                if (response.success == 1) {
                    aware.hideDialog();
                    if (response.code == 1) { //3D form
                        $(".payment-form-container").html(response.value);
                        aware.showLoading("Bankaya Yönlendiriliyorsunuz..");
                        setTimeout(function() {
                            $(".payment-form-container #PaymentForm").submit();
                            aware.hideDialog();
                        }, 1000);
                    } else { //XML API sonucu
                        $(".new-order-modal .remittance-message").addClass("dn");
                        $(".new-order-modal .redirect-message").show();
                        aware.delayedRefresh(3600, "siparis-detay-" + order.UniqueID);
                        $(".new-order-modal").modal({ backdrop: "static", keyboard: false });
                    }
                }
                else {
                    aware.showError(response.message, "İşlem Başarısız", "fa-minus-circle");
                }
            });
        },

        //CREDIT CARD
        onCardNumberKeydown: function (e) {
            var value = $(this).val();
            if ((e.keyCode >= 48 && e.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105)) {
                if (value.length == 4 || value.length == 9 || value.length == 14) {
                    value = value + " ";
                }
                $(this).val(value);
            }
        },
        onCardNumberKeyup: function (e) {
            var value = $(this).val();
            if (value.length == 7) {
                var currentBin = value.replace(/ /g, "").substring(0, 6);
                if (currentBin != elem.lastBinNumber) {
                    elem.lastBinNumber = currentBin;
                    handlers.getInstallments(elem.lastBinNumber, 0);
                }
            } else if (value.length < 7) {
                $(".installments-container .installment-options").hide();
                $(".installments-container .description").show();
                $(".bank-logo").hide();
            } else {
                $(".installments-container .installment-options").show();
                $(".installments-container .description").hide();
                $(".bank-logo").show();
            }

            if (value.length == 19) {
                handlers.isValidCardNumber($(this));
            }
        },
        getInstallments: function (binNumber, posID) {
            var orderTotal = $("#orderTotal").val();
            $.post("/Order/GetCardInfo", { binNumber: binNumber, posID: posID, orderTotal: orderTotal }, function (response) {
                $(".installments-container .description").hide();
                $(".installments-container .installment-options").show();

                if (response.success) {
                    $(".installments-container .installment-options").replaceWith(response.html);
                    if (response.card && binNumber.length > 0) {
                        $("#SelectedPosID").val(response.card.PosID);
                        $(".bank-logo .brand").attr("class", "brand brand-" + response.card.Brand);
                        $(".bank-logo .bank").attr("class", "bank bank_" + response.card.BankType + "_" + response.card.CardType);
                        $(".bank-logo").show();
                    }
                } else {
                    $(".installments-container .installment-options").html("<p>Taksit seçeneği bulunamadı!</p>");
                }

                if (binNumber.length > 0 && (!response.success || !response.card)) {
                    $(".bank-logo .bank").attr("class", "bank");
                    $(".bank-logo .brand").attr("class", "brand");
                    $(".bank-logo").hide();
                }
            });
        },
        onInstallmentSelection: function () {
            $(elem.rowInstallment).removeClass("selected");
            $(this).addClass("selected");

            var installmentName = $(this).find("td.name").html();
            var installmenCount = $(this).data("installment");
            var commissionRate = parseFloat($(this).data("commission-rate")) || 0;
            var orderTotal = $("#orderTotal").val();
            var paymentCost = orderTotal * commissionRate / 100;

            var shippingCost = parseFloat($("#shippingCost").val());
            $(".order-totals .payment-total-row .payment-name").html("Kredi Kartı (" + installmentName + ")");
            handlers.calculateCost(shippingCost, paymentCost);
            return false;
        },
        isValidCardNumber: function (item) {
            var cardNumber = $(item).val().replace(/ /g, "");
            if (cardNumber.length) {
                var sum = 0;
                for (var i = 0; i < cardNumber.length; i++) {
                    var lhun = parseInt(cardNumber[i]) * (i % 2 == 0 ? 2 : 1)
                    sum += lhun >= 10 ? 1 + lhun % 10 : lhun;
                }

                if (sum % 10 == 0) {
                    $(item).removeClass("error");
                    return true;
                } else { //not a credit card
                    $(item).addClass("error");
                }
            }
            return false;
        },
        showAllInstallments: function () {
            var btn = $(this);
            var isLoaded = $(btn).data("loaded") == 1;
            if (!isLoaded) {
                var total = $(".order-totals .gross-total").data("price");
                aware.showLoading();
                $.post("/Order/GetAllInstallments", { productID: 0, total: total, mode: 2 }, function (data) {
                    if (data.success) {
                        aware.hideDialog();
                        $(".installments-modal .all-installments").html(data.html);
                        $(".installments-modal").modal("show");
                    } else {
                        aware.showError("Taksit seçenekleri getirilemedi!", "Hata", "comments");
                    }
                    $(btn).data("loaded", 1);
                });
            } else {
                $(".installments-modal").modal("show");
            }
            return false;
        },
        ready: function () {
            $("div.shipping-method").click(function () {
                $("div.shipping-method").removeClass("selected");
                $(this).addClass("selected");

                var shippingCost = parseFloat($("div.shipping-method.selected").data("shipping-cost")) || 0;
                handlers.calculateCost(shippingCost, 0);
            });

            //Seçili adres için kargo varsa gösterilsin
            var selectedShippingAddress = $(elem.addressWrapper).find(".address-type.shipping.selected");
            if ($(selectedShippingAddress).length > 0) {
                $(selectedShippingAddress).parents(elem.addressWrapper + ":eq(0)").trigger("click");
            }
            aware.instantValidate("#payment_creditcard");

            //Eğer OOS aktifse sadece tek sanal pos varsa seçili gelsin
            if ($(elem.bankPos).length == 1) {
                $(elem.bankPos + ":eq(0)").click();
            }
            $(elem.tabPayment + ".active").click();

            var hasPaymentResult = $(".order-result-modal").length > 0;
            if (hasPaymentResult) {
                $(".order-result-modal").modal("show");
            }

            try {
                var chartList = $(".chart");
                if (chartList.length > 0) {
                    $.each(chartList, function (index, item) {
                        var elem = chartList.eq(index);
                        var option = {};
                        option.barColor = elem.attr("data-barcolor");
                        option.trackColor = elem.attr("data-trackcolor");
                        option.scaleColor = elem.attr("data-scalecolor");
                        option.lineWidth = elem.attr("data-linewidth");
                        option.lineCap = elem.attr("data-linecap");
                        option.size = elem.attr("data-size");
                        elem.easyPieChart(option);
                    });
                }
            }
            catch (err) {

            }
        }
    };

    var eventMetaData = [
        { selector: undefined, container: document, event: "ready", handler: handlers.ready, data: {} },
        { selector: elem.btnDeleteAddress, container: document, event: "click", handler: handlers.onDeleteAddress, data: {} },
        { selector: elem.btnEditAddress, container: document, event: "click", handler: handlers.editAddress, data: {} },
        { selector: elem.btnShowAddress, container: document, event: "click", handler: handlers.showAddress, data: {} },
        { selector: elem.addressWrapper, container: document, event: "click", handler: handlers.onAddressSelected, data: {} },
        { selector: elem.billingAddressWrapper, container: document, event: "click", handler: handlers.onBillingAddressSelected, data: {} },

        { selector: elem.btnContinueOrder, container: document, event: "click", handler: handlers.onBeforeSubmitOrder, data: {} },
        { selector: elem.btnApproveOrder, container: document, event: "click", handler: handlers.onBeforeApproveOrder, data: {} },
        { selector: elem.btnCancelOrder, container: document, event: "click", handler: handlers.onCancelOrder, data: {} },

        { selector: elem.rowRemittanceBank, container: document, event: "click", handler: handlers.onBankSelected, data: {} },
        { selector: elem.tabPayment, container: document, event: "click", handler: handlers.onPaymentTypeSelected, data: {} },
        { selector: elem.bankPos, container: document, event: "click", handler: handlers.onPosSelected, data: {} },

        { selector: elem.rowInstallment, container: document, event: "click", handler: handlers.onInstallmentSelection, data: {} },
        { selector: elem.btnAllInstallments, container: document, event: "click", handler: handlers.showAllInstallments, data: {} },
        { selector: elem.txtCardNumber, container: document, event: "keydown", handler: handlers.onCardNumberKeydown, data: {} },
        { selector: elem.txtCardNumber, container: document, event: "keyup", handler: handlers.onCardNumberKeyup, data: {} },
        {
            selector: elem.txtCardNumber, container: document, event: "blur paste", handler: function () {
                setTimeout(function () { handlers.isValidCardNumber($("#CardNumber")); }, 200);
            }, data: {}
        }
    ];

    $(function () {
        for (var it in eventMetaData) {
            if (eventMetaData.hasOwnProperty(it)) {
                var item = eventMetaData[it];
                $(item.container).on(item.event, item.selector, item.data, item.handler);
            }
        }
    });
}(jQuery));

function paymentTest() {
    var postData = {
        orderID: 16, posID: 8, installment: 1, CardHolder: "osman sokuoğlu",
        CardNumber: "4508034508034509",
        ExpireMonth: 12,
        ExpireYear: 2018,
        CVC: "000"
    };

    $.post("/Order/ProcessPayment", postData, function (response) {
        if (response.success == 1) {
            aware.hideDialog();
            if (response.code == 1) { //3D form
                $(".payment-form-container").html(response.value);
                $(".payment-form-container #PaymentForm").submit();
            } else { //XML API sonucu
                $(".new-order-modal .remittance-message").addClass("dn");
                $(".new-order-modal .redirect-message").show();
                aware.delayedRefresh(3600, "siparis-detay-" + order.UniqueID);
                $(".new-order-modal").modal({ backdrop: 'static', keyboard: false });
            }
        }
        else {
            aware.showError(response.message, "İşlem Başarısız", "fa-minus-circle");
        }
    });
}