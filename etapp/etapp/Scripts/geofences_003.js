//SQL: lat lon, lat lon, ...
//google fusion tables: lat,lon lat,lon lat,lon
//google maps poly: json format: {lat,lon} {lat,lon}...

var jsonGeofences = false;
var jsonGeofencesList = false;

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
            $(radiusTd).html(item.radiusFeet);
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

function getGeofences_All() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonGeofences = dbReadWrite('getGeofences_All', data, true, false);

        return true;
    }
    catch (err) {
        alert('getGeofences_All: ' + err.description);
    }
}

function getGeofences_AllList() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonGeofencesList = dbReadWrite('getGeofences_AllList', data, true, false);

        return true;
    }
    catch (err) {
        alert('getGeofences_AllList: ' + err.description);
    }
}

function loadGeofences() {
    try {
        //if (jsonGeofences == false) {
        //    getGeofences();
        //}
        getGeofences_All();
        $('#noGeofencesTitle').hide();
        if (jsonGeofences.length > 0) {
            clearGeofenceList();
            //for (var ind = 0; ind < jsonGeofences.geofences.length; ind++) {
            for (var ind = 0; ind < jsonGeofences.length; ind++) {
                var jsonItem = jsonGeofences[ind]//eval('(' + jsonGeofences[ind] + ')');
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


