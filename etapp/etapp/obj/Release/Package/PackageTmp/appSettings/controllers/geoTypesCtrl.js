etApp.controller('geoTypesCtrl', ['$scope', '$http', 'GeofenceType', function ($scope, $http, GeofenceType) {
    
    $scope.geoTypes = [];

    $scope.getAll = function () {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        GeofenceType.getAll({ token: token, noCache: noCache }, function (data) {
            $scope.geoTypes = data;
            $scope.$apply();
        }, function (err) {
            alert('We could not load your Geofences Types. Please try again or contact Technical Support.');
        });
    }

    $scope.save = function (id, name, callBack) {
        
        alert("dave")
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        var itm = new GeofenceType({ token: token });
        itm.id = id;
        itm.name = name;
        itm.$save({ token: token }, function (data) {
            if (_.isObject(jsonGeoTypes)) {
                jsonGeoTypes = false;
            }
            callBack();
        }, function (err) {
            var b = 1;
            alert('Failed saving Geofence Type');
        });
    }

    $scope.openDialog = function () {
        $('#dlgGeoTypes').dialog("open");
        $scope.getAll();
    }

    $scope.editGeoType = function (ind) {
        $scope.geoTypes[ind].viewMode = false;
        $scope.geoTypes[ind].editName = $scope.geoTypes[ind].name;
    }

    $scope.saveGeoType = function (ind) {
        //Saves an existing geofence type
        
        $scope.save($scope.geoTypes[ind].id, $scope.geoTypes[ind].editName, $scope.saveExistingCallBack);
    }

    $scope.saveExistingCallBack = function () {
        
        $scope.getAll();
    }

    $scope.cancelEditGeoType = function (ind) {
        //Cancels editing of a geofence type
        $scope.geoTypes[ind].viewMode = true;
    }

    $scope.saveNewType = function () {
        
        //Saves a new geofence type
        $scope.save(0, $scope.geoTypeNameNew, $scope.saveNewCallBack);
    }

    $scope.saveNewCallBack = function () {
        
        $scope.geoTypeNameNew = '';
        $scope.getAll();
    }

    $scope.deleteGeoType = function (id) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        var itm = new GeofenceType({ token: token });
        itm.$delGeoType({ token: token, id: id }, function (data) {
            if (_.isObject(jsonGeoTypes)) {
                jsonGeoTypes = false;
            }
            $scope.getAll();
        }, function (err) {
            var b = 1;
            alert('Failed deleting Geofence Type');
        });
    }

    $scope.closeDlg = function () {
        $('#dlgGeoTypes').dialog("close");
    }

}]);

function openGeofenceTypes() {
    try {
        var dlgGeoTypes = angular.element($("#dlgGeoTypes")).scope();
        dlgGeoTypes.openDialog();
    }
    catch (err) {

    }
}