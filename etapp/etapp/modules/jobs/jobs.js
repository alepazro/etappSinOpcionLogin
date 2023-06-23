// Global module
var JobsModule = (function () {
    var filters_ = '';
    var isnew = false;
    function InitModule() {

        var template = _.template($('#template-jobs-filters').html(), {tab:0});
        //  //app.console("template --->" + template);
        $('#tabboard').append(template);

        var template = _.template($('#template-jobs-filters').html(), { tab: 1 });
        //  //app.console("template --->" + template);
        $('#tabcalendar').append(template);


        var template = _.template($('#template-jobs-filters').html(), { tab: 2 });
        //  //app.console("template --->" + template);
        $('#tabmaps').append(template);

        var template = _.template($('#template-jobs-filters').html(), { tab: 3 });
        //  //app.console("template --->" + template);
        $('#tabbyUser').append(template);


        var template = _.template($('#template-jobs-filters').html(), { tab: 4 });
        //  //app.console("template --->" + template);
        $('#tabbyType').append(template);


        var template = _.template($('#template-jobs-radios').html(), {tab:1});
        //  //app.console("template --->" + template);
        $('.dv_radio').append(template);

        var template = _.template($('#template-jobs-radios').html(), { tab: 2 });
        //  //app.console("template --->" + template);
        $('.dv_radio_maps').append(template);

        $('input[name=calendar_chk_1]:radio').click(function () {   //Hook the click event for selected elements

          var   selectedVal = $('input[name="calendar_chk_1"]:checked').val();
          var dataUnassgined = _.where(app.dataJobs, { userId: '' });
          var lstData = [];

          switch (selectedVal) {
              case '0':
                         lstData = _.sortBy(dataUnassgined, function (_job) { return _job.custName });
                  break;
              case '1':
                  lstData = _.sortBy(dataUnassgined, function (_job) { return _job.priorityName });
                  break;
              case '2':
                  lstData = _.sortBy(dataUnassgined, function (_job) { return  new Date(_job.start) }).reverse();
                  break;

          }
              
              dataUnassigned(lstData);
        });


        $('input[name=calendar_chk_2]:radio').click(function () {   //Hook the click event for selected elements

            var selectedVal = $('input[name="calendar_chk_2"]:checked').val();         
            var lstData = [];
         
            switch (selectedVal) {
                case '0':
                    lstData = _.sortBy(app.dataJobs, function (_job) { return _job.custName });
                    break;
                case '1':
                    lstData = _.sortBy(app.dataJobs, function (_job) { return _job.priorityName });
                    break;
                case '2':
                    lstData = _.sortBy(app.dataJobs , function (_job) { return new Date(_job.start) }).reverse();
                    break;

            }

            dataDVmaps(lstData);;
        });



        $('.btnNewJob').click(function () {

            ShowJobForm(null);      
        });


          $("#boardGrid").kendoGrid({
            dataSource: [],
            selectable: "row",
            dataBound: function () {

                $('#boardGrid table tr').dblclick(function () {
                    try {
                        var grid = $('#boardGrid').data("kendoGrid");
                        var selectedrow = grid.dataItem(grid.select());
                        if (selectedrow == null)
                            return;
                       

                        getJob(selectedrow.jobId);

                       // ShowJobForm(selectedrow);
                    }
                    catch (exc) {
                        _Show("GridView.dblClick -->"+exc.message);
                    }
                });

            },
            filterable: {
                extra: false,
            },
            sortable: true,
            pageable: true,
            columnMenu: false,
            groupable: {
                extra: true
            },
            //-----------------------------------------------------------------------------------
            columns: [
                {
                    title: "Status", width: 90, field: 'statusColor',
                    template: '<div class="bold">#=data.statusName #</div>',
                    attributes: {
                        "class": "table-cell",
                        style: "color:#:data.statusForeColor#;background:#=data.statusColor#;font-size:12px;padding:2px;"
                    }
                },
                  {
                      title: "Priority", width: 80, field: 'priorityName',
                      attributes: {
                          "class": "table-cell",
                          style: "font-size:12px;padding:2px;"
                      }
                  },
                {
                    title: "Type", width: 100, field: 'categoryName',
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;"
                    }
                },
                {
                    title: "Work Order #", width: 100, field: 'jobNumber',
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;"
                    }
                },
                {
                    title: "Customer", width: 300, field: 'custName',
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;"
                    }
                },
                {
                    title: "Scheduled Start", width: 120, field: 'dueDate',
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;"
                    },
                    template: '#= formatDate(dueDate)#'
                },
                {
                    title: "Duration", width: 75, field: 'durationHHMM',
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;"
                    }
                },
                {
                    title: "Sheduler End", width: 75,
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;",
                    },
                    template: '#= endDate(dueDate,estDuration)#'
                    ,hidden:true
                },



                {
                    title: "Assigned To", width: 200, field: 'userName',
                    attributes: {
                        "class": "table-cell",
                        style: "font-size:12px;padding:2px;"
                    }
                }
            ],
            pageable: {
                refresh: true,
                previousNext: true,
                info: true
            },
            scrollable: {
                virtual: true
            }
        });

       /*
        $("#content_panels").kendoSplitter({
            panes: [{
                collapsible: true,
                size: "24%"
            }, {}],
            resize: function (e) {
              
            },
            layoutChange: function (e) {
                //app.maps.changeResize();
            }
        });


        var splitter = $("#content_panels").data("kendoSplitter"); 
        splitter.expand(".k-pane:first");
       // splitter.trigger("resize");
     

        $("#content_panels_maps").kendoSplitter({
            panes: [{
                collapsible: true,
                size: "24%"
            }, {}],
            resize: function (e) {
               
            },
            layoutChange: function (e) {
                //app.maps.changeResize();
            }
        });

        var splitter = $("#content_panels_maps").data("kendoSplitter");
        splitter.expand(".k-pane:first");

        */
          var starttime = moment().format('YYYY/MM/DD') + ' 8:00';

          
         var statusList=[]
          $.each(app.dataStatus, function (i, status) {
              statusList.push({
                  color: status.color,
                  text: status.name,
                  value: status.id
              });
             
          });

        //  console.log("statusList -->" + JSON.stringify(statusList));

        $("#disp_scheduler").kendoScheduler({
            date: new Date(moment().format("YYYY/MM/DD")),
            dataSource: {
                data: []
            },
           // startTime: new Date(starttime),
            allDaySlot: false,
            footer: false,
            views: changeView('day'),
         //   eventTemplate: $("#event-template").html(),
            editable: {
                edit: true

            },
           // timezone: "Etc/UTC",
            selectable: true,
            edit: function (e) {
                console.log(" edit e -->" + JSON.stringify(e.event));
                //    e.container.show();
                $('.k-overlay').remove();
                $('.k-window').remove();
                //uc.openDispatch(e.event);            
                    ShowJobForm(e.event);
            },
            add: function (e) {
              
               // console.log(" e -->" + JSON.stringify(e.event));
               
            },
            resize: false,
            navigate: function (e) {
              //  console.log("navigate e -->" + JSON.stringify(e.event));

                resizeScheduler();
            },
            change: function (e) {
              //  console.log("change e -->" + JSON.stringify(e.event));
            },
            remove: function (e) {
            },
            moveEnd: function (e) {         
            },
            moveStart: false,
            resources: [
                  {
                      field: "statusId",
                      title: "",
                      dataSource: statusList
                  }
            ]
        });
        /*
        $(".k-event").tooltip({
            disabled: true
        });
        */
        /*
        $("#disp_scheduler").kendoTooltip({
            filter: ".k-event",
            position: "top",
            width: 250,
            content: kendo.template($("#event-template-tooltip").html())
        });
        */
        for (var i = 0; i <= 4; i++) {

            $("#ddlCustomers"+i).kendoComboBox({
                placeholder: " Select Customer",
                dataTextField: "name",
                dataValueField: "id",
                dataSource: app.dataCustomers,
                autoBind: true,
                dataBound: function (e) {
                },
                change: function () {

                    var multiselect = $('#ddlCustomers' + app.currentTab).data("kendoComboBox");
                    var value = multiselect.value();
                    app.selectedCustomer = value;
                    _Show("value -->" + value);
                }
            });

            $("#ddlPriority"+i).kendoComboBox({
                placeholder: " Select Priority",
                dataTextField: "name",
                dataValueField: "id",
                dataSource: app.dataPriority,
                autoBind: true,
                dataBound: function (e) {
                },
                change: function () {
                    var multiselect = $('#ddlPriority' + app.currentTab).data("kendoComboBox");
                  
                    var value = multiselect.value();
                    app.selectedPriotity = value;
                }
            });


            $("#ddlUsers"+i).kendoComboBox({
                placeholder: " Select User",
                dataTextField: "name",
                dataValueField: "uniqueId",
                dataSource: app.dataUsers,
                autoBind: true,
                dataBound: function (e) {
                },
                change: function () {
                    var multiselect = $('#ddlUsers' + app.currentTab).data("kendoComboBox");
                    var value = multiselect.value();
                    app.selectedUser = value;
                }
            });


            $("#ddlStatus"+i).kendoComboBox({
                placeholder: " Select Status",
                dataTextField: "name",
                dataValueField: "id",
                dataSource: app.dataStatus,
                autoBind: true,
                dataBound: function (e) {
                },
                change: function () {
                    var multiselect = $('#ddlStatus' + app.currentTab).data("kendoComboBox");
                    var value = multiselect.value();
                    app.selectedStatus = value;
                }
            });


            $("#ddlCategory" + i).kendoComboBox({
                placeholder: " Select Type",
                dataTextField: "name",
                dataValueField: "id",
                dataSource: app.dataCategories,
                autoBind: true,
                dataBound: function (e) {
                },
                change: function () {
                    var multiselect = $('#ddlCategory' + app.currentTab).data("kendoComboBox");
                    var value = multiselect.value();
                    app.selectedType = value;
                }
            });

            $('#datepickerFilter'+i).dateFilter({
                onChange: function (range, start, end) {
                    try {


                    }
                    catch (exc) {
                        app.excManager(uc.key, "reportrange.onChange", exc);
                    }
                },
                ranges: defaultRanges,
                enableCustom: true,
                defaultRange: 'today',
                maxDate: moment().add('months', 12)
            });


            $("#tbxJobNumber"+i).keyup(function () {
              var value = $(this).val();
              app.jobNumber = value;
          }).keyup();
        }
        var defaultRanges = {
            //Label: [ from, to, rangeCode, format ]
            'Today': [moment().startOf('day'), moment({ hour: 23, minute: 59, seconds: 59 }), 'today', '@From'],
            'Yesterday': [moment().startOf('day').subtract('days', 1), moment().subtract('days', 1).endOf('day'), 'yesterday', '@From'],
            'This Week': [moment().day('Monday'), moment({ hour: 23, minute: 59, seconds: 59 }), 'This Week', '@From -  @To'],
            'Last 7 Days ago': [moment().startOf('day').subtract('days', 6), moment().endOf('day'), '-7', '@From - @To'],
            'Last Week': [moment().day(-7).day('Monday'), moment().day('Sunday'), 'Last Week', '@From -  @To'],
            'Last 14 Days ago': [moment().startOf('day').subtract('days', 14), moment().endOf('day'), '-14', '@From - @To'],
            'Last 30 Days ago': [moment().startOf('day').subtract('days', 29), moment().endOf('day'), '-30', '@From - @To'],
            'This Month': [moment().startOf('day').startOf('month'), moment().endOf('month'), 'thismonth', '@From - @To'],
            'Last  Month': [moment().startOf('day').subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month'), 'lastmonth', '@From - @To']
        };

      

        
        $("#tabstrip").kendoTabStrip({
            animation: {
                open: {
                    effects: "fadeIn"
                }
            },
            activate: function (e) {
                //  _Show("activate -->" + JSON.stringify(e));
                loadTab();
            },
            select: function (e) {

                var x = e.item;
                app.currentTab = $(e.item).index();

                
              



                _Show("select main -->" + app.currentTab);
            }
        });

        $('.btnsetSearch').click(function () {
           
            loadData();
        });


       // $('#tlbByUser').bootstrapTable();


        Resize();

       // google.maps.event.addDomListener(window, 'load', loadmaps);
        loadmaps();
      
        resizeMap();
    }

    function loadTab() {
        
     
        loadFilter();

        switch (app.currentTab) {
            case 0:
              //  $('#map').hide();


                dataGrid();
                break;
            case 1:
             // $('#map').hide();
                dataScheduler();
               // resizeScheduler()
                break;
            case 2:           
                dataMaps();
                resizeMap();
                
                break;
            case 3:
              //  $('#map').hide();
                dataJobByUser();
                break;
            case 4:
             ///   $('#map').hide();
                dataJobByType();
                break;

        }

        loading(false);
    }


    function loadFilter() {


        if (app.selectedCustomer != '-1' ) {
            var multiselect = $('#ddlCustomers' + app.currentTab).data("kendoComboBox");
            multiselect.value(app.selectedCustomer);
        }

        if (app.selectedUser != '-1') {
            var multiselect = $('#ddlUsers' + app.currentTab).data("kendoComboBox");
            multiselect.value(app.selectedUser);
        }

        if ( app.selectedStatus != '-1') {
            var multiselect = $('#ddlStatus' + app.currentTab).data("kendoComboBox");
            multiselect.value(app.selectedStatus);
        }


        if ( app.selectedPriotity != '-1') {
            var multiselect = $('#ddlPriority' + app.currentTab).data("kendoComboBox");
            multiselect.value(app.selectedPriotity);
        }

        if (app.selectedType != '-1') {
            var multiselect = $('#ddlCategory' + app.currentTab).data("kendoComboBox");
            multiselect.value(app.selectedType);
        }


      
        $('#tbxJobNumber' + app.currentTab).val(app.jobNumber);

    }

    function loadData() {
        loading(true);
        var filters = {};
        var tbxJobNumber = $('#tbxJobNumber' + app.currentTab).val();

      

        if (tbxJobNumber.trim() != '') {
            filters['jobNumber'] = tbxJobNumber;
        }
        else {

            var value = ddlCombo_value('ddlCustomers');

            if (value.trim() != '') {
                filters['customerId'] = value;
            }

            var value = ddlCombo_value('ddlPriority');

            if (value.trim() != '') {
                filters['priorityId'] = value;
            }

            var value = ddlCombo_value('ddlUsers');

            if (value.trim() != '') {
                filters['assignedToId'] = value;
            }

            var value = ddlCombo_value('ddlStatus');

            if (value.trim() != '') {
                filters['statusId'] = value;
            }

            var value = ddlCombo_value('ddlCategory');

            if (value.trim() != '') {
                filters['categoryId'] = value;
            }
        }

        var data = 'token=' + app.tkn + '&filter={}';
        if (Object.keys(filters).length > 0) {
            /*
            var newfrom = $('#datepickerFilter' + app.currentTab).attr('data-from');
            var newto = $('#datepickerFilter' + app.currentTab).attr('data-to');

            filters["dueDateFrom"] = newfrom;
            filters["dueDateTo"] = newto;
            */
            data = 'token=' + app.tkn + '&filter=' + JSON.stringify(filters);
            filters_ = data;
            _Show('Filters -->> ' + data);

        }

        dbSync('jobs.svc', 'getJobsList', data, "GET", Jobs_callback);
    }


    function Jobs_callback(rs) {
        try{
            _Show('Data -->> ' + JSON.stringify(rs));
            $.each(rs, function (z, row) {
                row.jobendDate = endDate(row.dueDate, row.estDuration);
                row.start = moment(row.dueDate).format("YYYY/MM/DD hh:mm A");
                row.end = moment(row.jobendDate).format("YYYY/MM/DD hh:mm A");
                row.char = '#';
                row.title = '\n ' + row.char + row.jobNumber +
                            '\n Customer : ' + row.custName +
                            '\n Address : ' + row.custAddress +
                            '\n ' + moment(row.start).format("MMM DD YYYY hh:mm A").toUpperCase() + ' - ' + moment(row.end).format("MMM DD YYYY hh:mm A").toUpperCase() +
                            '\n Duration : ( ' + row.durationHHMM + ' )\n User : ' + row.userName +
                            '\n Status : ' + row.statusName;
                row.color = row.statusColor;
            });


            app.dataJobs = rs;

            loadTab();

            // dataScheduler();
      
            //  dataMaps();

            //   console.log("data" + JSON.stringify(grid.dataSource.data()));
            //loading(false);
        }
        catch (exc) {
            _Show('EXC Jobs_callback -->>  ' + exc.message);
        }
    }


    function dataGrid() {

        var grid = $("#boardGrid").data("kendoGrid");
        var localDataSource = new kendo.data.DataSource({
            data: app.dataJobs,
            pageSize: 20,
            serverPaging: false,
            serverSorting: false,
            cache: false,
            sort: {
                field: 'start',
                dir: 'desc'
            }
        });

        grid.setDataSource(localDataSource);
        grid.refresh();
    }


    function dataScheduler() {
        /*
        var dataSource = new kendo.data.SchedulerDataSource({
            transport: {
                read: {
                    url: app.apiUrl + "/" + 'jobs.svc' + "/" + 'getJobsList' + '?' + filters_,
                    dataType: "jsonp"
                }
            },
            schema: {
                model: {
                    id: "jobId",
                    fields: {
                        ID: { type: "number" },
                        title: { from: "jobNumber"},
                        start: { type: "date", from: "start" },
                        end: { type: "date", from: "jobendDate" },
                        statusForeColor: { from: "statusForeColor" },
                        statusColor: { from: "statusColor" },
                        custAddress: { from: "custAddress" },
                        custLat: { from: "custLat" },
                        custLng: { from: "custLng" },
                        ownerId: { from: "userId" },
                        jobNumber: { from: "jobNumber" },
                        char: { from: "char" },
                        durationHHMM: { from: "durationHHMM" },
                        jobId: { from: "jobId" },
                        companyId: { from: "companyId" },
                        custContact: { from: "custContact" },
                        custEmail: { from: "custEmail" },
                        custName: { from: "custName" },
                        custPhone: { from: "custPhone" },
                        customerId: { from: "customerId" },
                        dueDate: { from: "dueDate" },
                        estDuration: { from: "estDuration" },
                        isOk: { from: "isOk" },
                        jobDescription: { from: "jobDescription" },
                        msg: { from: "msg" },
                        notes: { from: "notes" },
                        picturesList: { from: "picturesList" },
                        priorityId: { from: "priorityId" },
                        priorityName: { from: "priorityName" },
                        signature: { from: "signature" },
                        statusColor: { from: "statusColor" },
                        statusForeColor: { from: "statusForeColor" },
                        statusId: { from: "statusId" },
                        statusName: { from: "statusName" },
                        userId: { from: "userId" },
                        userName: { from: "userName" },
                    }
                }
            }
        });

        */
        var dataSource = new kendo.data.SchedulerDataSource({
            data: app.dataJobs
        });

        var starttime = moment().format('YYYY/MM/DD') + ' 07:00';
        var endtime = moment().format('YYYY/MM/DD') + ' 23:00';

        var scheduler = $("#disp_scheduler").data("kendoScheduler");
        scheduler.date(new Date(moment().format("YYYY/MM/DD")));
        scheduler.startTime=new Date(starttime);
        scheduler.setDataSource(dataSource);

        //filtro los unassigneds 
        var dataUnassgined = _.where(app.dataJobs, { userId: '' });

        dataUnassigned(dataUnassgined);

        resizeScheduler();
    }

    function dataUnassigned(dataUnassgined) {
        try {
            $('.dv_unassigned').empty();
            $('#lblUnassigned').html('( ' + dataUnassgined.length + ' )');
            if (typeof (dataUnassgined) != 'undefined') {
                $.each(dataUnassgined, function (f, row) {
                    try {
                        var template = _.template($('#unassigned-template').html(), { job: row });

                        //  //app.console("template --->" + template);
                        $('.dv_unassigned').append(template);

                        if (f % 2 == 0)
                            $('#row_' + row.jobId).addClass("ColorRow");
                    }
                    catch (exc) {
                        _Show("dataScheduler " + exc.message);
                    }
                });
            }
        } catch (exc) {
            _Show("dataUnassigned " + exc.message);
        }
    }

    function dataMaps() {

        dataDVmaps(app.dataJobs);
        maps_callback(app.dataJobs);
     

    }

    function dataDVmaps(data) {
        try {
            $('.dv_jobs_maps').empty();

            $('#lblJobsMaps').html('( ' + data.length + ' )');

            if (typeof (data) != 'undefined') {
                $.each(data, function (f, row) {
                    try {
                        var template = _.template($('#dvmap-template').html(), { job: row });

                        //  //app.console("template --->" + template);
                        $('.dv_jobs_maps').append(template);

                        if (f % 2 == 0)
                            $('#rowmap_' + row.jobId).addClass("ColorRow");
                    }
                    catch (exc) {
                        _Show("dataDVmaps div " + exc.message);
                    }
                });
            }
        } catch (exc) {
            _Show("dataDVmaps " + exc.message);
        }
    }

    function dataJobByUser() {
        try{
            var listuserHeader = [];
            var listuserContent = [];
            $('#byUserTableheader').empty();
            $('#byUserTablebody').empty();


            var h = $('#bodyContent').height();
            $.each(app.dataUsers, function (z, user) {

                listuserHeader.push('<th style="width:290px;text-align:center;border:1px solid #fff;font-weight:bold;"><span>' + user.firstName + ' ' + user.lastName + '</span><span style="margin-left:3px;" id="labUser_' + user.uniqueId + '"><span></th>');
                listuserContent.push('<td  style="border:1px solid #ccc;vertical-align: top;" ><div style="height:' + (h - 125) + 'px' + ';overflow:auto;" class="dvUserContent" id="cont_' + user.uniqueId + '"></div></td>');
          
            });

          
            //$('.dvUserContent').css('height', (h - 100) + 'px');

            $('#byUserTableheader').append('<tr>' + listuserHeader.join('') + '</tr>');
            $('#byUserTablebody').append('<tr>' + listuserContent.join('') + '</tr>');


            $.each(app.dataUsers, function (i, user) {

                var jobs = _.where(app.dataJobs, { userId: user.uniqueId });

                $('#labUser_' + user.uniqueId).html('( ' + jobs.length + ' )');

                $.each(jobs, function (j, objJob) {

                    var template = _.template($('#user-template').html(), { job: objJob });
                   // _Show("template " + user.uniqueId + " --->" + (j % 2));
                    $('#cont_' + user.uniqueId).append(template);

                    if (j % 2 == 0)
                        $('#user_' + objJob.jobId).addClass("ColorRow");


                });          
            });

            resizeUser();

        }
        catch (exc) {
            _Show("dataJobByUser " + exc.message);
        }
   
    }

    function dataJobByType() {
        try {

        var listHeader = [];
        var listContent = [];
        $('#byTypeTableheader').empty();
        $('#byTypeTablebody').empty();


        var h = $('#bodyContent').height();
        $.each(app.dataCategories, function (z, type) {

            listHeader.push('<th style="width:315px;text-align:center;border:1px solid #fff;font-weight:bold;"><span>' + type.name+ '</span><span style="margin-left:3px;" id="labType_' + type.id + '"><span></th>');
            listContent.push('<td  style="border:1px solid #ccc;vertical-align: top;" ><div style="margin.top:0px;height:' + (h - 135) + 'px' + ';overflow-y:auto;overflow-x:hidden;" class="dvTypeContent" id="conttype_' + type.id + '"></div></td>');
          
        });

          
        //$('.dvUserContent').css('height', (h - 100) + 'px');

        $('#byTypeTableheader').append('<tr>' + listHeader.join('') + '</tr>');
        $('#byTypeTablebody').append('<tr>' + listContent.join('') + '</tr>');

        resizeType();

        $.each(app.dataCategories, function (i, cat) {

            var jobs = _.where(app.dataJobs, { categoryId: cat.id });

            $('#labType_' + cat.id).html('( ' + jobs.length + ' )');

            $.each(jobs, function (j, objJob) {

                var template = _.template($('#type-template').html(), { job: objJob });
              //  _Show("template " + user.uniqueId + " --->" + (j % 2));
                $('#conttype_' + cat.id).append(template);

                if (j % 2 == 0)
                    $('#cat_' + objJob.jobId).addClass("ColorRow");


            });          
        });



    }
        catch (exc) {
            _Show("dataJobByUser " + exc.message);
        }
    }

    function ShowJobForm(row) {
        try {
            var TITLE = '';
            // dejo el tamaño del modal al tamaño del contedor del dashboarxd
            var height = $('#bodyContent').height();

            if (row!=null && typeof (row.jobNumber) != 'undefined')
                TITLE = 'Job Number #' + row.jobNumber;
            else
                TITLE="New Job"


             showDialog({
                moduleKey: 'modJobForm',                      //ID del contener en modules
                dialogId: 'jobForm',                        //'Dialog DIV Container'
                title: TITLE,                                //Dialog Title
                template: 'Loading',                            //HTML Content inicial
                width: 750,
                height: height ,
                modal: true,
                top: 20
            });

             $('#jobForm').load('modules/' + 'jobs' + '/' + 'jobForm.html' + '?v=' + app.version, function () {
                  JobForm.SetJob(row); //Una vez se cargue el HTML, se envia a cargar el Survey ...
            });
        } catch (exc) {
            _Show("ShowJobForm "+exc.message);
        }

    }

    function Resize() {

        var h = $('#bodyContent').height();
        var w = $('#bodyContent').width();
        $('.dv_tabs').css('height', (h - 45) + 'px');
        $('#boardGrid').css('height', (h - 100) + 'px');

        $('#tlbByUser').css('height', (h - 100) + 'px');
        $('.dvUserContent').css('height', (h - 100) + 'px');
  

      

        

        /*

        $('#content_panels').css('height', (h - 100) + 'px');
        $('#content_panels_maps').css('height', (h - 100) + 'px');

        */


        var grid = $('#boardGrid').data("kendoGrid");
        grid.refresh();
       


        $('.dv_ua').css('height', (h - 55) + 'px');

        $('.dv_unassigned').css('height', (h -185) + 'px');
        $('.dv_jobs_maps').css('height', (h - 185) + 'px');
        
        /*
        var splitter = $("#content_panels").data("kendoSplitter");
        splitter.trigger("resize");
        splitter.expand(".k-pane:first");


        var splitterM = $("#content_panels_maps").data("kendoSplitter");
        splitterM.trigger("resize");
        splitterM.expand(".k-pane:first");
        */

        $('#UnAssigned_Container').css('height', (h - 185) + 'px');
        $('#contentUnAssigned').css('height', (h - 185) + 'px');
        

        var dc = $('#UnAssigned_Container').height();
        $("#disp_scheduler").css('height', (h - 102) + 'px');

        var dc1 = $('.jobs_maps').height();

        
        $('#contentMaps').css('height', (h - 185) + 'px');
        $('#map').css('height', (dc1 - 55) + 'px');


        resizeScheduler();
        resizeType()
        resizeUser();
        resizeMap();
    }


    function resizeMap() {
     
        try {
                   $('#map').show();
                   var w = $('#bodyContent').width();

                   var height = $('#bodyContent').height();
                   var offset = $('#map_container').offset();


                   // _Show("offset -->"+JSON.stringify(offset));
                    if (typeof (offset) != 'undefined') {
                        var top = offset.top;
                        var left = offset.left + 1;
                        //    var height = $('#map_container').height() - 1;

                        $("#map").css('top', 0 + "px");
                        $("#map").css('left', 0 + "px");
                        $("#map").css('height', height-100 + "px");
                        $('#map').width(w-335+'px');

                        if (app.map != null) {
                          
                            google.maps.event.trigger(app.map, "resize");


                            if (app.markers.length > 0)
                                zoomToBouns();
                        }
                    }
                    
            } catch (exc) {
                _Show("EXC resizeMap " + exc.message);
            }
    }

    function resizeType() {

        var h = $('#bodyContent').height();
        var wt = $('#bodyContent').width();

        $('#tlbByType').css('height', (h - 105) + 'px');
        $('.horizontal').css('height', (h - 100) + 'px');
        $('.horizontal').css('width', (wt-30) + 'px');
    }


    function resizeUser() {
        var h = $('#bodyContent').height();
        var wt = $('#bodyContent').width();

        $('#tlbByUser').css('height', (h - 105) + 'px');
        $('.horizontal').css('height', (h - 100) + 'px');
        $('.horizontal').css('width', (wt - 30) + 'px');
    }

    function resizeScheduler() {

        //$('.k-scheduler-layout').css('font-size', '12px');
        //$('.k-scheduler-table').css('font-size', '12px');
    }

    function changeView (opc) {
        var views = [
                   { type: "day", selected: false },
                   { type: "week", selected: false},
                   { type: "month", selected: false},
                   { type: "agenda", selected: false}
        ];
           
        var pustview = [];
        $.each(views, function (z, view) {

            // //app.console("viewmode -->" + view+' opc -->'+opc);
            if (view.type == opc) {
                view = { type: opc, selected: true };
            }

            pustview.push(view);
        });
           
        //app.console("view -->"+JSON.stringify(pustview));
        return pustview;
    }

    function getJob(JobID) {
        try{
            loading(true);
            var postData = JobID;
            dbSync('jobs.svc', 'getJob', postData, "GETJOB", getJob_callback);
        } catch (exc) {
            _Show("exc getJob :" + exc.message);
        }

    }

    function getJob_callback(rs) {

        ShowJobForm(rs);
    }

    function onUnassigned(jobID) {
        try {

            switch (app.currentTab) {
                case 1:
                    $('.jobUnassigned').removeClass("ColorSelected");
                    $('#row_' + jobID).addClass("ColorSelected");
                    break;
                case 2:
                    $('.jobMap').removeClass("ColorSelected");
                    $('#rowmap_' + jobID).addClass("ColorSelected");
                    MarkerZoomTo(jobID);
                    break;
                case 4:
                    $('.jobType').removeClass("ColorSelected");
                    $('#type_' + jobID).addClass("ColorSelected");
                  
                    break;
                case 3:
                    $('.jobUser').removeClass("ColorSelected");
                    $('#user_' + jobID).addClass("ColorSelected");               
                    break;

            }

      

        } catch (exc) {
            _Show("EXC onUnassigned " + exc.message);
        }
    }

    function ondblclick(jobID) {
        try{
            getJob(jobID);
            //var jobRow = _.where(app.dataJobs, { jobId: jobID })[0];
            //if (jobRow!=null && typeof (jobRow) != 'undefined') {
           
            //}
        } catch (exc) {
            _Show("EXC ondblclick " + exc.message);
        }
    }

    return {
        InitModule: function () {
            InitModule();
        },
        resize: function () {
            Resize();
        },
        loadData: function () {
            loadData();
        },
        onUnassigned: function ( jobID) {
          
            onUnassigned(jobID);
        },
        ondblclick: function (jobID) {
            ondblclick(jobID);
        }

    };

    // Pull in jQuery and Underscore
})();

