crmApp.controller('userProfileCtrl', ['$scope', '$location', '$timeout', 'UserProfile', 'userProfile', function ($scope, $location, $timeout, UserProfile, userProfile) {
    var a = 1;

    $scope.companyName = userProfile.companyName;
    $scope.joined = moment(userProfile.createdOn, "MM/DD/YYYY").fromNow();

    var b = 1;

}]);
