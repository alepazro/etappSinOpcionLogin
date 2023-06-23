etApp.controller('customerNewController', ['$scope', '$location', 'Customer', function ($scope, $location, Customer) {

    $scope.customer = new Customer({ token: getCookie('ETCRMTK') });

    $scope.copyShipping2Billing = function () {
        $scope.customer.billingContact = $scope.customer.firstName + ' ' + $scope.customer.lastName;
        $scope.customer.billingEmail = $scope.customer.email;
        $scope.customer.billingPhone = $scope.customer.phone;

        $scope.customer.billingStreet = $scope.customer.street;
        $scope.customer.billingCity = $scope.customer.city;
        $scope.customer.billingState = $scope.customer.state;
        $scope.customer.billingPostalCode = $scope.customer.postalCode;
    };

    $scope.save = function () {
        var token = getCookie('ETCRMTK');
        $scope.customer.$saveNewCustomer({ action: 'new', token: token }, function (customer) {
            alert("Company created");
            location.href = "crmMainPanel.html";
        });
    };

    $scope.cancel = function () {
        delete $scope.customer;
        location.href = "crmMainPanel.html";
    };


} ]);