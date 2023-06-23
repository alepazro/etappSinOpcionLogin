var _easiTrackAddOn = new function () {
    var etAddOn = {};
    window.etAddOn = etAddOn;

    var easiTrackAPIKey = false, googleAPIKey = false;
    var map = false;
    var infowindow = null;
    var jsonDevices = false;

    etAddOn.loadjQuery = function (callback) {
        var script_tag = document.createElement('script');
        script_tag.setAttribute("src", 'https://code.jquery.com/jquery-1.9.1.min.js');
        script_tag.setAttribute("type", 'text/javascript');
        script_tag.onload = callback; // Run callback once jQuery has loaded
        document.getElementsByTagName("head")[0].appendChild(script_tag);
    }

    etAddOn.getDevicesOk = function (data, textStatus, jqXHR) {
        var a = 1;
    }

    etAddOn.getDevicesError = function (jqXHR, textStatus, errorThrown) {
        var a = 1;
    }

    etAddOn.getDevices = function () {
        var data = 't=' + easiTrackAPIKey;
        jsonDevices = $.ajax({
            url: 'https://etws.elasticbeanstalk.com/devList',
            data: data,
            dataType: 'json',
            type: 'GET',
            success: etAddOn.getDevicesOk,
            error: etAddOn.getDevicesError,
            async: true 
        });
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
            etAddOn.getDevices();
            //getCurrentZoomLevel();
        });
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
        if (typeof jQuery == 'undefined') {
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
