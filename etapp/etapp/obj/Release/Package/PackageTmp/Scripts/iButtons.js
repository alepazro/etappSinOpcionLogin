var isIButtonDlgReady = false;
var jsonIButtons = false;

function editIButton(obj) {
    try {
        if (isIButtonDlgReady == false) {
            setupNewIButtonDlg();
            setupIButtonRemoveDlg();
            isIButtonDlgReady = true;
        }
        var id = $(obj.target).attr('data-id');
        loadIButtonDlg('2', id)
    }
    catch (err) {
        alert('editIButton: ' + err.description);
    }
}

function deleteIButtonCommit() {
    try {
        var id = $('#iButtonRemoveId').attr('data-id');
        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('removeIButton', data, true, false);

        var tbl = document.getElementById('ibuttonsTable');
        var tr = document.getElementById('iButtonTR' + id);
        tbl.removeChild(tr);

        return true;
    }
    catch (err) {
        alert('deleteIButtonCommit: ' + err.description);
    }
}

function deleteIButton(obj) {
    try {
        if (isIButtonDlgReady == false) {
            setupNewIButtonDlg();
            setupIButtonRemoveDlg();
            isIButtonDlgReady = true;
        }
        if (validateUserAccess(32) == true) {
            var id = $(obj.target).attr('data-id');
            var name = $('#iButtonTR' + id).attr('data-iButtonName');
            $('#iButtonRemoveId').html(name);
            $('#iButtonRemoveId').attr('data-id', id);
            $("#iButtonRemoveDlg").dialog('open')
        }
    }
    catch (err) {
        alert('deleteIButton: ' + err.description);
    }
}

function modifyIButtonListRecord(id, itm) {
    try {
        var tr = document.getElementById('iButtonTR' + id);
        removeAllChildNodes(tr);
        fillIButtonRecord(tr, itm);
    }
    catch (err) {
        alert('modifyIButtonListRecord: ' + err.description);
    }
}

function saveIButton() {
    try {
        var ret = false;
        var id = $('#iButtonId').attr('data-id');
        var name = $('#iButton').val(); ;
        var type = $('#cbxIButtonTypes').val();
        var typeName = $('#cbxIButtonTypes option:selected').text();
        var logoutEventCode = '';
        var assignedToId = $('#cbxIButtonAssignedTo').val();
        var assignedToName = $('#cbxIButtonAssignedTo option:selected').text();

        var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&name=' + escape(name) + '&type=' + escape(type) + '&assignedToId=' + escape(assignedToId);
        var tmpJson = dbReadWrite('saveIButton', data, true, false);

        if (tmpJson == false) {
            alert('Failed saving iButton. Please try again or contact Technical Support.');
            return;
        }

        var itm = { 'id': tmpJson.result, 'iButtonHex': '', 'name': name, 'iButtonType': type, 'iButtonTypeName': typeName, 'logoutEventCode': logoutEventCode, 'createdOn': 'Today', 'assignedToId': assignedToId, 'assignedToName': assignedToName };
        itm.iButtonHex = tmpJson.value;

        if (id == '0') {
            addIButtonToList(itm);
            ret = true;
        }
        else {
            modifyIButtonListRecord(id, itm);
            ret = true;
        }

        return ret;
    }
    catch (err) {
        alert('saveIButton: ' + err.description);
    }
}

function loadIButtonDlg(mode, id) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        $('#iButtonId').attr('data-id', id);

        if (mode == 2) {
            $('#iButton').val($('#iButtonTR' + id).attr('data-iButtonName'));
            $('#cbxIButtonTypes').val($('#iButtonTR' + id).attr('data-iButtonType'));
            $('#cbxIButtonAssignedTo').val($('#iButtonTR' + id).attr('data-assignedToId'));
        }

        $("#newIButtonDlg").dialog('open');

    }
    catch (err) {
        alert('loadIButtonDlg: ' + err.description);
    }
}

function newIButton() {
    try {
        if (isIButtonDlgReady == false) {
            setupNewIButtonDlg();
            setupIButtonRemoveDlg();
            isIButtonDlgReady = true;
        }
        loadIButtonDlg(1, '0');
    }
    catch (err) {
        alert('newIButton: ' + err.description);
    }
}

function fillIButtonRecord(tr, item) {
    try {
        var tbl = document.getElementById('ibuttonsTable');

        if (item.iButtonHex == '') {
            item.iButtonHex = parseInt(item.name).toString(16)
        }
        $(tr).attr('id', 'iButtonTR' + item.id);
        $(tr).attr('data-id', item.id);
        $(tr).attr('data-iButtonHex', item.iButtonHex);
        $(tr).attr('data-iButtonName', item.name);
        $(tr).attr('data-iButtonType', item.iButtonType);
        $(tr).attr('data-iButtonTypeName', item.iButtonTypeName);
        $(tr).attr('data-assignedToId', item.assignedToId);
        $(tr).attr('data-assignedToName', item.assignedToName);
        $(tr).attr('data-iButtonCreatedOn', item.createdOn);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('alertsListOddTR');
        }

        //iButton name
        var nameTd = document.createElement('td');
        $(nameTd).html(item.name);
        $(nameTd).addClass('alertsListTD');
        tr.appendChild(nameTd);

        //iButton HEX
        var hexTd = document.createElement('td');
        $(hexTd).html(item.iButtonHex);
        $(hexTd).addClass('alertsListTD');
        tr.appendChild(hexTd);

        //iButton type
        var typeTd = document.createElement('td');
        $(typeTd).html(item.iButtonTypeName);
        $(typeTd).addClass('alertsListTD');
        tr.appendChild(typeTd);

        //iButton assigned to
        var assignedToTd = document.createElement('td');
        $(assignedToTd).html(item.assignedToName);
        $(assignedToTd).addClass('alertsListTD');
        tr.appendChild(assignedToTd);
        if (item.assignedToName == '') {
            if (item.isUsedWithoutDriver == true) {
                var ibndDiv = document.createElement('div');

                var ibtnNoDriverImg = document.createElement('img');
                $(ibtnNoDriverImg).attr('id', 'ibtnNoDriverImg' + item.id);
                $(ibtnNoDriverImg).attr('src', 'img/alert_16.png');
                $(ibtnNoDriverImg).attr('alt', 'iButton with no driver assigned');
                $(ibtnNoDriverImg).attr('width', '16');
                $(ibtnNoDriverImg).attr('height', '16');
                $(ibtnNoDriverImg).attr('title', 'iButton has been linked to a vehicle but no driver is assigned to this iButton.');
                $(ibtnNoDriverImg).attr('style', 'width:16px; height:16px;vertical-align: middle;');
                ibndDiv.appendChild(ibtnNoDriverImg);

                var ibnd = document.createElement('span');
                $(ibnd).text('iButton is being used but has no driver assigned!');
                ibndDiv.appendChild(ibnd);

                assignedToTd.appendChild(ibndDiv);
            }
        }

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
        $(editBtn).click(editIButton);

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
        $(delBtn).click(deleteIButton);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);
    }
    catch (err) {
        alert('fillIButtonRecord: ' + err.description);
    }
}

function addIButtonToList(item) {
    try {
        var tbl = document.getElementById('ibuttonsTable');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillIButtonRecord(tr, item);
    }
    catch (err) {
        alert('addIButtonToList: ' + err.description);
    }
}

function clearIButtonsList() {
    try {
        $("#ibuttonsTable").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearIButtonsList: ' + err.description);
    }
}

function getIButtons() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonIButtons = dbReadWrite('getIButtons', data, true, false);

        return true;
    }
    catch (err) {
        alert('getIButtons: ' + err.description);
    }
}

function loadIButtons() {
    try {
        //Always get a fresh set of schedules
        getIButtons();

        if (jsonIButtons) {
            clearIButtonsList();
            if (jsonIButtons) {
                for (var ind = 0; ind < jsonIButtons.ibuttons.length; ind++) {
                    var jsonItem = eval('(' + jsonIButtons.ibuttons[ind] + ')');
                    addIButtonToList(jsonItem);
                }
            }
        }
    }
    catch (err) {
        alert('loadIButtons: ' + err.description);
    }
}

function loadIButtonUsers() {
    try {
        if (jsonUsers == false) {
            getUsers();
        }
        if (jsonUsers) {
            var cbx = document.getElementById('cbxIButtonAssignedTo');
            loadComboBox(jsonUsers.users, cbx, 'Not Assigned');
        }
    }
    catch (err) {
        alert('loadIButtonUsers: ' + err.description);
    }
}

function setupNewIButtonDlg() {
    try {
        loadIButtonUsers();
        $("#newIButtonDlg").dialog({
            height: 300,
            width: 300,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveIButton() == true) {
                        jsonUsers = false;
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving iButton.  Please try again.');
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
                $('#iButtonId').attr('data-id', '');
                $('#iButton').val('');
                $('#cbxIButtonTypes').val('0');
                $('#cbxIButtonAssignedTo').val('0');
            }
        });
    }
    catch (err) {
        alert('setupNewIButtonDlg: ' + err.description);
    }
}

function setupIButtonRemoveDlg() {
    try {
        $("#iButtonRemoveDlg").dialog({
            height: 220,
            width: 320,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteIButtonCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing iButton.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#iButtonRemoveId').html('');
            }
        });
    }
    catch (err) {
        alert('setupIButtonRemoveDlg: ' + err.description);
    }
}