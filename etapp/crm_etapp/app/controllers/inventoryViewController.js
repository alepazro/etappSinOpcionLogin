etApp.controller('inventoryViewController', ['$scope', '$location', 'inventory', 'Inventory', function ($scope, $location, inventory, Inventory) {

    $scope.inventory = inventory;

    $("#grid").kendoGrid({
        columns: [
            { field: "deviceId", title: "Device ID" },
            { field: "deviceTypeName", title: "Type" },
            { field: "serialNumber", title: "Serial Number" },
            { field: "imei", title: "IMEI" },
            { field: "simNo", title: "Sim Number" },
            { field: "simAreaCode", title: "Area Code" },
            { field: 'simPhoneNumber', title: 'Phone Number' },
            { field: 'createdOn', title: 'Created On' },
            { field: 'lastUpdatedOn', title: 'Last Updated On' },
            { field: 'eventDate', title: 'Last Event Date' }
        ],
        dataSource: {
            data: inventory
        }
    });


} ]);
