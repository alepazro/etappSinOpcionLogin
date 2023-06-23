var companiesList = false;
var emailTypes = false;

function loadCompaniesList() {
    try {
        var token = getCookie('ETCRMTK');
        $.ajax({
            url: 'https://pre.etrack.ws/etrack.svc/getCompanies/' + token,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                companiesList = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch Companies');
            },
            async: false
        });
    }
    catch (err) {
        alert(err.description);
    }
}

function getCompaniesList() {
    try {
        if (companiesList == false) {
            loadCompaniesList();
        }
        return companiesList;
    }
    catch (err) {
        alert('getCompaniesList: ' + err.description);
    }
}

function loadEmailTypes(type) {
    try {
        var token = getCookie('ETCRMTK');
        $.ajax({
            url: 'https://pre.etrack.ws/etrack.svc/getEmailTypes/' + token + '/' + type,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                emailTypes = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch Email Types');
            },
            async: false
        });
    }
    catch (err) {
        alert(err.description);
    }
}

function getEmailTypes(type) {
    try {
        if (emailTypes == false) {
            loadEmailTypes(type);
        }
        return emailTypes;
    }
    catch (err) {
        alert('getEmailTypes: ' + err.description);
    }
}

function getGenericMasters(masterKey) {
    try {
        var genericMaster = false;
        var token = getCookie('ETCRMTK');
        $.ajax({
            url: 'https://pre.etrack.ws/etrack.svc/getGenericMasters/' + token + '/' + masterKey,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                genericMaster = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch generic master key: ' + masterKey);
            },
            async: false
        });

        return genericMaster;
    }
    catch (err) {
        alert('getGenericMasters: ' + err.description);
    }
}
var suspendedReasons = false;

function loadSuspendedReasonsList() {
    try {
        var token = getCookie('ETCRMTK');
        $.ajax({
            url: 'https://pre.etrack.ws/etrack.svc/getCompaniesSuspendedReasons/' + token,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                suspendedReasons = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch CompaniesSuspendedReasons');
            },
            async: false
        });
    }
    catch (err) {
        alert(err.description);
    }
}

function getSuspendedReasonsList() {
    try {
        if (suspendedReasons == false) {
            loadSuspendedReasonsList();
        }
        return suspendedReasons;
    }
    catch (err) {
        alert(err.description);
    }
}

var allCompanies = false;
function getAllCompanies(entityName) {
    try {
        basicList = false;
        var token = getCookie('ETCRMTK');
        var url = 'https://pre.etrack.ws/etrack.svc/getAllCompanies/' + token;
        $.ajax({
            url: url,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                allCompanies = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch basicList.entityName = ' + entityName);
            },
            async: false
        });

        return allCompanies;
    }
    catch (err) {
        alert('getBasicList: ' + err.description);
    }
}
