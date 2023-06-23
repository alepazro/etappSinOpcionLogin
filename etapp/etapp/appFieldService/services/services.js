var services = angular.module('etApp.services', ['ngResource']);

services.factory('SupportLists', ['$resource', function ($resource) {
    return $resource('etrest.svc/jobSupportTables/:token/:noCache',
        { token: '@token', noCache: '@noCache' },
        {
            getAll: { method: "GET", params: { token: 0, noCache: 0 } }
        });
}]);


//===============================================================================================
services.factory('WorkZone', ['$resource', function ($resource) {
    return $resource('etrest.svc/workZones/:token',
        { token: '@token', noCache: '@noCache' },
        {
            saveWorkZone: { method: "POST", params: { token: 0 } }
        });
}]);

services.factory('WorkZonesLoader', ['WorkZone', '$route', '$q', function (WorkZone, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETTK');
        WorkZone.query({ token: token, noCache: Math.random() }, function (workZones) {
            delay.resolve(workZones);
        }, function (data) {
            delay.reject('Unable to fetch Work Zones');
        });
        return delay.promise;
    };
}]);

//===============================================================================================
services.factory('Technician', ['$resource', function ($resource) {
    return $resource('etrest.svc/technicians/:token/:wzId',
        { token: '@token', noCache: '@noCache', wzId: '@wzId' },
        {
            saveTechnician: { method: "POST", params: { token: 0 } },
            getTechnicians: { method: "GET", isArray: true, params: { token: 0, wzId: 0 } }
        });
}]);

//===============================================================================================
services.factory('Job', ['$resource', function ($resource) {
    return $resource('etrest.svc/jobs/:token/:noCache/:statId/:wzId/:techId/:jobNo/:custName/:jobId',
        { token: '@token', noCache: '@noCache', statId: '@statId', wzId: '@wzId', techId: '@techId', jobNo: '@jobNo', custName: '@custName', jobId: '@jobId' },
        {
            saveJob: { method: "POST", params: { token: 0 } },
            getJobs: { method: "GET", isArray: true, params: { token: 0, noCache: 0, statId: 0, wzId: 0, techId: 0, jobNo: 0, custName: 0 } }
        });
}]);

services.factory('JobLoader', ['Job', '$route', '$q', function (Job, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        Job.get({ token: token, noCache: noCache, jobId: $route.current.params.id }, function (job) {
            delay.resolve(job);
        }, function (data) {
            delay.reject('Unable to fetch Job');
        });
        return delay.promise;
    };
}]);

//===============================================================================================
services.factory('JobStatus', ['$resource', function ($resource) {
    return $resource('etrest.svc/jobStatus/:token/:noCache',
        { token: '@token', noCache: '@noCache' },
        {
            getJobStatus: { method: "GET", isArray: true, params: { token: 0, noCache: 0 } }
        });
}]);

services.factory('CustomerSearch', ['$resource', function ($resource) {
    return $resource('https://localhost:44385/etrack.svc/customerSearch/:token/:noCache/:custName',
        { token: '@token', noCache: '@noCache', custName: '@custName' },
        {
            getCustomers: { method: "GET", isArray: true, params: { token: 0, noCache: 0, custName: 0 } }
        });
}]);

//===============================================================================================
