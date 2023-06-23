crmApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/', {
            controller: 'pendingCustomersCtrl',
            resolve: {
                pendingCustomers: function (PendingCustomersLoader) {
                    return PendingCustomersLoader();
                }
            },
            templateUrl: '../appOnBoarding/views/pendingCustomers.html'
        }).

        when('/:id', {
            controller: 'customerCtrl',
            resolve: {
                customer: function (CustomerLoader) {
                    return CustomerLoader();
                }
            },
            templateUrl: '../appOnBoarding/views/customer.html'
        }).

    otherwise({
        redirectTo: '/'
    });
}]);
