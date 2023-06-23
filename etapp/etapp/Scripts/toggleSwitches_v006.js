//7/14/2014:
//IMPORTANT:
//If this JS is used in a module other than Tracking, consider that UserPreferences are being set to Module 'TRACKING'.  Implement a change where the module is passed as a parameter.

var latLngGridLayer = false;
var trafficLayer = false;
var geoPolyArr = []
var geoPolyLabelsArr = []
var geoCirclesArr = []
var geoMarkersArr = []
var geoInfoWinArr = []

function forceAutoZoomFeature() {
    try {
        
        if (bAutoZoomFreezed == true) {
            $('#freezeAutoZoom').removeAttr("checked");
            $('#a_freezeAutoZoom').removeClass("checked");
            toggleFreezeAutoZoom(false);
        }
    }
    catch (err) {
        alert('forceAutoZoomFeature: ' + err.description);
    }
    
}

function toggleFreezeAutoZoom(val,source) {//1 fleetlocation - 2 mapcontrols  - 3 loadpage
    try {
        
        if (val == undefined) {
            val = false;
            onZoom(false)
        }
        if (val == true) {
            onZoom(true)
        } else {
            onZoom(false)
        }
        if (source != 1) {//from fleetlocation
            var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
            if (source == 2) { //from mapcontrols
                if (preferences.autoZoom != val) {
                    
                    bAutoZoomFreezed = val;
                    var val2 = map.getZoom();
                    var lat = map.getCenter().lat();
                    var lng = map.getCenter().lng();
                    var jsonUserPref = { moduleName: 'TRACKING', preference: 'AUTOZOOM', val1: val, val2: val2, lat: lat, lng: lng };
                    updateUserPref(jsonUserPref);//alex
                    jsonAppFeatures = false;//alex
                    getAppFeatures(1);//alex
                }
            }
            
        }

        //var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
        //if (preferences.autoZoom != val) {
            
        //    bAutoZoomFreezed = val;
        //    var val2 = map.getZoom();
        //    var jsonUserPref = { moduleName: 'TRACKING', preference: 'AUTOZOOM', val1: val, val2: val2 };
        //    updateUserPref(jsonUserPref);//alex
        //    jsonAppFeatures = false;//alex
        //    getAppFeatures(1);//alex
        //}
    }
    catch (err) {
        alert('toggleFreezeAutoZoom: ' + err.description);
    }
}
function toggleGoogleTraffic(val,source) {
    try {
        
        if (trafficLayer == false) {
            trafficLayer = new google.maps.TrafficLayer();
        }

        if (val == true) {
            trafficLayer.setMap(map);
        }
        else {
            trafficLayer.setMap(null);
        }
        if (source == 2) {
            var jsonUserPref = { moduleName: 'TRACKING', preference: 'SHOWTRAFFIC', val1: val, val2: val };//alex
            updateUserPref(jsonUserPref);//alex
            getAppFeatures(1);
        }      
    }
    catch (err) {
        alert('toggleGoogleTraffic: ' + err.description);
    }
}

async function toggleGeofencesLayer(val, source) {
    
    try {
         $.each(geoMarkersArr, function () {
            this.setMap(null);
        });
         $.each(geoPolyArr, function () {
            this.setMap(null);
            this.polyCenter.setMap(null);
        });
         $.each(geoPolyLabelsArr, function () {
            this.setMap(null);
        });

        geoCirclesArr = [];
        geoPolyArr = [];
        geoMarkersArr = [];
        if (val == true) {
            if (geoCirclesArr.length == 0 && geoPolyArr.length == 0 && geoMarkersArr.length == 0) {
                if (jsonGeofences == undefined) {
                    var data =  't=' + getTokenCookie('ETTK') + '&extended=true';
                    jsonGeofences = dbReadWrite('getGeofences_All', data, true, false);
                }

                var lat = 0;
                var lng = 0;
                var radius = 0;
                var polyTXT = '';
                var labelContent = '';
                if (jsonGeofences) {
                    /*for (var ind = 0; ind < jsonGeofences.geofences.length; ind++) {*/
                    for (var ind = 0; ind < jsonGeofences.length; ind++) {
                        jsonGeo =  jsonGeofences[ind];  //eval('(' + jsonGeofences.geofences[ind] + ')');
                        if (jsonGeo.shapeID == 1) {
                            var loc =  new google.maps.LatLng(jsonGeo.latitude, jsonGeo.longitude);
                            //labelContent = jsonGeo.Name;
                            //var marker = new google.maps.Marker({ map: map, position: loc, icon: jsonGeo.iconUrl });
                            var txtColor = '#000000';
                            var bgndColor = '#ffffff';
                            var marker = false;
                            marker =  new MarkerWithLabel({
                                position: loc,
                                draggable: false,
                                map: map,
                                title: jsonGeo.name,
                                icon: jsonGeo.IconURL//,
                                //labelContent: labelContent,
                                //labelAnchor: new google.maps.Point(22, 0),
                                //labelClass: "geoLabel", // the CSS class for the label
                                //labelStyle: { opacity: 1.00, 'color': txtColor, 'background-color': bgndColor }
                            });

                            //var marker = new google.maps.Marker({ map: map, position: loc });

                            //var content =jsonGeo.geofenceInfoTable;
                            //(function (marker, content) {
                            //    google.maps.event.addListener(marker, 'click', function () {
                            //        if (!infowindow) {
                            //            infowindow = new google.maps.InfoWindow();
                            //        }
                            //        var data = 't=' + getTokenCookie('ETTK') + '&id=' + id;
                            //        var infoWin = dbReadWrite('getGeofences_InfoWindow', data, true, false);
                            //        infowindow.setContent(content);
                            //        infowindow.setZIndex(999);
                            //        infowindow.open(map, marker);
                            //    });
                            //})(marker, content);

                            var geoId =  jsonGeo.id;
                             (function (marker, geoId) {
                                    google.maps.event.addListener(marker, 'click', function () {
                                    if (!infowindow) {
                                        infowindow = new google.maps.InfoWindow();
                                    }
                                    var data = 't=' + getTokenCookie('ETTK') + '&id=' + geoId;
                                    var infoWin = dbReadWrite('getGeofences_InfoWindow', data, true, false);
                                    jsonInfoWin = eval('(' + infoWin.geofence[0] + ')');
                                    if (_.isObject(jsonInfoWin)) {
                                         infowindow.setContent(jsonInfoWin.geofenceInfoTable);
                                         infowindow.setZIndex(999);
                                         infowindow.open(map, marker);
                                    }
                                });
                            })(marker, geoId);
                             geoMarkersArr.push(marker);

                            //                        var circle = drawCircle(loc, jsonGeo.radius);
                            //                        geoCirclesArr.push(circle);
                        }
                        else {
                            //Define the polygone and add to array
                            var jsonVertices =  eval('(' + jsonGeo.jsonPolyVerticesTXT + ')');
                            var poly =  drawPoly(jsonVertices, jsonGeo.iconUrl);
                            var geoId =  jsonGeo.id;
                            (function (poly, geoId) {
                                google.maps.event.addListener(poly, 'click', function () {
                                    if (!infowindow) {
                                        infowindow =  new google.maps.InfoWindow();
                                    }
                                    var data =  't=' + getTokenCookie('ETTK') + '&id=' + geoId;
                                    var infoWin =  dbReadWrite('getGeofences_InfoWindow', data, true, false);
                                    jsonInfoWin =  eval('(' + infoWin.geofence[0] + ')');
                                    if (_.isObject(jsonInfoWin)) {
                                         infowindow.setContent(jsonInfoWin.geofenceInfoTable);
                                         infowindow.setZIndex(999);
                                         infowindow.open(map, poly.polyCenter);
                                    }
                                });
                            })(poly, geoId);
                             geoPolyArr.push(poly);

                            //Define the polygon's label and add to array of labels
                            var polyBounds =  new google.maps.LatLngBounds();
                             poly.getPath().forEach(function (element, index) {
                                polyBounds.extend(element)
                            });

                            var boxText =  document.createElement("div");
                            boxText.style.cssText = "border: 1px solid black; margin-top: 1px; background: yellow; padding: 1px;";
                            boxText.innerHTML = jsonGeo.name;

                            var polyLabelOptions =  {
                                content: boxText,
                                boxStyle: {
                                    textAlign: "center",
                                    fontSize: "8pt",
                                    opacity: '0.75'
                                },
                                disableAutoPan: true,
                                pixelOffset: new google.maps.Size(-25, 0),
                                position: polyBounds.getCenter(),
                                closeBoxURL: "",
                                isHidden: false,
                                pane: "mapPane",
                                enableEventPropagation: true
                            };
                            var polygonLabel =  new InfoBox(polyLabelOptions);
                             geoPolyLabelsArr.push(polygonLabel);

                        }
                    }
                }
            }


             $.each(geoMarkersArr, function () {
                this.setMap(map);
            });
              $.each(geoPolyArr, function () {
                this.setMap(map);
                this.polyCenter.setMap(map);
            });
              $.each(geoPolyLabelsArr, function () {
                this.setMap(map);
            });

        }   
        if (source == 2) {
            var jsonUserPref = { moduleName: 'TRACKING', preference: 'SHOWGEOFENCES', val1: val, val2: val };
            updateUserPref(jsonUserPref);
            getAppFeatures(1);
        }
        
        

    }
    catch (err) {
        alert('toggleGeofencesLayer: ' + err.description);
    }
}
function toggleGeofencesLayerName(val, source) {
    
    try {
        $.each(geoMarkersArr, function () {
            this.setMap(null);
        });
        $.each(geoPolyArr, function () {
            this.setMap(null);
            this.polyCenter.setMap(null);
        });
        $.each(geoPolyLabelsArr, function () {
            this.setMap(null);
        });

        geoCirclesArr = [];
        geoPolyArr = [];
        geoMarkersArr = [];

        if (val == true) {

            if (geoCirclesArr.length == 0 && geoPolyArr.length == 0 && geoMarkersArr.length == 0) {
                if (jsonGeofences == undefined) {
                    var data = 't=' + getTokenCookie('ETTK') + '&extended=true';
                    jsonGeofences = dbReadWrite('getGeofences_All', data, true, false);
                }

                var lat = 0;
                var lng = 0;
                var radius = 0;
                var polyTXT = '';
                var labelContent = '';
                if (jsonGeofences) {
                    /*for (var ind = 0; ind < jsonGeofences.geofences.length; ind++) {*/
                    for (var ind = 0; ind < jsonGeofences.length; ind++) {
                        jsonGeo = jsonGeofences[ind];  //eval('(' + jsonGeofences.geofences[ind] + ')');
                        if (jsonGeo.shapeID == 1) {
                            var loc = new google.maps.LatLng(jsonGeo.latitude, jsonGeo.longitude);
                            labelContent = jsonGeo.name;
                            //var marker = new google.maps.Marker({ map: map, position: loc, icon: jsonGeo.iconUrl });
                            var txtColor = '#000000';
                            var bgndColor = '#ffffff';
                            var marker = false;
                            marker = new MarkerWithLabel({
                                position: loc,
                                draggable: false,
                                map: map,
                                title: jsonGeo.name,
                                icon: jsonGeo.IconURL,
                                labelContent: labelContent,
                                labelAnchor: new google.maps.Point(22, 0),
                                labelClass: "geoLabel", // the CSS class for the label
                                labelStyle: { opacity: 1.00, 'color': txtColor, 'background-color': bgndColor }
                            });

                            //var marker = new google.maps.Marker({ map: map, position: loc });

                            //var content =jsonGeo.geofenceInfoTable;
                            //(function (marker, content) {
                            //    google.maps.event.addListener(marker, 'click', function () {
                            //        if (!infowindow) {
                            //            infowindow = new google.maps.InfoWindow();
                            //        }
                            //        var data = 't=' + getTokenCookie('ETTK') + '&id=' + id;
                            //        var infoWin = dbReadWrite('getGeofences_InfoWindow', data, true, false);
                            //        infowindow.setContent(content);
                            //        infowindow.setZIndex(999);
                            //        infowindow.open(map, marker);
                            //    });
                            //})(marker, content);

                            var geoId = jsonGeo.id;
                            (function (marker, geoId) {
                                google.maps.event.addListener(marker, 'click', function () {
                                    if (!infowindow) {
                                        infowindow = new google.maps.InfoWindow();
                                    }
                                    var data = 't=' + getTokenCookie('ETTK') + '&id=' + geoId;
                                    var infoWin = dbReadWrite('getGeofences_InfoWindow', data, true, false);
                                    jsonInfoWin = eval('(' + infoWin.geofence[0] + ')');
                                    if (_.isObject(jsonInfoWin)) {
                                        infowindow.setContent(jsonInfoWin.geofenceInfoTable);
                                        infowindow.setZIndex(999);
                                        infowindow.open(map, marker);
                                    }
                                });
                            })(marker, geoId);
                            geoMarkersArr.push(marker);

                            //                        var circle = drawCircle(loc, jsonGeo.radius);
                            //                        geoCirclesArr.push(circle);
                        }
                        else {
                            //Define the polygone and add to array
                            var jsonVertices = eval('(' + jsonGeo.jsonPolyVerticesTXT + ')');
                            var poly = drawPoly(jsonVertices, jsonGeo.iconUrl);
                            var geoId = jsonGeo.id;
                            (function (poly, geoId) {
                                google.maps.event.addListener(poly, 'click', function () {
                                    if (!infowindow) {
                                        infowindow = new google.maps.InfoWindow();
                                    }
                                    var data = 't=' + getTokenCookie('ETTK') + '&id=' + geoId;
                                    var infoWin = dbReadWrite('getGeofences_InfoWindow', data, true, false);
                                    jsonInfoWin = eval('(' + infoWin.geofence[0] + ')');
                                    if (_.isObject(jsonInfoWin)) {
                                        infowindow.setContent(jsonInfoWin.geofenceInfoTable);
                                        infowindow.setZIndex(999);
                                        infowindow.open(map, poly.polyCenter);
                                    }
                                });
                            })(poly, geoId);
                            geoPolyArr.push(poly);

                            //Define the polygon's label and add to array of labels
                            var polyBounds = new google.maps.LatLngBounds();
                            poly.getPath().forEach(function (element, index) {
                                polyBounds.extend(element)
                            });

                            var boxText = document.createElement("div");
                            boxText.style.cssText = "border: 1px solid black; margin-top: 1px; background: yellow; padding: 1px;";
                            boxText.innerHTML = jsonGeo.name;

                            var polyLabelOptions = {
                                content: boxText,
                                boxStyle: {
                                    textAlign: "center",
                                    fontSize: "8pt",
                                    opacity: '0.75'
                                },
                                disableAutoPan: true,
                                pixelOffset: new google.maps.Size(-25, 0),
                                position: polyBounds.getCenter(),
                                closeBoxURL: "",
                                isHidden: false,
                                pane: "mapPane",
                                enableEventPropagation: true
                            };
                            var polygonLabel = new InfoBox(polyLabelOptions);
                            geoPolyLabelsArr.push(polygonLabel);

                        }
                    }
                }
            }

            $.each(geoMarkersArr, function () {
                this.setMap(map);
            });
            $.each(geoPolyArr, function () {
                this.setMap(map);
                this.polyCenter.setMap(map);
            });
            $.each(geoPolyLabelsArr, function () {
                this.setMap(map);
            });
        }
        if (source == 2) {
            var jsonUserPref = { moduleName: 'TRACKING', preference: 'SHOWGEOFENCESNAME', val1: val, val2: val };
            updateUserPref(jsonUserPref);
            getAppFeatures(1);
        }
       

    }
    catch (err) {
        alert('toggleGeofencesLayer: ' + err.description);
    }
}
function drawCircle(loc, radius) {
    try {
        var geoCircle = new google.maps.Circle({
            map: map,
            center: loc,
            radius: (parseFloat(radius) * 0.3048),    // in metres
            fillColor: '#AA0000'
        });

        return geoCircle;
    }
    catch (err) {
        alert('drawCircle: ' + err.description);
    }
}

function drawPoly(jsonVertices, iconURL) {
    try {
        var poly = new google.maps.Polygon({ map: map, strokeColor: "#FF0000", strokeOpacity: 0.8, strokeWeight: 2, fillColor: "#FF0000", fillOpacity: 0.35 });
        var bounds = new google.maps.LatLngBounds();

        for (var i = 0; i < jsonVertices.length; i++) {
            var jsonItem = jsonVertices[i];
            var loc = new google.maps.LatLng(jsonItem.lat, jsonItem.lng);
            poly.getPath().push(loc);
            bounds.extend(loc);
        }

        if (iconURL == undefined || iconURL == '') {
            iconURL = 'https://easitrack.net/icons/bullet-red16x16.png';
        }
        var ctr = new google.maps.Marker({ map: map, position: bounds.getCenter(), icon: iconURL });
        poly.polyCenter = ctr;

        return poly;
    }
    catch (err) {
        alert('drawPoly: ' + err.description);
    }
}

function toggleLatLngGridLayer(val, source) {
    //7/17/2015
    //This routine is based on the script latLngGridLayer
    
    try {
        if (val == undefined) {
            val = false;
        }

        if (val == true) {
            latLngGridLayer = new Graticule(map, true);
        }
        else {
            latLngGridLayer.setMap(null);
        }
        if (source == 2) {
            var jsonUserPref = { moduleName: 'TRACKING', preference: 'SHOWLATLNGGRID', val1: val, val2: val };
            updateUserPref(jsonUserPref);
            getAppFeatures(1);
        }
        
    }
    catch (err) {
        alert('toggleLatLngGridLayer: ' + err.description);
    }
}

function toggleSwitches() {
    try {
        
        $('body').attr('class', 'js');
        $('.checkbox').after(function () {
            
            if ($(this).is(":checked")) {
                return "<a href='#' class='toggle checked' ref='" + $(this).attr("id") + "' id='a_" + $(this).attr("id") + "'></a>";
            } else {
                return "<a href='#' class='toggle' ref='" + $(this).attr("id") + "' id='a_" + $(this).attr("id") + "'></a>";
            }
        });
        $('.toggle').click(function (e) {
            
            toggleThisSwitch(this, 2);

            //var checkboxID = $(this).attr("ref");
            //var checkbox = $('#' + checkboxID);

            //var funcName = $(checkbox).attr('data-function');

            //if (checkbox.is(":checked")) {
            //    checkbox.removeAttr("checked");
            //    window[funcName](false);

            //} else {
            //    checkbox.attr("checked", "true");
            //    window[funcName](true);
            //}
            //$(this).toggleClass("checked");

            e.preventDefault();

        });

        //applyToggleSwitchesPreferences();

    }
    catch (err) {
        alert('toggleSwitches: ' + err.description);
    }
}

function toggleThisSwitch(obj,source) {//1 fleetlocation - 2 mapcontrols
    try {
        
        if (source != 1) {
            var checkboxID = $(obj).attr("ref");
            var checkbox = $('#' + checkboxID);

            var funcName = $(checkbox).attr('data-function');

            if (checkbox.is(":checked")) {
                checkbox.removeAttr("checked");

                window[funcName](false, source);

            } else {
                checkbox.attr("checked", "true");

                window[funcName](true, source);
            }
            $(obj).toggleClass("checked");
        }        
    }
    catch (err) {
        alert('toggleThisSwitch: ' + err.description);
    }
}
function applyToggleSwitchesPreferences(source) {
    
    try {        
        if (_.isObject(jsonAppFeatures)) {            
            var preferences = JSON.parse(jsonAppFeatures.userPreferences[0]);
            
            var checkbox = $('#freezeAutoZoom');
            //if (preferences.autoZoom != checkbox.is(":checked")) {
            if (preferences.autoZoom) {
                toggleThisSwitch($('#a_freezeAutoZoom'), source);
            }
            
            if (preferences.autoZoom == true) {
                if (_.isNumber(preferences.autoZoomLevel)) {
                    
                    map.setZoom(preferences.autoZoomLevel);
                    map.setCenter({ lat: preferences.ZoomLat, lng: preferences.ZoomLng });
                    map.initialZoom = false;                    
                    getCurrentZoomLevel();
                    bAutoZoomFreezed = true;
                }
            }
            
            var checkbox = $('#mapTraffic');
            //if (preferences.showTraffic != checkbox.is(":checked")) {
            if (preferences.showTraffic) {
                toggleThisSwitch($('#a_mapTraffic'), source);
            }
            
            var checkbox = $('#geoLayers');
            //if (preferences.showGeofences != checkbox.is(":checked")) {
            if (preferences.showGeofences) {
                toggleThisSwitch($('#a_geoLayers'), source);
            }
            
            var checkbox = $('#geoLayersNames');
            //if (preferences.showGeofences != checkbox.is(":checked")) {
            if (preferences.ShowGeofencesName) {
                toggleThisSwitch($('#a_geoLayersNames'), source);
            }
            
            var checkbox = $('#latLngGridLayer');
            //if (preferences.showLatLngGrid != checkbox.is(":checked")) {
            if (preferences.showLatLngGrid) {
                toggleThisSwitch($('#a_latLngGridLayer'), source);
            }            
        }
    }
    catch (err) {
        alert('applyToggleSwitchesPreferences: ' + err.description);
    }
}

