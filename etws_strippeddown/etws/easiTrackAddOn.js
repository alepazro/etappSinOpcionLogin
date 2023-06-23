var _easiTrackAddOn = new function () {
    var etAddOn = {};
    window.etAddOn = etAddOn;

    var easiTrackAPIKey = false, googleAPIKey = false;
    var map = false;
    var infowindow = null;
    var jsonDevices = false;
    var markersArray = [];
    var infoWindowsArray = [];

    etAddOn.loadjQuery = function (callback) {
        var script_tag = document.createElement('script');
        script_tag.setAttribute("src", 'https://code.jquery.com/jquery-1.9.1.min.js');
        script_tag.setAttribute("type", 'text/javascript');
        script_tag.onload = callback; // Run callback once jQuery has loaded
        document.getElementsByTagName("head")[0].appendChild(script_tag);
    }

    etAddOn.deleteOverlays = function () {
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

    etAddOn.getDevicesOk = function (data, textStatus, jqXHR) {
        etAddOn.deleteOverlays();
        if (data.length > 0) {

            var bounds = new google.maps.LatLngBounds();

            for (var devInd = 0; devInd < data.length; devInd++) {

                var dev = data[devInd];
                var devLatLng = new google.maps.LatLng(dev.latitude, dev.longitude);
                var marker = false;
                marker = new google.maps.Marker({ position: devLatLng, map: map, title: dev.name });

                //markersArray.push(marker);
                markersArray[dev.id] = marker;

                //Adjust viewport
                bounds.extend(devLatLng);

                //Shows the marker
                marker.setMap(map);
            }

            map.fitBounds(bounds);
            google.maps.event.addListenerOnce(map, 'idle', function () {
                if (data.length === 1) {
                    map.setZoom(18);
                }
            });
        }
    }

    etAddOn.getDevicesError = function (jqXHR, textStatus, errorThrown) {
        var a = 1;
    }

    etAddOn.getDevices = function () {
        jsonDevices = $.ajax({
            type: "GET",
            url: 'https://pre.etrack.ws/etrest.svc/easiTrackMap/' + escape(easiTrackAPIKey),
            contentType: "application/json; charset=utf-8",
            data: 0,
            crossDomain: true,
            dataType: "jsonp",
            processdata: false,
            success: etAddOn.getDevicesOk,
            error: etAddOn.getDevicesError,
            async: true
        });
    }

    etAddOn.updateMap = function () {
        etAddOn.getDevices();
    }

    etAddOn.init3 = function () {
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
        map = new google.maps.Map(document.getElementById('easitrack'), myOptions);
        map.fitBounds(bounds);

        infowindow = new google.maps.InfoWindow();

        google.maps.event.addListener(map, 'idle', function () {
            //getCurrentZoomLevel();
        });

        etAddOn.updateMap();
        setInterval("etAddOn.updateMap()", 1 * 60 * 1000);

    };

    etAddOn.init2 = function () {
        easiTrackAPIKey = $('#easitrack').attr('data-easiTrackAPI');
        googleAPIKey = $('#easitrack').attr('data-googleAPI');

        //Load Google Maps
        var script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "https://maps.googleapis.com/maps/api/js?key=" + googleAPIKey + "&sensor=false&callback=etAddOn.init3";
        document.body.appendChild(script);
    };

    this.init = function () {
        //Load jQuery
        if (typeof jQuery === 'undefined') {
            // jQuery is not loaded
            etAddOn.loadjQuery(etAddOn.init2);
        }
        else {
            etAddOn.init2();
        }
    }

};

// Calling initial function
_easiTrackAddOn.init();
