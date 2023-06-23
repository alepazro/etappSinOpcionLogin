var etApp = angular.module('etApp', ['ngGrid', 'ui.bootstrap', 'ngResource', 'ngCookies', 'etApp.services', 'etApp.directives', '$strap.directives', 'DeviceSelectorDir']);

etApp.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/', {
            controller: 'mainPanelController',
            templateUrl: '../app/views/home.html'
        }).
        when('/customers/new', {
            controller: 'customerNewController',
            templateUrl: '../app/views/customer.html'
        }).
        when('/devices/search', {
            controller: 'devicesSearchCtrl',
            templateUrl: '../app/views/searchDevice.html'
        }).

    //=========================================================
    //Views
        when('/inventory', {
            controller: 'inventoryViewController',
            resolve: {
                inventory: function (InventoryLoader) {
                    return InventoryLoader();
                }
            },
            templateUrl: '../app/views/inventoryView.html'
        }).

        when('/inventory/add', {
            controller: 'inventoryAddController',
            templateUrl: '../app/views/inventoryAdd.html'
        }).

        when('/assign', {
            controller: 'assignUnitsController',
            resolve: {
                inventory: function (InventoryLoader) {
                    return InventoryLoader();
                }
            },
            templateUrl: '../app/views/assignDevices.html'
        }).

        when('/shipment', {
            controller: 'confirmShipmentController',
            templateUrl: '../app/views/confirmShipment.html'
        }).

        when('/inactivate', {
            controller: 'inactivateCompany',
            resolve: {
                suspendCompany: function (SuspendCompanyLoader) {
                    return SuspendCompanyLoader();
                }
            },
            templateUrl: '../app/views/inactivateCompany.html'
        }).
        when('/invoices', {
            controller: 'invoicesController',
            resolve: {
                invoices: function (InvoicesLoader) {
                    return InvoicesLoader();
                }
            },
            templateUrl: '../app/views/invoices.html'
        }).

    //=========================================================
    //QB Match

        when('/qbMatch', {
            controller: 'qbMatchCtrl',
            resolve: {
                qbMatch: function (QBMatchLoader) {
                    return QBMatchLoader();
                }
            },
            templateUrl: '../app/views/QBMatch.html'
        }).

    otherwise({
        redirectTo: '/'
    });
} ]);
