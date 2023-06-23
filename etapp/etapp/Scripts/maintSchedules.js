// MAINTENANCE SCHEDULES ROUTINES
//==========================================================================
var jsonMaintSchedules = false;
var isMaintServiceReady = false;

function delMaintSchedule(e) {
    try{
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        var id = dataItem.id;

        $('#scheduleRemoveDlg').attr('data-id', id);
        $('#scheduleRemoveDlg').dialog('open');;

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function getMaintSchedulesOk(data, textStatus, jqXHR) {
    try {
        var ds = new kendo.data.DataSource({
            data: data
        });

        $("#maintenanceSchedulesGrid").kendoGrid({
            toolbar: ["excel"],
            excel: {
                fileName: "MaintenanceSchedules.xlsx"
            },
            dataSource: ds,
            columns: [
                { field: "id", title: 'id', hidden: true },
                { field: "deviceName", title: 'Device' },
                { field: "taskName", title: 'Task' },
                { field: "taskMeassureName", title: 'Meassure' },
                { field: 'taskValueStr', title: 'Service every' },
                { field: "currentValueStr", title: 'Current Value' },
                { field: 'nextServiceStr', title: 'Next Service' },
                { field: "lastServiceOn", title: 'Last Service' },
                { command: { text: "Delete", click: delMaintSchedule }, title: " ", width: "120px" }
            ]
        });

        var rows = $('#maintenanceSchedulesGrid').data('kendoGrid').tbody.children();

        for (var j = 0; j < rows.length; j++) {
            var row = $(rows[j]);
            var dataItem = $('#maintenanceSchedulesGrid').data('kendoGrid').dataItem(row);

            if (dataItem.get("nextService") < 0) {
                row.addClass("red");
            }
            else {
                if (dataItem.get("nextService") < dataItem.get("notifyBefore")) {
                    row.addClass("yellow");
                }
            }
        }

    }
    catch (err) {
        alert('getMaintSchedulesOk: ' + err.description);
    }
}

function getMaintSchedulesError(jqXHR, textStatus, errorThrown) {
    try {
        var a = 1;
    }
    catch (err) {
        alert('getMaintSchedulesError: ' + err.description);
    }
}

function getMaintSchedules_v2(deviceId, taskId) {
    try {
        if (deviceId == undefined) {
            deviceID = '';
        }
        if (!isInteger(taskId)) {
            taskID = 0;
        }
        var token = getTokenCookie('ETTK');
        var noCache = Math.random();
        var data = 'deviceId=' + escape(deviceId) + '&taskId=' + taskId + '&noCache=' + noCache;
        url = 'etrest.svc/getMaintSchedules/'
        $.ajax({
            type: "GET",
            url: url + escape(token),
            contentType: 'application/json',
            data: data,
            dataType: "json",
            processdata: false,
            success: getMaintSchedulesOk,
            error: getMaintSchedulesError,
            async: true
        });
    }
    catch (err) {
        alert('getBoardDevices: ' + err.description);
    }
}

function btnSchedApplyFilter() {
    var devID = '';
    var taskId = 0;

    try{
        devId = $('#cbxSchedDevFilter').val();
        taskId = $('#cbxSchedTaskFilter').val();
    }
    catch (err) {
        devId = '';
        taskID = 0;
    }
    try {
        getMaintSchedules_v2(devId, taskId);
    }
    catch (err) {
        alert('btnSchedApplyFilter: ' + err.description);
    }
}

function loadMaintenanceSchedules() {
    try {
        if (isMaintServiceReady == false) {
            if (jsonDevices == false) {
                getDevices();
            }
            if (jsonDevices) {
                var cbx = document.getElementById('cbxSchedDevFilter');
                removeAllChildNodes(cbx);
                loadComboBox(jsonDevices.myDevices, cbx, 'All Devices');
            }

            if (jsonTasks == false) {
                getTasks();
            }
            if (jsonTasks) {
                var cbx = document.getElementById('cbxSchedTaskFilter');
                removeAllChildNodes(cbx);
                loadComboBox(jsonTasks.tasks, cbx, 'All Tasks');
            }

            isMaintServiceReady = true;

        }
        getMaintSchedules_v2('', 0);
    }
    catch (err) {
        alert('loadMaintenanceSchedules: ' + err.description);
    }
}

function loadMaintAlertUsers(mode, jsonAllUsers, jsonMaintAlertUsers) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllUsers) {
            var tbl = document.getElementById('newMaintAlertUsersTbl');
            removeAllChildNodes(tbl);

            var trHeader = document.createElement('tr');
            $(tbl).append(trHeader);

            var tdH1 = document.createElement('td');
            $(trHeader).append(tdH1);
            $(tdH1).attr('style', 'width:350px; font-weight:600;');
            $(tdH1).text('User');

            var tdH2 = document.createElement('td');
            $(trHeader).append(tdH2);
            $(tdH2).attr('style', 'width:50px; font-weight:600;');
            $(tdH2).text('Email');

//            var tdH3 = document.createElement('td');
//            $(trHeader).append(tdH3);
//            $(tdH3).attr('style', 'width:50px; font-weight:600;');
//            $(tdH3).text('SMS');


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

//                var chkSMS = document.createElement('input');
//                $(chkSMS).attr('type', 'checkbox');
//                $(chkSMS).attr('id', 'chkNewAlertUserSMS' + jsonItem.id);
//                $(chkSMS).attr('data-userId', jsonItem.id);
//                $(chkSMS).attr('style', 'padding:3px;');

//                var tdSMS = document.createElement('td');
//                $(tr).append(tdSMS);
//                $(tdSMS).append(chkSMS);

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonMaintAlertUsers.users.length; ind2++) {
                        var jsonItem2 = eval('(' + jsonMaintAlertUsers.users[ind2] + ')');
                        if (jsonItem.id == jsonItem2.id) {
                            if (jsonItem2.isEmail == true) {
                                $(chkEmail).prop('checked', true);
                            }
//                            if (jsonItem2.isSMS == true) {
//                                $(chkSMS).prop('checked', true);
//                            }

                            break;
                        }
                    }
                }
            }
        }

    }
    catch (err) {
        alert('loadMaintAlertUsers: ' + err.description);
    }
}

function editMaintSchedule(obj) {
    try {
        if (validateUserAccess(24) == true) {
            var id = $(obj.target).attr('data-id');
            $('#newScheduleDlg').attr('data-id', id);

            //Mode:1 = New ; Mode:2 = Edit
            $('#newScheduleDlg').attr('data-mode', 2);

            //Device
            $('#newScheduleDlgDeviceTR').hide();
            $('#editScheduleDlgDeviceTR').show();
            $('#txtEditScheduleDevice').attr('data-deviceId', $('#scheduleTR' + id).attr('data-deviceId'));
            $('#txtEditScheduleDevice').val($('#scheduleTR' + id).attr('data-deviceName'));

            //Task
            $('#newScheduleDlgTaskTR').hide();
            $('#editScheduleDlgTaskTR').show();
            $('#txtEditScheduleTask').attr('data-taskId', $('#scheduleTR' + id).attr('data-taskId'));
            $('#txtEditScheduleTask').val($('#scheduleTR' + id).attr('data-taskName'));


            //After assigning the taskId, call the setNewScheduleMeassureUnitName routine
            $('#cbxNewScheduleTasks').val($('#scheduleTR' + id).attr('data-taskId'));
            setNewScheduleMeassureUnitName();

            $('#dtpNewScheduleLastService').val($('#scheduleTR' + id).attr('data-lastServiceOnString'));
            $('#newScheduleValueName').val($('#scheduleTR' + id).attr('data-taskMeassureName'));

            //These values have to be set  after invoking setNewScheduleMeassureUnitName, or else the user input will be replaced by the task default.
            $('#newScheduleTaskValue').val($('#scheduleTR' + id).attr('data-taskValue'));
            $('#newScheduleValue').val($('#scheduleTR' + id).attr('data-currentValue'));

            //Notifications
            $('#newScheduleNotifications').val($('#scheduleTR' + id).attr('data-notifyBefore'));
            if ($('#scheduleTR' + id).attr('data-notifyEveryXDays') == '0') {
                $('#notifTypeOnce').prop('checked', true);
                $('#notifTypeMany').prop('checked', false);
            }
            else {
                $('#notifTypeOnce').prop('checked', false);
                $('#notifTypeMany').prop('checked', true);
                $('#cbxNotifyEveryXDays').val($('#scheduleTR' + id).attr('data-notifyEveryXDays'));
                if ($('#scheduleTR' + id).attr('data-excludeWeekends') == 'true') {
                    $('#excludeWeekends').prop('checked', true);
                }
                else {
                    $('#excludeWeekends').prop('checked', false);
                }
            }

//            //The following creates a dependency with users.js
//            if (jsonUsers == false) {
//                getUsers();
//            }

//            var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
//            var jsonMaintAlertUsers = dbReadWrite('getMaintAlertUsers', data, true, false);
//            loadMaintAlertUsers(2, jsonUsers, jsonMaintAlertUsers);

            $("#newScheduleDlg").dialog('open');
        }
    }
    catch (err) {
        alert('editMaintSchedule: ' + err.description);
    }
}

function newMaintSchedule() {
    try {
        $('#newScheduleDlg').attr('data-id', '0');

        //Mode: 1 = New; Mode: 2 = Edit
        $('#newScheduleDlg').attr('data-mode', 1);

        //Device
        $('#newScheduleDlgDeviceTR').show();
        $('#editScheduleDlgDeviceTR').hide();

        //Task
        $('#newScheduleDlgTaskTR').show();
        $('#editScheduleDlgTaskTR').hide();

        setNewScheduleMeassureUnitName();

//        //The following creates a dependency with users.js
//        if (jsonUsers == false) {
//            getUsers();
//        }
//        loadMaintAlertUsers(1, jsonUsers);

        $("#newScheduleDlg").dialog('open');
    }
    catch (err) {
        alert('newMaintSchedule: ' + err.description);
    }
}

function setNewScheduleMeassureUnitName(){
    try {
        var cbx = document.getElementById('cbxNewScheduleTasks');
        var opt = getComboBoxSelectedOption(cbx);

        $('.newScheduleMeassureName').each(function () {
            $(this).html('');
        });
        $('#newScheduleTaskName').html('');
        $('#newScheduleTaskValue').val('');

        $('#newScheduleValue').val('');
        $('#newScheduleNotifications').val('');

        if (opt != '0') {
            var jsonItem = getJsonRecord(jsonTasks.tasks, opt);
            if (jsonItem) {
                $('#newScheduleDlg').attr('data-taskMeassureId', jsonItem.taskMeassureId);
                $('#newScheduleDlg').attr('data-taskMeassureName', jsonItem.taskMeassureName);

                $('#newScheduleTaskName').html(jsonItem.name);
                $('#newScheduleTaskValue').val(jsonItem.value);

                $('.newScheduleMeassureName').each(function () {
                    $(this).html(jsonItem.taskMeassureName);
                });
            }
        }

    }
    catch (err) {
        alert('setNewScheduleMeassureUnitName: ' + err.description);
    }
}

function initializeDatePickers() {
    try {
        $('#dtpNewScheduleLastService').val('');
        $("#dtpNewScheduleLastService").datepicker();
    }
    catch (err) {
        alert('initializeDatePickers: ' + err.description);
    }
}

function loadNewScheduleDevices() {
    try {
        if (jsonDevices == false) {
            getDevices();
        }
        if (jsonDevices) {
            var cbx = document.getElementById('cbxNewScheduleDevices');
            removeAllChildNodes(cbx);
            loadComboBox(jsonDevices.myDevices, cbx, 'Pick a device');
        }
    }
    catch (err) {
        alert('loadNewScheduleDevices: ' + err.description);
    }
}

function loadNewScheduleTasks() {
    try {
        if (jsonTasks == false) {
            getTasks();
        }
        if (jsonTasks) {
            var cbx = document.getElementById('cbxNewScheduleTasks');
            removeAllChildNodes(cbx);
            loadComboBox(jsonTasks.tasks, cbx, 'Pick a task');
        }
    }
    catch (err) {
        alert('loadNewScheduleTasks: ' + err.description);
    }
}

function deleteMaintScheduleCommit() {
    try {
        var id = $('#scheduleRemoveDlg').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('deleteMaintSchedule', data, true, false);

        btnSchedApplyFilter();

        return true;

    }
    catch (err) {
        alert('deleteMaintScheduleCommit: ' + err.description);
    }
}

function deleteMaintSchedule(obj) {
    try {
        var id = $(obj.target).attr('data-id');
        $('#scheduleRemoveDlg').attr('data-id', id);
        $('#scheduleRemoveDlg').dialog('open'); ;
    }
    catch (err) {
        alert('deleteMaintSchedule: ' + err.description);
    }
}

function modifyScheduleListRecord(id, itm) {
    try {
        var tr = document.getElementById('scheduleTR' + id);
        removeAllChildNodes(tr);
        fillScheduleRecord(tr, itm);
    }
    catch (err) {
        alert('modifyScheduleListRecord: ' + err.description);
    }
}

function saveSchedule() {
    try {
        var id = $('#newScheduleDlg').attr('data-id');

        //Mode: 1 = New; Mode: 2 = Edit
        var mode = $('#newScheduleDlg').attr('data-mode');

        var deviceId = 0;
        var deviceName = '';
        var taskId = 0;
        var taskName = '';

        if (mode == 1) {
            deviceId = $('#cbxNewScheduleDevices').val();
            if (deviceId == '0') {
                alert('Please select a device');
                return false;
            }
            deviceName = $('#cbxNewScheduleDevices option:selected').text();

            taskId = $('#cbxNewScheduleTasks').val();
            if (taskId == '0') {
                alert('Please select a maintenance task');
                return false;
            }
            taskName = $('#cbxNewScheduleTasks option:selected').text();
        }
        else {
            deviceId = $('#txtEditScheduleDevice').attr('data-deviceId');
            deviceName = $('#txtEditScheduleDevice').val();

            taskId = $('#txtEditScheduleTask').attr('data-taskId');
            taskName = $('#txtEditScheduleTask').val();
        }


        var meassureId = $('#newScheduleDlg').attr('data-taskMeassureId');
        var meassureName = $('#newScheduleDlg').attr('data-taskMeassureName');

        var taskValue = $('#newScheduleTaskValue').val();
        var lastServiceOn = $('#dtpNewScheduleLastService').val();
        var currentValue = $('#newScheduleValue').val();
        if (currentValue == '') {
            currentValue = 0;
        }

        //Notifications
        var notifyBefore = $('#newScheduleNotifications').val();
        if (notifyBefore == '') {
            notifyBefore = 0;
        }
        var notifyEveryXDays = 0;
        var excludeWeekends = true;
        if (notifyBefore != 0) {
            var notifType = $('input[name="notificationType"]:checked').val();
            if (notifType == 'many') {
                notifyEveryXDays = $('#cbxNotifyEveryXDays').val();
                excludeWeekends = $('#excludeWeekends').prop('checked');
            }
        }

        var lstUsers = [];
//        var isAllUsers = true;
        var isAllUsers = false;
//        $('#newMaintAlertUsersTbl tr:gt(0)').each(function () {
//            var userId = $(this).attr('data-userId');
//            var isEmail = $(this).find('#chkNewAlertUserEmail' + userId).prop('checked');
//            // var isSMS = $(this).find('#chkNewAlertUserSMS' + userId).prop('checked');
//            var isSMS = false;

//            if (isEmail == true || isSMS == true) {
//                lstUsers.push({ 'id': userId, 'isEmail': isEmail, 'isSMS': isSMS });
//            }
//            if (isEmail == false && isSMS == false) {
//                isAllUsers = false;
//            }

//        });
        var jsonUsersTXT = JSON.stringify(lstUsers);

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&deviceId=' + escape(deviceId) + '&taskId=' + escape(taskId) + '&taskValue=' + escape(taskValue) + '&lastServiceOn=' + escape(lastServiceOn) + '&currentValue=' + escape(currentValue) + '&notifyBefore=' + escape(notifyBefore) + '&notifyEveryXDays=' + escape(notifyEveryXDays) + '&excludeWeekends=' + escape(excludeWeekends) + '&users=' + escape(jsonUsersTXT);
        var tmpJson = dbReadWrite('saveMaintSchedule', data, true, false);

        if (tmpJson.result != 'failure') {
            var itm = { 'id': tmpJson.result, 'deviceId': deviceId, 'deviceName': deviceName, 'taskId': taskId, 'taskName': taskName, 'taskMeassureId': meassureId, 'taskMeassureName': meassureName, 'taskValue': taskValue, 'lastServiceOn': lastServiceOn, 'lastServiceOnString': lastServiceOn, 'notifyBefore': notifyBefore, 'notifyEveryXDays': notifyEveryXDays, 'excludeWeekends': excludeWeekends, 'currentValue': currentValue };
            if (id == '0' || id == '') {
                addScheduleToList(itm);
            }
            else {
                modifyScheduleListRecord(id, itm);
            }
            return true;
        }
        else {
            return false;
        }
    }
    catch (err) {
        alert('saveSchedule: ' + err.description);
    }
}

function setupNewScheduleDlg() {
    try {
        loadNewScheduleDevices();
        loadNewScheduleTasks();
        $("#newScheduleDlg").dialog({
            height: 420,
            width: 550,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveSchedule() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving Schedule.  Please try again.');
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
                //Set to 0 the initial position of the combo boxes
                $('#cbxNewScheduleDevices option').eq(0).attr('selected', 'selected');
                $('#cbxNewScheduleTasks option').eq(0).attr('selected', 'selected');
                $('#dtpNewScheduleLastService').val(getNow());
                $('#dtpNewScheduleNextExpiration').val(getNow());
                $('#newScheduleValue').val('');
                $('#newScheduleNotifications').val('');
                $('#newScheduleTaskName').html('');
                $('#newScheduleTaskValue').val('');
                $('.newScheduleMeassureName').each(function () {
                    $(this).html('');
                });
            }
        });
    }
    catch (err) {
        alert('setupNewScheduleDlg: ' + err.description);
    }
}

function setupScheduleRemoveDlg() {
    try {
        $("#scheduleRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteMaintScheduleCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing schedule.  Please try again.');
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
        alert('setupScheduleRemoveDlg: ' + err.description);
    }
}

