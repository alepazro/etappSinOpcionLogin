var breadcrumbResult = false;

function generateStaticMap() {
    try {

        $('#reportContent').html('');
        $('#reportContent').append(breadcrumbResult.header);

        for (var devInd = 0; devInd < breadcrumbResult.devices.length; devInd++) {
            var dev = eval('(' + breadcrumbResult.devices[devInd] + ')');

            for (var detInd = 0; detInd < dev.devDet.length; detInd++) {
                var devDet = eval('(' + dev.devDet[detInd] + ')');

                var div = document.createElement('div');
                $(div).attr('class', 'asInstance');
                $(div).html($("#asItemTemplate").html());
                $(div).find('.asDevName').text(dev.devName);
                $(div).find('.asDate').text(devDet.actDate);
                $(div).find('.asIgnOn').text(devDet.ignOn);
                $(div).find('.asIgnOnAddress').text(devDet.ignOnLoc);
                $(div).find('.asIgnOff').text(devDet.ignOff);
                $(div).find('.asIgnOffAddress').text(devDet.ignOffLoc);
                $(div).find('.asHours').text(devDet.hours);
                $(div).find('.asMiles').text(devDet.miles);

                var breadcrumbPath = new google.maps.MVCArray();

                for (var locInd = 0; locInd < devDet.locs.length; locInd++) {
                    var loc = eval('(' + devDet.locs[locInd] + ')');
                    var devLatLng = new google.maps.LatLng(loc.lat, loc.lng);
                    breadcrumbPath.push(devLatLng);
                    if (locInd > 400) {
                        break;
                    }
                }
                var encodeString = google.maps.geometry.encoding.encodePath(breadcrumbPath);
                if (encodeString != null) {
                    var url = "https://maps.googleapis.com/maps/api/staticmap?size=556x343&sensor=false&key=AIzaSyAkc2PSbElBr5fANIa75Wmzc7bQnHUGTk4&path=weight:3%7Ccolor:red%7Cenc:" + escape(encodeString);
                    var googURL = { _url: escape(url) };
                    googURL = JSON.stringify(googURL);
                    var ws = 'etrest.svc/getSignature';
                    var sig = '';
                    $.ajax({
                        url: ws,
                        type: "POST",
                        data: googURL,
                        dataType: 'json',
                        contentType: "application/json; charset=utf-8",
                        processdata: false,
                        success: function (data, textStatus, jqXHR) {
                            sig = data.sig;
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            sig='';
                        },
                        async: false
                    });

                    $(div).find('.asTrail').attr("src", url + '&signature=' + sig);

                }

                for (var geoInd = 0; geoInd < devDet.geo.length; geoInd++) {
                    var geo = eval('(' + devDet.geo[geoInd] + ')');

                    var tr = document.createElement('tr');
                    if (geoInd % 2 == 0)
                        $(tr).addClass('rowListOddTR');

                    var tdName = document.createElement('td');
                    $(tdName).text(geo.geoName);
                    $(tr).append(tdName);

                    var tdArrival = document.createElement('td');
                    $(tdArrival).text(geo.arrival);
                    $(tr).append(tdArrival);

                    $(div).find('.asGeofencesList').append(tr);
                }

                $('#reportContent').append(div);
            }

        }
    }
    catch (err) {
        alert('generateStaticMap: ' + err.description);
    }
}

function multiTrailLoadFailure(jqXHR, textStatus, errorThrown) {
    try {
        alert('No data found in the selected timeframe.');
        $("#waitingSpinner").hide();

    }
    catch (err) {
        alert('multiTrailLoadFailure: ' + err.description);
    }
}

function multiTrailLoadSuccess(data, textStatus, jqXHR) {
    try {
        var jsonObj = false;

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
                    breadcrumbResult = jsonObj;
                    if (breadcrumbResult.devices.length > 0) {
                        generateStaticMap();
                    }
                    else {
                        alert('No data found in the selected timeframe');
                        $("#waitingSpinner").hide();
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
        alert('multiTrailLoadSuccess: ' + err.description);
    }
}

function executeActivitySummaryReport(deviceId, dateFrom, dateTo) {
    
    try {
        data = 't=' + escape(getTokenCookie('ETTK')) + '&deviceId=' + escape(deviceId) + '&dateFrom=' + escape(dateFrom) + '&dateTo=' + escape(dateTo);
        dbReadWriteAsync('getMultiTrail', data, multiTrailLoadSuccess, multiTrailLoadFailure);
    }
    catch (err) {
        alert('executeActivitySummaryReport: ' + err.description);
    }
}

/*======================================================*/
/* THE FOLLOWING ROUTINES ARE FOR TESTING PURPOSES ONLY */
/*======================================================*/
function multiTrailLoad(deviceId) {
    try {
        var dateFrom = '10/20/2012';
        var dateTo = '10/27/2012';

        data = 't=' + escape('7DC57DF6-1F2B-4DDC-9641-53E85BACE296') + '&deviceId=' + escape(deviceId) + '&dateFrom=' + escape(dateFrom) + '&dateTo=' + escape(dateTo);
        dbReadWriteAsync('getMultiTrail', data, multiTrailLoadSuccess, multiTrailLoadFailure);
    }
    catch (err) {
        alert('multiTrailLoad: ' + err.description);
    }
}

function createMultiTrail() {
    try {
        //var deviceId = 'M487';
        var deviceId = 'G562';

        if (deviceId == "0") {
            alert("Please select a device");
        }
        else {
            $("#waitingSpinner").show();
            multiTrailLoad(deviceId);
        }
    }
    catch (err) {
        alert('createMultiTrail: ' + err.description);
    }
}

