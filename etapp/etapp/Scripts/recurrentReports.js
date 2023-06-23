// This script has a dependency with devicesSettings.js - See newRecurrentReport.
var isRecRepDlgReady = false;
var jsonRecurrentReports = false;
var jsonReports = false;
var jsonFrequencies = false;

function getReports() {
    try {
        var data = 't=' + getTokenCookie('ETTK') + '&isRecurrent=1';
        jsonReports = dbReadWrite('getReports', data, true, false);

        return true;
    }
    catch (err) {
        alert('getReports: ' + err.description);
    }
}

function changeRecRepReports(obj, paramValue) {
    try {
        var selectedIndex = obj.selectedIndex;
        var id = obj[selectedIndex].getAttribute('data-id');
        var hasParam = obj[selectedIndex].getAttribute('data-hasParam');
        var paramName = obj[selectedIndex].getAttribute('data-paramName');

        hideRecRepParams();

        if (hasParam == 'true') {
            switch (paramName.toLowerCase()) {
                case 'eventcode':
                    $('#paramEventCode').show();
                    break;
                case 'speedlimit':
                    $('#paramSpeedLimit').show();
                    $('#pSpeedLimit').text(paramValue);
                    break;
                case 'mintime':
                    $('#paramMinTime').show();
                    $('#pMinTime').val(paramValue);
                    break;
                case 'idlelimit':
                    $('#paramIdleLimit').show();
                    $('#pIdleLimit').text(paramValue);
                    break;
                case 'driverid':
                    if (jsonDrivers == false) {
                        getDrivers();
                    }
                    if ($('#pDrivers').length <= 1) {
                        loadComboBox(jsonDrivers.drivers, document.getElementById('pDrivers'), 'Pick a Driver');
                    }
                    $('#paramDriverID').show();
                    break;
                case 'geofenceid':
                    if (jsonGeofencesList == false) {
                        getGeofences_AllList();
                    }
                    if ($('#pGeofences').length <= 1) {
                        loadComboBox(jsonGeofencesList.geofences, document.getElementById('pGeofences'), 'All Geofences');
                    }
                    $('#paramGeofenceID').show();
                    break;
                case 'retrogeo':
                    $('#paramRetroGeo').show();
                    $('#paramDeviceID').hide();
                    break;
            }
        }

    }
    catch (err) {
        alert('changeRecRepReports: ' + err.description);
    }
}

function hideRecRepParams() {
    try {
        $('#paramEventCode').hide();
        $('#paramSpeedLimit').hide();
        $('#paramIdleLimit').hide();
        $('#paramMinTime').hide();
        $('#paramDriverID').hide();
        $('#paramGeofenceID').hide();
    }
    catch (err) {
        alert('hideRecRepParams: ' + err.description);
    }
}

function getRecRepFrequencies() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonFrequencies = dbReadWrite('getRecRepFrequencies', data, true, false);

        return true;
    }
    catch (err) {
        alert('getRecRepFrequencies: ' + err.description);
    }
}

function loadRecRepDevices(mode, jsonAllDevices, jsonRecRepDevices) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllDevices) {
            var ul = document.getElementById('newRecRepDevicesList');
            removeAllChildNodes(ul);

            for (var ind = 0; ind < jsonAllDevices.myDevices.length; ind++) {
                jsonDevice = eval('(' + jsonAllDevices.myDevices[ind] + ')');
                //Add the <li>
                var li = document.createElement('li');
                $(li).attr('id', 'newRecRepDeviceId' + jsonDevice.deviceId);
                ul.appendChild(li);

                var chk = document.createElement('input');
                $(chk).attr('type', 'checkbox');
                $(chk).attr('id', 'chkNewRecRepDevice' + jsonDevice.deviceId);
                $(chk).attr('data-deviceId', jsonDevice.deviceId);
                $(chk).attr('style', 'padding:3px;');

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonRecRepDevices.myDevices.length; ind2++) {
                        var jsonDevice2 = eval('(' + jsonRecRepDevices.myDevices[ind2] + ')');
                        if (jsonDevice.deviceId == jsonDevice2.deviceId) {
                            $(chk).prop('checked', 'true');
                            break;
                        }
                    }
                }

                li.appendChild(chk);

                var span = document.createElement('span');
                li.appendChild(span);
                $(span).text(jsonDevice.name);
            }
        }
    }
    catch (err) {
        alert('loadRecRepDevices: ' + err.description);
    }
}

function loadRecRepUsers(mode, jsonAllUsers, jsonRecRepUsers) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllUsers) {
            var ul = document.getElementById('newRecRepUsersList');
            removeAllChildNodes(ul);

            for (var ind = 0; ind < jsonAllUsers.users.length; ind++) {
                var jsonItem = eval('(' + jsonAllUsers.users[ind] + ')');
                //Add the <li>
                var li = document.createElement('li');
                $(li).attr('id', 'newRecRepUserId' + jsonItem.id);
                ul.appendChild(li);

                var chk = document.createElement('input');
                $(chk).attr('type', 'checkbox');
                $(chk).attr('id', 'chkNewRecRepUser' + jsonItem.id);
                $(chk).attr('data-userId', jsonItem.id);
                $(chk).attr('style', 'padding:3px;');

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonRecRepUsers.users.length; ind2++) {
                        var jsonItem2 = eval('(' + jsonRecRepUsers.users[ind2] + ')');
                        if (jsonItem.id == jsonItem2.id) {
                            $(chk).prop('checked', 'true');
                            break;
                        }
                    }
                }

                li.appendChild(chk);

                var span = document.createElement('span');
                li.appendChild(span);
                $(span).text(jsonItem.firstName + ' ' + jsonItem.lastName);
            }
        }

    }
    catch (err) {
        alert('loadRecRepUsers: ' + err.description);
    }
}

function loadRecRepDlg(mode, id, jsonAllDevices, jsonAllUsers, jsonRecRepDevices, jsonRecRepUsers) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        $('#recRepId').attr('data-id', id);

        if (mode == 2) {
            $('#recRepId').attr('data-hasParam', $('#recRepTR' + id).attr('data-hasParam'));
            $('#recRepId').attr('data-paramName', $('#recRepTR' + id).attr('data-paramName'));
            $('#recRepId').attr('data-paramValue', $('#recRepTR' + id).attr('data-paramValue'));
        }

        $('#cbxRecRepReports').val($('#recRepTR' + id).attr('data-reportId'));
        $('#cbxRecRepFrequencies').val($('#recRepTR' + id).attr('data-frequencyId'));
        $('#chkRecRepWeekends').prop('checked', $('#recRepTR' + id).attr('data-excludeWeekends'));

        //Load the device
        loadRecRepDevices(mode, jsonAllDevices, jsonRecRepDevices);

        //Load the users
        loadRecRepUsers(mode, jsonAllUsers, jsonRecRepUsers);

        hideRecRepParams();
        if (mode == 2) {
            if ($('#recRepId').attr('data-hasParam').toLowerCase() == 'true') {
                changeRecRepReports(document.getElementById('cbxRecRepReports'), $('#recRepTR' + id).attr('data-paramValue'))
            }
        }

        $("#newRecRepDlg").dialog('open');
    }
    catch (err) {
        alert('loadRecRepDlg: ' + err.description);
    }
}

function newRecurrentReport() {
    try {
        if (isRecRepDlgReady == false) {
            setupNewRecRepDlg();
            setupRecRepRemoveDlg();
            isRecRepDlgReady = true;
        }
        //The following creates a dependency with devicesSettings.js
        if (jsonDevices == false) {
            getDevices();
        }

        //The following creates a dependency with users.js
        if (jsonUsers == false) {
            getUsers();
        }

        loadRecRepDlg(1, '0', jsonDevices, jsonUsers);
    }
    catch (err) {
        alert('newRecurrentReport: ' + err.description);
    }
}

function clearRecRepList() {
    try {
        $("#recurrentReportsTable").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearRecRepList: ' + err.description);
    }
}

function getRecurrentReports() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonRecurrentReports = dbReadWrite('getRecurrentReports', data, true, false);

        return true;
    }
    catch (err) {
        alert('getRecurrentReports: ' + err.description);
    }
}

function editRecRep(obj) {
    try {
        if (isRecRepDlgReady == false) {
            setupNewRecRepDlg();
            setupRecRepRemoveDlg();
            isRecRepDlgReady = true;
        }
        //The following creates a dependency with devicesSettings.js
        if (jsonDevices == false) {
            getDevices();
        }

        //The following creates a dependency with users.js
        if (jsonUsers == false) {
            getUsers();
        }

        var recRepId = $(obj.target).attr('data-id');

        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(recRepId);
        var jsonRecRepDevices = dbReadWrite('getRecRepDevices', data, true, false);
        var jsonRecRepUsers = dbReadWrite('getRecRepUsers', data, true, false);

        loadRecRepDlg(2, recRepId, jsonDevices, jsonUsers, jsonRecRepDevices, jsonRecRepUsers);
    }
    catch (err) {
        alert('editRecRep: ' + err.description);
    }

}

function fillRecRepRecord(tr, item) {
    try {
        var tbl = document.getElementById('recurrentReportsTable');

        $(tr).attr('id', 'recRepTR' + item.id);
        $(tr).attr('data-reportId', item.reportId);
        $(tr).attr('data-reportName', item.reportName);
        $(tr).attr('data-frequencyId', item.frequencyId);
        $(tr).attr('data-frequencyName', item.frequencyName);
        $(tr).attr('data-excludeWeekends', item.excludeWeekends);
        $(tr).attr('data-isAllDevices', item.isAllDevices);
        $(tr).attr('data-hasParam', item.hasParam);
        $(tr).attr('data-paramName', item.paramName);
        $(tr).attr('data-paramValue', item.paramValue);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('alertsListOddTR');
        }

        //report name
        var reportNameTd = document.createElement('td');
        $(reportNameTd).html(item.reportName);
        $(reportNameTd).addClass('alertsListTD');
        tr.appendChild(reportNameTd);

        //frequency name
        var frequencyNameTd = document.createElement('td');
        $(frequencyNameTd).html(item.frequencyName);
        $(frequencyNameTd).addClass('alertsListTD');
        tr.appendChild(frequencyNameTd);

        //include Weekends?
        var includeWeekendsTd = document.createElement('td');
        if (item.excludeWeekends == true) {
            $(includeWeekendsTd).html('No');
        }
        else {
            $(includeWeekendsTd).html('Yes');
        }
        $(includeWeekendsTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(includeWeekendsTd);

        //IsAllDevices?
        var isAllDevicesTd = document.createElement('td');
        if (item.isAllDevices == true) {
            $(isAllDevicesTd).html('Yes');
        }
        else {
            $(isAllDevicesTd).html('No');
        }
        $(isAllDevicesTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(isAllDevicesTd);

        //Created on
        var createdOnTd = document.createElement('td');
        $(createdOnTd).html(item.createdOn);
        $(createdOnTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(createdOnTd);

        //Edit
        var editTd = document.createElement('td');
        $(editTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(editTd);

        var editBtn = document.createElement('button');
        editTd.appendChild(editBtn);
        $(editBtn).attr('data-id', item.id);
        $(editBtn).click(editRecRep);

        var editImg = document.createElement('img');
        $(editImg).attr('src', 'icons/edit_inline.png');
        $(editImg).attr('alt', '');
        $(editImg).attr('width', '16');
        $(editImg).attr('height', '16');
        $(editImg).attr('data-id', item.id);
        editBtn.appendChild(editImg);

        //Delete
        var delTd = document.createElement('td');
        $(delTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(delTd);

        var delBtn = document.createElement('button');
        delTd.appendChild(delBtn);
        $(delBtn).attr('data-id', item.id);
        $(delBtn).click(deleteRecRep);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);

    }
    catch (err) {
        alert('fillRecRepRecord: ' + err.description);
    }
}

function modifyRecRepListRecord(id, itm) {
    try {
        var tr = document.getElementById('recRepTR' + id);
        removeAllChildNodes(tr);
        fillRecRepRecord(tr, itm);
    }
    catch (err) {
        alert('modifyRecRepListRecord: ' + err.description);
    }
}

function addRecRepToList(item) {
    try {
        var tbl = document.getElementById('recurrentReportsTable');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillRecRepRecord(tr, item);
    }
    catch (err) {
        alert('addRecRepToList: ' + err.description);
    }
}

function loadRecurrentReports() {
    try {
        //Always get a fresh set of recurrent reports
        getRecurrentReports();

        if (jsonRecurrentReports) {
            clearRecRepList();
            if (jsonRecurrentReports) {
                for (var ind = 0; ind < jsonRecurrentReports.reports.length; ind++) {
                    var jsonItem = eval('(' + jsonRecurrentReports.reports[ind] + ')');
                    addRecRepToList(jsonItem);
                }
            }
        }
    }
    catch (err) {
        alert('loadRecurrentReports: ' + err.description);
    }
}

function loadReports() {
    try {
        if (jsonReports == false) {
            getReports();
        }
        if (jsonReports) {
            var cbx = document.getElementById('cbxRecRepReports');
            removeAllChildNodes(cbx);

            var opt0 = document.createElement('option');
            $(opt0).attr('data-id', 0);
            cbx.appendChild(opt0);
            var opt0TXT = document.createTextNode('[Select a Report]');
            opt0.appendChild(opt0TXT);

            for (var ind = 0; ind < jsonReports.reports.length; ind++) {
                var jsonItm = '';
                try {
                    jsonItm = eval('(' + jsonReports.reports[ind] + ')');
                }
                catch (err) {
                    jsonItm = jsonReports.reports[ind];
                }
                var cbxOption = document.createElement('option');
                cbx.appendChild(cbxOption);
                $(cbxOption).attr('value', jsonItm.id);
                $(cbxOption).attr('data-id', jsonItm.id);
                $(cbxOption).attr('data-hasParam', jsonItm.hasParam);
                $(cbxOption).attr('data-paramName', jsonItm.paramName);
                var cbxOptionTxt = document.createTextNode(jsonItm.name);
                cbxOption.appendChild(cbxOptionTxt);
            }
        }
    }
    catch (err) {
        alert('loadReports: ' + err.description);
    }
}

function loadFrequencies() {
    try {
        if (jsonFrequencies == false) {
            getRecRepFrequencies();
        }
        if (jsonFrequencies) {
            var cbx = document.getElementById('cbxRecRepFrequencies');
            removeAllChildNodes(cbx);
            loadComboBox(jsonFrequencies.frequencies, cbx, 'Pick a frequency');
        }
    }
    catch (err) {
        alert('loadFrequencies: ' + err.description);
    }
}

function saveRecRep() {
    try {
        var id = $('#recRepId').attr('data-id');

        var reportId = $('#cbxRecRepReports').val();
        var reportName = $('#cbxRecRepReports option:selected').text();

        var selectedIndex = document.getElementById('cbxRecRepReports').selectedIndex;
        //var reportId = document.getElementById('cbxRecRepReports')[selectedIndex].getAttribute('data-id');
        var hasParam = document.getElementById('cbxRecRepReports')[selectedIndex].getAttribute('data-hasParam');
        var paramName = document.getElementById('cbxRecRepReports')[selectedIndex].getAttribute('data-paramName');
        var param = '';

        if (hasParam == 'true') {
            switch (paramName.toLowerCase()) {
                case 'eventcode':
                    param = getComboBoxSelectedOption(document.getElementById('pEvents'));
                    break;
                case 'speedlimit':
                    param = $('#pSpeedLimit').val();
                    break;
                case 'idlelimit':
                    param = $('#pIdleLimit').val();
                    break;
                case 'mintime':
                    param = $('#pMinTime').val();
                    break;
                case 'driverid':
                    param = getComboBoxSelectedOption(document.getElementById('pDrivers'));
                    break;
                case 'geofenceid':
                    param = getComboBoxSelectedOption(document.getElementById('pGeofences'));
                    break;
            }
        }

        var frequencyId = $('#cbxRecRepFrequencies').val();
        var frequencyName = $('#cbxRecRepFrequencies option:selected').text();

        var excludeWeekends = $('#chkRecRepWeekends').prop('checked');

        var lstDevices = [];
        var isAllDevices = true;
        $('#newRecRepDevicesList input:checkbox').each(function () {
            if (this.checked == true) {
                lstDevices.push({ 'deviceId': $(this).attr('data-deviceId') });
            }
            else {
                isAllDevices = false;
            }
        });
        var jsonDevicesTXT = JSON.stringify(lstDevices);

        var lstUsers = [];
        var isAllUsers = true;
        $('#newRecRepUsersList input:checkbox').each(function () {
            if (this.checked == true) {
                lstUsers.push({ 'id': $(this).attr('data-userId') });
            }
            else {
                isAllUsers = false;
            }
        });
        var jsonUsersTXT = JSON.stringify(lstUsers);

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&reportId=' + escape(reportId) + '&param=' + escape(param) + '&frequencyId=' + escape(frequencyId) + '&excludeWeekends=' + escape(excludeWeekends) + '&isAllDevices=' + escape(isAllDevices) + '&devices=' + escape(jsonDevicesTXT) + '&isAllUsers=' + escape(isAllUsers) + '&users=' + escape(jsonUsersTXT);
        var tmpJson = dbReadWrite('saveRecurrentReport', data, true, false);

        if (tmpJson.result != 'failure') {
            var itm = { 'id': tmpJson.result, 'reportId': reportId, 'reportName': reportName, 'hasParam': hasParam, 'paramName': paramName, 'paramValue': param, 'frequencyId': frequencyId, 'frequencyName': frequencyName, 'excludeWeekends': excludeWeekends, 'isAllDevices': isAllDevices, 'createdOn': 'Today' };
            if (id == '0' || id == '') {
                addRecRepToList(itm);
            }
            else {
                modifyRecRepListRecord(id, itm);
            }
        }

        return true;

    }
    catch (err) {
        alert('saveRecRep: ' + err.description);
    }
}

function setupNewRecRepDlg() {
    try {
        loadReports();
        loadFrequencies();
        $("#newRecRepDlg").dialog({
            height: 500,
            width: 400,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveRecRep() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving Recurrent Report.  Please try again.');
                    }
                },
                Cancel: function () {
                    $(this).dialog("close");
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
        alert('setupNewRecRepDlg: ' + err.description);
    }
}

function deleteRecRep(obj) {
    try {
        if (isRecRepDlgReady == false) {
            setupNewRecRepDlg();
            setupRecRepRemoveDlg();
            isRecRepDlgReady = true;
        }
        var id = $(obj.target).attr('data-id');
        var name = $('#recRepTR' + id).attr('data-reportName');
        $('#recRepRemoveName').html(name);
        $('#recRepRemoveName').attr('data-id', id);
        $("#recRepRemoveDlg").dialog('open')
    }
    catch (err) {
        alert('deleteRecRep: ' + err.description);
    }
}

function deleteRecRepCommit() {
    try {
        var id = $('#recRepRemoveName').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('removeRecurrentReport', data, true, false);

        var tbl = document.getElementById('recurrentReportsTable');
        var tr = document.getElementById('recRepTR' + id);
        tbl.removeChild(tr);

        return true;
    }
    catch (err) {
        alert('deleteRecRepCommit: ' + err.description);
    }
}

function setupRecRepRemoveDlg() {
    try {
        $("#recRepRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteRecRepCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing Recurrent Report.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#recRepRemoveName').html('');
            }
        });
    }
    catch (err) {
        alert('setupRecRepRemoveDlg: ' + err.description);
    }
}

