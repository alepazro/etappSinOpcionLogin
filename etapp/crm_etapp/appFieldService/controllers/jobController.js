etApp.controller('jobController', ['$scope', '$location', '$http', 'JobStatus', 'WorkZone', 'Technician', 'Job', 'SupportLists', 'CustomerSearch', 'job', function ($scope, $location, $http, JobStatus, WorkZone, Technician, Job, SupportLists, CustomerSearch, job) {

    var token = getCookie('ETTK');
    var noCache = Math.floor((Math.random() * 100000) + 1);

    SupportLists.get({ token: token, noCache: noCache }, function (data) {
        $scope.workZones = data.workZones;
        $scope.priorities = data.priorities;
        $scope.specialties = data.specialties;
        $scope.categories = data.categories;
        $scope.technicians = data.technicians;
        $scope.statuses = data.statuses;
    }, function (err) {
        var a = 1;
    });

    $scope.job = job;

    $scope.workZones = [];

    $scope.searchCustomers = function () {
        $scope.searchCustName = '';
        $("#searchCustGrid").data("kendoGrid").dataSource.data([]);
        $('#searchCustomersModal').modal({
            show: true,
            backdrop: false
        });
    }

    //SETTING UP THE MODAL CONTROLS
    //================================================
    $scope.dsSearchCust = new kendo.data.DataSource({
        data: []
    });
    $scope.searchCustGridOptions = {
        selectable: 'row',
        columns: [
            { field: 'name', title: 'Customer Name' },
            { field: 'workZoneName', title: 'Work Zone' }
        ],
        change: function (e) {
            var selectedRows = this.select();
            if (selectedRows.length > 0) {
                var selRow = this.dataItem(selectedRows[0]);
                $scope.job.customerId = selRow.id;
                $scope.job.customerName = selRow.name;
                $scope.job.contactName = selRow.contactName;
                $scope.job.address = selRow.address;
                $scope.job.phone = selRow.phone;
                $scope.job.workZoneId = selRow.workZoneId;
                $scope.job.workZoneName = selRow.workZoneName;
                $scope.$apply();
                $('#searchCustomersModal').modal('hide');
            }
        }
    }
    //================================================

    //Called from the Search Customers modal
    //This modal is defined in jobForm.html near the closing BODY tag
    //================================================
    $scope.searchCustomersNow = function () {
        noCache = Math.floor((Math.random() * 100000) + 1);
        var custName = $scope.searchCustName;
        if (custName == '' || _.isUndefined(custName)) {
            custName = '0';
        }
        CustomerSearch.getCustomers({ token: token, noCache: noCache, custName: custName }, function (data) {
            var ds = new kendo.data.DataSource({ data: data });
            var grid = $('#searchCustGrid').data("kendoGrid");
            grid.setDataSource(ds);
        }, function (err) {
        });
    }
    //================================================

    $scope.saveJob = function () {
        var a = 1;
        $scope.job.$saveJob({ token: token }, function (data) {
            $location.path('/');
        }, function (err) {
            var a = 1;
        });
    }

    $scope.saveAndMore = function () {
        var a = 1;
    }

    $scope.cancelEntry = function () {
        $location.path('/');
    }

    $scope.recalcDueDate = function () {
        var d = Date.parse($scope.job.scheduledStart);
        var mm = (parseInt($scope.job.durationHH) * 60) + parseInt($scope.job.durationMM);
        var dueBy = $("#datDueBy").data("kendoDateTimePicker");
        dueBy.value(new Date(d + (mm * 60000)));
    }

}]);