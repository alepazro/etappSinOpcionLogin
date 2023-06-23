function tooltipsSetUp() {
    try {
        $('#help_freezeZoom').tooltip();
        $('#help_showTraffic').tooltip();
        $('#help_geoLayer').tooltip();
//        $('#help_hotSpotsLayer').tooltip();
    }
    catch (err) {
        alert('tooltipsSetUp: ' + err.description);
    }
}