etApp.controller('quickMsgCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.deviceId = 0;
    $scope.deviceName = '';
    $scope.driverId = 0;
    $scope.message = '';
    $scope.drivers = getQuickMsgDriversList();

    $scope.openDialog = function (deviceId, deviceName, driverId) {
        $scope.deviceId = deviceId;
        $scope.deviceName = deviceName;
        $scope.driverId = driverId;

        $("#dlgQuickMsg").dialog("open");

    };

    $scope.quickMsgDriverChange = function () {
        $scope.driver = _.findWhere($scope.drivers, { id: $scope.driverId });
    };

    //channel: 1=sms;2=email
    $scope.quickMsgSend = function (channel) {
        if ($scope.driverId == undefined) {
            alert('Please select a driver.');
            return
        }
        if ($scope.driverId ==  '0') {
            alert('Please select a driver.');
            return
        }
        if (channel == 1 && $scope.driver.phone == '') {
            alert("Please set the driver's phone before sending messages via text.");
            return
        }
        if (channel == 2 && $scope.driver.email == '') {
            alert("Please set the driver's email before sending messages via email.");
            return
        }
        if ($scope.message.length == 0) {
            alert('Please enter a message.')
            return
        }

        var token = getTokenCookie('ETTK')
        var data = {
            driverId: $scope.driverId,
            channel: channel,
            message: $scope.message
        }

        $http({
            url: 'https://etrack.ws/etrest.svc/quickMsg/' + token,
            method: "POST",
            data: data
        }).then(function (response) {
            // success
            alert('Message Sent. It may take up to a minute for the message to be delivered by the system.');

            $scope.driverId = 0;
            $scope.message = '';
            $scope.driver = false;

            $("#dlgQuickMsg").dialog("close");

        }, function (response) { // optional
            // failed
            var b = 1;
        });

    }

}]);

function getQuickMsgDriversList() {
    try {
        var drivers = false;
	var url = 'https://etrack.ws/etrest.svc/getQuickMsgDriversList/' + getTokenCookie('ETTK') + '/' + Math.random();
        $.ajax({
            url: url,
            type: "GET",
            data: 0,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            processdata: true,
            success: function (data, textStatus, jqXHR) {
                drivers = data;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Failed to fetch drivers');
            },
            async: false
        });

        return drivers;
    }
    catch (err) {
        alert(err.description);
    }
}