var promoPrice = 0;
var promoPriceService = 0;
var promoPriceActivation = 0;
var promoPriceOBD = 0;
var promoPricePostedSL = 0;
var itm1 = 0;

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

function applyPromoCode() {
    try {
        promoPrice = 0;
        promoPriceService = 0;
        promoPriceOBD = 0;

        var promoCode = $('#promoCode').val();

        switch (promoCode) {
            case '43367':
                promoPrice = 159;
                alert('Valid Promotion Code!');
                break;
            case '16935':
                promoPrice = 169;
                alert('Valid Promotion Code!');
                break;
            case '65890':
                promoPrice = 149;
                alert('Valid Promotion Code!');
                break;
            case '37213':
                promoPrice = 139;
                alert('Valid Promotion Code!');
                break;
            case 'SPE129':
                promoPrice = 129;
                alert('Valid Promotion Code!');
                break;
            case 'STQ129':
                promoPrice = 129;
                alert('Valid Promotion Code!');
                break;
            case 'SPE119':
                promoPrice = 119;
                alert('Valid Promotion Code!');
                break;
            case 'AWQ119':
                promoPrice = 119;
                alert('Valid Promotion Code!');
                break;
            case 'BF99':
                promoPrice = 99;
                alert('Valid Promotion Code!');
                break;
            case 'SPE109':
                promoPrice = 109;
                alert('Valid Promotion Code!');
                break;
            case 'REW109':
                promoPrice = 109;
                alert('Valid Promotion Code!');
                break;
            case 'XMAS978':
                promoPrice = 109;
                alert('Valid Promotion Code!');
                break;
            default:
                promoPrice = 119;
        }

        calcForm();
    }
    catch (err) {
        alert('applyPromoCode: ' + err.description);
    }
}

function loadServicePrice() {
    try {
        var itm2 = 0.00;
        if ($('#cbxServices').length == 0) {
            itm2 = 17.99;
        }
        else {
            var serviceId = $('#cbxServices').val();
            switch (serviceId) {
                case "0":
                    itm2 = 0.00;
                    break;
                case "11":
                    itm2 = 9.99;
                    break;
                case "12":
                    itm2 = 15.99;
                    break;
                case "13":
                    itm2 = 17.99; //17.99
                    break;
                case "14":
                    itm2 = 19.99;
                    break;
                case "15":
                    itm2 = 13.99;
                    break;
                default:
                    itm2 = 0.00;
            }
        }
        return itm2;
    }
    catch (err) {
        alert('loadServicePrice: ' + err.description);
    }
}

function loadPrices(qty) {
    try {

        if (qty == undefined) {
            qty = 1;
        }

        if (itm1 == 0) {
            itm1 = 99.00; //129
            if (qty <= 2) {
                itm1 = 99.00; //129
            }
            else {
                if (qty <= 5) {
                    itm1 = 99.00; //129
                }
                else {
                    if (qty < 10) {
                        itm1 = 89.00; //119
                    }
                    else {
                        if (qty < 20) {
                            itm1 = 89.00; //109
                        }
                        else {
                            if (qty >= 20) {
                                itm1 = 89.00; //99
                            }
                            else {
                                itm1 = 89.00; //149
                            }
                        }
                    }
                }
            }
        }

        if (promoPrice > 0) {
            itm1 = promoPrice;
        }
        var itm2 = loadServicePrice(); //Service
        if (promoPriceService > 0) {
            itm2 = promoPriceService;
        }
        var itm3 = 0.00;  //Activation
        if (promoPriceActivation > 0) {
            itm3 = promoPriceActivation;
        }
        var itm4 = 15.00; //OBD Connector
        if (promoPriceOBD > 0) {
            itm4 = promoPriceOBD;
        }
        var itm5 = 4.00; //Posted Speed Limit
        if (promoPricePostedSL > 0) {
            itm4 = promoPricePostedSL;
        }

        $('.itm1').attr('data-price', itm1.toFixed(2)).html(itm1.toFixed(2));
        $('.itm2').attr('data-price', itm2.toFixed(2)).html(itm2.toFixed(2));
        $('.itm3').attr('data-price', itm3.toFixed(2)).html(itm3.toFixed(2));
        $('.itm4').attr('data-price', itm4.toFixed(2)).html(itm4.toFixed(2));
        $('.itm5').attr('data-price', itm5.toFixed(2)).html(itm5.toFixed(2));

    }
    catch (err) {
        alert('loadPrices: ' + err.description);
    }
}

function onEdit(obj) {
    try {
        $(obj).addClass('leftAligned');
        obj.select();
    }
    catch (err) {
        alert('onEdit: ' + err.description);
    }
}

function onEndEdit(obj) {
    try {
        $(obj).removeClass('leftAligned');
    }
    catch (err) {
        alert('onEndEdit: ' + err.description);
    }
}

function calcForm() {
    try {
        var qty = document.getElementById('qty').value;
        if (qty.length == 0) {
            qty = '0';
        }

        if (!(/^[0-9]+$/.test(qty))) {
            alert('Please enter a valid quantity');
        }
        else {
            qty = parseInt(qty);

            var price = 0;
            var itemTotal = 0;
            var orderSubtotal = 0;

            //Adjust price depending on qty
            loadPrices(qty);

            $('.orderItem').each(function () {
                price = $(this).find('.itemPrice').attr('data-price');
                if (price == undefined) {
                    price = 0;
                }
                itemTotal = price * qty;
                $(this).find('.enteredQty').html(qty);
                $(this).find('.itemTotal').html(itemTotal.toFixed(2));
                orderSubtotal = orderSubtotal + itemTotal;
            });

            $('.optionalOrderItem').each(function () {
                if ($(this).find('.optionalCheck').is(':checked')) {
                    price = $(this).find('.itemPrice').attr('data-price');
                    if (price == undefined) {
                        price = 0;
                    }
                    itemTotal = price * qty;
                    $(this).find('.enteredQty').html(qty);
                    $(this).find('.itemTotal').html(itemTotal.toFixed(2));
                    orderSubtotal = orderSubtotal + itemTotal;
                }
                else {
                    $(this).find('.enteredQty').html('0');
                    $(this).find('.itemTotal').html('0.00');
                };
            });

            $('.subTotal').html(orderSubtotal.toFixed(2));

            $('.shippingItem').find('.enteredQty').html(qty);
            price = $('#shippingOptions').val();

            itemTotal = price * 1;//qty;
            $('.shippingTotal').html(itemTotal.toFixed(2));

            orderSubtotal = orderSubtotal + itemTotal;
            $('.grandTotal').html(orderSubtotal.toFixed(2));
            var editionIndex = $('#cbxServices').prop('selectedIndex');
            var shippingIndex = $('#shippingOptions').prop('selectedIndex');

            //Sets cookies
            var exdate = new Date();
            var expiredays = 100;
            exdate.setDate(exdate.getDate() + expiredays);
            document.cookie = "ETQTY=" + qty + "; expires=" + exdate.toUTCString();
            document.cookie = "ETED=" + editionIndex + "; expires=" + exdate.toUTCString();
            document.cookie = "ETSH=" + shippingIndex + "; expires=" + exdate.toUTCString();
        }

    }
    catch (err) {
        alert('calcForm: ' + err.description);
    }
}

function loadForm(docId) {
    try {
        var etqty = document.cookie.match('(^|;) ?' + 'ETQTY' + '=([^;]*)(;|$)');
        var eted = document.cookie.match('(^|;) ?' + 'ETED' + '=([^;]*)(;|$)');
        var etsh = document.cookie.match('(^|;) ?' + 'ETSH' + '=([^;]*)(;|$)');
        var jsonDoc = false;

        if (docId == undefined) {
            docId = '';
        }

        var previousDoc = false;
        if (docId.length > 0) {
            var data = 'docId=' + docId;
            previousDoc = getDb('getDocQty', data);
        }

        if (etqty) {
            var qty = unescape(etqty[2]);
            if (previousDoc) {
                qty = previousDoc.qty;
            }
            $('#qty').val(qty);
            if (eted) {
                var editionIndex = unescape(eted[2]);
                $('#cbxServices').prop('selectedIndex', editionIndex);
            }
            if (etsh) {
                var shippingIndex = unescape(etsh[2]);
                if (previousDoc) {
                    shippingIndex = previousDoc.shippingOption;
                }
                $('#shippingOptions').prop('selectedIndex', shippingIndex);
            }

            loadPrices(qty);
            calcForm();
        }

    }
    catch (err) {
        alert('loadForm: ' + err.description);
    }
}

function copyShippingInfo() {
    try {
        $('#ccFirstName').val($('#firstName').val());
        $('#ccLastName').val($('#lastName').val());
        $('#ccStreet').val($('#street').val());
        $('#ccCity').val($('#city').val());
        $('#ccState').val($('#state').val());
        $('#ccPostalCode').val($('#postalCode').val());
    }
    catch (err) {
        alert('copyShippingInfo: ' + err.description);
    }
}

function anonFeedback() {
    try {
        var data = 'formId=' + escape($('#formId2').val()) +
                   '&msg=' + escape($('#message2').val());

        var result = saveWebForm(data);

        alert('Thank you!');

        $('#message2').val('');
    }
    catch (err) {
        alert('anonFeedback: ' + err.description);
    }
}

function contactUs() {
    try {
        var data = 'formId=' + escape($('#formId').val()) +
                   '&msg=' + escape($('#message').val()) +
                   '&fn=' + escape($('#firstName').val()) +
                   '&ln=' + escape($('#lastName').val()) +
                   '&ph=' + escape($('#phone').val()) +
                   '&email=' + escape($('#email').val()) +
                   '&repId=' + escape($('#repId').val());

        var result = saveWebForm(data);

        $('#message').val('');
        $('#firstName').val('');
        $('#lastName').val('');
        $('#email').val('');
        $('#phone').val('');
        $('#repId').val('');

        goTo('thank-you-contact.html');
    }
    catch (err) {
        alert('contactUs: ' + err.description);
    }
}

function getQuote() {
    try {
        var serviceId = 2;
        var qty = 0;
        qty = $('#qty').val();
        if (qty <= 0) {
            alert('Please enter a quantity');
            return;
        }

        var data = {
            formId: $('#formId').val(),
            qty: qty,
            serviceId: serviceId,
            ship: $('#shippingOptions').prop('selectedIndex'),
            fn: $('#firstName').val(),
            ln: $('#lastName').val(),
            email: $('#email').val(),
            ph: $('#phone').val(),
            co: $('#company').val()
        };
        data = JSON.stringify(data);
        var res = postDb('getQuote', data);
        if (res.isOk == true) {
            alert($('#firstName').val() + ' please check your inbox for our Quote!');
            $('#firstName').val('');
            $('#lastName').val('');
            $('#email').val('');
            $('#phone').val('');
            $('#company').val('');
        }
        else {
            alert('Sorry, we could not process your request.  Kindly try again.');
        }
    }
    catch (err) {
        alert('getQuote: ' + err.description);
    }
}

function buyNow() {
    try {
        //Valida the CC
        var isValidCC = false;
        if ($('#ccNumber').val().substring(0, 4) == '****') {
            isValidCC = true;
        }
        else {
            isValidCC = ValidateCreditCard();
            if (!isValidCC) {
                alert('Invalid Credit Card number');
                return;
            }
        }

        //Validate the email
        var isValidEmail = false;
        validateEmail($('#email').val());
        if (!validateEmail) {
            alert('Invalid Email Address');
            return;
        }

        var serviceId = 0;
        if ($('#cbxServices').length == 0) {
            serviceId = 2;
        }
        else {
            serviceId = $('#cbxServices').val();
            if (serviceId == '0') {
                alert('Please select a service edition');
                return;
            }
        }

        var qty = 0;
        qty = $('#qty').val();
        if (qty <= 0) {
            alert('Please enter a quantity');
            return;
        }

        var isOBDOption = false;
        try {
            if ($('#obdConnector').is(':checked')) {
                isOBDOption = true;
            }
        }
        catch (err) {
            isOBDOption = false;
        }

        var isPostedSLOption = false;
        try {
            if ($('#postedSpeedLimit').is(':checked')) {
                isPostedSLOption = true;
            }
        }
        catch (err) {
            isPostedSLOption = false;
        }

        var data = {
            formId: $('#formId').val(),
            qty: qty,
            serviceId: serviceId,
            isOBDOption: isOBDOption,
            isPostedSLOption: isPostedSLOption,
            ship: $('#shippingOptions').prop('selectedIndex'),
            fn: $('#firstName').val(),
            ln: $('#lastName').val(),
            email: $('#email').val(),
            ph: $('#phone').val(),
            cell: $('#cellPhone').val(),
            co: $('#company').val(),
            street: $('#street').val(),
            city: $('#city').val(),
            state: $('#state').val(),
            postal: $('#postalCode').val(),
            ccType: $('#ccType').val(),
            ccNo: $('#ccNumber').val(),
            ccSec: $('#ccSecCode').val(),
            ccMonth: $('#ccExpMonth').val(),
            ccYear: $('#ccExpYear').val(),
            ccFn: $('#ccFirstName').val(),
            ccLn: $('#ccLastName').val(),
            ccStreet: $('#ccStreet').val(),
            ccCity: $('#ccCity').val(),
            ccState: $('#ccState').val(),
            ccPostal: $('#ccPostalCode').val(),
            promoCode: $('#promoCode').val(),
            msg: $('#specialInstructions').val(),
            repId: $('#repId').val()
        };
        data = JSON.stringify(data);

        var url = 'ws.svc/saveWebForm';
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                $('#ccNumber').val('');
                $('#ccSecCode').val('');

                goTo('https://secure.easitrack.com/thank-you-order.html');
            },
            error: function (a, b, c) {
                alert('Sorry, something happend and we could not get your order.  Please try again.')
            }
        });

    }
    catch (err) {
        alert('buyNow: ' + err.description);
    }
}

function saveWebForm(data, isSLL) {
    try {
        if (isSLL == undefined) {
            isSLL = false;
        }

        var result = dbReadWrite('saveWebForm', data, true, false, isSLL);

        return result;
    }
    catch (err) {
        alert('saveWebForm: ' + err.description);
    }
}

function goTo(thisPage) {
    location.href = thisPage;
}

function loadUnsubscribeInfo() {
    try {
        var id = getParameterByName('id');
        if (id == undefined) {
            id = '';
        }
        if (id.length > 0) {
            var data = 'id=' + escape(id);
            var result = dbReadWrite('contactUnsubscribe', data, true, false);

            if (result) {
                if (result.info) {
                    if (result.info.length > 0) {
                        jsonInfo = eval('(' + result.info[0] + ')');
                        $('#contactName').html(jsonInfo.name);
                        $('#quote').html('"' + jsonInfo.quote + '"');
                    }
                }
            }
        }
    }
    catch (err) {
        alert('loadUnsubscribeInfo: ' + err.description);
    }
}

function loadCustomer(t) {
    try {
        var data = 'token=' + t;
        var jsonItem = getDb('getShoppingCartInfo', data);

        $('#firstName').val(jsonItem.fn);
        $('#lastName').val(jsonItem.ln);
        $('#email').val(jsonItem.email);
        $('#phone').val(jsonItem.ph);
        $('#company').val(jsonItem.co);
        $('#street').val(jsonItem.street);
        $('#city').val(jsonItem.city);
        $('#state').val(jsonItem.state);
        $('#postalCode').val(jsonItem.postalCode);
        $('#ccType').val(jsonItem.ccType);
        $('#ccNumber').val(jsonItem.ccNo);
        $('#ccExpMonth').val(jsonItem.ccMonth);
        $('#ccExpYear').val(jsonItem.ccYear);
        $('#ccFirstName').val(jsonItem.ccFn);
        $('#ccLastName').val(jsonItem.ccLn);
        $('#ccStreet').val(jsonItem.ccStreet);
        $('#ccCity').val(jsonItem.ccCity);
        $('#ccState').val(jsonItem.ccState);
        $('#ccPostalCode').val(jsonItem.ccPostalCode);
        $('#firstName').val(jsonItem.fn);

        loadPrices(1);
        itm1 = jsonItem.price;
        $('.itm1').attr('data-price', itm1.toFixed(2)).html(itm1.toFixed(2));
        calcForm();

        isResultOk = true;

    }
    catch (err) {
        alert('loadCustomer: ' + err.description);
    }
}

function getFamousQuote() {
    try {
        var data = 'type=' + escape('6C3BC03F-B6DC-44BD-A238-B4B52D59CC07');
        var result = dbReadWrite('getFamousQuote', data, true, false);
        if (result) {
            if (result.quote) {
                if (result.quote.length > 0) {
                    jsonInfo = eval('(' + result.quote[0] + ')');
                    $('#quote').html('"' + jsonInfo.quote + '"');
                }
            }
        }
    }
    catch (err) {
        alert('getFamousQuote: ' + err.description);
    }
}

function getAccessLink() {
    try {
        var data = 'formId=' + escape($('#formId').val()) +
                   '&fn=' + escape($('#firstName').val()) +
                   '&ln=' + escape($('#lastName').val()) +
                   '&ph=' + escape($('#phone').val()) +
                   '&email=' + escape($('#email').val()) +
                   '&repId=' + escape($('#repId').val());

        var result = saveWebForm(data);

        $('#firstName').val('');
        $('#lastName').val('');
        $('#email').val('');
        $('#phone').val('');
        $('#repId').val('');

        goTo('thank-you-live-demo.html');
    }
    catch (err) {
        alert('getAccessLink: ' + err.description);
    }
}

