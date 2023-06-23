function loadmaps() {
    try {
        //Carga el Script de Labels
        loadScript('js/maps/markerwithlabel.js');

        var mapOptions = {
            center: new google.maps.LatLng(31.1071179, -96.2436523),  //30.1071179   -82.2436523//30.1071179   -82.2436523
            zoom: 5,
            //  zoomControl: true,
            //   panControl: true,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        //alert(mapcanvas);
        app.map = new google.maps.Map(document.getElementById('map'), mapOptions);



    }
    catch (exc) {
        _Show(" loadmaps -->" + exc.message);
    }
}


function maps_callback(rs) {
    try {
        if (rs != null) {
            deleteMarkers();
            if (typeof (rs) != 'undefined') {
                var latlog;
                $.each(rs, function (index, value) {
                    var lat = new String(value.custLat);
                    var lon = new String(value.custLng     );


                    if ((value.custLat != "" && parseInt(lat) != 0) && (parseInt(lon) != 0 && value.custLng != "")) {

                        //busco los que estan en los markers y los actualizo 
                        var obj = _.find(app.markers, function (obj) {
                            return obj.customInfo == value.jobId;
                        });

                        // app.console("markers lat --> " + lat + " lon --> " + lon+" obj ->"+JSON.stringify(obj));
                        //miro si ese objecto es nuevo es decir que no lo encontro uc.markers
                        if (typeof (obj) == 'undefined') {
                            //  app.console('creo marker');
                            var myLatLng = new google.maps.LatLng(lat, lon);
                            latlog = myLatLng;

                        
                            var icon ='default.png';
                            switch(value.statusId){
                                case "1":
                                    icon = 'new.png';
                                    break;
                                case "2":
                                    icon = 'openinprogress.png';
                                    break;
                                case "3":
                                    icon = 'onhold.png';
                                    break;
                                case "4":
                                    icon = 'needsAttn.png';
                                    break;
                                case "5":
                                    icon = 'completed.png';
                                    break;
                                case "6":
                                    icon = 'cancelled.png';
                                    break;
                                case "7":
                                    icon = 'open.png';
                                    break;
                                case "8":
                                    icon = 'openpaused.png';
                                    break;
                                case "9":
                                    icon = 'unassigned.png';
                                    break;

                            }
                          
                            // app.console('nombre del icono -->' + icon);

                            var marker = new MarkerWithLabel({
                                position: myLatLng,
                                draggable: false,
                                raiseOnDrag: false,
                                icon: '/images/MapIcons/' + icon,
                                map: app.map,
                                customInfo: value,
                                jobId: value.jobId
                                //  labelContent: value.MarkerName,
                                //  labelAnchor: new google.maps.Point(22, 0),
                                //   labelClass: "labels", // the CSS class for the label
                            });

                            var valueLabel = ' #'+value.jobNumber;
                          
                            marker.labelContent = valueLabel;
                            marker.labelAnchor = new google.maps.Point(32, 0);
                            marker.labelClass = 'labels'; // the CSS class for the label
                            marker.labelStyle = { color: '#FF0000', opacity: 1, background: '#FFFFFF' };
                           

                            google.maps.event.addListener(marker, 'click', function () {
                                markerClick(marker);
                            });
                            //app.console('markers  lat :' + value.Latitude.replace(",", ".") + ' long :' + value.Longitude.replace(",", "."));

                            app.markers.push(marker);
                        }
                        else { //actualizo el marker si ya existe

                            var point = new google.maps.LatLng(lat, lon);
                            obj.setPosition(point);
                            //obj.setShadow(shadow);
                        }
                    }
                });

                zoomToBouns();
            }
        }
    } catch (exc) {
        _Show("maps_callback -->"+exc.message);
    }
}

function zoomToBouns() {
    try {
        var bounds = new google.maps.LatLngBounds();
        //   app.console("markers length --> " + uc.markers.length);
        if (app.markers.length > 0 && app.Flag_Refresh) {
            $.each(app.markers, function (index, value) {
                bounds.extend(value.position);
            });
            app.map.fitBounds(bounds);
            var curZoom = app.map.zoom;
            console.log(curZoom);
            var newZoom = (curZoom - 1);
            console.log('zoom ' + newZoom);
        }
    } catch (exc) {
        _Show("map zoomToBouns -->" + exc.message);
    }
}

function markerClick(marker) {
    try {
       // _Show("map marker -->" + JSON.stringify(marker));
        var template = _.template($('#infowindows-template').html(), { marker: marker.customInfo });

        if (app.infowindow != null) {
            app.infowindow.close();
            app.infowindow = null;
        }

        app.infowindow = new google.maps.InfoWindow({
            content: template
        });
        
        
        app.infowindow.open(app.map, marker);

    } catch (exc) {
        _Show("map markerClick -->" + exc.message);
    }
}

function MarkerZoomTo(jobID) {
    try {

        var marker = _.where(app.markers, { jobId: jobID })[0];

        if (marker != null && typeof (marker) != 'undefined') {

            //   app.console("marker custominfo" + marker.customInfo);

            var pt = marker.getPosition();
            var newpt = new google.maps.LatLng(pt.lat() + .02, pt.lng());
            app.map.panTo(newpt);
            markerClick(marker);


        } else {
            if (app.infowindow != null) {
                app.infowindow.close();
                app.infowindow = null;
            }
        }

    } catch (exc) {
        _Show("EXC MarkerZoomTo " + exc.message);
    }
}

function closeInfoWindows() {

    if (app.infowindow != null) {
        app.infowindow.close();
        app.infowindow = null;
    }
}

function setAllMap(map) {
    try {
        for (var i = 0; i < app.markers.length; i++) {

            app.markers[i].setMap(map);
        }
    }
    catch (exc) {
        _Show("EXC setAllMap " + exc.message);
    }
}

// Removes the markers from the map, but keeps them in the array.
function clearMarkers() {
    try {

        setAllMap(null);
    }
    catch (exc) {
        _Show("EXC clearMarkers " + exc.message);
    }
}

function deleteMarkers() {
    try {
        closeInfoWindows();
        clearMarkers();
        app.markers = [];
    }
    catch (exc) {
       _Show("EXC deleteMarkers "+exc.message);
    }
}


function loadScript() {
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = 'https://maps.googleapis.com/maps/api/js?v=3.exp&' +'callback=loadmaps';
    document.body.appendChild(script);
}



function getGeoCoder(address) {
    try {
        _Show("address init  -->" + address);
      
        var geocoderRequest;
        if (app.geocoder == null)
            app.geocoder = new google.maps.Geocoder();

        geocoderRequest = {
            address: address
        };

        app.geocoder.geocode(geocoderRequest,geocoder_callback);

    } catch (exc) {
        _Show("getGeoCoder"+ exc.message);
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

        var fullAddress = ($.trim(street) + " " + city + ", " + state + " " + postalcode + " " + country);
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
       _Show("GetLocation "+exc.message);

    }
    return null;
}

function geocoder_callback(arrGeocoderResult, status) {
    try {
        app.location = null;
        var result = null;
        _Show(" status --->" + status);
        if (status != google.maps.GeocoderStatus.OK) {
            alert("Address Not Found");
        }
        else {

            if (arrGeocoderResult != null && arrGeocoderResult.length > 0) {
                _Show(" # --->" + arrGeocoderResult.length);
                var limit = (app.from == 'address') ? 100 : 1;  //Para busquedas por Lat,Lng solo muestra el primer registro. Los otros son la ciudad, el estado y el Pais

                ///si encuentro lo debo de subir inmediatamente
                if (arrGeocoderResult.length > 0) {
                    app.location = GetLocation(arrGeocoderResult[0]);
                }               
            }
        }

            switch (app.currentModule) {

                case 0:

                    break;
                case 1:  //modulo creacion de jobs
                    _Show(" location --->" + JSON.stringify(app.location));
                    JobForm.SaveJob();
                    break;

            }
    }
    catch (exc) {
       _Show("geocoder_callback "+exc.message);
    }
}



window.onload = loadScript;