var dataGrid = function ($scope) {

    var a = 1;

    var filterOptions = {
        filterText: "",
        useExternalFilter: true
    };

    var pagingOptions = {
        pageSizes: [50, 100, 200],
        pageSize: 50,
        totalServerItems: 0,
        currentPage: 1
    };

    var setPagingData = function (data, page, pageSize) {
        $scope.accounts = data;
        pagingOptions.totalServerItems = data.length;
        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };
    var getPagedDataAsync = function (pageSize, page, searchText) {
        setTimeout(function () {

            var data;

            if (searchText == undefined) {
                searchText = '';
            }
            var ft = searchText.toLowerCase();
            var url = 'vtAPI.svc/accounts/pagedData?token=' + getCookie('vttk') + '&searchTerm=' + escape(ft) + '&page=' + page + '&pageSize=' + pageSize;

            $.ajax({
                url: url,
                type: "GET",
                data: 0,
                dataType: 'json',
                contentType: "application/json; charset=utf-8",
                processdata: true,
                success: function (data, textStatus, jqXHR) {
                    setPagingData(data, page, pageSize);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('error');
                },
                async: false
            });
        }, 100);
    };

    getPagedDataAsync(pagingOptions.pageSize, pagingOptions.currentPage);

    $scope.$watch('pagingOptions', function (newVal, oldVal) {
        if (newVal.currentPage !== oldVal.currentPage) {
            getPagedDataAsync(pagingOptions.pageSize, pagingOptions.currentPage, filterOptions.filterText);
        }
    }, true);
    $scope.$watch('filterOptions', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            getPagedDataAsync(pagingOptions.pageSize, pagingOptions.currentPage, filterOptions.filterText);
        }
    }, true);

    var editTemplate = '<button type="button" class="btn btn-mini" ng-click="editAccount(row)" >Edit</button>'
    var removeTemplate = '<button type="button" class="btn btn-mini btn-danger" ng-click="removeAccount(row)" >Del</button> '

    var gridOptions = {
        data: 'accounts',
        enablePaging: true,
        showFooter: true,
        pagingOptions: pagingOptions,
        filterOptions: filterOptions,
        rowHeight: 25,
        columnDefs: [
            { field: 'name', displayName: 'Account Name' },
            { field: 'contactName', displayName: 'Contact Name' },
            { field: 'email', displayName: 'Email' },
            { field: 'phone', displayName: 'Phone' },
            { field: 'fullAddress', displayName: 'Address' },
            { field: 'edit', displayName: 'Edit', cellTemplate: editTemplate, width: "40px" },
            { field: 'del', displayName: 'Del', cellTemplate: removeTemplate, width: "40px" }]
    };

    var editAccount = function (row) {
        $scope.editAccount(row);
    };

    var removeAccount = function (row) {
        $scope.removeAccount(row);
    };
}