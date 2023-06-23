function getDb(service, method, data, async) {
    try {
        var result = false;
        //var url = 'https://pre.etrack.ws/' + service + '/' + method;
        var url = 'https://pre.etrack.ws/' + service + '/' + method;
        if (data.length > 0) {
            url = url + '?' + data;
        }
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            data: 0,
            dataType: "json",
            processdata: false,
            cache: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {

            },
            async: async
        });

        return result;

    }
    catch (err) {

    }
}

function postDb(service, method, data, params) {
    try {
        var result = false;
        var url = 'https://pre.etrack.ws/' + service + '/' + method;
        if (!_.isUndefined(params)) {
            if (params.length > 0) {
                url = url + '?' + params;
            }
        }
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {
                var a = 1;
            },
            async: false
        });

        return result;

    }
    catch (err) {

    }
}
