//const { report } = require("process");

var jsonReports = false;
var jsonEvents = false;
var geocoder = null;
var retroGeoLastAddress = null;
var retroGeoLastLoc = null;
var jsonArrayReport = [];

function loadTroubleshootingReportGrid(data) {
    try {
        var ds = new kendo.data.DataSource({
            data: data
        });

        $("#reportContent").kendoGrid({
            toolbar: ["excel"],
            excel: {
                fileName: "TroubleshootingReport.xlsx"
            },
            dataSource: ds,
            sortable: {
                mode: "single"
            },
            columns: [
                { field: "deviceId", title: 'Dev.ID' },
                { field: "deviceName", title: 'Name' },
                { field: "serialNumber", title: 'S/N' },
                { field: "lastUpdatedOn", title: 'Last Update' },
                { field: "noShowDays", title: 'No Show Days' },
                { field: "powerCut", title: 'Power Cut' },
                { field: "mainPowerRestored", title: 'Main Pwr Restored' },
                { field: 'illegalPowerUp', title: 'Illegal Pwr Up' },
                { field: "powerDown", title: 'Pwr Down' },
                { field: "ignOffGPS15", title: 'Ign.Off old GPS' },
                { field: 'ignOffSpeed10', title: 'Ign.Off w/Speed' },
                { field: 'powerUp', title: 'Pwr Up' },
                { field: 'powerOffBatt', title: 'Pwr Off w/Batt' },
                { field: 'totalEvents', title: 'Total Events' }
            ]
        });

        $("#waitingSpinner").hide();

    }
    catch (err) {
        alert('loadTroubleshootingReportGrid: ' + err.message);
    }
}

function executeTroubleshootingReport(deviceID, dateFrom, dateTo) {
    try {
        var token = getTokenCookie('ETTK');
        var noCache = Math.random();
        var data = 'deviceId=' + deviceId + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo + '&noCache=' + noCache;
        url = 'etrest.svc/getTroubleshootingReport/'
        $.ajax({
            type: "GET",
            url: url + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                loadTroubleshootingReportGrid(data);
            },
            error: function (x, y, z) {
                var a = 1;
            },
            async: true
        });

    }
    catch (err) {
        alert('executeTroubleshootingReport: ' + err.message);
    }
}

function printReport() {
    try {
        var inf = $('#reportContent').html();
        if (inf != '') {
            inf = '<div><button onclick="window.print()"><img src="images/printPage.gif" alt="Print EasiTrack Report"/></button></div>' + inf;
            var win = window.open("print_Report.html", 'popup', 'toolbar = no, status = no scrollbars=yes');
            win.document.write(inf);
            win.document.close(); // new line 
        }
        else {
            alert('Please execute the report first. Once executed, you can print it.');
        }
    }
    catch (err) {
        alert('printReport: ' + err.description);
    }
}

function getRetroGeoParams() {
    try {
        var reportId = getComboBoxSelectedOption(document.getElementById('cboReports'));
        var address = '';
        var lat = $('#retroGeoLatitude').val();
        var lng = $('#retroGeoLongitude').val();
        if (lat != 0 && lng != 0) {
            address = '(' + lat.toString() + ', ' + lng.toString() + ')'
        }
        else {
            lat = retroGeoLastLoc.lat();
            lng = retroGeoLastLoc.lng();
            address = retroGeoLastAddress;
            //$('#retroGeoLatitude').val(lat);
            //$('#retroGeoLongitude').val(lng);
        }

        var dateFrom = $('#dtpFrom').val();
        var dateTo = $('#dtpTo').val();
        var hourFrom = getComboBoxSelectedOption(document.getElementById('cboHourFrom'));
        var hourTo = getComboBoxSelectedOption(document.getElementById('cboHourTo'));
        var radius = $('#retroGeoRadius').val();
        var minTime = $('#retroGeoMinTime').val();

        var data = 't=' + getTokenCookie('ETTK') + '&rId=' + escape(reportId) + '&address=' + escape(address) + '&lat=' + escape(lat) + '&lng=' + escape(lng) + '&radius=' + escape(radius) + '&minTime=' + escape(minTime) + '&dateFrom=' + escape(dateFrom) + '&dateTo=' + escape(dateTo) + '&hourFrom=' + escape(hourFrom) + '&hourTo=' + escape(hourTo);

        return data;
    }
    catch (err) {
        alert('getRetroGeoParams: ' + err.description);
    }
}

function runReport() {
    
    try {
        
        var deviceID = '';
        var dateFrom = '';
        var dateTo = '';
        var data = '';
        var reportId = getComboBoxSelectedOption(document.getElementById('cboReports'));
        var isDelayed = $('#cboReports option:selected').attr('data-isDelayed');

        if (reportId == 16) {
            findRetroGeoLocation();
        }
        else {
            if (reportId == 17) {
                deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));
                dateFrom = $('#dtpFrom').val();
                dateTo = $('#dtpTo').val();
                $("#waitingSpinner").show();
                executeActivitySummaryReport(deviceId, dateFrom, dateTo);
            }
            else {
                if (reportId == 37) {
                    deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));
                    dateFrom = $('#dtpFrom').val();
                    dateTo = $('#dtpTo').val();
                    $("#waitingSpinner").show();
                    executeTroubleshootingReport(deviceId, dateFrom, dateTo);
                }
                else {
                    deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));
                    dateFrom = $('#dtpFrom').val();
                    dateTo = $('#dtpTo').val();
                    if (reportId == 1 || reportId == 39) {
                        dateTo = dateFrom;
                    }
                    if (isDelayed == 'true') {
                        if (isToday(dateFrom)) {
                            alert("This report processes today's data at night.  Data is available up until the day before.");
                            return;
                        }
                    }
                    data = getReportParams();

                    if (data != false) {
                        $('#reportContent').html('');

                        //Define here if this report will run interactive or in batch
                        if (isInteractiveReport(reportId, deviceId, dateFrom, dateTo)) {
                            $("#waitingSpinner").show();
                            data = data + '&isBatch=false';
                            //var data = 't=' + getTokenCookie('ETTK') + '&isBatch=false' + '&rId=' + escape(reportId) + '&dId=' + escape(deviceId) + '&df=' + escape(dateFrom) + '&dt=' + escape(dateTo) + '&tf=' + escape(hourFrom) + '&tt=' + escape(hourTo) + '&p=' + escape(param);
                            runReportGET(data);
                        }
                        else {
                            alert('The result of this report can be too long.  For this reason and to avoid server overloads, this report will be processed in batch and the result will be sent to your email in a couple of minutes.');
                            data = data + '&isBatch=true';
                            //var data = 't=' + getTokenCookie('ETTK') + '&isBatch=true' + '&rId=' + escape(reportId) + '&dId=' + escape(deviceId) + '&df=' + escape(dateFrom) + '&dt=' + escape(dateTo) + '&tf=' + escape(hourFrom) + '&tt=' + escape(hourTo) + '&p=' + escape(param);
                            saveReportBatch(data);
                        }
                    }
                }
            }
        }
    }
    catch (err) {
        alert('runReport: ' + err.description);
    }
}
//function runReport(reportId, isDelayed) {
//    
//    try {
//       //reportContent
//        var deviceID = '';
//        var dateFrom = '';
//        var dateTo = '';
//        var data = '';
//        if (reportId == null) {
//            reportId = getComboBoxSelectedOption(document.getElementById('cboReports'));
//            isDelayed = $('#cboReports option:selected').attr('data-isDelayed');
//        }

//        if (reportId == 16) {
//            findRetroGeoLocation();
//        }
//        else {
//            if (reportId == 17) {
//                deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));
//                dateFrom = $('#dtpFrom').val();
//                dateTo = $('#dtpTo').val();
//                $("#waitingSpinner").show();
//                executeActivitySummaryReport(deviceId, dateFrom, dateTo);
//            }
//            else {
//                if (reportId == 37) {
//                    deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));
//                    dateFrom = $('#dtpFrom').val();
//                    dateTo = $('#dtpTo').val();
//                    $("#waitingSpinner").show();
//                    executeTroubleshootingReport(deviceId, dateFrom, dateTo);
//                }
//                else {
//                    if (reportId != 28) {
//                        deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));
//                        dateFrom = $('#dtpFrom').val();
//                        dateTo = $('#dtpTo').val();
//                        if (reportId == 1 || reportId == 39) {
//                            dateTo = dateFrom;
//                        }
//                        if (isDelayed == 'true') {
//                            if (isToday(dateFrom)) {
//                                alert("This report processes today's data at night.  Data is available up until the day before.");
//                                return;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        data = getReportParams();
//                        if (data != false) {
//                            $('#reportContent').html('');

//                            //Define here if this report will run interactive or in batch
//                            if (isInteractiveReport(reportId, deviceId, dateFrom, dateTo)) {
//                                $("#waitingSpinner").show();
//                                data = data + '&isBatch=false';
//                                //var data = 't=' + getTokenCookie('ETTK') + '&isBatch=false' + '&rId=' + escape(reportId) + '&dId=' + escape(deviceId) + '&df=' + escape(dateFrom) + '&dt=' + escape(dateTo) + '&tf=' + escape(hourFrom) + '&tt=' + escape(hourTo) + '&p=' + escape(param);

//                                if (reportId == 28) {
//                                    runReportGETV2(data);
//                                } else {
//                                    runReportGET(data);
//                                }


//                            }
//                            else {
//                                alert('The result of this report can be too long.  For this reason and to avoid server overloads, this report will be processed in batch and the result will be sent to your email in a couple of minutes.');
//                                data = data + '&isBatch=true';
//                                //var data = 't=' + getTokenCookie('ETTK') + '&isBatch=true' + '&rId=' + escape(reportId) + '&dId=' + escape(deviceId) + '&df=' + escape(dateFrom) + '&dt=' + escape(dateTo) + '&tf=' + escape(hourFrom) + '&tt=' + escape(hourTo) + '&p=' + escape(param);
//                                saveReportBatch(data);
//                            }
//                        }

//                    }


//                }
//            }
//        }
//    }
//    catch (err) {
//        alert('runReport: ' + err.description);
//    }
//}

function isToday(ref) {
    try {
        var refDate = new Date(ref);
        var now = new Date();
        now.setHours(0, 0, 0, 0);
        if (now <= refDate) {
            return true;
        }
        else {
            return false;
        }
    }
    catch (err) {
    }
}

function saveReportBatchSuccess(data, textStatus, jqXHR) {
    try {
        alert('Batch queued successfuly');
    }
    catch (err) {
    }
}

function saveReportBatchFailure(jqXHR, textStatus, errorThrown) {
    try {
        alert('Please save again the repor');
    }
    catch (err) {
    }
}

function saveReportBatch(data) {
    try {
        dbReadWriteAsync('runReport', data, saveReportBatchSuccess, saveReportBatchFailure);
    }
    catch (err) {
        alert('saveReportBatch: ' + err.description);
    }
}

function isInteractiveReport(reportId, deviceId, dateFrom, dateTo) {
    try {
        var isInteractive = true;

        if (reportId == 1 || reportId == 39) {
            if (dateFrom < dateTo) {
                isInteractive = false;
            }
        }

        return isInteractive;
    }
    catch (err) {
        alert('isInteractiveReport: ' + err.description);
        return true;
    }
}

function runReportGETSuccess(data, textStatus, jqXHR) {
    try {

        var jsonObj = false;
        var alertFailure = true;

        if (textStatus == 'success') {
            if (($("string", data).text()) == 'failure') {
                ret = false;
            }
            else {
                jsonObj = eval('(' + $("string", data).text() + ')');
                if (jsonObj.result == 'failure') {
                    if (alertFailure == true) {
                        alert(jsonObj.error);
                    }
                    ret = false;
                }
                else {

                    var jsonResult = jsonObj;
                    if (jsonResult) {
                        var htmlResult = jsonResult.result;
                        $('#reportContent').html(htmlResult);
                    }
                }
            }

        }
        else {
            ret = false;
        }
        $("#waitingSpinner").hide();
    }
    catch (err) {
        alert('runReportGETSuccess: ' + err.description);
    }
}

function runReportGETFailure(jqXHR, textStatus, errorThrown) {
    try {
        alert('No data found in the selected timeframe.');
        $("#waitingSpinner").hide();
    }
    catch (err) {
        alert('Could not retrieve selected data. Try a small timeframe');
        //alert('runReportGETFailure: ' + err.description);
        $("#waitingSpinner").hide();
    }
}

function runReportGET(data) {
    try {
        dbReadWriteAsync('runReport', data, runReportGETSuccess, runReportGETFailure);
    }
    catch (err) {
        alert('runReportGET: ' + err.description);
    }
}

function exportReport() {
    try {
        var data = getReportParams();
        if (data != false) {
            window.location.replace('rptExport.aspx?' + data);
        }
    }
    catch (err) {
        alert('exportReport: ' + err.description);
    }
}

function newRecurrentReport() {
    try {

    }
    catch (err) {
        alert('newRecurrentReport: ' + err.description);
    }
}

function initializeDates() {
    try {
        $('#dtpFrom').val(getNow());
        $('#dtpTo').val(getNow());

        $("#dtpFrom").datepicker();
        $("#dtpTo").datepicker();

        loadHoursCombo(document.getElementById('cboHourFrom'));
        loadHoursCombo(document.getElementById('cboHourTo'));

        setComboBoxOption(document.getElementById('cboHourFrom'), 7);
        setComboBoxOption(document.getElementById('cboHourTo'), 18);
    }
    catch (err) {
        alert('initializeDates: ' + err.description);
    }
}

function loadReports() {

    try {
        var data = 't=' + getTokenCookie('ETTK');
        var jsonReports = dbReadWrite('getReports', data, true, false);

        var cbx = document.getElementById('cboReports');
        removeAllChildNodes(cbx);

        var opt0 = document.createElement('option');
        $(opt0).attr('data-id', 0);
        cbx.appendChild(opt0);
        var opt0TXT = document.createTextNode('[Select a Report]');
        opt0.appendChild(opt0TXT);

        for (var ind = 0; ind < jsonReports.reports.length; ind++) {
            var jsonItm = '';
            try {
                jsonItm = eval('(' + jsonReports.reports[ind] + ')');
            }
            catch (err) {
                jsonItm = jsonReports.reports[ind];
            }
            var cbxOption = document.createElement('option');
            cbx.appendChild(cbxOption);
            $(cbxOption).attr('data-id', jsonItm.id);
            $(cbxOption).attr('data-hasParam', jsonItm.hasParam);
            $(cbxOption).attr('data-paramName', jsonItm.paramName);
            $(cbxOption).attr('data-showDates', jsonItm.showDates);
            $(cbxOption).attr('data-showHours', jsonItm.showHours);
            $(cbxOption).attr('data-isDelayed', jsonItm.isDelayed);
            $(cbxOption).attr('data-showAllDevicesOption', jsonItm.showAllDevicesOption);
            var cbxOptionTxt = document.createTextNode(jsonItm.name);
            cbxOption.appendChild(cbxOptionTxt);
        }



        //var jsonEvents = dbReadWrite('getCompaniesDevicesEvents', data, true, false);
        //loadComboBox(jsonEvents.events, document.getElementById('pEvents'), 'Pick an event');
        dbReadWriteAsync('getCompaniesDevicesEvents', data, loadCompaniesEventsSuccess, loadCompaniesEventsFailure)

        var jsonDrivers = dbReadWrite('getDrivers', data, true, false);
        loadComboBox(jsonDrivers.drivers, document.getElementById('pDrivers'), 'Pick a driver');

        //var jsonGeofences = dbReadWrite('getGeofences', data, true, false);
        //loadComboBox(jsonGeofences.geofences, document.getElementById('pGeofences'), 'All Geofences');

        dbReadWriteAsync('getGeofences_All', data, loadGeofencesSuccess, loadGeofencesFailure)

    }
    catch (err) {
        alert('loadReports: ' + err.description);
    }
}

function loadCompaniesEventsSuccess(xml, textStatus) {

    try {
        if (textStatus == 'success') {
            if (($("string", xml).text()) == 'failure') {
                ret = false;
            }
            else {
                jsonObj = eval('(' + $("string", xml).text() + ')');
                if (jsonObj.result == 'failure') {
                    if (alertFailure == true) {
                        alert(jsonObj.error);
                    }
                    if (jsonObj.error == 'LOGOUT') {
                        logout();
                    }
                    else {
                        jsonEvents = false;
                    }
                }
                else {
                    jsonEvents = jsonObj;
                    loadComboBox(jsonObj.events, document.getElementById('pEvents'), 'Pick an event');
                }
            }
        }
        else {
            jsonEvents = false;
        }

    }
    catch (err) {
        alert('loadCompaniesEventsSuccess: ' + err.description);
    }
}

function loadCompaniesEventsFailure() {
    try {
        alert('Unable to load Events - please refresh screen with F5');
    }
    catch (err) {
        alert('loadCompaniesEventsFailure: ' + err.description);
    }
}

function loadGeofencesSuccess(xml, textStatus) {
    
    
    try {
        if (textStatus == 'success') {
            if (($("string", xml).text()) == 'failure') {
                ret = false;
            }
            else {
                jsonObj = eval('(' + $("string", xml).text() + ')');
                if (jsonObj.result == 'failure') {
                    if (alertFailure == true) {
                        console.log(jsonObj.error);
                    }
                    if (jsonObj.error == 'LOGOUT') {
                        logout();
                    }
                }
                else {
                    //loadComboBox(jsonObj.geofences, document.getElementById('pGeofences'), 'All Geofences');
                    loadComboBox(jsonObj, document.getElementById('pGeofences'), 'All Geofences');
                }
            }
        }

    }
    catch (err) {
        console.log('loadGeofencesSuccess: ' + err.description);
    }
}

function loadGeofencesFailure() {
    try {
        alert('Unable to load Geofences - please refresh screen with F5');
    }
    catch (err) {
        alert('loadGeofencesFailure: ' + err.description);
    }
}

function loadDevices() {

    try {
        getDevices();
        loadComboBox(jsonDevices.myDevices, document.getElementById('cboDevices'), 'All Devices');
    }
    catch (err) {
        alert('loadDevices: ' + err.description);
    }
}

function loadDevicesCombo(withAllOption) {
    try {
        if (withAllOption == true || withAllOption == 'true') {
            loadComboBox(jsonDevices.myDevices, document.getElementById('cboDevices'), 'All Devices', false);
        }
        else {
            loadComboBox(jsonDevices.myDevices, document.getElementById('cboDevices'), '', true);
        }
    }
    catch (err) {
        alert('loadDevices: ' + err.description);
    }
}

function hideParams() {
    try {
        $('#paramSpeedLimit').hide();
        $('#paramIdleLimit').hide();
        $('#paramMinTime').hide();
        $('#paramEventCode').hide();
        $('#paramDriverID').hide();
        $('#paramGeofenceID').hide();
        $('#paramRetroGeo').hide();
        $('.paramDates').hide();
        $('#paramHours').hide();
    }
    catch (err) {
        alert('hideParams: ' + err.description);
    }
}

function changeReports(obj) {

    try {
        var selectedIndex = obj.selectedIndex;
        var id = obj[selectedIndex].getAttribute('data-id');
        var hasParam = obj[selectedIndex].getAttribute('data-hasParam');
        var paramName = obj[selectedIndex].getAttribute('data-paramName');
        var showDates = obj[selectedIndex].getAttribute('data-showDates');
        var showHours = obj[selectedIndex].getAttribute('data-showHours');
        var showAllDevicesOption = obj[selectedIndex].getAttribute('data-showAllDevicesOption');

        hideParams();

        loadDevicesCombo(showAllDevicesOption)

        $('#paramDeviceID').show();

        if (id == '1' || id == '39') {
            //All events report runs for a single day
            $('.paramDates').show();
            $('.paramDatesLabels').hide();
            $('.paramDateTo').hide();
            $('#paramHours').show();
        }
        else {
            if (showDates == 'true') {
                $('.paramDatesLabels').show();
                $('.paramDates').show();
                $('.paramDateTo').show();
            }
            if (showHours == 'true') {
                $('#paramHours').show();
            }

            //        switch (id) {
            //            case 17, "17":
            //                $('#paramHours').hide();
            //                break;
            //            default:
            //                $('#paramHours').show();
            //                break;
            //        }

            if (hasParam == 'true') {
                switch (paramName.toLowerCase()) {
                    case 'eventcode':
                        $('#paramEventCode').show();
                        break;
                    case 'speedlimit':
                        $('#paramSpeedLimit').show();
                        break;
                    case 'mintime':
                        $('#paramMinTime').show();
                        break;
                    case 'idlelimit':
                        $('#paramIdleLimit').show();
                        break;
                    case 'driverid':
                        $('#paramDriverID').show();
                        break;
                    case 'geofenceid':
                        $('#paramGeofenceID').show();
                        $('#paramMinTime').show();
                        break;
                    case 'retrogeo':
                        $('#paramRetroGeo').show();
                        $('#paramDeviceID').hide();
                        break;
                }
            }
        }

    }
    catch (err) {
        alert('changeReports: ' + err.description);
    }
}

function changeDevices(obj) {
    try {

    }
    catch (err) {
        alert('changeDevices: ' + err.description);
    }
}

//================================================================
//--- FIND LOCATION

function geocodeLocationCallback(bAddressFound) {
    try {
        if (bAddressFound == true) {

            var data = getRetroGeoParams();

            if (data != false) {
                $('#reportContent').html('');
                $("#waitingSpinner").show();
                dbReadWriteAsync('runReportRetroGeo', data, runReportGETSuccess, runReportGETFailure);
            }
        }
        else {
            alert('This address was not found.  Please try again.');
        }
    }
    catch (err) {
        alert('geocodeLocationCallback: ' + err.description);
    }
}

function geocodeLocation(address, callbackFunc) {
    try {
        var bFound = false;

        if (!geocoder) {
            geocoder = new google.maps.Geocoder();
        }

        var geocoderRequest = {
            address: address
        };

        geocoder.geocode(geocoderRequest, function (results, status) {
            //Handle the response here...
            switch (status) {
                case google.maps.GeocoderStatus.OK:
                    //OK The response contains a valid GeocoderResponse.
                    if (status == google.maps.GeocoderStatus.OK) {
                        retroGeoLastAddress = address;
                        retroGeoLastLoc = results[0].geometry.location;
                        bFound = true;
                    }

                    break;
                case google.maps.GeocoderStatus.ERROR:
                    //ERROR There was a problem contacting the Google servers.
                    break;
                case google.maps.GeocoderStatus.INVALID_REQUEST:
                    //INVALID_REQUEST This GeocoderRequest was invalid.
                    break;
                case google.maps.GeocoderStatus.OVER_QUERY_LIMIT:
                    //OVER_QUERY_LIMIT The webpage has gone over the requests limit in too short a period of time.
                    break;
                case google.maps.GeocoderStatus.REQUEST_DENIED:
                    //REQUEST_DENIED The webpage is not allowed to use the geocoder.
                    break;
                case google.maps.GeocoderStatus.UNKNOWN_ERROR:
                    //UNKNOWN_ERROR A geocoding request could not be processed due to a server error. The request may succeed if you try again.
                    break;
                case google.maps.GeocoderStatus.ZERO_RESULTS:
                    //ZERO_RESULTS No result was found for this GeocoderRequest.
                    break;
            }

            if (callbackFunc) {
                callbackFunc(bFound);
            }
            else {
                return bFound;
            }

        });
    }
    catch (err) {
        alert('geocodeLocation: ' + err.description);
    }
}

function findRetroGeoLocation() {
    try {
        var address = $('#retroGeoAddress').val();

        if (address.length > 0) {
            //Avoid search of very same address more than once in a row
            var bGetLoc = false;
            var bFound = false;
            if (retroGeoLastAddress != null) {
                if (address.toUpperCase() != retroGeoLastAddress.toUpperCase()) {
                    retroGeoLastAddress = address;
                    retroGeoLastLoc = null;
                    bGetLoc = true;
                }
                else {
                    bFound = true;
                }
            }
            else {
                retroGeoLastAddress = address;
                retroGeoLastLoc = null;
                bGetLoc = true;
            }

            if (bGetLoc == true) {
                bFound = geocodeLocation(address, geocodeLocationCallback);
            }
            else {
                if (bFound == true) {
                    geocodeLocationCallback(bFound);
                }
            }
        }
        else {
            var lat = $('#retroGeoLatitude').val();
            var lng = $('#retroGeoLongitude').val();
            if (lat != 0 && lng != 0) {
                bFound = true;
                geocodeLocationCallback(bFound);
            }
            else {
                alert("Please enter either an address or a location's Latitude/Longitude");
            }
        }

    }
    catch (err) {
        alert('findRetroGeoLocation: ' + err.description);
    }
}



function runReportGETV2(data) {
    var array = [];
    try {
        array = dbReadWriteAsyncNew('getReportNew', data, runReportGETSuccess, runReportGETFailure);
    }
    catch (err) {
        alert('getReportNew: ' + err.description);
    }
    return array;
}
function getLoadReports() {

    var array = [];
    var obj = {};
    var data = 't=' + getTokenCookie('ETTK');
    var jsonReports = dbReadWrite('getReports', data, true, false);

    for (var ind = 0; ind < jsonReports.reports.length; ind++) {
        var jsonItm = '';
        try {
            jsonItm = eval('(' + jsonReports.reports[ind] + ')');
            obj = { id: jsonItm.id, name: jsonItm.name, hasParam: jsonItm.hasParam, paramName: jsonItm.paramName, showDates: jsonItm.showDates, showHours: jsonItm.showHours, isDelayed: jsonItm.isDelayed, showAllDevicesOption: jsonItm.showAllDevicesOption }
            array.push(obj);
        }
        catch (err) {
            jsonItm = jsonReports.reports[ind];

        }

    }
    return array;

}
function getLoadSensors(arraysensors) {

    var array = [];
    var obj = {};
    var data = 't=' + getTokenCookie('ETTK');
    var jsonReports = dbReadWrite('getReports', data, true, false);

    for (var ind = 0; ind < arraysensors.length; ind++) {
        var jsonItm = '';
        try {
            jsonItm = eval('(' + jsonReports.reports[ind] + ')');
            obj = { id: jsonItm.id, name: jsonItm.name, hasParam: jsonItm.hasParam, paramName: jsonItm.paramName, showDates: jsonItm.showDates, showHours: jsonItm.showHours, isDelayed: jsonItm.isDelayed, showAllDevicesOption: jsonItm.showAllDevicesOption }
            array.push(obj);
        }
        catch (err) {
            jsonItm = jsonReports.reports[ind];

        }

    }
    return array;

}
function getLoadDevices() {

    var array = [];
    var obj = {};
    var data = 't=' + getTokenCookie('ETTK');
    getDevices();
    var jsonDevices1 = jsonDevices.myDevices;


    for (var ind = 0; ind < jsonDevices1.length; ind++) {
        var jsonItm = '';
        try {
            jsonItm = eval('(' + jsonDevices1[ind] + ')');
            obj = { id: jsonItm.id, deviceId: jsonItm.deviceId, name: jsonItm.name, latitude: jsonItm.latitude, longitude: jsonItm.longitude }
            array.push(obj);
        }
        catch (err) {
            jsonItm = jsonDevices1[ind];
        }

    }
    return array;

}
function getLoadCompaniesDevicesEvents() {

    var array = [];
    var obj = {};
    var data = 'token=' + getTokenCookie('ETTK');
    //getDevices();
    array = dbReadCompaniesDevicesEvents(data);
    if (array.length > 0) {
        array = JSON.parse(dbReadCompaniesDevicesEvents(data));
    }


    //
    //for (var ind = 0; ind < jsonDevices1.length; ind++) {
    //    var jsonItm = '';
    //    try {
    //        jsonItm = eval('(' + jsonDevices1[ind] + ')');
    //        obj = { id: jsonItm.id, deviceId: jsonItm.deviceId, name: jsonItm.name }
    //        array.push(obj);
    //    }
    //    catch (err) {
    //        jsonItm = jsonDevices1[ind];

    //    }

    //}
    return array;

}
function getLoadGeofences() {

    var array = [];
    var obj = {};
    var data = 'token=' + getTokenCookie('ETTK');
    //getDevices();
    array = dbReadGeofences(data);
    if (array.length > 0) {
        array = JSON.parse(dbReadGeofences(data));
    }

    return array;

}
function getLoadDrivers() {

    var array = [];
    var obj = {};
    var data = '';
    try {
        data = 'token=' + getTokenCookie('ETTK');
        //getDevices();    
        array = JSON.parse(dbReadDrivers(data));
    } catch (error) {
        console.log("Not Drivers: " + error.message)
    }


    return array;

}
function ddlCreateReport(arrayList) {

    $("#report").kendoDropDownList({
        dataTextField: "name",
        dataValueField: "id",
        dataSource: arrayList,
        select: onSelectddlCreateReport
    });
}
function ddlCreateSensors(arrayList) {

    $("#sensors").kendoDropDownList({
        dataTextField: "name",
        dataValueField: "sensorid",
        dataSource: arrayList,
        select: onSelectddlCreateSensors
    });
}
function ddlCreateDevice(arrayList) {
    $("#device").kendoDropDownList({
        dataTextField: "name",
        dataValueField: "id",
        dataSource: arrayList,
        select: onSelectddlCreateDevice
    });
}
function ddlCreateEventCode(arrayList) {
    $("#pEvents").kendoDropDownList({
        dataTextField: "Name",
        dataValueField: "EventCode",
        dataSource: arrayList,
        select: onSelectddlCreateEventCode
    });
}
function ddlCreateGeofences(arrayList) {
    $("#pGeofences").kendoDropDownList({
        dataTextField: "Name",
        dataValueField: "Id",
        dataSource: arrayList,
        select: onSelectddlCreateGeofences
    });
}
function ddlCreateDrivers(arrayList) {
    $("#pDrivers").kendoDropDownList({
        dataTextField: "Name",
        dataValueField: "Id",
        dataSource: arrayList,
        select: onSelectddlCreateDrivers
    });
}
function onSelectddlCreateReport(e) {
    hideParams();
    //if ("kendoConsole" in window) {

    try {
        var select = $(e.event)
        if (e.dataItem) {
            var dataItem = e.dataItem;
            IdReport = dataItem.id;
            hasParam = dataItem.hasParam;
            paramName = dataItem.paramName;
            if (hasParam) {
                switch (paramName.toLowerCase()) {
                    case 'eventcode':
                        $('#paramEventCode').show();
                        break;
                    case 'speedlimit':
                        $('#paramSpeedLimit').show();
                        break;
                    case 'mintime':
                        $('#paramMinTime').show();
                        break;
                    case 'idlelimit':
                        $('#paramIdleLimit').show();
                        break;
                    case 'driverid':
                        $('#paramDriverID').show();
                        break;
                    case 'geofenceid':
                        $('#paramGeofenceID').show();
                        $('#paramMinTime').show();
                        break;
                    case 'retrogeo':
                        $('#paramRetroGeo').show();
                        $('#paramDeviceID').hide();
                        break;
                }
            }

            //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");
        } else {
            //kendoConsole.log("event :: select");
        }

    } catch (error) {
        alert(error.message)
    }

    // }
}
function onSelectddlCreateDevice(e) {
    //if ("kendoConsole" in window) {
    var select = $(e.event)
    if (e.dataItem) {
        var dataItem = e.dataItem;
        IdDevice = dataItem.id;
        //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");
    } else {
        //kendoConsole.log("event :: select");
    }
    // }
}
function onSelectddlCreateEventCode(e) {

    //if ("kendoConsole" in window) {
    var select = $(e.event)
    if (e.dataItem) {
        var dataItem = e.dataItem;
        IdEventCode = dataItem.EventCode;
        //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");
    } else {
        //kendoConsole.log("event :: select");
    }
    // }
}
function onSelectddlCreateGeofences(e) {

    //if ("kendoConsole" in window) {
    var select = $(e.event)
    if (e.dataItem) {
        var dataItem = e.dataItem;
        IdGeofences = dataItem.Id;
        //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");
    } else {
        //kendoConsole.log("event :: select");
    }
    // }
}
function onSelectddlCreateDrivers(e) {

    //if ("kendoConsole" in window) {
    var select = $(e.event)
    if (e.dataItem) {
        var dataItem = e.dataItem;
        IdDriver = dataItem.ID;
        //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");
    } else {
        //kendoConsole.log("event :: select");
    }
    // }
}
function onSelectddlCreateSensors(e) {  
    try {
        
        var select = $(e.event)
        if (e.dataItem) {
            var dataItem = e.dataItem;
            IdSensor = dataItem.sensorid;
            buildSensorInformation(IdSensor);
            //hasParam = dataItem.hasParam;
            //paramName = dataItem.paramName;         
        } else {
            //kendoConsole.log("event :: select");
        }
    } catch (error) {
        alert(error.message)
    }
}
function getReportParams() {

    try {
        var data = '';

        var reportId = getComboBoxSelectedOption(document.getElementById('cboReports'));

        if (reportId == 16) {
            data = getRetroGeoParams();
        }
        else {
            var deviceId = getComboBoxSelectedOption(document.getElementById('cboDevices'));

            var showDates = $('#cboReports option:selected').attr('data-showDates');
            var dateFrom = '1/1/2000';
            var dateTo = '1/1/2000';
            if (showDates == 'true') {
                dateFrom = $('#dtpFrom').val();
                dateTo = $('#dtpTo').val();
                if (reportId == '1' || reportId == '39') {
                    dateTo = dateFrom;
                }
            }

            var showHours = $('#cboReports option:selected').attr('data-showHours');
            var hourFrom = 0;
            var hourTo = 24;

            if (showHours == 'true') {
                hourFrom = getComboBoxSelectedOption(document.getElementById('cboHourFrom'));
                hourTo = getComboBoxSelectedOption(document.getElementById('cboHourTo'));
            }

            if (reportId == 0) {
                alert('Please select a report');
                data = false;
            }
            else {
                //here
                var selectedIndex = document.getElementById('cboReports').selectedIndex;
                var id = document.getElementById('cboReports')[selectedIndex].getAttribute('data-id');
                var hasParam = document.getElementById('cboReports')[selectedIndex].getAttribute('data-hasParam');
                var paramName = document.getElementById('cboReports')[selectedIndex].getAttribute('data-paramName');

                var param = '';
                var param2 = '';

                if (hasParam == 'true') {
                    switch (paramName.toLowerCase()) {
                        case 'eventcode':
                            param = getComboBoxSelectedOption(document.getElementById('pEvents'));
                            break;
                        case 'speedlimit':
                            param = $('#pSpeedLimit').val();
                            break;
                        case 'idlelimit':
                            param = $('#pIdleLimit').val();
                            break;
                        case 'mintime':
                            param = $('#pMinTime').val();
                            break;
                        case 'driverid':
                            param = getComboBoxSelectedOption(document.getElementById('pDrivers'));
                            break;
                        case 'geofenceid':
                            param = getComboBoxSelectedOption(document.getElementById('pGeofences'));
                            param2 = $('#pMinTime').val();
                            break;
                    }
                }

                if (reportId == 28) {
                    // dateFrom = dateFrom+" "+ escape(hourFrom)+":00:00";
                    //dateTo = dateTo + " " + escape(hourTo) + ":00:00";

                    data = 'token=' + getTokenCookie('ETTK') + '&reportID=' + escape(report) + '&deviceID=' + escape(deviceId) + '&dateFrom=' + escape(dateFrom) + '&dateTo=' + escape(dateTo) + '&hourFrom=' + escape(hourFrom) + '&hourTo=' + escape(hourTo);

                } else {
                    data = 't=' + getTokenCookie('ETTK') + '&rId=' + escape(reportId) + '&dId=' + escape(deviceId) + '&df=' + escape(dateFrom) + '&dt=' + escape(dateTo) + '&tf=' + escape(hourFrom) + '&tt=' + escape(hourTo) + '&p=' + escape(param) + '&p2=' + escape(param2);

                }


            }
        }

        return data;

    }
    catch (err) {
        alert('getReportParams: ' + err.description);
    }
}
function getReportParamsNew(pReportId) {
    var param;
    var param2;

    try {
        var data = '';
        //var hasParam = document.getElementById('cboReports')[selectedIndex].getAttribute('data-hasParam');
        if (!hasParam || pReportId == 17) {
            //if (pReportId == 1 || pReportId == 5 || pReportId == 6 || pReportId == 10 || pReportId == 13 || pReportId == 14 || pReportId == 15 || pReportId == 20 || pReportId == 21 || pReportId == 22
            //    || pReportId == 23 || pReportId == 24 || pReportId == 25 || pReportId == 26 || pReportId == 2        ) {
            //    
            data = 'token=' + getTokenCookie('ETTK') + '&reportID=' + escape(IdReport) + '&deviceID=' + escape(IdDevice) + '&dateFrom=' + escape(DateFrom) + '&dateTo=' + escape(DateTo) + '&hourFrom=' + escape(hourFrom) + '&hourTo=' + escape(hourTo) + '&IsForExport=' + escape(1);
        }
        else if (pReportId == 2) {
            param = IdEventCode;
            data = 'token=' + getTokenCookie('ETTK') + '&IsBatch=' + escape(1) + '&RecurrentID=' + escape(0) + '&CompanyID=' + escape(0) + '&GroupID=' + escape(0) + '&IsAllDevices=' + escape(1) + '&ExcludeWeekends=' + escape(1) + '&ReportID=' + escape(IdReport) + '&DeviceID=' + escape(IdDevice) + '&DateFrom=' + escape(DateFrom) + '&DateTo=' + escape(DateTo) + '&HourFrom=' + escape(hourFrom) + '&HourTo=' + escape(hourTo) + '&ThisDayOfWeek=' + escape(0) + '&Param=' + escape(param) + '&Param2=' + escape('') + '&IsForExport=' + escape(1);
        }
        else if (pReportId == 3) {
            param = $("#pSpeedLimit").val();
            data = 'token=' + getTokenCookie('ETTK') + '&IsBatch=' + escape(1) + '&RecurrentID=' + escape(0) + '&CompanyID=' + escape(0) + '&GroupID=' + escape(0) + '&IsAllDevices=' + escape(1) + '&ExcludeWeekends=' + escape(1) + '&ReportID=' + escape(IdReport) + '&DeviceID=' + escape(IdDevice) + '&DateFrom=' + escape(DateFrom) + '&DateTo=' + escape(DateTo) + '&HourFrom=' + escape(hourFrom) + '&HourTo=' + escape(hourTo) + '&ThisDayOfWeek=' + escape(0) + '&Param=' + escape(param) + '&Param2=' + escape('') + '&IsForExport=' + escape(1);

        }
        else if (pReportId == 4 || pReportId == 11 || pReportId == 12 || pReportId == 38) {
            param = $("#pMinTime").val();
            data = 'token=' + getTokenCookie('ETTK') + '&IsBatch=' + escape(1) + '&RecurrentID=' + escape(0) + '&CompanyID=' + escape(0) + '&GroupID=' + escape(0) + '&IsAllDevices=' + escape(1) + '&ExcludeWeekends=' + escape(1) + '&ReportID=' + escape(IdReport) + '&DeviceID=' + escape(IdDevice) + '&DateFrom=' + escape(DateFrom) + '&DateTo=' + escape(DateTo) + '&HourFrom=' + escape(hourFrom) + '&HourTo=' + escape(hourTo) + '&ThisDayOfWeek=' + escape(0) + '&Param=' + escape(param) + '&Param2=' + escape('') + '&IsForExport=' + escape(1);

        } else if (pReportId == 7) {
            param = IdGeofences;
            param2 = $("#pMinTime").val();
            data = 'token=' + getTokenCookie('ETTK') + '&IsBatch=' + escape(1) + '&RecurrentID=' + escape(0) + '&CompanyID=' + escape(0) + '&GroupID=' + escape(0) + '&IsAllDevices=' + escape(1) + '&ExcludeWeekends=' + escape(1) + '&ReportID=' + escape(IdReport) + '&DeviceID=' + escape(IdDevice) + '&DateFrom=' + escape(DateFrom) + '&DateTo=' + escape(DateTo) + '&HourFrom=' + escape(hourFrom) + '&HourTo=' + escape(hourTo) + '&ThisDayOfWeek=' + escape(0) + '&Param=' + escape(param) + '&Param2=' + escape(param2) + '&IsForExport=' + escape(1);


        }
        else if (pReportId == 18 || pReportId == 19 || pReportId == 36 || pReportId == 27) {
            param = IdDriver;
            data = 'token=' + getTokenCookie('ETTK') + '&IsBatch=' + escape(1) + '&RecurrentID=' + escape(0) + '&CompanyID=' + escape(0) + '&GroupID=' + escape(0) + '&IsAllDevices=' + escape(1) + '&ExcludeWeekends=' + escape(1) + '&ReportID=' + escape(IdReport) + '&DeviceID=' + escape(IdDevice) + '&DateFrom=' + escape(DateFrom) + '&DateTo=' + escape(DateTo) + '&HourFrom=' + escape(hourFrom) + '&HourTo=' + escape(hourTo) + '&ThisDayOfWeek=' + escape(0) + '&Param=' + escape(param) + '&Param2=' + escape('') + '&IsForExport=' + escape(1);
        }
        else {
            console.log('not report identificated')
        }
        return data;

    }
    catch (err) {
        alert('getReportParams: ' + err.description);
    }
}
function runReportNew(reportId) {
    var array = [];
    try {
        var data = '';
        data = getReportParamsNew(reportId);
        array = runReportGETV2(data);
        return array;
    }
    catch (err) {
        alert('runReportNew: ' + err.description);
    }
}
function ddlCreateDate(arrayList) {
    $("#date").kendoDropDownList({
        dataTextField: "name",
        dataValueField: "value",
        dataSource: arrayList,
        select: onSelectddlCreateDevice
    });
}
function onSelectddlCreateDate(e) {
    //if ("kendoConsole" in window) {
    var select = $(e.event)
    if (e.dataItem) {
        var dataItem = e.dataItem;
        IdDevice = dataItem.id;
        //kendoConsole.log("event :: select (" + dataItem.text + " : " + dataItem.value + ")");
    } else {
        //kendoConsole.log("event :: select");
    }
    // }
}
function sendFeedBack() {
    let response;
    try {
        
        let idType = $("#Type").val();
        let description = $("#comment").val();
        let pageVisited = window.location.href;
        if (description.length < 5) {
            alert("enter a description");
            return;
        }
        response = postSendFeedBack(pageVisited, idType, description);
        if (response.value = "OK") {
            $("#comment").val('');
            alert("FeedBack sent successfully");

        } else {
            alert("error: " + response.value);
        }
        
        var error = ""
    }
    catch (err) {
        alert("error: " + err);
        console.log("error1 " + err);
    }
}
function loadFeedBackType() {
    var response;
    
    try {
        response = GetFeedBackType();
        
        for (var index = 0; index < response.ListResponse.length; index++) {
            $("#Type").append("<option value=" + response.ListResponse[index].ID + ">" + response.ListResponse[index].Name + "</option>");
        }
    }
    catch (err) {
        alert("error: " + err);
    }
}
