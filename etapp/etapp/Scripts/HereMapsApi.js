var map = false;
const platform = new H.service.Platform({ apikey: 'q6t4CQ5PyaeY0VGUYHigRoe0fGfZqyRsRddoaovbOX4' });
var behavior = false;
var bubble;

var position ={ lat: 37.376, lng: -122.034 }; 
var marker = new H.map.Marker(position);
var marker2 = new H.map.Marker(position);
//var plat
//var plng;
var geocoder;
var locationp; 
var i = 0;
var arrayjobstop = [];

function showLocationInMapHere(canvas, position1,pzoom, findLocation, location1) {
        
    var defaultLayers = platform.createDefaultLayers();
    //plat = plat1;
    //plng = plng1
    locationp = location1;
    if (map == false) {
        map = new H.Map(document.getElementById(canvas),
            defaultLayers.vector.normal.map, {
                center: position,
            zoom: pzoom,
            pixelRatio: window.devicePixelRatio || 1
        });
    } else {
        map.removeObjects(map.getObjects());
        map.setCenter(position);
    }    
    window.addEventListener('resize', () => map.getViewPort().resize());    
    if (behavior == false) {
        behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
    }     
}
function Searchgeocode(sourcejob,coordetate) {
    /* let split = location.split(",");*/
    $("#loading").show();
    setTimeout(function () {   
    
    let state = "";
    let jsonObject = $.parseJSON(getlocation(position, coordetate,'-1'))
    let location_p = jsonObject.ListResponse[0];
    if (location_p != undefined) {
            position = { lat: location_p.LatX4, lng: location_p.LngX4 };
        loadFieldsLotations(location_p.FullAddress, location_p.City, location_p.State, location_p.PostalCode, location_p.LatX4, location_p.LngX4, sourcejob);
        addMarker(position);
        $("#loading").hide();        
    }
    else {
        
        //example location: '52.5309,13.3847,150'
        geocoder = platform.getSearchService(),
            geocodingParameters = {
            at: '' + position.lat + ',' + position.lng + ''
                };
        geocoder.reverseGeocode(
                geocodingParameters,
                (result) => {
                    
                    let data = { LatX4: position.lat, LngX4: position.lng,FullAddress: result.items[0].address.label, Street: result.items[0].address.street, City: result.items[0].address.city, State: "", PostalCode: result.items[0].address.postalCode, CountryCode: result.items[0].address.countyCode, Hits: 0, County: result.items[0].address.county }
                    let jsonObject = $.parseJSON(addlocation(data));
                    // Add a marker for each location found            

                    marker = new H.map.Marker(result.items[0].position);
                    marker.draggable = true;
                    map.removeObjects(map.getObjects());
                    map.setCenter({ lat: result.items[0].position.lat, lng: result.items[0].position.lng });
                    map.addObject(marker);
                    if (result.items[0].address.state == undefined) {
                        status = result.items[0].address.county
                    } else {
                        status = result.items[0].address.state
                    }
                    loadFieldsLotations(result.items[0].address.label, result.items[0].address.city, status, result.items[0].address.postalCode, result.items[0].position.lat, result.items[0].position.lng, sourcejob)
                    findClosestDevice(marker);
                    $("#loading").hide();
                    
                    //result.items.forEach((item) => {
                    //});
                }, alert);
    }
    }, 100);
}
function addDraggableMarker() {

    map.addEventListener('dragstart', function (ev) {        
        var target = ev.target,
            pointer = ev.currentPointer;
        if (target instanceof H.map.Marker) {
            var targetPosition = map.geoToScreen(target.getGeometry());
            target['offset'] = new H.math.Point(pointer.viewportX - targetPosition.x, pointer.viewportY - targetPosition.y);
            behavior.disable();
        }
        
    }, false);    
    map.addEventListener('dragend', function (ev) {
               
        var target = ev.target;
        if (target instanceof H.map.Marker) {
            behavior.enable();            
            //plng = ;
            //locationp = plat + ',' + plng + ',150';
            position = { lat: parseFloat(target.a.lat.toString()), lng: parseFloat(target.a.lng.toString()) };
            addMarker(position)
            console.log(position);
                        

        }
    }, false);
    
    map.addEventListener('drag', function (ev) {        
        var target = ev.target,
            pointer = ev.currentPointer;
        if (target instanceof H.map.Marker) {
            target.setGeometry(map.screenToGeo(pointer.viewportX - target['offset'].x, pointer.viewportY - target['offset'].y));
        }
        
    }, false);
}
function loadFieldsLotations(dispatchStreet, dispatchCity, dispatchState, dispatchPostalCode, geof_latitude, geof_longitude, sourcejob) {
    if (sourcejob =='jobstop') {
        let jstop = {};
        $("#jstStreet").val(dispatchStreet);
        $("#jstCity").val(dispatchCity);
        $("#jstState").val(dispatchState);
        $("#jstPostalCode").val(dispatchPostalCode);        
        $('#stop_latitude').val(geof_latitude);
        $('#stop_longitude').val(geof_longitude);        
    } else {
        $("#dispatchStreet").val(dispatchStreet);
        $("#dispatchCity").val(dispatchCity);
        $("#dispatchState").val(dispatchState);
        $("#dispatchPostalCode").val(dispatchPostalCode);
        $('#geof_latitude').val(geof_latitude);
        $('#geof_longitude').val(geof_longitude);
    }
    
}
function addMarker(position1,boolcircle,radius) {
    
    
    marker.setGeometry(position);
    map.removeObjects(map.getObjects());
    map.setCenter(position);
    map.addObject(marker);
    if (boolcircle) {
        addCircleToMap(position, radius);
    }
    

    //marker = new H.map.Marker(position);
    //marker.draggable = true;
    //map.removeObjects(map.getObjects());
    //map.setCenter({ lat: location_p.LatX4, lng: location_p.LngX4 });
    //map.addObject(marker);

    return marker;

}
function addMarker2(position1) {
    
    marker2.setGeometry(position);
    //map.removeObjects(map.getObjects());
    //map.setCenter(position);
    map.addObject(marker2);
    //marker = new H.map.Marker(position);
    //marker.draggable = true;
    //map.removeObjects(map.getObjects());
    //map.setCenter({ lat: location_p.LatX4, lng: location_p.LngX4 });
    //map.addObject(marker);
    return marker2;

}
function getlocation(position1,type,fullAddress) {
    let data = 'lat=' + position.lat + '&lon=' + position.lng + '&type=' + type + '&fullAddress=' + fullAddress;
    let location = getDbJob('getLocation', data);
    return location;
}
function addlocation(data) {
    
    //let data = 'lat=' + lat + '&lon=' + lon;
    let data2 = JSON.stringify(data);
    let location = postDbJob('postLocation', data2);
    return location;
}

function calculateRouteFromAtoB(orglat, orglng, dstlat, dstlng) {
    //map.removeObjects(map.getObjects());
    addMarker(position);
    addMarker2({ lat: parseFloat(orglat), lng: parseFloat(orglng)});
    var router = platform.getRoutingService(null, 8),
        routeRequestParams = {
            routingMode: 'fast',
            transportMode: 'car',
            origin: orglat + ',' + orglng, // Brandenburg Gate
            destination: dstlat + ',' + dstlng, // Friedrichstraße Railway Station
            return: 'polyline,turnByTurnActions,actions,instructions,travelSummary'
        };

    router.calculateRoute(
        routeRequestParams,
        onSuccess,
        onError
    );
}
function onSuccess(result) {
    var route = result.routes[0];
    
    /*
     * The styling of the route response on the map is entirely under the developer's control.
     * A representative styling can be found the full JS + HTML code of this example
     * in the functions below:
     */
    addRouteShapeToMap(route);
    //addManueversToMap(route);
    //addWaypointsToPanel(route);
    //addManueversToPanel(route);
    //addSummaryToPanel(route);
    // ... etc.
}
function onError(error) {
    alert('Can\'t reach the remote server');
}
function addRouteShapeToMap(route) {
    route.sections.forEach((section) => {
        // decode LineString from the flexible polyline
        let linestring = H.geo.LineString.fromFlexiblePolyline(section.polyline);

        // Create a polyline to display the route:
        let polyline = new H.map.Polyline(linestring, {
            style: {
                lineWidth: 4,
                strokeColor: 'rgba(0, 128, 255, 0.7)'
            }
        });

        // Add the polyline to the map
        map.addObject(polyline);
        // And zoom to its bounding rectangle
        map.getViewModel().setLookAtData({
            bounds: polyline.getBoundingBox()
        });
    });
}
function addManueversToMap(route) {
    var svgMarkup = '<svg width="18" height="18" ' +
        'xmlns="http://www.w3.org/2000/svg">' +
        '<circle cx="8" cy="8" r="8" ' +
        'fill="#1b468d" stroke="white" stroke-width="1" />' +
        '</svg>',
        dotIcon = new H.map.Icon(svgMarkup, { anchor: { x: 8, y: 8 } }),
        group = new H.map.Group(),
        i,
        j;

    route.sections.forEach((section) => {
        let poly = H.geo.LineString.fromFlexiblePolyline(section.polyline).getLatLngAltArray();

        let actions = section.actions;
        // Add a marker for each maneuver
        for (i = 0; i < actions.length; i += 1) {
            let action = actions[i];
            var marker = new H.map.Marker({
                lat: poly[action.offset * 3],
                lng: poly[action.offset * 3 + 1]
            },
                { icon: dotIcon });
            marker.instruction = action.instruction;
            group.addObject(marker);
        }

        group.addEventListener('tap', function (evt) {
            map.setCenter(evt.target.getGeometry());
            openBubble(evt.target.getGeometry(), evt.target.instruction);
        }, false);

        // Add the maneuvers group to the map
        map.addObject(group);
    });
}
function addWaypointsToPanel(route) {
    var nodeH3 = document.createElement('h3'),
        labels = [];

    route.sections.forEach((section) => {
        labels.push(
            section.turnByTurnActions[0].nextRoad.name[0].value)
        labels.push(
            section.turnByTurnActions[section.turnByTurnActions.length - 1].currentRoad.name[0].value)
    });

    nodeH3.textContent = labels.join(' - ');
    routeInstructionsContainer.innerHTML = '';
    routeInstructionsContainer.appendChild(nodeH3);
}
function addjobstoparray(p_object) {
    let index = arrayjobstop.length;
    p_object.uniqueKey = "ns" + index;
    arrayjobstop.push(p_object);
    addrowstopjob();    
}
function addCircleToMap(position1, feets) {
    
    let pfeet = parseInt(feets / 3.281);
    map.addObject(new H.map.Circle(
        // The central point of the circle
        position,
        // The radius of the circle in meters
        pfeet,//mts
        {
            style: {
                strokeColor: 'rgba(55, 85, 170, 0.6)', // Color of the perimeter
                lineWidth: 2,
                fillColor: 'rgba(208, 126, 103, 0.7)'  // Color of the circle
            }
        }
    ));
}
function geocode(platform, addr) {   
    let result1 = null;
    var geocoder = platform.getSearchService(),
        geocodingParameters = {
            q: addr
        };

    var process=geocoder.geocode(
        geocodingParameters,
        function (result) {
            
            loadGeocoderResults(result.items);
        },
        onErrorAddress
    );
}
function onErrorAddress(error) {
    
    alert('Can\'t reach the remote server');
}
