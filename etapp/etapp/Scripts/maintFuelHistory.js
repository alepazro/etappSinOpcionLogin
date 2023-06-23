var isMaintFuelHistoryReady = false;

function loadFuelLogHistory() {
    try {
        if (isMaintFuelHistoryReady == false) {
            if (jsonDevices == false) {
                getDevices();
            }
            if (jsonDevices) {
                var cbx = document.getElementById('cbxHFuelDevices');
                removeAllChildNodes(cbx);
                loadComboBox(jsonDevices.myDevices, cbx, 'All Devices');
            }

            $('#dtpHFuelFrom').val(getNow());
            $('#dtpHFuelTo').val(getNow());

            $("#dtpHFuelFrom").datepicker();
            $("#dtpHFuelTo").datepicker();

            isMaintFuelHistoryReady = true;
        }
    }
    catch (err) {
        alert('loadFuelLogHistory: ' + err.description);
    }
}

function delMaintHFuel(e) {
    try {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var id = dataItem.id;

        $('#fuelHistoryRemoveDlg').attr('data-id', id);
        $('#fuelHistoryRemoveDlg').dialog('open');;

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function loadMaintHFuelGrid(data) {
    try {
        var ds = new kendo.data.DataSource({
            data: data
        });

        $("#fuelLogHistoryGrid").kendoGrid({
            toolbar: ["excel"],
            excel: {
                fileName: "FuelingHistory.xlsx"
            },
            dataSource: ds,
            columns: [
                { field: "id", title: 'id', hidden: true },
                { field: "deviceName", title: 'Device' },
                { field: "fuelDate", title: 'Fueling Date' },
                { field: "odometer", title: 'Odometer' },
                { field: 'gallons', title: 'Gallons' },
                { field: "cost", title: 'Cost' },
                { field: "stateName", title: 'State' },
                { field: 'comments', title: 'Comments' },
                { command: { text: "Delete", click: delMaintHFuel }, title: " ", width: "120px" }
            ]
        });

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function getHFuelData() {
    try {
        var deviceId = $('#cbxHFuelDevices').val();
        var dateFrom = $('#dtpHFuelFrom').val();
        var dateTo = $('#dtpHFuelTo').val();

        var token = getTokenCookie('ETTK');
        var noCache = Math.random();
        var data = 'deviceId=' + deviceId + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo + '&noCache=' + noCache;
        url = 'etrest.svc/getMaintHFuel/'
        $.ajax({
            type: "GET",
            url: url + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                loadMaintHFuelGrid(data);
            },
            error: function (x, y, z) {
                var a = 1;
            },
            async: true
        });
    }
    catch (err) {
        alert('getHFuelData: ' + err.description);
    }
}

function deleteFuelHistoryCommit() {
    try {
        var id = $('#fuelHistoryRemoveDlg').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('deleteMaintFuelLog', data, true, false);

        getHFuelData();

        return true;
    }
    catch (err) {
        alert('deleteFuelHistoryCommit: ' + err.description);
    }
}

function setupFuelHistoryRemoveDlg() {
    try {
        $("#fuelHistoryRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteFuelHistoryCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing fuel log.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
            }
        });
    }
    catch (err) {
        alert('setupFuelHistoryRemoveDlg: ' + err.description);
    }
}
