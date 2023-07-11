; (function ($) {
    var handlers = {
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
                        }
                        else {
                            $(".new-address").parent().before(data.html);
                            aware.showMessage("Adres Başarıyla Oluşturuldu", "", "", "check-circle");
                        }
                        if ($(".address-container").length >= 5) { $(".new-address").hide(); }

                        //Refresh shipping methods
                        if (!isExistingRegion && $("#HasShipping").val() == 1) {
                            handlers.refreshShippingMethods(_districtID);
                        }
                    }
                    else {
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
                }
                else {
                    aware.showError("", "Adres Gösterilemiyor", "fa-minus-circle");
                }
            });
            return false;
        },
        onAddressSelected: function () {
            $(elem.addressWrapper).find(".address-type.shipping").removeClass("selected");
            $(this).find(".address-type.shipping").addClass("selected");

            if ($(elem.billingAddressWrapper + ".selected").length == 0) {
                $(elem.billingAddressWrapper).removeClass("selected");
                $(this).find(".billing").addClass("selected");
            }

            var regionInfo = $(this).data("region-info");
            handlers.toggleShippingMethods(regionInfo);
        },
        refreshShippingMethods: function (regionID) {
            var found = false;
            $(elem.shippingMethod).each(function () {
                var shippingRegions = ($(this).data("region-info") + "").split(",");
                found = $.inArray(regionID, shippingRegions) > -1;
                if (found) { return false; }
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
                    }
                    else {
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
        onBillingAddressSelected: function () {
            $(elem.billingAddressWrapper).removeClass("selected");
            $(this).addClass("selected");
            return false;
        },
        onBeforeApproveOrder: function () {
            if (!$(".cbx-sales-aggreement").hasClass("active")) {
                aware.showError("Devam etmek için ön bilgilendirme formunu ve mesafeli satış sözleşmesini okuyup onaylamalısınız!", "Uyarı", "commenting-o");
                return false;
            }

            var oid = $("#orderID").val();
            aware.showLoading("Sipariş tamamlanıyor..", true);
            $.post("/Order/Approve", { orderID: oid }, function (data) {
                if (data.success == 1) {
                    aware.hideDialog();
                    $(".new-order-modal").modal();
                    aware.delayedRefresh(3600, "siparis-detay-" + data.idinfo);
                }
                else {
                    aware.showError(data.message, "İşlem Başarısız", "fa-minus-circle");
                }
            });
            return false;
        },
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

            var paymentType = $(".payment-method.selected").data("payment-id");
            if ($(".payment-method.selected").html() == undefined || paymentType == undefined || paymentType == 0) {
                aware.showError("Devam etmek için ödeme yöntemi seçmelisiniz!", "Ödeme Yöntemi Seçmediniz", "exclamation-circle");
                return false;
            }

            var subPaymentType = 0;
            if (paymentType == 4 && $(".sub-payment").length > 0) { //Havale için alt bilgi seçmeli!
                subPaymentType = $(".sub-payment.selected").data("id");
                if (!subPaymentType || subPaymentType <= 0) {
                    aware.showError("Devam etmek için havale yapacağınız bankayı seçmelisiniz!", "Havale Yapılacak Banka Seçilmedi", "exclamation-circle");
                    return false;
                }
            }

            var oid = $("#orderID").val();
            var note = $(".order-note").val();
            var order = { ID: oid, ShippingAddressID: selectedAddress, BillingAddressID: billingID, ShippingMethodID: _shippingMethod, PaymentType: paymentType, SubPaymentType: subPaymentType, Note: note };

            $.post("/Order/EditOrder", order, function (data) {
                if (data.success == 1) {
                    aware.showLoading("Sipariş Onay Ekranına Yönlendiriliyorsunuz..", true);
                    aware.delayedRefresh(800, "siparis-onay");
                }
                else {
                    aware.showError(data.message, "İşlem Başarısız", "minus-circle");
                }
            });
            return false;
        },
        onDeleteAddress: function () {
            var addressID = $(this).parents("div.address-container:eq(0)").data("address-id");
            aware.confirm("Bu adresi silmek istediğinizden emin misiniz?", function () {
                $.post("/Address/DeleteAddress", { id: addressID }, function (data) {
                    if (data.success == 1) {
                        $(".address-container[data-address-id='" + addressID + "']").parent().remove();
                        if ($(".address-container").length < 5) { $(".new-address").removeClass("dn").show(); }
                        aware.showMessage("İşlem Başarılı", "Adresiniz Başarıyla Silindi", "fa-check-circle");
                    }
                    else {
                        aware.showError(data.message, "Adres Silinemedi", "minus-circle");
                    }
                });
            });
        },
        onCancelOrder: function () {
            var orderID = $(this).data("order-id");
            aware.confirm("Siparişinizi iptal etmek istediğinizden emin misiniz?", function () {
                $.post("/Order/CancelOrder", { id: orderID }, function (result) {
                    if (result.success == 1) {
                        aware.showMessage("Siparişiniz İptal Edildi", "", "", "check-circle");
                        aware.delayedRefresh(800);
                    }
                    else {
                        aware.showError(data.message, "İşlem Başarısız", "minus-circle");
                    }
                });
            });
        },
        onBankSelected: function () {
            $(elem.rowRemittanceBank).removeClass("selected");
            $(elem.rowRemittanceBank).find("td i").removeClass("fa-check-square").addClass("fa-square-o");

            $(this).addClass("selected");
            $(this).find("td i").addClass("fa-check-square").removeClass("fa-square-o");
            return false;
        },
        calculateCost: function (paymentName) {
            var shippingCost = parseFloat($("div.shipping-method.selected").data("shipping-cost")) || 0;
            var paymentCost = parseFloat($("div.payment-method.selected").data("payment-cost")) || 0;
            var orderBasketTotal = parseFloat($("#orderBasketTotal").val());
            var currency = $("#currency").val();

            if (paymentCost == 0) {
                $(".order-totals .payment-total-row").addClass("dn");
            } else {
                $(".order-totals .payment-total-row").removeClass("dn");
            }

            $(".order-totals .shipping-cost").html(shippingCost.toFixed(2).replace(".", ",") + " " + currency);
            $(".order-totals .payment-cost").html(paymentCost.toFixed(2).replace(".", ",") + " " + currency);
            $(".order-totals .gross-total").html((orderBasketTotal + shippingCost + paymentCost).toFixed(2).replace(".", ",") + " " + currency);
        },
        ready: function () {
            $(".order-approval,.order-approval .billing").click(function () { return false; });

            $("div.payment-method").click(function () {
                $("div.payment-method").removeClass("selected");
                $(this).addClass("selected");

                var name = $(this).find("span.name").html();
                $(".order-totals .payment-total-row .payment-name").html(name);
                handlers.calculateCost();
            });

            $("div.shipping-method").click(function () {
                $("div.shipping-method").removeClass("selected");
                $(this).addClass("selected");
                handlers.calculateCost();
            });

            //Seçili adres için kargo varsa gösterilsin
            var selectedShippingAddress = $(elem.addressWrapper).find(".address-type.shipping.selected");
            if ($(selectedShippingAddress).length > 0) {
                $(selectedShippingAddress).parents(elem.addressWrapper + ":eq(0)").trigger("click");
            }

            try {
                var chartList = $(".chart");
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
            catch (err) {

            }
        }
    };

    var elem = {
        addressModal: "#addressModal",
        btnDeleteAddress: ".btn-delete-address",
        btnEditAddress: "#addressModal .btn-edit-address",
        btnShowAddress: ".btn-show-address",
        addressWrapper: "div.address-container",
        billingAddressWrapper: "div.address-container .address-type.billing",
        btnContinueOrder: ".btn-continue-order",
        btnApproveOrder: ".btn-approve-order",
        btnCancelOrder: ".btn-cancel-order",
        shippingMethod: ".order-shipments .shipping-method",
        shippingMessage: ".order-shipments .shipping-message",
        rowRemittanceBank: ".table-remittance tr"
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
        { selector: elem.rowRemittanceBank, container: document, event: "click", handler: handlers.onBankSelected, data: {} }
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