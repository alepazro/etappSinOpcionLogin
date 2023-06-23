//Create New Job
//- Type: Service, PM, Maintenance, PickUp/DropOff
//-- Save (Save, Save and Create New, Save and Create Copy) / Close (Are you sure you want to leave this job unsaved?)

//After save, the following options are presented:
//    * Pick Up
//    * Start
//    * Attention: Attention Reasons
//    * On-Hold: On-Hold Reasons
//    * Complete
//    * Cancel: Cancel Reasons
//    * Reopen

//Status: 
//    --> New: PickUp, Attention, On-Hold, Cancel
//--> Needs Attention: On-Hold, Complete, Cancel, ReOpen
//--> On Hold: Attention, Complete, Cancel, ReOpen
//--> Open-In Progress: Pause, Attention, On-Hold, Complete, Cancel

//All actions: Pick Up | Start | Pause | Attention | On-Hold | Complete | Cancel | ReOpen

//Other tables:
//    Priorities


var basicLists = {};
var customers = [];
var customer = {};
var token = getTokenCookie('ETTK');
token = '3F3C864A-2F05-42DC-ADB2-3446F40DDA85';

var __job = {
    statusId: 15, //Starts with Status=empty
    statusActions: [],
    toolbarButtons: [],
    setStatus: function () {
        var currentStatus = _.findWhere(basicLists.statuses, { value: __job.statusId.toString() });
        var cbxStat = $('#jobStatusName').data("kendoComboBox");
        if (cbxStat.dataSource.data().length == 0) {
            cbxStat.dataSource.data(basicLists.statuses);
        }
        cbxStat.value(__job.statusId);


        __job.statusActions = _.where(basicLists.statusActions, { statusId: __job.statusId });

        var actions = [];
        _.each(__job.statusActions, function (itm) {
            actions.push(_.pick(itm, 'actionId', 'actionName'));
        })

        var toolbar = $("#toolbar").data("kendoToolBar");
    
        _.each(__job.toolbarButtons, function (itm) {
            toolbar.remove($('#' + itm));
        });

        __job.toolbarButtons = [];
        var tbItems = [];
        _.each(__job.statusActions, function (itm) {
            tbItems.push({
                type: "button",
                id: "tbActionButton_" + itm.actionId,
                text: itm.actionName,
                click: function (e) {
                    __job.actionClickHandler(itm.actionId);
                }
            });
            __job.toolbarButtons.push("tbActionButton_" + itm.actionId);
        });

        _.each(tbItems, function (itm) {
            toolbar.add(itm);
        });
    },
    actionClickHandler: function(actionId){

        var newStatus = _.findWhere(basicLists.statusActions, { statusId: __job.statusId, actionId: actionId });

        __job.statusId = newStatus.targetStatusId;
        __job.setStatus();
    },
    setForm: function(){
        try {
            $('#jobCustomerForm').attr('isDirty', false);

            $('#jobCustomerForm').find(':input').keyup(function () {
                $('#jobCustomerForm').attr('isDirty', true);
            });
        }
        catch (err) {

        }
    },
    getJobLists : function(){
        //Load support lists
        var query = 'token=' + token;
        basicLists = getDb('jobs.svc', 'jobSupportTables', query, false);
        $("#cbxWorkZones").kendoComboBox({
            dataTextField: "name",
            dataValueField: "workZoneId",
            dataSource: basicLists.workZones,
            filter: "contains",
            suggest: true,
            index: 0
        });

        $("#cbxCategories").kendoComboBox({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: basicLists.categories,
            filter: "contains",
            suggest: true,
            index: -1
        });

        $("#cbxSpecialties").kendoComboBox({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: basicLists.specialties,
            filter: "contains",
            suggest: true,
            index: -1
        });

        $("#cbxPriorities").kendoComboBox({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: basicLists.priorities,
            filter: "contains",
            suggest: true,
            index: -1
        });

        $("#cbxUsers").kendoComboBox({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: basicLists.technicians,
            filter: "contains",
            suggest: true,
            index: -1
        });
    },
    loadCustomers : function(){
        try{
            //Load Customers
            var query = 'token=' + token + '&wzId=0';
            customers = getDb('jobs.svc', 'getCustomersIdName', query, false);
            $("#cbxCustomers").kendoComboBox({
                cascadeFrom: "cbxWorkZones",
                dataTextField: "text",
                dataValueField: "value",
                dataSource: customers,
                filter: "contains",
                suggest: true,
                index: -1,
                change: function (e) {
                    var value = this.value();
                    __job.getCustomer(value);
                }
            });

            var combobox = $("#cbxCustomers").data("kendoComboBox");
            combobox.refresh();
        }
        catch (err) {

        }
    },
    reloadCustomers: function(){
        try{
            var query = 'token=' + token + '&wzId=0';
            customers = getDb('jobs.svc', 'getCustomersIdName', query, false);
            $('#cbxCustomers').data('kendoComboBox').dataSource.data(customers)
        }
        catch (err) {

        }
    },
    getCustomer: function (id) {
        try {
            var query = 'token=' + token + '&custId=' + id;
            customer = getDb('jobs.svc', 'getCustomer', query, false);
            __job.loadCustomer(customer);
        }
        catch (err) {

        }
    },
    loadCustomer: function(cust){
        try{
            $('#txtContact').val(customer.contactName);
            $('#txtPhone').val(customer.phone);
            $('#txtEmail').val(customer.email);
            $('#txtAddress').val(customer.fullAddress);
        }
        catch(err){

        }
    },
    clearForm: function () {
        $('#jobCustomerForm').find(':input').not(':button, :submit, :reset, :hidden, .not-reset').val('');
        $('#jobCustomerForm').find(':checkbox, :radio').prop('checked', false);
    },
    main: function () {
        __job.setForm();
        __job.getJobLists();
        __job.loadCustomers();
        __job.setStatus();
    }
};

var __ce = {
    setupEditor: function () {
        try {
            $('#customerEditor').kendoWindow({
                width: "600px",
                height: "600px",
                title: "Customer Editor",
                visible: false,
                actions: [
                    "Pin",
                    "Minimize",
                    "Maximize",
                    "Close"
                ],
                close: __ce.onClose
            });
        }
        catch (err) {

        }
    },
    newCustomer: function(){
        try {
            if ($('#jobCustomerForm').attr('isDirty') == 'true') {
                if (confirm('There are unsaved changes.  Press CANCEL to go back or OK to continue.') == false) {
                    return;
                }
            }
            $('#jobCustomerForm').attr('isDirty', false);

            customer = {};
            __job.clearForm();

            $('#ceId').val('0');

            var editor = $('#customerEditor');
            editor.data("kendoWindow").center().open();
        }
        catch (err) {

        }
    },
    editCustomer: function(){
        try {
            $('#ceId').val(customer.id);
            $('#ceName').val(customer.name);

            $('#ceStreet').val(customer.street);
            $('#ceCity').val(customer.city);
            $('#ceState').val(customer.state);
            $('#cePostalCode').val(customer.postalCode);

            $('#ceContactName').val(customer.contactName);
            $('#cePhone').val(customer.phone);
            $('#ceEmail').val(customer.email);

            var editor = $('#customerEditor');
            editor.data("kendoWindow").center().open();
        }
        catch (err) {

        }
    },
    save: function () {
        var isNew = false;
        customer.id = $('#ceId').val();
        if(customer.id == '' || customer.id == '0'){
            isNew = true;
        }

        var origName = customer.name;
        customer.name = $('#ceName').val();

        if (customer.name == '') {
            alert('Please enter the customer name');
            return;
        }

        if ($('#ceStreet').val() == '' || $('#ceCity').val() == '' || $('#ceState').val() == '' || $('#cePostalCode').val() == '') {
            alert('Please complete all address fields: Street, City, State, and Postal Code');
            return;
        }

        var isAddressChanged = false;
        customer.origAddress = '';
        if (!_.isUndefined(customer.fullAddress)) {
            customer.origAddress = customer.fullAddress;
        }

        if (customer.street != $('#ceStreet').val()) {
            isAddressChanged = true;
            customer.street = $('#ceStreet').val();
        }
        if (customer.city != $('#ceCity').val()) {
            isAddressChanged = true;
            customer.city = $('#ceCity').val();
        }
        if (customer.state != $('#ceState').val()) {
            isAddressChanged = true;
            customer.state = $('#ceState').val();
        }
        if (customer.postalCode != $('#cePostalCode').val()) {
            isAddressChanged = true;
            customer.postalCode = $('#cePostalCode').val();
        }

        if (isAddressChanged == true) {
            customer.fullAddress = getFullAddress(customer);
            if (customer.fullAddress == '') {
                alert('Please enter a valid address');
                return;
            }
        }

        customer.contactName = $('#ceContactName').val();
        customer.phone = $('#cePhone').val();
        customer.email = $('#ceEmail').val();

        //var customer = {
        //    workZoneId: '0'
        //}


        if(customer.origAddress != customer.fullAddress){
            var geocoder = new google.maps.Geocoder();
            geocoder.geocode({ 'address': customer.fullAddress }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    //Process results
                    var jsonAddress = getGoogleAddressComponents(results[0]);
                    customer.street = jsonAddress.street;
                    customer.streetNumber = jsonAddress.streetNumber;
                    customer.route = jsonAddress.route;
                    customer.city = jsonAddress.city;
                    customer.county = jsonAddress.county;
                    customer.state = jsonAddress.state;
                    customer.postalCode = jsonAddress.postalCode;
                    customer.fullAddress = jsonAddress.fullAddress;
                    customer.lat = jsonAddress.lat;
                    customer.lng = jsonAddress.lng;

                    __ce.afterSave(customer, isNew);

                } else {
                    alert("Please enter a valid address.  Geocode was not successful for the following reason: " + status);
                    return;
                }
            });
        }
        else {
            __ce.afterSave(customer, isNew);
        }
    },
    afterSave: function(customer, isNew){
        try{
            var param = 'token=' + token;
            var data = JSON.stringify(customer);
            customer = postDb('jobs.svc', 'postJobCustomer', data, param);

            if (isNew == true) {
                __job.reloadCustomers();
            }
            __job.getCustomer(customer.id);

            var cbxWZ = $("#cbxWorkZones").data("kendoComboBox")
            cbxWZ.value(customer.workZoneId);

            var cbxCustomers = $("#cbxCustomers").data("kendoComboBox")
            cbxCustomers.value(customer.id);

            __ce.closeEditor();
        }
        catch (err) {

        }
    },
    closeEditor: function(){
        try {
            var editor = $('#customerEditor');
            editor.data("kendoWindow").close();
        }
        catch (err) {

        }
    },
    onClose: function () {
        __ce.clearForm();
    },
    clearForm: function () {
        $('#customerEditor').find(':input').not(':button, :submit, :reset, :hidden, .not-reset').val('');
        $('#customerEditor').find(':checkbox, :radio').prop('checked', false);
    },
    main: function () {
        __ce.setupEditor();
    }
};
$(document).ready(function () {
    $("#toolbar").kendoToolBar({
        items: [
            { template: "<label>Status:</label>" },
            { template: '<input id="jobStatusName" readonly="readonly" />', overflow: "never" },
            { type: "separator" },
            { template: "<label>Action:</label>" }
        ]
    });

    $('#jobStatusName').kendoComboBox({
        dataSource: [],
        dataTextField: "text",
        dataValueField: "value",
        readonly: true
    });

    $("#datScheduledStart").kendoDateTimePicker({
        value: new Date()
    });
    $("#datDueBy").kendoDateTimePicker({
        value: new Date()
    });

    $("#btnAddCustomer").kendoTooltip({
        autoHide: true
    });

    $("#btnEditCustomer").kendoTooltip({
        autoHide: true
    });

    $("#btnReloadCustomers").kendoTooltip({
        autoHide: true
    });

    __ce.main();
    __job.main();

    $('#jobCustomerForm').show();
});

