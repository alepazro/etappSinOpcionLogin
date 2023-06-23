var services = angular.module('crmApp.services', ['ngResource']);

services.factory('PendingCustomer', ['$resource', function ($resource) {
    return $resource('cases.svc/onboarding/pending/:token/:noCache/:id',
        { token: '@token', noCache: '@noCache', id: '@id' },
        {
            onBoardingDone: { method: "POST", params: { token: 0, id: 0 } }
        });
}]);

services.factory('PendingCustomersLoader', ['PendingCustomer', '$route', '$q', function (PendingCustomer, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        if (token == '') {
            location.href = 'crmLogin.html';
        }
        else {
            PendingCustomer.query({ token: token, noCache: Math.random() }, function (pendingCustomers) {
                delay.resolve(pendingCustomers);
            }, function (data) {
                delay.reject('Unable to fetch Pending Customers');
            });
            return delay.promise;
        }
    };
}]);

services.factory('CustomerLoader', ['PendingCustomer', '$route', '$q', function (PendingCustomer, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        if (token == '') {
            location.href = 'crmLogin.html';
        }
        else {
            PendingCustomer.get({ token: token, noCache: Math.random(), id: $route.current.params.id },
                function (customer) {
                    delay.resolve(customer);
                }, function (data) {
                    delay.reject('Unable to fetch Customer');
                });
            return delay.promise;
        }
    };
}]);