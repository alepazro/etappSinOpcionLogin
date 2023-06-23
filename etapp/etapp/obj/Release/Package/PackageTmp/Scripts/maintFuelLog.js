var isMaintFuelLogReady = false;

function clearAllFuelLog() {
    try {
        cancelFuelLog();
        $("#maintFuelLogTbl").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearAllFuelLog: ' + err.description);
    }
}

function loadMaintFuelLog() {
    try {
        if (isMaintFuelLogReady == false) {
            if (jsonDevices == false) {
                getDevices();
            }
            if (jsonDevices) {
                var cbx = document.getElementById('cbxRegFuelDevices');
                removeAllChildNodes(cbx);
                loadComboBox(jsonDevices.myDevices, cbx, 'Pick a device');
            }
            if (jsonStates == false) {
                getStates();
            }
            if (jsonStates) {
                var cbx = document.getElementById('cbxRegFuelStates');
                removeAllChildNodes(cbx);
                loadComboBox(jsonStates.states, cbx, 'Pick a state');
            }

            $('#dtpRegFuelDate').val('');
            //https://plugins.jquery.com/datetimepicker/
            $('#dtpRegFuelDate').datetimepicker({ step: 5 });

            //$("#dtpRegFuelDate").datepicker();

            setForNewFuelLog();

            isMaintFuelLogReady = true;
        }
    }
    catch (err) {
        alert('loadMaintFuelLog: ' + err.description);
    }
}

function setForNewFuelLog() {
    try {
        $('#regFuelLogTbl').attr('data-id', '0');
        //Mode: 1 - New ; Mode: 2 - Edit
        $('#regFuelLogTbl').attr('data-mode', 1);
        $('#dtpRegFuelDate').val('');
        $('#txtRegFuelOdometer').val('');
        $('#txtRegFuelGallons').val('');
        $('#txtRegFuelCost').val('');
        $('#txtRegFuelComments').val('');
    }
    catch (err) {
        alert('setForNewFuelLog: ' + err.description);
    }
}

function clearAllFuelLogForm() {
    try {
        setForNewFuelLog();
        $('#cbxRegFuelStates').val('0');
    }
    catch (err) {
        alert('clearAllFuelLogForm: ' + err.description);
    }
}

function editMaintFuelLog(obj) {
    try {
        var id = $(obj.target).attr('data-id');
        $('#regFuelLogTbl').attr('data-id', id);
        //Mode: 1 - New ; Mode: 2 - Edit
        $('#regFuelLogTbl').attr('data-mode', 2);
        $('#cbxRegFuelDevices').val($('#fuelLogTR' + id).attr('data-deviceId'));
        $('#dtpRegFuelDate').val($('#fuelLogTR' + id).attr('data-fuelingDateString'));
        $('#txtRegFuelOdometer').val($('#fuelLogTR' + id).attr('data-odometer'));
        $('#txtRegFuelGallons').val($('#fuelLogTR' + id).attr('data-gallons'));
        $('#txtRegFuelCost').val($('#fuelLogTR' + id).attr('data-cost'));
        $('#cbxRegFuelStates').val($('#fuelLogTR' + id).attr('data-stateId'));
        $('#txtRegFuelComments').val($('#fuelLogTR' + id).attr('data-comments'));
    }
    catch (err) {
        alert('editMaintFuelLog: ' + err.description);
    }
}

function deleteFuelLogCommit() {
    try {
        var id = $('#fuelLogRemoveDlg').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('deleteMaintFuelLog', data, true, false);

        var tbl = document.getElementById('maintFuelLogTbl');
        var tr = document.getElementById('fuelLogTR' + id);
        tbl.removeChild(tr);

        //Special case: the deleted record could also be in the form being edited... check for this.
        if ($('#regFuelLogTbl').attr('data-id')) {
            var id2 = $('#regFuelLogTbl').attr('data-id');
            if (id == id2) {
                $('#regFuelLogTbl').attr('data-id', '0');
                $('#regFuelLogTbl').attr('data-mode', '1');
            }
        }

        return true;
    }
    catch (err) {
        alert('deleteFuelLogCommit: ' + err.description);
    }
}

function deleteMaintFuelLog(obj) {
    try {
        var id = $(obj.target).attr('data-id');
        $('#fuelLogRemoveDlg').attr('data-id', id);
        $('#fuelLogRemoveDlg').dialog('open'); ;
    }
    catch (err) {
        alert('deleteMaintFuelLog: ' + err.description);
    }
}

function cancelFuelLog() {
    try {
        $('#cbxRegFuelDevices').val('0');
        clearAllServiceLogForm();
    }
    catch (err) {
        alert('cancelFuelLog: ' + err.description);
    }
}

function fillFuelLogRecord(tr, item) {
    try {
        var tbl = document.getElementById('maintFuelLogTbl');

        $(tr).attr('id', 'fuelLogTR' + item.id);
        $(tr).attr('data-id', item.id);
        $(tr).attr('data-deviceId', item.deviceId);
        $(tr).attr('data-deviceName', item.deviceName);
        $(tr).attr('data-fuelingDate', item.fuelingDate);
        $(tr).attr('data-fuelingDateString', item.fuelingDateString);
        $(tr).attr('data-odometer', item.odometer);
        $(tr).attr('data-gallons', item.gallons);
        $(tr).attr('data-cost', item.cost);
        $(tr).attr('data-stateId', item.stateId);
        $(tr).attr('data-stateName', item.stateName);
        $(tr).attr('data-comments', item.comments);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('maintListOddTR');
        }

        //Device
        var deviceNameTd = document.createElement('td');
        $(deviceNameTd).html(item.deviceName);
        $(deviceNameTd).addClass('maintListTD');
        tr.appendChild(deviceNameTd);

        //Fuel Date
        var fuelDateTd = document.createElement('td');
        $(fuelDateTd).html(item.fuelingDateString);
        $(fuelDateTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(fuelDateTd);

        //Odometer
        var odometerTd = document.createElement('td');
        $(odometerTd).html(item.odometer);
        $(odometerTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(odometerTd);

        //Gallons
        var gallonsTd = document.createElement('td');
        $(gallonsTd).html(item.gallons);
        $(gallonsTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(gallonsTd);

        //Cost
        var costTd = document.createElement('td');
        $(costTd).html('$' + item.cost);
        $(costTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(costTd);

        //State
        var stateTd = document.createElement('td');
        $(stateTd).html(item.stateName);
        $(stateTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(stateTd);

        //Edit
        var editTd = document.createElement('td');
        $(editTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(editTd);

        var editBtn = document.createElement('button');
        editTd.appendChild(editBtn);
        $(editBtn).attr('data-id', item.id);
        $(editBtn).click(editMaintFuelLog);

        var editImg = document.createElement('img');
        $(editImg).attr('src', 'icons/edit_inline.png');
        $(editImg).attr('alt', '');
        $(editImg).attr('width', '16');
        $(editImg).attr('height', '16');
        $(editImg).attr('data-id', item.id);
        editBtn.appendChild(editImg);

        //Delete
        var delTd = document.createElement('td');
        $(delTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(delTd);

        var delBtn = document.createElement('button');
        delTd.appendChild(delBtn);
        $(delBtn).attr('data-id', item.id);
        $(delBtn).click(deleteMaintFuelLog);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);

    }
    catch (err) {
        alert('fillFuelLogRecord: ' + err.description);
    }
}

function addFuelLogToList(itm) {
    try {
        var tbl = document.getElementById('maintFuelLogTbl');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillFuelLogRecord(tr, itm);
    }
    catch (err) {
        alert('addFuelLogToList: ' + err.description);
    }
}

function modifyFuelLogListRecord(id, itm) {
    try {
        var tr = document.getElementById('fuelLogTR' + id);
        removeAllChildNodes(tr);
        fillFuelLogRecord(tr, itm);
    }
    catch (err) {
        alert('modifyFuelLogListRecord: ' + err.description);
    }
}

function saveFuelLog() {
    try {
        var id = $('#regFuelLogTbl').attr('data-id');
        var mode = $('#regFuelLogTbl').attr('data-mode');

        var deviceId = $('#cbxRegFuelDevices').val();
        if (deviceId == '0') {
            alert('Please select a device');
            return false;
        }
        deviceName = $('#cbxRegFuelDevices option:selected').text();

        var fuelingDate = $('#dtpRegFuelDate').val();
        var odometer = $('#txtRegFuelOdometer').val();
        if (odometer == '') {
            odometer = 0;
        }
        var gallons = $('#txtRegFuelGallons').val();
        if (gallons == '') {
            gallons = 0;
        }
        var cost = $('#txtRegFuelCost').val();
        if (cost == '') {
            cost = 0;
        }

        var stateId = $('#cbxRegFuelStates').val();
        if (stateId == '0') {
            alert('Please select a state');
            return false;
        }
        stateName = $('#cbxRegFuelStates option:selected').text();

        var comments = $('#txtRegFuelComments').val();

        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&deviceId=' + escape(deviceId) + '&fuelingDate=' + escape(fuelingDate) + '&odometer=' + escape(odometer) + '&gallons=' + escape(gallons) + '&cost=' + escape(cost) + '&stateId=' + escape(stateId) + '&comments=' + escape(comments);
        var tmpJson = dbReadWrite('saveMaintFuelLog', data, true, false);

        if (tmpJson.result != 'failure') {
            var itm = { 'id': tmpJson.result, 'deviceId': deviceId, 'deviceName': deviceName, 'fuelingDate': fuelingDate, 'fuelingDateString': fuelingDate, 'odometer': odometer, 'gallons': gallons, 'stateId': stateId, 'stateName': stateName, 'cost': cost, 'comments': comments };
            if (id == '0' || id == '') {
                addFuelLogToList(itm);
            }
            else {
                modifyFuelLogListRecord(id, itm);
            }

            //Finally, set for new entry...
            setForNewFuelLog();
        }
        else {
            alert('Failed saving record.  Please try again.  If the problem persists, please contact Support.');
        }
    }
    catch (err) {
        alert('saveFuelLog: ' + err.description);
    }
}

function setupFuelLogRemoveDlg() {
    try {
        $("#fuelLogRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteFuelLogCommit() == true) {
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
        alert('setupFuelLogRemoveDlg: ' + err.description);
    }
}