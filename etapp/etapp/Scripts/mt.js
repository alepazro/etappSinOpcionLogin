//Multi-Tracking
var map1 = false;
var map2 = false;
var map3 = false;
var map4 = false;
var markersMap1 = [];
var markersMap2 = [];
var markersMap3 = [];
var markersMap4 = [];
var list1 = [];
var list2 = [];
var list3 = [];
var list4 = [];
var boolpanel1 = false;
var boolpanel2 = false;
var boolpanel3 = false;
var boolpanel4 = false;
var arrayGroupsSelect = [];

var globalSelectedGroupId = 0;

function updateMT() {
    try {
        getDevices();

        if (jsonDevices) {
            if (jsonDevices.myDevices) {

                var panelId = 0;
                var marker = false;
                var bounds1 = new google.maps.LatLngBounds();
                var bounds2 = new google.maps.LatLngBounds();
                var bounds3 = new google.maps.LatLngBounds();
                var bounds4 = new google.maps.LatLngBounds();

                for (var ind = 0; ind < jsonDevices.myDevices.length; ind++) {
                    jsonDevice = eval('(' + jsonDevices.myDevices[ind] + ')');
                    $('.mapDevicesList li[data-deviceId="' + jsonDevice.deviceId + '"]').each(function () {
                        
                        $(this).attr('style', 'background-color:' + eventColor(jsonDevice.eventCode));

                        panelId = $(this).attr('data-panelId');

                        var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
                        var content = jsonDevice.infoTable;

                        switch (panelId) {
                            case "1":
                                marker = new google.maps.Marker({ position: devLatLng, map: map1, title: jsonDevice.name, icon: jsonDevice.iconUrl });
                                markersMap1[jsonDevice.deviceId] = marker;

                                (function (marker, content) {
                                    google.maps.event.addListener(marker, 'click', function () {
                                        if (!infowindow) {
                                            infowindow = new google.maps.InfoWindow();
                                        }
                                        infowindow.setContent(content);
                                        infowindow.open(map1, marker);
                                    });
                                })(marker, content);

                                bounds1.extend(devLatLng);
                                marker.setMap(map1);

                                break;
                            case "2":
                                marker = new google.maps.Marker({ position: devLatLng, map: map2, title: jsonDevice.name, icon: jsonDevice.iconUrl });
                                markersMap2[jsonDevice.deviceId] = marker;

                                (function (marker, content) {
                                    google.maps.event.addListener(marker, 'click', function () {
                                        if (!infowindow) {
                                            infowindow = new google.maps.InfoWindow();
                                        }
                                        infowindow.setContent(content);
                                        infowindow.open(map2, marker);
                                    });
                                })(marker, content);

                                bounds2.extend(devLatLng);
                                marker.setMap(map2);

                                break;
                            case "3":
                                marker = new google.maps.Marker({ position: devLatLng, map: map3, title: jsonDevice.name, icon: jsonDevice.iconUrl });
                                markersMap3[jsonDevice.deviceId] = marker;

                                (function (marker, content) {
                                    google.maps.event.addListener(marker, 'click', function () {
                                        if (!infowindow) {
                                            infowindow = new google.maps.InfoWindow();
                                        }
                                        infowindow.setContent(content);
                                        infowindow.open(map3, marker);
                                    });
                                })(marker, content);

                                bounds3.extend(devLatLng);
                                marker.setMap(map3);

                                break;
                            case "4":
                                marker = new google.maps.Marker({ position: devLatLng, map: map4, title: jsonDevice.name, icon: jsonDevice.iconUrl });
                                markersMap4[jsonDevice.deviceId] = marker;

                                (function (marker, content) {
                                    google.maps.event.addListener(marker, 'click', function () {
                                        if (!infowindow) {
                                            infowindow = new google.maps.InfoWindow();
                                        }
                                        infowindow.setContent(content);
                                        infowindow.open(map4, marker);
                                    });
                                })(marker, content);

                                bounds4.extend(devLatLng);
                                marker.setMap(map4);

                                break;
                        }

                    });
                }
                map1.fitBounds(bounds1);
                google.maps.event.addListenerOnce(map1, 'idle', function () {
                    if (map1.getZoom() > 16) {
                        map1.setZoom(16);
                    }
                });

                map2.fitBounds(bounds2);
                google.maps.event.addListenerOnce(map2, 'idle', function () {
                    if (map2.getZoom() > 16) {
                        map2.setZoom(16);
                    }
                });

                map3.fitBounds(bounds3);
                google.maps.event.addListenerOnce(map3, 'idle', function () {
                    if (map3.getZoom() > 16) {
                        map3.setZoom(16);
                    }
                });

                map4.fitBounds(bounds4);
                google.maps.event.addListenerOnce(map4, 'idle', function () {
                    if (map4.getZoom() > 16) {
                        map4.setZoom(16);
                    }
                });
            }
        }

    }
    catch (err) {
        alert('updateMT: ' + err.description);
    }
}

function openMTDeviceInfoWindow(panelId, deviceId) {
    try {
        var jsonDevice = getDeviceFromJson(deviceId);
        var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
        infowindow.setContent(jsonDevice.infoTable);
        infowindow.setPosition(devLatLng);

        switch (panelId) {
            case 1:
                infowindow.open(map1);
                break;
            case 2:
                infowindow.open(map2);
                break;
            case 3:
                infowindow.open(map3);
                break;
            case 4:
                infowindow.open(map4);
                break;
        }
    }
    catch (err) {
        alert('openMTDeviceInfoWindow: ' + err.descripion);
    }
}

function loadGroupDetInPanel(panelId, groupId, groupName, deviceId, deviceName) {
    try {
        var ul = document.getElementById('map' + panelId.toString() + 'UL');
        var li = document.createElement('li');
        ul.appendChild(li);
        $(li).attr('data-deviceId', deviceId).attr('data-panelId', panelId);

        var div = document.createElement('div');
        li.appendChild(div);
        $(div).addClass('groupItemName');
        $(div).attr("onclick", "openMTDeviceInfoWindow(" + panelId + "," + deviceId + ")");

        var span = document.createElement('span');
        div.appendChild(span);
        $(span).html(deviceName);

        eval("list" + panelId.toString() + ".push({ 'deviceId': deviceId, 'deviceName': deviceName })");
    }
    catch (err) {
        alert('loadGroupDetInPanel: ' + err.description);
    }
}

function getMTGroups() {
    try {
        var data = 't=' + getTokenCookie('ETTK');
        var jsonResult = dbReadWrite('getMTGroupsByPanel', data, true, false);
        if (jsonResult) {
            if (jsonResult.groups) {
                //Clear all groups
                for (var i = 1; i <= 4; i++) {
                    var ul = document.getElementById('map' + i.toString() + 'UL');
                    removeAllChildNodes(ul);
                }
                for (var ind = 0; ind < jsonResult.groups.length; ind++) {
                    var jsonGroup = eval('(' + jsonResult.groups[ind] + ')');
                    $('#group' + jsonGroup.panelId).html(jsonGroup.name).attr('data-groupId', jsonGroup.id).attr('data-groupName', jsonGroup.name);
                    loadGroupDetInPanel(jsonGroup.panelId, jsonGroup.id, jsonGroup.name, jsonGroup.deviceId, jsonGroup.deviceName);
                }
            }
        }
    }
    catch (err) {
        alert('getMTGroups' + err.description);
    }
}

function editGroup(groupId) {
    try {
        $("#groupBuilderDlg1").dialog('open')
    }
    catch (err) {
        alert('editGroup: ' + err.description);
    }
}

function isDeviceInList(deviceId) {
    try {
        var list = false;
        var isInList = false;
        switch (globalSelectedGroupId) {
            case 1:
                list = list1;
                break;
            case 2:
                list = list2;
                break;
            case 3:
                list = list3;
                break;
            case 4:
                list = list4;
                break;
        }
        if (list) {
            for (var ind = 0; ind < list.length; ind++) {
                if (list[ind].deviceId == deviceId) {
                    isInList = true;
                    break;
                }
            }
        }

        return isInList;
    }
    catch (err) {
        alert('findDeviceInArray' + err.description);
    }
}

function loadGroupList() {
    try {
        if (globalSelectedGroupId > 0) {
            var jsonDevice = false;

            if ($('#group' + globalSelectedGroupId.toString()).attr('data-groupId')) {
                $('#groupName').attr('data-groupId', $('#group' + globalSelectedGroupId.toString()).attr('data-groupId'));
            }
            else {
                $('#groupName').attr('data-groupId', '');
            }

            if ($('#group' + globalSelectedGroupId.toString()).attr('data-groupName')) {
                $('#groupName').val($('#group' + globalSelectedGroupId.toString()).attr('data-groupName'));
            }
            else {
                $('#groupName').val('Group ' + globalSelectedGroupId.toString());
            }

            var ul = document.getElementById('groupDevicesList');
            removeAllChildNodes(ul);
            for (var ind = 0; ind < jsonDevices.myDevices.length; ind++) {
                jsonDevice = eval('(' + jsonDevices.myDevices[ind] + ')');

                var li = document.createElement('li');
                ul.appendChild(li);

                var div = document.createElement('div');
                li.appendChild(div);

                var chk = document.createElement('input');
                $(chk).attr('id', 'chk' + jsonDevice.id);
                $(chk).attr('data-id', jsonDevice.id);
                $(chk).attr('data-name', jsonDevice.name);
                $(chk).attr('type', 'checkbox');
                div.appendChild(chk);

                //If the device is in the map, check it
                if (isDeviceInList(jsonDevice.id)) {
                    $(chk).prop("checked", true);
                }

                var span = document.createElement('span');
                div.appendChild(span);
                $(span).attr('id', 'dev' + jsonDevice.id);
                $(span).html(jsonDevice.name);
                $(span).addClass('groupItem');
            }
        }
    }
    catch (err) {
        alert('loadGroupList: ' + err.description);
    }
}

function saveGroupList() {
    try {
        var isOk = true;
        var i = 0;
        var tmpList = [];
        var groupId = '';
        var groupName = '';

        $("#groupDevicesList input:checkbox").each(function () {
            if (this.checked) {
                i += 1;
                var deviceId = $(this).attr('data-id');
                var deviceName = $(this).attr('data-name');
                tmpList.push({ 'deviceId': deviceId, 'deviceName': deviceName });
            }
        });
        if (i > 0) {
            groupId = $('#groupName').attr('data-groupId');
            groupName = $('#groupName').val();
            if (groupName.length == 0) {
                groupName = 'Group ' + globalSelectedGroupId.toString();
            }
            var devices = JSON.stringify(tmpList);
            var data = 't=' + getTokenCookie('ETTK') + '&p=' + escape(globalSelectedGroupId.toString()) + '&id=' + escape(groupId) + '&name=' + escape(groupName) + '&devices=' + escape(devices);
            var jsonResult = dbReadWrite('saveGroup', data, true, false);

            if (jsonResult.result == 'true') {
                groupId = jsonResult.value;
                $('#group' + globalSelectedGroupId.toString()).attr('data-groupId', groupId).attr('data-groupName', groupName).html(groupName);

                eval("list" + globalSelectedGroupId.toString() + ".length=0");

                var ul = document.getElementById('map' + globalSelectedGroupId.toString() + 'UL');
                removeAllChildNodes(ul);
                for (var ind = 0; ind < tmpList.length; ind++) {
                    loadGroupDetInPanel(globalSelectedGroupId, groupId, groupName, tmpList[ind].deviceId, tmpList[ind].deviceName);
                }
            }
            else {
                isOk = false;
            }
        }

        return isOk;

    }
    catch (err) {
        isOk = false;
        alert('saveGroupList: ' + err.description);
    }
}

function setupGroupDlg() {
    try {
        $("#groupBuilderDlg1").dialog({
            height: 500,
            width: 400,
            autoOpen: false,
            modal: true//,
            //buttons: {
            //    Save: function () {
            //        if (saveGroupList() == true) {
            //            $(this).dialog("close");
            //        }
            //        else {
            //            alert('Failed saving group.  Please try again.');
            //        }
            //    },
            //    Cancel: function () {
            //        $(this).dialog("close");
            //    }
            //},
            //open: function () {
            //    //Actions to perform upon open
            //    loadGroupList();
            //},
            //close: function () {
            //    //Actions to perform upon close
            //}
        });
    }
    catch (err) {
        alert('setupGroupDlg: ' + err.description);
    }
}

function setMapsHeights() {
    try {
        var h2 = ((h - 90) / 2) - 1;
        var h3 = h2 - 15;

        $(document.getElementById('box1')).attr('style', 'height:' + h2 + 'px;');
        $(document.getElementById('box2')).attr('style', 'height:' + h2 + 'px;');
        $(document.getElementById('box3')).attr('style', 'height:' + h2 + 'px;');
        $(document.getElementById('box4')).attr('style', 'height:' + h2 + 'px;');

        $(document.getElementById('map1')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map1Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map1Main')).attr('style', 'height:' + h3 + 'px;');

        $(document.getElementById('map2')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map2Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map2Main')).attr('style', 'height:' + h3 + 'px;');

        $(document.getElementById('map3')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map3Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map3Main')).attr('style', 'height:' + h3 + 'px;');

        $(document.getElementById('map4')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map4Side')).attr('style', 'height:' + h3 + 'px;');
        $(document.getElementById('map4Main')).attr('style', 'height:' + h3 + 'px;');
    }
    catch (err) {
        alert('setMapsHeights: ' + err.description);
    }
}

function initializeMTMaps() {
    try {
        var bounds = false;
        var cntr = false;
        var sw = false;
        var ne = false;

        //Center in the entire USA
        sw = new google.maps.LatLng(25, -123.20);
        ne = new google.maps.LatLng(43, -75.20);

        // Create a bounding box
        bounds = new google.maps.LatLngBounds(sw, ne);
        cntr = bounds.getCenter();

        var myOptions = {
            zoom: 4,
            center: cntr,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }

        map1 = new google.maps.Map(document.getElementById("map1"), myOptions);
        map1.fitBounds(bounds);

        map2 = new google.maps.Map(document.getElementById("map2"), myOptions);
        map2.fitBounds(bounds);

        map3 = new google.maps.Map(document.getElementById("map3"), myOptions);
        map3.fitBounds(bounds);

        map4 = new google.maps.Map(document.getElementById("map4"), myOptions);
        map4.fitBounds(bounds);

        infowindow = new google.maps.InfoWindow();
    }
    catch (err) {
        alert('initializeMTMaps: ' + err.description);
    }
}

function initializeMT() {
    try {
        initializeMTMaps();
        //getMTGroups();
        //updateMT();
    }
    catch (err) {
        alert('initializeMT: ' + err.description);
    }
}

function updateMTDeviceInLists(panelId, jsonDevice) {
    try {
        $('.mapDevicesList li[data-deviceId="' + jsonDevice.deviceId + '"]').each(function () {
            $(this).attr('style', 'background-color:' + eventColor(jsonDevice.eventCode));
        });
    }
    catch (err) {
        alert('updateMTDeviceInLists: ' + err.description);
    }
}

function updateMTDeviceMarkerPosition(panelId, jsonDevice) {
    try {
        var content = jsonDevice.infoTable;

        switch (panelId) {
            case "1":
                if (markersMap1[jsonDevice.deviceId]) {
                    var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
                    markersMap1[jsonDevice.deviceId].setPosition(devLatLng);
                    markersMap1[jsonDevice.deviceId].setIcon(jsonDevice.iconUrl);

                    (function (marker, content) {
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }
                            infowindow.setContent(content);
                            infowindow.open(map1, marker);
                        });
                    })(markersMap1[jsonDevice.deviceId], content);
                }
                break;
            case "2":
                if (markersMap2[jsonDevice.deviceId]) {
                    var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
                    markersMap2[jsonDevice.deviceId].setPosition(devLatLng);
                    markersMap2[jsonDevice.deviceId].setIcon(jsonDevice.iconUrl);

                    (function (marker, content) {
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }
                            infowindow.setContent(content);
                            infowindow.open(map1, marker);
                        });
                    })(markersMap2[jsonDevice.deviceId], content);
                }
                break;
            case "3":
                if (markersMap3[jsonDevice.deviceId]) {
                    var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
                    markersMap3[jsonDevice.deviceId].setPosition(devLatLng);
                    markersMap3[jsonDevice.deviceId].setIcon(jsonDevice.iconUrl);

                    (function (marker, content) {
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }
                            infowindow.setContent(content);
                            infowindow.open(map1, marker);
                        });
                    })(markersMap3[jsonDevice.deviceId], content);
                }
                break;
            case "4":
                if (markersMap4[jsonDevice.deviceId]) {
                    var devLatLng = new google.maps.LatLng(jsonDevice.latitude, jsonDevice.longitude);
                    markersMap4[jsonDevice.deviceId].setPosition(devLatLng);
                    markersMap4[jsonDevice.deviceId].setIcon(jsonDevice.iconUrl);

                    (function (marker, content) {
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }
                            infowindow.setContent(content);
                            infowindow.open(map1, marker);
                        });
                    })(markersMap4[jsonDevice.deviceId], content);
                }
                break;
        }
    }
    catch (err) {
        alert('updateMTDeviceMarkerPosition: ' + err.description);
    }
}

function getMTLastKnowLocation() {
    try {
        getDevices();
        
        
        if (jsonDevices) {
            
            if (jsonDevices.myDevices) {
                var panelId = 0;
                for (var ind = 0; ind < jsonDevices.myDevices.length; ind++) {
                    jsonDevice = eval('(' + jsonDevices.myDevices[ind] + ')');

                    $('.mapDevicesList li[data-deviceId="' + jsonDevice.deviceId + '"]').each(function () {
                        
                        panelId = $(this).attr('data-panelId');
                        updateMTDeviceInLists(panelId, jsonDevice);
                        updateMTDeviceMarkerPosition(panelId, jsonDevice);
                    });
                }
                MTAutoCenter();
            }
        }
    }
    catch (error) {
        alert('getMTLastKnowLocation: ' + error.description);
        console.log(error);
    }
}

function MTAutoCenter() {
    try {
        var bounds1 = new google.maps.LatLngBounds();
        for (i in markersMap1) {
            if (markersMap1[i].map != null) {
                bounds1.extend(markersMap1[i].position);
            }
        }
        map1.fitBounds(bounds1);
        google.maps.event.addListenerOnce(map1, 'idle', function () {
            if (map1.getZoom() > 16) {
                map1.setZoom(16);
            }
        });

        var bounds2 = new google.maps.LatLngBounds();
        for (i in markersMap2) {
            if (markersMap2[i].map != null) {
                bounds2.extend(markersMap2[i].position);
            }
        }
        map2.fitBounds(bounds2);
        google.maps.event.addListenerOnce(map2, 'idle', function () {
            if (map2.getZoom() > 16) {
                map2.setZoom(16);
            }
        });

        var bounds3 = new google.maps.LatLngBounds();
        for (i in markersMap3) {
            if (markersMap3[i].map != null) {
                bounds3.extend(markersMap3[i].position);
            }
        }
        map3.fitBounds(bounds3);
        google.maps.event.addListenerOnce(map3, 'idle', function () {
            if (map3.getZoom() > 16) {
                map3.setZoom(16);
            }
        });

        var bounds4 = new google.maps.LatLngBounds();
        for (i in markersMap4) {
            if (markersMap4[i].map != null) {
                bounds4.extend(markersMap4[i].position);
            }
        }
        map4.fitBounds(bounds4);
        google.maps.event.addListenerOnce(map4, 'idle', function () {
            if (map4.getZoom() > 16) {
                map4.setZoom(16);
            }
        });

    }
    catch (err) {
        alert('autoCenter: ' + err.Description);
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
function getMTLastKnowLocationNew() {//original function getMTLastKnowLocation()
    try {
        /*getDevices();*/
        getDevicesGroupNew(1,"");
        /*var group = jsonDevicesGroupsNew.groups;*/
        //var devices = jsonDevicesGroupsNew.myDevices;
        
        createGroupsDivs(jsonDevicesGroupsNew.groups);
        loadPreferences(jsonDevicesGroupsNew.groups);
        
    }
    catch (error) {
        alert('getMTLastKnowLocation: ' + error.description);
        console.log(error);
    }
}
function createGroupsDivs(groups) {
    var jsongroup = null;  
    
    $("#groupBuilderDlg1").empty();
    var div = document.getElementById('groupBuilderDlg1');   
    var ul = document.createElement('ul');
    //$(ul).addClass("list-group");
    var li;
    var chk1; 
    var span; 
    var table = document.createElement('table');
    $(table).addClass("table table-hover");
    var tr;
    var td;
    var trtitle = document.createElement('tr');
    var trtitlebtn = document.createElement('tr');
    var tdtitle = document.createElement('td');
    var tdtitle2 = document.createElement('td');
    $(tdtitle).text("Select Group");
    $(tdtitle).attr('style', 'font-weight:bold');
    $(tdtitle2).text("Assign To Map");
    $(tdtitle2).attr('style', 'font-weight:bold');
    $(trtitle).append(tdtitle);
    $(trtitle).append(tdtitle2);
    var btn = document.createElement('button');
    $(btn).attr("onclick", "savePreferences(2)");
    $(btn).attr("style", "p");
    $(btn).addClass("btn btn-outline-primary");
    $(btn).text("save preferences");
    $(trtitlebtn).append(btn);
    table.appendChild(trtitlebtn);
    table.appendChild(trtitle);
    for (var ind = 0; ind < groups.length; ind++) {
        
        jsongroup = eval('(' + groups[ind] + ')');
        if (!jsongroup.isdefault) {
            tr = document.createElement('tr');
            td = document.createElement('td');
            td1 = document.createElement('td');
            select = document.createElement('select');
            $(select).addClass('form-select');
            $(select).attr('disabled','disabled');
            $(select).attr('onchange', 'selectionPanel(this)');
            $(select).attr('id', 'selectgroup' + jsongroup.id);
            $(select).attr('data-group',jsongroup.id);
            $(select).append('<option value=0 selected>Select One..</option>');
            $(select).append('<option value=1>Map 1</option>');
            $(select).append('<option value=2>Map 2</option>');
            $(select).append('<option value=3>Map 3</option>');
            $(select).append('<option value=4>Map 4</option>');
            $(select).addClass('selectgroup')             

            span = document.createElement('span');
            $(span).text(jsongroup.name);
            chk1 = document.createElement('input');
            $(chk1).attr('type', 'checkbox');
            $(chk1).attr('id', 'chkgroup' + jsongroup.id);
            $(chk1).addClass("chkgroup" + jsongroup.id);
            $(chk1).addClass("mtchkgroup");
            $(chk1).attr('style', 'margin:3px');
            //$(chk1).attr('value', 'val' + jsongroup.id);
            $(chk1).attr("onclick", "SelectAllDevices('" + jsongroup.id + "','" + jsongroup.name + "','" + jsongroup.map + "','" + jsongroup.showgroup+"')");//from individual group 
            $(chk1).prop("checked", jsongroup.showgroup);                   
            td.appendChild(chk1);
            td.appendChild(span);
            td1.appendChild(select);
            tr.appendChild(td);
            tr.appendChild(td1);           
            table.appendChild(tr);            
        }        
    }    
    div.appendChild(table);    
}
function SelectAllDevices(groupId, groupName, map, showgroup) {
    
    var countItems = $('.mtchkgroup:checked').size();    
    if (countItems <= 4) {
        
        if ($("#chkgroup" + groupId).prop('checked')) {            
            addGroupArray(groupId, groupName, -1, 1,2);
            $('#selectgroup' + groupId).prop('disabled', false);
        }else{            
            deletegroupfromarray(groupId, groupName, globalSelectedGroupId);
            $("#selectgroup" + groupId).val("0");
            $('#selectgroup' + groupId).prop('disabled', true);
            $(".mt" + map).remove();
            switch (map) {
                case "1":
                    if (markersMap1) {
                        for (i in markersMap1) {
                            markersMap1[i].setMap(null);
                        }
                    }
                    break;
                case "2":
                    if (markersMap2) {
                        for (i in markersMap2) {
                            markersMap2[i].setMap(null);
                        }
                    }
                    break;
                case "3":
                    if (markersMap3) {
                        for (i in markersMap3) {
                            markersMap3[i].setMap(null);
                        }
                    }
                    break;
                case "4":
                    if (markersMap4) {
                        for (i in markersMap4) {
                            markersMap4[i].setMap(null);
                        }
                    }
                    break;
            }
            
        }       
    }else{
        $("#chkgroup" + groupId).prop("checked", false);
        toastr.warning("Warning",'You can only select up to 4 groups');
    }
   
}
function createGroupsDevicesDivs(array,div,panel) {
    var jsongroup = null;
    //var div = document.getElementById('groupBuilderDlg1');
    //var ul = document.createElement('ul');
    //$(ul).addClass("list-group");
    //var li;
    var chk1;
    var span;
    var table = document.createElement('table');
    $(table).addClass("table table-hover");
    var img;
    var tr;
    var td;    
    for (var ind = 0; ind < array.length; ind++) {
        jsonDevice = array[ind];/* eval('(' + array[ind] + ')');*/
        
        tr = document.createElement('tr');
        $(tr).attr('style', 'background-color:' + eventColor(jsonDevice.eventCode));
        td = document.createElement('td');
        span = document.createElement('span');
        $(span).text(jsonDevice.deviceId);  
        img = document.createElement('img');
        $(img).attr('src', jsonDevice.iconUrl);
        $(img).attr('width', '25');
        $(img).attr('height', '25');
        //chk1 = document.createElement('input');
        //$(chk1).attr('type', 'checkbox');
        //$(chk1).attr('id', 'chkdevice' + jsonDevice.deviceId);
        //$(chk1).addClass("chkdevice" + jsonDevice.deviceId);
        //$(chk1).addClass("mt" + panel);
        //$(chk1).attr('style', 'margin:3px');
        ////$(chk1).attr('value', 'val' + jsongroup.id);
        ////$(chk1).attr("onclick", "SelectAllDevices('" + jsongroup.id + "')");//from individual group 
        //$(chk1).prop("checked", jsonDevice.showDevice);
        //td.appendChild(chk1);
        td.appendChild(span);
        td.appendChild(img);
        tr.appendChild(td);        
        table.appendChild(tr);
        
    }
    $(table).addClass("mt" + panel);
    $(table).addClass("mtglobal");
    div.appendChild(table);

}
function updateMTNew(array = [], panel) {
    try {
        //getDevices();        
        var marker = false;
        var bounds1 = new google.maps.LatLngBounds();
        var devLatLng = null;
        var content = null;
        array.forEach(function(object) {
                   
            content = object.infoTable;
            switch (panel) {
                case "1":
                    
                    devLatLng = new google.maps.LatLng(object.latitude, object.longitude);
                    marker = new google.maps.Marker({ position: devLatLng, map: map1, title: object.name, icon: object.iconUrl });
                    markersMap1[object.deviceId] = marker;
                    (function (marker, content) {
                        
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }
                            
                            infowindow.setContent(content);
                            infowindow.open(map1, marker);
                        });
                    })(marker, content);
                    bounds1.extend(devLatLng);
                    marker.setMap(map1);
                    break;
                case "2":
                    
                    devLatLng = new google.maps.LatLng(object.latitude, object.longitude);
                    marker = new google.maps.Marker({ position: devLatLng, map: map2, title: object.name, icon: object.iconUrl });
                    markersMap2[object.deviceId] = marker;
                    (function (marker, content) {
                        
                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }
                            
                            infowindow.setContent(content);
                            infowindow.open(map2, marker);
                        });
                    })(marker, content);
                    bounds1.extend(devLatLng);
                    marker.setMap(map2);
                    break;
                case "3":
                    devLatLng = new google.maps.LatLng(object.latitude, object.longitude);
                    marker = new google.maps.Marker({ position: devLatLng, map: map3, title: object.name, icon: object.iconUrl });
                    markersMap3[object.deviceId] = marker;
                    (function (marker, content) {

                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }

                            infowindow.setContent(content);
                            infowindow.open(map3, marker);
                        });
                    })(marker, content);
                    bounds1.extend(devLatLng);
                    marker.setMap(map3);
                    break;
                case "4":
                    devLatLng = new google.maps.LatLng(object.latitude, object.longitude);
                    marker = new google.maps.Marker({ position: devLatLng, map: map4, title: object.name, icon: object.iconUrl });
                    markersMap4[object.deviceId] = marker;
                    (function (marker, content) {

                        google.maps.event.addListener(marker, 'click', function () {
                            if (!infowindow) {
                                infowindow = new google.maps.InfoWindow();
                            }

                            infowindow.setContent(content);
                            infowindow.open(map4, marker);
                        });
                    })(marker, content);
                    bounds1.extend(devLatLng);
                    marker.setMap(map4);
                    break;
            }

        });
    }
    catch (err) {
        alert('updateMT: ' + err.description);
    }
}
function addGroupArray(pgroupId, pgroupName,panel, pshowGoup,source) {  //source: 1 load , 2 register
    
    let obj = {};
    if (source == 1) {
        obj = { groupId: pgroupId, groupName: pgroupName, panel: panel, showGoup: pshowGoup }
    } else {
        obj = { groupId: pgroupId, groupName: pgroupName, panel: panel, showGoup: pshowGoup }
    }
    arrayGroupsSelect.push(obj);
    
}
function searchGroup(pgroupId) {//¿can you change the group?
    let authorize = false;
    let existGroup = false;
    let existpanel = false;
    if (arrayGroupsSelect.length > 0) {
        arrayGroupsSelect.forEach(function (object) {            
            if (object.groupId == pgroupId) {// && object.panel == ppanel) {
                existGroup = true;
                if (object.panel == ppanel) {
                    existpanel = true;
                }                
                return;           
            }
        }); 
        if (existGroup && existpanel) {
            authorize = true;
        }
        if (existGroup && !existpanel) {
            authorize = false;
        }
        if (!existGroup && !existpanel) {
            authorize = true;
        }
        
    }else {
        authorize = true;
    }
    return authorize;
}
function deletegroupfromarray(pgroupId, pgroupName, ppanel) {
    let obj = { groupId: pgroupId, groupName: pgroupName, panel: ppanel }
    arrayGroupsSelect = $.grep(arrayGroupsSelect, function (value) {
        return value.groupId != obj.groupId;
    });

}
function savePreferences(source) {    
    let object = {};
    list1 = null;
    var div;
    let boolContinue = true;
    
    arrayGroupsSelect.forEach(function (value) {
        if (value.panel=="-1") {
            boolContinue = false;
            toastr.warning("Warning", value.groupName+' group does not have a map assigned');
            return;
        }
    });
    
    if (boolContinue) {
        var listPreferences = [];  
        $(".mtglobal").remove(); 
        arrayGroupsSelect.forEach(function (value) {            
            list1 = [];            
            object = { moduleName: "MULTITRAKING", preference: "-1", val1: value.groupId, val2: value.showGoup, val3: value.panel };
            listPreferences.push(object);               
            if (object.val3 == 1) {
                $("#group1BD").text(value.groupName);
            }
            if (object.val3 == 2) {
                $("#group2BD").text(value.groupName);
            }
            if (object.val3 == 3) {
                $("#group3BD").text(value.groupName);
            }
            if (object.val3 == 4) {
                $("#group4BD").text(value.groupName);
            }
            for (var ind = 0; ind < jsonDevicesGroupsNew.myDevices.length; ind++) {
                jsondevice = eval('(' + jsonDevicesGroupsNew.myDevices[ind] + ')');
                if (jsondevice.GroupID == object.val1) {
                    
                    list1.push(jsondevice);                   
                }
            }            
            div = selectDiv(object.val3.toString());
            createGroupsDevicesDivs(list1, div, object.val3);
            updateMTNew(list1, object.val3.toString())
            if (object.val2) {
                $("#selectgroup" + object.val1).val(object.val3.toString());
                $('#selectgroup' + object.val1).prop('disabled', false);
            } 
        });
        if (arrayGroupsSelect.length == 0) {
            object = { moduleName: "MULTITRAKING", preference: "-1", val1: "-1", val2: "-1", val3: "-1" };
            listPreferences.push(object);
        }
        if (source==2) {
            updateUserPrefGroup(listPreferences);
        }
         
        
        
    }
    
    //createGroupsDevicesDivs(arrayGroupsSelect, div1, globalSelectedGroupId);
    //updateMTNew(arrayGroupsSelect, globalSelectedGroupId)
} 
function loadPreferences(groups) {   
    groups.forEach(function (value) {
        jsongroup = eval('(' + value + ')');
        
        if (jsongroup.showgroup) {            
            addGroupArray(jsongroup.id, jsongroup.name,jsongroup.map,jsongroup.showgroup,1);//1: load
            //globalSelectedGroupId = jsongroup.map;
            //SelectAllDevices(jsongroup.id, jsongroup.name, jsongroup.showgroup);            
        }
    });
    savePreferences(1);//source 1: loadPreferences
}
function selectionPanel(groupId) {  
    
    globalSelectedGroupId = groupId.value;
    let boolContinue = true;
    let group = parseInt($(groupId).attr('data-group'));
    
    arrayGroupsSelect.forEach(function (object) {
        
        if (object.panel == globalSelectedGroupId) {     
            toastr.warning("Warning", 'Map ' + globalSelectedGroupId + ' was assigned to the ' + object.groupName);   
            $("#selectgroup" + group).val("0");
            boolContinue = false;
        }        
    });
    if (boolContinue) {
        arrayGroupsSelect.forEach(function (object) {            
            if (object.groupId == group) {
                object.panel = globalSelectedGroupId;
            }
        });
    }
    
}
function selectDiv(panel) {
    var div;
    switch (panel) {
        case "1":
            $(".mt1").empty();
            div = document.getElementById('mySidenav');
            break;
        case "2":
            $(".mt2").empty();
            div = document.getElementById('2mySidenav');
            break;
        case "3":
            $(".mt3").empty();
            div = document.getElementById('3mySidenav');
            break;
        case "4":
            $(".mt4").empty();
            div = document.getElementById('4mySidenav');
            break;
    }

    return div;

}
