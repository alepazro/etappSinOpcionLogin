var services = angular.module('crmApp.services', ['ngResource']);

services.factory('OnlineUser', ['$resource', function ($resource) {
    return $resource('cases.svc/onlineSupport/onlineUsers/:token/:noCache/:id',
        { token: '@token', noCache: '@noCache', id: '@id' },
        {
            onlineSupportDone: { method: "POST", params: { token: 0, id: 0 } }
        });
}]);

services.factory('OnlineUsersLoader', ['OnlineUser', '$route', '$q', function (OnlineUser, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        if (token == '') {
            location.href = 'crmLogin.html';
        }
        else {
            OnlineUser.get({ token: token, noCache: Math.random() }, function (onlineUsers) {
                delay.resolve(onlineUsers);
            }, function (data) {
                delay.reject('Unable to fetch online users');
            }, function (x, y, z) {
                var a = 1;
            });
            return delay.promise;
        }
    };
}]);


services.factory('UserProfile', ['$resource', function ($resource) {
    return $resource('cases.svc/onlineSupport/userProfile/:token/:noCache/:id',
        { token: '@token', noCache: '@noCache', id: '@id' },
        {
            userProfileDone: { method: "POST", params: { token: 0, id: 0 } }
        });
}]);

services.factory('UserProfileLoader', ['UserProfile', '$route', '$q', function (UserProfile, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        if (token == '') {
            location.href = 'crmLogin.html';
        }
        else {
            UserProfile.get({ token: token, noCache: Math.random(), id: $route.current.params.id }, function (userProfile) {
                delay.resolve(userProfile);
            }, function (data) {
                delay.reject('Unable to fetch user profile');
            }, function (x, y, z) {
                var a = 1;
            });
            return delay.promise;
        }
    };
}]);
