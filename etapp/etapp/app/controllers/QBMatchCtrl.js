etApp.controller('qbMatchCtrl', ['$scope', '$location', 'qbMatch', 'QBMatch', function ($scope, $location, qbMatch, QBMatch) {

    $scope.qbMatch = qbMatch;

    $scope.crmFilterFunc = function (itm) {

        if (_.isUndefined($scope.crmFilterOpt)) {
            $scope.crmFilterOpt = '0';
        }

        switch ($scope.crmFilterOpt) {
            case '0':
                return true;
                break;
            case '1':
                return !itm.isMatched;
                break;
            case '2':
                return itm.isMatched;
                break;
        }
    }

    $scope.crmPick = function (id) {
        $scope.crmPickedId = id;
    }

    $scope.qbPick = function (id) {
        $scope.qbPickedId = id;
    }

    $scope.linkPair = function () {
        if (_.isUndefined($scope.crmPickedId)) {
            $scope.crmPickedId = '';
        }
        if (_.isUndefined($scope.qbPickedId)) {
            $scope.qbPickedId = '';
        }

        if ($scope.crmPickedId == '') {
            alert('Please select a CRM Customer');
            return;
        }
        if ($scope.qbPickedId == '') {
            alert('Please select a Quickbooks Customer');
            return;
        }

        var crmCustomer = _.find($scope.qbMatch.crmCustomers, function (obj) {
            return (obj.id == $scope.crmPickedId);
        });

        var qbCustomer = _.find($scope.qbMatch.qbCustomers, function (obj) {
            return (obj.id == $scope.qbPickedId);
        });

        var token = getCookie('ETCRMTK');
        var crmId = $scope.crmPickedId;
        var qbId = $scope.qbPickedId;

        var qbLink = new QBMatch({ token: token, crmId: crmId, qbId: qbId });
        qbLink.$linkItems({ token: token, crmId: crmId, qbId: qbId }, function (data) {
            if (data.isOk == true) {
                crmCustomer.isMatched = true;
                $scope.crmPickedId = '';
                $scope.qbPickedId = '';
            }
            else {
                alert('Failed to link these customers.  Please try again or contact Technical Support.');
            }
        }, function (err) {
            alert('Could not link these customers.  Please try again or contact Technical Support.');
        });

    }

    $scope.unlinkPair = function (id) {
        var token = getCookie('ETCRMTK');
        var qbLink = new QBMatch({ token: token, crmId: id });
        qbLink.$unlinkItems({ token: token, crmId: id }, function (data) {
            if (data.isOk == true) {
                var crmCustomer = _.find($scope.qbMatch.crmCustomers, function (obj) {
                    return (obj.id == id);
                });
                if (_.isObject(crmCustomer)) {
                    crmCustomer.isMatched = false;
                    $scope.crmPickedId = '';
                    $scope.qbPickedId = '';
                }
            }
            else {
                alert('Failed to unlink this customer.  Please try again or contact Technical Support.');
            }
        }, function (err) {
            alert('Could not unlink this customer.  Please try again or contact Technical Support.');
        });

    }

    $scope.showMatch = function (id) {
        var crmCustomer = _.find($scope.qbMatch.crmCustomers, function (obj) {
            return (obj.id == id);
        });

        if (_.isObject(crmCustomer)) {
            var qbId = crmCustomer.qbId;

            if (qbId == '') {
                alert(crmCustomer.name + ' is not linked yet.');
            }
            else {
                var qbCustomer = _.find($scope.qbMatch.qbCustomers, function (obj) {
                    return (obj.id == qbId);
                });

                if (_.isObject(qbCustomer)) {
                    alert('CRM: ' + crmCustomer.name + ' is matched with QB: ' + qbCustomer.companyName);
                }
                else {
                    alert('No match found.  If it is linked, please unlink and link again.');
                }
            }

        }
        else {
            alert('Something went wrong... you clicked a customer that I could not find. Please try again.');
        }

    }

    $scope.markQBMatched = function () {
        _.each($scope.qbMatch.crmCustomers, function (crmObj) {
            if (crmObj.qbId != '') {
                var qbCustomer = _.find($scope.qbMatch.qbCustomers, function (qbObj) {
                    return (qbObj.id == crmObj.qbId);
                });
                if (_.isObject(qbCustomer)) {
                    qbCustomer.isMatched = true;
                }
            }
        });
    }

    $scope.markQBMatched();

}]);
