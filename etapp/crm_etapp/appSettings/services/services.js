var services = angular.module('etApp.services', ['ngResource']);

services.factory('DevOutput', ['$resource', function ($resource) {
    return $resource('https://etrack.ws/etrack.svc/telemetry/:token/:noCache/:deviceId/:relNum/:relState',
        { token: '@token', noCache: '@noCache', deviceId: '@deviceId', relNum: '@relNum', relState: '@relState' },
        {
            getOutputs: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0 } },
            getAllDevices: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } }
    });
}]);

services.factory('HourMeter', ['$resource', function ($resource) {
    return $resource('https://etrack.ws/etrack.svc/hourMeters/:token/:noCache/:deviceId',
        { token: '@token', noCache: '@noCache', deviceId: '@deviceId' },
        {
            getHourMeters: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0 } },
            getAllDevices: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } },
            saveHourMeter: { method: "POST", params: { token: 0 } }
        });
}]);

services.factory('GeofenceType', ['$resource', function ($resource) {
    return $resource('https://etrack.ws/etrack.svc/geofenceType/:token/:noCache/:id',
        { token: '@token', noCache: '@noCache', id: '@id' },
        {
            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } },
            save: { method: "POST", params: { token: 0 } },
            delGeoType: { method: "DELETE", params: { token: 0, id: 0 } }
        });
}]);
