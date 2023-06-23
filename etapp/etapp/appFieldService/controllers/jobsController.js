
etApp.controller('jobsController', ['$scope', 'SupportLists', 'Job', function ($scope, SupportLists, Job) {

    //Get basic tables
    //===========================================================
    var token = getTokenCookie('ETTK');
    var noCache = Math.floor((Math.random() * 100000) + 1);
    var jobId;
    SupportLists.getAll({ token: token, noCache: noCache }, function (data) {
        $scope.workZones = data.workZones;
        $scope.woStatus = data.statuses;
        $scope.technicians = data.technicians;
        $scope.categories = data.categories;
        $scope.priorities = data.priorities;
        $scope.specialties = data.specialties;

        $scope.dsWorkZones = new kendo.data.DataSource({
            data: $scope.workZones
        });

        $scope.dsWOStats = new kendo.data.DataSource({
            data: $scope.woStatus
        });

        $scope.dsTechnicians = new kendo.data.DataSource({
            data: $scope.technicians
        });

    }, function (err) {
        var a = 1;
    });
    //===========================================================

    $scope.jobs = [];

    $scope.jobFilterStatus = '0';
    $scope.jobFilterWorkZoneId = '0';
    $scope.jobFilterAssignedToId = '0';


    // SET DATA SOURCES
    //==============================================
    $scope.dsJobs = new kendo.data.DataSource({
        data: []
    });
    //==============================================
    // SET GRIDS COLUMNS
    //==============================================
    $scope.woStatsGridOptions = {
        selectable: 'row',
        columns: [
            { field: 'name', title: 'Job Status' },
            { field: 'qty', title: 'Quantity' },
            { field: 'overdue', title: 'Overdue' }
        ],
        change: function (e) {
            var selectedRows = this.select();
            if (selectedRows.length > 0) {
                var selRow = this.dataItem(selectedRows[0]);
                $scope.jobFilterStatus = selRow.id;
                $scope.loadJobs();
            }
        }
    }

    $scope.wzGridOptions = {
        selectable: 'row',
        columns: [
            { field: "name", title: "Work Zones" }
        ],
        change: function (e) {
            var selectedRows = this.select();
            if (selectedRows.length > 0) {
                var selRow = this.dataItem(selectedRows[0]);
                var wzId = selRow.id;
                $scope.jobFilterWorkZoneId = selRow.id;
                //$scope.jobFilterAssignedToId = '0'; //Reset the AssignedToId filter when there is a change in Work Zone

                //var token = getCookie('ETTK');
                //Technician.getTechnicians({token: token, wzId: wzId}, function (data) {
                //    technicians = data;

                //    var ds = new kendo.data.DataSource({ data: data });
                //    var grid = $('#techsGrid').data("kendoGrid");
                //    grid.setDataSource(ds); 

                //}, function (arg) {
                //    var a = 1;
                //});

                $scope.loadJobs();

            }
        }
    };

    $scope.techsGridOptions = {
        selectable: 'row',
        columns: [
            { field: 'name', title: 'Technicians' }
        ],
        change: function (e) {
            var selectedRows = this.select();
            if (selectedRows.length > 0) {
                var selRow = this.dataItem(selectedRows[0]);
                $scope.jobFilterAssignedToId = selRow.id;
                $scope.loadJobs();
            }
        }
    };

    //==============================================
    $scope.jobsGridOptions = {
        selectable: 'row',
        sortable: true,
        resizable: true,
        pageable: true,
        columns: [
            {
                command: {
                    text: "View Traking", click: function (e) {                       
                        e.preventDefault();
                        
                        var selectedRows1 = this.select();
                        if (selectedRows1.length > 0) {
                            var selRow = this.dataItem(selectedRows1[0]);
                            
                            window.open("http://localhost:55328/trakingnumber.html?guid=" +selRow);
                        }                       
                    }
                },
                title: " ",
                width: "120px"
            },
            {
                command: {
                    text: "Send Traking", click: function (e) {
                        //editJob(e);
                        clearinputs();
                        
                        jobId = 0;
                        e.preventDefault();
                        var selectedRows = this.select();
                        if (selectedRows.length > 0) {
                            var selRow = this.dataItem(selectedRows[0]);
                            jobId = selRow.id;
                            $('#device').text(selRow.Device);
                            $('#destination').text(selRow.Destination);
                            //document.getElementById('TrakingNumber').innerHTML = selRow.Destination
                            $('#formtrakingn').modal('show'); 
                            $("#ValidUntil").text(selRow.dueOn);
                            $("#DeviceId2").text(selRow.DeviceId2);
                            $("#GeofencesTarget").text(selRow.GeofencesTarget);
                            
                            //editJob(jobId);
                            //$rootScope.$apply(function () {
                            //    $location.path("job/" + jobId);
                            //});
                        }
                    }
                },
                title: " ",
                width: "120px"
            },
            {
                command: {
                    text: "Edit", click: function (e) {
                        //editJob(e);
                        
                        jobId = 0;
                        e.preventDefault();
                        var selectedRows = this.select();
                        if (selectedRows.length > 0) {
                            var selRow = this.dataItem(selectedRows[0]);
                            jobId = selRow.id;
                            
                            editJob(jobId);
                            //$rootScope.$apply(function () {
                            //    $location.path("job/" + jobId);
                            //});
                        }
                    }
                },
                title: " ",
                width: "100px"
            },
            //{ field: 'workZoneName', title: 'Work Zone', width: "100px" },
            { field: 'jobNumber', title: 'Job No.', width: "100px" },
            { field: 'customerName', title: 'Customer', width:"150px" },
            { field: 'jobDescription', title: 'Job Description' },
            { field: 'assignedToName', title: 'Assigned To', width: "100px" },
            { field: 'jobStatus', title: 'Status', width: "100px" },
            { field: 'priority', title: 'Priority', width: "100px" },
            { field: 'createdOn', title: 'Created On', width: "100px" },
            { field: 'StartOn', title: 'Start On', width: "100px" },
            { field: 'dueOn', title: 'Due On', width: "100px" },
            { field: 'DurationJob', title: 'Duration Job', width: "80px" },
            { field: 'latitud', title: 'latitud', width: "100px",hidden:true },
            { field: 'longitud', title: 'longitud', width: "100px", hidden: true },
            { field: 'deviceid', title: 'deviceid', width: "100px", hidden: true },
            { field: 'Device', title: 'Device', width: "100px", hidden: true },
            { field: 'Destination', title: 'Destination', width: "100px", hidden: true },
            { field: 'DeviceId2', title: 'DeviceId2', width: "100px", hidden: true },
            { field: 'GeofencesTarget', title: 'GeofencesTarget', width: "100px", hidden: true },
        ],
        change: function (e) {
            var selectedRows = this.select();
            if (selectedRows.length > 0) {
                var selRow = this.dataItem(selectedRows[0]);
                var jobId = selRow.id;
            }
        }
    };
    //==============================================

    $scope.loadJobs = function () {
        
        var token = getCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        var filterJobNumber = $scope.filterJobNumber;
        if (filterJobNumber == '' || _.isUndefined(filterJobNumber)) {
            filterJobNumber = '0';
        }
        var filterCustomerName = $scope.filterCustomerName;
        if (filterCustomerName == '' || _.isUndefined(filterCustomerName)) {
            filterCustomerName = '0';
        }
        Job.getJobs({ token: token, noCache: noCache, statId: $scope.jobFilterStatus, wzId: $scope.jobFilterWorkZoneId, techId: $scope.jobFilterAssignedToId, jobNo: filterJobNumber, custName: filterCustomerName }, function (data) {
            
            var ds = new kendo.data.DataSource({ data: data });
            var grid = $('#jobsGrid').data("kendoGrid");
            grid.setDataSource(ds);

        }, function (err) {
            var b = 1;
        });
    }

    // CLEAR GRIDS SELECTIONS
    //=================================================
    $scope.woStatsGridClear = function () {
        var grid = $('#woStatsGrid').data("kendoGrid");
        grid.clearSelection();
        $scope.jobFilterStatus = '0';
        $scope.loadJobs();
    }
    $scope.wzGridClear = function () {
        var grid = $('#wzGrid').data("kendoGrid");
        grid.clearSelection();
        $scope.jobFilterWorkZoneId = '0';
        //$scope.jobFilterAssignedToId = '0';
        //Clear the technicians grid
        //$("#techsGrid").data("kendoGrid").dataSource.data([]);
        $scope.loadJobs();
    }

    $scope.techsGridClear = function () {
        var grid = $('#techsGrid').data("kendoGrid");
        grid.clearSelection();
        $scope.jobFilterAssignedToId = '0';
        $scope.loadJobs();
    }
    //=================================================

    $scope.clearFilters = function () {
        $scope.filterJobNumber = '';
        $scope.filterCustomerName = '';
        $scope.loadJobs();
    }

    $scope.newJob = function () {    
        $('#jobDlg').dialog("open");
        map.getViewPort().resize();
        clearJobs(3);
        
    }
    $scope.cancelEntry = function () {
        $('#jobDlg').dialog("close");
        map.getViewPort().resize();
        clearJobs(3);
    }
    $scope.viewMap = function () {
        //$('#jobDlgViewMap').dialog("open");
        
        /*initialize("map");    */
        let geof_latitud = parseFloat($('#geof_latitude').val());
        let geof_longitud = parseFloat($('#geof_longitude').val());
        let geof_name = $("#geof_name").text();

        showLocationInMapHere('mapGeo', geof_latitud, geof_longitud, 16);
        $("#jobDlgViewMap").dialog('open');
       
        //showLocationInMap1('mapGeo', geof_latitud, geof_longitud, "#jobDlgViewMap", 16);
       /* initMap("mapGeo", geof_latitud, geof_longitud, geof_name);*/
        //console.log('viewmap: lat '+geof_latitud +'long '+ geof_longitud +'name '+geof_name);
        
    }
    function initMap(p_map, p_lat, p_lng, title) {
        alert('JobController ' + initMap)
        console.log('initMap: lat ' + p_lat + 'long ' + p_lng + 'name ' + title);

        map = new google.maps.Map(document.getElementById(p_map), {
            center: { lat: 37.09024, lng: -95.712891 },//eeuu
            zoom: 13,
        });
        var marker = new google.maps.Marker({
            position: { lat: p_lat, lng: p_lng },
            map: map,
            title: title
        });
    }
}]);
