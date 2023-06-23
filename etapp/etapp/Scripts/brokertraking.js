var pmap = null;
var pmarkerDevice = null;
var pmarkertarget = null;
var pmarkertargetpickup = null;
var pmarkerStop = null;
var traking = "";
function gettraking(traking) {
    let data1;
    try {
        $.ajax({
            url: 'https://localhost:44385/Brokers.svc/gettn/' + traking,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                data1 = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {

                data1 = null;
                alert('Failed to fetch device info');
            },
            async: false
        });

        return data1;
    }
    catch (err) {

        console.log(err)
    }
}
function changeMarkerPosition(marker, info) {
    var latlng = new google.maps.LatLng(info.Latitude, info.Longitude);
    marker.setPosition(latlng);
}
function LatLngBounds(info, map) {
    
    var puntos = [];
    puntos.push(new google.maps.LatLng(info.PickupLatitude, info.PickupLongitude));
    puntos.push(new google.maps.LatLng(info.DeliveryLatitud, info.DeliveryLongitud));
    puntos.push(new google.maps.LatLng(info.Latitude, info.Longitude));
    
    $.each(info.Stops, function (ind, elem) {
        
        puntos.push(new google.maps.LatLng(elem.PickupAddresscoordinatesLat, elem.PickupAddresscoordinatesLng));
        
    });
    
    // Crea un objeto LatLngBounds
    var bounds = new google.maps.LatLngBounds();
    // Agrega cada punto al objeto LatLngBounds
    for (var i = 0; i < puntos.length; i++) {
        bounds.extend(puntos[i]);
    }
    // Ajusta el zoom del mapa para que se ajuste a los límites de los puntos
    map.fitBounds(bounds);
}




function initMap(info) {
    
    //var pzoom = parseInt(localStorage.getItem("zoom"));
    //if (pzoom == 0) {
    //    pzoom = 4;
    //}
    pmap = new google.maps.Map(document.getElementById('mapbrokertraking'), {
        center: { lat: info.DeliveryLatitud, lng: info.DeliveryLongitud },
        zoom: 4
    });
}
function marker(info) {
    const infoWindowpmarkertargetpickup = new google.maps.InfoWindow();
    const infoWindowpmarkertarget = new google.maps.InfoWindow();
    const infoWindowDevice = new google.maps.InfoWindow();
    var infoWindowsStops;

    const iconp = {
        url: "/images/pickupaddress2.png", // url
        scaledSize: new google.maps.Size(50, 50), // scaled size
        origin: new google.maps.Point(0, 0), // origin
        anchor: new google.maps.Point(0, 0) // anchor
    };
    const icond = {
        url: "/images/Deliveryaddress2.png", // url
        scaledSize: new google.maps.Size(50, 50), // scaled size
        origin: new google.maps.Point(0, 0), // origin
        anchor: new google.maps.Point(0, 0) // anchor
    };
    const icondevice = {
        url: info.IconDevice, // url
        scaledSize: new google.maps.Size(32, 32), // scaled size
        origin: new google.maps.Point(0, 0), // origin
        anchor: new google.maps.Point(0, 0) // anchor
    };

    pmarkertargetpickup = new google.maps.Marker({
        position: { lat: info.PickupLatitude, lng: info.PickupLongitude },
        map: pmap,
        title: "Pick up Address --> " + info.PickupAddress,
        icon: iconp
    });

    pmarkertarget = new google.maps.Marker({
        position: { lat: info.DeliveryLatitud, lng: info.DeliveryLongitud },
        map: pmap,
        title: "Delivery Address --> " + info.DeliveryAddress,
        icon: icond
    });
    pmarkerDevice = new google.maps.Marker({
        position: { lat: info.Latitude, lng: info.Longitude },
        map: pmap,
        title: "Devices --> " + info.Name,
        icon: icondevice
    });


    pmarkertargetpickup.addListener("click", () => {
        infoWindowpmarkertargetpickup.close();
        infoWindowpmarkertargetpickup.setContent(pmarkertargetpickup.getTitle());
        infoWindowpmarkertargetpickup.open(pmarkertargetpickup.getMap(), pmarkertargetpickup);
    });
    pmarkertarget.addListener("click", () => {
        infoWindowpmarkertarget.close();
        infoWindowpmarkertarget.setContent(pmarkertarget.getTitle());
        infoWindowpmarkertarget.open(pmarkertarget.getMap(), pmarkertarget);
    });
    pmarkerDevice.addListener("click", () => {
        infoWindowDevice.close();
        infoWindowDevice.setContent(pmarkerDevice.getTitle());
        infoWindowDevice.open(pmarkerDevice.getMap(), pmarkerDevice);
    });

    
    const icon = {
        url: "/images/BrokerStop2.png", // url
        scaledSize: new google.maps.Size(25, 25), // scaled size
        origin: new google.maps.Point(0, 0), // origin
        anchor: new google.maps.Point(0, 0) // anchor
    };
    $.each(info.Stops, function (ind, elem) {
                
        infoWindowsStops = new google.maps.InfoWindow();
        pmarkerStop = new google.maps.Marker({
            position: { lat: elem.PickupAddresscoordinatesLat, lng: elem.PickupAddresscoordinatesLng },
            map: pmap,
            title: "Stop --> " + elem.PickupAddress + " Date: " + elem.Pickupdetetime,
            icon: icon
        });
        pmarkerStop.addListener("click", () => {
            infoWindowsStops.close();
            infoWindowsStops.setContent(pmarkerStop.getTitle());
            infoWindowsStops.open(pmarkerStop.getMap(), pmarkerStop);
        });
    }); 

   
}
function buidtTraking(info) {
    //document.getElementById('Device').innerHTML = info.Device;
    document.getElementById('brokerNumber').innerHTML = info.BrokerNumber;
    document.getElementById('ValidUntil').innerHTML = info.DeliveryDatetime;
    document.getElementById('Observations').innerHTML = "TEST";

    initMap(info);

    marker(info);
    
    LatLngBounds(info, pmap);
    setInterval(function () {
        
        info = gettraking(traking);
        var pzoom = parseInt(localStorage.getItem("zoom"));        
        
        if (info != null) {
            changeMarkerPosition(pmarkerDevice, info);

            if (pzoom <= 4) {
                LatLngBounds(info, pmap);
            } else {
                var pmaplat = pmap.getCenter().lat();
                var pmaplon = pmap.getCenter().lng();
                var zoom = parseInt(localStorage.getItem("zoom"));
                pmap.setCenter({ lat: pmaplat, lng: pmaplon });
                pmap.setZoom(zoom);                    
            }

            //pmap.setCenter({ lat: info.Lat, lng: info.Lng });
        }
    }, 60000);

}
