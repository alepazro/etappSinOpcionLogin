etApp.controller('jobsController', ['$scope', 'SupportLists', 'Job', function ($scope, SupportLists, Job) {

    //Get basic tables
    //===========================================================
    var token = getTokenCookie('ETTK');
    var noCache = Math.floor((Math.random() * 100000) + 1);
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
                    text: "Edit", click: function (e) {
                        //editJob(e);

                        e.preventDefault();
                        var selectedRows = this.select();
                        if (selectedRows.length > 0) {
                            var selRow = this.dataItem(selectedRows[0]);
                            var jobId = selRow.id;

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
            { field: 'workZoneName', title: 'Work Zone', width: "100px" },
            { field: 'jobNumber', title: 'Job No.', width: "100px" },
            { field: 'customerName', title: 'Customer', width:"150px" },
            { field: 'jobDescription', title: 'Job Description' },
            { field: 'assignedToName', title: 'Assigned To', width: "100px" },
            { field: 'jobStatus', title: 'Status', width: "100px" },
            { field: 'priority', title: 'Priority', width: "100px" },
            { field: 'createdOn', title: 'Created On', width: "100px" },
            { field: 'dueOn', title: 'Due On', width: "100px" }
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
    }

}]);
