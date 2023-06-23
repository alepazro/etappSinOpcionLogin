var isAlertDlgReady = false;
var jsonAlerts = false;
var jsonAlertsTypes = false;

function newAlertSelectAll(elemId, val) {
    try {
        $('#' + elemId + ' input:checkbox').each(function () {
            this.checked = val;
        });
    }
    catch (err) {
        alert('newAlertSelectAll:' + err.description);
    }
}

function loadAlertDevices(mode, jsonAllDevices, jsonAlertDevices) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllDevices) {
            var ul = document.getElementById('newAlertDevicesList');
            removeAllChildNodes(ul);

            for (var ind = 0; ind < jsonAllDevices.myDevices.length; ind++) {
                jsonDevice = eval('(' + jsonAllDevices.myDevices[ind] + ')');
                //Add the <li>
                var li = document.createElement('li');
                $(li).attr('id', 'newAlertDeviceId' + jsonDevice.deviceId);
                ul.appendChild(li);

                var chk = document.createElement('input');
                $(chk).attr('type', 'checkbox');
                $(chk).attr('id', 'chkNewAlertDevice' + jsonDevice.deviceId);
                $(chk).attr('data-deviceId', jsonDevice.deviceId);
                $(chk).attr('style', 'padding:3px;');

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonAlertDevices.myDevices.length; ind2++) {
                        var jsonDevice2 = eval('(' + jsonAlertDevices.myDevices[ind2] + ')');
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
        alert('loadAlertDevices: ' + err.description);
    }
}

function loadAlertUsers(mode, jsonAllUsers, jsonAlertUsers) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllUsers) {
            var ul = false;
            ul = document.getElementById('newAlertUsersList');
            removeAllChildNodes(ul);

            var tbl = document.getElementById('newAlertUsersTbl');
            removeAllChildNodes(tbl);

            var trHeader = document.createElement('tr');
            $(tbl).append(trHeader);

            var tdH1 = document.createElement('td');
            $(trHeader).append(tdH1);
            $(tdH1).attr('style', 'width:200px; font-weight:600;');
            $(tdH1).text('User');

            var tdH2 = document.createElement('td');
            $(trHeader).append(tdH2);
            $(tdH2).attr('style', 'width:50px; font-weight:600;');
            $(tdH2).text('Email');

            var tdH3 = document.createElement('td');
            $(trHeader).append(tdH3);
            $(tdH3).attr('style', 'width:50px; font-weight:600;');
            $(tdH3).text('SMS');


            for (var ind = 0; ind < jsonAllUsers.users.length; ind++) {
                var jsonItem = eval('(' + jsonAllUsers.users[ind] + ')');

                //Create the table - new option
                var tr = document.createElement('tr');
                $(tbl).append(tr);
                $(tr).attr('id', 'chkNewAlertUserTr' + jsonItem.id);
                $(tr).attr('data-userId', jsonItem.id);

                var td1 = document.createElement('td');
                $(tr).append(td1);
                var span1 = document.createElement('span');
                $(td1).append(span1);
                $(span1).text(jsonItem.firstName + ' ' + jsonItem.lastName);

                var chkEmail = document.createElement('input');
                $(chkEmail).attr('type', 'checkbox');
                $(chkEmail).attr('id', 'chkNewAlertUserEmail' + jsonItem.id);
                $(chkEmail).attr('data-userId', jsonItem.id);
                $(chkEmail).attr('style', 'padding:3px;');

                var tdEmail = document.createElement('td');
                $(tr).append(tdEmail);
                $(tdEmail).append(chkEmail);

                var chkSMS = document.createElement('input');
                $(chkSMS).attr('type', 'checkbox');
                $(chkSMS).attr('id', 'chkNewAlertUserSMS' + jsonItem.id);
                $(chkSMS).attr('data-userId', jsonItem.id);
                $(chkSMS).attr('style', 'padding:3px;');

                var tdSMS = document.createElement('td');
                $(tr).append(tdSMS);
                $(tdSMS).append(chkSMS);

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonAlertUsers.users.length; ind2++) {
                        var jsonItem2 = eval('(' + jsonAlertUsers.users[ind2] + ')');
                        if (jsonItem.id == jsonItem2.id) {
                            if (jsonItem2.isEmail == true) {
                                $(chkEmail).prop('checked', true);
                            }
                            if (jsonItem2.isSMS == true) {
                                $(chkSMS).prop('checked', true);
                            }

                            break;
                        }
                    }
                }
            }
        }

    }
    catch (err) {
        alert('loadAlertUsers: ' + err.description);
    }
}

function loadAlertsDlg(mode, alertId, alertTypeId, alertTypeName, alertName, value, jsonAllDevices, jsonAllUsers, jsonAlertDevices, jsonAlertUsers, mon, tue, wed, thu, fri, sat, sun, hourFrom, hourTo, minInterval, setPointMin, setPointMax) {
//mode:  1 = NEW, 2 = EDIT
    try {
        $('#newAlertValueTR').hide(); //2-25-2012
        $('#newAlertGeofenceMsgTR').hide();
        $('#newAlertScheduleDaysTR').hide();
        $('#newAlertScheduleHoursTR').hide();
        $('.newAlertTemp').hide();

        $('#newAlertName').attr('data-id', alertId);
        $('#newAlertName').attr('data-alertTypeId', alertTypeId);
        $('#newAlertName').attr('data-alertTypeName', alertTypeName);

        $('#alertTitle').text(alertTypeName);
        //Open dialog for other alerts
        switch (alertTypeId) {
            case "1":
                $('#newAlertGeofenceMsgTR').show();
                break;
            case "4":
                $('#newAlertValueTR').show();
                $('#newAlertValueLabel').text('Speed Limit');
                break;
            case "5":
                $('#newAlertValueTR').show();
                $('#newAlertValueLabel').text('Idle Threshold (mins)');
                break;
            case "6":
                break;
            case "7":
            case 7:
                if (mon == undefined) mon = false;
                if (tue == undefined) tue = false;
                if (wed == undefined) wed = false;
                if (thu == undefined) thu = false;
                if (fri == undefined) fri = false;
                if (sat == undefined) sat = false;
                if (sun == undefined) sun = false;
                if (hourFrom == undefined) hourFrom = 0;
                if (hourTo == undefined) hourTo = 0;

                $('#newAlertSchedMon').prop('checked', ('true' == mon));
                $('#newAlertSchedTue').prop('checked', ('true' == tue));
                $('#newAlertSchedWed').prop('checked', ('true' == wed));
                $('#newAlertSchedThu').prop('checked', ('true' == thu));
                $('#newAlertSchedFri').prop('checked', ('true' == fri));
                $('#newAlertSchedSat').prop('checked', ('true' == sat));
                $('#newAlertSchedSun').prop('checked', ('true' == sun));

                $('#newAlertsHourFrom').val(hourFrom);
                $('#newAlertsHourTo').val(hourTo);

                $('#newAlertScheduleDaysTR').show();
                $('#newAlertScheduleHoursTR').show();
                break;
            case "20":
            case 20:
                $('#newAlertTempLow').val(setPointMin);
                $('#newAlertTempHigh').val(setPointMax);
                $('.newAlertTemp').show();
                break;
        }

        if (alertId != '0' && alertId != '') {
            $('#newAlertName').val(alertName);
            $('#newAlertValue').val(value);
            
        }
        if (mode == 1) {
            $('#newAlertMinInterval').val(0);
        }
        else {
            $('#newAlertMinInterval').val(minInterval);
        }

        //Load the device
        loadAlertDevices(mode, jsonAllDevices, jsonAlertDevices);

        //Load the users
        loadAlertUsers(mode, jsonAllUsers, jsonAlertUsers);

        $("#newAlertDlg").dialog('open');
    }
    catch (err) {
        alert('loadAlertsDlg: ' + err.description);
    }
}

function newAlert() {
    try {
        if (isAlertDlgReady == false) {
            setupNewAlertDlg();
            setupAlertRemoveDlg();
            isAlertDlgReady = true;
        }
        var alertTypeId = $('#cbxAlertsTypes').val();
        var alertTypeName = $('#cbxAlertsTypes option:selected').text();
        if (alertTypeId == 0) {
            alert('Please select an alert type');
        }
        else {
            //The following creates a dependency with devicesSettings.js
            if (jsonDevices == false) {
                getDevices();
            }

            //The following creates a dependency with users.js
            if (jsonUsers == false) {
                getUsers();
            }

            loadAlertsDlg(1, '0', alertTypeId, alertTypeName, '', 0, jsonDevices, jsonUsers);
        }
    }
    catch (err) {
        alert('newAlert: ' + err.description);
    }
}

function editAlert(obj) {
    try {
        if (isAlertDlgReady == false) {
            setupNewAlertDlg();
            setupAlertRemoveDlg();
            isAlertDlgReady = true;
        }

        //The following creates a dependency with devicesSettings.js
        if (jsonDevices == false) {
            getDevices();
        }

        //The following creates a dependency with users.js
        if (jsonUsers == false) {
            getUsers();
        }

        var alertId = $(obj.target).attr('data-id');

        var alertTypeId = $('#alertTR' + alertId).attr('data-alertTypeId');
        var alertTypeName = $('#alertTR' + alertId).attr('data-alertTypeName');
        var alertName = $('#alertTR' + alertId).attr('data-alertName');
        var alertValue = $('#alertTR' + alertId).attr('data-alertValue');
        var alertTempHigh = $('#alertTR' + alertId).attr('data-alertTempHigh');
        var alertTempLow = $('#alertTR' + alertId).attr('data-alertTempLow');
        var alertMinInterval = $('#alertTR' + alertId).attr('data-alertMinInterval');
        var mon = $('#alertTR' + alertId).attr('data-mon');
        var tue = $('#alertTR' + alertId).attr('data-tue');
        var wed = $('#alertTR' + alertId).attr('data-wed');
        var thu = $('#alertTR' + alertId).attr('data-thu');
        var fri = $('#alertTR' + alertId).attr('data-fri');
        var sat = $('#alertTR' + alertId).attr('data-sat');
        var sun = $('#alertTR' + alertId).attr('data-sun');
        var hourFrom = $('#alertTR' + alertId).attr('data-hourFrom');
        var hourTo = $('#alertTR' + alertId).attr('data-hourTo');

        var setPointMin = 0;
        var setPointMax = 0;
        try{
            setPointMin = $('#alertTR' + alertId).attr('data-spMin');
            setPointMax = $('#alertTR' + alertId).attr('data-spMax');
        } catch (err) {
            setPointMin = 0;
            setPointMax = 0;
        }

        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(alertId);
        var jsonAlertDevices = dbReadWrite('getAlertDevices', data, true, false);
        var jsonAlertUsers = dbReadWrite('getAlertUsers', data, true, false);

        loadAlertsDlg(2, alertId, alertTypeId, alertTypeName, alertName, alertValue, jsonDevices, jsonUsers, jsonAlertDevices, jsonAlertUsers, mon, tue, wed, thu, fri, sat, sun, hourFrom, hourTo, alertMinInterval, setPointMin, setPointMax);

    }
    catch (err) {
        alert('editAlert: ' + err.description);
    }

}

function deleteAlert(obj) {
    try {
        if (isAlertDlgReady == false) {
            setupNewAlertDlg();
            setupAlertRemoveDlg();
            isAlertDlgReady = true;
        }
        var id = $(obj.target).attr('data-id');
        var name = $('#alertTR' + id).attr('data-alertName');
        $('#alertRemoveName').html(name);
        $('#alertRemoveName').attr('data-id', id);
        $("#alertRemoveDlg").dialog('open')
    }
    catch (err) {
        alert('deleteAlert: ' + err.description);
    }
}

function modifyAlertListRecord(id, itm) {
    try {
        var tr = document.getElementById('alertTR' + id);
        removeAllChildNodes(tr);
        fillAlertRecord(tr, itm);
    }
    catch (err) {
        alert('modifyAlertListRecord: ' + err.description);
    }
}

function fillAlertRecord(tr, item) {
    try {
        var tbl = document.getElementById('alertsTbl');

        $(tr).attr('id', 'alertTR' + item.id);
        $(tr).attr('data-alertName', item.name);
        $(tr).attr('data-alertTypeId', item.alertTypeId);
        $(tr).attr('data-alertTypeName', item.alertTypeName);
        $(tr).attr('data-alertValue', item.value);
        $(tr).attr('data-alertTempHigh', item.alertTempHigh);
        $(tr).attr('data-alertTempLow', item.alertTempLow);
        $(tr).attr('data-alertMinInterval', item.minInterval);
        $(tr).attr('data-mon', item.mon);
        $(tr).attr('data-tue', item.tue);
        $(tr).attr('data-wed', item.wed);
        $(tr).attr('data-thu', item.thu);
        $(tr).attr('data-fri', item.fri);
        $(tr).attr('data-sat', item.sat);
        $(tr).attr('data-sun', item.sun);
        $(tr).attr('data-hourFrom', item.hourFrom);
        $(tr).attr('data-hourTo', item.hourTo);
        $(tr).attr('data-spMin', item.setPointMin);
        $(tr).attr('data-spMax', item.setPointMax);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('alertsListOddTR');
        }

        //Alert type name
        var alertTypeNameTd = document.createElement('td');
        $(alertTypeNameTd).html(item.alertTypeName);
        $(alertTypeNameTd).addClass('alertsListTD');
        tr.appendChild(alertTypeNameTd);

        //Alert name
        var alertNameTd = document.createElement('td');
        $(alertNameTd).html(item.name);
        $(alertNameTd).addClass('alertsListTD');
        tr.appendChild(alertNameTd);

        //Value
        var alertValueTd = document.createElement('td');
        $(alertValueTd).html(item.valueDescription);
        $(alertValueTd).addClass('alertsListTD');
        tr.appendChild(alertValueTd);

        //Min. Interval
        var alertMinIntervalTd = document.createElement('td');
        $(alertMinIntervalTd).html(item.minInterval);
        $(alertMinIntervalTd).addClass('alertsListTD');
        tr.appendChild(alertMinIntervalTd);

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
        $(createdOnTd).html(item.createdOnString);
        $(createdOnTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(createdOnTd);

        //Edit
        var editTd = document.createElement('td');
        $(editTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(editTd);

        var editBtn = document.createElement('button');
        editTd.appendChild(editBtn);
        $(editBtn).attr('data-id', item.id);
        $(editBtn).click(editAlert);

        var editImg = document.createElement('img');
        $(editImg).attr('src', 'icons/edit_inline.png');
        $(editImg).attr('alt', '');
        $(editImg).attr('width', '16');
        $(editImg).attr('height', '16');
        $(editImg).attr('data-id', item.id);
        editBtn.appendChild(editImg);

        //Delete alert
        var delTd = document.createElement('td');
        $(delTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(delTd);

        var delBtn = document.createElement('button');
        delTd.appendChild(delBtn);
        $(delBtn).attr('data-id', item.id);
        $(delBtn).click(deleteAlert);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);

    }
    catch (err) {
        alert('fillAlertRecord: ' + err.description);
    }
}

function addAlertToList(item) {
    try {
        var tbl = document.getElementById('alertsTbl');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillAlertRecord(tr, item);
    }
    catch (err) {
        alert('addAlertToList: ' + err.description);
    }
}

function clearAlertsList() {
    try {
        $("#alertsTbl").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearAlertsList: ' + err.description);
    }
}

function getAlerts() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonAlerts = dbReadWrite('getAlerts', data, true, false);

        return true;
    }
    catch (err) {
        alert('getAlerts: ' + err.description);
    }
}

function loadAlerts() {
    try {
        loadAlertsTypes();
        if (jsonAlerts == false) {
            if (getAlerts() == true) {
                clearAlertsList();
                if (jsonAlerts) {
                    for (var ind = 0; ind < jsonAlerts.alerts.length; ind++) {
                        var jsonItem = eval('(' + jsonAlerts.alerts[ind] + ')');
                        addAlertToList(jsonItem);
                    }
                }
            }
        }
    }
    catch (err) {
        alert('loadAlerts: ' + err.description);
    }
}

function loadAlertsTypes() {
    try {
        if (jsonAlertsTypes == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonAlertsTypes = dbReadWrite('getAlertsTypes', data, true, false);

            if (jsonAlertsTypes) {
                var cbx = document.getElementById('cbxAlertsTypes');
                removeAllChildNodes(cbx);
                loadComboBox(jsonAlertsTypes.types, cbx, 'Pick a type');
            }
        }
    }
    catch (err) {
        alert('loadAlertsTypes: ' + err.description);
    }
}

function saveAlert() {
    try {
        var id = $('#newAlertName').attr('data-id');
        var alertTypeId = $('#newAlertName').attr('data-alertTypeId');
        var alertTypeName = $('#newAlertName').attr('data-alertTypeName');
        var alertName = $('#newAlertName').val();
        var alertValue = $('#newAlertValue').val();
        var alertValueDescription = $('#newAlertValue').val();
        var geofenceGUID = ''; //We put this here for when Geofence alerts be implemented
        var setPointMin = 0;
        var setPointMax = 0;

        var mon = $('#newAlertSchedMon').is(':checked');
        if (mon == undefined) mon = false;
        var tue = $('#newAlertSchedTue').is(':checked');
        if (tue == undefined) tue = false;
        var wed = $('#newAlertSchedWed').is(':checked');
        if (wed == undefined) wed = false;
        var thu = $('#newAlertSchedThu').is(':checked');
        if (thu == undefined) thu = false;
        var fri = $('#newAlertSchedFri').is(':checked');
        if (fri == undefined) fri = false;
        var sat = $('#newAlertSchedSat').is(':checked');
        if (sat == undefined) sat = false;
        var sun = $('#newAlertSchedSun').is(':checked');
        if (sun == undefined) sun = false;
        var hourFrom = getComboBoxSelectedOption(document.getElementById('newAlertsHourFrom'));
        if (hourFrom == undefined) hourFrom = 0;
        var hourTo = getComboBoxSelectedOption(document.getElementById('newAlertsHourTo'));
        if (hourTo == undefined) hourTo = 0;

        var minInterval = $('#newAlertMinInterval').val();
        if (_.isUndefined(minInterval)) {
            minInterval = 0;
        }
        if (!$.isNumeric(minInterval)) {
            minInterval = 0;
        }

        if (alertTypeId == 1) {
            geofenceGUID = id;
            alertValue = 0;
            alertValueDescription = '';
        }
        else {
            if (alertTypeId == 4 || alertTypeId == 5) {
                if ($.isNumeric(alertValue) == false) {
                    alertValue = 0;
                    alert('Please enter a value for the alert');
                    return;
                }
            }
            else {
                if (alertTypeId == 7) {
                    if (hourFrom == 0 && hourTo== 0) {
                        alert('Please set an hours range')
                        return;
                    }
                    if (mon == false && tue== false && wed == false && thu == false && fri == false && sat == false && sun == false){
                        alert('Please select at least one day of the week')
                        return;
                    }
                    alertValueDescription = '';
                    if (mon == true) alertValueDescription = alertValueDescription + 'M.';
                    if (tue == true) alertValueDescription = alertValueDescription + 'T.';
                    if (wed == true) alertValueDescription = alertValueDescription + 'W.';
                    if (thu == true) alertValueDescription = alertValueDescription + 'Th.';
                    if (fri == true) alertValueDescription = alertValueDescription + 'F.';
                    if (sat == true) alertValueDescription = alertValueDescription + 'S.';
                    if (sun == true) alertValueDescription = alertValueDescription + 'Su.';
                    alertValueDescription = alertValueDescription + '/' + hourFrom.toString() + ' to ' + hourTo.toString();
                } else {
                    if (alertTypeId == 20) {
                        try {
                            setPointMin = $('#newAlertTempLow').val();
                        } catch (err) {
                            setPointMin = 0;
                        }
                        try{
                            setPointMax = $('#newAlertTempHigh').val();
                        } catch (err) {
                            setPointMax = 0;
                        }
                    }
                }
            }
        }

        var lstDevices = [];
        var isAllDevices = true;
        $('#newAlertDevicesList input:checkbox').each(function () {
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
        $('#newAlertUsersTbl tr:gt(0)').each(function () {
            var userId = $(this).attr('data-userId');
            var isEmail = $(this).find('#chkNewAlertUserEmail' + userId).prop('checked');
            var isSMS = $(this).find('#chkNewAlertUserSMS' + userId).prop('checked');

            if (isEmail == true || isSMS == true) {
                lstUsers.push({ 'id': $(this).attr('data-userId'), 'isEmail': isEmail, 'isSMS': isSMS });
            }
            else {
                isAllUsers = false;
            }
        });
        var jsonUsersTXT = JSON.stringify(lstUsers);

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&typeId=' + escape(alertTypeId) + '&name=' + escape(alertName) + '&geofenceGUID=' + escape(geofenceGUID) + '&val=' + escape(alertValue) + '&isAllDevices=' + escape(isAllDevices) + '&devices=' + escape(jsonDevicesTXT) + '&isAllUsers=' + escape(isAllUsers) + '&users=' + escape(jsonUsersTXT) + '&mon=' + escape(mon) + '&tue=' + escape(tue) + '&wed=' + escape(wed) + '&thu=' + escape(thu) + '&fri=' + escape(fri) + '&sat=' + escape(sat) + '&sun=' + escape(sun) + '&hourFrom=' + escape(hourFrom) + '&hourTo=' + escape(hourTo) + '&minInterval=' + minInterval + '&spMin=' + setPointMin + '&spMax=' + setPointMax;
        var tmpJson = dbReadWrite('saveAlert', data, true, false);

        if (tmpJson.result != 'failure') {
            var itm = { 'id': tmpJson.result, 'name': alertName, 'alertTypeId': alertTypeId, 'alertTypeName': alertTypeName, 'geofenceGUID': geofenceGUID, 'value': alertValue, 'valueDescription': alertValueDescription, 'isAllDevices': isAllDevices, 'createdOnString': 'Today', 'mon': mon, 'tue': tue, 'wed': wed, 'thu': thu, 'fri': fri, 'sat': sat, 'sun': sun, 'hourFrom': hourFrom, 'hourTo': hourTo, 'minInterval': minInterval };

            if (id == '0' || id == '') {
                addAlertToList(itm);
            }
            else {
                modifyAlertListRecord(id, itm);
            }
        }

        return true;

    }
    catch (err) {
        alert('saveAlert: ' + err.description);
    }
}

function initAlertHours() {
    try {
        loadHoursCombo(document.getElementById('newAlertsHourFrom'));
        loadHoursCombo(document.getElementById('newAlertsHourTo'));
    }
    catch (err) {
        alert('initAlertHours: ' + err.description);
    }
}

function setupNewAlertDlg() {
    try {
        initAlertHours();
        $("#newAlertDlg").dialog({
            height: 600,
            width: 750,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveAlert() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving alert.  Please try again.');
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
                $('#newAlertName').val('');
                $('#newAlertValue').val('');
            }
        });
    }
    catch (err) {
        alert('setupNewAlertDlg: ' + err.description);
    }
}

function deleteAlertCommit() {
    try {
        var id = $('#alertRemoveName').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('removeAlert', data, true, false);

        var tbl = document.getElementById('alertsTbl');
        var tr = document.getElementById('alertTR' + id);
        tbl.removeChild(tr);

        return true;
    }
    catch (err) {
        alert('deleteAlertCommit: ' + err.description);
    }
}

function setupAlertRemoveDlg() {
    try {
        $("#alertRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteAlertCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing alert.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#alertRemoveName').html('');
            }
        });
    }
    catch (err) {
        alert('setupAlertRemoveDlg: ' + err.description);
    }
}
