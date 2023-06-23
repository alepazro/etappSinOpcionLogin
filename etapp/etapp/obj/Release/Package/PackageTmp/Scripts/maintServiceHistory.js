var isMaintServiceHistoryReady = false;

function loadMaintenanceHistory() {
    try {
        if (isMaintServiceHistoryReady == false) {
            if (jsonDevices == false) {
                getDevices();
            }
            if (jsonDevices) {
                var cbx = document.getElementById('cbxHServicesDevices');
                removeAllChildNodes(cbx);
                loadComboBox(jsonDevices.myDevices, cbx, 'All Devices');
            }

            if (jsonTasks == false) {
                getTasks();
            }
            if (jsonTasks) {
                var cbx = document.getElementById('cbxHServicesTasks');
                removeAllChildNodes(cbx);
                loadComboBox(jsonTasks.tasks, cbx, 'All Tasks');
            }

            $('#dtpHServicesFrom').val(getNow());
            $('#dtpHServicesTo').val(getNow());

            $("#dtpHServicesFrom").datepicker();
            $("#dtpHServicesTo").datepicker();

            isMaintServiceHistoryReady = true;
        }
    }
    catch (err) {
        alert('loadMaintenanceHistory: ' + err.description);
    }
}

function delMaintHistory(e) {
    try {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var id = dataItem.id;

        $('#serviceHistoryRemoveDlg').attr('data-id', id);
        $('#serviceHistoryRemoveDlg').dialog('open');;

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function loadMaintHistoryGrid(data) {
    try{
        var ds = new kendo.data.DataSource({
            data: data
        });

        $("#maintenanceHistoryGrid").kendoGrid({
            toolbar: ["excel"],
            excel: {
                fileName: "ServiceHistory.xlsx"
            },
            dataSource: ds,
            columns: [
                { field: "id", title: 'id', hidden: true },
                { field: "deviceName", title: 'Device' },
                { field: "taskName", title: 'Task' },
                { field: "serviceDate", title: 'Service Date' },
                { field: 'odometer', title: 'Odometer' },
                { field: "cost", title: 'Cost' },
                { field: 'comments', title: 'Comments' },
                { command: { text: "Delete", click: delMaintHistory }, title: " ", width: "120px" }
            ]
        });

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function getHServicesData() {
    try {
        var deviceId = $('#cbxHServicesDevices').val();
        var taskId = $('#cbxHServicesTasks').val();
        var dateFrom = $('#dtpHServicesFrom').val();
        var dateTo = $('#dtpHServicesTo').val();

        var token = getTokenCookie('ETTK');
        var noCache = Math.random();
        var data = 'deviceId=' + deviceId + '&taskId=' + taskId + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo + '&noCache=' + noCache;
        url = 'etrest.svc/getMaintHServices/'
        $.ajax({
            type: "GET",
            url: url + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                loadMaintHistoryGrid(data);
            },
            error: function (x, y, z) {
                var a = 1;
            },
            async: true
        });
    }
    catch (err) {
        alert('getHServicesData: ' + err.description);
    }
}

function deleteServiceHistoryCommit() {
    try {
        var id = $('#serviceHistoryRemoveDlg').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('deleteMaintServiceLog', data, true, false);

        getHServicesData();

        return true;
    }
    catch (err) {
        alert('deleteMaintHistoryCommit: ' + err.description);
    }
}

function setupServiceHistoryRemoveDlg() {
    try {
        $("#serviceHistoryRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteServiceHistoryCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing service log.  Please try again.');
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
        alert('setupServiceHistoryRemoveDlg: ' + err.description);
    }
}

