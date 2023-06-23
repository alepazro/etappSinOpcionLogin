etApp.controller('inactivateCompany', ['$scope', '$location', 'SuspendCompany', 'suspendCompany', function ($scope, $location, SuspendCompany, suspendCompany) {

    $scope.companies = suspendCompany;
    $scope.suspendedReasons = getSuspendedReasonsList();
    $scope.selectedCompany = [];

    $scope.changeCompany = function () {
        $scope.selectedCompany = _.findWhere($scope.companies, { id: $scope.companyId });
    }

    $scope.save = function () {
        var token = getCookie('ETCRMTK');
        var suspendCompany = new SuspendCompany({ token: getCookie('ETCRMTK') });
        suspendCompany.id = $scope.selectedCompany.id;
        suspendCompany.isSuspended = $scope.selectedCompany.isSuspended;
        suspendCompany.suspendedId = $scope.selectedCompany.suspendedId;

        suspendCompany.$saveSuspendCompany({ token: token }, function (data) {
            location.href = "crmMainPanel.html";
        });

    };

    $scope.cancel = function () {
        delete $scope.companies;
        location.href = "crmMainPanel.html";
    };

} ]);