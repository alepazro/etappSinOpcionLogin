var thisDelay = 30000;

function pageEngagement() {
    sendTicker();
    engagementTicker();
}

function engagementTicker() {
    setTimeout(function () {
        sendTicker();
        engagementTicker();
    }, thisDelay);
}

function sendTicker() {
    try {
        
        var transId = getEngagementCookie();
        var data = { transId: transId, delay: thisDelay }
        data = JSON.stringify(data);

        $.ajax({
            type: "POST",
            url: "etrest.svc/pageEngagement",
            contentType: "application/json",
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                

                var a = 1;
            },
            error: function (a, b, c) {
                

                var x = 1;
            }
        });

    }
    catch (err) {

    }
}

function getEngagementCookie() {
    try {
        var t = "";
        var c_name = 'ETENG';
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
        alert('getEngagementCookie: ' + err.message);
    }
}