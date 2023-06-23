var isDevGroupsDlgReady = false;
var jsonDevGroups = false;

function newDevGroupSelectAll(elemId, val) {
    try {
        $('#' + elemId + ' input:checkbox').each(function () {
            this.checked = val;
        });
    }
    catch (err) {
        alert('newDevGroupSelectAll:' + err.description);
    }
}

function addDevGroupToList(itm) {
    try {
        var tbl = document.getElementById('devGroupsTable');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillDevGroupRecord(tr, item);
    }
    catch (err) {
        alert('addDevGroupToList: ' + err.description);
    }
}

function modifyDevGroupListRecord(id, itm) {
    
    try {
        var tr = document.getElementById('devGroupTR' + id);
        removeAllChildNodes(tr);
        fillDevGroupRecord(tr, itm);
    }
    catch (err) {
        alert('modifyDevGroupListRecord: ' + err.description);
    }
}

function saveDevGroup() {
    try {
        var id = $('#newDevGroupId').attr('data-id');
        var name = $('#newDevGroupName').val();
        var hasSpeedGauge = $('#chkGroupHasSpeedGauge').is(':checked');


        var lstDevices = [];
        var isAllDevices = true;
        $('#newDevGroupDevicesList input:checkbox').each(function () {
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
        $('#newDevGroupUsersList input:checkbox').each(function () {
            if (this.checked == true) {
                lstUsers.push({ 'id': $(this).attr('data-userId') });
            }
            else {
                isAllUsers = false;
            }
        });
        var jsonUsersTXT = JSON.stringify(lstUsers);

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id) + '&name=' + escape(name) + '&isAllDevices=' + escape(isAllDevices) + '&devices=' + escape(jsonDevicesTXT) + '&isAllUsers=' + escape(isAllUsers) + '&users=' + escape(jsonUsersTXT) + '&hasSG=' + escape(hasSpeedGauge);
        var tmpJson = dbReadWrite('saveDeviceGroup', data, true, false);

        if (tmpJson.result != 'failure') {
            var itm = { 'id': tmpJson.result, 'name': name, 'isPublic': true, 'isAllDevices': isAllDevices, 'isAllUsers':isAllUsers, 'hasSpeedGauge': hasSpeedGauge, 'isDefault': false, 'createdOn': 'Today' };
            if (id == '0' || id == '') {
                addDevGroupToList(itm);
            }
            else {
                modifyDevGroupListRecord(id, itm);
            }
        }

        return true;

    }
    catch (err) {
        alert('saveDevGroup: ' + err.description);
    }
}

function newDevGroup() {
    try {
        if (validateUserAccess(35) == true) {
            if (isDevGroupsDlgReady == false) {
                setupNewDevGroupDlg();
                setupDevGroupRemoveDlg();
                isDevGroupsDlgReady = true;
            }

            //The following creates a dependency with devicesSettings.js
            if (jsonDevices == false) {
                getDevices();
            }

            //The following creates a dependency with users.js
            if (jsonUsers == false) {
                getUsers();
            }

            loadDevGroupDlg(1, '0', jsonDevices, jsonUsers);
        }
    }
    catch (err) {
        alert('newIButton: ' + err.description);
    }
}

function editDevGroup(obj) {
    
    try {
        if (validateUserAccess(36) == true) {
            if (isDevGroupsDlgReady == false) {
                setupNewDevGroupDlg();
                setupDevGroupRemoveDlg();
                isDevGroupsDlgReady = true;
            }

            //The following creates a dependency with devicesSettings.js
            if (jsonDevices == false) {
                getDevices();
            }

            //The following creates a dependency with users.js
            if (jsonUsers == false) {
                getUsers();
            }

            var devGroupId = $(obj.target).attr('data-id');
            var data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(devGroupId);
            var jsonDevGroupDevices = dbReadWrite('getDevGroupDevices', data, true, false);
            var jsonDevGroupUsers = dbReadWrite('getDevGroupUsers', data, true, false);

            loadDevGroupDlg(2, devGroupId, jsonDevices, jsonUsers, jsonDevGroupDevices, jsonDevGroupUsers);
        }
    }
    catch (err) {
        alert('editDevGroup: ' + err.description);
    }
}

function deleteDevGroup(obj) {
    try {
        if (validateUserAccess(34) == true) {
            if (isDevGroupsDlgReady == false) {
                setupNewDevGroupDlg();
                setupDevGroupRemoveDlg();
                isDevGroupsDlgReady = true;
            }
            var id = $(obj.target).attr('data-id');
            var name = $('#devGroupTR' + id).attr('data-devGroupName');
            var isDefault = $('#devGroupTR' + id).attr('data-devGroupIsDefault');
            if (isDefault == 'true') {
                alert('The [All Devices] groups cannot be deleted.');
            }
            else {
                $('#devGroupRemoveId').html(name);
                $('#devGroupRemoveId').attr('data-id', id);
                $("#devGroupRemoveDlg").dialog('open')
            }
        }
    }
    catch (err) {
        alert('deleteDevGroup: ' + err.description);
    }
}

function deleteDevGroupCommit() {
    try {
        var id = $('#devGroupRemoveId').attr('data-id');

        data = 't=' + getTokenCookie('ETTK') + '&id=' + escape(id);
        var tmpJson = dbReadWrite('deleteDeviceGroup', data, true, false);

        var tbl = document.getElementById('devGroupsTable');
        var tr = document.getElementById('devGroupTR' + id);
        tbl.removeChild(tr);

        return true;
    }
    catch (err) {
        alert('deleteDevGroupCommit: ' + err.description);
    }
}

function fillDevGroupRecord(tr, item) {
    try {
        var tbl = document.getElementById('devGroupsTable');

        $(tr).attr('id', 'devGroupTR' + item.id);
        $(tr).attr('data-id', item.id);
        $(tr).attr('data-devGroupName', item.name);
        $(tr).attr('data-devGroupIsPublic', item.isPublic);
        $(tr).attr('data-devGroupIsAllDevices', item.isAllDevices);
        $(tr).attr('data-devGroupIsAllUsers', item.isAllUsers);
        $(tr).attr('data-devGroupHasSpeedGauge', item.hasSpeedGauge);
        $(tr).attr('data-devGroupIsDefault', item.isDefault);
        $(tr).attr('data-devGroupCreatedOn', item.createdOn);

        if (tbl.childNodes.length % 2 == 0) {
            $(tr).addClass('alertsListOddTR');
        }

        //iButton name
        var nameTd = document.createElement('td');
        $(nameTd).html(item.name);
        $(nameTd).addClass('alertsListTD');
        tr.appendChild(nameTd);

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

        //IsAllUsers?
        var isAllUsersTd = document.createElement('td');
        if (item.isAllUsers == true) {
            $(isAllUsersTd).html('Yes');
        }
        else {
            $(isAllUsersTd).html('No');
        }
        $(isAllUsersTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(isAllUsersTd);

        //hasSpeedGauge?
        var hasSpeedGaugeTd = document.createElement('td');
        if (item.hasSpeedGauge == true) {
            $(hasSpeedGaugeTd).html('Yes');
        }
        else {
            $(hasSpeedGaugeTd).html('No');
        }
        $(hasSpeedGaugeTd).addClass('alertsListTD alertsListCenteredTD');
        tr.appendChild(hasSpeedGaugeTd);

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
        $(editBtn).click(editDevGroup);

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
        $(delBtn).click(deleteDevGroup);

        var delImg = document.createElement('img');
        $(delImg).attr('src', 'icons/RedCloseX.bmp');
        $(delImg).attr('alt', '');
        $(delImg).attr('width', '16');
        $(delImg).attr('height', '16');
        $(delImg).attr('data-id', item.id);
        delBtn.appendChild(delImg);
    }
    catch (err) {
        alert('fillDevGroupRecord: ' + err.description);
    }
}

function addDevGroupToList(item) {
    try {
        var tbl = document.getElementById('devGroupsTable');
        var tr = document.createElement('tr');
        tbl.appendChild(tr);
        fillDevGroupRecord(tr, item);
    }
    catch (err) {
        alert('addDevGroupToList: ' + err.description);
    }
}

function clearDevGroupsList() {
    try {
        $("#devGroupsTable").find("tr:gt(0)").remove();
    }
    catch (err) {
        alert('clearDevGroupsList: ' + err.description);
    }
}

function getDevGroups() {
    
    try {
        var data = 't=' + getTokenCookie('ETTK');
        jsonDevGroups = dbReadWrite('getDevGroups', data, true, false);

        return true;
    }
    catch (err) {
        alert('getDevGroups: ' + err.description);
    }
}

function loadDevGroups() {
    
    try {
        //Always get a fresh set of dev groups
        getDevGroups();

        if (jsonDevGroups) {
            clearDevGroupsList();
            if (jsonDevGroups) {
                for (var ind = 0; ind < jsonDevGroups.groups.length; ind++) {
                    var jsonItem = eval('(' + jsonDevGroups.groups[ind] + ')');
                    addDevGroupToList(jsonItem);
                }
            }
        }
    }
    catch (err) {
        alert('loadDevGroups: ' + err.description);
    }
}

function loadDevGroupDevices(mode, jsonAllDevices, jsonDevGroupDevices, isDefault) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllDevices) {
            var ul = document.getElementById('newDevGroupDevicesList');
            removeAllChildNodes(ul);

            for (var ind = 0; ind < jsonAllDevices.myDevices.length; ind++) {
                 
                jsonDevice = eval('(' + jsonAllDevices.myDevices[ind] + ')');
                            
                //Add the <li>
                var li = document.createElement('li');
                $(li).attr('id', 'newDevGroupDeviceId' + jsonDevice.deviceId);
                ul.appendChild(li);

                var chk = document.createElement('input');
                $(chk).attr('type', 'checkbox');
                $(chk).attr('id', 'chkNewDevGroupDevice' + jsonDevice.deviceId);
                $(chk).attr('data-deviceId', jsonDevice.deviceId);
                $(chk).attr('style', 'padding:3px;');
                if (isDefault == 'true') {
                    $(chk).attr('disabled', 'disabled');
                }

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonDevGroupDevices.myDevices.length; ind2++) {
                        var jsonDevice2 = eval('(' + jsonDevGroupDevices.myDevices[ind2] + ')');
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
        alert('loadDevGroupDevices: ' + err.description);
    }
}

function loadDevGroupUsers(mode, jsonAllUsers, jsonDevGroupUsers) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        if (jsonAllUsers) {
            var ul = document.getElementById('newDevGroupUsersList');
            removeAllChildNodes(ul);

            for (var ind = 0; ind < jsonAllUsers.users.length; ind++) {
                jsonUser = eval('(' + jsonAllUsers.users[ind] + ')');
                //Add the <li>
                var li = document.createElement('li');
                $(li).attr('id', 'newDevGroupUserId' + jsonUser.id);
                ul.appendChild(li);

                var chk = document.createElement('input');
                $(chk).attr('type', 'checkbox');
                $(chk).attr('id', 'chkNewDevGroupUser' + jsonUser.id);
                $(chk).attr('data-userId', jsonUser.id);
                $(chk).attr('style', 'padding:3px;');

                if (mode == 2) {
                    for (var ind2 = 0; ind2 < jsonDevGroupUsers.users.length; ind2++) {
                        var jsonUser2 = eval('(' + jsonDevGroupUsers.users[ind2] + ')');
                        if (jsonUser.id == jsonUser2.id) {
                            $(chk).prop('checked', 'true');
                            break;
                        }
                    }
                }

                li.appendChild(chk);

                var span = document.createElement('span');
                li.appendChild(span);
                $(span).text(jsonUser.name);
            }
        }
    }
    catch (err) {
        alert('loadDevGroupUsers: ' + err.description);
    }
}

function loadDevGroupDlg(mode, id, jsonAllDevices, jsonAllUsers, jsonDevGroupDevices, jsonDevGroupUsers) {
    //mode:  1 = NEW, 2 = EDIT
    try {
        $('#newDevGroupId').attr('data-id', id);
        var isDefault = 0;

        if (mode == 2) {
            $('#newDevGroupName').val($('#devGroupTR' + id).attr('data-devGroupName'));

            var hasSpeedGauge = $('#devGroupTR' + id).attr('data-devGroupHasSpeedGauge');
            if (hasSpeedGauge == true || hasSpeedGauge == 'true') {
                $('#chkGroupHasSpeedGauge').prop('checked', true);
            }
            else {
                $('#chkGroupHasSpeedGauge').prop('checked', false);
            }

            isDefault = $('#devGroupTR' + id).attr('data-devGroupIsDefault');
            if (isDefault == 'true') {
                $('.newDevGroupDevicesSelectors').hide();
                $('#newDevGroupName').attr('disabled', 'disabled');
            }
            else {
                $('.newDevGroupDevicesSelectors').show();
                $("#newDevGroupName").removeAttr("disabled");
            }
        }
        else {
            $('#chkGroupHasSpeedGauge').prop('checked', false);
        }

        //Load the device

        
        loadDevGroupDevices(mode, jsonAllDevices, jsonDevGroupDevices, isDefault);

        //Load the users
        loadDevGroupUsers(mode, jsonAllUsers, jsonDevGroupUsers);

        $("#newDevGroupDlg").dialog('open');

    }
    catch (err) {
        alert('loadDevGroupDlg: ' + err.description);
    }
}

function setupNewDevGroupDlg() {
    try {
        $("#newDevGroupDlg").dialog({
            height: 500,
            width: 800,
            autoOpen: false,
            modal: true,
            buttons: {
                Save: function () {
                    if (saveDevGroup() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed saving Group.  Please try again.');
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
                $('#newDevGroupId').attr('data-id', '');
                $('#newDevGroupName').val('');
            }
        });
    }
    catch (err) {
        alert('setupNewDevGroupDlg: ' + err.description);
    }
}

function setupDevGroupRemoveDlg() {
    try {
        $("#devGroupRemoveDlg").dialog({
            height: 150,
            width: 320,
            autoOpen: false,
            modal: true,
            buttons: {
                Cancel: function () {
                    $(this).dialog("close");
                },
                Yes: function () {
                    if (deleteDevGroupCommit() == true) {
                        $(this).dialog("close");
                    }
                    else {
                        alert('Failed removing group.  Please try again.');
                    }
                }
            },
            open: function () {
                //Actions to perform upon open
            },
            close: function () {
                //Actions to perform upon close
                $('#devGroupRemoveId').html('');
            }
        });
    }
    catch (err) {
        alert('setupDevGroupRemoveDlg: ' + err.description);
    }
}