etApp.controller('hourMetersTableCtrl', ['$scope', '$http', 'HourMeter', function ($scope, $http, HourMeter) {
    $scope.devices = [];

    $scope.loadDevices = function () {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        HourMeter.getAllDevices({ token: token, noCache: noCache }, function (data) {
            $scope.devices = data;
            $scope.$apply();
        }, function (err) {
            alert('We could not load your hour meters information. Please try again or contact Technical Support.');
        });
    }

    $scope.inputStatusText = function (ind, inputNum) {
        var txt = '';

        switch (inputNum) {
            case 0:
                txt = 'Ignition: ' + $scope.devices[ind].ignitionOnTime;
                break;
            case 1:
                txt = $scope.devices[ind].input1OnTime + ' hours';
                break;
            case 2:
                txt = $scope.devices[ind].input2Name + ': ' + $scope.devices[ind].input2OnTime + ' hours';
                break;
            case 3:
                txt = $scope.devices[ind].input3Name + ': ' + $scope.devices[ind].input3OnTime + ' hours';
                break;
            case 4:
                txt = $scope.devices[ind].input4Name + ': ' + $scope.devices[ind].input4OnTime + ' hours';
                break;
        }
        return txt;
    }

    $scope.editDevice = function (devId) {
        var dlgScope = angular.element($("#dlgHourMeters")).scope();
        dlgScope.openDialog(devId);
    }

}]);