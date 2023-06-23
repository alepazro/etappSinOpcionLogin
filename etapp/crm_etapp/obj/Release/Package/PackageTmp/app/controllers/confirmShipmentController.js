etApp.controller('confirmShipmentController', ['$scope', '$location', 'ConfirmShipment', function ($scope, $location, ConfirmShipment) {

    $scope.courriers = getGenericMasters('COURRIER');
    $scope.emails = getEmailTypes('SHIPPING');

    $scope.save = function () {
        if (_.isUndefined($scope.orderNumber) || $scope.orderNumber == '') {
            alert('Please enter Order Number');
            return;
        }
        if (_.isUndefined($scope.trackingNumber) || $scope.trackingNumber == '') {
            alert('Please enter Tracking Number');
            return;
        }

        var token = getCookie('ETCRMTK');
        var confirmShipment = new ConfirmShipment({ token: getCookie('ETCRMTK') });
        confirmShipment.orderNo = $scope.orderNumber;
        confirmShipment.emailTypeId = $scope.emailTypeId
        confirmShipment.courrierId = $scope.courrierId;
        confirmShipment.trackingNumber = $scope.trackingNumber;

        confirmShipment.$save({ token: token }, function (data) {
            if (data.isOk == true) {
                alert('Order shipment confirmed');
                $scope.orderNumber = '';
                $scope.trackingNumber = '';
            }
            else {
                alert(data.msg);
            }
        }, function (msg, x, y, z) {
            var a = 1;
            alert('ERROR PROCESSING SHIPMENT CONFIRMATION');
        });
    };

    $scope.cancel = function () {
        location.href = "crmMainPanel.html";
    };


}]);
