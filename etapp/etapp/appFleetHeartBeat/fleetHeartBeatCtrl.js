etApp.controller('fleetHeartBeatCtrl', ['$scope', '$http', '$timeout', 'FleetHeartBeat', function ($scope, $http, $timeout, FleetHeartBeat) {

    var refreshingPromise = 0;
    var isRefreshing = false;

    $scope.fleet = {
        fleetMiles: 0,
        drivingHours: 0,
        idleHours: 0,
        mostActive: { name: '', miles: 0 },
        bestDriver: { name: '', incidents: 0 },
        worstDriver: { name: '', incidents: 0 }
    };


    $scope.startRefreshing = function () {
        if (isRefreshing) return;
        isRefreshing = true;
        (function refreshEvery() {
            //Do refresh
            var token = getTokenCookie('ETTK');
            FleetHeartBeat.get({ token: token, noCache: Math.random() }, function (newData) {
                var b = 1;

                $scope.fleet = newData;

            }, function (data, y, z) {
                //An error has happened in the refresh
                var b = 1;
            });

            //If async in then in callback do...
            refreshingPromise = $timeout(refreshEvery, 90000)
        }());
    }

    $scope.startRefreshing();
}]);