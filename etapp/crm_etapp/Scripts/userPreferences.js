function updateUserPref(jsonUserPref) {
    try {
        var token = getTokenCookie('ETTK');
        $.ajax({
            type: "POST",
            url: 'https://pre.etrack.ws/etrack.svc/updateUserPref/' + escape(token),
            contentType: 'application/json',
            data: JSON.stringify(jsonUserPref),
            dataType: "json",
            processdata: false,
            success: function (a, b, c) {
                var a = 1;
            },
            error: function (a, b, c) {
                var b = 1;
            },
            async: false
        });
    }
    catch (err) {
        alert('updateUserPref: ' + err.description);
    }
}