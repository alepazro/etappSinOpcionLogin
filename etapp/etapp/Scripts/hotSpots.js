var map = false;
var hotSpotsData = [];
var heatmap = false;

function changeGradient() {
    var gradient = [
    'rgba(0, 255, 255, 0)',
    'rgba(0, 255, 255, 1)',
    'rgba(0, 191, 255, 1)',
    'rgba(0, 127, 255, 1)',
    'rgba(0, 63, 255, 1)',
    'rgba(0, 0, 255, 1)',
    'rgba(0, 0, 223, 1)',
    'rgba(0, 0, 191, 1)',
    'rgba(0, 0, 159, 1)',
    'rgba(0, 0, 127, 1)',
    'rgba(63, 0, 91, 1)',
    'rgba(127, 0, 63, 1)',
    'rgba(191, 0, 31, 1)',
    'rgba(255, 0, 0, 1)'
  ]
    heatmap.setOptions({
        gradient: heatmap.get('gradient') ? null : gradient
    });
}
function hotSpotsDataOk(data, textStatus, jqXHR) {
    try {
        
        var template = '';
        if (data[0].isOk == true) {
            hotSpotsData.length = 0;
            if (heatmap) {
                heatmap.setMap(null);
                heatmap = false;
            }

            var bounds = new google.maps.LatLngBounds();

            for (ind = 0; ind < data.length; ind++) {
                var hsLatLng = new google.maps.LatLng(data[ind].lat, data[ind].lng);
                hotSpotsData.push({ location: hsLatLng, weight: data[ind].qty });
                bounds.extend(hsLatLng);
            }

            map.fitBounds(bounds);
            google.maps.event.addListenerOnce(map, 'idle', function () {
                heatmap = new google.maps.visualization.HeatmapLayer({
                    data: hotSpotsData
                });
                heatmap.setMap(map);
                heatmap.setOptions({ radius: heatmap.get('radius') ? null : 20 });
                changeGradient();
            });
                        
            template = _.template($("#hotSpotEvents-template").html(), { spots: data });
            $('#hotSpotsDet').html(template);

        }
        else {
            template = _.template($('#noDataFound-template').html());
            $('#hotSpotsDet').html(template);

        }
    }
    catch (err) {
        console.log(err);
        alert('hotSpotsDataOk: ' + err.description);
    }
}

function hotSpotsDataError(jqXHR, textStatus, errorThrown) {
    try {
        var a = 1;
    }
    catch (err) {
        alert('hotSpotsDataError: ' + err.description);
    }
}

function getHotSpots() {
    
    try {
        $('#hotSpotsDet').empty();
        var token = getTokenCookie('ETTK');
        var deviceId = $('#cbxDevices').val();
        var data = 'deviceId=' + escape(deviceId);
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/etrack.svc/getHotSpots/' + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: hotSpotsDataOk,
            error: hotSpotsDataError,
            async: true
        });
    }
    catch (err) {
        alert('getHotSpots: ' + err.description);
    }
}

function getHotSpotDevicesOk(data) {
    try {
        var jsonDevices = data;
        $.each(jsonDevices, function (i, value) {
            $('#cbxDevices').append($('<option>').text(value.value).attr('value', value.id));
        });
    }
    catch (err) {
    }
}

function getHotSpotsDevices() {
    try {
         getBasicList('devices', getHotSpotDevicesOk);
    }
    catch (err) {
    }
}

function initMap(canvas) {
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
            maxZoom: 15,
            center: cntr,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
        map = new google.maps.Map(document.getElementById(canvas), myOptions);
        map.fitBounds(bounds);

        //infowindow = new google.maps.InfoWindow();

        google.maps.event.addListener(map, 'idle', function () {
            //getCurrentZoomLevel();
        });
    }
    catch (err) {
        alert('initialize: ' + err.description);
    }
}