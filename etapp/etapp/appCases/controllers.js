crmApp.controller('casesController', ['$scope', function ($scope) {

    $scope.filter = {
        categoryId: '',
        assignedToId: '',
        onlyMine: 0,
        onlyOpen: 0
    }

    $scope.hasFilter = false;

    $scope.cases = jsonGET('cases.svc', 'cases', 0, true);

    var data = new kendo.data.DataSource({
        data: $scope.cases
    });

    $scope.gridOptions = {
        dataSource: data,
        sortable: {
            mode: "single"
        },
        columns: [
                {
                    template: "<button class='k-button' ng-click='editCase(this)'>Edit</button>",
                    width: "100px"
                },
                {
                    field: "id",
                    title: "ID",
                    hidden: true
                }, {
                    field: "statusName",
                    title: "Status"
                }, {
                    field: "companyName",
                    title: "Company"
                }, {
                    field: "subject",
                    title: "Subject"
                }, {
                    field: "assignedToName",
                    title: "Assigned To"
                }, {
                    field: "categoryName",
                    title: "Category"
                }, {
                    field: "typeName",
                    title: "Type"
                }, {
                    field: "deviceName",
                    title: "Device"
                }, {
                    field: "lastUpdatedOn",
                    title: "Last Activity On"
                }, {
                    field: "assignedOn",
                    title: "Assigned On"
                }, {
                    field: "createdOn",
                    title: "Created On"
                }
        ]
    };


    $scope.loadBasicTables = function () {
        $scope.basicTables = jsonGET('cases.svc', 'casesBasicTables', 0, false);
        $scope.technicians = jsonGET('cases.svc', 'technicians', 0, true);
    };
    $scope.loadBasicTables();

    $scope.applyFilters = function () {

        $scope.hasFilter = true;

        var params = {
            categoryId: $scope.filter.categoryId,
            assignedToId: $scope.filter.assignedToId,
            onlyMine: $scope.filter.onlyMine,
            onlyOpen: $scope.filter.onlyOpen
        };
        $scope.cases = jsonPOST('cases.svc', 'casesFiltered', params);

        $scope.gridOptions.dataSource = $scope.cases;
        try {
            $('#casesListGrid').data('kendoGrid').dataSource.data($scope.cases);
        }
        catch (err) {
            var a = 1;
        }
    }

    $scope.refreshView = function () {
        if ($scope.hasFilter == false) {
            $scope.cases = jsonGET('cases.svc', 'cases', 0, true);
            $scope.gridOptions.dataSource = $scope.cases;
                try {
                    $('#casesListGrid').data('kendoGrid').dataSource.data($scope.cases);
                }
                catch (err) {
                    var a = 1;
                }
            }
        else {
            $scope.applyFilters();
        }
    }

    $scope.clearFilters = function () {
        $scope.filter.categoryId = '';
        $scope.filter.assignedToId = '';
        $scope.filter.onlyMine = 0;
        $scope.filter.onlyOpen = 0;

        $scope.hasFilter = false;

        $scope.refreshView();
    }

    $scope.editCase = function (e) {
        var elem = e.dataItem;
        var id = elem.id;
        window.open('crmCase.html?' + 'id=' + id, target = "_blank");
    }


}]);