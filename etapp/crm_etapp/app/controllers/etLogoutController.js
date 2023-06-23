etApp.controller('logoutController', ['$scope', '$location', function ($scope, $location) {

    $scope.logout = function () {
        deleteCookie('ETTK');
        location.href = 'login.html';
    };

}]);