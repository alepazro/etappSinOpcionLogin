var services = angular.module('etApp.services', ['ngResource']);

services.factory('IDNameList', ['$resource', function ($resource) {
    return $resource('https://localhost:44385/etrack.svc/idNameList/:token/:noCache/:listName',
        { token: '@token', noCache: '@noCache', listName: '@listName' },
        {
            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0, listName: 0 } }
        });
}]);

services.factory('Device', ['$resource', function ($resource) {
    return $resource('https://localhost:44385/etrack.svc/maintDevice/:token/:noCache/:id',
        { token: '@token', noCache: '@noCache', id: '@id' },
        {
            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } },
            saveDevice: { method: "POST", params: { token: 0 } }
        });
}]);

//services.factory('ServiceItem', ['$resource', function ($resource) {
//    return $resource('https://localhost:44385/etrack.svc/serviceItem/:token/:noCache/:id',
//        { token: '@token', noCache: '@noCache', id: '@id' },
//        {
//            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } }
//        });
//}]);

services.factory('MaintSchedule', ['$resource', function ($resource) {
    return $resource('https://localhost:44385/etrack.svc/maintSchedule/:token/:noCache/:deviceId/:id',
        { token: '@token', noCache: '@noCache', deviceId: '@deviceId', id: '@id' },
        {
            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } },
            getByDevice: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0 } }
        });
}]);

services.factory('MaintLog', ['$resource', function ($resource) {
    return $resource('https://localhost:44385/etrack.svc/maintLog/:token/:noCache/:deviceId/:id',
        { token: '@token', noCache: '@noCache', deviceId: '@deviceId', id: '@id' },
        {
            getAll: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } },
            getByDevice: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0 } }
        });
}]);

services.factory('SupportLists', ['$resource', function ($resource) {
    return $resource('https://localhost:44385/etrack.svc/maintSupportLists/:token/:noCache',
        { token: '@token', noCache: '@noCache' },
        {
            getAll: { method: "GET", params: { token: 0, noCache: 0 } }
        });
}]);

services.factory('MaintItem', ['$resource', function ($resource) {
    return $resource('etrest.svc/maintItem/:token/:noCache/:deviceId/:id',
        { token: '@token', noCache: '@noCache', deviceId: '@deviceId', id: '@id' },
        {
            saveItem: { method: "POST", params: { token: 0, deviceId: 0, id: 0 } },
            getItem: { method: "GET", params: { token: 0, noCache: 0, deviceId: 0, id: 0 } },
            delItem: { method: "DELETE", params: { token: 0, deviceId: 0, id: 0 } },
            saveCompletedItem: { method: "PUT", params: { token: 0, deviceId: 0, id: 0 } }
        });
}]);
