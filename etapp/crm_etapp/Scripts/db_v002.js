//et_01_1.0.js
var jsonDevices = false;
var lastRefreshOn = false;

function getBasicListError(jqXHR, textStatus, errorThrown) {
    try {
        return false;
    }
    catch (err) {
        alert('getBasicListError: ' + err.description);
    }
}

//This routine returns a json with ID, VALUE of an entity.
function getBasicList(entityName, callback) {
    try {
        var token = getTokenCookie('ETTK');
        var data = 'entityName=' + escape(entityName);
        $.ajax({
            type: "GET",
            url: 'https://pre.etrack.ws/etrack.svc/getBasicList/' + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: callback,
            error: getBasicListError,
            async: false
        });
    }
    catch (err) {
        alert('getBasicList: ' + err.description);
    }
}

function getDeviceById(deviceId) {
    try {
        var jsonDevice = false;
        var t = getTokenCookie('ETTK');
        $.ajax({
            url: 'https://pre.etrack.ws/etrack.svc/deviceInfo/' + t + '/' + escape(deviceId),
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                jsonDevice = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch device info');
            },
            async: false
        });

        return jsonDevice;
    }
    catch (err) {
    }
}

function getDeviceBySearch(source,groupId,searchText) {
    try {
        if (source == null) {
            source = 0;
        }
        if (_.isUndefined(groupId)) {
            groupId = '';
        }
        if (searchText == null) {
            searchText = '';
        }
        if (_.isUndefined(searchText)) {
            searchText = '';
        }
        var t = getTokenCookie('ETTK');
        var ret = false;
        if ((t == null) || (t == '')) {
            if (location.pathname.toLowerCase().indexOf('index.html') == -1) {
                location.href = 'index.html';
            }
            ret = false;
        }
        else {
            jQuery.ajax({
                url: 'ETWS.asmx/getDeviceBySearchText',
                data: 't=' + escape(t) + '&lastRefreshOn=' + escape(lastRefreshOn) + '&source=' + source + '&groupId=' + groupId + '&searchText=' + searchText,
                dataType: 'xml',
                type: "POST",
                success: function (xml, textStatus) {
                    if (textStatus == 'success') {
                        if (($("string", xml).text()) == 'failure') {
                            // alert('Error loading devices');
                            ret = false;
                        }

                        jsonDevices = eval('(' + $("string", xml).text() + ')');
                        if (jsonDevices.result == 'failure') {
                            //alert(jsonDevices.error);
                            if (jsonDevices.error = 'LOGOUT') {
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
                    else {
                        //alert('Error loading devices');
                        ret = false;
                    }
                },
                error: function (result) {
                    //alert('Error loading devices');
                    ret = false;
                },
                async: false
            });
        }
        return ret;


    } catch (err) {

    }

}

function getDevices(source, groupId) {
    try {
        if (source == null) {
            source = 0;
        }
        if (_.isUndefined(groupId)) {
            groupId = '';
        }
        var t = getTokenCookie('ETTK');
        var ret = false;
        if ((t == null) || (t == '')) {
            if (location.pathname.toLowerCase().indexOf('index.html') == -1) {
                location.href = 'index.html';
            }
            ret = false;
        }
        else {
            jQuery.ajax({
                url: 'ETWS.asmx/getDevices',
                data: 't=' + escape(t) + '&lastRefreshOn=' + escape(lastRefreshOn) + '&source=' + source + '&groupId=' + groupId,
                dataType: 'xml',
                type: "POST",
                success: function (xml, textStatus) {
                    if (textStatus == 'success') {
                        if (($("string", xml).text()) == 'failure') {
                            // alert('Error loading devices');
                            ret = false;
                        }

                        jsonDevices = eval('(' + $("string", xml).text() + ')');
                        if (jsonDevices.result == 'failure') {
                            //alert(jsonDevices.error);
                            if (jsonDevices.error = 'LOGOUT') {
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
                    else {
                        //alert('Error loading devices');
                        ret = false;
                    }
                },
                error: function (result) {
                    //alert('Error loading devices');
                    ret = false;
                },
                async: false
            });
        }
        return ret;
    }
    catch (err) {
        //alert('Error loading devices');
        return false;
    }
}

function dbReadWriteAsync(methodName, data, success, failure) {
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
                url: 'ETWS.asmx/' + methodName,
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

function dbReadWrite(methodName, data, alertFailure, isAsync) {
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
                url: 'ETWS.asmx/' + methodName,
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
                                if (jsonObj.error == 'LOGOUT') {
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

function getValue(valueName) {
    try {
        var jsonValue = false;
        var t = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        $.ajax({
            url: 'etrest.svc/getValue/' + t + '/' + noCache + '/' + escape(valueName),
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                jsonValue = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch info');
            },
            async: false
        });

        return jsonValue;

    }
    catch (err) {

    }
}

function getMethodSync(method, param1, param2, param3) {
    try{
        var jsonResult = false;
        var t = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);

        var url = 'etrest.svc/' + method + '/' + t + '/' + noCache;

        if (!_.isUndefined(param1)) {
            url = url + '/' + param1;
        }
        if (!_.isUndefined(param2)) {
            url = url + '/' + param2;
        }
        if (!_.isUndefined(param3)) {
            url = url + '/' + param3;
        }


        $.ajax({
            url: url,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                jsonResult = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch info');
            },
            async: false
        });

        return jsonResult;
    }
    catch (err) {

    }
}

function getDb(method, data) {
    try {
        var result = false;
        var url = 'https://pre.etrack.ws/ws.svc/' + method;
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
        //var url = 'https://pre.etrack.ws/ws.svc/' + method;
        var url = 'https://pre.etrack.ws/ws.svc/' + method;
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
