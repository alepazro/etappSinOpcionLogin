etApp.controller('telemetryCtrl', ['$scope', '$http', 'DevOutput', function ($scope, $http, DevOutput) {

    $scope.dev = {};

    $scope.openDialog = function (deviceId) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        DevOutput.getOutputs({ token: token, noCache: noCache, deviceId: deviceId }, function (data) {
            $scope.dev = data;
            $("#dlgTelemetry").dialog("open");
        }, function (err) {
            alert('We could not open your Telemetry module. Please try again or contact Technical Support.');
        });
    }

    $scope.statusText = function (relNum, stat, inverse) {
        if (stat == undefined) {
            return '';
        }
        else {
            var txt = '';
            if (inverse == true) {
                stat = !stat;
            }
            switch (relNum) {
                case 1:
                    if (stat == true) {
                        txt = $scope.dev.on1Name;
                    }
                    else {
                        txt = $scope.dev.off1Name;
                    }
                    break;
                case 2:
                    if (stat == true) {
                        txt = $scope.dev.on2Name;
                    }
                    else {
                        txt = $scope.dev.off2Name;
                    }
                    break;
                case 3:
                    if (stat == true) {
                        txt = $scope.dev.on3Name;
                    }
                    else {
                        txt = $scope.dev.off3Name;
                    }
                    break;
                case 4:
                    if (stat == true) {
                        txt = $scope.dev.on4Name;
                    }
                    else {
                        txt = $scope.dev.off4Name;
                    }
                    break;
            }

            return txt;

        }
    }

    $scope.switchOutput = function (relNum, stat) {
        stat = !stat;

        switch (relNum)
        {
            case 1:
                $scope.dev.status1 = stat;
                break;
            case 2:
                $scope.dev.status2 = stat;
                break;
            case 3:
                $scope.dev.status3 = stat;
                break;
            case 4:
                $scope.dev.status4 = stat;
                break;
        }

        var token = getTokenCookie('ETTK')
        var data = {
            deviceId: $scope.dev.deviceId,
            relayNum: relNum,
            newStatus: stat
        }

        $http({
            url: 'https://localhost:44385/etrack.svc/telemetry/' + token,
            method: "POST",
            data: data
        }).then(function (response) {
            // success
            alert('Command sent to device.  Please wait up to 1 minute for it to take effect.');
        }, function (response) { // optional
            // failed
            var b = 1;
        });
    }

}]);
