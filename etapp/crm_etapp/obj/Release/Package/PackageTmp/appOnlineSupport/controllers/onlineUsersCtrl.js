crmApp.controller('onlineUsersCtrl', ['$scope', '$location', '$timeout', 'OnlineUser', 'onlineUsers', function ($scope, $location, $timeout, OnlineUser, onlineUsers) {

    var refreshingPromise = 0;
    var isRefreshing = false;

    $scope.qtyUnits = onlineUsers.qtyUnits;
    $scope.usersCount = onlineUsers.onlineUsers.length;

    var data = new kendo.data.DataSource({
        data: onlineUsers.onlineUsers
    });

    $scope.gridOptions = {
        dataSource: data,
        sortable: {
            mode: "single"
        },
        columns: [
          { template: "<button class='k-button' ng-click='viewDetails(this)'>View Profile</button>", width: "100px" },
          { field: "id", title: "id", hidden: true },
          { field: "companyName", title: "Company", width: "200px" },
          { field: "userName", title: "User", width: "200px" },
          { field: "phone", title: "Phone", width: "120px" },
          { field: "mobile", title: "Mobile", width: "120px" },
          { field: "email", title: "Email", width: "150px" },
          { field: "currentPage", title: "Current Page", width: "150px" },
          { field: "currentPageTime", title: "Time On Page", width: "150px" },
          { field: "sessionTime", title: "Session Time", width: "150px" }
        ]
    };

    $scope.viewDetails = function (e) {
        var elem = e.dataItem;
        var id = elem.id;
        $location.path('/' + id);
        if (isRefreshing == true) {
            $timeout.cancel(refreshingPromise);
            isRefreshing = false;
        }
    }

     $scope.startRefreshing = function () {
        if (isRefreshing) return;
        isRefreshing = true;
        (function refreshEvery() {
            //Do refresh
            var token = getCookie('ETCRMTK');
            OnlineUser.get({ token: token, noCache: Math.random() }, function (newData) {
                var b = 1;
                $scope.gridOptions.dataSource = newData.onlineUsers;
                try{
                    $('#onlineUsersGrid').data('kendoGrid').dataSource.data(newData.onlineUsers);
                    $scope.usersCount = newData.onlineUsers.length;
                    $scope.qtyUnits = newData.qtyUnits;
                }
                catch(err){
                    var a = 1;
                }
            }, function (data) {
                //An error has happened in the refresh
                var b = 1;
            });

            //If async in then in callback do...
            refreshingPromise = $timeout(refreshEvery, 30000)
        }());
    }

    $scope.startRefreshing();
    
}]);