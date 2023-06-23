var streetViewPanorama = false;
var streetViewMap = false;
var panoramaOptions = false;

function setStreetViewDlg() {
    try {
        $("#streetViewDlg").dialog({
            height: 500,
            width: 450,
            autoOpen: false,
            modal: true,
            buttons: {
                Close: function () {
                    $(this).dialog("close");
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                streetViewPanorama = false;
                streetViewMap = false;
                panoramaOptions = false;
            }
        });
    }
    catch (err) {
        alert('setStreetViewDlg: ' + err.description);
    }
}

function openStreetView(obj) {
    try {
        var id = $(obj.target).attr('data-id');
        var itm = document.getElementById('streetViewDiv' + id);
        var lat = $(itm).attr('data-lat');
        var lng = $(itm).attr('data-lng');
        var heading = $(itm).attr('data-heading');
        var intHeading = getHeadingDegrees(heading);

        streetViewMap = new google.maps.LatLng(lat, lng);
        panoramaOptions = {
            position: streetViewMap,
            pov: {
                heading: intHeading,
                pitch: 0,
                zoom: 1
            }
        };
        $("#streetViewDlg").dialog('open')

        streetViewPanorama = new google.maps.StreetViewPanorama(document.getElementById("streetViewContainer"), panoramaOptions);
        streetViewPanorama.setVisible(true);

    }
    catch (err) {
        alert('openStreetView: ' + err.description);
    }
}

function updateStreetViewBtn(id, lat, lng, heading) {
    try {
        if ($('#streetViewDiv' + id)) {
            $('#streetViewDiv' + id).attr('data-lat', lat);
            $('#streetViewDiv' + id).attr('data-lng', lng);
            $('#streetViewDiv' + id).attr('data-heading', heading);
        }
    }
    catch (err) {
        alert('updateStreetViewBtn: ' + err.description);
    }
}

function getStreetViewBtn(id, lat, lng, heading) {
    try {
        var streetViewDiv = document.createElement('div');
        $(streetViewDiv).attr('id', 'streetViewDiv' + id);
        $(streetViewDiv).attr('data-lat', lat);
        $(streetViewDiv).attr('data-lng', lng);
        $(streetViewDiv).attr('data-heading', heading);
        $(streetViewDiv).attr('style', 'float:right;');

        var streetViewBtn = document.createElement('button');
        streetViewDiv.appendChild(streetViewBtn);
        $(streetViewBtn).attr('data-id', id);
        $(streetViewBtn).click(openStreetView);

        var streetViewImg = document.createElement('img');
        $(streetViewImg).attr('src', 'icons/camera.png');
        $(streetViewImg).attr('alt', '');
        $(streetViewImg).attr('width', '20');
        $(streetViewImg).attr('height', '20');
        $(streetViewImg).attr('data-id', id);
        streetViewBtn.appendChild(streetViewImg);

        return streetViewDiv;

    }
    catch (err) {
        alert('getStreetViewBtn: ' + err.description);
    }
}

function getHeadingDegrees(heading) {
    try {
        var intHeading = 0;
        switch (heading) {
            case 'N':
                intHeading = 0;
                break;
            case 'NE':
                intHeading = 45;
                break;
            case 'E':
                intHeading = 90;
                break;
            case 'SE':
                intHeading = 135;
                break;
            case 'S':
                intHeading = 180;
                break;
            case 'SW':
                intHeading = 225;
                break;
            case 'W':
                intHeading = 270;
                break;
            case 'NW':
                intHeading = 315;
                break;
        }

        return intHeading;
    }
    catch (err) {
        alert('getHeadingDegrees: ' + err.description);
    }
}