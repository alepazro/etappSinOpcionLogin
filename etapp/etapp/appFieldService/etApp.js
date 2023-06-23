var etApp = angular.module('etApp', ['ngResource', 'ngRoute', 'etApp.services', 'kendo.directives']);

//etApp.config(['$routeProvider', function ($routeProvider) {
//    $routeProvider.
//        when('/', {
//            controller: 'jobsController',
//            resolve: {
//                workZones: function (WorkZonesLoader) {
//                    return WorkZonesLoader();
//                }
//            },
//            templateUrl: '../appFieldService/views/jobs.html'
//        }).
//        when('/job/:id', {
//            controller: 'jobController',
//            resolve: {
//                job: function (JobLoader) {
//                    return JobLoader();
//                }
//            },
//            templateUrl: '../appFieldService/views/job.html'
//        }).
//        otherwise({
//            redirectTo: '/'
//        });
//}]);
