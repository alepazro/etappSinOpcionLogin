etApp.controller('hourMetersDeviceCtrl', ['$scope', '$http', 'HourMeter', function ($scope, $http, HourMeter) {

    $scope.dev = {};
    $scope.devBackup = {};

    $scope.openDialog = function (deviceId) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        HourMeter.getHourMeters({ token: token, noCache: noCache, deviceId: deviceId }, function (data) {
            $scope.dev = data;
            $scope.devBackup = angular.copy($scope.dev);
            $scope.$apply();
            $('#dlgHourMeters').dialog("open");
        }, function (err) {
            alert('We could not open your Hour Meters module. Please try again or contact Technical Support.');
        });
    }

    $scope.copyDateTime = function () {
        $scope.dev.input1LastSetOn = $scope.dev.ignitionLastSetOn;
        $scope.dev.input2LastSetOn = $scope.dev.ignitionLastSetOn;
        $scope.dev.input3LastSetOn = $scope.dev.ignitionLastSetOn;
        $scope.dev.input4LastSetOn = $scope.dev.ignitionLastSetOn;
    }

    $scope.saveDlg = function () {
        var token = getTokenCookie('ETTK');
        var dev = new HourMeter({ token: token });
        dev = $scope.dev;
        dev.$saveHourMeter({ token: token }, function (data) {
            $scope.closeDlg();
        }, function (err) {
            var b = 1;
        });
    }

    $scope.closeDlg = function () {
        $('#dlgHourMeters').dialog("close");
        var dlgScope = angular.element($("#hourMetersTable")).scope();
        dlgScope.loadDevices();
    }

}]);