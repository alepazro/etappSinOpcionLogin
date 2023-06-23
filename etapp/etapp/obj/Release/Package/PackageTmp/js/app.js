var app = {
    apiUrl: 'https://pre.etrack.ws',
    lan: 'en',
    debug: true,
    version: '1.0',
    map:null,
    tkn: null,
    dataCustomers: [],
    dataPriority: [],
    dataStatus: [],
    dataUsers: [],
    dataCategories:[],
    reloadFilters: true,  //cargo todos los filtros de una,
    currentTab: 0,
    markers: [],
    Flag_Refresh: true,
    infowindow: null,
    dataJobs: [],
    selectedCustomer: '-1',
    selectedPriotity: '-1',
    selectedUser: '-1',
    selectedStatus: '-1',
    selectedType: '-1',
    jobNumber: '',
    geocoder: null,
    from: 'address',
    location: null,
    currentModule: 0,
    excManager: null,
    findaddress:null
};


$(document).ready(function () {

    loadCookie();
    LoadCustomers();   
});


$(function () {
   
    resize();
});


$(window).load(function () {
    $(window).resize(function () {
        if (_.isObject(app)) {
            resize();
            JobsModule.resize();
        }
    });
});

function resize() {
    var h = $(document).height();
    $('#bodyContent').css('height', (h - 80) + 'px');
    return h;
}

function Remove() {
    $('#bodyContent').empty();
}

function CallModule() {
    try{

        JobsModule.InitModule();


    } catch (exc) {
        _Show("CallModule" + " -->" + exc);
    }
}

function _Show(msg) {
    try {
        if (app.debug)
            console.debug("MSG: " + msg);
    }
    catch (e) {
    }
}


function dbSync(controller, fn, jDataJson, method, callback, pt) {
    try {

        if (method == "POST") {

            $.ajax({
                url: app.apiUrl + "/" + controller + "/" + fn + "/"+app.tkn,
                type: method,
               // contentType: "application/javascript",
               // dataType: "jsonp",
                data: jDataJson,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (data, textStatus) {
                    if (typeof (callback) != 'undefined')
                        callback(data, pt);
                },

                error: function (xhr, status, errorThrown) {
                    if (typeof (callback) != 'undefined') {
                        var result = { resultCode: -1, resultMsg: xhr.statusText };
                        callback(result, pt);
                    }
                }
            });
        }
        else if (method == "GET") {
            $.ajax({
                url: app.apiUrl + "/" + controller + "/" + fn + '?' + jDataJson,
                type: method,
                contentType: "application/javascript",
                dataType: "jsonp",
                //  data: jDataJson,
                success: function (data, textStatus) {
                    if (typeof (callback) != 'undefined')
                        callback(data, pt);
                },

                error: function (xhr, status, errorThrown) {
                    if (typeof (callback) != 'undefined') {
                        var result = { resultCode: -1, resultMsg: xhr.statusText };
                        callback(result, pt);
                    }
                }
            });
        }
        else {
            $.ajax({
                url: app.apiUrl + "/" + controller + "/" + fn  + "/" + app.tkn + '/' + jDataJson,
                type: "GET",
                contentType: "application/javascript",
                dataType: "jsonp",
                //  data: jDataJson,
                success: function (data, textStatus) {
                    if (typeof (callback) != 'undefined')
                        callback(data, pt);
                },

                error: function (xhr, status, errorThrown) {
                    if (typeof (callback) != 'undefined') {
                        var result = { resultCode: -1, resultMsg: xhr.statusText };
                        callback(result, pt);
                    }
                }
            });

        }

    }
    catch (err) {
        console.log('dbSync: Error loading controller ' + controller + ', fn: ' + fn);
    }
}

function loading(show) {
    try {
        if (show == true) {
            if ($.find('.loading-indicator') == "") {
                $('<div class="loading-indicator"></div>').appendTo('body');
            }
        }
        else {
            if ($.find('.loading-indicator') != "") {
                $('.loading-indicator').remove();
            }
        }
    }
    catch (exc) {
        uc.excManager("app", "loading", exc);
    }
};

function loadCookie() {
    app.tkn = $.cookies.get("ETTK");

    if (app.tkn!=null && typeof (app.tkn) != 'undefined') {
    }
    else {
        //app.tkn = 'A138ED95-3479-480F-AC98-61D0F93C373D';
        alert('Invalid token');
    }

}

function LoadCustomers(callback) {
  
    var data = 'token=' + app.tkn;
    dbSync('jobs.svc', 'getCustomersList', data, "GET", LoadCustomers_callback, callback);
}


function LoadCustomers_callback(rs, callback) {
    try{
        app.dataCustomers = rs;

        if (app.reloadFilters)
            LoadPriority();

        if (callback != null && typeof (callback) != 'undefined') {
            UpdateCustomer(callback);
        }

    } catch (exc) {
        _Show("LoadCustomers_callback -->" + exc.message);
    }
}


function UpdateCustomer(callback) {
    try{
        for (var i = 0; i <= 4; i++) {
            var combox = $("#ddlCustomers" + i).data("kendoComboBox");
            combox.setDataSource(app.dataCustomers);
        }

      
        if (callback != null && typeof (callback) != 'undefined') {
           
            callback();
        }
    }
    catch (exc) {
        _Show("UpdateCustomer -->" + exc.message);
    }
}

function LoadPriority() {
    
    var data = 'token=' + app.tkn;
    dbSync('jobs.svc', 'getJobPrioritiesList', data, "GET", LoadPriority_callback);
}

function LoadPriority_callback(rs) {
    try{
    app.dataPriority = rs;
    if (app.reloadFilters)
        LoadUsers();
    } catch (exc) {
        _Show("LoadPriority_callback -->" + exc.message);
    }
}

function LoadUsers() {

    var data = 'token=' + app.tkn;
    dbSync('jobs.svc', 'getUsersList', data, "GET", LoadUsers_callback);
}

function LoadUsers_callback(rs) {
    try {
        var users = [];
        $.each(rs, function (f, user) {
                user.name = user.firstName + ' ' + user.lastName;
                users.push(user);
        });

        app.dataUsers = users;
    if (app.reloadFilters)
        LoadStatus();
    } catch (exc) {
        _Show("LoadUsers_callback -->" + exc.message);
    }
}


function LoadStatus() {
    var data = 'token=' + app.tkn;
    dbSync('jobs.svc', 'getJobStatusList', data, "GET", LoadStatus_callback);
}

function LoadStatus_callback(rs) {
    try{
        app.dataStatus = rs;
        if (app.reloadFilters)
            LoadCategories();
    } catch (exc) {
        _Show("LoadStatus_callback -->" + exc.message);
    }

}


function LoadCategories() {
    var data = 'token=' + app.tkn;
    dbSync('jobs.svc', 'getJobCategoriesList', data, "GET", LoadCategories_callback);
}

function LoadCategories_callback(rs) {
    try {
        app.dataCategories = rs;
        if (app.reloadFilters)
            CallModule();
    } catch (exc) {
        _Show("LoadStatus_callback -->" + exc.message);
    }

}

function ddlCombo_value(element) {
    try {
        var obj = $('#' + element + app.currentTab).data("kendoComboBox");
        var dataobj = obj.dataItem();
        if (typeof (dataobj) != 'undefined')
            dataobj = obj.value();
        else dataobj = '';

        return dataobj;
    } catch (exc) {
        _Show("Exc ddlCombo_value : " + exc.message);
    }
    return '';
}

function ddlComboNormal_value(element) {
    try{
        var obj = $('#' + element).data("kendoComboBox");
        var dataobj = obj.dataItem();

        if (typeof (dataobj) != 'undefined')
            dataobj = obj.value();
        else dataobj = '';

        return dataobj;
    } catch (exc) {
        _Show("Exc ddlComboNormal_value : " + exc.message);
    }
    return '';
}


function formatDate(objDate) {
    try {
        // app.console("fecha -->" + objDate);
        if (typeof (objDate) != 'undefined' && objDate.trim() != '') {     
            var strDate = moment(objDate).format("MMM DD YYYY hh:mm A");            
            return strDate.toUpperCase();
        }
        else
            return "";
    } catch (exc) {
        _Show("formatDate -->" + exc.message);
    }
}

function endDate(start,timeStr) {
    try {
        var endDateDispatch = moment(start).add(timeStr, 'm').format("MMM DD YYYY hh:mm A");

        return endDateDispatch;
    } catch (exc) {
        _Show("endDate -->" + exc.message);
    }
}

function loadScript(path, callback) {
    try {
        $.getScript(path, function (data, textStatus, jqxhr) {
            if (typeof (callback) != 'undefined' && callback != null)
                callback();
        });
    }
    catch (exc) {
        Show("loadScript -->" + exc.message);
    }
}

function showDialog(opts) {
    try {
        var options = {
            moduleKey: '#modal',                      //ID del contener en modules
            dialogId: 'dialog-form',               //'Dialog DIV Container'
            title: 'Dialog',                            //Dialog Title
            template: 'Create a template for this dialog', //HTML Content
            width: 500,
            height: 180,
            modal: true,
            top: 150,
            closeCallback: null
        }
        $.extend(options, opts);

        if ($("#" + options.dialogId).data('kendoWindow') != null) {
            $("#" + options.dialogId).data('kendoWindow').destroy();
            $('#' + options.dialogId).remove();
        }

        $('<div id="' + options.dialogId + '" class="ModalWindow">' + options.template + '</div>').appendTo('body');

        $("#" + options.dialogId).kendoWindow({
            title: options.title,
            width: options.width,
            height: options.height,
            modal: options.modal,
            position: {
                top: options.top,
                left: ($('#bodyContent').width() - options.width) / 2
            },
            actions: ["Minimize", "Maximize", "Close"],
            animation: {
                open: { effects: "fade:in" },
                close: { effects: 'expand:vertical', reverse: true }
            },
            close: function (e) {
                try {
                    if (options.closeCallback != null) {
                        try {
                            options.closeCallback();
                        }
                        catch (exc) {
                           _Show("showDialog.closeCallback "+exc.message);
                        }
                    }

                    $("#" + options.dialogId).data('kendoWindow').destroy();
                    $('#' + options.dialogId).remove();
                }
                catch (exc) {
                   _Show("showDialog.close "+exc.message);
                }
            }
        });
    }
    catch (exc) {
        _Show("showDialog " + exc.message);
    }
}

function getfulladdress(results) {
    var result = '';
    try {
        if (results != null) {

            if ( typeof(results.street)!='undefined' && results.street.trim() != "") {
                result = $.trim(results.street) + ', ';
            } else if (typeof (results.address1) != 'undefined' && results.address1.trim() != "") {
                result = $.trim(results.address1) + ', ';
            }
            result= result+
                    $.trim(results.city + ' ' +
                    results.state) + ' ' +
                    $.trim(results.postalCode + ' ' +
                    results.country);
            
        }
    }
    catch (exc) {
        _Show("getfulladdress " + exc.message);
    }
    return result;
}



app.excManager = function (jslib, fn, exc) {
    try {
        var msg = "EXC " + jslib + ", fn: " + fn + ", exc: " + exc.message;
        // alert(msg);
        console.error("EXC " + jslib + ", fn: " + fn + ", exc: " + exc.message);
        uc.loading(false);
    }
    catch (exc2) {

    }
}

app.findaddress = function () {

    showDialog({           //Ventana de Dialogo
        moduleKey: 'findaddress',
        dialogId: 'divFindAddress',
        title: '',
        template: 'Loading',
        width: 700,
        height: 530,
        top: 20,
        closeCallback: function () {
            try {
                if (ucfindaddress.results != null) {                                //1. Se accede al UserControl justo antes de hacerle Unload del DOM.
                    var locationFA = ucfindaddress.results;                         //2. Se definition un JSON location = { address1, address2, city,... radius }
                    //    En este caso, se copia a otra variable local
                    switch (app.currentModule) {

                        case 0:

                            break;
                        case 1:  //modulo creacion de jobs
                        
                            break;
                        case 3: ////modulo creacion de customer

                         //   _Show("location -->" + JSON.stringify(locationFA));

                            customerForm.SetLocation(locationFA);

                            break;

                    }                 
                }
            }
            catch (exc) {
                app.excManager(uc.key, "setuseres_address.closecallback", exc);
            }
        }
    });

    $('#divFindAddress').load('js/libs/findaddress/findaddress.htm?v=' + app.version, function () {    //Carga del UserControl en la ventana de Dialogo

        var locationModule = null;
        switch (app.currentModule) {

            case 0:

                break;
            case 1:  //modulo creacion de jobs

                break;
            case 3: ////modulo creacion de customer

                locationModule = customerForm.getLocation();

                break;

        }

      


        if (locationModule == null) {
            locationModule = {
                address1: '',
                address2: '',
                city: '',
                state: '',
                postalcode: '',
                country: 'US',
                lat: 0,
                lng: 0,
                radius: 0
            };
        }
      
      //  _Show("location find address  --> " + JSON.stringify(locationModule));

        ucfindaddress.setInfo({         //Funcion para inicializar
            location: locationModule,
            bylocation: false,          //Default de busqueda es por Address.
            enableradius: false         //Se habilita el control de Radius. 
        });
    });

}


