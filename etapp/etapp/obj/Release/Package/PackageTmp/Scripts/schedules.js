var isSchedulesDlgReady = false;
var jsonSchedules = false;

//-------- LOAD SCHEDULES IN MAIN INTERFACE ------------------------------
function getSchedules() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonSchedules = dbReadWrite('getSchedules', data, true, false);

        return true;
    }
    catch (err) {
        alert('getSchedules: ' + err.description);
    }
}

function clearSchedulesList() {
    try {
        $("#schedulesTable").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearSchedulesList: ' + err.description);
    }
}

function addScheduleToList(item) {
    try {
        var tbl = document.getElementById('schedulesTable');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillScheduleRecord(tr, item);
    }
    catch (err) {
        alert('addScheduleToList: ' + err.description);
    }
}

function fillScheduleRecord(tr, item) {
    try {
        var tbl = document.getElementById('schedulesTable');

        $(tr).attr('id', 'scheduleTR' + item.id);
        $(tr).attr('data-id', item.id);
        $(tr).attr('data-scheduleName', item.name);
        $(tr).attr('data-scheduleCreatedOn', item.createdOn);
        $(tr).attr('data-scheduleHasExceptionDays', item.hasExceptionDays);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('alertsListOddTR');
        }

        //schedule name
        var scheduleNameTd = document.createElement('td');
        $(scheduleNameTd).html(item.name);
        $(scheduleNameTd).addClass('alertsListTD');
        tr.appendChild(scheduleNameTd);

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
        $(editBtn).click(editSchedule);

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
        $(delBtn).click(deleteSchedule);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);

    }
    catch (err) {
        alert('fillScheduleRecord: ' + err.description);
    }
}

function editSchedule(obj) {
    try {
        if (isSchedulesDlgReady == false) {
            setupNewScheduleDlg();
            setupScheduleRemoveDlg();
            isSchedulesDlgReady = true;
        }
        var scheduleId = $(obj.target).attr('data-id');
        var scheduleName = $('#scheduleTR' + scheduleId).attr('data-scheduleName');
        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(scheduleId);
        var jsonScheduleDays = dbReadWrite('getScheduleDays', data, true, false);

        loadSchedulesDlg(2, scheduleId, scheduleName, jsonScheduleDays);

    }
    catch (err) {
        alert('editSchedule: ' + err.description);
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

function deleteSchedule(obj) {
    try {
    }
    catch (err) {
        alert('deleteSchedule: ' + err.description);
    }
}

function loadSchedules() {
    try {
        //Always get a fresh set of schedules
        getSchedules();

        if (jsonSchedules) {
            clearSchedulesList();
            if (jsonSchedules) {
                for (var ind = 0; ind < jsonSchedules.schedules.length; ind++) {
                    var jsonItem = eval('(' + jsonSchedules.schedules[ind] + ')');
                    addScheduleToList(jsonItem);
                }
            }
        }
    }
    catch (err) {
        alert('loadSchedules: ' + err.description);
    }
}

//-------- NEW SCHEDULE

function loadSchedulesDlg(mode, id, scheduleName, scheduleDays) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        $('#scheduleId').attr('data-id', id);
        $("#newScheduleDlg").dialog('open');

        if (mode == 2) {
            $('#scheduleName').val(scheduleName);

            for (var ind = 0; ind < scheduleDays.scheduleDays.length; ind++) {
                var jsonBlock = eval('(' + scheduleDays.scheduleDays[ind] + ')');
                schedSelectBlock(jsonBlock.dayId, jsonBlock.hourFrom, jsonBlock.hourTo);
            }

        }

    }
    catch (err) {
        alert('loadSchedulesDlg: ' + err.description);
    }
}

function newSchedule() {
    try {
        if (isSchedulesDlgReady == false) {
            setupNewScheduleDlg();
            setupScheduleRemoveDlg();
            isSchedulesDlgReady = true;
        }

        loadSchedulesDlg(1, '0');
    }
    catch (err) {
        alert('newSchedule: ' + err.description);
    }
}

function saveSchedule() {
    try {
        //Traverse each day and identify all the blocks of hours of green.
        var hasExceptionDays = false;
        var applyGlobalExceptions = false;

        var hour = 0;
        var startBlock = 0;
        var endBlock = 0;
        var jsonObj = [];

        for (var day = 1; day <= 7; day++) {
            hour = 0;
            startBlock = -1;
            endBlock = -1;
            $("#scheduleConfigTbl tr:gt(0) > td:nth-child(" + (day + 1) + ")").each(function () {
                if ($(this).hasClass('schedConfigSelected')) {
                    if (startBlock == -1) {
                        startBlock = hour;
                        endBlock = -1;
                    }
                }
                else {
                    if (startBlock > -1 && endBlock == -1) {
                        endBlock = hour;
                        jsonObj.push({ "day": day, "from": startBlock, "to": endBlock });
                        startBlock = -1;
                        endBlock = -1;
                    }
                }
                hour += 1;
            });
            if (startBlock > -1) {
                endBlock = hour;
                jsonObj.push({ "day": day, "from": startBlock, "to": endBlock });
            }
        }
        var jsonObjTXT = JSON.stringify(jsonObj);
        var scheduleId = $('#scheduleId').attr('data-id');
        if (scheduleId == undefined) {
            scheduleId = '';
        }
        var scheduleName = $('#scheduleName').val();
        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(scheduleId) + '&scheduleName=' + escape(scheduleName) + '&hasExceptionDays=' + escape(hasExceptionDays) + '&applyGlobalExceptions=' + escape(applyGlobalExceptions) + '&values=' + escape(jsonObjTXT);

        var jsonResult = dbReadWrite('saveSchedule', data, true, false);

        if (jsonResult.result != 'failure') {
            if (scheduleName == '') {
                scheduleName = 'New Schedule';
            }
            var itm = { 'id': jsonResult.result, 'name': scheduleName, 'createdOn': 'Today', 'hasExceptionDays': false};
            if (scheduleId == '0' || scheduleId == '') {
                addScheduleToList(itm);
            }
            else {
                modifyScheduleListRecord(scheduleId, itm);
            }
        }

        return true;
    }
    catch (err) {
        alert('saveSchedule: ' + err.description);
    }
}

function deleteScheduleCommit() {
    try {

    }
    catch (err) {
        alert('deleteScheduleCommit: ' + err.description);
    }
}

//---------- Set up dialogs --------------------------

function setupNewScheduleDlg() {
    try {
        $("#newScheduleDlg").dialog({
            height: 700,
            width: 600,
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
                setupScheduleConfigTbl();
            },
            close: function () {
                //Actions to perform upon close
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
                    if (deleteScheduleCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing Schedule.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#scheduleRemoveName').html('');
            }
        });
    }
    catch (err) {
        alert('setupScheduleRemoveDlg: ' + err.description);
    }
}

//-------- Setup Schedule Configuration Table

function schedSelectBlock(day, hourFrom, hourTo) {
    try {
        $("#scheduleConfigTbl tr:gt(0) > td:nth-child(" + (day + 1) + ")").each(function () {
            if ($(this).attr('data-time') >= hourFrom && $(this).attr('data-time') < hourTo) {
                $(this).removeClass('schedConfigUnselected').addClass('schedConfigSelected');
            }
        });
    }
    catch (err) {
        alert('schedToggleBlock: ' + err.description);
    }
}

function schedToggleAllDay(day) {
    try {

        var selState = $('#scheduleConfigTbl tr:eq(0) > td:eq(' + day + ')').attr('data-selState');
        if (selState == undefined) {
            selState = 1;
        }

        if (selState == 1) {
            $("#scheduleConfigTbl tr:gt(0) > td:nth-child(" + (day + 1) + ")").removeClass('schedConfigUnselected').addClass('schedConfigSelected');
            $('#scheduleConfigTbl tr:eq(0) > td:eq(' + day + ')').attr('data-selState', 2);
        }
        else {
            $("#scheduleConfigTbl tr:gt(0) > td:nth-child(" + (day + 1) + ")").addClass('schedConfigUnselected').removeClass('schedConfigSelected');
            $('#scheduleConfigTbl tr:eq(0) > td:eq(' + day + ')').attr('data-selState', 1);
        }
    }
    catch (err) {
        alert('schedToggleAllDay: ' + err.description);
    }
}

function schedToggleAllHour(obj) {
    try {
        var objTd = null;
        if ($(obj.target).get(0).tagName == 'TD') {
            objTd = obj;
        }
        else {
            objTd = $(obj.target).parent();
        }
        var hour = $(objTd).attr('data-schedHour');
        var selState = $(objTd).attr('data-selState');
        if (selState == undefined) {
            selState = 1;
        }
        var closestTR = $(objTd).closest('tr');
        $(closestTR).find('td:gt(0)').each(function () {
            if (selState == 1) {
                $(this).removeClass('schedConfigUnselected').addClass('schedConfigSelected');
                $(objTd).attr('data-selState', 2);
            }
            else {
                $(this).addClass('schedConfigUnselected').removeClass('schedConfigSelected');
                $(objTd).attr('data-selState', 1);
            }
        });
   }
    catch (err) {
        alert('schedToggleAllHour: ' + err.description);
    }
}

function schedConfigDayToggle(obj) {
    try {
        $(obj.target).toggleClass('schedConfigUnselected').toggleClass('schedConfigSelected');
    }
    catch (err) {
        alert('schedConfigToggle: ' + err.description);
    }
}

function setupScheduleConfigTbl() {
    try {
        var txtTime = '';
        //Clean all after titles
        $("#scheduleConfigTbl").find("tr:gt(0)").remove();

        var tbl = document.getElementById('scheduleConfigTbl');
        for (var ind = 0; ind < 24; ind++) {
            var tr = document.createElement('tr');
            tbl.appendChild(tr);

            //add time cell
            var timeTd = document.createElement('td');

            if (ind == 0) {
                txtTime = '12:00 AM';
            }
            else if (ind < 12) {
                txtTime = ind.toString() + ':00 AM';
            }
            else if (ind == 12) {
                txtTime = '12:00 PM';
            }
            else {
                txtTime = (ind - 12).toString() + ':00 PM';
            }

            $(timeTd).html('<a href="#">' + txtTime + '</a>');
            $(timeTd).addClass('schedConfigTime');
            $(timeTd).attr('data-schedHour', ind);
            $(timeTd).click(schedToggleAllHour);
            tr.appendChild(timeTd);

            //add each day's cell
            for (var j = 0; j < 7; j++) {
                var dayTd = document.createElement('td');
                $(dayTd).addClass('schedConfigDay schedConfigUnselected');
                $(dayTd).attr('data-time', ind);
                $(dayTd).click(schedConfigDayToggle);
                tr.appendChild(dayTd);
            }

        }
    }
    catch (err) {
        alert('setupScheduleConfigTbl: ' + err.description);
    }
}