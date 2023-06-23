var services = angular.module('etApp.services', ['ngResource']);

services.factory('DevicesList', ['$resource', function ($resource) {
    return $resource('deviceSvc.svc/device/settingsList/:token/:noCache',
        {  token: '@token', noCache: '@noCache' });
}]);

services.factory('Device', ['$resource', function ($resource) {
    return $resource('deviceSvc.svc/device/:token/:noCache/:action/:id/:usrComment',
        { token: '@token', noCache: '@noCache', action: '@action', id: '@id', usrComment: '@usrComment' },
        {
            saveDevice: { method: "POST", params: { token: 0, action: 0, id: 0 } },
            changeDeviceStatus: { method: "GET", params: { token: 0, noCache: 0, action: 0, id: 0, usrComment: 0 } }
        });
}]);

services.factory('IDNameList', ['$resource', function ($resource) {
    return $resource('etrest.svc/idNameList/:token/:noCache/:listName',
        { token: '@token', noCache: '@noCache', listName: '@listName' },
        {
            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0, listName: 0 } }
        });
}]);



