etApp.controller('telemetryTableCtrl', ['$scope', '$http', 'DevOutput', function ($scope, $http, DevOutput) {

    $scope.devices = [];

    $scope.loadDevices = function () {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        DevOutput.getAllDevices({ token: token, noCache: noCache }, function (data) {
            $scope.devices = data;
            $scope.$apply();
        }, function (err) {
            alert('We could not load your Telemetry information. Please try again or contact Technical Support.');
        });
    }

    $scope.editDevice = function (deviceId, ioType) {
        //ioType = 1: Input, 2: Output
        var b = 1;
        var dlgScope = angular.element($("#dlgTelemetry")).scope();
        dlgScope.openDialog(deviceId, ioType);
    }


    $scope.statusText = function (ind, devId, relNum, stat, inverse) {
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
                        txt = $scope.devices[ind].name1 + ': ' + $scope.devices[ind].on1Name;
                    }
                    else {
                        txt = $scope.devices[ind].name1 + ': ' + $scope.devices[ind].off1Name;
                    }
                    break;
                case 2:
                    if (stat == true) {
                        txt = $scope.devices[ind].name2 + ': ' + $scope.devices[ind].on2Name;
                    }
                    else {
                        txt = $scope.devices[ind].name2 + ': ' + $scope.devices[ind].off2Name;
                    }
                    break;
                case 3:
                    if (stat == true) {
                        txt = $scope.devices[ind].name3 + ': ' + $scope.devices[ind].on3Name;
                    }
                    else {
                        txt = $scope.devices[ind].name3 + ': ' + $scope.devices[ind].off3Name;
                    }
                    break;
                case 4:
                    if (stat == true) {
                        txt = $scope.devices[ind].name4 + ': ' + $scope.devices[ind].on4Name;
                    }
                    else {
                        txt = $scope.devices[ind].name4 + ': ' + $scope.devices[ind].off4Name;
                    }
                    break;
            }

            return txt;

        }
    }

    $scope.InputStatusText = function (ind, devId, InputNum, stat, inverse) {
        if (stat == undefined) {
            return '';
        }
        else {
            var txt = '';
            if (inverse == true) {
                stat = !stat;
            }

            switch (InputNum) {
                case 1:
                    if (stat == true) {
                        txt = $scope.devices[ind].inputName1 + ': ' + $scope.devices[ind].inputOn1Name;
                    }
                    else {
                        txt = $scope.devices[ind].inputName1 + ': ' + $scope.devices[ind].inputOff1Name;
                    }
                    break;
                case 2:
                    if (stat == true) {
                        txt = $scope.devices[ind].inputName2 + ': ' + $scope.devices[ind].inputOn2Name;
                    }
                    else {
                        txt = $scope.devices[ind].inputName2 + ': ' + $scope.devices[ind].inputOff2Name;
                    }
                    break;
                case 3:
                    if (stat == true) {
                        txt = $scope.devices[ind].inputName3 + ': ' + $scope.devices[ind].inputOn3Name;
                    }
                    else {
                        txt = $scope.devices[ind].inputName3 + ': ' + $scope.devices[ind].inputOff3Name;
                    }
                    break;
                case 4:
                    if (stat == true) {
                        txt = $scope.devices[ind].inputName4 + ': ' + $scope.devices[ind].inputOn4Name;
                    }
                    else {
                        txt = $scope.devices[ind].inputName4 + ': ' + $scope.devices[ind].inputOff4Name;
                    }
                    break;
            }

            return txt;

        }
    }

}]);