crmApp.controller('caseController', ['$scope', function ($scope) {

    $scope.thisCase = {};
    $scope.basicTables = [];

    $scope.saveCase = function (isValid) {
        if (!isValid) {
            alert('Please fill all required fields');
            return;
        }

        var res = {};
        res = jsonPOST('cases.svc', 'case', $scope.thisCase);
        if (res.isOk == true) {
            $scope.thisCase = res;
            alert('Saved');
        }
        else {
            alert('Please try again');
        }
    }

    $scope.closeCaseForm = function () {
        window.close();
    }

    $scope.loadBasicTables = function () {
        $scope.basicTables = jsonGET('cases.svc', 'casesBasicTables', 0, false);
        $scope.companies = jsonGET('cases.svc', 'companies', 0, true);
        $scope.technicians = jsonGET('cases.svc', 'technicians', 0, true);
    };

    $scope.loadCase = function () {
        var id = getParameterByName('id');
        if (id != '0' && id != '') {
            var params='id=' + id
            $scope.thisCase = jsonGET('cases.svc', 'case', params, false);
        }
        else {
            $scope.thisCase.id = '';
            $scope.thisCase.companyId = '';
            $scope.thisCase.categoryId = '';
            $scope.thisCase.typeId = '';
            $scope.thisCase.subTypeId = '';
            $scope.thisCase.assignedToId = '';
            $scope.thisCase.statusId = '0';
            $scope.thisCase.isClosed = false;
        }
    };

    $scope.companyChange = function (companyId) {
        var params = 'companyId=' + companyId;
        $scope.devices = jsonGET('cases.svc', 'devices', params, true);
    }

    $scope.pickupCase = function () {
        $scope.changeStatus('PICKUP', 0, 0);
    }

    $scope.completeCase = function () {
        $scope.changeStatus('COMPLETE', 0, 0);
    }

    //=======================================================
    //ASSIGN
    $scope.assignCase = function () {
        $('#modAssign').modal('show');
        $compile($('#modAssign'))($scope);
    }

    $scope.assignCommit = function () {
        if ($scope.assignedToId == '') {
            alert('Please select a person from the list.');
            return;
        }

        $scope.changeStatus('ASSIGN', $scope.assignedToId, 0);
        var usr = _.find($scope.technicians, function (itm) {
            return (itm.id == $scope.assignedToId);
        });
        $scope.thisCase.assignedToId = usr.id;
        $scope.thisCase.assignedToName = usr.name;
        $('#modAssign').modal('hide');
    }
    //=======================================================
    //REQUEST USER ACTION
    $scope.requestUserAction = function () {
        $scope.changeStatus('REQUESTUSERACTION', 0, 0);
    }
    //=======================================================

    $scope.cancelCase = function () {
        $scope.changeStatus('CANCEL', 0, 0);
    }

    $scope.reopenCase = function () {
        $scope.changeStatus('REOPEN', 0, 0);
    }

    $scope.changeStatus = function (actionName, param1, param2) {
        if ($scope.thisCase.id == '') {
            alert('Please save the case before changing status.');
            return;
        }

        var res = {};
        var data = {
            caseId: $scope.thisCase.id,
            action: actionName,
            param1: param1,
            param2: param2
        };
        res = jsonPOST('cases.svc', 'changeStatus', data);
        if (res.isOk == true) {
            $scope.thisCase.statusId = res.statusId;
            var stat = _.find($scope.basicTables.casesStatus, function (itm) {
                return (itm.id == res.statusId);
            });
            $scope.thisCase.statusName = stat.name;
            $scope.thisCase.isClosed = stat.isClosed;
        }
    }

    //=======================================================
    //ADD NOTE
    $scope.addNote = function () {
        $('#modAddNote').modal('show');
        $compile($('#modAddNote'))($scope);
    }
    $scope.addNoteCommit = function () {
        $scope.changeStatus('ADDNOTE', $scope.newNote, 0);
        $('#modAddNote').modal('hide');
        $scope.thisCase.notes = $scope.thisCase.notes + ' - ' + $scope.newNote;
        $scope.newNote = '';
    }
    //=======================================================

    $scope.loadBasicTables();
    $scope.loadCase();

}]);