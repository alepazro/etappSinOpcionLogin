var ucfindaddress = usercontrol_findaddress();
ucfindaddress.initialize();


function usercontrol_findaddress() {
    var uc = {
        key: "findaddress",
        res: 'generics/controls/res/findaddress',
        id: -1,
        map: null,
        geocoder: null,         //Geocoder
        markers: [],            //Markers
        bounds: null,           //Envelope
        results: null,
        from: 'address',
        circle: null,
        collectionCountry: [],
        currentMarker: null,
        currentPonit:null
      
    };
    try {
  
        uc.initialize = function () {
            try {                //3) Inicializa los controles de la forma
                uc.loadControls();

                //4) Inicializa el Mapa de la forma
                uc.loadMap();
            }
            catch (exc) {
                app.excManager(uc.key, "initialize", exc);
            }
        };

        /*-----------------------------------------------
       - 3) Resize de la Forma
       ------------------------------------------------*/
        uc.resize = function () { };

        /*-----------------------------------------------
        - Load Controls
        ------------------------------------------------*/
        uc.loadControls = function (id) {
            try {
                uc.id = id || -1;
                $('#address_lbxresults').click(function () {
                    try {
                        var selectedValue = $(this).val();
                        if (typeof (selectedValue) != 'undefined') {
                            var marker = uc.markers[selectedValue];
                            var position = marker.getPosition();
                            uc.map.panTo(position);
                            $("#address_btnselect").removeAttr("disabled");
                        }
                    }
                    catch (exc) {
                        app.excManager(uc.key, "lbxresults.click", exc);
                    }
                });

                $("#address_lbxresults").dblclick(function () {
                    try {
                        var selectedValue = $(this).val();
                        if (typeof (selectedValue) != 'undefined') {
                            var marker = uc.markers[selectedValue];
                            uc.map.panTo(marker.getPosition());
                            uc.map.setZoom(18);
                            $("#address_btnselect").removeAttr("disabled");
                        }
                    }
                    catch (exc) {
                        app.excManager(uc.key, "lbxresults.dblclick", exc);
                    }
                });

                $('#btnCancel_findAddress').val("Close");
                $('#btnAddressSearch').val("Search");
                $("#address_search").attr("placeholder", "Address required");
                $('#address_btnselect').val("Select Address");
                $('#divFindAddress_wnd_title').text("Find Address");


         
            }
            catch (exc) {
                app.excManager(uc.key, "loadControls", exc);
            }
        };

        /*-----------------------------------------------
        - Find the location entered
        ------------------------------------------------*/
        uc.btnfind_click = function (from) {
            //from: 1: Address, 2: Lat/Lng
            try {
                uc.from = from;

                var geocoderRequest;
                if (uc.geocoder == null)
                    uc.geocoder = new google.maps.Geocoder();

                var fullAddress = $('#address_search').val();
                if (fullAddress == '') {
                    app.toast({ type: 'warning', msg:"Address is required" });
                    return;
                }

                
                geocoderRequest = {
                    address: fullAddress
                };

                uc.clearOverlays();
                uc.geocoder.geocode(geocoderRequest, ucfindaddress.geocoder_callback);
            }
            catch (exc) {
                app.excManager(uc.key, "btnFind_Click", exc);
            }
        }

        /*-----------------------------------------------
        - Process results from google
        ------------------------------------------------*/
        uc.geocoder_callback = function (arrGeocoderResult, status) {
            try {
                var lbxAddress = $("#address_lbxresults");
                lbxAddress.empty();
                if (status != google.maps.GeocoderStatus.OK) {
                    app.toast({ type: 'warning', msg: "Address not found" });
                    $("#address_btnselect").attr("disabled", true);
                }
                else {
                    if (arrGeocoderResult.length > 0) {
                        var markers = [];
                        var lenCollecntionMarkers = 0;
                        var limit = (uc.from == 'address') ? 1000 : 1;  //Para busquedas por Lat,Lng solo muestra el primer registro. Los otros son la ciudad, el estado y el Pais

                        if (uc.results != null) {
                           
                            var lat = uc.results.lat.toString().replace(',','.');
                            var lng = uc.results.lng.toString().replace(',', '.');
                            var state = uc.results.state;
                            var country = uc.results.country;
                            var street = uc.results.address1;
                            var city = uc.results.city;
                            var postalcode = uc.results.postalcode;
                            //_Show('lat' + lat + ' lng:' + lng);
                            if ((lat.trim()!='' && lng.trim()!='') && (parseFloat(lat) != 0 && parseFloat(lng) != 0) ) {

                                var fullAddress = ($.trim(street) + " " + city + ", <br>" + state + " " + postalcode + " " + country);
                                var location = {
                                    address1: $.trim(street),
                                    address2: '',
                                    city: $.trim(city),
                                    state: $.trim(state),
                                    postalcode: $.trim(postalcode),
                                    country: $.trim(country),
                                    lat: lat,
                                    lng: lng,
                                    radius: 0, //Por definir en la seleccion
                                    arrGeocoderResult: arrGeocoderResult
                                }
                          
                                
                                var optionText = fullAddress;
                                $('<option value="' + 0+ '">' + '<span>' + (1).toString() + " </span>" + " - " + optionText + "</option>").appendTo("#address_lbxresults");
   
                                var point = new google.maps.LatLng(lat, lng);
                                var googleMarker = new google.maps.Marker({
                                    position: point,
                                    map: uc.map,
                                    clickable: false,
                                    title: ("Current").toString(),
                                    draggable: false,
                                    animation: google.maps.Animation.DROP,
                                    shadow: new google.maps.MarkerImage("https://chart.googleapis.com/chart?chst=d_text_outline&chld=000|12|h|fff|b|" + ("Current").toString(), null, null, new google.maps.Point(-4, -1)),
                                    tooltip: '<B>' + fullAddress + '</B>',
                                    //Custom
                                    location: location
                                });
    
                                uc.markers.push(googleMarker);
                                uc.bounds.extend(point);
                                lenCollecntionMarkers++;
                               
                            }
                        }


                        for (var n = 0; n < arrGeocoderResult.length && n < limit; n++) {


                            geocoderResult = arrGeocoderResult[n];

                       //     _Show('geocoderResult : ' + JSON.stringify(geocoderResult));

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
                                radius: 0, //Por definir en la seleccion
                                arrGeocoderResult: arrGeocoderResult
                            }

                            /*
                            var template = _.template($('#location-item').html(), { fullAddress: fullAddress, index: i });
                            $("#address_lbxresults").append(template);
                            */
                           
                            var optionText = fullAddress;
                            $('<option value="' + (n + lenCollecntionMarkers).toString() + '">' + '<span>' + (n + 1 + lenCollecntionMarkers).toString() + " </span>" + " - " + optionText + "</option>").appendTo("#address_lbxresults");
                            
                            var point = new google.maps.LatLng(lat, lng);
                            var googleMarker = new google.maps.Marker({
                                position: point,
                                map: uc.map,
                                clickable: true,
                                title: fullAddress,
                                draggable: false,
                                animation: google.maps.Animation.DROP,
                                shadow: new google.maps.MarkerImage("https://chart.googleapis.com/chart?chst=d_text_outline&chld=000|12|h|fff|b|" + (n + 1).toString(), null, null, new google.maps.Point(-4, -1)),
                                tooltip: '<B>' + fullAddress + '</B>',
                                //Custom
                                location: location
                            });

                            uc.markers.push(googleMarker);
                            uc.bounds.extend(point);
                        }

                        uc.map.fitBounds(uc.bounds);
                        uc.map.panToBounds(uc.bounds);
                        uc.map.panTo(uc.bounds.getCenter());

                        if (uc.markers.length == 1)
                            uc.map.setZoom(16);

                        lbxAddress.selectedIndex = 0;
                        lbxAddress.focus();
                        $("#address_btnselect").removeAttr("disabled");
                    }
                }
            }
            catch (exc) {
                app.excManager(uc.key, "geocoder_callback", exc);
            }
        }





        /*-----------------------------------------------
        - seleccionar una ubicacion correcta
        ------------------------------------------------*/
        uc.SelectLocations = function (arrGeocoderResult) {
            try {
                    if (arrGeocoderResult.length > 0) {
                        var markers = [];
                        var lenCollecntionMarkers = 0;
                        var limit = (uc.from == 'address') ? 1000 : 1;  //Para busquedas por Lat,Lng solo muestra el primer registro. Los otros son la ciudad, el estado y el Pais

                        if (uc.results != null) {

                            var lat = uc.results.lat.toString().replace(',', '.');
                            var lng = uc.results.lng.toString().replace(',', '.');
                            var state = uc.results.state;
                            var country = uc.results.country;
                            var street = uc.results.address1;
                            var city = uc.results.city;
                            var postalcode = uc.results.postalcode;
                            //_Show('lat' + lat + ' lng:' + lng);
                            if ((lat.trim() != '' && lng.trim() != '') && (parseFloat(lat) != 0 && parseFloat(lng) != 0)) {

                                var fullAddress = ($.trim(street) + " " + city + ", <br>" + state + " " + postalcode + " " + country);
                                var location = {
                                    address1: $.trim(street),
                                    address2: '',
                                    city: $.trim(city),
                                    state: $.trim(state),
                                    postalcode: $.trim(postalcode),
                                    country: $.trim(country),
                                    lat: lat,
                                    lng: lng,
                                    radius: 0, //Por definir en la seleccion
                                    arrGeocoderResult: arrGeocoderResult
                                }


                                var optionText = fullAddress;
                                $('<option value="' + 0 + '">' + '<span>' + (1).toString() + " </span>" + " - " + optionText + "</option>").appendTo("#address_lbxresults");

                                var point = new google.maps.LatLng(lat, lng);
                                var googleMarker = new google.maps.Marker({
                                    position: point,
                                    map: uc.map,
                                    clickable: false,
                                    title: ("Current").toString(),
                                    draggable: false,
                                    animation: google.maps.Animation.DROP,
                                    shadow: new google.maps.MarkerImage("https://chart.googleapis.com/chart?chst=d_text_outline&chld=000|12|h|fff|b|" + ("Current").toString(), null, null, new google.maps.Point(-4, -1)),
                                    tooltip: '<B>' + fullAddress + '</B>',
                                    //Custom
                                    location: location
                                });

                                uc.markers.push(googleMarker);
                                uc.bounds.extend(point);
                                lenCollecntionMarkers++;

                            }
                        }


                        for (var n = 0; n < arrGeocoderResult.length && n < limit; n++) {


                            geocoderResult = arrGeocoderResult[n];

                            //     _Show('geocoderResult : ' + JSON.stringify(geocoderResult));

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
                                radius: 0, //Por definir en la seleccion
                                arrGeocoderResult: arrGeocoderResult
                            }

                            /*
                            var template = _.template($('#location-item').html(), { fullAddress: fullAddress, index: i });
                            $("#address_lbxresults").append(template);
                            */

                            var optionText = fullAddress;
                            $('<option value="' + (n + lenCollecntionMarkers).toString() + '">' + '<span>' + (n + 1 + lenCollecntionMarkers).toString() + " </span>" + " - " + optionText + "</option>").appendTo("#address_lbxresults");

                            var point = new google.maps.LatLng(lat, lng);
                            var googleMarker = new google.maps.Marker({
                                position: point,
                                map: uc.map,
                                clickable: true,
                                title: fullAddress,
                                draggable: false,
                                animation: google.maps.Animation.DROP,
                                shadow: new google.maps.MarkerImage("https://chart.googleapis.com/chart?chst=d_text_outline&chld=000|12|h|fff|b|" + (n + 1).toString(), null, null, new google.maps.Point(-4, -1)),
                                tooltip: '<B>' + fullAddress + '</B>',
                                //Custom
                                location: location
                            });

                            uc.markers.push(googleMarker);
                            uc.bounds.extend(point);
                        }

                        uc.map.fitBounds(uc.bounds);
                        uc.map.panToBounds(uc.bounds);
                        uc.map.panTo(uc.bounds.getCenter());

                        if (uc.markers.length == 1)
                            uc.map.setZoom(16);

                        lbxAddress.selectedIndex = 0;
                        lbxAddress.focus();
                        $("#address_btnselect").removeAttr("disabled");
                    }
            }
            catch (exc) {
                app.excManager(uc.key, "geocoder_callback", exc);
            }
        }



        /*-----------------------------------------------
        - Clear all markers on map
        ------------------------------------------------*/
        uc.clearOverlays = function () {
            try {
                $.each(uc.markers, function (index, marker) {
                    if (typeof (marker) != 'undefined') {
                        marker.setMap(null);
                        delete uc.markers[index];
                    }
                });
                uc.bounds = new google.maps.LatLngBounds();
                uc.markers = [];

                if (uc.circle != null) {
                    uc.circle.setMap(null);
                    uc.circle = null;
                }
            }
            catch (exc) {
                app.excManager(uc.key, "ClearOverlays", exc);
            }
        }

        /*-----------------------------------------------
        - Select an address
        ------------------------------------------------*/
        uc.btnselect_click = function () {
            try {
                var index = $('#address_lbxresults').val();
                if (typeof (index) != 'undefined') {
                    uc.results = uc.markers[index].location;
                    uc.clearOverlays();
                    $('#divFindAddress').data('kendoWindow').close();
                }
            }
            catch (exc) {
                app.excManager(uc.key, "btnSelect_Click", exc);
            }
        }

        /*-----------------------------------------------
        - Address formatter
        ------------------------------------------------*/
        uc.getfulladdress = function () {
            var result = '';
            try {
                if (uc.results != null) {
                    result = $.trim(uc.results.address1 + ' ' +
                        uc.results.address2) + ', ' +
                        $.trim(uc.results.city + ' ' +
                        uc.results.state) + ' ' +
                        $.trim(uc.results.postalcode + ' ' +
                        uc.results.country);
                }
            }
            catch (exc) {
                app.excManager(uc.key, "getFullAddress", exc);
            }
            return result;
        }

        uc.getaddress = function () {
            var result = '';
            try {
                if (uc.results != null) {
                    result = $.trim(uc.results.address1 + ' ' +
                        uc.results.address2);
                       
                }
            }
            catch (exc) {
                app.excManager(uc.key, "getFullAddress", exc);
            }
            return result;
        }
        /*-----------------------------------------------
        - Set information, Used to initialize control
        ------------------------------------------------*/
        uc.setInfo = function (params) {
            try {                
                var options = {
                    location: null,
                    bylocation: false,
                    enableradius: false
                };
                $.extend(options, params);

              //  _Show("params -->" + JSON.stringify(options.location));
                
                if (params.location != null) {
                    uc.results = options.location;
                   
                    if(params.hasOwnProperty("isNew"))
                        $('#address_search').val("");
                    else
                        $('#address_search').val(uc.getfulladdress());

                    uc.btnfind_click('address');
                }

               // uc.CountryCodes();
            }
            catch (exc) {
                app.excManager(uc.key, "setInfo", exc);
            }
        }

        /*-----------------------------------------------
        - Load the map modulo
        ------------------------------------------------*/
        uc.loadMap = function () {
            try {
                var myOptions = {
                    zoom: 3,
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    scaleControl: true,
                    scrollWheel: true,
                    mapTypeControlOptions: {
                        mapTypeIds: [google.maps.MapTypeId.HYBRID, google.maps.MapTypeId.SATELLITE, google.maps.MapTypeId.TERRAIN, google.maps.MapTypeId.ROADMAP],
                        style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
                        position: google.maps.ControlPosition.TOP_LEFT
                    },
                    zoomControl: true,
                    zoomControlOptions: {
                        style: google.maps.ZoomControlStyle.SMALL
                    }
                };

                $('#address_map').html('');
                uc.map = new google.maps.Map(document.getElementById("address_map"), myOptions);

                var initialpoint = new google.maps.LatLng(27.698638, -83.804601);
                uc.map.setCenter(initialpoint);

                google.maps.event.addListener(uc.map, 'rightclick', function (event) {
                  //  uc.clearOverlays();
                    var point = new google.maps.LatLng(event.latLng.lat(), event.latLng.lng());

                    if (uc.geocoder == null)
                        uc.geocoder = new google.maps.Geocoder();

                    var geocoderRequest = {
                        latLng: point
                    };

                    uc.currentPonit = point;
                   // _Show('app lat event: '+JSON.stringify(point));

                 //   uc.clearOverlays();
                    uc.geocoder.geocode(geocoderRequest, uc.geocoderLatlng_callback);
              
                })
            }
            catch (exc) {
                app.excManager(uc.key, "loadMap", exc);
            }
        }

        uc.geocoderLatlng_callback = function (arrGeocoderResult, status) {
            try {
              //  _Show('app result point : ' + JSON.stringify(arrGeocoderResult));
                if (arrGeocoderResult.length > 0) {
                  
                      var  geocoderResult = arrGeocoderResult[0];

                      //  _Show('geocoderResult : ' + JSON.stringify(geocoderResult));

                        //var lat = geocoderResult.geometry.location.lat();
                        //var lng = geocoderResult.geometry.location.lng();
                        var lat = uc.currentPonit.lat();
                        var lng = uc.currentPonit.lng();
                        var state = "";
                        var country = "";
                        var street = "";
                        var city = "";
                        var postalcode = "";

                        //Recorre los objetos de direccion resultantes
                        for (var j = 0; j < geocoderResult.address_components.length; j++) {
                            var type = geocoderResult.address_components[j].types[0];
                            //_Show("type Find -->" + JSON.stringify(type)+" element -->"+JSON.stringify(geocoderResult.address_components[j]));
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

                     
                        var point = new google.maps.LatLng(lat, lng);
                     //   _Show('app lat marker: ' + JSON.stringify(point));


                        var location = {
                            address1: app.ltrim(street),
                            address2: '',
                            city: app.ltrim(city),
                            state: app.ltrim(state),
                            postalcode: app.ltrim(postalcode),
                            country: country,
                            lat: lat,
                            lng: lng,
                            radius: 0 //Por definir en la seleccion
                        }

                        var fullAddress = ($.trim(street) + " " + city + ", " + state + " " + postalcode + " " + country);
                        var point = new google.maps.LatLng(lat, lng);
                        // _Show('app lat marker: ' + JSON.stringify(point));
                        var googleMarker = new google.maps.Marker({
                            position: point,
                            map: uc.map,
                            clickable: true,
                            title: fullAddress,
                            draggable: false,
                            animation: google.maps.Animation.DROP,
                            shadow: new google.maps.MarkerImage("https://chart.googleapis.com/chart?chst=d_text_outline&chld=000|12|h|fff|b|" + 'User Click', null, null, new google.maps.Point(-4, -1)),

                            //Custom
                            location: location
                        });

                        google.maps.event.addListener(googleMarker, 'click', function (marker) {

                            var template = _.template($('#infoWindows-location').html(), { location: location }).toString();
                            var newOPtion = '';
                            $.each(uc.collectionCountry, function (i, row) {
                                try {
                                    var option = '';
                                    if (row.ID == location.country.trim())
                                        option = '<option value="' + row.ID + '" selected >' + row.Name + '</option>'
                                    else
                                        option = '<option value="' + row.ID + '">' + row.Name + '</option>'
                                    newOPtion = newOPtion + option;
                                }
                                catch (exc) {
                                    app.excManager(uc.key, "fill countrys", exc);
                                }
                            });

                            googleMarker.infowindow = new google.maps.InfoWindow({
                                content: template
                            });

                            googleMarker.infowindow.open(uc.map, googleMarker);


                            if (uc.currentMarker != null) {
                                uc.currentMarker.infowindow.close();
                            }
                            uc.currentMarker = googleMarker;

                           // _Show("Location Find -->" + JSON.stringify(location));
                       
                            $('#txb_faAddress').val(location.street);
                            $('#txb_faCity').val(location.city);
                            $('#txb_faState').val(location.state);
                            $('#txb_faPC').val(location.postalcode);
                            $('#tbx_faCountry').val(location.country);
                            

                        });

                        google.maps.event.addListener(googleMarker.infowindow, 'closeclick', function () {
                            googleMarker.setMap(null); //removes the marker
                            // then, remove the infowindows name from the array
                        });
                    uc.markers.push(googleMarker);
                    uc.bounds.extend(point);
                    uc.map.fitBounds(uc.bounds);
                    uc.map.panToBounds(uc.bounds);
                    uc.map.panTo(uc.bounds.getCenter());

                }

            } catch (exc) {
                app.excManager(uc.key, "geocoderLatlng_callback", exc);
            }
        }

        uc.closeInfoWindows = function () {
            try {
                uc.currentMarker.infowindow.close();
                $('#modalFindAddress').remove();
             
            } catch (exc) {
                app.excManager(uc.key, "closeInfoWindows", exc);
            }
        }

        uc.AddLocation = function () {
            try {
                var valCountry=$('#tbx_faCountry').val();
                if (valCountry != '-1') {
               
                        var valAddress = $('#txb_faAddress').val();
                        if (valAddress.trim() != '') {
                            var valCity = $('#txb_faCity').val();
                            if (valCity.trim() != '') {
                                var valState = $('#txb_faState').val();
                                if (valState.trim() != '') {
                                    var valPostalCode = $('#txb_faPC').val();
                                    ///aqui tengo yya validado la direccion como el usuario la ingreso
                                    var fullAddress = ($.trim(valAddress) + " " + valCity + ", <br>" + valState + " " + valPostalCode + " " + valCountry);
                                    var location = {
                                        address1: $.trim(valAddress),
                                        address2: '',
                                        city: $.trim(valCity),
                                        state: $.trim(valState),
                                        postalcode: $.trim(valPostalCode),
                                        country: $.trim(valCountry),
                                        lat: uc.currentMarker.location.lat,
                                        lng: uc.currentMarker.location.lng,
                                        radius: 0 //Por definir en la seleccion
                                    }

                                    uc.currentMarker.location = location;
                                    uc.currentMarker.tooltip = '<B>' + fullAddress + '</B>';

                                    var n = uc.markers.length;
                                    var optionText = fullAddress;
                                    $('<option value="' + n + '">' + '<span>' + (n + 1).toString() + " </span>" + " - " + optionText + "</option>").appendTo("#address_lbxresults");

                                    uc.markers.push(uc.currentMarker);
                                    uc.bounds.extend(uc.currentMarker.position);

                                    uc.closeInfoWindows();

                                }
                                else
                                   _Show(": AddLocation 1: " + "Select State" );

                            }
                            else
                                _Show(": AddLocation 2: " + "Select City" );

                        }
                        else
                            _Show(": AddLocation 3: " + "Select Address" );
                }
                else
                    _Show(": AddLocation 4: " + "Select Country" );
            } catch (exc) {
                app.excManager(uc.key, "AddLocation", exc);
            }
        }

        uc.loadPointMap=function(){
        
            try {        
                $.ajax({
                    url: 'https://maps.googleapis.com/maps/api/geocode/json?address=Winnetka&bounds=34.172684,-118.604794|34.236144,-118.500938&sensor=false&key=',
                    type: "POST",
                    contentType: "application/javascript",
                    dataType: "jsonp",
                    data: jDataJson,

                    success: function (data, textStatus) {
                        if (typeof (callback) != 'undefined')
                            callback(data, pt);
                    },

                    error: function (xhr, status, errorThrown) {
                        if (typeof (callback) != 'undefined') {
                            var result = { resultCode: -1, resultMsg: xhr.statusText };
                            callback(result, pt);
                        }
                    }
                });
            }
            catch (err) {
                console.log('loadPointMap: Error loading controller ' + controller + ', fn: ' + fn);
            }
        
        }


        uc.CountryCodes = function () {
            try {
                var data = {
                    tkn: app.tkn,
                    key: 'COUNTRYCODES',
                    id: -1,
                    f1: '',
                    f2: '',
                    f3: '',
                    lan: 'en'
                }
                dbSync('Generics', "GetModuleInfo", data, uc.CountryCode_callback);
            } catch (exc) {
                app.excManager(uc.key, "loadWorkzones :", exc);
                app.loading(false);
            }
        }

        /*-----------------------------------------------
        - Country Code Callback
        ------------------------------------------------*/
        uc.CountryCode_callback = function (rs) {
            try {
                var response = JSON.parse(rs);

                var newItem = { ID: -1, Name: "Select Country" };
                response.unshift(newItem);

                uc.collectionCountry = response;
            } catch (exc) {
                app.excManager(uc.key, "CountryCode_callback :", exc);
                app.loading(false);
            }
        }

    }
    catch (exc) {
        app.excManager("findAddress", "usercontrol_findaddress", exc);
    }
    return uc;
}