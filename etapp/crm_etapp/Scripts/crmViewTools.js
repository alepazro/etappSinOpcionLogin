var jsonDeviceData = false;
var jsonCustomer = false;
var uid = false;
var idDevices=0;
var idCompany=0;
var listDealers = [];
var company = '';
var dealers;
var devices = '';
var listDevicesAcell = [];
var listDevicesAcellConfiguration = [];
var listDevicesAcellConfigurationString = "";
var tokenp = getTokenCookie('ETCRMTK')
var comandid = "";
var deviceid1 = "";
function getCrmDealers() {
    
    try {
        
        var token = getTokenCookie('ETCRMTK');
       

        $.ajax({
            //url: 'crm.svc/getCrmDealers?token=' + token,
            url: 'crm.svc/getCrmDealers/' + token,
            type: "POST",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                ;
                listDealers = jQuery.parseJSON(data);
                for (var i = 0; i < listDealers.length; i++) {
                    $("#cmbDealers").append('<option value="' + listDealers[i].ID + '">' + listDealers[i].Name + '</option>');
                    
                    console.log(listDealers[i].ID + " -> " + listDealers[i].Name);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ;
                listDealers = [];
            },
            cache: false,
            async: false
        });

        

    }
    catch (err) {

        alert('Error getCrmDealers: ' + err.message);
    }
}

function createDevicesGrid() {
    try {
        jQuery("#tbldevices").jqGrid({
            datatype: "local",
            height: 200,
            width: 1200,
            colNames: ['ID','DeviceID', 'Devices', 'idCompanies', 'company', 'idDealers', 'Dealers'],
            colModel: [
                { name: 'ID', index: 'ID', title: false, hidden: true, width: 100 },
                { name: 'DeviceID', index: 'DeviceID', width: 100 },
                { name: 'NameDevices', index: 'NameDevices', width: 300 },
                { name: 'idCompanies', index: 'idCompanies', hidden: true, width: 100 },
                { name: 'company', index: 'company', width: 180 },
                { name: 'idDealers', index: 'idDealers', hidden: true, width: 100 },
                { name: 'Dealers', index: 'Dealers', width: 100 },               
            ]   ,
            rowNum: 50,
            rowList: [10, 20, 30],
            pager: '#tbldevicesPager',
            sortname: 'ID',
            viewrecords: true,
            sortorder: "desc",
            caption: "devices result",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            onSelectRow: function (id) {                
                idDevices = '';
                var rd = $(this).getRowData(id);
                idDevices = rd.ID;
                devices = rd.NameDevices;
                //Edit this record
                //viewUser(id, rd);

                //This is how the data is updated in the grid...
                //var result = $('#tblCustomers').jqGrid('setRowData', gr, rd, '');

            }
        });
        $("#tbldevices").jqGrid('navGrid', '#tbldevicesPager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}
function createDevicesAcelGrid() {
    try {
        jQuery("#tblDevices_acel").jqGrid({
            datatype: "local",
            height: 200,
            width: 1200,
            colNames: ['ID', 'DeviceID', 'deviceName', 'CompanyID', 'CompanyName', 'Definition', 'Accel_Definition', 'configurationToSend'],
            colModel: [
                { name: 'ID', index: 'ID', title: false, hidden: true, width: 100 },
                { name: 'DeviceID', index: 'DeviceID', width: 100 },
                { name: 'deviceName', index: 'deviceName', width: 100 },
                { name: 'CompanyID', index: 'CompanyID', hidden: true, width: 100 },
                { name: 'CompanyName', index: 'CompanyName', width: 180 },
                { name: 'VehicleType', index: 'VehicleType', width: 300 },
                { name: 'currentConfiguration', index: 'currentConfiguration', width: 300 },                
                {
                    label: 'configurationToSend',
                    name: 'configurationToSend',
                    index: 'configurationToSend',
                    width: 200,
                    editable: true,
                    edittype: "select",
                    editoptions: {
                        value: listDevicesAcellConfigurationString// "ALFKI:ALFKI;ANATR:ANATR;ANTON:ANTON;AROUT:AROUT;BERGS:BERGS;BLAUS:BLAUS;BLONP:BLONP;BOLID:BOLID;BONAP:BONAP;BOTTM:BOTTM;BSBEV:BSBEV;CACTU:CACTU;CENTC:CENTC;CHOPS:CHOPS;COMMI:COMMI;CONSH:CONSH;DRACD:DRACD;DUMON:DUMON;EASTC:EASTC;ERNSH:ERNSH;FAMIA:FAMIA;FISSA:FISSA;FOLIG:FOLIG;FOLKO:FOLKO;FRANK:FRANK;FRANR:FRANR;FRANS:FRANS;FURIB:FURIB;GALED:GALED;GODOS:GODOS;GOURL:GOURL;GREAL:GREAL;GROSR:GROSR;HANAR:HANAR;HILAA:HILAA;HUNGC:HUNGC;HUNGO:HUNGO;ISLAT:ISLAT;KOENE:KOENE;LACOR:LACOR;LAMAI:LAMAI;LAUGB:LAUGB;LAZYK:LAZYK;LEHMS:LEHMS;LETSS:LETSS;LILAS:LILAS;LINOD:LINOD;LONEP:LONEP;MAGAA:MAGAA;MAISD:MAISD;MEREP:MEREP;MORGK:MORGK;NORTS:NORTS;OCEAN:OCEAN;OLDWO:OLDWO;OTTIK:OTTIK;PARIS:PARIS;PERIC:PERIC;PICCO:PICCO;PRINI:PRINI;QUEDE:QUEDE;QUEEN:QUEEN;QUICK:QUICK;RANCH:RANCH;RATTC:RATTC;REGGC:REGGC;RICAR:RICAR;RICSU:RICSU;ROMEY:ROMEY;SANTG:SANTG;SAVEA:SAVEA;SEVES:SEVES;SIMOB:SIMOB;SPECD:SPECD;SPLIR:SPLIR;SUPRD:SUPRD;THEBI:THEBI;THECR:THECR;TOMSP:TOMSP;TORTU:TORTU;TRADH:TRADH;TRAIH:TRAIH;VAFFE:VAFFE;VICTE:VICTE;VINET:VINET;WANDK:WANDK;WARTH:WARTH;WELLI:WELLI;WHITC:WHITC;WILMK:WILMK;WOLZA:WOLZA"
                    }
                },
                

            ],
            rowNum: 50,
            rowList: [10, 20, 30],
            pager: '#tblDevices_acelPager',
            sortname: 'ID',
            viewrecords: true,
            sortorder: "desc",
            caption: "Devices Result",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            onSelectRow: function (id) {
                
                deviceid = '';
                idCompany = '';
                var rd = $(this).getRowData(id);                
                deviceid = rd.ID;
                idCompany = rd.CompanyID;
               

            }
        }).navGrid("#tblDevices_acelPager", { edit: true, add: false, refresh: false, del: false, search: false, }, 
            {//edit options
                width: 600,
                height:100,
                url: "/crm.svc/putaccelerometer/" + tokenp,
                datatype: 'json',
                ajaxEditOptions: { contentType: "application/json" },
                //mtype:"POST",
                //datatype: 'json',
                //ajaxEditOptions: { contentType: "application/json" },
                closeAfterEdit: true,                
                serializeEditData: function (postData) {
                    PutAceleration = {};
                    PutAceleration.commandid = postData.configurationToSend;
                    PutAceleration.deviceid = deviceid;
                    
                    var json = JSON.stringify(PutAceleration);
                    return json;
                    
                    //
                    //var rowid = $('#tblDevices_acel').jqGrid('getGridParam', 'selrow');
                    //deviceid1 = $('#tblDevices_acel').jqGrid('getCell', 1, 'ID')
                    //
                    
                },
                //beforeSubmit: function (postdata) {
                //    
                //    comandid = postdata.configurationToSend;

                //},
                afterComplete: function (response) {
                    
                    getdevicesAccelerometer(idCompany);
                    //var response1 = JSON.parse(response.responseText);
                    //
                    //if (response1.isOk == 3) {
                    //    alert("The position of the sensor is registered");
                    //} else if (response1.isOk == 4) {
                    //    alert("The sensor exists in the device ");
                    //} else if (response1.isOk == 2) {
                    //    alert("The sensor is assigned to another device");
                    //}
                    //else {
                    //    $('#tblSensors').setGridParam({ datatype: 'json', page: 1 }).trigger('reloadGrid');
                    //    jQuery('#tblSensors').jqGrid('clearGridData');
                    //    jQuery('#tblDeviceName').jqGrid('clearGridData');
                    //    jQuery('#tblDeviceData').jqGrid('clearGridData');
                    //    jQuery('#tblDeviceInternalData').jqGrid('clearGridData');
                    //    loadDeviceData();
                    //}
                }
            },{},{});


    //    $("#tblDevices_acel").jqGrid('navGrid', '#tblDevices_acelPager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function getCrmDevices() {
    
    $("#tbldevices").jqGrid("clearGridData");
    try {
        var token = getTokenCookie('ETCRMTK');
        var devidesId = $("#inputDevices").val();
        //did = getParameterByName('did');
        var listDealers = [];
            $.ajax({
                //url: 'crm.svc/getCrmDealers?token=' + token,
                url: 'crm.svc/getCrmDevices?token=' + token + "&devicesId=" + devidesId,
                type: "POST",
                data: 0,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (data, textStatus, jqXHR) {
                    
                    jsonDeviceData = jQuery.parseJSON(data);
                    if (jsonDeviceData.Devices.length>0) {
                        populatetblDeviceNameDataGrid(jsonDeviceData)
                    } else {
                        alert('Not Information Devices');
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus);
                    listDealers = [];
                },
                cache: false,
                async: false
            });
        
        

        

    }
    catch (err) {

        alert('getNotes: ' + err.message);
    }
}
function populatetblDeviceNameDataGrid(jsonData) {
    try {
        for (var i = 0; i < jsonData.Devices.length; i++) {
            var row = eval('(' + jsonData.Devices[i] + ')');
            jQuery("#tbldevices").jqGrid('addRowData', i + 1, row);
            /*  //*/
        }
    }
    catch (err) {
    }
}

function assignDealers() {
    //var idDevices = '';
    var idDealers = '';
    var Dealers = '';
    try {        
        var token = getTokenCookie('ETCRMTK');
        idDealers = $("#cmbDealers").val();
        Dealers = $("#cmbDealers option:selected").text(); 
        if (idDealers == '' || idDealers == null) {
            alert('please select a dealer');
            return;
        }
        if (idDevices == '' || idDevices == null) {
            alert('please select a Devices');
            return;
        }

        $.confirm({
            title: 'Confirm!',
            content: '¿you are sure to move unit ' + devices + ' to dealer ' + Dealers+' ?',
            buttons: {
                confirm: function () {
                    
                    updateDealerts(token, idDevices, idDealers);                    
                    $.alert('update completed!');
                },
                cancel: function () {
                    $.alert('Canceled!');
                }
            }
        });

    } catch (e) {
        alert('Error '+e.message)
    }  


}

function updateDealerts(token, idDevices, idDealers) {

    $.ajax({
        //url: 'crm.svc/getCrmDealers?token=' + token,
        url: 'crm.svc/CRM_UpdateDealerDevices?token=' + token + "&idDevices=" + idDevices + "&idDealer=" + idDealers,
        type: "POST",
        data: 0,
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        processdata: true,
        success: function (data, textStatus, jqXHR) {
            getCrmDevices();
            $("#cmbDealers").val('');

        },
        error: function (jqXHR, textStatus, errorThrown) {

            alert('Error Updating: ' + textStatus)
        },
        cache: false,
        async: false
    });

}

function createCompanyGrid(table,tablepager) {
        

    try {
        /*jQuery("#tblcompanys").jqGrid({*/
        jQuery("#" + table).jqGrid({
            datatype: "local",
            height: 150,
            width: 1200,
            colNames: ['idCompany', 'idDealers', 'Dealers', 'Company', 'Phone', 'Email', 'CreatedOn'],
            colModel: [
                { name: 'idCompany', index: 'idCompany', title: false, hidden: true, width: 50 },
                { name: 'idDealers', index: 'idDealers', hidden: true, width: 50 },
                { name: 'nameDealers', index: 'nameDealers', width: 100 },
                { name: 'nameCompany', index: 'nameCompany', width: 200 },
                { name: 'Phone', index: 'Phone', width: 100 },
                { name: 'Email', index: 'idDEmailealers', width: 100 },
                { name: 'CreatedOn', index: 'CreatedOn', width: 150 }
               
            ],
            rowNum: 50,
            rowList: [10, 20, 30],
            pager: '#' + tablepager,
            sortname: 'idCompany',
            viewrecords: true,
            sortorder: "desc",
            caption: "Company result",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            onSelectRow: function (id) {                
                idCompany = '';
                var rd = $(this).getRowData(id);
                idCompany = rd.idCompany;
                getdevicesAccelerometer(idCompany);
                //company = rd.nameCompany;
                //dealers = rd.nameDealers;
            }
        });
        $("#" + table).jqGrid('navGrid', '#'+tablepager, { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function CRM_Get_Company(search,table) {
    /*$("#tblcompanys").jqGrid("clearGridData");*/
    $("#" + table).jqGrid("clearGridData");
    
    try {
        var jsonCompanys = [];
        var token = getTokenCookie('ETCRMTK');
       
        //did = getParameterByName('did');
        var listCompanys = [];
        $.ajax({
            url: 'crm.svc/CRM_GetCompanys?token=' + token + '&search=' + search,
            type: "POST",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                
                jsonCompanys = jQuery.parseJSON(data);

                populatetblCompanysDataGrid(jsonCompanys,table)


            },
            error: function (jqXHR, textStatus, errorThrown) {
                
                listDealers = [];
            },
            cache: false,
            async: false
        });





    }
    catch (err) {

        alert('getNotes: ' + err.message);
    }
}
function searchCompany(inputsearch,table) {
    
    //var input = $("#inputSearch").val();
    var input = $("#" + inputsearch).val();
    if (input.length>3) {
        CRM_Get_Company(input,table);
    }

}
function populatetblCompanysDataGrid(jsonData,table) {
    try {
        for (var i = 0; i < jsonData.Companys.length; i++) {
            var row = eval('(' + jsonData.Companys[i] + ')');
            //jQuery("#tblcompanys").jqGrid('addRowData', i + 1, row);
            jQuery("#" + table).jqGrid('addRowData', i + 1, row);

            /*  //*/
        }
    }
    catch (err) {
    }
}
function getCrmDealersCompany() {

    try {

        var token = getTokenCookie('ETCRMTK');


        $.ajax({
            //url: 'crm.svc/getCrmDealers?token=' + token,
            url: 'crm.svc/getCrmDealers/' + token,
            type: "POST",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                ;
                listDealers = jQuery.parseJSON(data);
                for (var i = 0; i < listDealers.length; i++) {
                    $("#cmbDealersCompany").append('<option value="' + listDealers[i].ID + '">' + listDealers[i].Name + '</option>');

                    console.log(listDealers[i].ID + " -> " + listDealers[i].Name);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ;
                listDealers = [];
            },
            cache: false,
            async: false
        });



    }
    catch (err) {

        alert('Error getCrmDealers: ' + err.message);
    }
}


function moveCompany() {
    
    //var idDevices = '';
    var idDealersCompany = '';
    try {
        var token = getTokenCookie('ETCRMTK');
        idDealersCompany = $("#cmbDealersCompany").val();
        dealers = $("#cmbDealersCompany option:selected").text();
        if (idDealersCompany == '' || idDealersCompany == null) {
            alert('please select a dealer');
            idCompany = '';
            return;
        }
        if (idCompany == '' || idCompany == null) {
            alert('please select a company');
            return;
        }

        $.confirm({
            title: 'Confirm!',
            content: '¿you are sure to move the company ' + company + ' to dealers ' + dealers+' ?',
            buttons: {
                confirm: function () {
                    updateMoveCompany(token, idDealersCompany, idCompany);
                    searchCompany();
                    $.alert('update completed!');
                },
                cancel: function () {
                    $.alert('Canceled!');
                }
            }
        });

    } catch (e) {
        alert('error: ' + e.message);
    }


}

function updateMoveCompany(token, DealersCompany, Company) {

    $.ajax({
        //url: 'crm.svc/getCrmDealers?token=' + token,
        url: 'crm.svc/CRM_updateMoveCompany?token=' + token + "&DealersCompany=" + DealersCompany + "&Company=" + Company,
        type: "POST",
        data: 0,
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        processdata: true,
        success: function (data, textStatus, jqXHR) {
            CRM_Get_Company();
            $("#cmbDealersCompany").val('');
            idCompany = '';

        },
        error: function (jqXHR, textStatus, errorThrown) {

            alert('Error Updating: ' + textStatus)
        },
        cache: false,
        async: false
    });

}

function clearGrid1() {

    $("#tbldevices").jqGrid("clearGridData");
    $("#inputDevices").val('');
}
function clearGrid2() {
    $("#tblcompanys").jqGrid("clearGridData");
    $("#inputSearch").val('');    
}
function clearGrid(table,inputsearch) {
    $("#" + table).jqGrid("clearGridData");
    $("#" + inputsearch).val('');
}
function getdevicesAccelerometer(companyid) {

    try {        
        var token = getTokenCookie('ETCRMTK');
        $.ajax({
            //url: 'crm.svc/getCrmDealers?token=' + token,
            url: 'crm.svc/getdevicesAccelerometer?token=' + token + '&companyid='+companyid,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                
                listDevicesAcell = data;// jQuery.parseJSON(data);
                
                populatetblDevicesAcellDataGrid(listDevicesAcell, "tblDevices_acel");
                
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
                listDealers = [];
            },
            cache: false,
            async: false
        });



    }
    catch (err) {

        alert('Error getCrmDealers: ' + err.message);
    }
}
function populatetblDevicesAcellDataGrid(jsonData, table) {
    try {
          
        $("#" + table).jqGrid("clearGridData");
        for (var i = 0; i < jsonData.length; i++) {
            var row = jsonData[i];/*eval('(' + jsonData[i] + ')');*/
            //jQuery("#tblcompanys").jqGrid('addRowData', i + 1, row);
            jQuery("#" + table).jqGrid('addRowData', i + 1, row);

            /*  //*/
        }
    }
    catch (err) {
    }
}
function getDevices_AccelConfiguration() {
    try {
        
        var token = getTokenCookie('ETCRMTK');
        $.ajax({
            //url: 'crm.svc/getCrmDealers?token=' + token,
            url: 'crm.svc/getAccelConfiguration?token=' + token,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                
                listDevicesAcellConfiguration = data;
                $.each(listDevicesAcellConfiguration, function (key, value) {
                    listDevicesAcellConfigurationString += value.ID.toString() + ":" + value.VehicleType.toString() + ";";

                });
                listDevicesAcellConfigurationString = listDevicesAcellConfigurationString.substring(0, listDevicesAcellConfigurationString.length - 1);

                //populatetblDevicesAcellDataGrid(listDevicesAcell, "tblDevices_acel");
                
            },
            error: function (jqXHR, textStatus, errorThrown) {

                listDealers = [];
            },
            cache: false,
            async: false
        });
    }
    catch (err) {

        alert('Error getCrmDealers: ' + err.message);
    }
}
