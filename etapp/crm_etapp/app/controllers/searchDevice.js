etApp.controller('devicesSearchCtrl', ['$scope', '$location', 'Device', 'DeviceAction', function ($scope, $location, Device, DeviceAction) {

    $scope.companies = getAllCompanies();
    //$scope.companies = getCompaniesList();

    $scope.searchDevice = function () {
        var token = getCookie('ETCRMTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        Device.get({ token: token, noCache: noCache, searchKey: $scope.searchKey, keyValue: $scope.keyValue }, function (data) {
            $scope.dev = data;
        }, function (err) {
            alert('ooppss... something went wrong.  Please try again or contact Technical Support.');
        });

    }

    $scope.closeForm = function () {
        location.href = "crmMainPanel.html";
    }

    //-------------------------------------------------------------------------------------------
    //MOVE DEVICE
    $scope.moveDevice = function () {
        if (_.isUndefined($scope.newCompanyId) || $scope.newCompanyId == '') {
            alert('Please select a valid destination company.');
            return;
        }
        else {
            var newCompanyName = _.findWhere($scope.companies, {id: $scope.newCompanyId});
            if (confirm('Are you sure you want to move this device to ' + newCompanyName.value + '?')) {
                // Save it!
                var token = getCookie('ETCRMTK');
                var devAction = new DeviceAction({ token: getCookie('ETCRMTK') });
                devAction.action = 'CHANGE_COMPANY';
                devAction.id = $scope.dev.deviceGUID
                devAction.param1 = $scope.newCompanyId;
                devAction.param2 = '';

                devAction.$setDeviceAction({ token: token }, function (data) {
                    if (data.isOk == true) {
                        alert('Device has been moved.');
                        $scope.searchDevice();
                    }
                    else {
                        alert('Failed moving device to new company');
                    }
                });
            } else {
                // Do nothing!
            }
        }
    }

    //-------------------------------------------------------------------------------------------
    //SPEEDGAUGE
    $scope.activateSG = function () {
        if (!isNumber($scope.dev.sgFee)) {
            alert('Please enter a valid SpeedGauge fee');
            return;
        }
        else {
            $scope.dev.sgFee = Number($scope.dev.sgFee);
            if ($scope.dev.sgFee <= 0) {
                alert('Please enter a valid SpeedGauge fee');
            }
            else {
                $scope.setSpeedGauge('SG_ACTIVATE', $scope.dev.sgFee);
            }
        }
    }

    $scope.inactivateSG = function () {
        $scope.setSpeedGauge('SG_INACTIVATE', '');
    }

    $scope.setSpeedGauge = function (action, fee) {
        var token = getCookie('ETCRMTK');
        var devAction = new DeviceAction({ token: getCookie('ETCRMTK') });
        devAction.action = action;
        devAction.id = $scope.dev.deviceGUID
        devAction.param1 = '';
        devAction.param2 = '';

        devAction.$setDeviceAction({ token: token }, function (data) {
            if (data.isOk == true) {
                if (action == 'SG_ACTIVATE') {
                    $scope.dev.hasSpeedGauge = true;
                }
                else {
                    $scope.dev.hasSpeedGauge = false;
                }
                $scope.dev.sgStatus = '';
                alert('SpeedGauge status change applied');
            }
            else {
                alert('Failed SpeedGauge change of status');
            }
        });
    }
    //-------------------------------------------------------------------------------------------


}]);

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}