function dbReadWriteAsync(ws, methodName, data, success, failure) {
    try {
        var ret = true;
        if (methodName == undefined || methodName == null) {
            alert('Invalid method name');
            ret = false;
        }
        else {
            if (data == undefined || data == null) {
                data = '';
            }

            jQuery.ajax({
                url: ws + '/' + methodName,
                data: data,
                dataType: 'xml',
                type: "POST",
                success: success,
                error: failure,
                async: true
            });
        }
    }
    catch (err) {
        alert('dbReadWriteAsync: ' + err.description);
    }
}

function dbReadWrite(ws, methodName, data, alertFailure, isAsync) {
    var ret = true;
    var jsonObj = false;

    if (isAsync == undefined) {
        isAsync = true;
    }

    if (alertFailure == undefined) {
        alertFailure = true;
    }

    try {
        if (methodName == undefined || methodName == null) {
            alert('Invalid method name');
            ret = false;
        }
        else {
            if (data == undefined || data == null) {
                data = '';
            }

            jQuery.ajax({
                url: ws + '/' + methodName,
                data: data,
                dataType: 'xml',
                type: "POST",
                success: function (xml, textStatus) {
                    if (textStatus == 'success') {
                        if (($("string", xml).text()) == 'failure') {
                            ret = false;
                        }
                        else {
                            jsonObj = eval('(' + $("string", xml).text() + ')');
                            if (jsonObj.result == 'failure') {
                                if (alertFailure == true) {
                                    alert(jsonObj.error);
                                }
                                if (jsonObj.error = 'LOGOUT') {
                                    logout();
                                }
                                else {
                                    ret = false;
                                }
                            }
                            else {
                                ret = true;
                            }
                        }
                    }
                    else {
                        ret = false;
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //alert(errorThrown);
                    ret = false;
                },
                async: isAsync
            });
        }

        return jsonObj;

    }
    catch (err) {
        alert('dbReadWrite: Error loading ' + methodName);
        return false;
    }
}

function jsonGET(ws, method, params, isArray) {
    try {
        var t = getTokenCookie('ETCRMTK');

        if (t == '') {
            location.href = 'crmLogin.html';
            return;
        }

        var url = ws + '/' + method + '/' + t + '/' + Math.random();
        if (params != '') {
            url = url + '?' + params;
        }

        var jsonData;
        if (isArray == true) {
            jsonData = [];
        }

        $.ajax({
            url: url,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                jsonData = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch info.  Please try again.');
            },
            async: false
        });

        return jsonData;
    }
    catch (err) {

    }
}

function jsonPOST(ws, method, data) {
    try {
        var t = getTokenCookie('ETCRMTK');

        if (t == '') {
            location.href = 'crmLogin.html';
            return;
        }

        data = JSON.stringify(data);

        var url = ws + '/' + method + '/' + t;
        var res;

        $.ajax({
            url: url,
            type: "POST",
            data: data,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                res = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to save info.  Please try again.');
                res = { isOk: false };
            },
            async: false
        });

        return res;
    }
    catch (err) {
    }
}

function getDb(method, data) {
    try {
        var result = false;
        var url = 'https://localhost:44385/ws.svc/' + method;
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
            success: function (data) {
                result = data;
            },
            error: function (err) {

            },
            async: false
        });

        return result;

    }
    catch (err) {

    }
}

function postDb(method, data, params) {
    try {
        var result = false;
        var url = 'https://localhost:44385/ws.svc/' + method;
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
