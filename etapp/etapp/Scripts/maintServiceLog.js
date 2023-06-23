var isMaintServiceLogReady = false;
var meassureTypeId = 0;
var meassureValueOnDayOfService = 0;

function printServiceSheet() {
    try {
        var data = false;
        var deviceName = $('#cbxRegServiceDevices option:selected').text();
        var serviceDate = $('#dtpRegServiceDate').val();
        var odometer = $('#txtRegServiceOdometer').val();

        var serviceDet = [];
        $('#maintServiceLogTbl tr:gt(0)').each(function () {
            serviceDet.push({ 'serviceType': $(this).attr('data-serviceTypeName'), 'description': $(this).attr('data-serviceDescription'), 'cost': $(this).attr('data-cost'), 'comments': $(this).attr('data-comments') });
        });
        var serviceDetTXT = JSON.stringify(serviceDet);

        data = { 'deviceName': deviceName, 'serviceDate': serviceDate, 'odometer': odometer, 'serviceDet': serviceDetTXT };
        var dataTXT = JSON.stringify(data);

        window.open("print_ServiceSheet.html?data=" + escape(dataTXT), 'popup', 'toolbar = no, status = no, width = 600');
    }
    catch (err) {
        alert('printServiceSheet: ' + err.description);
    }
}

function clearAllServiceLog() {
    try {
        cancelServiceLog();
        $("#maintServiceLogTbl").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearAllServiceLog: ' + err.description);
    }
}

function loadMaintServiceLog() {
    try {
        if (isMaintServiceLogReady == false) {
            if (jsonDevices == false) {
                getDevices();
            }
            if (jsonDevices) {
                var cbx = document.getElementById('cbxRegServiceDevices');
                removeAllChildNodes(cbx);
                loadComboBox(jsonDevices.myDevices, cbx, 'Pick a device');
            }
            if (jsonTasks == false) {
                getTasks();
            }
            if (jsonTasks) {
                var cbxTasks = document.getElementById('cbxRegServiceTasks');
                removeAllChildNodes(cbxTasks);
                loadComboBox(jsonTasks.tasks, cbxTasks, 'Pick a task');
            }
            if (jsonServicesTypes == false) {
                getServicesTypes();
            }
            if (jsonServicesTypes) {
                var cbxTypes = document.getElementById('cbxRegServiceTypes');
                removeAllChildNodes(cbxTypes);
                loadComboBox(jsonServicesTypes.types, cbxTypes, 'Pick a type');
            }

            $('#dtpRegServiceDate').val('');
            $("#dtpRegServiceDate").datepicker();

            setForNewServiceLog();

            isMaintServiceLogReady = true;
        }
    }
    catch (err) {
        alert('loadMaintServiceLogForm: ' + err.description);
    }
}

function setForNewServiceLog() {
    try {
        $('#regServiceTbl').attr('data-id', '0');
        //Mode: 1 - New ; Mode: 2 - Edit
        $('#regServiceTbl').attr('data-mode', 1);
        $('#cbxRegServiceTypes').val('0');
        $('#cbxRegServiceTasks').val('0');
        $('#txtRegServiceDescription').val('');
        $('#txtRegServiceCost').val('');
        $('#txtRegServiceComments').val('');
    }
    catch (err) {
        alert('setForNewServiceLog: ' + err.description);
    }
}

function changeMaintServiceType() {
    try {
        var id = $('#cbxRegServiceTypes').val();

//        $('#regServicePM').hide();
//        $('#regServiceRepair').hide();
//        if (id == 1) {
//            $('#regServicePM').show();
//        }
//        else {
//            $('#regServiceRepair').show();
//        }
    }
    catch (err) {
        alert('changeMaintServiceType: ' + err.description);
    }
}

function changeMaintServiceTask() {
    try {
        meassureTypeId = 0;
        meassureValueOnDayOfService = 0;

        $('#trValueVal').hide();
        $('#trValueTitle').hide();
        $('#tdEngineHours').hide();
        $('#tdInput1').hide();
        $('#tdInput2').hide();
        $('#tdInput3').hide();
        $('#tdInput4').hide();

        var id = $('#cbxRegServiceTasks').val();

        var task = _.find(jsonTasks.tasks, function (obj) {
            var o = JSON.parse(obj);
            return o.id == id;
        });

        if (!_.isUndefined(task)) {
            var t = JSON.parse(task);
            meassureTypeId = t.taskMeassureId;

            switch (meassureTypeId) {
                case 2:
                    $('#trValueTitle').show();
                    $('#trValueVal').show();
                    $('#tdEngineHours').show();
                    break;
                case 4:
                    $('#trValueTitle').show();
                    $('#trValueVal').show();
                    $('#tdInput1').show();
                    break;
                case 5:
                    $('#trValueTitle').show();
                    $('#trValueVal').show();
                    $('#tdInput2').show();
                    break;
                case 6:
                    $('#trValueTitle').show();
                    $('#trValueVal').show();
                    $('#tdInput3').show();
                    break;
                case 7:
                    $('#trValueTitle').show();
                    $('#trValueVal').show();
                    $('#tdInput4').show();
                    break;
            }
        }
    }
    catch (err) {
        alert('changeMaintServiceTask: ' + err.description);
    }
}

function clearAllServiceLogForm() {
    try {
        setForNewServiceLog();
        $('#dtpRegServiceDate').val('');
        $('#txtRegServiceOdometer').val('');
    }
    catch (err) {
        alert('clearAllServiceLogForm: ' + err.description);
    }
}

function editMaintServiceLog(obj) {
    try {
        var id = $(obj.target).attr('data-id');
        $('#regServiceTbl').attr('data-id', id);
        //Mode: 1 - New ; Mode: 2 - Edit
        $('#regServiceTbl').attr('data-mode', 2);
        $('#cbxRegServiceDevices').val($('#serviceLogTR' + id).attr('data-deviceId'));
        $('#dtpRegServiceDate').val($('#serviceLogTR' + id).attr('data-serviceDateString'));
        $('#txtRegServiceOdometer').val($('#serviceLogTR' + id).attr('data-odometer'));
        $('#cbxRegServiceTypes').val($('#serviceLogTR' + id).attr('data-serviceTypeId'));
        $('#cbxRegServiceTasks').val($('#serviceLogTR' + id).attr('data-taskId'));
        $('#txtRegServiceDescription').val($('#serviceLogTR' + id).attr('data-serviceDescription'));
        $('#txtRegServiceCost').val($('#serviceLogTR' + id).attr('data-cost'));
        $('#txtRegServiceComments').val($('#serviceLogTR' + id).attr('data-comments'));

    }
    catch (err) {
        alert('editMaintServiceLog: ' + err.description);
    }
}

function deleteServiceLogCommit() {
    try {
        var id = $('#serviceLogRemoveDlg').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('deleteMaintServiceLog', data, true, false);

        var tbl = document.getElementById('maintServiceLogTbl');
        var tr = document.getElementById('serviceLogTR' + id);
        tbl.removeChild(tr);

        //Special case: the deleted record could also be in the form being edited... check for this.
        if ($('#regServiceTbl').attr('data-id')) {
            var id2 = $('#regServiceTbl').attr('data-id');
            if (id == id2) {
                $('#regServiceTbl').attr('data-id', '0');
                $('#regServiceTbl').attr('data-mode', '1');
            }
        }

        return true;
    }
    catch (err) {
        alert('deleteServiceLogCommit: ' + err.description);
    }
}

function deleteMaintServiceLog(obj) {
    try {
        var id = $(obj.target).attr('data-id');
        $('#serviceLogRemoveDlg').attr('data-id', id);
        $('#serviceLogRemoveDlg').dialog('open'); ;
    }
    catch (err) {
        alert('deleteMaintServiceLog: ' + err.description);
    }
}

function cancelServiceLog() {
    try {
        $('#cbxRegServiceDevices').val('0');
        clearAllServiceLogForm();
    }
    catch (err) {
        alert('cancelServiceLog: ' + err.description);
    }
}

function fillServiceLogRecord(tr, item) {
    try {
        var tbl = document.getElementById('maintServiceLogTbl');

        $(tr).attr('id', 'serviceLogTR' + item.id);
        $(tr).attr('data-id', item.id);
        $(tr).attr('data-deviceId', item.deviceId);
        $(tr).attr('data-deviceName', item.deviceName);
        $(tr).attr('data-serviceTypeId', item.serviceTypeId); //NEW
        $(tr).attr('data-serviceTypeName', item.serviceTypeName); //NEW
        $(tr).attr('data-taskId', item.taskId);
        $(tr).attr('data-serviceDescription', item.serviceDescription); //NEW
        $(tr).attr('data-serviceDate', item.serviceDate);
        $(tr).attr('data-serviceDateString', item.serviceDateString);
        $(tr).attr('data-odometer', item.odometer);
        $(tr).attr('data-cost', item.cost);
        $(tr).attr('data-comments', item.comments);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('maintListOddTR');
        }

        //Device
        var deviceNameTd = document.createElement('td');
        $(deviceNameTd).html(item.deviceName);
        $(deviceNameTd).addClass('maintListTD');
        tr.appendChild(deviceNameTd);

        //Service Type
        var serviceTypeTd = document.createElement('td');
        $(serviceTypeTd).html(item.serviceTypeName);
        $(serviceTypeTd).addClass('maintListTD');
        tr.appendChild(serviceTypeTd);

        //Description
        var descriptionTd = document.createElement('td');
        $(descriptionTd).html(item.serviceDescription);
        $(descriptionTd).addClass('maintListTD');
        tr.appendChild(descriptionTd);

        //Service Date
        var serviceOnTd = document.createElement('td');
        $(serviceOnTd).html(item.serviceDateString);
        $(serviceOnTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(serviceOnTd);

        //Odometer
        var odometerTd = document.createElement('td');
        $(odometerTd).html(item.odometer);
        $(odometerTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(odometerTd);

        //Cost
        var costTd = document.createElement('td');
        $(costTd).html('$' + item.cost);
        $(costTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(costTd);

        //Edit
        var editTd = document.createElement('td');
        $(editTd).addClass('maintListTD maintListCenteredTD');
        tr.appendChild(editTd);

        var editBtn = document.createElement('button');
        editTd.appendChild(editBtn);
        $(editBtn).attr('data-id', item.id);
        $(editBtn).click(editMaintServiceLog);

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
        $(delBtn).click(deleteMaintServiceLog);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);

    }
    catch (err) {
        alert('fillServiceLogRecord: ' + err.description);
    }
}

function addServiceLogToList(itm) {
    try {
        var tbl = document.getElementById('maintServiceLogTbl');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillServiceLogRecord(tr, itm);
    }
    catch (err) {
        alert('addServiceLogToList: ' + err.description);
    }
}

function modifyServiceLogListRecord(id, itm) {
    try {
        var tr = document.getElementById('serviceLogTR' + id);
        removeAllChildNodes(tr);
        fillServiceLogRecord(tr, itm);
    }
    catch (err) {
        alert('modifyServiceLogListRecord: ' + err.description);
    }
}

function saveServiceLog() {
    try {
        var id = $('#regServiceTbl').attr('data-id');
        var mode = $('#regServiceTbl').attr('data-mode');

        var deviceId = $('#cbxRegServiceDevices').val();
        if (deviceId == '0') {
            alert('Please select a device');
            return false;
        }
        deviceName = $('#cbxRegServiceDevices option:selected').text();

        var serviceTypeId = $('#cbxRegServiceTypes').val();
        if (serviceTypeId == '0') {
            alert('Please select a service type');
            return false;
        }
        serviceTypeName = $('#cbxRegServiceTypes option:selected').text();

        var taskId = 0;
        var serviceDescription = '';

        //        if (serviceTypeId == 1) {
        //            taskId = $('#cbxRegServiceTasks').val();
        //            if (taskId == '0') {
        //                alert('Please select a maintenance task');
        //                return false;
        //            }
        //            serviceDescription = $('#cbxRegServiceTasks option:selected').text();
        //            if ($('#txtRegServiceDescription').val() != '') {
        //                serviceDescription = serviceDescription + ' - ' + $('#txtRegServiceDescription').val();
        //            }
        //        }
        //        else {
        //            serviceDescription = $('#txtRegServiceDescription').val();
        //        }

        taskId = $('#cbxRegServiceTasks').val();
        if (taskId != '0') {
            serviceDescription = $('#cbxRegServiceTasks option:selected').text();
        }
        if ($('#txtRegServiceDescription').val() != '') {
            if (serviceDescription != '') {
                serviceDescription = serviceDescription + ' - ';
            }
            serviceDescription = serviceDescription + $('#txtRegServiceDescription').val();
        }

        //
        //

        var serviceDate = $('#dtpRegServiceDate').val();
        var odometer = $('#txtRegServiceOdometer').val();
        if (odometer == '') {
            odometer = 0;
        }
        var cost = $('#txtRegServiceCost').val();
        if (cost == '') {
            cost = 0;
        }
        var comments = $('#txtRegServiceComments').val();

        meassureValueOnDayOfService = $('#dayOfServiceValue').val();


        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&deviceId=' + escape(deviceId) + '&serviceTypeId=' + escape(serviceTypeId) + '&taskId=' + escape(taskId) + '&serviceDescription=' + escape(serviceDescription) + '&serviceDate=' + escape(serviceDate) + '&odometer=' + escape(odometer) + '&cost=' + escape(cost) + '&comments=' + escape(comments) + '&val=' + escape(meassureValueOnDayOfService);
        var tmpJson = dbReadWrite('saveMaintServiceLog', data, true, false);

        if (tmpJson.result != 'failure') {
            var itm = { 'id': tmpJson.result, 'deviceId': deviceId, 'deviceName': deviceName, 'serviceTypeId': serviceTypeId, 'serviceTypeName': serviceTypeName, 'taskId': taskId, 'serviceDescription': serviceDescription, 'serviceDate': serviceDate, 'serviceDateString': serviceDate, 'odometer': odometer, 'cost': cost, 'comments': comments };
            if (id == '0' || id == '') {
                addServiceLogToList(itm);
            }
            else {
                modifyServiceLogListRecord(id, itm);
            }

            //Finally, set for new entry...
            setForNewServiceLog();
        }
        else {
            alert('Failed saving record.  Please try again.  If the problem persists, please contact Support.');
        }
    }
    catch (err) {
        alert('saveServiceLog: ' + err.description);
    }
}

function setupServiceLogRemoveDlg() {
    try {
        $("#serviceLogRemoveDlg").dialog({
            height: 160,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteServiceLogCommit() == true) {
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
        alert('setupServiceLogRemoveDlg: ' + err.description);
    }
}