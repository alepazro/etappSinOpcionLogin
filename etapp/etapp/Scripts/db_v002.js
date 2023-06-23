//const { forEach } = require("core-js/library/es6/array");
//test git
//et_01_1.0.js
var jsonDevices = false;
var jsonDevicesGroupsNew = false;
var lastRefreshOn = false;
var urlServicio = 'https://localhost:44385';
//rest git
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
            url: urlServicio + '/etrack.svc/getBasicList/' + escape(token),
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

            //url: urlServicio+'/etrack.svc/deviceInfo/' + t + '/' + escape(deviceId),
            url: urlServicio + '/etrack.svc/deviceInfo/' + t + '/' + escape(deviceId),
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
function getDeviceBySearch(source, groupId, searchText) {
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
                        jsonDevicesGroupsNew = eval('(' + $("string", xml).text() + ')');
                        if (jsonDevicesGroupsNew.result == 'failure') {
                            //alert(jsonDevices.error);
                            if (jsonDevicesGroupsNew.error = 'LOGOUT') {
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
        /* if (_.isUndefined(groupId) || groupId==null ) {*/
        if (groupId == null) {
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
function getDevicesGroupNew(source, groupId) {
    try {
        if (source == null) {
            source = 0;
        } 
        /* if (_.isUndefined(groupId) || groupId==null ) {*/
        if (groupId == null) {
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
                url: 'ETWS.asmx/getDevicesGroupsNew',
                data: 't=' + escape(t) + '&lastRefreshOn=' + escape(lastRefreshOn) + '&source=' + source + '&groupId=' + groupId,
                dataType: 'xml',
                type: "POST",
                success: function (xml, textStatus) {
                    if (textStatus == 'success') {
                        if (($("string", xml).text()) == 'failure') {
                            // alert('Error loading devices');
                            ret = false;
                        }
                        jsonDevicesGroupsNew = eval('(' + $("string", xml).text() + ')');
                        if (jsonDevicesGroupsNew.result == 'failure') {
                            //alert(jsonDevices.error);
                            if (jsonDevicesGroupsNew.error = 'LOGOUT') {
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
        var arrS1 = [];
        var arrS2 = [];
        var arrS3 = [];
        var arrS4 = [];
        if (methodName == undefined || methodName == null) {
            alert('Invalid method name');
            ret = false;
        }
        else if (methodName == "getTemperatureLog") {
            if (data == undefined || data == null) {
                data = '';
            }
            try {
                //jQuery.ajax({
                //    url: urlServicio+'/reports?' + data,
                //    data: 0,
                //    dataType: 'xml',
                //    type: "GET",
                //    success: success,
                //    error: failure,
                //    async: true
                //});
                $.ajax({
                    url: urlServicio + '/etrest.svc/reports',
                    type: "GET",
                    data: data,
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8",
                    processdata: true,
                    success: function (data, textStatus, jqXHR) {
                        var s1 = 'd013431beeb1';
                        var s2 = 'f9fda1d95b81';
                        var s3 = 'f66d3d4273bb';

                        data.forEach(function (element) {
                            if (element.SensorID == s1) {
                                arrS1.push(element.Temp);
                            } else if (element.SensorID == s2) {
                                arrS2.push(element.Temp);

                            } else if (element.SensorID == s3) {
                                arrS3.push(element.Temp);
                            } else {
                                arrS4.push(element.Temp);

                            }
                        });
                        $(document).ready(loadReportGraphic(arrS1, arrS2, arrS3, arrS4));
                        $(document).bind("kendo:skinChange", loadReportGraphic(arrS1, arrS2, arrS3, arrS4));
                        var hola = "";

                        //var gridDataSource = new kendo.data.DataSource({
                        //    data: data,
                        //    schema: {
                        //        model: {
                        //            fields: {
                        //                ID: { type: "number" },
                        //                HDevicesID: { type: "number" },
                        //                DeviceID: { type: "string" },
                        //                EventDate: { type: "date" },
                        //                SensorID: { type: "string" },
                        //                Temp: { type: "string" }
                        //            }
                        //        }
                        //    },
                        //    pageSize: 10,
                        //    sort: {
                        //        field: "ID",
                        //        dir: "desc"
                        //    }
                        //});
                        //$("#reportContent").kendoGrid({
                        //    dataSource: gridDataSource,
                        //    height: 400,
                        //    pageable: true,
                        //    sortable: true,
                        //    filterable: true,
                        //    columns: [{
                        //        field: "ID",
                        //        title: "ID",
                        //        width: 50
                        //    }, {
                        //        field: "HDevicesID",
                        //        width: 50,
                        //    }, {
                        //        field: "DeviceID",
                        //        title: "DeviceID",
                        //        width: 50,
                        //    }, {
                        //        field: "EventDate",
                        //        title: "EventDate",
                        //        width: 50,
                        //    }, {
                        //        field: "SensorID",
                        //        title: "SensorID",
                        //        width: 50,
                        //    },
                        //        {
                        //            field: "Temp",
                        //            title: "Temp",
                        //            width: 50,
                        //        } ]
                        //});

                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        var a = 1;
                    },
                    async: true
                });

            }
            catch (err) {
                console.log(err)
            }



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
                                    console.log(jsonObj.error);
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
function dbReadWriteNEW(methodName, data, alertFailure, isAsync) {
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
    try {
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
        var url = urlServicio + '/ws.svc/' + method;
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
        var url = urlServicio + '/ws.svc/' + method;
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
function loadReportGraphic(S1, S2, S3, S4) {
    $("#chart").kendoChart({
        title: {
            text: "Gross domestic product growth \n /GDP annual %/"
        },
        legend: {
            position: "bottom"
        },
        chartArea: {
            background: ""
        },
        seriesDefaults: {
            type: "line",
            style: "smooth"
        },
        series: [{
            name: "S1",
            data: [3.907, 7.943, 7.848, 9.284, 9.263, 9.801, 3.890, 8.238, 9.552, 6.855]
        }, {
            name: "S2",
            data: [3.907, 7.943, 7.848, 9.284, 9.263, 9.801, 3.890, 8.238, 9.552, 6.855]
        }, {
            name: "S3",
            data: [3.907, 7.943, 7.848, 9.284, 9.263, 9.801, 3.890, 8.238, 9.552, 6.855]
        }, {
            name: "S4",
            data: [3.907, 7.943, 7.848, 9.284, 9.263, 9.801, 3.890, 8.238, 9.552, 6.855]
        }],
        valueAxis: {
            labels: {
                format: "{0}%"
            },
            line: {
                visible: false
            },
            axisCrossingValue: -10
        },
        categoryAxis: {
            categories: [2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011],
            majorGridLines: {
                visible: false
            },
            labels: {
                rotation: "auto"
            }
        },
        tooltip: {
            visible: true,
            format: "{0}%",
            template: "#= series.name #: #= value #"
        }
    });



}
function dbReadWriteAsyncNew(methodName, data, success, failure) {

    var array = [];
    try {
        var ret = true;
        if (methodName == undefined || methodName == null) {
            alert('Invalid method name');
            ret = false;
        }
        else if (methodName == "getReportNew") {
            if (data == undefined || data == null) {
                data = '';
            }
            try {


                $.ajax({
                    url: urlServicio + '/etrest.svc/reports',
                    type: "GET",
                    data: data,
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8",
                    processdata: true,
                    success: function (data, textStatus, jqXHR) {
                        array = data;
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        var a = 1;
                    },
                    async: false
                });

            }
            catch (err) {
                console.log(err)
            }
        }
        return array;
    }
    catch (err) {
        alert('dbReadWriteAsync: ' + err.description);
    }
}
function dbReadCompaniesDevicesEvents(data, success, failure) {

    var array = '';
    try {
        $.ajax({
            url: urlServicio + '/etrest.svc/getDevicesEvents',
            type: "GET",
            data: data,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                array = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var a = 1;
            },
            async: false
        });

        return array;
    }
    catch (err) {
        alert('dbReadCompaniesDevicesEvents: ' + err.description);
    }
}
function dbReadGeofences(data, success, failure) {

    var array = '';
    try {
        $.ajax({
            url: urlServicio + '/etrest.svc/getGeofences',
            type: "GET",
            data: data,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                array = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var a = 1;
            },
            async: false
        });

        return array;
    }
    catch (err) {
        alert('dbReadGeofences: ' + err.description);
    }
}
function dbReadDrivers(data, success, failure) {

    var array = '';
    try {
        $.ajax({
            url: urlServicio + '/etrest.svc/getDrivers',
            type: "GET",
            data: data,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                array = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var a = 1;
            },
            async: false
        });

        return array;
    }
    catch (err) {
        alert('dbReadGeofences: ' + err.description);
    }
}
function postSendFeedBack(visitedPage, type, description) {
    var response = "";
    try {
        var token = getTokenCookie('ETTK');
        var data = 'token=' + escape(token) + '&visitedPage=' + escape(visitedPage) + '&type=' + escape(type) + '&description=' + escape(description);
        $.ajax({
            type: "POST",
            url: urlServicio + '/etrest.svc/postSendFeedBack?' + data,
            contentType: 'application/json',
            data: 0,
            dataType: "json",
            processdata: false,
            success: function (data, textStatus, jqXHR) {
                
                response = JSON.parse(data);
            },
            error: function () {
                response = "{id:-1,value:Error in Javascript,name:Error in Javascript}"

            },
            async: false
        });
    }
    catch (err) {
        
        alert('getBasicList: ' + err.description);
        response = "{id:-1,value:Error " + err.description+",name:Error in Javascript}"
    }
    return response;
}
function GetFeedBackType() {
    var response = "";
    
    try {
        var token = getTokenCookie('ETTK');
        $.ajax({
            type: "GET",
            url: urlServicio + '/etrest.svc/GetFeedbakTypes?token=' + escape(token),
            contentType: 'application/json',
            data: 0,
            dataType: "json",
            processdata: false,
            success: function (data, textStatus, jqXHR) {
                
                response = JSON.parse(data);
            },
            error: function () {
                response = "{id:-1,value:Error in Javascript,name:Error in Javascript}"

            },
            async: false
        });
    }
    catch (err) {

        alert('GetdFeedBackType: ' + err.description);
    }
    return response;
}
function GetJobSupport() {
    try {
        var jsonJob = false;
        var t = getTokenCookie('ETTK');
        $.ajax({

            //url: urlServicio+'/etrack.svc/deviceInfo/' + t + '/' + escape(deviceId),
            url: urlServicio + '/jobs.svc/jobSupportTables?token=' + escape(t),
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {

                jsonJob = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {

                alert('Failed to fetch device info');
            },
            async: false
        });

        return jsonJob;
    }
    catch (err) {
    }
}
function saveJobDb(jobObj) {
    
    try {
        //data = 't=' + getTokenCookie('ETTK') + '&JobNumber=' + escape(JobNumber) + '&deviceId=' + escape(deviceId) + '&isGeofence=' + escape(isGeofence) + '&geofenceId=' + escape(geofenceId) + '&name=' + escape(name) + '&phone=' + escape(phone) + '&street=' + escape(street) + '&city=' + escape(city) + '&state=' + escape(state) + '&postalCode=' + escape(postalCode) + '&description=' + escape(description) + '&lat=' + escape(lat) + '&lng=' + escape(lng) + '&sendSMS=' + escape(sendSMS) + '&driverId=' + driverId + '&via=' + via + '&durationHH=' + durationHH + '&durationMM=' + durationMM + '&dueDate=' + dueDate + '&StartOn=' + StartOn + '&jobpriority=' + jobpriority + '&jobcategories=' + jobcategories;
        var tmpJson = dbReadWrite('saveWorkOrderNEW', jobObj, true, false);

        if (tmpJson) {
            if (tmpJson.result != 'failure') {
                if (tmpJson.result == 'NOTSENT') {
                    //alert('Could not send Job.  Please make sure that the device has a driver assigned and that the driver has a cellphone assigned.');
                    toastr.error('Could not send Job.  Please make sure that the device has a driver assigned and that the driver has a cellphone assigned.');
                }
                else {
                    //alert('Job sent successfully');
                    //clearDispatchPanel();
                    toastr.success('Job sent successfully');
                    clearJobs(3);
                    $("#txtaDescription").val('');
                    $("#dispatchJobDescription").val('');                    
                    $('#jobDlg').dialog("close");
                }
            }
            else {
                toastr.error('Job could not be sent.  Please try again.');
            }
        }
    }
    catch (err) {
        alert('dispatchThisVehicle: ' + err.description);
    }
}
function getDbNEW(service, method, datap, async) {
    try {
        var result = false;
        //var url = 'https://localhost:44385/' + service + '/' + method;
        var url = 'https://localhost:44385/' + service + '/' + method;
        if (datap.length > 0) {
            url = url + '?' + datap;
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
                
                console.log(err)
            },
            async: async
        });

        return result;

    }
    catch (err) {

    }
}
function GetJobStops(jobId) {
    try {
        var jsonJobStop = false;        
        var t = getTokenCookie('ETTK');
        $.ajax({

            //url: urlServicio+'/etrack.svc/deviceInfo/' + t + '/' + escape(deviceId),
            url: urlServicio + '/jobs.svc/GetJobStops?token=' + escape(t) + '&jobUniquekey=' + escape(jobId),
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                jsonJobStop = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch device info');
            },
            async: false
        });
        return jsonJobStop;
    }
    catch (err) {
    }
}
function delJobStopDB(data) {
    try {
        
        var jsonJobStop = false;
        var t = getTokenCookie('ETTK');
        var objeto = JSON.stringify(data);
        $.ajax({
            //url: urlServicio+'/etrack.svc/deviceInfo/' + t + '/' + escape(deviceId),
            url: urlServicio + '/jobs.svc/jobStop/'+escape(t),
            type: "POST",
            data: objeto,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                
                jsonJobStop = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
                alert('Failed to fetch device info');
                console.log(jqXHR)
            },
            async: false
        });
        return jsonJobStop;
    }
    catch (err) {
    }

}
function getDbJob(method, data) {
    try {
        
        var result = false;
        var token = getTokenCookie('ETTK');
        var url = urlServicio + '/jobs.svc/' + method;
        if (data!=null && data.length > 0) {
            url = url + '?token=' + token + '&' + data;
        } else {
            url = url + '?token=' + token
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
function postDbJob(method, data, params) {postDbJob
    try {
        
        
        var result = false;
        var token = getTokenCookie('ETTK');
        var url = urlServicio + '/jobs.svc/' + method + '?token=' + token;
        if (!_.isUndefined(params)) {
            if (params.length > 0) {
                url = url + '&' + params;
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
                
                console.log(err)
                var a = 1;
            },
            async: false
        });

        return result;

    }
    catch (err) {
        console.log("Error----> "+err);
    }
}
function getvalidatetoken() {
    try {
        
        var data = '';
        var result = false;
        var token = getTokenCookie('ETTK');
        var url = urlServicio + '/etrest.svc/sso/videoapp/' + token + '?ep=false';       
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                
                result = data;
            },
            error: function (err) {
                
                console.log(err)
                var a = 1;
            },
            async: false
        });

        return result;

    }
    catch (err) {
        console.log("error-> "+err);
    }
}
function postTempSensors(data) {
    var response = "";
    $("#msg").val = "";
    
    var objeto = JSON.stringify(data);
    try {
        var token = getTokenCookie('ETTK');
        //var data = 'token=' + escape(token);
        $.ajax({
            type: "POST",
            url: urlServicio + '/ws.svc/savesensor/' + token,
            contentType: 'application/json',
            data: objeto,
            dataType: "json",
            processdata: false,
            success: function (data, textStatus, jqXHR) {
                
                response = JSON.parse(data);
            },
            error: function () {
                
                response = "{id:-1,value:Error in Javascript,name:Error in Javascript}"

            },
            async: false
        });
    }
    catch (err) {
        
        console.log(err);
    }
    return response;
}
function getsensors() {
    try {
        
        var data = '';
        var result = false;
        var token = getTokenCookie('ETTK');
        var url = urlServicio + '/ws.svc/sensors/' + token;
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                
                result = data;
            },
            error: function (err) {
                
                console.log(err)
                var a = 1;
            },
            async: false
        });

        return result;

    }
    catch (err) {
        console.log("error-> " + err);
    }
}
function UpdateTempSensors(data) {
    var response = "";
    $("#msg").val = "";
    
    var objeto = JSON.stringify(data);
    try {
        var token = getTokenCookie('ETTK');
        //var data = 'token=' + escape(token);
        $.ajax({
            type: "POST",
            url: urlServicio + '/ws.svc/updatesensor/' + token,
            contentType: 'application/json',
            data: objeto,
            dataType: "json",
            processdata: false,
            success: function (data, textStatus, jqXHR) {
                
                response = data;
            },
            error: function () {
                
                response = "{id:-1,value:Error in Javascript,name:Error in Javascript}"

            },
            async: false
        });
    }
    catch (err) {
        
        console.log(err);
    }
    return response;
}

function getTrakingnumberDb() {
    let data1;
    var token = getTokenCookie('ETTK');
    
    try {
        $.ajax({
            url: 'https://localhost:44385/ws.svc/getlisttn/' + token,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {

                data1 = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {

                data1 = null;
                alert('Failed to fetch device info');
            },
            async: false
        });

        return data1;
    }
    catch (err) {

        console.log(err)
    }
}
function putTrakingnumber(data) {
    
    var response = "";
    $("#msg").val = "";
    
    var objeto = JSON.stringify(data);
    try {
        var token = getTokenCookie('ETTK');
        //var data = 'token=' + escape(token);
        $.ajax({
            type: "POST",
            url: urlServicio + '/ws.svc/updatetn/' + token,
            contentType: 'application/json',
            data: objeto,
            dataType: "json",
            processdata: false,
            success: function (data, textStatus, jqXHR) {
                
                response = data;// JSON.parse(data);
            },
            error: function () {
                
                response = "{id:-1,value:Error in Javascript,name:Error in Javascript}"

            },
            async: false
        });
    }
    catch (err) {

        console.log(err);
    }
    return response;
}
function postTrakingnumber(data) {
    
    var response = "";
    $("#msg").val = "";
    
    var objeto = JSON.stringify(data);
    try {
        var token = getTokenCookie('ETTK');
        //var data = 'token=' + escape(token);
        $.ajax({
            type: "POST",
            url: urlServicio + '/ws.svc/savetn/' + token,
            contentType: 'application/json',
            data: objeto,
            dataType: "json",
            processdata: false,
            success: function (data, textStatus, jqXHR) {
                
                response = data;// JSON.parse(data);
            },
            error: function () {
                
                response = "{id:-1,value:Error in Javascript,name:Error in Javascript}"

            },
            async: false
        });
    }
    catch (err) {

        console.log(err);
    }
    return response;
}
function postDbJob2(method, data) {
    
    try {


        var result = false;
        var token = getTokenCookie('ETTK');
        var url = urlServicio + '/jobs.svc/' + method + '?token=' + token;        
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
                
                console.log(err)
                var a = 1;
            },
            async: false
        });

        return result;

    }
    catch (err) {
        console.log("Error----> " + err);
    }
}
