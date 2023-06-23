var jsonCompany = false;
var jsonCC = false;

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
                if (validateUserAccess(37) == true) {
                    loadCompanyInfo();
                    $('#companyDiv').show();
                }
                break;
            case 2:
                if (validateUserAccess(38) == true) {
                    loadBillingInfo();
                    $('#billingDiv').show();
                }
                break;
        }
    }
    catch (err) {
        alert('showAccountDivs: ' + err.description);
    }
}

function loadCompanyInfo() {
    try {
        var isResultOk = false;
        var data = 't=' + getTokenCookie('ETTK');
        jsonCompany = dbReadWrite('getCompanyInfo', data, true, false);

        if (jsonCompany.companyInfo.length == 1) {
            var jsonItem = eval('(' + jsonCompany.companyInfo[0] + ')');

            if (jsonItem.result == true) {
                $('#companyName').val(jsonItem.companyName);
                $('#phone').val(jsonItem.phone);
                $('#website').val(jsonItem.website);
                $('#industry').val(jsonItem.industry);
                $('#street').val(jsonItem.street);
                $('#city').val(jsonItem.city);
                $('#state').val(jsonItem.state);
                $('#postalCode').val(jsonItem.postalCode);
                $('#cbxCountry').val(jsonItem.countryCode);

                isResultOk = true;
            }
        }

        if (isResultOk == false) {
            alert('Could not retrieve the information of this account.');
        }
    }
    catch (err) {
        alert('loadCompanyInfo: ' + err.description);
    }
}

function saveCompanyInfo() {
    try {
        var name = $('#companyName').val();
        var phone = $('#phone').val();
        var website = $('#website').val();
        var industry = $('#industry').val();
        var street = $('#street').val();
        var city = $('#city').val();
        var state = $('#state').val();
        var postalCode = $('#postalCode').val();
        var countryCode = $('#cbxCountry').val();

        var data = 't=' + getTokenCookie('ETTK') + '&name=' + escape(name) + '&phone=' + escape(phone) + '&website=' + escape(website) + '&industry=' + escape(industry) +
                   '&street=' + escape(street) + '&city=' + escape(city) + '&state=' + escape(state) + '&postalCode=' + escape(postalCode) + '&countryCode=' + escape(countryCode);

        var tmpJson = dbReadWrite('saveCompanyInfo', data, true, false);

        if (tmpJson.result == 'true') {
            alert('Information saved');
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
        var isResultOk = false;
        var data = 't=' + getTokenCookie('ETTK');
        jsonCC = dbReadWrite('getCCInfo', data, true, false);

        if (jsonCC.ccInfo.length == 1) {
            var jsonItem = eval('(' + jsonCC.ccInfo[0] + ')');

            if (jsonItem.result == true) {
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

                isResultOk = true;
            }
        }
    }
    catch (err) {
        alert('loadBillingInfo: ' + err.description);
    }
}

function saveBillingInfo() {
    try {
        var billingContact = $('#billingContact').val();
        var billingEmail = $('#billingEmail').val();
        var billingPhone = $('#billingPhone').val();

        var ccType = $('#ccType').val();
        var ccNumber = $('#ccNumber').val();
        var ccSecCode = $('#ccSecCode').val();
        var ccMonth = $('#ccExpMonth').val();
        var ccYear = $('#ccExpYear').val();
        var ccFirstName = $('#ccFirstName').val();
        var ccLastName = $('#ccLastName').val();
        var ccStreet = $('#ccStreet').val();
        var ccCity = $('#ccCity').val();
        var ccState = $('#ccState').val();
        var ccPostalCode = $('#ccPostalCode').val();
        var ccCountry = $('#ccCountry').val();

        var bOk = true;
        //Clean all errors...
        $('.inputError').each(function () {
            $(this).text('');
        });

        if (ccType == '0') {
            $('#err_ccType').text('Please select the Credit Card Type');
            bOk = false;
        }
        if (ccNumber == '') {
            $('#err_ccNumber').text('Please enter the Credit Card Number');
            bOk = false;
        }
        else {
            if (ccNumber.length <= 12) {
                $('#err_ccNumber').text('Invalid Credit Card Number');
                bOk = false;
            }
        }
        if (ccSecCode == '') {
            $('#err_ccSecCode').text('Please enter the Credit Card Security Code');
            bOk = false;
        }
        else {
            if (ccType == 'AMEX') {
                if (ccSecCode.length != 4) {
                    $('#err_ccSecCode').text('AMEX has a 4-digit Security Code.  Please correct.');
                    bOk = false;
                }
            }
        }
        if (ccMonth == '0') {
            $('#err_ccExpMonth').text('Invalid Expiration Month');
            bOk = false;
        }
        if (ccYear == '0') {
            $('#err_ccExpYear').text('Invalid Expiration Year');
            bOk = false;
        }
        if (ccFirstName.length == 0) {
            $('#err_ccFirstName').text('Invalid First Name');
            bOk = false;
        }
        if (ccLastName.length == 0) {
            $('#err_ccLastName').text('Invalid Last Name');
            bOk = false;
        }
        if (ccStreet.length == 0) {
            $('#err_ccStreet').text('Invalid Street');
            bOk = false;
        }
        if (ccCity.length == 0) {
            $('#err_ccCity').text('Invalid City');
            bOk = false;
        }
        if (ccState.length == 0) {
            $('#err_ccState').text('Invalid State');
            bOk = false;
        }
        if (ccPostalCode.length == 0) {
            $('#err_ccPostalCode').text('Invalid Postal Code');
            bOk = false;
        }
        if (bOk == true) {
            bOk = ValidateCreditCard(ccType, ccNumber);
            if (bOk == false) {
                alert('Please re-enter your Credit Card Number.');
            }
        }
        if (bOk == true) {

            var data = 't=' + getTokenCookie('ETTK') + '&billingContact=' + escape(billingContact) + '&billingEmail=' + escape(billingEmail) + '&billingPhone=' + escape(billingPhone) + '&type=' + escape(ccType) + '&number=' + escape(ccNumber) + '&secCode=' + escape(ccSecCode) + '&expMonth=' + escape(ccMonth) + '&expYear=' + escape(ccYear) + '&fName=' + escape(ccFirstName) + '&lName=' + escape(ccLastName) +
                       '&street=' + escape(ccStreet) + '&city=' + escape(ccCity) + '&state=' + escape(ccState) + '&postalCode=' + escape(ccPostalCode) + '&countryCode=' + escape(ccCountry);

            var tmpJson = dbReadWrite('saveBillingInfo', data, true, false);

            if (tmpJson.result == 'true') {
                alert('Information saved');
            }
            else {
                alert('Failed to save information.  Please try again.');
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
