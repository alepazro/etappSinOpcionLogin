//SQL: lat lon, lat lon, ...
//google fusion tables: lat,lon lat,lon lat,lon
//google maps poly: json format: {lat,lon} {lat,lon}...

var jsonGeofences = false;

function clearGeofenceList() {
    try {
        $("#geofencesTbl").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearGeofenceList: ' + err.description);
    }
}

function fillGeofenceRecord(tr, item) {
    try {
        var tbl = document.getElementById('geofencesTbl');

        $(tr).attr('id', 'geofenceTR' + item.id);
        $(tr).attr('data-name', item.name);
        $(tr).attr('data-contactName', item.contactName);
        $(tr).attr('data-phone', item.phone);

        //GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
        $(tr).attr('data-contactEmail', item.contactEmail);
        $(tr).attr('data-contactHasSMSAlert', item.contactSMSAlert);
        $(tr).attr('data-contactHasEmailAlert', item.contactEmailAlert);
        $(tr).attr('data-contactAlertTypeId', item.contactAlertTypeId);
        $(tr).attr('data-arrivalMsgId', item.arrivalMsgId);
        $(tr).attr('data-departureMsgId', item.departureMsgId);
        //============================================

        $(tr).attr('data-fullAddress', item.fullAddress);
        $(tr).attr('data-street', item.street);
        $(tr).attr('data-streetNumber', item.streetNumber);
        $(tr).attr('data-route', item.route);
        $(tr).attr('data-suite', item.suite);
        $(tr).attr('data-city', item.city);
        $(tr).attr('data-county', item.county);
        $(tr).attr('data-state', item.state);
        $(tr).attr('data-postalCode', item.postalCode);
        $(tr).attr('data-country', item.country);
        $(tr).attr('data-latitude', item.latitude);
        $(tr).attr('data-longitude', item.longitude);
        $(tr).attr('data-radius', item.radius);
        $(tr).attr('data-geofenceTypeId', item.geofenceTypeId);
        $(tr).attr('data-comments', item.comments);
        $(tr).attr('data-geofenceAlertTypeId', item.geofenceAlertTypeId);
        $(tr).attr('data-shapeId', item.shapeId);
        $(tr).attr('data-isSpeedLimit', item.isSpeedLimit);
        $(tr).attr('data-speedLimit', item.speedLimit);
        $(tr).attr('data-jsonPolyVerticesTXT', item.jsonPolyVerticesTXT);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('geofencesListOddTR');
        }

        //Type
        var geoTypeTd = document.createElement('td');
        $(geoTypeTd).html(item.geofenceTypeName);
        $(geoTypeTd).addClass('geofencesListTD');
        tr.appendChild(geoTypeTd);

        //Name
        var nameTd = document.createElement('td');
        $(nameTd).html(item.name);
        $(nameTd).addClass('geofencesListTD');
        tr.appendChild(nameTd);

        //Address
        var addressTd = document.createElement('td');
        $(addressTd).html(item.fullAddress);
        $(addressTd).addClass('geofencesListTD');
        tr.appendChild(addressTd);

        //Latitude
        var latitudeTd = document.createElement('td');
        $(latitudeTd).html(item.latitude);
        $(latitudeTd).addClass('geofencesListTD geofencesListCenteredTD');
        tr.appendChild(latitudeTd);

        //Longitude
        var longitudeTd = document.createElement('td');
        $(longitudeTd).html(item.longitude);
        $(longitudeTd).addClass('geofencesListTD geofencesListCenteredTD');
        tr.appendChild(longitudeTd);

        //Radius
        var radiusTd = document.createElement('td');
        if (item.shapeId == 1) {
            $(radiusTd).html(item.radius);
        }
        else {
            $(radiusTd).html('POLY');
        }
        $(radiusTd).addClass('geofencesListTD geofencesListCenteredTD');
        tr.appendChild(radiusTd);

        //Geofence Alert Type
        var geoAlertTypeTd = document.createElement('td');
        $(geoAlertTypeTd).html(item.geofenceAlertTypeName);
        $(geoAlertTypeTd).addClass('geofencesListTD');
        tr.appendChild(geoAlertTypeTd);

        //Edit
        var editTd = document.createElement('td');
        $(editTd).addClass('geofencesListTD geofencesListCenteredTD');
        tr.appendChild(editTd);

        var editBtn = document.createElement('button');
        editTd.appendChild(editBtn);
        $(editBtn).attr('data-id', item.id);
        $(editBtn).click(editGeofence);

        var editImg = document.createElement('img');
        $(editImg).attr('src', 'icons/edit_inline.png');
        $(editImg).attr('alt', '');
        $(editImg).attr('width', '16');
        $(editImg).attr('height', '16');
        $(editImg).attr('data-id', item.id);
        editBtn.appendChild(editImg);

        //Delete geofence
        var delTd = document.createElement('td');
        $(delTd).addClass('geofencesListTD geofencesListCenteredTD');
        tr.appendChild(delTd);

        var delBtn = document.createElement('button');
        delTd.appendChild(delBtn);
        $(delBtn).attr('data-id', item.id);
        $(delBtn).click(deleteGeofence);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);
    }
    catch (err) {
        alert('fillGeofenceRecord: ' + err.description);
    }
}

function modifyGeofenceListRecord(id, itm) {
    try {
        var tr = document.getElementById('geofenceTR' + id);
        removeAllChildNodes(tr);
        fillGeofenceRecord(tr, itm);
    }
    catch (err) {
        alert('modifyGeofenceListRecord: ' + err.description);
    }
}

function addGeofenceToList(item) {
    try {
        var tbl = document.getElementById('geofencesTbl');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillGeofenceRecord(tr, item);
    }
    catch (err) {
        alert('addGeofenceToList: ' + err.description);
    }
}

function getGeofences() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonGeofences = dbReadWrite('getGeofences', data, true, false);

        return true;
    }
    catch (err) {
        alert('getGeofences: ' + err.description);
    }
}

function loadGeofences() {
    try {
        //if (jsonGeofences == false) {
        //    getGeofences();
        //}
        getGeofences();
        $('#noGeofencesTitle').hide();
        if (jsonGeofences.geofences.length > 0) {
            clearGeofenceList();
            for (var ind = 0; ind < jsonGeofences.geofences.length; ind++) {
                var jsonItem = eval('(' + jsonGeofences.geofences[ind] + ')');
                addGeofenceToList(jsonItem);
            }
        }
        else {
            $('#noGeofencesTitle').show();
        }
    }
    catch (err) {
        alert('loadGeofences: ' + err.description);
    }
}


