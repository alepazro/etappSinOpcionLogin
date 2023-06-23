//et_02
var welcomeTitle = '';
var userCompanyName = '';
var userFullName = '';
var userALID = 0;
var userIsAdmin = false;
var jsonAppFeatures = false;
var defaultModuleId = false;
var isSuspended = false;
var suspendedReasonId = 0;
var userGUID = '';
var userDealerId = -1;
var rememberMe = false;
var aut_latitude = 0.0;
var aut_longitude = 0.0;
var newapp = false;

function recoverCredentialsResponse(xml, textStatus) {
    try {
        if (textStatus == 'success') {
            if (($("string", xml).text()) == 'failure') {
                alert('We could not recover your credentials.  Please contact Support at support@easitrack.com.');
                return false;
            }
            var objResponse = $("string", xml).text();
            if (objResponse == '') {
                alert('Email not found.  Please try again.');
                return false;
            }
            var jsonResponse = eval('(' + objResponse + ')');
            if (jsonResponse.value == 'OK') {
                alert('Your credentials have been sent to your email.');
                location.href = 'login.html';
            }
            else {
                alert('Email not found.  Please try again.');
            }
            return true;
        }
        else {
            alert('Sign in status: ' + textStatus);
            return false;
        }
    }
    catch (err) {
        alert('recoverCredentialsResponse: ' + err.description);
    }
}

function recoverCredentials() {
    try {
        var email = document.getElementById('txtEmail').value;
        var reqParams = 'email=' + escape(email);
        var url = "ETWS.asmx/recoverCredentials";
        $.post(url, reqParams, function (xml, textStatus) { recoverCredentialsResponse(xml, textStatus); });
    }
    catch (err) {
        alert('recoverCredentials: ' + err.description);
    }
}

function setUserModules(moduleId) {
    try {
        var jsonModule = false;
        
        $('#mod1').hide();
        $('#mod2').hide();
        $('#mod3').hide();
        $('#mod4').hide();
        $('#mod5').hide();
        $('#mod13').hide();
        $('#mod6').hide()
        $('#mod7').hide()
        //document.getElementById("li_trackingFleetLocation").style.display = "none";
        //document.getElementById("li_trackingQuickDispatch").style.display = "none";
        //document.getElementById("li_trackingBreadcrumbTrail").style.display = "none";
        //document.getElementById("li_trackingFleetStatusBoard").style.display = "none";
        //document.getElementById("li_trackingHotSpots").style.display = "none";
        //document.getElementById("li_trakingnumberPanel").style.display = "none";

        //if (moduleId == 1) {
        //    document.getElementById("li_brokerorderpanel").style.display = "none";
        //}
        if (document.getElementById("li_brokerorderpanel") !== null) {
            document.getElementById("li_brokerorderpanel").style.display = "none";
        }
        
        
        if (jsonAppFeatures.modules) {
            for (var ind = 0; ind < jsonAppFeatures.modules.length; ind++) {
                jsonModule = eval('(' + jsonAppFeatures.modules[ind] + ')');

                switch (jsonModule.moduleId) {
                    case '1', 1:
                        $('#mod1').show();
                        break;
                    case '2', 2:
                        $('#mod2').show();
                        break;
                    case '3', 3:
                        $('#mod3').show();
                        break;
                    case '4', 4:
                        $('#mod4').show();
                        break;
                    case '5', 5:
                        $('#mod5').show();
                        break;
                    case '6', 6:
                        $('#mod6').show();
                        break;
                    case '7', 7:
                        $('#mod7').show();
                        break;
                    case '13', 13:
                        $('#mod13').show();
                        break;
                }
            }
        }        
        if (jsonAppFeatures.features) {// && moduleId==1) {
            
            for (var ind = 0; ind < jsonAppFeatures.features.length; ind++) {
                jsonModule = eval('(' + jsonAppFeatures.features[ind] + ')');
                
                if (jsonModule.hasbrokerorder) {
                    $('#li_brokerorderpanel').show();
                } else {
                    ind = jsonAppFeatures.features.length;
                }
            }
        }
        
    }
    catch (err) {
        alert('setUserModules: ' + err.description);
    }
}

function getFeatureALID(featureId) {
    try {
        var jsonFeature = false;
        var featureALID = 999;

        for (var ind = 0; ind < jsonAppFeatures.features.length; ind++) {
            jsonFeature = eval('(' + jsonAppFeatures.features[ind] + ')');

            if (featureId == jsonFeature.id) {
                featureALID = jsonFeature.minALID;
                break;
            }
        }

        return featureALID;
    }
    catch (err) {
        alert('getFeatureALID: ' + err.description);
    }
}

function validateUserAccess(featureId, showMsg) {
    try {
        if (showMsg == undefined) {
            showMsg = true;
        }
        var isOk = false;
        if (jsonAppFeatures == false) {
            getAppFeatures();
        }
        if (jsonAppFeatures != false) {
            var featureALID = getFeatureALID(featureId);
            if (userALID >= featureALID) {
                isOk = true;
            }
        }
        if (isOk == false) {
            if (showMsg == true) {
                alert('Sorry, you dont have access to this feature. Please ask your System Administrator for assistance.');
            }
        }
        return isOk;
    }
    catch (err) {
        alert('validateUserAccess: ' + err.description);
        return isOk;
    }
}

function getAppFeatures(moduleId, sendToLogin) {
    
    try {
        if (sendToLogin == undefined) {
            sendToLogin = true;
        }
        if (jsonAppFeatures == false) {
            if (moduleId == undefined) {
                moduleId = 0;
            }
            var token = getTokenCookie('ETTK');
            if (token != '') {
                var data = 't=' + getTokenCookie('ETTK') + '&moduleId=' + escape(moduleId);
                jsonAppFeatures = dbReadWrite('getAppFeatures', data, true, false);
                

                setUserModules(moduleId);
            }
            else {
                if (sendToLogin == true) {
                    location.href = 'login.html';
                }
            }
        }
    }
    catch (err) {
        alert('getAppFeatures: ' + err.description);
    }
}

function setCookie(cName, t) {
    try {
        var exdate = new Date();
        var expiredays = 1;

        exdate.setDate(exdate.getDate() + expiredays);
        document.cookie = cName + "=" + escape(t) + "; expires=" + exdate.toUTCString();
    }
    catch (err) {
        alert('setTokenCookie: ' + err.Description);
    }
}

function setTokenCookie(jsonResponse) {
    try {
        var exdate = new Date();
        var expiredays = 100;

        exdate.setDate(exdate.getDate() + expiredays);
        document.cookie = jsonResponse.tokenCookie + "=" + escape(jsonResponse.token) + "; expires=" + exdate.toUTCString();
    }
    catch (err) {
        alert('setTokenCookie: ' + err.Description);
    }
}

function getTokenCookie(c_name) {
    try {
        var t = "";
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=");
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1;
                c_end = document.cookie.indexOf(";", c_start);
                if (c_end == -1) c_end = document.cookie.length;
                t = unescape(document.cookie.substring(c_start, c_end));
            }
        }
        return t;
    }
    catch (err) {
        alert('getTokenCookie: ' + err.Description);
    }
}

function deleteTokenCookie(c_name) {
    try {
        if (document.cookie.length > 0) {
            document.cookie = c_name + "=";
        }
    }
    catch (err) {
        alert('deleteTokenCookie: ' + err.Description);
    }
}

function setRememberMeCookie() {
    try {
        var exdate = new Date();
        var expiredays = 100;

        exdate.setDate(exdate.getDate() + expiredays);
        document.cookie = "ETRM=" + document.getElementById('txtLogin').value + "; expires=" + exdate.toUTCString();
    }
    catch (err) {
        alert('setRememberMeCookie: ' + err.Description);
    }
}
function getRememberMeCookie() {
    try {
        
        var c_name = 'ETRM';
        var userName = "";
        let newapp = localStorage.getItem('chknewApp') == "true" ? true : false;
        if (document.cookie.length > 0) {
            c_start = document.cookie.indexOf(c_name + "=");
            if (c_start != -1) {
                c_start = c_start + c_name.length + 1;
                c_end = document.cookie.indexOf(";", c_start);
                if (c_end == -1) c_end = document.cookie.length;
                userName = unescape(document.cookie.substring(c_start, c_end));
                document.getElementById('txtLogin').value = userName;
                $('#chkRememberMe').prop('checked', true);
                $('#chknewApp').prop('checked', newapp);
            }
        }
    }
    catch (err) {
        alert('getRememberMeCookie: ' + err.Description);
    }
}
function deleteRememberMeCookie() {
    try {
        var c_name = 'ETRM';
        if (document.cookie.length > 0) {
            document.cookie = c_name + "=";
        }
    }
    catch (err) {
        alert('deleteRememberMeCookie: ' + err.Description);
    }
}

//Main Navigation
//1: Tracking, 2: Reports, 3: Settings
function changePanel(panelId) {
    
    try {
        switch (panelId) {
            case '1', 1:
                location.href = 'Tracking.html';
                break;
            case '2', 2:
                location.href = 'Multi-Tracking.html';
                break;
            case '3', 3:
                location.href = 'Reports.html';                
                break;
            case '4', 4:
                location.href = 'Settings.html';
                break;
            case '5', 5:
                location.href = 'maintenance.html';
                break;
            case '6', 6:
                location.href = 'jobs.html';
                break;       
            case '13', 13:
                //location.href = 'elog.html';
                window.open('elog.html', '_blank');
                break;
            case '14', 14:
                //location.href = 'elog.html';
                window.open('hvideo.html');
                break;
            case '15', 15:
                //location.href = 'elog.html';
                location.href = 'EtReport.html';
                break;
            case '16', 16:
                //location.href = 'elog.html';
                location.href = 'EtTraking.html';
                break;
            case '17', 17:
                //location.href = 'elog.html';                
                var data = getvalidatetoken();
                
                if (data.HasVideo =="True") {
                    window.open('https://video.easitrack.net/sso?access_token=' + data.VideoKey, '_blank');
                } else {
                    if (data.IsDirectDealer) {
                        //window.open('videoservice.html', '_blank');
                        window.location.href = "videoservice.html";
                    } else {
                        alert("You have not activated your Video account yet.  Please contact us!");
                        window.location.href = "login.html";
                    }
                    
                }
                break;
        }
    }
    catch (err) {
        alert('changePanel: ' + err.description);
    }
}

// Credentials
function validateCredentialsResponse(xml, textStatus, isMobile) {
    try {
        
        if (textStatus == 'success') {
            if (($("string", xml).text()) == 'failure') {
                alert('Login failed.  Please try again.');
                return false;
            }
            var objResponse = $("string", xml).text();
            if (objResponse == '') {
                alert('Invalid Credentials.  Please try again.');
                return false;
            }
            var jsonResponse = eval('(' + objResponse + ')');
            if (jsonResponse.result == 'failure') {
                alert(jsonResponse.error);
                return false;
            }
            else {
                setTokenCookie(jsonResponse);
                
                welcomeTitle = jsonResponse.welcomeTitle;
                userCompanyName = jsonResponse.companyName;
                userFullName = jsonResponse.fullName;
                userALID = jsonResponse.accessLevelId;
                defaultModuleId = jsonResponse.defaultModuleId;
                userIsAdmin = jsonResponse.isAdministrator;
                isSuspended = jsonResponse.isSuspended;
                suspendedReasonId = jsonResponse.suspendedReasonId;
                userGUID = jsonResponse.UserGUID;
                userDealerId = jsonResponse.dealerId;
                
                aut_latitude = jsonResponse.latitude;
                aut_longitude = jsonResponse.longitude;
                
                
                if (rememberMe == true) {
                    setRememberMeCookie();
                }
                else {
                    deleteRememberMeCookie();
                }

                if (rememberMe == true) {
                    setRememberMeCookie();
                }
                else {
                    deleteRememberMeCookie();
                }

                document.getElementById('txtLogin').value = '';
                document.getElementById('txtPassword').value = '';

                if (isSuspended == true && suspendedReasonId == 1) {
                    location.href = 'https://easitrack.net/billingUpdate.html?uid=' + userGUID;
                }
                else {
                    if (isMobile == true) {
                        location.href = 'mDevicesList.html';
                    }
                    else {
                        if (defaultModuleId == false) {
                            defaultModuleId = 1;
                        }
                        changePanel(defaultModuleId);
                        //location.href = 'tracking.html';
                    }
                }
                return true;
            }
        }
        else {
            alert('Sign in status: ' + textStatus);
            return false;
        }
    }
    catch (err) {
        alert('validateCredentialsResponse: ' + err.Description);
        return false;
    }
}

function validateCredentials(isMobile) {
    
    try {
        if (isMobile == undefined) {
            isMobile = false;
        }
        
        
        rememberMe = $('#chkRememberMe').is(':checked');
        newapp = true;// localStorage.getItem('chknewApp') == "true" ? true : false;
        if (!newapp) {
            location.replace("https://classic.easitrack.net/login.html?newapp=false")
            return;
        }


        var login = document.getElementById('txtLogin').value;
        var pw = document.getElementById('txtPassword').value;
        var language = $('#language').val();
        if (login.length < 1) { alert('enter a valid user'); return; }
        if (pw.length < 3) { alert('enter a valid password'); return; }
        if (language=="-1") { alert('Select a language'); return; }

        var reqParams = 'Login=' + escape(login) + '&pw=' + escape(pw) + '&rememberMe=' + rememberMe + '&language=' + language + '&newApp=' + newapp;
        var url = "ETWS.asmx/ValidateCredentials";
        $.post(url, reqParams, function (xml, textStatus) { validateCredentialsResponse(xml, textStatus, isMobile); });
    }
    catch (err) {
        console.log(err);
        alert('validateCredentials: ' + err.Description);
    }
}

function logout() {
    try {
        deleteTokenCookie('ETTK');
        location.href = 'login.html';
    }
    catch (err) {
        alert('Logout: ' + err.Description);
    }
}

function validateToken(isMobile, sourcePage) {
    
    let napp = localStorage.getItem('chknewApp');
    if (napp == false) {
        location.replace("https://classic.easitrack.net/login.html?")
        return;
    }
    
    //This has to be placed outside the try...catch to permit its use in the catch.
    if (isMobile == undefined) {
        isMobile = false;
    }
    if (sourcePage == undefined) {
        sourcePage = '';
    }

    var loginPage = ''
    if (isMobile == true) {
        loginPage = 'mLogin.html';
    }
    else {
        loginPage = 'login.html';
    }

    try {
        var t = getTokenCookie('ETTK');
        var ret = false;
        if ((t == null) || (t == '')) {
            if (location.pathname.toLowerCase().indexOf(loginPage.toLowerCase()) == -1) {
                location.href = loginPage;
            }
            return false;
        }
        else {
            jQuery.ajax({
                url: 'ETWS.asmx/validateToken',
                data: 't=' + escape(t) + '&sourcePage=' + escape(sourcePage) + '&sourceId=' + escape('WEB'),
                dataType: 'xml',
                type: "POST",
                success: function (xml, textStatus) {
                    
                    if (textStatus == 'success') {

                        var objResponse = $("string", xml).text();
                        if (objResponse == '') {
                            alert('Invalid Credentials.  Please try again.');
                            return false;
                        }
                        var jsonResponse = eval('(' + objResponse + ')');
                        if (jsonResponse.isValid == true) {
                            //2
                            welcomeTitle = jsonResponse.welcomeTitle;
                            userCompanyName = jsonResponse.companyName;
                            userFullName = jsonResponse.fullName;
                            userALID = jsonResponse.accessLevelId;
                            userIsAdmin = jsonResponse.isAdministrator;
                            defaultModuleId = jsonResponse.defaultModuleId;
                            isSuspended = jsonResponse.isSuspended;
                            suspendedReasonId = jsonResponse.suspendedReasonId;
                            userGUID = jsonResponse.UserGUID;
                            userDealerId = jsonResponse.dealerId;
                            aut_latitude = jsonResponse.latitude;
                            aut_longitude = jsonResponse.longitude;
                            
                            try{
                                transactionID = jsonResponse.transactionId;
                                var exdate = new Date();
                                var expiredays = 100;
                                exdate.setDate(exdate.getDate() + expiredays);
                                document.cookie = 'ETENG' + "=" + escape(transactionID) + "; expires=" + exdate.toUTCString();
                            }
                            catch(err){
                                
                            }
                            
                            if (isMobile == true) {
                            }
                            else {
                                if (location.pathname.toLowerCase().indexOf('tracking.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('reports.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('notifications.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('maintenance.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('dashboard.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('settings.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('account.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('fleetstatusboard.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('hotspots.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('jobs.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('job.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('elog.html') == -1 &&
                                location.pathname.toLowerCase().indexOf('roto.wlius') == -1 &&
                                location.pathname.toLowerCase().indexOf('settingsunits.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('settings_v2.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('etreport.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('ettraking.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('trackingp.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('settingsProduccion.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('trakingnumberpanel.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('brokerorders.html') == -1 &&
                                    location.pathname.toLowerCase().indexOf('brokerorderpanel.html') == -1) {
                                    
                                    location.href = 'tracking.html';
                                }
                            }
                            
                            ret = true;
                        }
                        else {
                            if (location.pathname.toLowerCase().indexOf('login.html') == -1) {
                                location.href = loginPage;
                            }
                            ret = false;
                        }
                    }
                    else {
                        alert('Error validating token');
                        ret = false;
                    }
                },
                error: function (result) {
                    alert('Error validating token');
                    ret = false;
                },
                async: false
            });
        }
        return ret;
    }
    catch (err) {
        if (location.pathname.toLowerCase().indexOf(loginPage.toLowerCase()) == -1) {
            location.href = loginPage;
            return false;
        }
    }
}

function validateDemoToken() {
    try {
        var token = getParameterByName('t');
        if (token.length > 0) {
            data = 't=' + escape(token);
            var jsonResponse = dbReadWrite('validateDemoToken', data, true, false);
            setTokenCookie(jsonResponse);

            welcomeTitle = jsonResponse.welcomeTitle;
            userCompanyName = jsonResponse.companyName;
            userFullName = jsonResponse.fullName;
            userALID = jsonResponse.accessLevelId;
            userIsAdmin = jsonResponse.isAdministrator;
            defaultModuleId = jsonResponse.defaultModuleId;
            isSuspended = jsonResponse.isSuspended;
            suspendedReasonId = jsonResponse.suspendedReasonId;
            userGUID = jsonResponse.UserGUID;
            userDealerId = jsonResponse.dealerId;


            location.href = 'tracking.html';
            return true;
        }
        else {
            alert('Invalid access token');
            return false;
        }
    }
    catch (err) {
        alert('validateDemoToken: ' + err.description);
    }
}

function getUserGUIDParam() {
    try {
        
        var userGUID = getParameterByName('userGUID');
        if (userGUID != '') {
            var data = 'userGUID=' + userGUID;
            var jsonToken = dbReadWrite('getTokenFromUserGUID', data, true, false);
            if (jsonToken.token != '') {
                setTokenCookie(jsonToken);
            }
        }
    }
    catch (err) {
    }
}
