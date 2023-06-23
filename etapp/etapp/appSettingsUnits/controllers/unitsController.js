etApp.controller('unitsController', ['$scope', '$compile', 'DevicesList', 'Device', 'IDNameList', 'popupModalService', function ($scope, $compile, DevicesList, Device, IDNameList, popupModalService) {

    $scope.devicesList = [];

    $scope.activeActionsTemplate = '<div class="btn-group">' +
                                   '<button type="button" class="btn btn-xs btn-success" ng-click="grid.appScope.edit(row)">Edit</button>' +
                                   '</div>'

    $scope.activeActionsTemplate2 = '<div class="btn-group">' +
                                    '<button type="button" class="btn btn-xs btn-danger"  ng-click="grid.appScope.inactivate(row)">Inactivate</button>' +
                                    '</div>'

    $scope.sleepingActionsTemplate = '<div class="btn-group">' +
                                     '<button type="button" class="btn btn-xs btn-success" ng-click="grid.appScope.reactivate(row)">Reactivate</button>' +
                                     '<button type="button" class="btn btn-xs btn-danger"  ng-click="grid.appScope.cancel(row)">Cancel</button>' +
                                     '</div>'

    $scope.cancelledActionsTemplate = '<div class="btn-group">' +
                                      '<button type="button" class="btn btn-xs btn-success" ng-click="grid.appScope.reinstate(row)">Reinstate</button>'
                                      '</div>'

    $scope.gridActive = {
        enableSorting: true,
        minRowsToShow: 800,
        columnDefs: [
          { name: 'actions', displayName: 'Edit', cellTemplate: $scope.activeActionsTemplate, width: 60, enableColumnMenu:false },
          { field: 'iconUrl', displayName: '', cellTemplate: '<img src="{{row.entity.iconURL}}" />', width: 50, enableColumnMenu: false },
          { field: 'deviceId', displayName: 'ID', width:50 },
          { field: 'name', displayName: 'Name', width: 100 },
          { field: 'shortName', displayName: 'Short Name', width:100 },
          { field: 'lastUpdatedOn', displayName: 'Last Update', width: 150 },
          { field: 'lastEventName', displayName: 'Last Event', width: 75 },
          { field: 'lastEventOn', displayName: 'Event Date', width: 150 },
          { field: 'driverName', displayName: 'Driver', width: 120 },
          { field: 'idleLimit', displayName: 'Idle', width: 70 },
          { field: 'speedLimit', displayName: 'Speed', width: 70 },
          { field: 'odometer', displayName: 'Odometer', width: 100 },
          { field: 'vin', displayName: 'VIN', width: 100 },
          { field: 'serialNumber', displayName: 'S/N', width: 100 },
          { name: 'actions2', displayName: 'Inactivate', cellTemplate: $scope.activeActionsTemplate2, width: 90, enableColumnMenu: false }
        ]
    };

    $scope.gridNotInstalled = {
        enableSorting: true,
        minRowsToShow: 500,
        columnDefs: [
          { name: 'actions', displayName: 'Edit', cellTemplate: $scope.activeActionsTemplate, width: 60, enableColumnMenu: false },
          { field: 'deviceId', displayName: 'ID', width: 50 },
          { field: 'name', displayName: 'Name', width: 100 },
          { field: 'shortName', displayName: 'Short Name', width: 100 },
          { field: 'serialNumber', displayName: 'S/N', width: 100 },
          { field: 'assignedOn', displayName: 'Assigned On', width:150 }
        ]
    };

    $scope.gridSleeping = {
        enableSorting: true,
        minRowsToShow: 500,
        columnDefs: [
          { name: 'actions', displayName: 'Actions', cellTemplate: $scope.sleepingActionsTemplate, width: 150, enableColumnMenu: false },
          { field: 'deviceId', displayName: 'ID', width: 50 },
          { field: 'name', displayName: 'Name', width: 100 },
          { field: 'shortName', displayName: 'Short Name', width: 100 },
          { field: 'lastUpdatedOn', displayName: 'Last Update', width: 150 },
          { field: 'vin', displayName: 'VIN', width:100 },
          { field: 'serialNumber', displayName: 'S/N', width: 100 },
          { field: 'sleepingSince', displayName: 'Suspended Since', width:150 }
        ]
    };

    $scope.gridCancelled = {
        enableSorting: true,
        minRowsToShow: 500,
        columnDefs: [
          { name: 'actions', displayName: 'Reinstate', cellTemplate: $scope.cancelledActionsTemplate, width: 70, enableColumnMenu: false },
          { field: 'deviceId', displayName: 'ID', width: 50 },
          { field: 'name', displayName: 'Name', width: 100 },
          { field: 'shortName', displayName: 'Short Name', width: 100 },
          { field: 'lastUpdatedOn', displayName: 'Last Update', width: 150 },
          { field: 'vin', displayName: 'VIN', width: 100 },
          { field: 'serialNumber', displayName: 'S/N', width: 100 },
          { field: 'cancelledSince', displayName: 'Cancelled Since', width: 150 }
        ]
    };

    $scope.gridRMA = {
        enableSorting: true,
        minRowsToShow: 500,
        columnDefs: [
          { field: 'deviceId', displayName: 'ID', width: 50 },
          { field: 'name', displayName: 'Name', width: 100 },
          { field: 'shortName', displayName: 'Short Name', width: 100 },
          { field: 'lastUpdatedOn', displayName: 'Last Update', width: 150 },
          { field: 'vin', displayName: 'VIN', width: 100 },
          { field: 'serialNumber', displayName: 'S/N', width: 100 }
        ]
    };

    $scope.loadDevices = function () {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        DevicesList.query({ token: token, noCache: noCache }, function (data) {
            
            $scope.devicesList = data;
            $scope.activeDevices = _.filter($scope.devicesList, function (itm) { return itm.deviceStatus == 1; });;
            $scope.sleepingDevices = _.filter($scope.devicesList, function (itm) { return itm.deviceStatus == 2; });;
            $scope.cancelledDevices = _.filter($scope.devicesList, function (itm) { return itm.deviceStatus == 3; });
            $scope.notInstalledDevices = _.filter($scope.devicesList, function (itm) { return itm.deviceStatus == 4; });
            $scope.rmaDevices = _.filter($scope.devicesList, function (itm) { return itm.deviceStatus == 5; });

            $scope.gridActive.data = $scope.activeDevices;
            $scope.gridSleeping.data = $scope.sleepingDevices;
            $scope.gridCancelled.data = $scope.cancelledDevices;
            $scope.gridRMA.data = $scope.rmaDevices;
            $scope.gridNotInstalled.data = $scope.notInstalledDevices;

        }, function (err) {
            var b = 0;
        });
    }

    $scope.loadList = function (listName) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        IDNameList.query({ token: token, noCache: noCache, listName: listName }, function (data) {
            switch (listName) {
                case 'DRIVERS':
                    $scope.drivers = data;
                    break;
                case 'ICONS':
                    $scope.icons = data;
                    break;
            }
        }, function (err) {
            var b = 0;
        });
    }

    $scope.edit = function (row) {

        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        var id = row.entity.id;
        Device.get({ token: token, noCache: noCache, id: id }, function (data) {
            $scope.device = data;
            $scope.device.action = 'EDIT';
            $scope.device.opt = 0;
            $scope.openModal($scope.device, 'Edit Unit', 'editDeviceTemplate');
        }, function (err) {
            alert('Could not open device ' + row.entity.name + ' for edition.  Please try again.');
        });

    };

    $scope.inactivate = function (row) {

        $scope.device = {
            id: row.entity.id,
            deviceId: row.entity.deviceId,
            name: row.entity.name,
            action: 'INACTIVATE',
            opt: 0
        };

        $scope.openModal($scope.device, 'Inactivate Unit', 'inactiveDeviceTemplate');

    };

    $scope.reactivate = function (row) {

        $scope.device = {
            id: row.entity.id,
            deviceId: row.entity.deviceId,
            name: row.entity.name,
            action: 'REACTIVATE',
            opt: 0
        };

        $scope.openModal($scope.device, 'Reactivate Unit', 'reactivateDeviceTemplate');

    };

    $scope.cancel = function (row) {

        $scope.device = {
            id: row.entity.id,
            deviceId: row.entity.deviceId,
            name: row.entity.name,
            action: 'CANCEL',
            opt: 0
        };

        $scope.openModal($scope.device, 'Cancel Unit', 'cancelDeviceTemplate');
    };

    $scope.reinstate = function (row) {

        $scope.device = {
            id: row.entity.id,
            deviceId: row.entity.deviceId,
            name: row.entity.name,
            action: 'REINSTATE',
            opt: 0
        };

        $scope.openModal($scope.device, 'Reinstate Unit', 'reinstateDeviceTemplate');

    };

    $scope.savePopupModal = function () {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        var action = '';

        if ($scope.device.odometer == '') {
            $scope.device.odometer = 0;
        }

        switch ($scope.device.action) {

            case 'EDIT':
                action = 'EDIT';
                $scope.device.$saveDevice({ token: token, action: 'SAVE', id: $scope.device.id }, function (data) {
                    $scope.loadDevices();
                }, function (err) {
                    alert(action + ' failed.  Please try again or contact Technical Support.');
                });

                break;
            case 'INACTIVATE':
                action = '';
                switch ($scope.device.opt) {
                    case 1:
                    case '1':
                        action = 'SUSPEND';
                        break;
                    case 2:
                    case '2':
                        action = 'CANCEL';
                        break;
                    case 3:
                    case '3':
                        action = 'RMA';
                        break;
                    default:
                        alert('No option was selected.  Nothing will be done.');
                        break;
                }
                if (action != '') {
                    if ($scope.usrComment == '') {
                        $scope.usrComment = 'na';
                    }
                    Device.changeDeviceStatus({ token: token, noCache: noCache, action: action, id: $scope.device.id, usrComment: $scope.usrComment }, function (data) {
                        $scope.loadDevices();
                        $scope.usrComment = '';
                    }, function (err) {
                        alert(action + ' failed.  Please try again or contact Technical Support.');
                    });
                }
                break;
            case 'REACTIVATE':
                action = 'REACTIVATE';
                if ($scope.usrComment == '') {
                    $scope.usrComment = 'na';
                }
                Device.changeDeviceStatus({ token: token, noCache: noCache, action: action, id: $scope.device.id, usrComment: $scope.usrComment }, function (data) {
                    $scope.loadDevices();
                    $scope.usrComment = '';
                }, function (err) {
                    alert(action + ' failed.  Please try again or contact Technical Support.');
                });
                break;
            case 'CANCEL':
                action = 'CANCEL';
                if ($scope.usrComment == '') {
                    $scope.usrComment = 'na';
                }
                Device.changeDeviceStatus({ token: token, noCache: noCache, action: action, id: $scope.device.id, usrComment: $scope.usrComment }, function (data) {
                    $scope.loadDevices();
                    $scope.usrComment = '';
                }, function (err) {
                    alert(action + ' failed.  Please try again or contact Technical Support.');
                });
                break;
            case 'REINSTATE':
                action = 'REINSTATE';
                if ($scope.usrComment == '') {
                    $scope.usrComment = 'na';
                }
                Device.changeDeviceStatus({ token: token, noCache: noCache, action: action, id: $scope.device.id, usrComment: $scope.usrComment }, function (data) {
                    $scope.loadDevices();
                    $scope.usrComment = '';
                }, function (err) {
                    alert(action + ' failed.  Please try again or contact Technical Support.');
                });
                break;
        }

        popupModalService.close('#popupModal');
    };

    $scope.openModal = function (obj, title, templateName) {
        
        $scope.actionModalTitle = title;

        $("#tmpTemplates").load("appSettingsUnits/templates/" + templateName + ".html", function () {
            var template = _.template($('#tmpTemplates').html(), { device: obj });
            $('#popupModal').empty();
            $(template).appendTo($('#popupModal'));
            $compile($('#popupModal'))($scope);

            $('[data-toggle="tooltip"]').tooltip()
            deviceShortNameColor();

            popupModalService.open('#popupModal');
        });
    }

    $scope.changeIcon = function () {
        var icon = _.find($scope.icons, function (val) {
            return val.id == $scope.device.iconId;
        });

        $scope.device.iconURL = icon.value1;
    }

    //Initialize data
    $scope.usrComment = '';
    $scope.loadList('DRIVERS');
    $scope.loadList('ICONS');
    $scope.loadDevices();

    $scope.changeTextColor = function (color) {
        return ("color:" + color);
    }
    $scope.changeBackgroundColor = function (color) {
        return ("background-color:" + color);
    }

}]);

function deviceShortNameColor() {
    try {
        $('#deviceUpdateTextColor').colorPicker();
        $('#deviceUpdateBgndColor').colorPicker();
        $('#deviceUpdateTextColor').change(function () {
            var color = $('#deviceUpdateTextColor').val();
            $('#deviceUpdateShortName').css('color', color);
            $('#deviceUpdateShortName2').css('color', color);
        });
        $('#deviceUpdateBgndColor').change(function () {
            var color = $('#deviceUpdateBgndColor').val();
            $('#deviceUpdateShortName').css('background-color', color);
            $('#deviceUpdateShortName2').css('background-color', color);
        });
    }
    catch (err) {

    }
}
