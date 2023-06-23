var isGeofenceDlgReady = false;
var newGeofenceMap = false;
var newGeofenceMarker = false;
var geocoder = false;
var jsonGeoTypes = false;
var jsonGeoAlertsTypes = false;
var isPolyClosed = false;
var poly = false;
var polyMarkers = [];
var polyClickListener = false;
var geoCircle = false;

function loadGeofenceTemplateOk(data, textStatus, jqXHR) {
    try {
        if (textStatus == 'success') {
            $('body').append($(data).html());
        }
    }
    catch (err) {
        alert('loadGeofenceTemplateOk: ' + err.description);
    }
}

function loadGeofenceTemplates() {
    try {
        if ($('#newGeofenceDlg').length == 0) {
            $.ajax({
                url: 'templateGeofences.html',
                success: loadGeofenceTemplateOk,
                dataType: 'html',
                async: false
            });
        }
    }
    catch (err) {
        alert('loadGeofenceTemplates: ' + err.description);
    }
}

function setReadyGeofenceDlgs() {
    try {
        if (isGeofenceDlgReady == false) {
            loadGeofenceTemplates();
            setupNewGeofenceDlg();
            setupGeofenceRemoveDlg();
            isGeofenceDlgReady = true;
        }
        loadGeofencesTypes();
        loadGeoMessages();
    }
    catch (err) {
        alert('setReadyGeofenceDlgs: ' + err.description);
    }
}

function editGeofence(obj) {
    try {
        setReadyGeofenceDlgs();
        var id = $(obj.target).attr('data-id');
        $('#newGeofenceName').val($('#geofenceTR' + id).attr('data-name'));
        $('#newGeofenceName').attr('data-id', id);
        $('#newGeofenceAddress').val($('#geofenceTR' + id).attr('data-fullAddress'));

        $('#newGeofenceContactName').val($('#geofenceTR' + id).attr('data-contactName'));
        $('#newGeofencePhone').val($('#geofenceTR' + id).attr('data-phone'));

        //GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
        $('#newGeofenceContactEmail').val($('#geofenceTR' + id).attr('data-contactEmail'));

        if ($('#geofenceTR' + id).attr('data-contactHasSMSAlert') == 'true') {
            $('#geoContactSMSAlert').prop('checked', true);
        }
        else {
            $('#geoContactSMSAlert').prop('checked', false);
        }

        if ($('#geofenceTR' + id).attr('data-contactHasEmailAlert') == 'true') {
            $('#geoContactEmailAlert').prop('checked', true);
        }
        else {
            $('#geoContactEmailAlert').prop('checked', false);
        }

        $('#geoContactAlertType').val($('#geofenceTR' + id).attr('data-contactAlertTypeId'));

        $('#cbxContAlertArr').val($('#geofenceTR' + id).attr('data-arrivalMsgId'));
        if ($('#geofenceTR' + id).attr('data-arrivalMsgId') == 0) {
            $('#contAlertArrNew').prop('checked', true);
            $('#contAlertArrExisting').prop('checked', false);
        }
        else {
            $('#contAlertArrNew').prop('checked', false);
            $('#contAlertArrExisting').prop('checked', true);
        }
        setContactMsgArrival();

        $('#cbxContAlertDep').val($('#geofenceTR' + id).attr('data-departureMsgId'));
        if ($('#geofenceTR' + id).attr('data-departureMsgId') == 0) {
            $('#contAlertDepNew').prop('checked', true);
            $('#contAlertDepExisting').prop('checked', false);
        }
        else {
            $('#contAlertDepNew').prop('checked', false);
            $('#contAlertDepExisting').prop('checked', true);
        }
        setContactMsgDeparture();

        //============================================


        if ($('#geofenceTR' + id).attr('data-isSpeedLimit') == 'true') {
            $('#newGeofenceIsSpeedLimit').prop('checked', true);
        }
        else {
            $('#newGeofenceIsSpeedLimit').prop('checked', false);
        }
        $('#newGeofenceSpeedLimit').val($('#geofenceTR' + id).attr('data-speedLimit'));

        $('#cbxGeofencesTypes').val($('#geofenceTR' + id).attr('data-geofenceTypeId'));
        $('#newGeofenceRadius').val($('#geofenceTR' + id).attr('data-radius'));
        $('#newGeofenceComments').val($('#geofenceTR' + id).attr('data-comments'));
        $('#cbxGeofencesAlertsTypes').val($('#geofenceTR' + id).attr('data-geofenceAlertTypeId'));

        var shapeId = $('#geofenceTR' + id).attr('data-shapeId');
        initGeoType(shapeId);

        var div = document.getElementById('newGeofenceResults');
        removeAllChildNodes(div);

        var divResult = document.createElement('div');
        $(divResult).addClass('newGeofenceResultItem');
        $(divResult).addClass('newGeofenceResultItemSelected');
        $(divResult).click(changeNewGeofenceResult);
        $(divResult).attr('id', 'divResult' + 0);
        $(divResult).attr('data-ind', 0);
        $(divResult).attr('data-id', id);
        $(divResult).attr('data-latitude', $('#geofenceTR' + id).attr('data-latitude'));
        $(divResult).attr('data-longitude', $('#geofenceTR' + id).attr('data-longitude'));
        $(divResult).attr('data-fullAddress', $('#geofenceTR' + id).attr('data-fullAddress'));

        $(divResult).attr('data-contactName', $('#geofenceTR' + id).attr('data-contactName'));
        $(divResult).attr('data-phone', $('#geofenceTR' + id).attr('data-phone'));

        //GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
        $(divResult).attr('data-contactEmail', $('#geofenceTR' + id).attr('data-contactEmail'));
        $(divResult).attr('data-contactHasSMSAlert', $('#geofenceTR' + id).attr('data-contactHasSMSAlert'));
        $(divResult).attr('data-contactHasEmailAlert', $('#geofenceTR' + id).attr('data-contactHasEmailAlert'));
        $(divResult).attr('data-contactAlertTypeId', $('#geofenceTR' + id).attr('data-contactAlertTypeId'));
        $(divResult).attr('data-arrivalMsgId', $('#geofenceTR' + id).attr('data-arrivalMsgId'));
        $(divResult).attr('data-departureMsgId', $('#geofenceTR' + id).attr('data-departureMsgId'));
        //============================================

        $(divResult).attr('data-isSpeedLimit', $('#geofenceTR' + id).attr('data-isSpeedLimit'));
        $(divResult).attr('data-speedLimit', $('#geofenceTR' + id).attr('data-speedLimit'));

        $(divResult).attr('data-postalCode', $('#geofenceTR' + id).attr('data-postalCode'));
        $(divResult).attr('data-country', $('#geofenceTR' + id).attr('data-country'));
        $(divResult).attr('data-state', $('#geofenceTR' + id).attr('data-state'));
        $(divResult).attr('data-county', $('#geofenceTR' + id).attr('data-county'));
        $(divResult).attr('data-city', $('#geofenceTR' + id).attr('data-city'));
        $(divResult).attr('data-street', $('#geofenceTR' + id).attr('data-street'));
        $(divResult).attr('data-route', $('#geofenceTR' + id).attr('data-route'));
        $(divResult).attr('data-streetNumber', $('#geofenceTR' + id).attr('data-streetNumber'));
        $(divResult).attr('data-suite', $('#geofenceTR' + id).attr('data-suite'));
        $(divResult).attr('data-shapeId', shapeId);
        $(divResult).attr('data-jsonPolyVerticesTXT', $('#geofenceTR' + id).attr('data-jsonPolyVerticesTXT'));
        div.appendChild(divResult);

        var spanResult = document.createElement('span');
        $(spanResult).text($('#geofenceTR' + id).attr('data-fullAddress'));
        $(spanResult).attr('data-ind', 0);
        divResult.appendChild(spanResult);
        selectNewGeofenceResult(0);

        $("#newGeofenceDlg").dialog('open');
    }
    catch (err) {
        alert('editGeofence: ' + err.description);
    }
}

function deleteGeofence(obj) {
    try {
        setReadyGeofenceDlgs();
        if (validateUserAccess(23) == true) {
            var id = $(obj.target).attr('data-id');
            var name = $('#geofenceTR' + id).attr('data-name');
            $('#geofenceRemoveName').html(name);
            $('#geofenceRemoveName').attr('data-id', id);
            $("#geofenceRemoveDlg").dialog('open')
        }
    }
    catch (err) {
        alert('deleteGeofence: ' + err.description);
    }
}

function deleteGeofenceCommit() {
    try {
        var id = $('#geofenceRemoveName').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('removeGeofence', data, true, false);

        var tbl = document.getElementById('geofencesTbl');
        var tr = document.getElementById('geofenceTR' + id);
        tbl.removeChild(tr);

        return true;
    }
    catch (err) {
        alert('deleteGeofenceCommit: ' + err.description);
    }
}

function setupGeofenceRemoveDlg() {
    try {
        $("#geofenceRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteGeofenceCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing geofence.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#geofenceRemoveName').html('');
            }
        });
    }
    catch (err) {
        alert('setupGeofenceRemoveDlg: ' + err.description);
    }
}

function initGeoType(shapeId) {
    try {
        if (shapeId == undefined) {
            shapeId = 1;
        }
        if (shapeId == 1) {
            $('#geoTypeCircle').prop('checked', true);
            $('#geoTypePoly').prop('checked', false);
        }
        else {
            $('#geoTypeCircle').prop('checked', false);
            $('#geoTypePoly').prop('checked', true);
        }
        setGeoType();
    }
    catch (err) {
        alert('' + err.description);
    }
}

function geocodePosition(pos) {
    try {
        var currAddress = $('.newGeofenceResultItemSelected').attr('data-fullAddress');
        if (currAddress > '') {
            //Set new lat/lng
            $('.newGeofenceResultItemSelected').attr('data-latitude', pos.k);
            $('.newGeofenceResultItemSelected').attr('data-longitude', pos.B);
        }
        else {
            var geocoder = new google.maps.Geocoder();
            var address = '';
            var latlng = new google.maps.LatLng(pos.k, pos.B);
            geocoder.geocode({
                'latLng': latlng
            }, function (responses, status) {
                if (responses && responses.length > 0) {
                    $('.newGeofenceResultItemSelected').attr('data-latitude', responses[0].geometry.location.lat());
                    $('.newGeofenceResultItemSelected').attr('data-longitude', responses[0].geometry.location.lng());

                    var jsonAddress = getGoogleAddressComponents(responses[0]);
                    $('.newGeofenceResultItemSelected').attr('data-postalCode', jsonAddress.postalCode);
                    $('.newGeofenceResultItemSelected').attr('data-country', jsonAddress.country);
                    $('.newGeofenceResultItemSelected').attr('data-state', jsonAddress.state);
                    $('.newGeofenceResultItemSelected').attr('data-county', jsonAddress.county);
                    $('.newGeofenceResultItemSelected').attr('data-city', jsonAddress.city);
                    $('.newGeofenceResultItemSelected').attr('data-street', jsonAddress.street);
                    $('.newGeofenceResultItemSelected').attr('data-route', jsonAddress.route);
                    $('.newGeofenceResultItemSelected').attr('data-streetNumber', jsonAddress.streetNumber);
                    $('.newGeofenceResultItemSelected').attr('data-suite', jsonAddress.suite);
                    $('.newGeofenceResultItemSelected').attr('data-fullAddress', jsonAddress.fullAddress);

                    //updateMarkerAddress(responses[0].formatted_address);
                } else {
                    alert('Cannot determine location information at this point.');
                }
            });
        }
    }
    catch (err) {
        alert('geocodePosition: ' + err.description);
    }
}

function selectNewGeofenceResult(ind) {
    try {
        if (newGeofenceMap == false) {
            initializeNewGeofenceMap();
        }

        var lat = $('#divResult' + ind).attr('data-latitude');
        var lng = $('#divResult' + ind).attr('data-longitude');
        var latlng = new google.maps.LatLng(lat, lng);
        var bounds = new google.maps.LatLngBounds();

        if (newGeofenceMarker == false) {
            newGeofenceMarker = new google.maps.Marker({ map: newGeofenceMap, draggable: true });
        }
        else {
            newGeofenceMarker.setMap(newGeofenceMap);
        }

        var shapeId = $('#divResult' + ind).attr('data-shapeId');
        if (shapeId == 1) {
            newGeoDrawCircle();
        }
        else {
            var jsonTXT = $('#divResult' + ind).attr('data-jsonPolyVerticesTXT');
            var jsonVertices = eval('(' + jsonTXT + ')');
            drawPoly(jsonVertices);
        }

        bounds.extend(latlng);
        newGeofenceMarker.setPosition(latlng);
        newGeofenceMap.fitBounds(bounds);

        google.maps.event.addListenerOnce(newGeofenceMap, 'idle', function () {
            if (newGeofenceMap.getZoom() > 18) {
                newGeofenceMap.setZoom(18);
                google.maps.event.trigger(newGeofenceMap, 'resize');
                newGeofenceMap.setCenter(latlng);
            }
        });
        google.maps.event.addListener(newGeofenceMarker, 'dragend', function () {
            geocodePosition(newGeofenceMarker.getPosition());
        });
    }
    catch (err) {
        alert('selectNewGeofenceResult: ' + err.description);
    }
}

function changeNewGeofenceResult(obj) {
    try {

        var ind = $(obj.target).attr('data-ind');

        $('.newGeofenceResultItem').each(function () { $(this).removeClass('newGeofenceResultItemSelected') });
        $('#divResult' + ind).addClass('newGeofenceResultItemSelected');
        selectNewGeofenceResult(ind);
    }
    catch (err) {
        alert('changeNewGeofenceResult: ' + err.description);
    }
}

function loadGeocoderResults(results) {
    try {
        var jsonAddress = false;
        var div = document.getElementById('newGeofenceResults');
        removeAllChildNodes(div);

        for (var ind = 0; ind < results.length; ind++) {
            var divResult = document.createElement('div');
            $(divResult).addClass('newGeofenceResultItem');
            if (ind == 0) {
                $(divResult).addClass('newGeofenceResultItemSelected');
            }
            $(divResult).click(changeNewGeofenceResult);
            $(divResult).attr('id', 'divResult' + ind);
            $(divResult).attr('data-ind', ind);
            $(divResult).attr('data-id', 0);
            $(divResult).attr('data-latitude', results[ind].geometry.location.lat());
            $(divResult).attr('data-longitude', results[ind].geometry.location.lng());

            jsonAddress = getGoogleAddressComponents(results[ind]);
            $(divResult).attr('data-postalCode', jsonAddress.postalCode);
            $(divResult).attr('data-country', jsonAddress.country);
            $(divResult).attr('data-state', jsonAddress.state);
            $(divResult).attr('data-county', jsonAddress.county);
            $(divResult).attr('data-city', jsonAddress.city);
            $(divResult).attr('data-street', jsonAddress.street);
            $(divResult).attr('data-route', jsonAddress.route);
            $(divResult).attr('data-streetNumber', jsonAddress.streetNumber);
            $(divResult).attr('data-suite', jsonAddress.suite);
            $(divResult).attr('data-fullAddress', jsonAddress.fullAddress);

            div.appendChild(divResult);

            var spanResult = document.createElement('span');
            $(spanResult).text(results[ind].formatted_address);
            $(spanResult).attr('data-ind', ind);
            divResult.appendChild(spanResult);
        }
        selectNewGeofenceResult(0);
    }
    catch (err) {
        alert('loadGeocoderResults: ' + err.description);
    }
}

function searchNewGeofenceAddress() {
    try {
        var addr = $('#newGeofenceAddress').val();

        if (addr.length == 0) {
            alert('Please enter the geofence address');
        }
        else {
            if (geocoder == false) {
                geocoder = new google.maps.Geocoder();
            }
            geocoder.geocode({ 'address': addr }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    loadGeocoderResults(results);
                } else {
                    alert("Geocode was not successful for the following reason: " + status);
                }
            });
        }
    }
    catch (err) {
        alert('searchNewGeofenceAddress: ' + err.description);
    }
}

function newGeofence() {
    try {
        setReadyGeofenceDlgs();

        //Make sure it starts with type Circle
        initGeoType();

        $('#newGeofenceName').val('');
        $('#newGeofenceName').attr('data-id', 0);
        $('#newGeofenceAddress').val('');
        $('#newGeofenceRadius').val('500');
        $('#newGeofenceComments').val('');

        $('#newGeofenceContactName').val('');
        $('#newGeofencePhone').val('');

        //GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
        $('#newGeofenceContactEmail').val('');
        $('#geoContactSMSAlert').prop('checked', false);
        $('#geoContactEmailAlert').prop('checked', false);
        $('#geoContactAlertType').val(0);

        $('#contAlertArrNew').prop('checked', true);
        $('#contAlertDepNew').prop('checked', true);

        $('#txtContAlertArr').val('');
        $('#txtContAlertDep').val('');

        $('#cbxContAlertArr').val(0);
        $('#cbxContAlertDep').val(0);
        //============================================

        $('#newGeofenceIsSpeedLimit').prop('checked', false);
        $('#newGeofenceSpeedLimit').val('0');

        initializeNewGeofenceMap();
        $("#newGeofenceDlg").dialog('open')
    }
    catch (err) {
        alert('newGeofence: ' + err.description);
    }
}

function geofenceExists(name) {
    try {
        var bExists = false;
        data = 't=' + getTokenCookie('ETTK') + '&name=' + escape(name);
        var tmpJson = dbReadWrite('geofenceExists', data, true, false);
        if (tmpJson.result == 'true') {
            bExists = true;
        }

        return bExists;
    }
    catch (err) {
        alert('geofenceExists: ' + err.description);
    }
}

function saveNewGeofence() {
    try {
        var id = '';
        var suite = '';
        var street = '';
        var streetNumber = '';
        var route = '';
        var city = '';
        var county = '';
        var state = '';
        var postalCode = '';
        var country = '';
        var lat = '';
        var lng = '';
        var isOk = true;

        id = $('#newGeofenceName').attr('data-id');
        var name = $('#newGeofenceName').val();

        //Is there other geofence with the same name?
        if (id == '' || id == '0') {
            if (geofenceExists(name) == true) {
                alert('There is already a geofence with the name "' + name + '".  Please choose a different name.');
                isOk = false;
            }
        }
        if (isOk == true) {
            var contactName = $('#newGeofenceContactName').val();
            var phone = $('#newGeofencePhone').val();

            //GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
            var contactEmail = $('#newGeofenceContactEmail').val();
            var contactHasSMSAlert = $('#geoContactSMSAlert').prop('checked');
            var contactHasEmailAlert = $('#geoContactEmailAlert').prop('checked');
            var contactAlertTypeId = $('#geoContactAlertType').val();
            //============================================

            var isSpeedLimit = $('#newGeofenceIsSpeedLimit').prop('checked');
            var speedLimit = $('#newGeofenceSpeedLimit').val();

            var geofenceTypeId = $('#cbxGeofencesTypes').val();
            var geofenceTypeName = $('#cbxGeofencesTypes option:selected').text();

            var geofenceAlertTypeId = $('#cbxGeofencesAlertsTypes').val();
            var geofenceAlertTypeName = $('#cbxGeofencesAlertsTypes option:selected').text()

            var shapeTypeId = 0;
            var radius = 0;
            var polyVertices = [];
            var shapeType = getGeoType();
            if (shapeType == 'poly') {
                shapeTypeId = 2;

                // Since this Polygon only has one path, we can call getPath()
                // to return the MVCArray of LatLngs
                var vertices = poly.getPath();

                // Iterate over the vertices.
                for (var i = 0; i < vertices.length; i++) {
                    var xy = vertices.getAt(i);
                    polyVertices.push({ 'lat': xy.lat(), 'lng': +xy.lng() });

                    if (lat == '') {
                        lat = xy.lat();
                        lng = xy.lng();
                    }
                }
            }
            else {
                shapeTypeId = 1;
                radius = $('#newGeofenceRadius').val();
            }

            var jsonPolyVerticesTXT = JSON.stringify(polyVertices);

            var comments = $('#newGeofenceComments').val();
            var fullAddress = $('.newGeofenceResultItemSelected').attr('data-fullAddress');
            if (fullAddress == 'undefined') {
                fullAddress = '';
            }


            if ($('.newGeofenceResultItemSelected').attr('data-suite')) {
                suite = $('.newGeofenceResultItemSelected').attr('data-suite');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-street')) {
                street = $('.newGeofenceResultItemSelected').attr('data-street');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-streetNumber')) {
                streetNumber = $('.newGeofenceResultItemSelected').attr('data-streetNumber');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-route')) {
                route = $('.newGeofenceResultItemSelected').attr('data-route');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-city')) {
                city = $('.newGeofenceResultItemSelected').attr('data-city');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-county')) {
                county = $('.newGeofenceResultItemSelected').attr('data-county');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-state')) {
                state = $('.newGeofenceResultItemSelected').attr('data-state');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-postalCode')) {
                postalCode = $('.newGeofenceResultItemSelected').attr('data-postalCode');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-country')) {
                country = $('.newGeofenceResultItemSelected').attr('data-country');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-latitude')) {
                lat = $('.newGeofenceResultItemSelected').attr('data-latitude');
            }
            if ($('.newGeofenceResultItemSelected').attr('data-longitude')) {
                lng = $('.newGeofenceResultItemSelected').attr('data-longitude');
            }

            var arrMsgTxt = '';
            var arrMsgId = 0;
            if ($('#contAlertArrNew').prop('checked') == true) {
                 arrMsgTxt = $('#txtContAlertArr').val();
                 arrMsgId = 0;
            }
            else {
                 arrMsgTxt = '';
                 arrMsgId = $('#cbxContAlertArr').val();
            }

            var depMsgTxt = '';
            var depMsgId = 0;
            if ($('#contAlertDepNew').prop('checked') == true) {
                depMsgTxt = $('#txtContAlertDep').val();
                depMsgId = 0;
            }
            else {
                depMsgTxt = '';
                depMsgId = $('#cbxContAlertDep').val();
            }

            //data = 't=' + getTokenCookie('ETTK') +
            //       '&id=' + escape(id) +
            //       '&geofenceTypeId=' + escape(geofenceTypeId) +
            //       '&name=' + escape(name) +
            //       '&contactName=' + escape(contactName) +
            //       '&phone=' + escape(phone) +
            //       '&contEmail=' + escape(contactEmail) +
            //       '&contSMSAlert=' + escape(contactHasSMSAlert) +
            //       '&contEmailAlert=' + escape(contactHasEmailAlert) +
            //       '&contAlertType=' + escape(contactAlertTypeId) +
            //       '&fullAddress=' + escape(fullAddress) +
            //       '&street=' + escape(street) +
            //       '&streetNumber=' + escape(streetNumber) +
            //       '&route=' + escape(route) +
            //       '&suite=' + escape(suite) +
            //       '&city=' + escape(city) +
            //       '&county=' + escape(county) +
            //       '&state=' + escape(state) +
            //       '&postalCode=' + escape(postalCode) +
            //       '&country=' + escape(country) +
            //       '&lat=' + escape(lat) +
            //       '&lng=' + escape(lng) +
            //       '&alertTypeId=' + escape(geofenceAlertTypeId) +
            //       '&radius=' + escape(radius) +
            //       '&comments=' + escape(comments) +
            //       '&shapeId=' + escape(shapeTypeId) +
            //       '&vertices=' + escape(jsonPolyVerticesTXT) +
            //       '&isSpeedLimit=' + escape(isSpeedLimit) +
            //       '&speedLimit=' + escape(speedLimit);
            data = {
                token: getTokenCookie('ETTK'),
                id: id,
                geofenceTypeId: geofenceTypeId,
                geofenceTypeName: geofenceTypeName,
                name: name,
                contactName: contactName,
                phone: phone,
                contactEmail: contactEmail,
                contactSMSAlert: contactHasSMSAlert,
                contactEmailAlert: contactHasEmailAlert,
                contactAlertTypeId: contactAlertTypeId,
                fullAddress: fullAddress,
                street: street,
                streetNumber: streetNumber,
                route: route,
                suite: suite,
                city: city,
                county: county,
                state: state,
                postalCode: postalCode,
                country: country,
                latitude: lat,
                longitude: lng,
                geofenceAlertTypeId: geofenceAlertTypeId,
                geofenceAlertTypeName: geofenceAlertTypeName,
                radius: radius,
                comments: comments,
                shapeId: shapeTypeId,
                jsonPolyVerticesTXT: jsonPolyVerticesTXT,
                isSpeedLimit: isSpeedLimit,
                speedLimit: speedLimit,
                arrivalMsgId: arrMsgId,
                arrivalMsgTxt: arrMsgTxt,
                departureMsgId: depMsgId,
                departureMsgTxt: depMsgTxt
        }
            var postData = JSON.stringify(data);
            $.ajax({
                url: 'https://localhost:44385/etrack.svc/saveGeofence',
                type: "POST",
                data: postData,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (result, textStatus, jqXHR) {
                    if (id == '0') {
                        addGeofenceToList(data);
                    }
                    else {
                        modifyGeofenceListRecord(id, data);
                    }
                    return true;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Failed saving geofence. Please try again or contact Technical Support.');
                    return true;
                },
                async: true
            });

            //var tmpJson = dbReadWrite('saveGeofence', txtData, true, false);

            //if (tmpJson == false) {
            //    alert('Failed saving geofence. Please try again or contact Technical Support.');
            //    return;
            //}

            //var itm = {
            //    'id': tmpJson.result,
            //    'geofenceTypeId': geofenceTypeId,
            //    'geofenceTypeName': geofenceTypeName,
            //    'name': name,
            //    'contactName': contactName,
            //    'contactEmail': contactEmail,
            //    'contactSMSAlert': contactHasSMSAlert,
            //    'contactEmailAlert': contactHasEmailAlert,
            //    'contactAlertTypeId': contactAlertTypeId,
            //    'phone': phone,
            //    'fullAddress': fullAddress,
            //    'street': street,
            //    'streetNumber': streetNumber,
            //    'route': route,
            //    'suite': suite,
            //    'city': city,
            //    'county': county,
            //    'state': state,
            //    'postalCode': postalCode,
            //    'country': country,
            //    'latitude': lat,
            //    'longitude': lng,
            //    'radius': 100,
            //    'geofenceAlertTypeId': geofenceAlertTypeId,
            //    'geofenceAlertTypeName': geofenceAlertTypeName,
            //    'radius': radius,
            //    'comments': comments,
            //    'shapeId': shapeTypeId,
            //    'jsonPolyVerticesTXT': jsonPolyVerticesTXT,
            //    'isSpeedLimit': isSpeedLimit,
            //    'speedLimit': speedLimit
            //};

            //if (id == '0') {
            //    addGeofenceToList(data);
            //}
            //else {
            //    modifyGeofenceListRecord(id, data);
            //}

            return true;
        }
        else {
            return false;
        }
    }
    catch (err) {
        alert('saveNewGeofence: ' + err.description);
    }
}

function loadGeofencesAlertsTypes() {
    try {
        if (jsonGeoAlertsTypes == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonGeoAlertsTypes = dbReadWrite('getGeofencesAlertsTypes', data, true, false);
            if (jsonGeoAlertsTypes) {
                var cbx = document.getElementById('cbxGeofencesAlertsTypes');
                removeAllChildNodes(cbx);
                loadComboBox(jsonGeoAlertsTypes.types, cbx, 'Pick Event');
                var cbx = document.getElementById('geoContactAlertType');
                removeAllChildNodes(cbx);
                loadComboBox(jsonGeoAlertsTypes.types, cbx, 'Pick Event');
            }
        }
    }
    catch (err) {
        alert('loadGeofencesAlertsTypes: ' + err.description);
    }
}

function loadGeofencesTypes() {
    try {
        //Always reload geofences types
        //jsonGeoTypes = false; 
        if (jsonGeoTypes == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonGeoTypes = dbReadWrite('getGeofencesTypes', data, true, false);
            if (jsonGeoTypes) {
                var cbx = document.getElementById('cbxGeofencesTypes');
                removeAllChildNodes(cbx);
                loadComboBox(jsonGeoTypes.types, cbx, 'Pick Type');
            }
        }
    }
    catch (err) {
        alert('loadGeofencesTypes: ' + err.description);
    }
}

function loadGeoMessages() {
    try {
        //Get the ARRIVAL custom messages
        var a = 1;

        $.ajax({
            url: 'https://localhost:44385/etrack.svc/getGeofencesMessages/' + getTokenCookie('ETTK') + '/1',
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                var cbx = document.getElementById('cbxContAlertArr');
                removeAllChildNodes(cbx);
                loadComboBox(data, cbx, 'Pick Message');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed geofence custom messages 1');
            },
            async: false
        });

        //Get the DEPARTURE custom messages
        $.ajax({
            url: 'https://localhost:44385/etrack.svc/getGeofencesMessages/' + getTokenCookie('ETTK') + '/2',
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                var cbx = document.getElementById('cbxContAlertDep');
                removeAllChildNodes(cbx);
                loadComboBox(data, cbx, 'Pick Message');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed geofence custom messages 2');
            },
            async: false
        });
    }
    catch (err) {
        alert('loadGeoMessages: ' + err.description);
    }
}

function setupNewGeofenceDlg() {
    try {
        loadGeofencesAlertsTypes();
        loadGeofencesTypes();
        $("#newGeofenceDlg").dialog({
            height: 780,
            width: 980,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveNewGeofence() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving geofence.  Please try again.');
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#newGeofenceName').val('');
                $('#newGeofenceAddress').val('');
                if (newGeofenceMarker != false) {
                    newGeofenceMarker.setMap(null);
                }
                var div = document.getElementById('newGeofenceResults');
                removeAllChildNodes(div);
            }
        });
    }
    catch (err) {
        alert('setupNewGeofenceDlg: ' + err.description);
    }
}

function initializeNewGeofenceMap() {
    try {
        var bounds = false;
        var cntr = false;
        var sw = false;
        var ne = false;

        //Center in the entire USA
        sw = new google.maps.LatLng(25, -123.20);
        ne = new google.maps.LatLng(43, -75.20);

        // Create a bounding box
        bounds = new google.maps.LatLngBounds(sw, ne);
        cntr = bounds.getCenter();

        var myOptions = {
            zoom: 4,
            center: cntr,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }

        if (newGeofenceMap == false) {
            newGeofenceMap = new google.maps.Map(document.getElementById("newGeofenceMap1"), myOptions);
        }
        newGeofenceMap.fitBounds(bounds);
        google.maps.event.addListenerOnce(newGeofenceMap, 'idle', function () {
            if (newGeofenceMap.getZoom() > 18) {
                newGeofenceMap.setZoom(18);
            }
        });

    }
    catch (err) {
        alert('initializeNewGeofenceMap: ' + err.description);
    }
}

function getGeoType() {
    try {
        return $('input[name="geoType"]:checked').val();
    }
    catch (err) {
        alert('getGeotype: ' + err.description);
    }
}

function setGeoType() {
    try {
        var val = getGeoType();
        if (val == 'poly') {
            clearCircleGeo();
            $('#btnNewGeoPolyStart').html('Start Drawing Polygon in map');
            $('#newGeoDlgPoly').show();
            $('#newGeoDlgCircle').hide();
        }
        else {
            clearPolyGeo();
            $('#newGeoDlgPoly').hide();
            $('#newGeoDlgCircle').show();
        }

    }
    catch (err) {
        alert('setGeoType: ' + err.description);
    }
}

function setContactMsgArrival() {
    try {
        if ($('#contAlertArrNew').prop('checked') == true) {
            $('#txtContAlertArr').show();
            $('#cbxContAlertArr').hide();
        }
        else {
            $('#txtContAlertArr').hide();
            $('#cbxContAlertArr').show();
        }
    }
    catch (err) {
        alert('setContactMsgArrival: ' + err.description);
    }
}

function setContactMsgDeparture() {
    try {
        if ($('#contAlertDepNew').prop('checked') == true) {
            $('#txtContAlertDep').show();
            $('#cbxContAlertDep').hide();
        }
        else {
            $('#txtContAlertDep').hide();
            $('#cbxContAlertDep').show();
        }
    }
    catch (err) {
        alert('setContactMsgDeparture: ' + err.description);
    }
}

function clearCircleGeo() {
    try {
        if (geoCircle != false) {
            geoCircle.setMap(null);
            geoCircle = false;
        }
    }
    catch (err) {
        alert('clearCircleGeo: ' + err.description);
    }
}

function clearPolyGeo() {
    try {
        if (poly != false) {
            poly.setMap(null);
            poly.getPath.length = 0;
            poly = false;

            if (polyMarkers) {
                for (i in polyMarkers) {
                    polyMarkers[i].setMap(null);
                }
                polyMarkers.length = 0;
            }
            if (polyClickListener) {
                google.maps.event.removeListener(polyClickListener);
                polyClickListener = false;
            }
        }
    }
    catch (err) {
        alert('clearPolyGeo: ' + err.description);
    }
}

function newGeoDrawCircle() {
    try {
        //Get the radius, in feet
        var radius = $('#newGeofenceRadius').val();
        if (isNumber(radius) == true) {
            clearCircleGeo();

            geoCircle = new google.maps.Circle({
                map: newGeofenceMap,
                radius: (parseFloat(radius) * 0.3048),    // 10 miles in metres
                fillColor: '#AA0000'
            });
            geoCircle.bindTo('center', newGeofenceMarker, 'position');
        }
        else {
            alert('Please enter a valid radius');
        }
    }
    catch (err) {
        alert('newGeoDrawCircle: ' + err.description);
    }
}

function drawPoly(jsonVertices) {
    try {
        clearPolyGeo();

        poly = new google.maps.Polyline({ map: newGeofenceMap, path: [], strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2 });

        for (var i = 0; i < jsonVertices.length; i++) {
            var jsonItem = jsonVertices[i];
            var loc = new google.maps.LatLng(jsonItem.lat, jsonItem.lng);
            var marker = new google.maps.Marker({ map: newGeofenceMap, position: loc, draggable: true });
            polyMarkers.push(marker);
            google.maps.event.addListener(marker, 'drag', function (dragEvent) {
                poly.getPath().setAt(i, dragEvent.latLng);
            });
            poly.getPath().push(loc);
        }
        var path = poly.getPath();
        poly.setMap(null);
        poly = new google.maps.Polygon({ map: newGeofenceMap, path: path, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35 });
    }
    catch (err) {
    }
}

function startPolygon() {
    try {
        $('#btnNewGeoPolyStart').html('Click again to erase and re-draw');
        isPolyClosed = false;
        clearPolyGeo();

        poly = new google.maps.Polyline({ map: newGeofenceMap, path: [], strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2 });

        polyClickListener = google.maps.event.addListener(newGeofenceMap, 'click', function (clickEvent) {
            if (isPolyClosed)
                return;
            var markerIndex = poly.getPath().length;
            var isFirstMarker = markerIndex === 0;
            var marker = new google.maps.Marker({ map: newGeofenceMap, position: clickEvent.latLng, draggable: true });

            polyMarkers.push(marker);

            if (isFirstMarker) {
                google.maps.event.addListener(marker, 'click', function () {
                    if (isPolyClosed)
                        return;
                    var path = poly.getPath();
                    poly.setMap(null);
                    poly = new google.maps.Polygon({ map: newGeofenceMap, path: path, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35 });
                    isPolyClosed = true;
                });
            }
            google.maps.event.addListener(marker, 'drag', function (dragEvent) {
                poly.getPath().setAt(markerIndex, dragEvent.latLng);
            });
            poly.getPath().push(clickEvent.latLng);
        });

    }
    catch (err) {
        alert('startPolygon: ' + err.description);
    }
}

