var isReadyGeofenceDlg = false;

function addGeofenceFromPoint(deviceId, lat, lng) {
    try {
        if (isReadyGeofenceDlg == false) {
            setGeofenceDlg();
            isReadyGeofenceDlg = true;
        }
        var jsonAddress = false;
        var geocoder = new google.maps.Geocoder();
        var latlng = new google.maps.LatLng(lat, lng);

        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    jsonAddress = getGoogleAddressComponents(results[0]);
                    $('#geoFormStreet').val(jsonAddress.street);
                    $('#geoFormCity').val(jsonAddress.city);
                    $('#geoFormState').val(jsonAddress.state);
                    $('#geoFormPostalCode').val(jsonAddress.postalCode);
                    $('#geoFormLatitude').val(lat);
                    $('#geoFormLongitude').val(lng);
                    $('#geoFormRadius').val(200);
                    $('#geofenceForm').attr('data-jsonAddress', JSON.stringify(jsonAddress));
                    $("#geofenceForm").dialog('open');
                }
            }
        });
    }
    catch (err) {
        alert('addGeofenceFromPoint: ' + err.description);
    }
}

function setGeofenceDlg() {
    try {
        tips = $(".validateTips");
        $("#geofenceForm:ui-dialog").dialog("destroy");

        $("#geofenceForm").dialog({
            autoOpen: false,
            height: 270,
            width: 380,
            modal: true,
            buttons: {
                "Save": function () {
                    var bValid = true;

                    bValid = bValid && checkLength($('#geoFormName'), "Name", 2, 20);
                    bValid = bValid && checkLength($('#geoFormStreet'), "Street", 2, 50);
                    bValid = bValid && checkLength($('#geoFormCity'), "City", 2, 20);
                    bValid = bValid && checkLength($('#geoFormState'), "State", 2, 20);
                    bValid = bValid && checkLength($('#geoFormPostalCode'), "Postal Code", 2, 10);

                    if (bValid) {
                        var name = $('#geoFormName').val();
                        var jsonAddress = eval('(' + $('#geofenceForm').attr('data-jsonAddress') + ')');
                        var lat = $('#geoFormLatitude').val();
                        var lng = $('#geoFormLongitude').val();
                        var radius = $('#geoFormRadius').val();

                        //var address = buildGeofenceFormAddress();
                        //bValid = geocodeLocation(address, false);

                        //data = 't=' + getTokenCookie('ETTK') + '&id=0' + '&name=' + escape(name) + '&fullAddress=' + escape(jsonAddress.fullAddress) + '&street=' + escape(jsonAddress.street) + '&streetNumber=' + escape(jsonAddress.streetNumber) + '&route=' + escape(jsonAddress.route) + '&suite=' + escape(jsonAddress.suite) + '&city=' + escape(jsonAddress.city) + '&county=' + escape(jsonAddress.county) + '&state=' + escape(jsonAddress.state) + '&postalCode=' + escape(jsonAddress.postalCode) + '&country=' + escape(jsonAddress.country) + '&lat=' + lat + '&lng=' + lng + '&radius=' + escape(radius);
                        //var tmpJson = dbReadWrite('saveGeofence', data, true, false);

                        //==================================
                        data = {
                            token: getTokenCookie('ETTK'),
                            id: 0,
                            geofenceTypeId: 0,
                            name: name,
                            contactName: '',
                            phone: '',
                            contactEmail: '',
                            contactSMSAlert: false,
                            contactEmailAlert: false,
                            contactAlertTypeId: 0,
                            fullAddress: jsonAddress.fullAddress,
                            street: jsonAddress.street,
                            streetNumber: jsonAddress.streetNumber,
                            route: jsonAddress.route,
                            suite: jsonAddress.suite,
                            city: jsonAddress.city,
                            county: jsonAddress.county,
                            state: jsonAddress.state,
                            postalCode: jsonAddress.postalCode,
                            country: jsonAddress.country,
                            latitude: lat,
                            longitude: lng,
                            geofenceAlertTypeId: 0,
                            geofenceAlertTypeName: '',
                            radius: radius,
                            comments: '',
                            shapeId: 1,
                            jsonPolyVerticesTXT: '',
                            isSpeedLimit: false,
                            speedLimit: 0,
                            arrivalMsgId: 0,
                            arrivalMsgTxt: '',
                            departureMsgId: 0,
                            departureMsgTxt: ''
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
                                alert('Geofence created');
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                alert('Failed saving geofence. Please try again or contact Technical Support.');
                                return true;
                            },
                            async: true
                        });
                        //==================================

                        //alert('Geofence created');

                        $(this).dialog("close");
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            },
            close: function () {
                $('#geoFormName').val('');
                $('#geoFormStreet').val('');
                $('#geoFormCity').val('');
                $('#geoFormState').val('');
                $('#geoFormPostalCode').val('');
                $('#geoFormLatitude').val('');
                $('#geoFormLongitude').val('');
                $('#geofenceForm').attr('data-jsonAddress', '');
            }
        });
    }
    catch (err) {
        alert('setGeofenceDlg: ' + err.description);
    }
}
