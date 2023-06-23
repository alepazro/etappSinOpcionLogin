var map = false;
var infowindow = null;
var markersArray = [];
var infoWindowsArray = [];
var bAutoCenter = true;
var bAutoZoomFreezed = false;
var geocoder = null;
var directionsService = null;
var directionsResult = null;
var dispatchLastAddress = null;
var dispatchLastLoc = null;
var dispatchLastMarker = null;
var dispatchGoogleResults = null;
var directionsResultArray = [];
var directionsRenderer = null;
var breadcrumbResult = null;
var breadcrumbPath = null;
var polyline = null;
var polylineMarkers = []; 
var polylineDet = [];
var userZoomLevel = 0;

function getCurrentZoomLevel() {
    try {
        userZoomLevel = map.getZoom();
    }
    catch (err) {
        alert('getCurrentZoomLevel: ' + err.description);
    }
}

// Deletes all markers in the array by removing references to them
function deleteOverlays() {
    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(null);
        }
        markersArray.length = 0;
    }
    if (infoWindowsArray) {
        infoWindowsArray.length = 0;
    }
}
//=============================================================================

function createFleetMarkers() {
    try {
        
        deleteOverlays();

        var labelContent = '';

        if (jsonDevices.myDevices.length > 0) {

            var bounds = new google.maps.LatLngBounds();

            for (var devInd = 0; devInd < jsonDevices.myDevices.length; devInd++) {
                var dev = eval('(' + jsonDevices.myDevices[devInd] + ')');
                if (dev.isNotWorking == false) {
                    var devLatLng = new google.maps.LatLng(dev.latitude, dev.longitude);
                    var txtColor = dev.txtColor;
                    var bgndColor = dev.bgndColor;
                    var marker = false;
                    if (dev.shortName != '') {
                        if (dev.iconLabelLine2 != '') {
                            labelContent = '<span>' + dev.shortName + '<br />' + dev.iconLabelLine2 + '</span>';
                        }
                        else {
                            labelContent = dev.shortName;
                        }
                        marker = new MarkerWithLabel({
                            position: devLatLng,
                            draggable: false,
                            map: map,
                            title: dev.name,
                            icon: dev.iconUrl,
                            labelContent: labelContent,
                            labelAnchor: new google.maps.Point(22, 0),
                            labelClass: "deviceLabel", // the CSS class for the label
                            labelStyle: { opacity: 1.00, 'color': txtColor, 'background-color': bgndColor }
                        });
                    }
                    else {
                        marker = new google.maps.Marker({ position: devLatLng, map: map, title: dev.name, icon: dev.iconUrl });
                    }

                    //markersArray.push(marker);
                    markersArray[dev.deviceId] = marker;

                    var content = dev.infoTable;
                    var deviceId = dev.deviceId;
                    (function (marker, content, deviceId) {
                        
                        //The general claim is to have infowindows triggered by a click rather than by a mouseover event.
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }

                            try{
                                var infoWin = getMethodSync('getInfoWindow', deviceId)

                                updateDeviceInFleetList(infoWin);
                                updateDeviceMarkerPosition(infoWin);
                                updateActiveInfoWindow(infoWin);

                                infowindow.deviceId = deviceId;
                                infowindow.setContent(infoWin.infoTable);
                                infowindow.open(map, marker);
                            }
                            catch (err) {

                            }
                        });

                        //2015.4.11  Lines commented because this is causing the infobubble to hide so it is not possible to click on the internal buttons/links.
                        //google.maps.event.addListener(marker, 'mouseout', function () {
                        //    infowindow.close();
                        //});
                    })(marker, content, deviceId);

                    //Adjust viewport
                    bounds.extend(devLatLng);

                    //Shows the marker
                    marker.setMap(map);

                }
                
            }
            
            map.fitBounds(bounds);
            google.maps.event.addListenerOnce(map, 'idle', function () {
                
                if (jsonDevices.myDevices.length == 1) {
                    map.setZoom(19);
                    map.initialZoom = false;
                    getCurrentZoomLevel();
                }
                var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
                if (preferences.autoZoom == true) {
                    map.setZoom(preferences.autoZoomLevel);
                    getCurrentZoomLevel();
                }
            });
        }
    }
    catch (err) {
        alert('createFleetMarkers: ' + err.description);
    }
}
function createFleetMarkersGroupsNew() {    
    try {        
        deleteOverlays();        
        var labelContent = '';
        if (jsonDevicesGroupsNew.myDevices.length > 0) {
            var bounds = new google.maps.LatLngBounds();            
            for (var devInd = 0; devInd < jsonDevicesGroupsNew.myDevices.length; devInd++) {
                var dev = eval('(' + jsonDevicesGroupsNew.myDevices[devInd] + ')');                
                    if (dev.isNotWorking == false) {
                        var devLatLng = new google.maps.LatLng(dev.latitude, dev.longitude);
                        var txtColor = dev.txtColor;
                        var bgndColor = dev.bgndColor;
                        var marker = false;
                        if (dev.shortName != '') {
                            if (dev.iconLabelLine2 != '') {
                                labelContent = '<span>' + dev.shortName + '<br />' + dev.iconLabelLine2 + '</span>';
                            }
                            else {
                                labelContent = dev.shortName;
                            }
                            marker = new MarkerWithLabel({
                                position: devLatLng,
                                draggable: false,
                                map: map,
                                title: dev.name,
                                icon: dev.iconUrl,
                                labelContent: labelContent,
                                labelAnchor: new google.maps.Point(22, 0),
                                labelClass: "deviceLabel", // the CSS class for the label
                                labelStyle: { opacity: 1.00, 'color': txtColor, 'background-color': bgndColor }
                            });

                        }
                        else {
                            marker = new google.maps.Marker({ position: devLatLng, map: map, title: dev.name, icon: dev.iconUrl });
                        }                        
                        //markersArray.push(marker);
                        markersArray[dev.deviceId] = marker;
                        var content = dev.infoTable;
                        var deviceId = dev.deviceId;                         
                        (function (marker, content, deviceId) {
                                //The general claim is to have infowindows triggered by a click rather than by a mouseover event.                        
                                google.maps.event.addListener(marker, 'click', function () {
                                    if (!infowindow) {
                                        infowindow = new google.maps.InfoWindow();
                                    }
                                    try {
                                        var infoWin = getMethodSync('getInfoWindow', deviceId)
                                        updateDeviceInFleetList(infoWin);
                                        updateDeviceMarkerPosition(infoWin);
                                        updateActiveInfoWindow(infoWin);

                                        infowindow.deviceId = deviceId;
                                        infowindow.setContent(infoWin.infoTable);
                                        infowindow.open(map, marker);
                                    }
                                    catch (err) {

                                    }
                                });

                                //2015.4.11  Lines commented because this is causing the infobubble to hide so it is not possible to click on the internal buttons/links.
                                //google.maps.event.addListener(marker, 'mouseout', function () {
                                //    infowindow.close();
                                //});
                            })(marker, content, deviceId);                        
                        //Adjust viewport
                        bounds.extend(devLatLng);
                        //Shows the marker   
                        if (dev.showDevice) {
                            marker.setMap(map);
                        } else {
                            marker.setMap(null);
                        }
                    }
                    console.log('contador: ' + devInd)
            }
            map.fitBounds(bounds);            
            google.maps.event.addListenerOnce(map, 'idle', function () {
                
                if (jsonDevicesGroupsNew.myDevices.length == 1) {
                    map.setZoom(19);
                    map.initialZoom = false;
                    getCurrentZoomLevel();
                }
                var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
                if (preferences.autoZoom == true) {
                    map.setZoom(preferences.autoZoomLevel);
                    getCurrentZoomLevel();
                }
            });
            
            
        }
    }
    catch (err) {
        
        alert('createFleetMarkers: ' + err);
        console.log('createFleetMarkers: ' + err)
    }
}

function initialize(canvas) {
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
            maxZoom: 19,
            center: cntr,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
        map = new google.maps.Map(document.getElementById(canvas), myOptions);
        map.fitBounds(bounds);

        infowindow = new google.maps.InfoWindow();

        google.maps.event.addListener(infowindow, 'closeclick', function () {
            closeInfoWindow();
        });

        google.maps.event.addListener(map, 'idle', function () {
            
            getCurrentZoomLevel();
        });

        google.maps.event.addListener(map, 'zoom_changed', function () {
            //If auto-zoom is set, save this new zoom level
            
            if (_.isObject(jsonAppFeatures.userPreferences)) {
                var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
                if (preferences.autoZoom == true) {
                    //toggleFreezeAutoZoom(false);
                    //jsonAppFeatures = false;
                    //getAppFeatures(1);
                }
            }
        });
        google.maps.event.addListener(map, 'dragend', function () {
            //If auto-zoom is set, save this new zoom level
            
            if (_.isObject(jsonAppFeatures.userPreferences)) {
                //var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
                //if (preferences.autoZoom == true) {
                //    toggleFreezeAutoZoom(false);
                //    jsonAppFeatures = false;
                //    getAppFeatures(1);
                //}
            }
        });


    }
    catch (err) {
        alert('initialize: ' + err.description);
    }
}
function initializeFreeze(coordinates,val) {
    try {
        
        var bounds = false;
        var cntr = false;
        var bounds1 = null;
        var southWest = null;
        var northEast = null;
        var myOptions = {};


        if (val) {
            freeze = val;
            try {
                bounds1 = coordinates.getBounds();
                southWest = bounds1.getSouthWest();
                northEast = bounds1.getNorthEast();
            } catch (error) {
                southWest = new google.maps.LatLng(25, -123.20);
                northEast = new google.maps.LatLng(43, -75.20);
            }
            // Create a bounding box
            bounds = new google.maps.LatLngBounds(southWest, northEast);
            cntr = bounds.getCenter();
            myOptions = {
                zoom: coordinates.getZoom(),
                maxZoom: 19,
                center: cntr,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                draggable: true,
                zoomControl: false,
                scrollwheel: false,
                disableDoubleClickZoom: true,

            }
            map.setOptions(myOptions);
            map.fitBounds(bounds);
        } else {
            freeze = false;
            myOptions = {
                zoom: coordinates.getZoom(),
                maxZoom: 19,
                center: cntr,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                draggable: true,
                zoomControl: true,
                scrollwheel: true,
                disableDoubleClickZoom: false,

            }
            map.setOptions(myOptions);
        }
    }
    catch (err) {
        alert('initializeFreeze: ' + err.description);
    }
}

function openDeviceInfoWindow(deviceId) {
    try {
        infowindow.deviceId = deviceId;
        var jsonDevice = getDeviceFromJson(deviceId);
        var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
        //var opt = google.maps.InfoWindowOptions({content: 'test', position:devLatLng});
        infowindow.setContent(jsonDevice.infoTable);
        infowindow.setPosition(devLatLng);
        infowindow.open(map);
        //map.panTo(devLatLng);
    }
    catch (err) {
        alert('openDeviceInfoWindow: ' + err.description);
    }
}

function closeInfoWindow() {
    try {
        infowindow.deviceId = '';
    }
    catch (err) {
        alert('closeInfoWindow: ' + err.description);
    }
}

function autoCenter() {
    try {
        
        if (bAutoCenter == true && bAutoZoomFreezed == false) {
            var intCount = 0;

            //From a good fellow here: you.arenot.me/2010/06/29/google-maps-api-v3-0-multiple-markers-multiple-infowindows/
            //  Create a new viewpoint bound
            var bounds = new google.maps.LatLngBounds();

            //  Go through each device...
            for (i in markersArray) {
                if (markersArray[i].map != null) {
                    bounds.extend(markersArray[i].position);
                    intCount += 1;
                }
            }

            //        $.each(markersArray, function (index, marker) {
            //            if (marker.map != null) {
            //                bounds.extend(marker.position);
            //            }
            //        });
            //Consider now the current landmark from Dispatch..
            if (dispatchLastLoc) {
                bounds.extend(dispatchLastLoc);
            }

            //  Fit these bounds to the map
            if (intCount > 1) {
                map.fitBounds(bounds);
            }
            else {
                map.setCenter(bounds.getCenter());
                if (userZoomLevel == 0) {
                    map.setZoom(19);
                }
                else {
                    map.setZoom(userZoomLevel);
                }
            }
            google.maps.event.addListenerOnce(map, 'idle', function () {
                if (map.getZoom() > 19) {
                    map.setZoom(19);
                    getCurrentZoomLevel();
                }
            });
            
        }
    }
    catch (err) {
        alert('autoCenter: ' + err.Description);
    }
}

//================================================================
//--- FIND LOCATION

function geocodeDispatchLocationCallback(bAddressFound) {
    try {
        if (bAddressFound == true) {

            var location = GetLocation(dispatchGoogleResults)

            //Update the interface fields
            if ($('#dispatchStreet').val() == '') {
                $('#dispatchStreet').val(location.address1);
            }
            if ($('#dispatchCity').val() == '') {
                $('#dispatchCity').val(location.city);
            }
            if ($('#dispatchState').val() == '') {
                $('#dispatchState').val(location.state);
            }
            if ($('#dispatchPostalCode').val() == '') {
                $('#dispatchPostalCode').val(location.postalCode);
            }

            findClosestDevice(dispatchLastLoc);
            enableDriversList();
            $('#saveDispatchLocation').show();
        }
    }
    catch (err) {
        alert('geocodeDispatchLocationCallback: ' + err.description);
    }
}
function geocodeDispatchLocationCallbackJobNew(bAddressFound) {
    try {
        if (bAddressFound == true) {
            var location = GetLocation(dispatchGoogleResults)

            //Update the interface fields
            if ($('#dispatchStreet').val() == '') {
                $('#dispatchStreet').val(location.address1);
            }
            if ($('#dispatchCity').val() == '') {
                $('#dispatchCity').val(location.city);
            }
            if ($('#dispatchState').val() == '') {
                $('#dispatchState').val(location.state);
            }
            if ($('#dispatchPostalCode').val() == '') {
                $('#dispatchPostalCode').val(location.postalCode);
            }

            //findClosestDevice(dispatchLastLoc);
            //enableDriversList();
            //$('#saveDispatchLocation').show();
        }
    }
    catch (err) {
        alert('geocodeDispatchLocationCallback: ' + err.description);
    }
}
function geocodeLocation(address, bMapIt, callbackFunc) {
    try {
        var bFound = false;

        if (bMapIt == 'undefined') {
            bMapIt = false;
        }

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
                        dispatchGoogleResults = results[0];
                        dispatchLastAddress = address;
                        dispatchLastLoc = results[0].geometry.location;
                        if (!dispatchLastMarker) {
                            dispatchLastMarker = new google.maps.Marker();
                        }
                        dispatchLastMarker.setPosition(dispatchLastLoc);
                        dispatchLastMarker.setIcon('https://easitrack.net/icons/DispatchTo.png');

                        if (bMapIt == true) {
                            map.setCenter(dispatchLastLoc);
                            dispatchLastMarker.setMap(map);
                            infowindow.setContent(dispatchLastAddress);
                            infowindow.setPosition(dispatchLastLoc);
                            infowindow.open(map);
                        }

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
function geocodeLocationJobNew(address, bMapIt, callbackFunc) {
    try {
        
        var bFound = false;

        if (bMapIt == 'undefined') {
            bMapIt = false;
        }

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
                        dispatchGoogleResults = results[0];
                        
                        $("#dispatchStreet").val(results[0].formatted_address);
                        $("#dispatchCity").val(results[0].address_components[3].long_name);
                        $("#dispatchState").val(results[0].address_components[5].long_name);
                        //$("#dispatchPostalCode").val(results[0].address_components[7].long_name);
                        $("#geof_latitude").val(results[0].geometry.location.lat);
                        $("#geof_longitude").val(results[0].geometry.location.lng);
                        console.log($("#geof_latitude").val() + ' lat  long ' + $("#geof_longitude").val());
                        dispatchLastAddress = address;
                        dispatchLastLoc = results[0].geometry.location;
                        if (!dispatchLastMarker) {
                            dispatchLastMarker = new google.maps.Marker();
                        }
                        dispatchLastMarker.setPosition(dispatchLastLoc);
                        dispatchLastMarker.setIcon('https://easitrack.net/icons/DispatchTo.png');

                        if (bMapIt == true) {
                            map.setCenter(dispatchLastLoc);
                            dispatchLastMarker.setMap(map);
                            //infowindow.setContent(dispatchLastAddress);
                            //infowindow.setPosition(dispatchLastLoc);
                            //infowindow.open(map);
                        }
                        findClosestDevice(dispatchLastLoc);
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

function findDispatchLocation() {
    
    try {
        var address = buildDispatchAddress();

        if (address.length > 0) {
            //Avoid search of very same address more than once in a row
            var bGetLoc = false;
            var bFound = false;
            if (dispatchLastAddress != null) {
                if (address.toUpperCase() != dispatchLastAddress.toUpperCase()) {
                    dispatchLastAddress = address;
                    dispatchLastLoc = null;
                    dispatchLastMarker = null;
                    bGetLoc = true;
                }
                else {
                    bFound = true;
                }
            }
            else {
                dispatchLastAddress = address;
                dispatchLastLoc = null;
                dispatchLastMarker = null;
                dispatchGoogleResults = null;
                bGetLoc = true;
            }

            if (bGetLoc == true) {
                bFound = geocodeLocation(address, true, geocodeDispatchLocationCallback);
            }
            else {
                geocodeDispatchLocationCallback(true);
            }

        }

    }
    catch (err) {
        alert('findLocation: ' + err.description);
    }
}
function findDispatchLocationJobNew() {
    
    try {
        var address = buildDispatchAddressJobNew();
        if (address.length > 0) {
            dispatchLastAddress = address.toUpperCase();
            showLocationInMapHere("map_jobLocation", 37.376, -122.034, 15, true, dispatchLastAddress);            
        }
    }
    catch (err) {
        alert('findLocation: ' + err.description);
        console.log("error "+err.description);
    }
}
//=================================================================
//---- GET DIRECTIONS

function getDirections(refId, startPoint, endPoint, callbackFunc) {
    try {
        var request = {
            origin: startPoint,
            destination: endPoint,
            travelMode: google.maps.TravelMode.DRIVING
        };

        if (!directionsService) {
            directionsService = new google.maps.DirectionsService();
        }

        directionsService.route(request, function (result, status) {

            switch (status) {
                case google.maps.DirectionsStatus.OK:
                    //Indicates the response contains a valid DirectionsResult
                    directionsResult = result;

                    directionsResultArray[refId] = directionsResult;

                    callbackFunc(refId, directionsResult);

                    //directionsDisplay.setDirections(result);
                    break;
                case google.maps.DirectionsStatus.NOT_FOUND:
                    //indicates at least one of the locations specified in the requests's origin, destination, or waypoints could not be geocoded.
                    break;
                case google.maps.DirectionsStatus.ZERO_RESULTS:
                    //indicates no route could be found between the origin and destination.
                    break;
                case google.maps.DirectionsStatus.MAX_WAYPOINTS_EXCEEDED:
                    //indicates that too many DirectionsWaypoints were provided in the DirectionsRequest. The maximum allowed waypoints is 8, plus the origin, and destination. Maps API Premier customers are allowed 23 waypoints, plus the origin, and destination.
                    break;
                case google.maps.DirectionsStatus.INVALID_REQUEST:
                    //indicates that the provided DirectionsRequest was invalid.
                    break;
                case google.maps.DirectionsStatus.OVER_QUERY_LIMIT:
                    //indicates the webpage has sent too many requests within the allowed time period.
                    break;
                case google.maps.DirectionsStatus.REQUEST_DENIED:
                    //indicates the webpage is not allowed to use the directions service.
                    break;
                case google.maps.DirectionsStatus.UNKNOWN_ERROR:
                    //indicates a directions request could not be processed due to a server error. The request may succeed if you try again.
                    break;
            }
        });
        
    }
    catch (err) {
        alert('getDirections: ' + err.description);
    }
}

//=================================================================
//--- SOME ROUTINES FOUND IN THE INTERNET THAT MAY BECOME HANDY... 
//================================================================

//----- Adds a marker to the array
function UNUSED_addMarker(location) {
    marker = new google.maps.Marker({ position: location, map: map });
    markersArray.push(marker);
}

// Removes the overlays from the map, but keeps them in the array 
function UNUSED_clearOverlays() {
    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(null);
        }
    }
}
// Shows any overlays currently in the array 
function UNUSED_showOverlays() {
    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(map);
        }
    }
}