var JobForm = (function () {
    var currentTabForm = -1;
    var job = null;
    var validator = null;
    var default_time = '3:0';
    var currentcustomer = null;
    var currentModule = 1; //modulo de creacion de jobs
    var clear = false;
    var firstImg = null;
    var firstload = false;
    var firstloadSign = false;

    function InitModule() {

        $("#tabstripJob").kendoTabStrip({
            animation: {
                open: {
                    effects: "fadeIn"
                }
            },
            activate: function (e) {
                //  _Show("activate -->" + JSON.stringify(e));
                switch (currentTabForm) {
                    case 2: //imagenes
                        if (!firstload) {
                            if (firstImg != null) {
                                onImgClick(firstImg, 1);
                                firstload = true;
                            }
                        }
                        break;
                    case 3: //signature
                        if (!firstloadSign) {
                          //  onImgClick(firstImg, 0);
                            firstload = true;
                        }
                        break;
                }
            },
            select: function (e) {

                var x = e.item;
                currentTabForm = $(e.item).index();
                _Show("currentTabForm -->" + currentTabForm);
            }
        });


        validator = $("#tabstripJob").kendoValidator().data("kendoValidator");

        $("#btnJobFormSave").click(function () {
            //validate the input elements and check if there are any errors
            if (validator.validate() === false) {
                // get the errors and write them out to the "errors" html container
                var errors = validator.errors();
                $(errors).each(function () {                 
                    alert('Fields in red are required');
                });
            }
            else {
                app.currentModule = currentModule;
                clear = false;
                geocodingAddrress();
               // SaveValues();
            }
        });


        $("#btnJobSaveNew").click(function () {
            //validate the input elements and check if there are any errors
            if (validator.validate() === false) {
                // get the errors and write them out to the "errors" html container
                var errors = validator.errors();
                $(errors).each(function () {
                    alert('Fields in red are required');
                });
            }
            else {
                app.currentModule = currentModule;
                clear = true;
                geocodingAddrress();
               // SaveValues(true);  //limpio el formulario
            }
        });

        $('#btnJobFormCancel').click(function () {

            cancel();
        });

        $('#btnAddCostumer').click(function () {


          
                ShowCostumerForm(null);
        
           
        });

        $('#btnUpdCostumer').click(function () {

            ShowCostumerForm(currentcustomer);
        });

        $("#ddlFormCustomer").kendoComboBox({
            placeholder: " Select Customer",
            dataTextField: "name",
            dataValueField: "id",
            dataSource: app.dataCustomers,
            autoBind: true,
            dataBound: function (e) {
            },
            change: function () {
                var multiselect = $('#ddlFormCustomer').data("kendoComboBox");
                var value = multiselect.value();
                var data = multiselect.dataItem();

                if (typeof (data) != 'undefined') {

                    currentcustomer = data;
                  //  _Show("Customer -->" + JSON.stringify(data));

                    $('#tbxContact').val(data.contactName);
                    $('#tbxEmail').val(data.email);
                    $('#tbxAddress').val(getfulladdress(data));
                    $('#tbxPhone').val(data.phone);


                }

            }
        });

        $("#dllUserForm").kendoComboBox({
            placeholder: " Select User",
            dataTextField: "name",
            dataValueField: "uniqueId",
            dataSource: app.dataUsers,
            autoBind: true,
            dataBound: function (e) {
            },
            change: function () {
                var multiselect = $('#dllUserForm').data("kendoComboBox");
                var value = multiselect.value();
            }
        });


        $("#ddlStatusForm").kendoComboBox({
            placeholder: " Select Status",
            dataTextField: "name",
            dataValueField: "id",
            dataSource: app.dataStatus,
            autoBind: true,
            dataBound: function (e) {
            },
            change: function () {
                var multiselect = $('#ddlStatusForm').data("kendoComboBox");
                var value = multiselect.value();
            }
        });

        $("#ddlPriorityForm").kendoComboBox({
            placeholder: " Select Priority",
            dataTextField: "name",
            dataValueField: "id",
            dataSource: app.dataPriority,
            autoBind: true,
            dataBound: function (e) {
            },
            change: function () {
                var multiselect = $('#ddlPriorityForm').data("kendoComboBox");

                var value = multiselect.value();
            }
        });

        $("#ddlCategoryForm").kendoComboBox({
            placeholder: " Select Type",
            dataTextField: "name",
            dataValueField: "id",
            dataSource: app.dataCategories,
            autoBind: true,
            dataBound: function (e) {
            },
            change: function () {
                var multiselect = $('#ddlCategoryForm').data("kendoComboBox");
                var value = multiselect.value();
            }
        });

        $("#tbxDateForm").kendoDateTimePicker({
            value: new Date(moment().format("MM/DD/YYYY hh:mm A"))
        });

        $("#tbxTimeForm").kendoTimePicker({
            format: "HH:mm",
            parseFormats: ["HH:mm"],
            min: new Date(2000, 0, 1, 0, 0, 0), //date part is ignored
            interval: 5
        });

        Resize();
    }

    function Resize() {
        var h = $('#bodyContent').height();
        $('.dv_tabsForm').css('height', (h - 100) + 'px');


        

        $('.JobNotes').css('height', (h - 100) + 'px');
        $('.dvpics').css('height', (h - 102) + 'px');
        $('.dvpics_Content').css('height', (h - 102) + 'px');

        
        
    }

    function setJob(job_) {
        try{
            job = job_;


           // _Show("Job -->" + JSON.stringify(job));

            if (job != null || typeof(job.jobId)=='undefined') {

           

        
       
                if (job.customerId != '') {
                    var multiselect = $('#ddlFormCustomer').data("kendoComboBox");
                    multiselect.value(job.customerId);
                    currentcustomer = multiselect.dataItem();

                }

                $('#txbJobNumberForm').val(job.jobNumber);
                $('#tbxContact').val(job.custContact);
                $('#tbxEmail').val(job.custEmail);
                $('#tbxAddress').val(job.custAddress);
                $('#tbxPhone').val(job.custPhone);
                $('#tbxDescription').val(job.jobDescription);

                if (job.userId != '') {
                    var multiselect = $('#dllUserForm').data("kendoComboBox");
                    multiselect.value(job.userId);
                }
        
                if (job.statusId != '') {
                    var multiselect = $('#ddlStatusForm').data("kendoComboBox");
                    multiselect.value(job.statusId);
                }

          
                if (job.priorityId != '') {
                    var multiselect = $('#ddlPriorityForm').data("kendoComboBox");
                    multiselect.value(job.priorityId);
                }

                if (job.categoryId != '') {
                    var multiselect = $('#ddlCategoryForm').data("kendoComboBox");
                    multiselect.value(job.categoryId);
                }


                if (job.dueDate != "") {

                    var timepicker = $("#tbxDateForm").data("kendoDateTimePicker");
                    timepicker.value(new Date(moment(job.dueDate).format("MM/DD/YYYY hh:mm A")));
                }


                if (job.estDuration != "") {

                    var duration = job.durationHHMM.split(':');

                    var hh = parseInt(duration[0]);
                    var mm = parseInt(duration[1]);

                    var timepicker = $("#tbxTimeForm").data("kendoTimePicker");

                    timepicker.value(hh + ':' + mm);

                }


                if (job.notes != null && typeof (job.notes) != 'undefined') {

                    $.each(job.notes, function (h, note) {
                        try {
                            var template = _.template($('#note-template').html(), { index: h+1,note:note });

                            //  //app.console("template --->" + template);
                            $('.JobNotes').append(template);

                        }
                        catch (exc) {
                            _Show("setJob Notes " + exc.message);
                        }
                    });
                }


                if (job.picturesList != null && typeof (job.picturesList) != 'undefined') {

                    $.each(job.picturesList, function (h, pic) {
                        try {
                            var template = _.template($('#picture-template').html(), { index: h + 1, pic: pic });

                            //  //app.console("template --->" + template);
                            $('.dvpics').append(template);

                            if (h % 2 == 0)
                                $('#imgJob_' + pic.imageId).addClass("ColorRow");


                            if (h == 0)
                                firstImg = pic.imageId;
                        }
                        catch (exc) {
                            _Show("setJob JobPictures " + exc.message);
                        }
                    });
                }


                if (job.signature != null && typeof (job.signature) != 'undefined') {

                    var template = _.template($('#signature-template').html(), { sign: job.signature });

                    $('.JobSignatures').append(template);

                }

            }
            else {
                var tabStrip = $("#tabstripJob").kendoTabStrip().data("kendoTabStrip");
                tabStrip.remove("li:last");
                tabStrip.remove("li:last");
                tabStrip.remove("li:last");
            
          
                DefaultValue();
            }
        }
        catch (exc) {
            _Show("EXC setJob -->" + exc.message);
        }
    }

    function DefaultValue() {
        var multiselect = $('#ddlStatusForm').data("kendoComboBox");
        multiselect.value('1');


        var multiselect = $('#ddlPriorityForm').data("kendoComboBox");
        multiselect.value('owqiQpXWoV');


        var multiselect = $('#ddlCategoryForm').data("kendoComboBox");
        multiselect.value('fdferygf45');


        var timepicker = $("#tbxDateForm").data("kendoDateTimePicker");
        timepicker.value(new Date(moment().format("MM/DD/YYYY hh:mm A")));

        var timepicker = $("#tbxTimeForm").data("kendoTimePicker");

        timepicker.value(default_time);

    }

    function clearForm() {
        var multiselect = $('#ddlFormCustomer').data("kendoComboBox");
        multiselect.value('');
        job = null;       
        $('#txbJobNumberForm').val('');
        $('#tbxContact').val('');
        $('#tbxEmail').val('');
        $('#tbxAddress').val();
        $('#tbxPhone').val('');
        $('#tbxDescription').val('');
        var multiselect = $('#dllUserForm').data("kendoComboBox");
        multiselect.value('');

        DefaultValue();
    }

    function SaveValues(clear) {
        try{
            var token = app.tkn;


            var jobId = '';
            var jobNumber = '';

            if ($('#txbJobNumberForm').val().trim() != '') {
                jobNumber = $('#txbJobNumberForm').val();
            }

            if (job != null && typeof (job.jobId) != 'undefined') {
                jobId = job.jobId;
            }
     
            var custId = ddlComboNormal_value('ddlFormCustomer');
            var custContact = $('#tbxContact').val();
            var address = $('#tbxAddress').val();
            var custPhone = $('#tbxPhone').val();
            var custEmail = $('#tbxEmail').val();   
            var jobDetails = $('#tbxDescription').val();
            var assignedToId = ddlComboNormal_value('dllUserForm');

    
            var statusId = ddlComboNormal_value('ddlStatusForm');
            var priorityId = ddlComboNormal_value('ddlPriorityForm');
            var categoryId = ddlComboNormal_value('ddlCategoryForm');


            var datetimepicker = $("#tbxDateForm").data("kendoDateTimePicker");
            var dueDate = $("#tbxDateForm").val();// datetimepicker.value();

            var timestr1 = $("#tbxTimeForm").val();
            if (timestr1.trim() == '' || timestr1.trim()== '00:00' || timestr1.trim()=='0:00')
                timestr1 = default_time;
            var timestr = timestr1.split(':');
            var timeInt = parseInt(timestr[0]) * 60 + parseInt(timestr[1]);
            if (address.trim() != '') {
                var estDuration = timeInt;


                if (app.location == null) {
                    app.location = {
                        lat: currentcustomer.lat,
                        lng: currentcustomer.lng
                    };
                }

                var postData = {
                    jobId: jobId,
                    jobNumber: jobNumber,
                    customerId: custId,
                    custContact: custContact,
                    custAddress: address,
                    custPhone: custPhone,
                    custEmail: escape(custEmail),
                    //    dueDate: escape(dueDate),
                    dueDate: dueDate,
                    jobDescription: jobDetails,
                    userId: assignedToId,
                    custLat: app.location.lat,
                    custLng: app.location.lng,
                    statusId: statusId,
                    priorityId: priorityId,
                    categoryId: categoryId,
                    estDuration: estDuration
                };

                postData = JSON.stringify(postData);

                //_Show("Post Data -->" + postData);


                dbSync('jobs.svc', 'postJob', postData, "POST", JobForm_callback, clear);
            } else
                alert('Please enter Costumer Address');
        } catch (exc) {
            _Show("EXC Error -->" + exc.message);
        }
       
    }

    function JobForm_callback(rs, clear) {
        try {

            if (typeof (rs.resultCode) != 'undefined') {
                alert("EXC  JobForm_callback -->" + rs.resultMsg);
                return;
            }

            if (rs.isOk) {
                if(job==null)
                    alert("The Work Order #"+rs.docNum+" was created successfully ");
                else
                    alert("The Work Order #" + rs.docNum + " was updated successfully ");
                JobsModule.loadData();
                if (typeof (clear) != 'undefined' || clear==true) {
                    clearForm();
                }
                else {
                    cancel();
                }
            }
            else {
                alert("Error --> " + rs.msg);
            }

        } catch (exc) {
            _Show("EXC JobForm_callback -->" + exc.message);

        }
    }

    function cancel() {
        app.currentModule = 0;// siempre se debe poner para que las funciones genericas queden en estado inicial
        $("#jobForm").data('kendoWindow').close();
    }

    function geocodingAddrress() {
        var address = $('#tbxAddress').val();

        if (currentcustomer.lat == 0 && currentcustomer.lng == 0)
            getGeoCoder(address);
        else {
            SaveValues();
        }
    }

    function onImgClick(imgId, index) {
        loading(true);
        if(index!=0)
            $('.dvpics_Content').empty();
 

        data = 'token=' + app.tkn + '&imageId=' + imgId;
        dbSync('jobs.svc', 'getImage', data, "GET", onImgClick_callcaback,index);
    }

    function onImgClick_callcaback(rs,index) {
        //_Show("Imagenn -->> " + JSON.stringify(rs));
        if (index != 0) {
            var template = _.template($('#img-template').html(), { img: rs, index: index });

            $('.dvpics_Content').append(template);
        }
        else {

            $('#imgSignature').attr('src', rs.imgData);
            $('#imgSignature').show();
            $('#imglnkSing').hide();
            firstloadSign = true;           
        }

        loading(false);
    }

    function ShowCostumerForm(customer) {
        try {

            var value = ddlComboNormal_value('ddlFormCustomer');
         //   alert("valor -->" + value + ' customer-->' + JSON.stringify(customer));
            if (value == '')
                customer = null;
          
            var TITLE = '';

            if (customer != null)
                TITLE = 'Update Customer ' + customer.name;
            else
                TITLE = 'Create / Update Customer '

            // dejo el tamaño del modal al tamaño del contedor del dashboarxd
           // var height = $('#bodyContent').height();

            showDialog({
                moduleKey: 'modcostumerForm',                      //ID del contener en modules
                dialogId: 'customerForm',                        //'Dialog DIV Container'
                title: TITLE,                                //Dialog Title
                template: 'Loading',                            //HTML Content inicial
                width: 670,
                height: 250,
                modal: true,
                top: 20,
                closeCallback: function () {
                    try{
                        //_Show("Customer new --> " + JSON.stringify(customerForm.getCustomer()));
                        var customer = customerForm.getCustomer();

                        if (customer != null) {
                        
                            currentcustomer = customer;

                            $('#tbxContact').val(currentcustomer.contactName);
                            $('#tbxEmail').val(currentcustomer.email);
                            $('#tbxPhone').val(currentcustomer.phone);


                            var location = customerForm.getLocation();
                            if (location != null) {
                                $('#tbxAddress').val(getfulladdress(location));
                            }
                       
                            var combox = $("#ddlFormCustomer").data("kendoComboBox");
                            combox.setDataSource(app.dataCustomers);

                       
                            if(currentcustomer.id==0){

                                var obj = _.where(app.dataCustomers, { name: currentcustomer.name })[0];

                                if (obj != null && typeof (obj) != 'undefined') {
                                    combox.value(obj.id);
                                }   
                            }


                        }
                    } catch (exc) {
                        _Show("Exc Close Create Customer " + exc.message);
                    }

                    }
             
            });

            $('#customerForm').load('modules/' + 'jobs' + '/' + 'costumerForm.html' + '?v=' + app.version, function () {
                customerForm.SetCostumer(customer); //Una vez se cargue el HTML, se envia a cargar el Survey ...
            });
        } catch (exc) {
            _Show("ShowJobForm " + exc.message);
        }

    }

    return {
        InitModule: function () {
            InitModule();
        },
        resize: function () {
            Resize();
        },
        SetJob: function (job_) {
            setJob(job_);
            loading(false);

        },
        SaveJob: function () {

            if(!clear)
                SaveValues();
            else
                SaveValues(true);
        },
        onImgClick: function (imgId,index) {
           
         
            onImgClick(imgId,index);
        }
    };

    // Pull in jQuery and Underscore
})();

JobForm.InitModule();
app.currentModule = 2;
