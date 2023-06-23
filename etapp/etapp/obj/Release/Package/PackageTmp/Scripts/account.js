var jsonCompany = false;
var jsonCC = false;

function closePage() {
    window.close();
}

function landingEvent() {
    try {
        bOk = true;
        var t = getParameterByName('t');

        if (t == '') {
            alert('Invalid call');
            bOk = false;
        }
        else {
            setCookie('ETTK', t);
        }

        return bOk;
    }
    catch (err) {
        alert('landingEvent: ' + err.description);
    }
}

function hideAccountDivs() {
    try {
        $('#companyDiv').hide();
        $('#billingDiv').hide();
    }
    catch (err) {
        alert('hideAccountDivs: ' + err.description);
    }
}

function showAccountDivs(id) {
    try {
        hideAccountDivs();

        switch (id) {
            case 1:
                loadCompanyInfo();
                $('#companyDiv').show();
                break;
            case 2:
                loadBillingInfo();
                $('#billingDiv').show();
                break;
        }
    }
    catch (err) {
        alert('showAccountDivs: ' + err.description);
    }
}

function loadCompanyInfo() {
    try {
        var data = 'token=' + getTokenCookie('ETTK');
        var jsonItem = getDb('getCompanyInfo', data);

        $('#companyName').val(jsonItem.companyName);
        $('#phone').val(jsonItem.phone);
        $('#website').val(jsonItem.website);
        $('#industry').val(jsonItem.industry);
        $('#street').val(jsonItem.street);
        $('#city').val(jsonItem.city);
        $('#state').val(jsonItem.state);
        $('#postalCode').val(jsonItem.postalCode);
        $('#cbxCountry').val(jsonItem.countryCode);
    }
    catch (err) {
        alert('loadCompanyInfo: ' + err.description);
    }
}

function saveCompanyInfo() {
    try {
        var params = 'token=' + getTokenCookie('ETTK');
        var data = {
            companyName: $('#companyName').val(),
            phone : $('#phone').val(),
            website : $('#website').val(),
            industry : $('#industry').val(),
            street : $('#street').val(),
            city : $('#city').val(),
            state : $('#state').val(),
            postalCode : $('#postalCode').val(),
            countryCode : $('#cbxCountry').val()
        }

        data = JSON.stringify(data);
        var res = postDb('saveCompanyInfo', data, params);
        if (res.isOk == true) {
            alert('Information saved');
        }
        else {
            alert('We could not save your information.  Please try again or send it to us via email at support@easitrack.com.');
        }
    }
    catch (err) {
        alert('saveCompanyInfo: ' + err.description);
    }
}

function cancelCompanyInfo() {
    try {
        hideAccountDivs();
    }
    catch (err) {
        alert('cancelCompanyInfo: ' + err.description);
    }
}

function loadBillingInfo() {
    try {
        var data = 'token=' + getTokenCookie('ETTK');
        var jsonItem = getDb('getCCInfo', data);

        $('#billingContact').val(jsonItem.billingContact);
        $('#billingEmail').val(jsonItem.billingEmail);
        $('#billingPhone').val(jsonItem.billingPhone);
        $('#ccType').val(jsonItem.ccType);
        $('#ccNumber').val(jsonItem.ccNumber);
        $('#ccSecCode').val(jsonItem.ccSecCode);
        $('#ccExpMonth').val(jsonItem.ccExpMonth);
        $('#ccExpYear').val(jsonItem.ccExpYear);
        $('#ccFirstName').val(jsonItem.ccFirstName);
        $('#ccLastName').val(jsonItem.ccLastName);
        $('#ccStreet').val(jsonItem.ccStreet);
        $('#ccCity').val(jsonItem.ccCity);
        $('#ccState').val(jsonItem.ccState);
        $('#ccPostalCode').val(jsonItem.ccPostalCode);
        $('#ccCountry').val(jsonItem.ccCountryCode);

    }
    catch (err) {
        alert('loadBillingInfo: ' + err.description);
    }
}

function saveBillingInfo() {
    try {
        var params = 'token=' + getTokenCookie('ETTK');
        var data = {
            billingContact: $('#billingContact').val(),
            billingEmail: $('#billingEmail').val(),
            billingPhone: $('#billingPhone').val(),
            ccType: $('#ccType').val(),
            ccNumber: $('#ccNumber').val(),
            ccSecCode: $('#ccSecCode').val(),
            ccExpMonth: $('#ccExpMonth').val(),
            ccExpYear: $('#ccExpYear').val(),
            ccFirstName: $('#ccFirstName').val(),
            ccLastName: $('#ccLastName').val(),
            ccStreet: $('#ccStreet').val(),
            ccCity: $('#ccCity').val(),
            ccState: $('#ccState').val(),
            ccPostalCode: $('#ccPostalCode').val(),
            ccCountry: $('#ccCountry').val()
        };

        var bOk = true;
        //Clean all errors...
        $('.inputError').each(function () {
            $(this).text('');
        });

        if (data.billingContact == '' || data.billingEmail == '' || data.billingPhone == '') {
            alert('Please complete the Billing Contact, Billing Email, and Billing Phone information.');
            bOk = false;
        }

        if (data.ccType == '0') {
            $('#err_ccType').text('Please select the Credit Card Type');
            bOk = false;
        }
        if (data.ccNumber == '') {
            $('#err_ccNumber').text('Please enter the Credit Card Number');
            bOk = false;
        }
        else {
            if (data.ccNumber.length <= 12) {
                $('#err_ccNumber').text('Invalid Credit Card Number');
                bOk = false;
            }
        }
        if (data.ccSecCode == '') {
            $('#err_ccSecCode').text('Please enter the Credit Card Security Code');
            bOk = false;
        }
        else {
            if (data.ccType == 'AMEX') {
                if (data.ccSecCode.length != 4) {
                    $('#err_ccSecCode').text('AMEX has a 4-digit Security Code.  Please correct.');
                    bOk = false;
                }
            }
        }
        if (data.ccExpMonth == '0') {
            $('#err_ccExpMonth').text('Invalid Expiration Month');
            bOk = false;
        }
        if (data.ccExpYear == '0') {
            $('#err_ccExpYear').text('Invalid Expiration Year');
            bOk = false;
        }
        if (data.ccFirstName.length == 0) {
            $('#err_ccFirstName').text('Invalid First Name');
            bOk = false;
        }
        if (data.ccLastName.length == 0) {
            $('#err_ccLastName').text('Invalid Last Name');
            bOk = false;
        }
        if (data.ccStreet.length == 0) {
            $('#err_ccStreet').text('Invalid Street');
            bOk = false;
        }
        if (data.ccCity.length == 0) {
            $('#err_ccCity').text('Invalid City');
            bOk = false;
        }
        if (data.ccState.length == 0) {
            $('#err_ccState').text('Invalid State');
            bOk = false;
        }
        if (data.ccPostalCode.length == 0) {
            $('#err_ccPostalCode').text('Invalid Postal Code');
            bOk = false;
        }
        if (bOk == true) {
            if (data.ccNumber.substring(0, 4) != '****') {
                bOk = ValidateCreditCard(data.ccType, data.ccNumber);
                if (bOk == false) {
                    alert('Please re-enter your Credit Card Number.');
                }
            }
        }
        if (bOk == true) {

            data = JSON.stringify(data);
            var res = postDb('saveBillingInfo', data, params);
            if (res.isOk == true) {
                alert('Information saved');
            }
            else {
                alert('We could not save your information.  Please try again or call us to receive this information directly.');
            }
        }
    }
    catch (err) {
        alert('saveBillingInfo: ' + err.description);
    }
}

function cancelBillingInfo() {
    try {
        hideAccountDivs();
    }
    catch (err) {
        alert('cancelBillingInfo: ' + err.description);
    }
}
