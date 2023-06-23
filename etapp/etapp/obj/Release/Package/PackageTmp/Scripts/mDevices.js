var map = false;
var marker = false;
var infoWindow = false;

$('div:jqmData(url="mDevicesList.html")').live('pageshow', function () {
    // code to execute on that page
    //$(this) works as expected - refers the page
    if (validateToken(true) == true) {
        getDevices();
        mLoadDevicesList();
    }
});

$(document).bind("pagebeforechange", function (event, data) {
    $.mobile.pageData = (data && data.options && data.options.pageData)
        ? data.options.pageData
        : null;
});

$("#mMaphtml").live("pagebeforeshow", function (e, data) {
    try {
        if ($.mobile.pageData && $.mobile.pageData.id) {
            var deviceId = $.mobile.pageData.id;
            var lat = $.mobile.pageData.lat;
            var lng = $.mobile.pageData.lng;
            var iconUrl = unescape($.mobile.pageData.iconUrl);
            var iconName = unescape($.mobile.pageData.name);
            var infoTable = unescape($.mobile.pageData.info);
            $('#mMapDeviceId').val(deviceId);
            $(document.getElementById('map_canvas')).attr('style', 'height:' + (h - 20) + 'px;width:' + (w - 0) + 'px;');
            initialize(lat, lng);
            updateMarkerPosition(lat, lng, iconName, iconUrl, infoTable);
        }
    }
    catch (err) {
        alert('mMap_pagebeforeshow' + err.description);
    }
});

$("#mMaphtml").live("pageshow", function (e, data) {
    try {
        //google.maps.event.trigger(map, 'resize');
    }
    catch (err) {
        alert('mMap_pageshow' + err.description);
    }
});

function updateMarkerPosition(lat, lng, iconName, iconUrl, infoTable) {
    try {
        var devLatLng = new google.maps.LatLng(lat, lng);
        if (marker == false) {
            marker = new google.maps.Marker({ position: devLatLng, map: map, title: iconName, icon: iconUrl });
        }
        else {
            marker.setPosition(devLatLng);
            marker.setTitle(iconName);
            marker.setIcon(iconUrl);
            marker.setMap(map);
        }

        (function (marker, content) {
            google.maps.event.addListener(marker, 'click', function () {
                if (!infoWindow) {
                    infoWindow = new google.maps.InfoWindow();
                }
                infoWindow.setContent(content);
                infoWindow.open(map, marker);
            });
        })(marker, infoTable);

        var bounds = new google.maps.LatLngBounds();
        bounds.extend(devLatLng);
        map.fitBounds(bounds);
        google.maps.event.addListenerOnce(map, 'idle', function () {
            if (map.getZoom() > 18) {
                map.setZoom(18);
                google.maps.event.trigger(map, 'resize');
            }
        });
    }
    catch (err) {
        alert('updateMarkerPosition: ' + err.description);
    }
}

function initialize(lat, lng) {
    try {
        
        var latlng = new google.maps.LatLng(lat, lng);
        var myOptions = {
            zoom: 8,
            center: latlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
        google.maps.event.trigger(map, 'resize');
    }
    catch (err) {
        alert('initialize: ' + err.description);
    }
}

function mRefreshDevicesList() {
    try {
        getDevices();
        mLoadDevicesList();
    }
    catch (err) {
        alert('Could not refresh list');
    }
}

function mLoadDevicesList() {
    try {
        var ul = document.getElementById('fleetList');
        removeAllChildNodes(ul);

        for (var ind = 0; ind < jsonDevices.myDevices.length; ind++) {
            jsonDevice = eval('(' + jsonDevices.myDevices[ind] + ')');

            var li = document.createElement('li');
            $(li).attr('id', 'deviceId' + jsonDevice.deviceId);
            $('#fleetList').append(li);

            var iconImg = document.createElement('img');
            $(iconImg).attr('src', jsonDevice.iconUrl);
            $(iconImg).attr('alt', '');
            $(iconImg).attr('width', '24');
            $(iconImg).attr('height', '24');
            $(iconImg).prop('class', 'ui-li-icon');
            $(li).append(iconImg);

            var a = document.createElement('a');
            $(a).attr('href', 'mMap.html?id=' + jsonDevice.deviceId + '&lat=' + escape(jsonDevice.latitude) + '&lng=' + jsonDevice.longitude + '&name=' + escape(jsonDevice.name) + '&iconUrl=' + escape(jsonDevice.iconUrl) + '&info=' + escape(jsonDevice.infoTable));
            $(li).append(a);

            var div = document.createElement('div');
            $(div).attr('style', 'padding-left:20px;');
            $(a).append(div);

            var p0 = document.createElement('p');
            $(p0).attr('style', 'font-size:14px;font-weight:600;margin:5px;');
            $(p0).text(jsonDevice.name + ' (' + jsonDevice.eventName + ')')
            $(div).append(p0);

            var p1 = document.createElement('p');
            $(p1).text(jsonDevice.fullAddress);
            $(div).append(p1);

            var p2 = document.createElement('p');
            $(p2).text(jsonDevice.speed + ' mph heading ' + jsonDevice.heading + ' at ' + jsonDevice.eventDateString);
            $(div).append(p2);

            //This is a hidden div with info on each device, used to pass data to the next form
            var dataDiv = document.createElement('div');
            $(dataDiv).attr('style', 'display:none;');
            $(div).append(dataDiv);

            var dataDeviceId = document.createElement('span');
            $(dataDeviceId).attr('id', 'dataDeviceId');
            $(dataDeviceId).html(jsonDevice.deviceId)
            $(dataDiv).append(dataDeviceId);

            var dataDeviceName = document.createElement('span');
            $(dataDeviceName).attr('id', 'dataDeviceName');
            $(dataDeviceName).html(jsonDevice.name)
            $(dataDiv).append(dataDeviceName);

        }
        $('#fleetList').listview('refresh');
    }
    catch (err) {
        alert('Devices List not loaded');
    }
}