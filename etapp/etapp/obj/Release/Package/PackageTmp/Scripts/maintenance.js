var isMaintDlgReady = false;
var jsonDevices = false;
var jsonTasks = false;
var jsonTasksMeassures = false;
var jsonServicesTypes = false;
var jsonStates = false;
var jsonTaskNotificationSendTo = false;

// BASIC ROUTINES
//==========================================================================

function hideMaintenanceDivs() {
    try {
        //$('#introDiv').hide();
        $('#tasksDiv').hide();
        $('#maintenanceSchedulesDiv').hide();
        $('#maintenanceHistoryDiv').hide();
        $('#fuelLogHistoryDiv').hide();
        $('#registerServicesDiv').hide();
        $('#registerFuelLogDiv').hide();

        $('#serviceModuleDiv').hide();

        $('#uploadFuelLogDiv').hide();

    }
    catch (err) {
        alert('hideMaintenanceDivs: ' + err.description);
    }
}

function showMaintenanceDivs(id) {
    try {
        hideMaintenanceDivs();

        if (isMaintDlgReady == false) {
            loadDevices();
            getTasks();
            getTasksMeassures();
            initializeDatePickers();
            setupNewTaskDlg();
            setupTaskRemoveDlg();
            setupNewScheduleDlg();
            setupScheduleRemoveDlg();
            setupServiceLogRemoveDlg();
            setupFuelLogRemoveDlg();
            setupFuelHistoryRemoveDlg();
            setupServiceHistoryRemoveDlg();

            isMaintDlgReady = true;
        }

        switch (id) {
            case 0:
                //$('#introDiv').show();
                break;
            case 1:
                if (validateUserAccess(17) == true) {
                    $('#tasksDiv').show();
                    loadTasks();
                }
                break;
            case 2:
                if (validateUserAccess(18) == true) {
                    $('#maintenanceSchedulesDiv').show();
                    loadMaintenanceSchedules();
                }
                break;
            case 3:
                if (validateUserAccess(19) == true) {
                    $('#maintenanceHistoryDiv').show();
                    loadMaintenanceHistory();
                }
                break;
            case 4:
                if (validateUserAccess(25) == true) {
                    $('#registerServicesDiv').show();
                    loadMaintServiceLog();
                }
                break;
            case 5:
                if (validateUserAccess(26) == true) {
                    $('#registerFuelLogDiv').show();
                    loadMaintFuelLog();
                }
                break;
            case 6:
                if (validateUserAccess(28) == true) {
                    $('#fuelLogHistoryDiv').show();
                    loadFuelLogHistory();
                }
                break;
            case 7:
                if (validateUserAccess(47) == true) {
                    $('#serviceModuleDiv').show();
                    var dlgScope = angular.element($("#serviceModuleDiv")).scope();
                    dlgScope.loadModule();
                }
                break;
            case 8:
                $('#uploadFuelLogDiv').show();
                //Get the fuel card format used by this company
                var jsonValue = false;
                jsonValue = getValue('FuelCardFormatID');
                fuelCardFormatId = jsonValue.value;
                if (fuelCardFormatId == '0') {
                    $('#dataFuelLogUploadUnavailable').show();
                    $('#dataFuelLogTextAreaDiv').hide();
                    $('#dataFuelLogTableDiv').hide();
                }
                else {
                    $('#dataFuelLogUploadUnavailable').hide();
                    clearUploadFuelLogArea();
                    loadExpectedColumnsInformation();
                }
        }
    }
    catch (err) {
        alert('showMaintenanceDivs: ' + err.description);
    }
}

// BASIC TABLES ROUTINES
//==========================================================================

function getDevices() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonDevices = dbReadWrite('getDevices', data, true, false);

        return true;
    }
    catch (err) {
        alert('getDevices: ' + err.description);
    }
}

function loadDevices() {
    try {
        //Always get a fresh version of the devices set
        if (jsonDevices == false) {
            getDevices();
        }
    }
    catch (err) {
        alert('loadDevices: ' + err.description);
    }
}

function getServicesTypes() {
    try {
        if (jsonServicesTypes == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonServicesTypes = dbReadWrite('getMaintServicesTypes', data, true, false);
        }

        return true;
    }
    catch (err) {
        alert('getServicesTypes: ' + err.description);
    }
}

function getTasks() {
    try {
        if (jsonTasks == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonTasks = dbReadWrite('getMaintenanceTasks', data, true, false);
        }

        return true;
    }
    catch (err) {
        alert('getTasks: ' + err.description);
    }
}

function getTasksMeassures() {
    try {
        if (jsonTasksMeassures == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonTasksMeassures = dbReadWrite('getMaintTasksMeassures', data, true, false);
        }

        return true;
    }
    catch (err) {
        alert('getTasksMeassures: ' + err.description);
    }
}

function getStates() {
    try {
        if (jsonStates == false) {
            var data = 't=' + getTokenCookie('ETTK');
            jsonStates = dbReadWrite('getStates', data, true, false);
        }

        return true;
    }
    catch (err) {
        alert('getStates: ' + err.description);
    }
}

// TASKS ROUTINES
//==========================================================================

function addTaskToList(item) {
    try {
        var tbl = document.getElementById('maintTasksTbl');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillTasksRecord(tr, item);
    }
    catch (err) {
        alert('addTaskToList: ' + err.description);
    }
}

function modifyTaskListRecord(id, itm) {
    try {
        var tr = document.getElementById('taskTR' + id);
        removeAllChildNodes(tr);
        fillTasksRecord(tr, itm);
    }
    catch (err) {
        alert('modifyTaskListRecord: ' + err.description);
    }
}

function saveTask() {
    try {
        tips = $(".validateTips");

        var bOk = true;
        var taskMeassureId = $('#cbxNewTaskMeassures').val();

        if (taskMeassureId == '0') {
            alert('Please select a meassure');
            return false;
        }
        else {
            bOk = bOk && checkLength($('#newTaskName'), "Task Name", 3, 20);

            if (bOk == true) {
                var id = $('#newTaskName').attr('data-id');
                var name = $('#newTaskName').val();

                var taskMeassureName = $('#cbxNewTaskMeassures option:selected').text();
                var value = $('#newTaskValue').val();

                var lstUsers = [];
                var isAllUsers = true;
                $('#taskAlertsTBody tr').each(function () {
                    var userId = $(this).attr('data-userId');
                    var isEmail = $(this).find('#chkNewAlertUserEmail' + userId).prop('checked');
                    // var isSMS = $(this).find('#chkNewAlertUserSMS' + userId).prop('checked');
                    var isSMS = false;

                    if (isEmail == true || isSMS == true) {
                        lstUsers.push({ 'id': userId, 'isEmail': isEmail, 'isSMS': isSMS });
                    }
                    if (isEmail == false && isSMS == false) {
                        isAllUsers = false;
                    }
                });
                var jsonUsersTXT = JSON.stringify(lstUsers);

                data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&n=' + escape(name) + '&m=' + escape(taskMeassureId) + '&v=' + escape(value) + '&users=' + escape(jsonUsersTXT);
                var tmpJson = dbReadWrite('saveMaintTask', data, true, false);

                var itm = { 'id': tmpJson.result, 'name': name, 'taskMeassureId': taskMeassureId, 'taskMeassureName': taskMeassureName, 'value': value };

                if (id == '0') {
                    addTaskToList(itm);
                }
                else {
                    modifyTaskListRecord(id, itm);
                }

                return true;
            }
            else {
                return false;
            }
        }
    }
    catch (err) {
        alert('saveTask: ' + err.description);
    }
}

function editTask(obj) {
    try {
        if (validateUserAccess(21) == true) {
            var taskId = $(obj.target).attr('data-id');
            $('#newTaskName').val($('#taskTR' + taskId).attr('data-name'));
            $('#newTaskName').attr('data-id', taskId);

            var taskMeassureId = $('#taskTR' + taskId).attr('data-taskMeassureId');
            var taskMeassureName = $('#taskTR' + taskId).attr('data-taskMeassureName')

            $('#cbxNewTaskMeassures').val(taskMeassureId);
            $('#newTaskValue').val($('#taskTR' + taskId).attr('data-value'));

            setTaskValueField(taskMeassureId, taskMeassureName);

            loadTaskAlertUsers(taskId);


            $('#cbxNewTaskMeassures:not(.changeBound)').addClass('changeBound').change(function () {
                var id = $(this).val();
                var valName = $('#cbxNewTaskMeassures option:selected').text();
                setTaskValueField(id, valName);
            });

            //$('#cbxNewTaskMeassures').change(function () {
            //    var id = $(this).val();
            //    var valName = $('#cbxNewTaskMeassures option:selected').text();
            //    setTaskValueField(id, valName);
            //});

            $("#newTaskDlg").dialog('open')
        }
    }
    catch (err) {
        alert('editTask: ' + err.description);
    }
}

function deleteTaskCommit() {
    try {
        var id = $('#taskRemoveName').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('removeTask', data, true, false);

        var tbl = document.getElementById('maintTasksTbl');
        var tr = document.getElementById('taskTR' + id);
        tbl.removeChild(tr);

        return true;
    }
    catch (err) {
        alert('deleteTaskCommit: ' + err.description);
    }
}

function deleteTask(obj) {
    try {
        if (validateUserAccess(22) == true) {
            var id = $(obj.target).attr('data-id');
            var name = $('#taskTR' + id).attr('data-name');
            $('#taskRemoveName').html(name);
            $('#taskRemoveName').attr('data-id', id);
            $("#taskRemoveDlg").dialog('open')
        }
    }
    catch (err) {
        alert('deleteTask: ' + err.description);
    }
}

function fillTasksRecord(tr, item) {
    try {
        var tbl = document.getElementById('maintTasksTbl');

        $(tr).attr('id', 'taskTR' + item.id);
        $(tr).attr('data-name', item.name);
        $(tr).attr('data-taskMeassureId', item.taskMeassureId);
        $(tr).attr('data-taskMeassureName', item.taskMeassureName);
        $(tr).attr('data-value', item.value);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('alertsListOddTR');
        }

        //name
        var taskNameTd = document.createElement('td');
        $(taskNameTd).html(item.name);
        $(taskNameTd).addClass('alertsListTD');
        tr.appendChild(taskNameTd);

        //taskMeassureName
        var taskMeassureNameTd = document.createElement('td');
        $(taskMeassureNameTd).html(item.taskMeassureName);
        $(taskMeassureNameTd).addClass('alertsListTD');
        tr.appendChild(taskMeassureNameTd);

        //Value
        var valueTd = document.createElement('td');
        $(valueTd).html(item.value + ' ' + item.taskMeassureName);
        $(valueTd).addClass('alertsListTD alertsListRightTD');
        tr.appendChild(valueTd);

        //Edit
        var editTd = document.createElement('td');
        $(editTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(editTd);

        var editBtn = document.createElement('button');
        editTd.appendChild(editBtn);
        $(editBtn).attr('data-id', item.id);
        $(editBtn).click(editTask);

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
        $(delBtn).click(deleteTask);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);

    }
    catch (err) {
        alert('fillTasksRecord: ' + err.description);
    }
}

function clearTasksList() {
    try {
        $("#maintTasksTbl").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearTasksList: ' + err.description);
    }
}

function loadTasks() {
    try {
        //Always get a fresh set of tasks
        getTasks();

        if (jsonTasks) {
            clearTasksList();
            if (jsonTasks) {
                for (var ind = 0; ind < jsonTasks.tasks.length; ind++) {
                    var jsonItem = eval('(' + jsonTasks.tasks[ind] + ')');
                    addTaskToList(jsonItem);
                }
            }
        }
    }
    catch (err) {
        alert('loadTasks: ' + err.description);
    }
}

function setTaskValueField(id, valName) {
    try {

        $('#newTaskDlgValueTR').hide();

        switch (id) {
            case "1": //Miles
                $('#newTaskDlgValueName').html(valName);
                $('#newTaskDlgValueTR').show();
                break;
            case "2": //Engine Hours
                $('#newTaskDlgValueName').html(valName);
                $('#newTaskDlgValueTR').show();
                break;
            case "3": //Time (days)
                $('#newTaskDlgValueName').html(valName);
                $('#newTaskDlgValueTR').show();
                break;
            case "4": //Input 1 (Hours)
                $('#newTaskDlgValueName').html('Hours');
                $('#newTaskDlgValueTR').show();
                break;
            case "5": //Input 2 (Hours)
                $('#newTaskDlgValueName').html('Hours');
                $('#newTaskDlgValueTR').show();
                break;
            case "6": //Input 3 (Hours)
                $('#newTaskDlgValueName').html('Hours');
                $('#newTaskDlgValueTR').show();
                break;
            case "7": //Input 4 (Hours)
                $('#newTaskDlgValueName').html('Hours');
                $('#newTaskDlgValueTR').show();
                break;
        }
    }
    catch (err) {
        alert('setTaskValueField: ' + err.description);
    }
}

function loadTaskAlertUsers(taskId) {
    try {
        getNotificationsSendTo(false, 'maintenanceTasks', taskId);
        var template = _.template($('#userAlertTR-template').html(), { users: jsonNotificationsSendTo });
        $('#taskAlertsTBody').html(template);
    }
    catch (err) {
        alert('loadTaskAlertUsers: ' + err.description);
    }
}

function newMaintenanceTask() {
    try {
        $('#newTaskName').val('');
        $('#newTaskName').attr('data-id', 0);
        $('#newTaskValue').val('');

        $('#cbxNewTaskMeassures:not(.changeBound)').addClass('changeBound').change(function () {
            var id = $(this).val();
            var valName = $('#cbxNewTaskMeassures option:selected').text();
            setTaskValueField(id, valName);
        });

        //$('#cbxNewTaskMeassures').change(function () {
        //    var id = $(this).val();
        //    var valName = $('#cbxNewTaskMeassures option:selected').text();
        //    setTaskValueField(id, valName);

        //});

        loadTaskAlertUsers('');

        $("#newTaskDlg").dialog('open')
    }
    catch (err) {
        alert('newMaintenanceTask: ' + err.description);
    }
}

function loadCbxMeassures() {
    try {
        if (jsonTasksMeassures == false) {
            getTasksMeassures();
        }
        if (jsonTasksMeassures) {
            var cbx = document.getElementById('cbxNewTaskMeassures');
            removeAllChildNodes(cbx);
            loadComboBox(jsonTasksMeassures.meassures, cbx, 'Pick a meassure');
        }
    }
    catch (err) {
        alert('loadCbxMeassures: ' + err.description);
    }
}

function setupNewTaskDlg() {
    try {
        loadCbxMeassures();
        $("#newTaskDlg").dialog({
            height: 400,
            width: 500,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveTask() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving Task.  Please try again.');
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
        alert('setupNewTaskDlg: ' + err.description);
    }
}

function setupTaskRemoveDlg() {
    try {
        $("#taskRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteTaskCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing task.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#taskRemoveName').html('');
            }
        });
    }
    catch (err) {
        alert('setupTaskRemoveDlg: ' + err.description);
    }
}
function sendFeedBack() {
    let response;
    try {
        
        let idType = $("#Type").val();
        let description = $("#comment").val();
        let pageVisited = window.location.href;
        if (description.length < 5) {
            alert("enter a description");
            return;
        }
        response = postSendFeedBack(pageVisited, idType, description);
        if (response.value = "OK") {
            $("#comment").val('');
            alert("FeedBack sent successfully");

        } else {
            alert("error: " + response.value);
        }
        
        var error = ""
    }
    catch (err) {
        alert("error: " + err);
        console.log("error1 " + err);
    }
}
function loadFeedBackType() {
    var response;
    
    try {
        response = GetFeedBackType();
        
        for (var index = 0; index < response.ListResponse.length; index++) {
            $("#Type").append("<option value=" + response.ListResponse[index].ID + ">" + response.ListResponse[index].Name + "</option>");
        }
    }
    catch (err) {
        alert("error: " + err);
    }
}

