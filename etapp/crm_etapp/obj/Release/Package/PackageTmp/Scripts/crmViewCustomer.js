var jsonCustomer = false;
var uid = false;
var urlServicio = 'https://pre.etrack.ws/crm.svc';
function addNote(createdBy, createdOn, note) {
    try {
        var noteSpan = document.createElement('span');
        $(noteSpan).text(createdBy + ' on ' + createdOn + ': ' + note);

        var noteDiv = document.createElement('div');
        $(noteDiv).addClass('companyNote');
        noteDiv.appendChild(noteSpan);

        var notesDiv = document.getElementById('Notes');
        notesDiv.appendChild(noteDiv);

    }
    catch (err) {

    }
}

function getNotes() {
    try {
        var token = getTokenCookie('ETCRMTK');
        var notes = [];

        $.ajax({
            url: 'etrest.svc/getCompanyNotes?token=' + token + '&custId=' + uid,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                notes = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notes = [];
            },
            cache: false,
            async: false
        });

        for (var i = 0; i < notes.length; i++) {
            addNote(notes[i].createdBy, notes[i].createdOn, notes[i].note);
        }
        
    }
    catch (err) {
        alert('getNotes: ' + err.message);
    }
}

function saveNote() {
    try{
        var token = getTokenCookie('ETCRMTK');

        var note = $('#txtNewNote').val();

        if (note.length == 0) {
            alert("Please enter a note before saving");
            return;
        }

        var data = {
            companyId: uid,
            note: $('#txtNewNote').val()
        }

        data = JSON.stringify(data);

        $.ajax({
            url: 'etrest.svc/saveCompanyNote?token=' + token,
            type: "POST",
            data: data,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                addNote('Me', 'Now', note);
                $('#txtNewNote').val('');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var a = 1;
                alert('Failed saving note.  Please try again.');
            },
            async: false
        });

    }
    catch (err) {
        alert('saveNote: ' + err.message);
    }
}

function loadBillingControls() {
    try {
        var paymentMethods = $("#cbxPaymentMethods");
        paymentMethods.append($("<option />").val(1).text('Credit Card'));
        paymentMethods.append($("<option />").val(2).text('Check'));
        paymentMethods.append($("<option />").val(3).text('ACH'));

        var billingDay = $("#cbxMonthDays");
        for (i = 1; i < 31; i++) {
            billingDay.append($("<option />").val(i).text(i));
        }
    }
    catch (err) {
        alert('loadBillingControls: ' + err.description);
    }
}

function saveBillingDet() {
    try {
        if (uid != '') {
            var paymentMethod = $('#cbxPaymentMethods').val();
            var billingDay = $('#cbxMonthDays').val();
            var isVVIP = $('#isVVIP').is(':checked')

            var data = 't=' + getTokenCookie('ETCRMTK') + '&uid=' + escape(uid) + '&pm=' + paymentMethod + '&bd=' + billingDay + '&isVVIP=' + isVVIP;
            dbReadWrite('ETCRMWS.asmx', 'crmCustomerSaveBillingSpecs', data, true, false);
        }
    }
    catch (err) {
        alert('saveBillingDet: ' + err.description);
    }
}

function snoozeCollections() {
    try {
        var data = 't=' + getTokenCookie('ETCRMTK') + '&uid=' + escape(uid);
        dbReadWrite('ETCRMWS.asmx', 'crmCustomerSnoozeCollections', data, true, false);
        alert("Snoozed!");
    }
    catch (err) {
        alert('snoozeCollections: ' + err.description);
    }
}

function loadCustomer() {
    
    try {
        
        createCustomerGrids();
        uid = getParameterByName('uid');
        if (uid != '') {
            var data = '?token=' + getTokenCookie('ETCRMTK') + '&uid=' + escape(uid);
            //jsonCustomer = dbReadWrite('ETCRMWS.asmx', 'crmGetCustomerByUniqueKey', data, true, false);
            
            jsonCustomer = dbReadWriteWS(urlServicio, 'crmGetCustomerByUniqueKey', data, true, false);
            
            var company = jsonCustomer.ListCompanies// eval('(' + jsonCustomer.company[0] + ')');
            document.title = company[0].name;
            $('#companyName').text(company[0].name);
            $('#newCustomerCase').html(company[0].newCustomerCase);
            $('#cbxPaymentMethods').val(company[0].paymentMethod);
            $('#cbxMonthDays').val(company[0].billingDay);
            $("#isVVIP").prop('checked', company[0].isVVIP);

            populateUsersGrid(jsonCustomer.ListUsers);
            populateDevicesGrid(jsonCustomer.ListDevices);
        }
    }
    catch (err) {
    }
}

function createCustomerGrids() {
    try {
        createUsersGrid();
        createDevicesGrid();
    }
    catch (err) {
    }
}

function populateUsersGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var user = jsonData[i];//eval('(' + jsonData[i] + ')');
            jQuery("#tblUsers").jqGrid('addRowData', i + 1, user);
        }
    }
    catch (err) {
    }
}

function populateDevicesGrid(jsonData) {
    try {
        for (var i = 0; i <= jsonData.length; i++) {
            var device = jsonData[i];// eval('(' + jsonData[i] + ')');
            jQuery("#tblDevices").jqGrid('addRowData', i + 1, device);
        }
    }
    catch (err) {
    }
}

function createUsersGrid() {
    try {
        jQuery("#tblUsers").jqGrid({
            datatype: "local",
            height: 200,
            width: 1200,
            colNames: ['guid', 'id', 'Name', 'Login', 'Email', 'Phone', 'Cell', 'Last Login', 'QtyLogins', 'Time Zone', 'Driver', 'Login', 'Credentials Email', 'Case'],
            colModel: [
           		                    { name: 'guid', index: 'guid', title: false, hidden: true, width: 1 },
           		                    { name: 'id', index: 'id', width: 30 },
           		                    { name: 'name', index: 'name', width: 300 },
           		                    { name: 'userName', index: 'userName', width: 100 },
           		                    { name: 'email', index: 'email', width: 180 },
           		                    { name: 'phone', index: 'phone', width: 100 },
           		                    { name: 'cellPhone', index: 'cellPhone', width: 100 },
           		                    { name: 'lastLoginOn', index: 'lastLoginOn', width: 100 },
           		                    { name: 'qtyLogins', index: 'qtyLogins', width: 100 },
           		                    { name: 'timeZoneCode', index: 'timeZoneCode', width: 40 },
           		                    { name: 'isDriver', index: 'isDriver', width: 40 },
           		                    { name: 'userLogin', index: 'userLogin', width: 80 },
                                    { name: 'credentialsReminder', index: 'credentialsReminder', width: 100 },
           		                    { name: 'newUserCase', index: 'newUserCase', width: 80 },
           	                    ],
            rowNum: 50,
            rowList: [10, 20, 30],
            pager: '#tblUsersPager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Users",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            ondblClickRow: function (id) {
                var rd = $(this).getRowData(id);

                //Edit this record
                //viewUser(id, rd);

                //This is how the data is updated in the grid...
                //var result = $('#tblCustomers').jqGrid('setRowData', gr, rd, '');

            }
        });
        $("#tblUsers").jqGrid('navGrid', '#tblUsersPager', { edit: false, add: false, del: false });
    }
    catch (err) {
    }
}

function createDevicesGrid() {
    try {
        var isCarrierHidden = false;
        var isSimNumberHidden = false;
        var isSimPhoneHidden = false;

        if (isLimitedAccess == true) {
             isCarrierHidden = true;
             isSimNumberHidden = true;
             isSimPhoneHidden = true;
         }

        jQuery("#tblDevices").jqGrid({
            datatype: "local",
            height: 200,
            width: 1200,
            colNames: ['guid', 'id', 'Type', 'Dev.ID', 'Name', 'Last Update', 'Ev.Code', 'Ev.Date', 'GPSStat', 'S.N.', 'Carrier', 'SimNum', 'SimPhone', 'Note*', 'Case', 'Fee'],
            colModel: [
           		                    { name: 'guid', index: 'guid', title: false, hidden: true, width: 1 },
           		                    { name: 'id', index: 'id', width: 30 },
           		                    { name: 'deviceType', index: 'deviceType', width: 60 },
           		                    { name: 'deviceId', index: 'deviceId', width: 60 },
           		                    { name: 'name', index: 'name', width: 60 },
           		                    { name: 'lastUpdatedOn', index: 'lastUpdatedOn', width: 200 },
           		                    { name: 'eventCode', index: 'eventCode', width: 50 },
           		                    { name: 'eventDate', index: 'eventDate', width: 200 },
           		                    { name: 'gpsStatus', index: 'gpsStatus', width: 50 },
           		                    { name: 'serialNumber', index: 'serialNumber', width: 140 },
           		                    { name: 'carrier', index: 'carrier', hidden: isCarrierHidden, width: 80 },
           		                    { name: 'simNumber', index: 'simNumber', hidden: isSimNumberHidden, width: 100 },
           		                    { name: 'simPhone', index: 'simPhone', hidden: isSimPhoneHidden, width: 120 },
           		                    { name: 'note', index: 'note', width: 50 },
           		                    { name: 'newDeviceCase', index: 'newDeviceCase', width: 80 },
           		                    { name: 'monthlyFee', index: 'monthlyFee', width: 50 }
           	                    ],
            rowNum: 50,
            rowList: [10, 20, 30],
            pager: '#tblDevicesPager',
            sortname: 'id',
            viewrecords: true,
            sortorder: "desc",
            caption: "Devices",
            autowidth: true,  // set 'true' here
            shrinkToFit: false,// well, it's 'true' by default
            ondblClickRow: function (id) {
                var rd = $(this).getRowData(id);
                

                //Edit this record
                viewDeviceData(id, rd);

                //This is how the data is updated in the grid...
                //var result = $('#tblCustomers').jqGrid('setRowData', gr, rd, '');

            }
        });
        $("#tblDevices").jqGrid('navGrid', '#tblDevicesPager', { edit: false, add: false, del: false });

        $("#bedata").click(function () {
            // Gets the row number that has been selected...
            var gr = jQuery("#tblDevices").jqGrid('getGridParam', 'selrow');
            var rd = $("#tblDevices").getRowData(gr);

            //View this record
            viewDeviceData(gr, rd);

            //This is how the data is updated in the grid...
            //var result = $('#tblCustomers').jqGrid('setRowData', gr, rd, '');
        });
    }
    catch (err) {
    }
}

function btnViewDeviceData() {
    try {
        var gr = jQuery("#tblDevices").jqGrid('getGridParam', 'selrow');
        var rd = $("#tblDevices").getRowData(gr);

        //View this record
        viewDeviceData(gr, rd);
    }
    catch (err) {
    }
}

function viewDeviceData(rowId, data) {
    
    try {
        window.open('crmDeviceData.html?' + 'did=' + data.guid, target = "_blank");
    }
    catch (err) {
    }
}

function sendCredentialsReminderEmail(guid) {
    try {
        var data = 't=' + getTokenCookie('ETCRMTK') + '&guid=' + escape(guid);
        dbReadWrite('ETCRMWS.asmx', 'sendCredentialsEmail', data, true, false);
        alert('Email Sent');
    }
    catch (err) {
        alert('sendCredentialsReminderEmail: ' + err.description);
    }
}
