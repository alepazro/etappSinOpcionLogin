var jsonElog = false;

function getTokenCookie(c_name) {
    try {
        var t = "";
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=");
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1;
                c_end = document.cookie.indexOf(";", c_start);
                if (c_end == -1) c_end = document.cookie.length;
                t = unescape(document.cookie.substring(c_start, c_end));
            }
        }
        return t;
    }
    catch (err) {
        alert('getTokenCookie: ' + err.Description);
    }
}

function openElog() {
    try{
        var token = getTokenCookie('ETTK');
        if (token != '') {
           
            var url = 'https://pre.etrack.ws/etrest.svc/wlius/key/' + token;

            $.ajax({
                url: url,
                type: "POST",
                data: 0,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (data, textStatus, jqXHR) {
                    if (data.HasElog == true) {
                        if (data.ApiKey != '') {
                            location.href = 'http://roto.wlius.com/FleetWebClient/LoginForm.aspx?apiKey=' + data.ApiKey;
                        } else {
                            alert("Invalid eLog API Key. Please contact us.");
                            window.close();
                        }
                    } else {
                        alert("You have not activated your ELOG account yet.  Please contact us!");
                        window.close();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var a = 1;
                },
                async: true
            });

        }

    } catch (err) {
        alert(err.message);
    }
}
function openHvideo() {
    try {
        
        var token = getTokenCookie('ETTK');
        if (token != '') {

            //var url = 'https://pre.etrack.ws/etrest.svc/hvideo/key/' + token;
            var url = 'https://pre.etrack.ws/etrest.svc/hvideo/key/' + token;

            $.ajax({
                url: url,
                type: "POST",
                data: 0,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (data, textStatus, jqXHR) {
                    if (data.HasVideo == true) {
                        if (data.ApiKey != '') {
                            location.href = 'http://video.easitrack.net/?custId=' + data.ApiKey;
                        } else {
                            alert("Invalid hVideo API Key. Please contact us.");
                            window.close();
                        }
                    } else {
                        alert("You have not activated your HVideo account yet.  Please contact us!");
                        window.close();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    
                    var a = 1;
                },
                async: true
            });

        }

    } catch (err) {
        
        console.log(err.message);
    }
}