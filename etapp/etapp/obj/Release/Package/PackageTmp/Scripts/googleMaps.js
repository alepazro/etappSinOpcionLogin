function geocodeAddress(address) {
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            //Process results
            var a = results[0];

        } else {
            alert("Geocode was not successful for the following reason: " + status);
        }
    });
}