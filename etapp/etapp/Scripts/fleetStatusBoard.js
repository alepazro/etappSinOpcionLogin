var lastFetchOn = 0;
var isRefresh = false;
var totalDevices = 0;
var qtyPanels = 1;
var devicesPerPanel = 0;

function applyAccessRights() {
    try {
        $('#fsbHotSpots').hide();

        if (validateUserAccess(52, false)) {
            $('#fsbHotSpots').show();
        }

    }
    catch (err) {
        alert('applyTrackingAccessRights: ' + err.description);
    }
}
function setLastFetchOn() {
    try {
        if (lastFetchOn == 0) {
            lastFetchOn = '1-1-2000';
        }
        else {
            var now = new Date();
            var now_utc = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
            lastFetchOn = now_utc.getFullYear().toString() + '-' + pad((now_utc.getMonth() + 1).toString(), 2) + '-' + pad(now_utc.getDate().toString(), 2) + ' ' + now_utc.getHours().toString() + ':' + pad(now_utc.getMinutes().toString(), 2) + ':' + pad(now_utc.getSeconds().toString(), 2);
        }
    }
    catch (err) {
        alert('setLastFetchOn: ' + err.description);
    }
}

function getBoardDevicesOk(data, textStatus, jqXHR) {
    try {
        
        var ind = 0;
        setLastFetchOn();
        var template = '';
        if (isRefresh == false) {
            totalDevices = data.length;

            for (ind = 1; ind <= qtyPanels; ind++) {
                var dev = _.where(data, { panelId: ind });
                template = _.template($('#device-template').html(), { devices: dev });
                $('#fleetBoard-' + ind).html(template);
                isRefresh = true;
            }
        }
        else {
            if (data[0].isOk) {
                for (ind = 0; ind < data.length; ind++) {
                    var dev = data[ind];
                    var newRow = _.template($('#devRow-template').html(), dev);
                    $('#' + dev.id).replaceWith(newRow);
                }
            }
        }
    }
    catch (err) {
        console.log(err);
        alert('getBoardDevicesOk: ' + err.description);
    }
}

function getBoardDevicesError(jqXHR, textStatus, errorThrown) {
    try {
        var a = 1;
    }
    catch (err) {
        alert('getBoardDevicesError: ' + err.description);
    }
}

function getBoardDevices() {
    try {
        
        var token = getTokenCookie('ETTK');
        var data = 'lastFetchOn=' + escape(lastFetchOn) + '&qtyPanels=' + qtyPanels + '&devicesPerPanel=' + devicesPerPanel;
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/etrack.svc/getDevices/' + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: getBoardDevicesOk,
            error: getBoardDevicesError,
            async: true
        });
    }
    catch (err) {
        alert('getBoardDevices: ' + err.description);
    }
}

function loadFleetBoard() {
    try {
        getBoardDevices();

    }
    catch (err) {
        alert('loadFleetBoard: ' + err.description);
    }
}

function refreshFleetStatusBoard() {
    try {
        getBoardDevices();
    }
    catch (err) {
        alert('refreshFleetStatusBoard: ' + err.description);
    }
}

function setPanels() {
    try {
        qtyPanels = 1;
        if (totalDevices == 0) {
            getBoardDevices();
        }
        devicesPerPanel = $('#devPerPanel').val();
        if (!isInteger(devicesPerPanel)) {
            devicesPerPanel = 0;
        }
        if (devicesPerPanel > 0) {
            qtyPanels = Math.ceil(totalDevices / devicesPerPanel);
            if (qtyPanels <= 0) {
                qtyPanels = 1;
            }
        }

        $('#boardPanels').empty().append('<ul id="tabsUl"></ul>');
        var ind = 0;
        for (ind = 1; ind <= qtyPanels; ind++) {
            $('#tabsUl').append('<li><a href="#tabs-' + ind + '">Panel ' + ind + '</a></li>');
        }
        for (ind = 1; ind <= qtyPanels; ind++) {
            $('#boardPanels').append('<div id="tabs-' + ind + '"><table id="fleetBoard-' + ind + '" class="table table-striped table-bordered table-hover table-condensed"></table></div>');
        }
        $("#boardPanels").tabs("destroy").tabs({ selected: 0 });
        isRefresh = false;
        lastFetchOn = 0;
        setLastFetchOn();
        getBoardDevices();
    }
    catch (err) {
        alert('setPanels: ' + err.description);
    }
}