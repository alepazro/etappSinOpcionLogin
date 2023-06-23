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
    }
    catch (err) {
        alert('hideSettingsDivs: ' + err.description);
    }
}

function showSettingsDivs(id) {
    debugger;
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
                        if ((userDealerId == 1 || userDealerId == 7) && userIsAdmin == true) {
                            //location.href = 'settingsUnits.html';
                            $('#devDiv').show();
                            loadDevices();
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
            window.open('https://easitrack.net/buy-now.html?s=1&t=' + getTokenCookie('ETTK'), target = "_blank");
        }
        else {
            alert('This option is only available for Administrator users');
        }
    }
    catch (err) {
        alert('accountSettings: ' + err.description);
    }
}

