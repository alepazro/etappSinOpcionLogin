etApp.controller('invoicesController', ['$scope', '$location', 'invoices', 'Invoice', function ($scope, $location, invoices, Invoice) {

    $scope.companies = getCompaniesList();

    $scope.queryInvoices = function () {
        var token = getCookie('ETCRMTK');
        var custId = $scope.companyId;
        var noCache = Math.floor((Math.random() * 100000) + 1);
        Invoice.query({ token: token, custId: custId, noCache: noCache }, function (data) {
            $scope.invoices = data;

            $("#grid").kendoGrid({
                columns: [{
                    field: "invoiceNumber",
                    title: "Inv. No."
                },
                {
                    field: "invoiceDate",
                    title: "Date"
                },
                { field: "total", title: "Total Amount" },
                { field: "paid", title: "Total Paid" },
                { field: "balance", title: "Balance" }
                ],
                dataSource: {
                    data: data
                }
            });

        });

    }

}]);
