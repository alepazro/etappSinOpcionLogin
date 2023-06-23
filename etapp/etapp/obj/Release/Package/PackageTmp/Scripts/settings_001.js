function hideSettingsDivs() {
    try {
        $('#usrDiv').hide();
        $('#devDiv').hide();
        $('#devGroupsDiv').hide();
        $('#geoDiv').hide();
        $('#alertsDiv').hide();
        $('#recurrentReportsDiv').hide();
        $('#schedulesDiv').hide();
        $('#routesDiv').hide();
        $('#ibuttonsDiv').hide();
        $('#tempProfilesDiv').hide();
        $('#telemetryDiv').hide();
        $('#hourMetersDiv').hide();
        $('#mobileAppsDiv').hide();
        $('#tempsensor').hide();
    }
    catch (err) {
        alert('hideSettingsDivs: ' + err.description);
    }
}

function showSettingsDivs(id) {
    
    try {       
        
        hideSettingsDivs();

        switch (id) {
            case "1":
            case 1:
                if (validateUserAccess(5) == true) {
                    $('#usrDiv').show();
                    
                    loadUsers();
                }
                break;
            case "2":
            case 2:
                if (validateUserAccess(10) == true) {
                    try{
                        if ((userDealerId == 1 || userDealerId == 7 || userDealerId == 33) && userIsAdmin == true) {
                            location.href = 'settingsUnits.html';
                        }
                        else {
                            $('#devDiv').show();
                            loadDevices();
                        }
                    }
                    catch(err){
                        $('#devDiv').show();
                        loadDevices();
                    }
                }
                break;
            case "3":
            case 3:
                if (validateUserAccess(11) == true) {
                    $('#geoDiv').show();
                    loadGeofences();
                }
                break;
            case "4":
            case 4:
                if (validateUserAccess(13) == true) {
                    $('#alertsDiv').show();
                    loadAlerts();
                }
                break;
            case "5":
            case 5:
                if (validateUserAccess(14) == true) {
                    $('#recurrentReportsDiv').show();
                    loadRecurrentReports();
                }
                break;
            case "6":
            case 6:
                if (validateUserAccess(29) == true) {
                    $('#schedulesDiv').show();
                    loadSchedules();
                }
                break;
            case "7":
            case 7:
                if (validateUserAccess(30) == true) {
                    $('#routesDiv').show();
                    loadRoutes();
                }
                break;
            case "8":
            case 8:
                if (validateUserAccess(31) == true) {
                    $('#ibuttonsDiv').show();
                    loadIButtons();
                }
                break;
            case "9":
            case 9:
                if (validateUserAccess(33) == true) {
                    $('#devGroupsDiv').show();
                    loadDevGroups();
                }
                break;
            case "10":
            case 10:
                if (validateUserAccess(39) == true) {
                    $('#tempProfilesDiv').show();
                    loadTempProfiles();
                }
                break;
            case "12":
            case 12:
                $('#mobileAppsDiv').show();
                break;
            case "13":
            case 13:
                if (validateUserAccess(45) == true) {
                    $('#telemetryDiv').show();
                    var dlgScope = angular.element($("#telemetryTable")).scope();
                    dlgScope.loadDevices();
                }
                break;
            case "14":
            case 14:
                if (validateUserAccess(46) == true) {
                    $('#hourMetersDiv').show();
                    var dlgScope = angular.element($("#hourMetersTable")).scope();
                    dlgScope.loadDevices();
                }
                break;
            case 15:
                if (validateUserAccess(5) == true) {
                    $('#tempsensor').show();
                    
                    LoadSensors();
                    //var dlgScope = angular.element($("#hourMetersTable")).scope();
                    //dlgScope.loadDevices();
                }
                break;
        }

    }
    catch (err) {
        alert('showSettingsDivs: ' + err.description);
    }
}

function accountSettings() {
    try {
        if (userIsAdmin == true) {
            window.open('https://easitrack.net/account.html?t=' + getTokenCookie('ETTK'), target = "_blank");
        }
        else {
            alert('This option is only available for Administrator users');
        }
    }
    catch (err) {
        alert('accountSettings: ' + err.description);
    }
}

function buyNow() {
    try {
        if (userIsAdmin == true) {
            window.open('https://localhost:44358/buy-now.html?s=1&t=' + getTokenCookie('ETTK'), target = "_blank");
        }
        else {
            alert('This option is only available for Administrator users');
        }
    }
    catch (err) {
        alert('accountSettings: ' + err.description);
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
function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
    return false;
}
function showTempSensorForm() {
    $("#tempsensorPost").show();
}
function saveSensor() {
    
    var TempSensors = {};
    if (validarSensor().length > 0) {
        $("#msg").val = validarSensor();
    } else {
        TempSensors.ID = $("#ID").val();
        TempSensors.Devices = $("#Devices").val();
        TempSensors.Name = $("#Name").val();
        TempSensors.TempNumber = $("#TempNumber").val();        
        var response = postTempSensors(TempSensors);

    }
}
function UpdateSensor() {
    
    var TempSensors = {};
    if (validarSensor().length > 0) {
        $("#msg").val = validarSensor();
    } else {
        TempSensors.ID = $("#ID").val();
        TempSensors.Devices = $("#Devices").val();
        TempSensors.Name = $("#Name").val();
        TempSensors.TempNumber = $("#TempNumber").val();
        TempSensors.Action = 5;
        var response = UpdateTempSensors(TempSensors);
        if (response.isOk == 1) {
            $("#tempsensorPost2").modal('hide');//ocultamos el modal
            $('body').removeClass('modal-open');//eliminamos la clase del body para poder hacer scroll
            $('.modal-backdrop').remove();//eliminamos el backdrop del modal
            LoadSensors();
        }
    }
}
function validarSensor() {
    var msg = "";
    if ($("#IMEI").val() == '') {
        msg = "IMEI cannot be empty"
    }
    if ($("#TempNumber").val() == '') {
        msg = "TempNumber cannot be empty"
    }
    if (!Number.isInteger(parseInt($("#TempNumber").val()))) {
        msg = "The TempNumber field must be an integer value."
    }
    if ($("#SensorID").val() == '') {
        msg = "SensorID cannot be empty"
    }
    if ($("#Name").val() == '') {
        msg = "Name cannot be empty"
    }
    return msg;
}
function LoadSensors() {
    
    try {
        $("#tbody_tempsensorList").empty();
        var row = "";

        var data = getsensors();

        //data = JSON.parse(data);
        var date;
        $.each(data, function (key, value) {
            //alert(key + ": " + value.name);
            //date = new Date(value.LastUpdatedOn).toDateString();;
            
            row += "<tr>";
            /*row += "<td><a href=" + viewCustomer(e) + "class=link - primary>Primary link</a></td>";*/

            row += "<td><button type='button' data-toggle='modal' data-target='#tempsensorPost2' onclick=viewsensor()>Edit</button></td>";
            row += "<td style=display:none>" + value.ID + "</td>";
            row += "<td>" + value.Devices + "</td>";
            row += "<td>" + value.Name + "</td>";
            row += "<td>" + value.TempNumber + "</td>";
            row += "<td>" + value.LastUpdatedOn + "</td>";
            row += "</tr>";
        });
        $("#tbody_tempsensorList").append(row)
        // $('#customersGrid').data('kendoGrid').dataSource.data(data);

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}
function viewsensor() {
    try {
        
        var bool = true;
        //e.preventDefault();
        //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //window.open('crmViewCustomer.html?' + 'uid=' + dataItem.uniqueKey, target = "_blank");
        $("#tempsensorListGrid tr").click(function () {
            
            
            if (bool) {
                var tr = $(this)[0];
                var ID = tr.cells[1].innerText;
                var Devices = tr.cells[2].innerText;
                var Name = tr.cells[3].innerText;
                var TempNumber = tr.cells[4].innerText;
                
                $("#ID").val(ID);
                $("#Devices").val(Devices);
                $("#Name").val(Name);
                $("#TempNumber").val(TempNumber);               

                //window.open('crmViewCustomer.html?' + 'uid=' + trID, target = "_blank");                
            }
            bool=false
            
        });
    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}



