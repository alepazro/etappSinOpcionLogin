var jsonDeviceData = false;
var jsonDeviceDataSMS = false;
var did = false;
var token = getTokenCookie('ETCRMTK')
var jsonSensor = {} ;
var urlServicio = 'https://pre.etrack.ws/ws.svc';
//AQUI
function loadDeviceData() {    
    try {       
        
        createDeviceNameDataGrid();
        createDeviceDataGrid();
        createDeviceInternalDataGrid();
        createSendMessageDeviceDataGrid();
        createGetResponseMessageDeviceDataGrid();
        
        createSensorsGrid();
        createHistorySensorGrid();

        did = getParameterByName('did');
        if (did != '') {
            var data = "?t=" + getTokenCookie('ETCRMTK') + "&did=" + escape(did);
            //jsonDeviceData = dbReadWrite('ETCRMWS.asmx', 'crmGetDeviceData', data, true, false);
            jsonDeviceData = dbReadWriteWS(urlServicio, 'crmGetDeviceData', data, true, false);
            //jsonDeviceData = JSON.stringify(jsonDeviceData);
            
            var device = jsonDeviceData.Device// eval('(' + jsonDeviceData.Name + ')');
            document.title = device.Name;
            $('#deviceName').text(device.Name);

            populatetblDeviceNameDataGrid(jsonDeviceData.Device)
            
            populateDeviceDataGrid(jsonDeviceData.ListHDevices);
            populateDeviceInternalDataGrid(jsonDeviceData.ListHDevicesInternal);
            populateTempSensorDataGrid(jsonDeviceData.ListHTempSensor);
            populateHistorySensorDataGrid(jsonDeviceData.HistorySensor);

        }
    }
    catch (err) {
    }
}
function populatetblDeviceNameDataGrid(jsonData) {
    try {
        
        jQuery("#tblDeviceName").jqGrid('addRowData',1,jsonData);
        //for (var i = 0; i <= jsonData.length; i++) {
        //    var row = eval('(' + jsonData[i] + ')');
        //    jQuery("#tblDeviceName").jqGrid('addRowData', i + 1, row);
        //    /*  //*/
        //}
    }
    catch (err) {
    }
}

function populateDeviceDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = jsonData[i]; //eval('(' + jsonData[i] + ')');
            jQuery("#tblDeviceData").jqGrid('addRowData', i + 1, row);
      /*  //*/}
    }
    catch (err) {
    }
}

function populateDeviceInternalDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = jsonData[i]; //eval('(' + jsonData[i] + ')');
            jQuery("#tblDeviceInternalData").jqGrid('addRowData', i + 1, row);
        }
    }
    catch (err) {
    }
}
function populateTempSensorDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = jsonData[i]; //eval('(' + jsonData[i] + ')');
            jQuery("#tblSensors").jqGrid('addRowData', i + 1, row);
        }
    }
    catch (err) {
    }
}
function populateHistorySensorDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = jsonData[i]; //eval('(' + jsonData[i] + ')');
            jQuery("#tblhistorysensor").jqGrid('addRowData', i + 1, row);
        }
    }
    catch (err) {
    }
}

function createDeviceNameDataGrid() {
    try {
        jQuery("#tblDeviceName").jqGrid({
            datatype: "local",
            height: 100,
            colNames: ['DeviceType', 'Model', 'DeviceID', 'Name', 'SimNoDB', 'SimNoUnit', 'ReportIgnON', 'ReportTimerIgnOff', 'ReportTurnAngle', 'ReportDistance', 'FakeIgn', 'IgnON', 'IgnOFF', 'ServerIP','ServerPort'],
            colModel: [
                { name: 'DeviceType', index: 'DeviceType', width: 150 },
                { name: 'Model', index: 'Model', width: 150 },
                { name: 'DeviceID', index: 'DeviceID', width: 150 },
                { name: 'Name', index: 'Name', width: 120 },
                { name: 'SimNoDB', eventCode: 'SimNoDB', width: 200 },
                { name: 'SimNoUnit', index: 'SimNoUnit', width: 200 },
                { name: 'ReportIgnON', index: 'ReportIgnON', width: 200 },
                { name: 'ReportTimerIgnOff', index: 'ReportTimerIgnOff', width: 200 },
                { name: 'ReportTurnAngle', index: 'ReportTurnAngle', width: 200 },
                { name: 'ReportDistance', index: 'ReportDistance', width: 200 },
                { name: 'FakeIgn', index: 'FakeIgn', width: 200 },
                { name: 'IgnON', index: 'IgnON', width: 200 },
                { name: 'IgnOFF', index: 'IgnOFF', width: 200 },
                { name: 'ServerIP', index: 'ServerIP', width: 200 },
                { name: 'ServerPort', index: 'ServerPort', width: 200 }
            ],
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblDeviceNamePager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Device Information",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            hiddengrid: false
        });
        $("#tblDeviceName").jqGrid('navGrid', '#tblDeviceNamePager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function createDeviceDataGrid() {
    try {
        jQuery("#tblDeviceData").jqGrid({
            datatype: "local",
            height: 300,
            colNames: ['ID', 'Ign.Stat', 'Ev.Code', 'Ev.Name', 'Ev.Date', 'Speed', 'CreatedOn', 'GPSAge', 'GPSCount', 'Consecutive', 'DeviceTypeID', 'IsBrief',/* 'OriginalEvent', */'Lat', 'Lng', 'RSSI', 'Ble', 'Int.Bat', 'Ext.Bat', 'Delay'],
            colModel: [
                { name: 'id', index: 'id', width: 50 },
                { name: 'ignitionStatus', index: 'ignitionStatus', width: 50 },
                { name: 'eventCode', eventCode: 'eventCode', width: 50 },
                { name: 'eventName', index: 'eventName', width: 100 },
                { name: 'eventDate', index: 'eventDate', width: 150 },
                { name: 'speed', index: 'speed', width: 50 },
                { name: 'createdOn', index: 'createdOn', width: 150 },
                { name: 'gpsAge', index: 'gpsAge', width: 50 },
                { name: 'gpsCount', index: 'gpsCount', width: 100},
                { name: 'consecutive', index: 'consecutive', width: 100 },
                { name: 'deviceTypeID', index: 'deviceTypeID', width: 50 },
                { name: 'isBrief', index: 'isBrief', width: 50 },
                //{ name: 'originalEvent', index: 'originalEvent', width: 50 },
                { name: 'lat', index: 'lat', width: 100 },
                { name: 'lng', index: 'lng', width: 100 },
                { name: 'rssi', index: 'rssi', width: 100 },
                { name: 'ble', index: 'ble', width: 50 },
                { name: 'bi', index: 'bi', width: 50 },
                { name: 'be', index: 'be', width: 50 },
                { name: 'msgDelay', index: 'msgDelay', width: 50 }
                //{ name: 'Temperature1', index: 'Temperature1', width: 50 },
                //{ name: 'Temperature2', index: 'Temperature2', width: 50 },
                //{ name: 'Temperature3', index: 'Temperature3', width: 50 },
                //{ name: 'Temperature4', index: 'Temperature4', width: 50 }
           	],           
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblDeviceDataPager',
            sortname: 'ID',
            viewrecords: true,
            sortorder: "desc",
            caption: "Device Data",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            hiddengrid: false
        });
        $("#tblDeviceData").jqGrid('navGrid', '#tblDeviceDataPager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}
function createHistorySensorGrid() {
    try {
        jQuery("#tblhistorysensor").jqGrid({
            datatype: "local",
            height: 200,
            colNames: ['ID','EventDate', 'Sensor 1', 'Sensor 2', 'Sensor 3', 'Sensor 4', 'Latitude', 'Longitude'],
            colModel: [
                { name: 'ID', index: 'id', width: 100, hidden: true },
                { name: 'EventDate', index: 'ignitionStatus', width: 200 },
                { name: 'Temperature1', eventCode: 'eventCode', width: 100 },
                { name: 'Temperature2', index: 'eventName', width: 100 },
                { name: 'Temperature3', index: 'eventDate', width: 100 },
                { name: 'Temperature4', index: 'speed', width: 100 },
                { name: 'Latitude', index: 'createdOn', width: 100 },
                { name: 'Longitude', index: 'gpsAge', width: 100 }                
            ],
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblhistorysensorPager',
            sortname: 'ID',
            viewrecords: true,
            sortorder: "desc",
            caption: "Device Sensor",
            autowidth: true,  // set 'true' here
            emtyrecords: 'No Records To Display',
            shrinkToFit: true,// well, it's 'true' by default
            hiddengrid: true
        });
        $("#tblhistorysensor").jqGrid('navGrid', '#tblhistorysensorPager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function createDeviceInternalDataGrid() {
    try {
        jQuery("#tblDeviceInternalData").jqGrid({
            datatype: "local",
            height: 200,

            //colNames: ['ID', 'Ign.Stat', 'Ev.Code', 'Ev.Name', 'Ev.Date', 'Created', 'Delay', 'Speed', 'GPSAge', 'Consec', 'IsBrief', 'OriginalEvent', 'Lat', 'Lng', 'ExtraData'],
            //colModel: [
           	//	                    { name: 'id', index: 'id', width: 50 },
           	//	                    { name: 'ignitionStatus', index: 'ignitionStatus', width: 50 },
           	//	                    { name: 'eventCode', eventCode: 'id', width: 50 },
           	//	                    { name: 'eventName', index: 'eventName', width: 100 },
           	//	                    { name: 'eventDate', index: 'eventDate', width: 150 },
           	//	                    { name: 'createdOn', index: 'createdOn', width: 150 },
           	//	                    { name: 'msgDelay', index: 'msgDelay', width: 50 },
           	//	                    { name: 'speed', index: 'speed', width: 50 },
           	//	                    { name: 'gpsAge', index: 'gpsAge', width: 50 },
           	//	                    { name: 'consecutive', index: 'consecutive', width: 50 },
           	//	                    { name: 'isBrief', index: 'isBrief', width: 50 },
           	//	                    { name: 'originalEvent', index: 'originalEvent', width: 50 },
           	//	                    { name: 'lat', index: 'lat', width: 50 },
           	//	                    { name: 'lng', index: 'lng', width: 50 },
           	//	                    { name: 'extraData', index: 'extraData', width: 150 }
           	//                    ],
            colNames: ['MessageType', 'MessageDevice', 'IP', 'Port', 'CreatedOn'],
            colModel: [
                { name: 'MessageType', index: 'MessageType', width: 100 },
                { name: 'MessageDevice', index: 'MessageDevice', width: 500 },
                { name: 'IP', eventCode: 'IP', width: 100 },
                { name: 'Port', index: 'Port', width: 100 },
                { name: 'CreatedOn', index: 'CreatedOn', width: 150 }
            ],
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblDeviceInternalDataPager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Internal Data",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by defaults,
            hiddengrid: false
        });
        $("#tblDeviceInternalData").jqGrid('navGrid', '#tblDeviceInternalDataPager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function sendCommand() {
    
    try{
        var cmd = $('#txtCommand').val();
        if (cmd == '') {
            alert('Please enter a command');
            return;
        }

        var data = { deviceId: did, cmd: cmd };
        data = JSON.stringify(data);
        var res = postDb('sendDeviceCommand', data, '');
        if (res.isOk) {
            alert('the message was successfully registered');
            $('#txtCommand').val('');
            loadDeviceDataSendSMS();

        } else {
            alert('an error occurred while logging the message');
        }

    }
    catch (err) {
        alert('The server reported the following exception' + err)
    }
}

function getResponses() {
    try{
        loadDeviceDataSendSMS();
    }
    catch (err) {

    }
}

function createSendMessageDeviceDataGrid() {
    try {
        jQuery("#tblsendMessageDevice").jqGrid({
            datatype: "local",
            height: 200,

            
            colNames: ['DeviceID','PhoneNumber', 'Destination', 'SMS', 'IsProcessed', 'ProcessedOn', 'SMSCMessage', 'MessageID', 'CreatedOn','SimCarrier'],
            colModel: [
                { name: 'DeviceID', index: 'DeviceID', width: 100 },
                { name: 'PhoneNumber', index: 'PhoneNumber', width: 100 },
                { name: 'Destination', index: 'Destination', width: 100 },
                { name: 'SMS', eventCode: 'SMS', width: 150 },
                { name: 'IsProcessed', index: 'IsProcessed', width: 100 },
                { name: 'ProcessedOn', index: 'ProcessedOn', width: 150 },
                { name: 'SMSCMessage', index: 'SMSCMessage', width: 100 },
                { name: 'MessageID', index: 'MessageID', width: 100 },
                { name: 'CreatedOn', index: 'CreatedOn', width: 150 },
                { name: 'SimCarrier', index: 'SimCarrier', width: 150 }
            ],
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblsendMessageDevicePager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Send Messages",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by defaults
            hiddengrid: false
        });
        $("#tblsendMessageDevice").jqGrid('navGrid', '#tblsendMessageDevicePager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}
function createGetResponseMessageDeviceDataGrid() {
    try {
        jQuery("#tblGetResponseMessageDevice").jqGrid({
            datatype: "local",
            height: 200,


            //colNames: ['DeviceID','Response', 'AreaCode', 'PhoneNumber', 'CreatedOn', 'IsProcessed', 'ProcessedOn'],
            colNames: ['DeviceID', 'Response', 'AreaCode', 'PhoneNumber', 'CreatedOn'],
            colModel: [
                { name: 'DeviceID', index: 'DeviceID', width: 100 },
                { name: 'Response', index: 'Response', width: 500 },
                { name: 'AreaCode', index: 'AreaCode', width: 100 },
                { name: 'PhoneNumber', eventCode: 'PhoneNumber', width: 150 },
                { name: 'CreatedOn', index: 'CreatedOn', width: 150 }
                //{ name: 'IsProcessed', index: 'IsProcessed', width: 100 },
                //{ name: 'ProcessedOn', index: 'ProcessedOn', width: 150 }                
            ],
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblGetResponseMessageDevicePager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Device responses",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by defaults
            hiddengrid: false
        });
        $("#tblGetResponseMessageDevice").jqGrid('navGrid', '#tblGetResponseMessageDevicePager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function adjustScreen() {
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblDeviceName').setGridWidth(outerwidth); // setGridWidth method sets a new width to the grid dynamically
        $('#tblDeviceData').setGridWidth(outerwidth); // setGridWidth method sets a new width to the grid dynamically
        $('#tblDeviceInternalData').setGridWidth(outerwidth); // setGridWidth method sets a new width to the grid dynamically
        $('#tblsendMessageDevice').setGridWidth(outerwidth); // setGridWidth method sets a new width to the grid dynamically
        $('#tblGetResponseMessageDevice').setGridWidth(outerwidth); // setGridWidth method sets a new width to the grid dynamically
    });
    $(window).unbind('resize.myEvents').bind('resize.myEvents', function () {
        var outerwidth = $('#grid').width();
        $('#tblDeviceName').setGridWidth(outerwidth); // setGridWidth method sets a new width to the grid dynamically
        $('#tblDeviceData').setGridWidth(outerwidth);
        $('#tblsendMessageDevice').setGridWidth(outerwidth);
        $('#tblGetResponseMessageDevice').setGridWidth(outerwidth);
    });
}
function loadDeviceDataSendSMS() {
    try {
                
        did = getParameterByName('did');
        if (did != '') {
            var data = 't=' + getTokenCookie('ETCRMTK') + '&did=' + escape(did);
            jsonDeviceDataSMS = dbReadWrite('ETCRMWS.asmx', 'crmGetDeviceDataSendMSM', data, true, false);
            $("#tblsendMessageDevice").jqGrid("clearGridData", true).trigger("reloadGrid");
            $("#tblGetResponseMessageDevice").jqGrid("clearGridData", true).trigger("reloadGrid");
            //var device = eval('(' + jsonDeviceDataSMS.device[0] + ')');
            //document.title = device.name;
            //$('#deviceName').text(device.name);

            //populatetblDeviceNameDataGrid(jsonDeviceDataSMS.device)
            //populateDeviceDataGrid(jsonDeviceDataSMS.dataUser);
            populateDeviceSendSMSDataGrid(jsonDeviceDataSMS.datasendSMS);
            populateDeviceResponseSMSDataGrid(jsonDeviceDataSMS.dataResponseSMS);
        }
    }
    catch (err) {
    }

}
function populateDeviceSendSMSDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = eval('(' + jsonData[i] + ')');
            jQuery("#tblsendMessageDevice").jqGrid('addRowData', i + 1, row);
        }
    }
    catch (err) {
    }
}
function populateDeviceResponseSMSDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = eval('(' + jsonData[i] + ')');
            jQuery("#tblGetResponseMessageDevice").jqGrid('addRowData', i + 1, row);
        }
    }
    catch (err) {
    }
}
function createSensorsGrid(){
    try {
        jQuery("#tblSensors").jqGrid({
            height: 200,
            colNames: ['ID', 'IMEI', 'TempNumber', 'SensorID', 'Name', 'CreatedOn', 'LastUpdatedOn'],
            colModel: [
                { label: 'ID', name: 'ID', key: true, index: 'ID', hidden: true, width: 150, editable: true },
                { label: 'IMEI', name: 'IMEI', index: 'IMEI', width: 200, editable: false, sortable: true },
                { label: 'TempNumber', name: 'TempNumber', index: 'TempNumber', width: 200, editable: true, sortable: true },
                { label: 'SensorID', name: 'SensorID', index: 'SensorID', width: 200, editable: true, sortable: true },
                { label: 'Name', name: 'Name', index: 'Name', width: 200, editable: false, sortable: true },
                { label: 'CreatedOn', name: 'CreatedOn', index: 'CreatedOn', width: 200, editable: false, sortable: true },
                { label: 'LastUpdatedOn', name: 'LastUpdatedOn', index: 'LastUpdatedOn', width: 200, editable: false, sortable: true }
            ],
            //height: 100,
            autowidth: true,
            rowNum: 20,
            loadonce: true,
            rowList: [10, 20, 30],
            pager: "#tblSensorsPager",
            sortname: 'ID',
            viewrecords: true,
            sortorder: 'asc',
            caption: 'Temperature Sensors',
            emtyrecords: 'No Records To Display',
            hiddengrid: false,
            jsonreader: {
                repeatitems: false,
                ID: 0
            },
            multiselect: false,
        }).navGrid("#tblSensorsPager", { edit: true, add: true, refresh: true, del: true, search: true, },
            {//edit options
                with: 600,
                url: urlServicio + '/updatesensor/' + token,
                datatype: 'json',
                ajaxEditOptions: { contentType: "application/json" },
                closeAfterEdit: true,
                onInitializeForm: function () {
                    $('#LastUpdatedOn').datepicker({
                        changeMonth: true,
                        changeYear: true
                    })
                },
                serializeEditData: function (postData) {
                    
                    jsonSensor = {};
                    //var rowid = $('#tblSensors').jqGrid('getGridParam', 'selrow');
                    var imei = $('#tblSensors').jqGrid('getCell', 1, 'IMEI')
                    if (postData.Name == "" || postData.Name.length == 0) {
                        postData.Name = "Sensor" + postData.TempNumber;
                    }
                    //var objeto = {};
                    jsonSensor.ID = postData.ID;
                    jsonSensor.IMEI = imei;
                    jsonSensor.TempNumber = parseInt(postData.TempNumber.trim());
                    jsonSensor.SensorID = postData.SensorID.trim();
                    jsonSensor.Name = postData.Name.trim();
                    jsonSensor.Reassigned = false;
                    var json = JSON.stringify(jsonSensor);
                    return json;
                },
                afterComplete: function (response) {
                    var response1 = JSON.parse(response.responseText);
                    
                    if (response1.isOk == 3) {
                        alert("The position of the sensor is registered");
                    } else if (response1.isOk == 4) {
                        alert("The sensor exists in the device ");
                    } else if (response1.isOk == 2) {
                        alert("The sensor is assigned to another device");
                    }
                    else {
                        $('#tblSensors').setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');
                        jQuery('#tblSensors').jqGrid('clearGridData');
                        jQuery('#tblDeviceName').jqGrid('clearGridData');
                        jQuery('#tblDeviceData').jqGrid('clearGridData');
                        jQuery('#tblDeviceInternalData').jqGrid('clearGridData');
                        loadDeviceData();
                    }                    
                }
            },
            {//add options
                with: 600,
                url: urlServicio + '/savesensor/' + token,
                datatype: 'json',
                ajaxEditOptions: { contentType: "application/json" },
                closeAfterEdit: true,
                onInitializeForm: function () {
                    $('#LastUpdatedOn').datepicker({
                        changeMonth: true,
                        changeYear: true
                    })
                },
                serializeEditData: function (postData) {
                    
                    jsonSensor = {};
                    var imei = $('#tblSensors').jqGrid('getCell', 1, 'IMEI')
                    did = getParameterByName('did');
                    if (postData.Name == "" || postData.Name.length == 0) {
                        postData.Name = "Sensor" + postData.TempNumber;
                    }                    
                    jsonSensor.ID = 0;
                    jsonSensor.IMEI = imei;
                    jsonSensor.TempNumber = parseInt(postData.TempNumber.trim());
                    jsonSensor.SensorID = postData.SensorID.trim();
                    jsonSensor.Name = postData.Name.trim();
                    jsonSensor.Did = did;
                    jsonSensor.Reassigned = false;
                    var json = JSON.stringify(jsonSensor);
                    return json;
                },
                afterComplete: function (response) {
                    
                    var response1 = JSON.parse(response.responseText);
                    if (response1.isOk == 2) {
                        if (confirm("The sensor was assigned to another device.Do you want reassigned to this device ?") == true) {
                            jsonSensor.Reassigned = true;
                            var json = JSON.stringify(jsonSensor);
                            
                            $.ajax(
                                {
                                    url: urlServicio + '/savesensor/' + token,
                                    type: "POST",
                                    data: json,
                                    contentType: "application/json"
                                })
                                .done(function (data) {
                                    
                                    jQuery('#tblSensors').jqGrid('clearGridData');
                                    jQuery('#tblDeviceName').jqGrid('clearGridData');
                                    jQuery('#tblDeviceData').jqGrid('clearGridData');
                                    jQuery('#tblDeviceInternalData').jqGrid('clearGridData');
                                    loadDeviceData();
                                })
                                .fail(function (data) {
                                    alert("error");
                                })
                                

                        } else {
                            alert("You canceled!");
                        }
                    } else if (response1.isOk == 3) {
                        alert("The position of the sensor is registered");
                    }else if (response1.isOk == 4) {
                        alert("The sensor exists in the device ");
                    }
                    else {
                        $('#tblSensors').setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');
                        jQuery('#tblSensors').jqGrid('clearGridData');
                        jQuery('#tblDeviceName').jqGrid('clearGridData');
                        jQuery('#tblDeviceData').jqGrid('clearGridData');
                        jQuery('#tblDeviceInternalData').jqGrid('clearGridData');
                        loadDeviceData();
                    }
                }
            }
            
        );
    
    }
    catch (err) {
    }
}
