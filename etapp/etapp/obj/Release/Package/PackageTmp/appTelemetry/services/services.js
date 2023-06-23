var services = angular.module('etApp.services', ['ngResource']);

services.factory('DevOutput', ['$resource', function ($resource) {
    return $resource('https://pre.etrack.ws/etrack.svc/telemetry/:token/:noCache/:deviceId/:relNum/:relState',
        { token: '@token', noCache: '@noCache', deviceId: '@deviceId', relNum: '@relNum', relState: '@relState' },
        {
            getOutputs: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0 } }
        });
}]);


services.factory('FleetHeartBeat', ['$resource', function ($resource) {
    return $resource('https://pre.etrack.ws/etrack.svc/fleetheartbeat/:token/:noCache',
        { token: '@token', noCache: '@noCache'},
        {
            getHeartBeat: { method: "GET", params: { token: 0, noCache: 0} }
        });
}]);

//    return $resource('https://pre.etrack.ws/etrack.svc/fleetheartbeat/:token/:noCache',


