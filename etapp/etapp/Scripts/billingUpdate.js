var uid = false;
var companyName = '';
var firstName = '';
var lastName = '';
var token = '';
var jsonCompany = false;

function closePage() {
    window.close();
}

function landingEvent() {
    try {
        bOk = true;
        uid = getParameterByName('uid');

        if (uid == '') {
            alert('Invalid call');
            bOk = false;
        }

        return bOk;
    }
    catch (err) {
        alert('landingEvent: ' + err.description);
    }
}

function loadCompanyInfo() {
    try {
        var isResultOk = false;
        var data = 'uid=' + uid;
        jsonCompany = getDb('getCompanyByUID', data);
        if (!_.isUndefined(jsonCompany)) {
            companyName = jsonCompany.companyName;
            firstName = jsonCompany.firstName;
            lastName = jsonCompany.lastName;
            token = jsonCompany.token;
            $('#companyName').html(jsonCompany.companyName);
            isResultOk = true
        }
        else {
            if (isResultOk == false) {
                alert('Could not retrieve the information of this account.');
            }
        }

        return isResultOk;

    }
    catch (err) {
        alert('loadCompanyInfo: ' + err.description);
    }
}

function saveBillingInfo_DEPRECATED() {
    try {
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

            var data = 'uid=' + uid + '&type=' + escape(ccType) + '&number=' + escape(ccNumber) + '&secCode=' + escape(ccSecCode) + '&expMonth=' + escape(ccMonth) + '&expYear=' + escape(ccYear) + '&fName=' + escape(ccFirstName) + '&lName=' + escape(ccLastName) +
                       '&street=' + escape(ccStreet) + '&city=' + escape(ccCity) + '&state=' + escape(ccState) + '&postalCode=' + escape(ccPostalCode) + '&countryCode=' + escape(ccCountry);

            var tmpJson = dbReadWrite('saveBillingInfoByUID', data, true, false);

            if (tmpJson.result == 'true') {
                location.href = 'billingUpdateThankYou.html';
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

function saveBillingInfo() {
    try {
        var params = 'token=' + token;
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
