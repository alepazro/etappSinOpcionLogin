crmApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/', {
            controller: 'onlineUsersCtrl',
            resolve: {
                onlineUsers: function (OnlineUsersLoader) {
                    return OnlineUsersLoader();
                }
            },
            templateUrl: '../appOnlineSupport/views/onlineUsers.html'
        }).
        when('/:id', {
            controller: 'userProfileCtrl',
            resolve: {
                userProfile: function (UserProfileLoader) {
                    return UserProfileLoader();
                }
            },
            templateUrl:'../appOnlineSupport/views/userProfile.html'

        }).

    otherwise({
        redirectTo: '/'
    });
}]);
