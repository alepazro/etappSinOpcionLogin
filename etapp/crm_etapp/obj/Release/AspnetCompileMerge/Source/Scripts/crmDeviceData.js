var jsonDeviceData = false;
var did = false;

function loadDeviceData() {
    try {
        createDeviceNameDataGrid();
        createDeviceDataGrid();
        createDeviceInternalDataGrid();
        did = getParameterByName('did');
        if (did != '') {
            var data = 't=' + getTokenCookie('ETCRMTK') + '&did=' + escape(did);
            jsonDeviceData = dbReadWrite('ETCRMWS.asmx', 'crmGetDeviceData', data, true, false);

            var device = eval('(' + jsonDeviceData.device[0] + ')');
            document.title = device.name;
            $('#deviceName').text(device.name);
            debugger;

            populatetblDeviceNameDataGrid(jsonDeviceData.device)
            populateDeviceDataGrid(jsonDeviceData.dataUser);
            populateDeviceInternalDataGrid(jsonDeviceData.dataInternal);
        }
    }
    catch (err) {
    }
}

function populatetblDeviceNameDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = eval('(' + jsonData[i] + ')');
            jQuery("#tblDeviceName").jqGrid('addRowData', i + 1, row);
            /*  //*/
        }
    }
    catch (err) {
    }
}

function populateDeviceDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = eval('(' + jsonData[i] + ')');
            jQuery("#tblDeviceData").jqGrid('addRowData', i + 1, row);
      /*  //*/}
    }
    catch (err) {
    }
}

function populateDeviceInternalDataGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var row = eval('(' + jsonData[i] + ')');
            jQuery("#tblDeviceInternalData").jqGrid('addRowData', i + 1, row);
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
            colNames: ['Name', 'ReportIgnON', 'ReportTimerIgnOff', 'ReportTurnAngle', 'ReportDistance', 'FakeIgn', 'SimNo'],
            colModel: [
                { name: 'name', index: 'name', width: 150 },
                { name: 'ReportIgnON', index: 'ReportIgnON', width: 120 },
                { name: 'ReportTimerIgnOff', eventCode: 'ReportTimerIgnOff', width: 120 },
                { name: 'ReportTurnAngle', index: 'ReportTurnAngle', width: 120 },
                { name: 'ReportDistance', index: 'ReportDistance', width: 150 },
                { name: 'FakeIgn', index: 'FakeIgn', width: 150 },
                { name: 'SimNo', index: 'SimNo', width: 120}
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

            colNames: ['ID', 'Ign.Stat', 'Ev.Code', 'Ev.Name', 'Ev.Date', 'Created', 'Delay', 'Speed', 'GPSAge', 'Consec', 'IsBrief', 'OriginalEvent', 'Lat', 'Lng', 'RSSI', 'Ble', 'BI','BE'],
            colModel: [
           		                    { name: 'id', index: 'id', width: 50 },
           		                    { name: 'ignitionStatus', index: 'ignitionStatus', width: 50 },
           		                    { name: 'eventCode', eventCode: 'id', width: 50 },
           		                    { name: 'eventName', index: 'eventName', width: 100 },
           		                    { name: 'eventDate', index: 'eventDate', width: 150 },
           		                    { name: 'createdOn', index: 'createdOn', width: 150 },
           		                    { name: 'msgDelay', index: 'msgDelay', width: 50 },
           		                    { name: 'speed', index: 'speed', width: 50 },
           		                    { name: 'gpsAge', index: 'gpsAge', width: 50 },
           		                    { name: 'consecutive', index: 'consecutive', width: 100 },
           		                    { name: 'isBrief', index: 'isBrief', width: 50 },
           		                    { name: 'originalEvent', index: 'originalEvent', width: 50 },
           		                    { name: 'lat', index: 'lat', width: 100 },
           		                    { name: 'lng', index: 'lng', width: 100 },
                                    { name: 'RSSI', index: 'RSSI', width: 50 },
                                    { name: 'Ble', index: 'Ble', width: 50 },
                                    { name: 'BI', index: 'BI', width: 50 },
                                    { name: 'BE', index: 'BE', width: 50 }
           	                    ],           
            rowNum: 100,
            rowList: [10, 20, 30],
            pager: '#tblDeviceDataPager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Device Data",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
        });
        $("#tblDeviceData").jqGrid('navGrid', '#tblDeviceDataPager', { edit: false, add: false, del: false });
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
            shrinkToFit: false,// well, it's 'true' by default
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

    }
    catch (err) {

    }
}

function getResponses() {
    try{

    }
    catch (err) {

    }
}
