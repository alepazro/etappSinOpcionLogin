crmApp.controller('customerCtrl', ['$scope', '$location', 'PendingCustomer', 'customer', function ($scope, $location, PendingCustomer, customer) {

    $scope.customer = customer;

    $scope.onBoardingDone = function () {
        var token = getCookie('ETCRMTK');
        $scope.customer.$onBoardingDone({ token: token, id: $scope.customer.id }, function (data) {
            if (data.isOk == true) {
                alert('Customer marked as OnBoarded.');
                $location.path('/');
            }
            else {
                alert('Failed marking this customer as OnBoarded.  Please try again.');
            }
        }, 
        function (data) {
            var a = 1;
        });
    }

}]);