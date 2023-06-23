//et_03
var w = $(window).width();
var h = $(window).height();
var tips = false;
var map = false;
var marker = false;

function getFullAddress(jsonAddress) {
    try{
        var fullAddress = '';

        if (jsonAddress.street.length > 0) {
            fullAddress = jsonAddress.street;
        }
        if (jsonAddress.city.length > 0) {
            if (fullAddress.length > 0) {
                fullAddress += ', ';
            }
            fullAddress += jsonAddress.city;
        }
        if (jsonAddress.state.length > 0) {
            if (fullAddress.length > 0) {
                fullAddress += ', ';
            }
            fullAddress += jsonAddress.state;
        }
        if (jsonAddress.postalCode.length > 0) {
            if (fullAddress.length > 0) {
                fullAddress += ' ';
            }
            fullAddress += jsonAddress.postalCode;
        }

        return fullAddress;

    }
    catch (err) {

    }
}

function GetLocation(geocoderResult) {
    try {
        var lat = geocoderResult.geometry.location.lat();
        var lng = geocoderResult.geometry.location.lng();
        var state = "";
        var country = "";
        var street = "";
        var city = "";
        var postalcode = "";

        //Recorre los objetos de direccion resultantes
        for (var j = 0; j < geocoderResult.address_components.length; j++) {
            var type = geocoderResult.address_components[j].types[0];
            switch (type) {
                case "street_number":
                case "establishment":
                    street += geocoderResult.address_components[j].long_name + " ";
                    break;
                case "route":
                    street += geocoderResult.address_components[j].long_name + " ";
                    break;
                case "postal_code":
                    postalcode = geocoderResult.address_components[j].long_name;
                    break;
                case "country":
                    country = geocoderResult.address_components[j].short_name;
                    break;
                case "administrative_area_level_1":
                    state = geocoderResult.address_components[j].short_name;
                    break;
                case "locality":
                    city = geocoderResult.address_components[j].long_name;
                    break;

            }
        }

        var fullAddress = ($.trim(street) + ", " + city + ", " + state + " " + postalcode + " " + country);
        var location = {
            address1: $.trim(street),
            address2: '',
            city: $.trim(city),
            state: $.trim(state),
            postalcode: $.trim(postalcode),
            country: $.trim(country),
            lat: lat,
            lng: lng,
            radius: 0 //Por definir en la seleccion
        };

        ////////app.console("location --> " + JSON.stringify(location));

        return location;
    } catch (exc) {
        _Show("GetLocation " + exc.message);

    }
    return null;
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function isInteger(val) {
    try {
        var ret = false;
        var intRegex = /^\d+$/;
        if (intRegex.test(val)) {
            ret = true;
        }
        return ret;
    }
    catch (err) {
        alert('isInteger: ' + err.description);
    }
}

function pad(str, max) {
    return str.length < max ? pad("0" + str, max) : str;
}

function showLocationInMap(canvas, lat, lng) {
    try {
        var cntr = false;
        if (map == false) {
            cntr = new google.maps.LatLng(lat, lng);
            var myOptions = {
                zoom: 16,
                center: cntr,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }
            map = new google.maps.Map(document.getElementById(canvas), myOptions);
        }
        else {
            cntr = new google.maps.LatLng(lat, lng);
            map.setCenter(cntr);
            map.setZoom(16);
        }
        if (marker == false) {
            marker = new google.maps.Marker({ position: cntr, map: map });
        }
        else {
            marker.setPosition(cntr);
        }
        $("#eventLocationMapDlg").dialog('open');

    }
    catch (err) {
        alert('showLocationInMap: ' + err.description);
    }
}

function setupEventLocationMapDlg() {
    try {
        $("#eventLocationMapDlg").dialog({
            height: 500,
            width: 440,
            autoOpen: false,
            modal: false,
            buttons: [
                {
                   text:'Close',
                   click: function(){
                       $(this).dialog("close");
                   }
                }
            ]
        });
    }
    catch (err) {
        alert('setupEventLocationMapDlg: ' + err.description);
    }
}

function rgb2hex(color) {
    try {
        // If we have a standard or shorthand Hex color, return that value.
        if (color.match(/[0-9A-F]{6}|[0-9A-F]{3}$/i)) {
            return (color.charAt(0) === "#") ? color : ("#" + color);

            // Alternatively, check for RGB color, then convert and return it as Hex.
        } else if (color.match(/^rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$/)) {
            var c = ([parseInt(RegExp.$1, 10), parseInt(RegExp.$2, 10), parseInt(RegExp.$3, 10)]),
                    pad = function (str) {
                        if (str.length < 2) {
                            for (var i = 0, len = 2 - str.length; i < len; i++) {
                                str = '0' + str;
                            }
                        }

                        return str;
                    };

            if (c.length === 3) {
                var r = pad(c[0].toString(16)),
                        g = pad(c[1].toString(16)),
                        b = pad(c[2].toString(16));

                return '#' + r + g + b;
            }

            // Otherwise we wont do anything.
        } else {
            return false;

        }
    }
    catch (err) {
        alert('err: ' + err.description);
    }
}

function setWebEngage() {
    try {
        $('#feedback').hide();

//        if (document.domain.toLowerCase().indexOf('easitrack') != -1) {
//            $('#feedback').show();
//            window.webengageWidgetInit = window.webengageWidgetInit || function () {
//                webengage.init({
//                    licenseCode: "~10a5cbb2c"
//                }).onReady(function () {
//                    webengage.render();
//                });
//            };

//            (function (d) {
//                var _we = d.createElement('script');
//                _we.type = 'text/javascript';
//                _we.async = true;
//                _we.src = (d.location.protocol == 'https:' ? "//ssl.widgets.webengage.com" : "//cdn.widgets.webengage.com") + "/js/widget/webengage-min-v-3.0.js";
//                var _sNode = d.getElementById('_webengage_script_tag');
//                _sNode.parentNode.insertBefore(_we, _sNode);
//            })(document);
//        } 
//        else {
//            $('#feedback').hide();
//        }
    }
    catch (err) {
        alert('setWebEngage: ' + err.description);
    }
}

function getDiagInfo(devId, hId) {
    try {
        var data = 't=' + getTokenCookie('ETTK') + '&deviceId=' + escape(devId) + '&hId=' + escape(hId);
        var jsonDiag = dbReadWrite('getDiagInfo', data, true, false);
        alert('Diag info: Consecutive = ' + jsonDiag.consecutive);
    }
    catch (err) {
        alert('getDiagInfo: ' + err.description);
    }
}

function setLiveChat() {
    try {
        var seWtVh = document.createElement("script");
        seWtVh.type = "text/javascript";
        var seWtVhs = (location.protocol.indexOf("https") == 0 ? "https" : "http") + "://image.providesupport.com/js/0325vm2g5aj6809q0cspkf44yw/safe-textlink.js?ps_h=WtVh&ps_t=" + new Date().getTime() + "&online-link-html=Live%20Chat&offline-link-html=" + "&customer=" + escape(userCompanyName) + ":" + escape(userFullName);
        seWtVh.src = seWtVhs; 
        document.getElementById('sdWtVh').appendChild(seWtVh)
    }
    catch (err) {
    }

    try {
        var seSsCz = document.createElement("script");
        seSsCz.type = "text/javascript";
        var seSsCzs = (location.protocol.indexOf("https") == 0 ? "https" : "http") + "://image.providesupport.com/js/0325vm2g5aj6809q0cspkf44yw/safe-monitor.js?ps_h=SsCz&ps_t=" + new Date().getTime() + "&customer=" + escape(userCompanyName) + ":" + escape(userFullName);
        seSsCz.src = seSsCzs;
        document.getElementById('sdSsCz').appendChild(seSsCz);
    }
    catch (err) {
    }
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function activityIndicator() {
    try {
        // Setup the ajax indicator
        $('body').append('<div id="ajaxBusy"><p><img src="images/loading.gif"></p></div>');

        $('#ajaxBusy').css({
            display: "none",
            margin: "0px",
            paddingLeft: "0px",
            paddingRight: "0px",
            paddingTop: "0px",
            paddingBottom: "0px",
            position: "absolute",
            right: "3px",
            top: "3px",
            width: "auto"
        });
    }
    catch (err) {
        alert('activityIndicator: ' + err.description);
    }
}

function getGoogleAddressFromPoint(lat, lng) {
    try {

        var jsonAddress = false;
        var geocoder = new google.maps.Geocoder();
        var latlng = new google.maps.LatLng(lat, lng);

        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (results[0]) {
                    jsonAddress = getGoogleAddressComponents(results[0]);
                }
            }
        });

        return jsonAddress;

    }
    catch (err) {
        alert('getGoogleAddressFromPoint: ' + err.description);
    }
}

function getGoogleAddressComponents(result) {
    try {
        var street = '';
        var streetNumber = '';
        var route = '';
        var suite = '';
        var city = '';
        var county = '';
        var state = '';
        var postalCode = '';
        var country = '';
        var fullAddress = result.formatted_address;
        var lat = result.geometry.location.lat();
        var lng = result.geometry.location.lng();
        var jsonAddress = false;

        for (var j = 0; j < result.address_components.length; j++) {
            for (var k = 0; k < result.address_components[j].types.length; k++) {
                switch (result.address_components[j].types[k]) {
                    case 'street_address':
                        street = result.address_components[j].short_name;
                        break;
                    case 'street_number':
                        streetNumber = result.address_components[j].short_name;
                        break;
                    case 'route':
                        route = result.address_components[j].short_name;
                        break;
                    case 'subpremise':
                        suite = result.address_components[j].short_name;
                        break;
                    case 'locality':
                        city = result.address_components[j].short_name;
                        break;
                    case 'administrative_area_level_2':
                        county = result.address_components[j].short_name;
                        break;
                    case 'administrative_area_level_1':
                        state = result.address_components[j].short_name;
                        break;
                    case 'postal_code':
                        postalCode =  result.address_components[j].short_name;
                        break;
                    case 'country':
                        country = result.address_components[j].short_name;
                        break;
                }
            }
        }

        if (street == '') {
            street = streetNumber + ' ' + route;
        }

        jsonAddress = {
            'street': street,
            'streetNumber': streetNumber,
            'route': route,
            'suite': suite,
            'city': city,
            'county': county,
            'state': state,
            'postalCode': postalCode,
            'country': country,
            'fullAddress': fullAddress,
            'lat': lat,
            'lng': lng
        };

        return jsonAddress;

    }
    catch (err) {
        alert('getGoogleAddressComponents: ' + err.message + ' ' + err.description);
    }
}

function getCurrentHour() {
    try {
        var currentTime = new Date()
        var hour = currentTime.getHours();
        return hour;
    }
    catch (err) {
        alert('getCurrentHour: ' + err.description);
    }
}

//delta should come as a negative number
function getPreviousDates(delta) {
    try {
        var theDate = new Date()
        theDate.setDate(theDate.getDate() + delta)
        var month = theDate.getMonth() + 1
        var day = theDate.getDate()
        var year = theDate.getFullYear()
        return month + "/" + day + "/" + year;
    }
    catch (err) {
        alert('getPreviousDates: ' + err.description);
    }
}

function getCurrentDate() {
    return getNow();
}

function getNow() {
    try {
        var currentTime = new Date()
        var month = currentTime.getMonth() + 1
        var day = currentTime.getDate()
        var year = currentTime.getFullYear()
        return month + "/" + day + "/" + year;
    }
    catch (err) {
        alert('getNow: ' + err.description);
    }
}

function loadHoursCombo(obj) {
    try {
        var ampm = '';
        var i = 0;
        var j = 0;
        var jsonHours = [];
        for (i = 1; i <= 2; i++) { if (i == 1) { ampm = 'AM'; } else { ampm = 'PM'; } for (j = 1; j <= 12; j++) { if (i == 1 && j == 12) { jsonHours.push({ 'id': (j + (12 * (i - 1))), 'name': 'Noon' }); } else { jsonHours.push({ 'id': (j + (12 * (i - 1))), 'name': (j.toString() + ' ' + ampm) }); } } }
        loadComboBox(jsonHours,obj, '00 AM');
    }
    catch (err) {
        alert('loadHoursCombo: ' + err.description);
    }
}

function moveTo(e, objTargetId) {
    try {
        var evt = e || window.event;
        var keyPressed = evt.which || evt.keyCode;

        if (keyPressed == 13) {
            document.getElementById(objTargetId.id).focus();
        }
    }
    catch (err) {
        alert('moveTo: ' + err.Description);
    }
}

function checkLength(o, n, min, max) {
    if (o.val().length > max || o.val().length < min) {
        o.addClass("ui-state-error");
        updateTips("Length of " + n + " must be between " +
					min + " and " + max + ".");
        return false;
    } else {
        return true;
    }
}

function updateTips(t) {
    tips
				.text(t)
				.addClass("ui-state-highlight");
    setTimeout(function () {
        tips.removeClass("ui-state-highlight", 1500);
    }, 500);
}

function removeAllChildNodes(node) {
    if (node && node.hasChildNodes && node.removeChild) {
        while (node.hasChildNodes()) {
            node.removeChild(node.firstChild);
        }
    }
} // removeAllChildNodes()

function validateEmail(email) {
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    return emailPattern.test(email);
}

function setComboBoxOption(cbx, selectedValue) {
    try {
        for (var ind = 0; ind < cbx.length; ind++) {
            var tmpId = cbx[ind].getAttribute('data-id');
            if (selectedValue == tmpId) {
                cbx.selectedIndex = ind;
                break;
            }
        }
    }
    catch (err) {
        alert('setComboBoxOption: ' + err.description);
    }
}

function getComboBoxSelectedOption(cbx) {
    try {
        var selectedIndex = cbx.selectedIndex;
        var id = cbx[selectedIndex].getAttribute('data-id');

        return id;
    }
    catch (err) {
        alert('getComboBoxSelectedOption: ' + itmName);
    }
}

function getJsonRecord(jsonLst, id) {
    try {
        var tmpJson = null;

        for (var ind = 0; ind < jsonLst.length; ind++) {
            var jsonItm = eval('(' + jsonLst[ind] + ')');
            if (jsonItm.id == id) {
                tmpJson = jsonItm;
                break;
            }
        }

        return tmpJson;
    }
    catch (err) {
        alert('getJsonRecord: ' + err.description);
    }
}

function loadComboBox(jsonLst, cbx, defaultOptionName, excludeDefaultOption) {
    try {
        //alert(defaultOptionName);
        removeAllChildNodes(cbx);

        if (excludeDefaultOption == undefined) {
            excludeDefaultOption = false;
        }
        if (excludeDefaultOption == false) {
            var opt0 = document.createElement('option');
            $(opt0).attr('data-id', 0);
            $(opt0).attr('value', 0);
            cbx.appendChild(opt0);
            var opt0TXT = document.createTextNode('[' + defaultOptionName + ']');

            opt0.appendChild(opt0TXT);
        }

        for (var ind = 0; ind < jsonLst.length; ind++) {
            var jsonItm = '';
            try {
                jsonItm = eval('(' + jsonLst[ind] + ')');
            }
            catch (err) {
                jsonItm = jsonLst[ind];
            }
            var cbxOption = document.createElement('option');
            cbx.appendChild(cbxOption);
            $(cbxOption).attr('value', jsonItm.id);
            $(cbxOption).attr('data-id', jsonItm.id);
            var cbxOptionTxt = document.createTextNode(jsonItm.name);
            cbxOption.appendChild(cbxOptionTxt);
        }

        return cbx;
    }
    catch (err) {
        alert('loadComboBox: ' + err.description);
    }
}

function getDeviceFromJson(deviceId) {
    try {
        var jsonDevice = getDeviceById(deviceId);


//        for (var ind = 0; ind < jsonDevices.myDevices.length; ind++) {
//            jsonDevice = eval('(' + jsonDevices.myDevices[ind] + ')');

//            if (deviceId == jsonDevice.deviceId) {
//                break;
//            }
//        }

        return jsonDevice;
    }
    catch (err) {
        alert('getDeviceFromJson: ' + err.description);
    }
}

function speedingColor(speed) {
    try {
        var clr = '#ffffff';

        if (speed <= 55) {
            clr = '#ffffff';
        }
        else {
            if (speed > 55 && speed <= 65) {
                clr = '#ffff00';
            }
            else {
                if (speed > 65 && speed <= 75) {
                    clr = '#ffa500';
                }
                else {
                    clr = '#ff0000';
                }
            }
        }

        return clr;
    }
    catch (err) {
        alert('speedingColor: ' + err.description);
    }
}

function eventColor(eventCode) {
    try {
        var clr = '#ffffff';

        switch (eventCode) {
            case '01':
                clr = '#7bccfb';
                break;
            case '02':
                clr = '#fc655a';
                break;
            case '03':
                clr = '#51ec51';
                break;
            case '04':
                clr = '#ffff00';
                break;
            case '05':
                clr = '#fc655a';
                break;
            case '08':
                clr = '#aaf200';
                break;
            case '09':
                clr = '#ff6600';
                break;
            case '43':
                clr = '#E18E2E';
                break;
            case '45':
                clr = '#ffff00';
                break;
        }

        return clr;
    }
    catch (err) {
        alert('eventColor: ' + err.description);
    }
}

function setWelcomeTitle() {
    try {
        $('#welcomeTitleSpan').text(welcomeTitle);
        if ($('#userFullName').length > 0) {
            $('#userFullName').val(userFullName);
        }
    }
    catch (err) {
        alert('setWelcomeTitle: ' + err.description);
    }
}

//Points are given in Google LatLng format
function distanceTo(point1, point2) {
    try {
        //return haversineDistanceTo(point1, point2);
        //We'll use for now this faster routine
        return equirectangularDistanceTo(point1, point2);
    }
    catch (err) {
        alert('distanceTo: ' + err.description);
    }
}

//Haversine formula
function haversineDistanceTo(point1, point2) {
    try {
        var R = 6371;
        var lat1 = point1.x.toRad(), lon1 = point1.y.toRad();
        var lat2 = point2.x.toRad(), lon2 = point2.y.toRad();
        var dLat = lat2 - lat1;
        var dLon = lon2 - lon1;

        var x = (lon2 - lon1) * Math.cos((lat1 + lat2) / 2);
        var y = (lat2 - lat1);
        var d = Math.sqrt(x * x + y * y) * R;

        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
          Math.cos(lat1) * Math.cos(lat2) *
          Math.sin(dLon / 2) * Math.sin(dLon / 2);
        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        var d = R * c;

        return d.toPrecisionFixed(4);
    }
    catch (err) {
        alert('haversineDistanceTo: ' + err.description);
    }
}

//Equirectangular formula, based on Pythagoras theorem.  Faster/less calculations/less accuracy too, but good enough in small places
function equirectangularDistanceTo(point1, point2) {
    try {
        var R = 6371;
        var lat1 = point1.lat().toRad(), lon1 = point1.lng().toRad();
        var lat2 = point2.lat().toRad(), lon2 = point2.lng().toRad();
        var dLat = lat2 - lat1;
        var dLon = lon2 - lon1;

        var x = dLon * Math.cos((lat1 + lat2) / 2);
        var y = dLat;
        //Distance converted to miles
        var d = Math.sqrt(x * x + y * y) * R * 0.621371192;

        return d.toFixed(2);
    }
    catch (err) {
        alert('equirectangularDistanceTo: ' + err.description);
    }
}

/** Converts numeric degrees to radians */
if (typeof (Number.prototype.toRad) === "undefined") {
    Number.prototype.toRad = function () {
        return this * Math.PI / 180;
    }
}

//============================================================================

function buildAddress(street, city, state, postalCode) {
    try {
        var address = '';

        //Street
        if (street != null) {
            if (street.length > 0) {
                address = street;
            }
        }
        //City
        if (city != null) {
            if (city.length > 0) {
                if (address.length == 0) {
                    address = city;
                }
                else {
                    address = address + ', ' + city;
                }
            }
        }
        //State
        if (state != null) {
            if (state.length > 0) {
                if (address.length == 0) {
                    address = state;
                }
                else {
                    address = address + ', ' + state;
                }
            }
        }
        //Postal Code
        if (postalCode != null) {
            if (postalCode.length > 0) {
                if (address.length == 0) {
                    address = postalCode;
                }
                else {
                    address = address + ', ' + postalCode;
                }
            }
        }

        return address;
    }
    catch (err) {
        alert('buildAddress: ' + err.description);
    }
}

function buildDispatchAddress() {
    try {
        var street = $('#dispatchStreet').attr('value');
        var city = $('#dispatchCity').attr('value');
        var state = $('#dispatchState').attr('value');
        var postalCode = $('#dispatchPostalCode').attr('value');

        return buildAddress(street, city, state, postalCode);
    }
    catch (err) {
        alert('buildDispatchAddress: ' + err.description);
    }
}

function buildGeofenceFormAddress() {
    try {
        var street = $('#geoFormStreet').attr('value');
        var city = $('#geoFormCity').attr('value');
        var state = $('#geoFormState').attr('value');
        var postalCode = $('#geoFormPostalCode').attr('value');

        return buildAddress(street, city, state, postalCode);
    }
    catch (err) {
        alert('buildDispatchAddress: ' + err.description);
    }
}

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


