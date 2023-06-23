crmApp.controller('pendingCustomersCtrl', ['$scope', '$location', 'PendingCustomer', 'pendingCustomers', function ($scope, $location, PendingCustomer, pendingCustomers) {

    var data = new kendo.data.DataSource({
        data: pendingCustomers
    });

    $scope.gridOptions = {
        dataSource: data,
        sortable: {
            mode: "single"
        },
        columns: [
          { template: "<button class='k-button' ng-click='viewDetails(this)'>View Details</button>", width: "150px" },
          { field: "id", title: "id", hidden: true },
          { field: "name", title: "Name", width: "300px" },
          { field: "email", title: "Email", width: "300px" },
          { field: "phone", title: "Phone", width: "120px" },
          { field: "billCity", title: "Bill-To City", width: "120px" },
          { field: "billState", title: "Bill-To State", width: "120px" },
          { field: "createdOn", title: "Created On", width: "240px" },
          {field: "onBoardingNotes", title:"OnBoarding Notes"}
        ]
    };

    $scope.viewDetails = function (e) {
        var elem = e.dataItem;
        var id = elem.id;
        $location.path('/' + id);
    }

}]);