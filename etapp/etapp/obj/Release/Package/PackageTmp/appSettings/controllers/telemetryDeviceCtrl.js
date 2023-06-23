etApp.controller('telemetryDeviceCtrl', ['$scope', '$http', 'DevOutput', function ($scope, $http, DevOutput) {

    $scope.dev = {};
    $scope.devBackup = {};

    $scope.openDialog = function (deviceId, ioType) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        DevOutput.getOutputs({ token: token, noCache: noCache, deviceId: deviceId }, function (data) {
            $scope.ioType = ioType;
            if (ioType == 1) {
                $scope.IOTypeName = 'Inputs';
                $scope.dev = {
                    ioType: ioType,
                    deviceId: data.deviceId,

                    name1: data.inputName1,
                    on1Name: data.inputOn1Name,
                    off1Name: data.inputOff1Name,

                    name2: data.inputName2,
                    on2Name: data.inputOn2Name,
                    off2Name: data.inputOff2Name,

                    name3: data.inputName3,
                    on3Name: data.inputOn3Name,
                    off3Name: data.inputOff3Name,

                    name4: data.inputName4,
                    on4Name: data.inputOn4Name,
                    off4Name: data.inputOff4Name
                }
            }
            else {
                $scope.ioTypeName = 'Relays';
                $scope.dev = {
                    ioType: ioType,                    
                    deviceId: data.deviceId,

                    name1: data.name1,
                    on1Name: data.on1Name,
                    off1Name: data.off1Name,

                    name2: data.name2,
                    on2Name: data.on2Name,
                    off2Name: data.off2Name,

                    name3: data.name3,
                    on3Name: data.on3Name,
                    off3Name: data.off3Name,

                    name4: data.name4,
                    on4Name: data.on4Name,
                    off4Name: data.off4Name
                }
            }
            $scope.devBackup = angular.copy($scope.dev);
            $('#dlgTelemetry').dialog("open");
        }, function (err) {
            alert('We could not open your Telemetry module. Please try again or contact Technical Support.');
        });
    }

    $scope.cancelChange = function (ioNum) {
        switch (ioNum) {
            case 1:
                $scope.dev.name1 = $scope.devBackup.name1;
                $scope.dev.on1Name = $scope.devBackup.on1Name;
                $scope.dev.off1Name = $scope.devBackup.off1Name;
                break;
            case 2:
                $scope.dev.name2 = $scope.devBackup.name2;
                $scope.dev.on2Name = $scope.devBackup.on2Name;
                $scope.dev.off2Name = $scope.devBackup.off2Name;
                break;
            case 3:
                $scope.dev.name3 = $scope.devBackup.name3;
                $scope.dev.on3Name = $scope.devBackup.on3Name;
                $scope.dev.off3Name = $scope.devBackup.off3Name;
                break;
            case 4:
                $scope.dev.name4 = $scope.devBackup.name4;
                $scope.dev.on4Name = $scope.devBackup.on4Name;
                $scope.dev.off4Name = $scope.devBackup.off4Name;
                break;
        }
    }

    $scope.applyChange = function (ioNum, toAll) {
        var ioName = '';
        var onName = '';
        var offName = '';

        switch (ioNum) {
            case 1:
                ioName = $scope.dev.name1;
                onName = $scope.dev.on1Name;
                offName = $scope.dev.off1Name;
                break;
            case 2:
                ioName = $scope.dev.name2;
                onName = $scope.dev.on2Name;
                offName = $scope.dev.off2Name;
                break;
            case 3:
                ioName = $scope.dev.name3;
                onName = $scope.dev.on3Name;
                offName = $scope.dev.off3Name;
                break;
            case 4:
                ioName = $scope.dev.name4;
                onName = $scope.dev.on4Name;
                offName = $scope.dev.off4Name;
                break;
        }

        if (ioName == '') {
            alert('Name cannot be blank');
            return;
        }
        if (onName == '') {
            alert('On Status Name cannot be blank');
            return;
        }
        if (offName == '') {
            alert('Off Status Name cannot be blank');
            return;
        }

        var token = getTokenCookie('ETTK')
        var data = {
            deviceId: $scope.dev.deviceId,
            ioType: $scope.dev.ioType,
            ioNum: ioNum,
            name: ioName,
            onStatus: onName,
            offStatus: offName,
            isAll: toAll
        }

        $http({
            url: 'https://pre.etrack.ws/etrack.svc/telemetrySetUp/' + token,
            method: "POST",
            data: data
        }).then(function (response) {
            // success
            $scope.devBackup = angular.copy($scope.dev);
            alert('Changes saved');
        }, function (response) { // optional
            // failed
            var b = 1;
        });
    }

    $scope.closeDlg = function () {
        $('#dlgTelemetry').dialog("close");
        var dlgScope = angular.element($("#telemetryTable")).scope();
        dlgScope.loadDevices();
    }

}]);