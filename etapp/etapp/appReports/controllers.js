crmApp.controller('reportsController', ['$scope', function ($scope) {

    $scope.filter = {
        reportId: '',
        param1: ''
    }

    var data = new kendo.data.DataSource({
        data: $scope.report
    });

    $scope.gridOptions = {
        dataSource: data,
        sortable: {
            mode: "single"
        }
    };

    $scope.loadBasicTables = function () {
        $scope.basicTables = jsonGET('crm.svc', 'reportsBasicTables', 0, false);
    };
    $scope.loadBasicTables();

    $scope.refreshView = function () {
        var _param1 = '';
        try{
            _param1 = $('#txtParam1').val()
        } catch (err) {
            _param1 = '';
        }
        var params = {
            reportId: $scope.filter.reportId,
            param1: _param1,
            userToken: getTokenCookie('ETCRMTK')
        };
        $scope.report = jsonPOST('crm.svc', 'crmreport', params);

        $scope.report = JSON.parse($scope.report);

        $scope.gridOptions.dataSource = $scope.report;
        try {
            $('#reportGrid').data('kendoGrid').dataSource.data($scope.report);
        }
        catch (err) {
            var a = 1;
        }
    }

}]);