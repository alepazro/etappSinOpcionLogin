var services = angular.module('etApp.services',  ['ngResource']);

services.factory('Customer', ['$resource', function ($resource) {
    return $resource('etrest.svc/customers/:action/token/:token/:noCache/:id',
        { action: '@action', token: '@token', noCache: '@noCache', id: '@id' },
        {
            saveNewCustomer: { method: "POST", params: { action: 0, token: 0} }
        });
}]);

    services.factory('CustomerLoader', ['Customer', '$route', '$q', function (Customer, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        Customer.get({ token: token, noCache: Math.random(), id: $route.current.params.customerId },
            function (customer) {
                delay.resolve(customer);
            },
            function (data) {
                delay.reject('Unable to fetch customer ' + $route.current.params.customerId);
            }
        );
        return delay.promise;
    };
}]);

    services.factory('Device', ['$resource', function ($resource) {
        return $resource('etrest.svc/devices/search/:token/:searchKey/:keyValue/:noCache',
            { token: '@token', noCache: '@noCache', searchKey: '@searchKey', keyValue: '@keyValue' },
            {
                saveDevice: { method: "POST", params: { token: 0 } }
            });
    }]);

    services.factory('DeviceAction', ['$resource', function ($resource) {
        return $resource('etrest.svc/deviceAction/:token',
            { token: '@token' },
            {
                setDeviceAction: { method: "POST", params: { token: 0 } }
            });
    }]);


//======================================================================
//Inventory

services.factory('Inventory', ['$resource', function ($resource) {
    return $resource('etrest.svc/inventory/:action/:token/:custId/:noCache/:id',
        { action: '@action', token: '@token', custId: '@custId', noCache: '@noCache', id: '@id' },
        {
            saveInventory: { method: "POST", params: { token: 0, id: 0} },
            saveAssignment: { method: "POST", params: { action: 0, token: 0, custId: 0} }
        });
} ]);

services.factory('InventoryLoader', ['Inventory', '$route', '$q', function (Inventory, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        Inventory.query({ token: token}, function (inventory) {
            delay.resolve(inventory);
        }, function (data) {
            alert('Unable to fetch inventory');
            delay.reject('Unable to fetch inventory');
        });
        return delay.promise;
    };
} ]);

services.factory('AssignInventory', ['$resource', function ($resource) {
    return $resource('etrest.svc/assignInventory/:token',
        { token: '@token' },
        {
            saveAssignedInventory: { method: "POST", params: { token: 0} }
        });
} ]);

services.factory('AddInventory', ['$resource', function ($resource) {
    return $resource('etrest.svc/addInventory/:token',
        { token: '@token' },
        {
            saveNewInventory: { method: "POST", params: { token: 0 } }
        });
}]);

//======================================================================
    //SuspendCompany

services.factory('SuspendCompany', ['$resource', function ($resource) {
    return $resource('etrest.svc/suspendCompany/:token',
    { token: '@token' },
    {
        saveSuspendCompany: { method: "POST", params: { token: 0} }
    });
} ]);


services.factory('SuspendCompanyLoader', ['SuspendCompany', '$route', '$q', function (SuspendCompany, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        SuspendCompany.query({ token: token }, function (suspendCompany) {
            delay.resolve(suspendCompany);
        }, function (data) {
            alert('Unable to fetch suspendCompany');
            delay.reject('Unable to fetch suspendCompany');
        });
        return delay.promise;
    };
} ]);

//======================================================================
//QB Match
services.factory('QBMatch', ['$resource', function ($resource) {
    return $resource('etrest.svc/qbMatch/:token/:crmId/:qbId',
    { token: '@token', crmId: '@crmId', qbId: '@qbId' },
    {
        linkItems: { method: "POST", params: { token: 0, crmId: 0, qbId: 0 } },
        unlinkItems: { method: "POST", params: { token: 0, crmId: 0 } }
    });
}]);

services.factory('QBMatchLoader', ['QBMatch', '$route', '$q', function (QBMatch, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        QBMatch.get({ token: token }, function (qbMatch) {
            delay.resolve(qbMatch);
        }, function (data) {
            alert('Unable to fetch Customers for Match');
            delay.reject('Unable to fetch Customers for Match');
        });
        return delay.promise;
    };
}]);

//======================================================================
//Invoices

services.factory('Invoice', ['$resource', function ($resource) {
    return $resource('etrest.svc/invoice/:token/:custId/:noCache/:id',
        { token: '@token', custId: '@custId', noCache: '@noCache', id: '@id' },
        {
            saveInvoice: { method: "POST", params: { token: 0, id: 0 } }
        });
}]);

services.factory('InvoicesLoader', ['Invoice', '$route', '$q', function (Invoice, $route, $q) {
    return function () {
        var delay = $q.defer();
        var token = getCookie('ETCRMTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        Invoice.query({ token: token, custId: '0', noCache: noCache }, function (invoices) {
            delay.resolve(invoices);
        }, function (data, b, c) {
            alert('Unable to fetch invoices');
            delay.reject('Unable to fetch invoices');
        });
        return delay.promise;
    };
}]);

//======================================================================
//Shipments

services.factory('ConfirmShipment', ['$resource', function ($resource) {
    return $resource('etrest.svc/confirmShipment/:token',
        { token: '@token' },
        {
            save: { method: "POST", params: { token: 0 } }
        });
}]);
