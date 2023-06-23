var services = angular.module('etApp.services', ['ngResource']);

services.factory('CustomerDet', ['$resource', function ($resource) {
    return $resource('https://pre.etrack.ws/etrack.svc/customerDetails/:token/:noCache/:id',
        { token: '@token', noCache: '@noCache', id: '@id' },
        {
            getDetails: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0 } },
            save: { method: "POST", params: { token: 0 } }
        });
}]);