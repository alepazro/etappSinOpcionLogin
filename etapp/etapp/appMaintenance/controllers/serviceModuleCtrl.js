etApp.controller('serviceModuleCtrl', ['$scope', '$http', 'Device', 'MaintSchedule', 'MaintLog', 'IDNameList', 'SupportLists', 'MaintItem', function ($scope, $http, Device, MaintSchedule, MaintLog, IDNameList, SupportLists, MaintItem) {

    //Get basic tables
    //===========================================================
    var token = getTokenCookie('ETTK');
    var noCache = Math.floor((Math.random() * 100000) + 1);
    SupportLists.getAll({ token: token, noCache: noCache }, function (data) {
        $scope.servicesTypes = data.servicesTypes;
        $scope.maintTasks = data.maintTasks;
        $scope.maintMeassures = data.maintMeassures;
        $scope.timeReferences = data.timeReferences;
        $scope.vehicleTypes = data.vehicleTypes;
    }, function (err) {
        var a = 1;
    });
    //===========================================================

    var data = new kendo.data.DataSource({
        data: []
    });

    $scope.serviceLogTabGridOptions = {
        toolbar: ["excel"],
        excel: {
            fileName: "ServiceLog.xlsx"
        },
        dataSource: data,
        sortable: {
            mode: "single"
        },
        columns: [
          { field: "completedOn", title: "Date", width: "120px" },
          { field: "taskName", title: "Service Item", width: "120px" },
          { field: "odometer", title: "Odometer", width: "120px" },
          { field: "ignitionHours", title: "Engine Hours", width: "100px" },
          { field: "input1Hours", title: "Input 1", width: "100px" },
          { field: "input2Hours", title: "Input 2", width: "100px" },
          { field: "input3Hours", title: "Input 3", width: "100px" },
          { field: "input4Hours", title: "Input 4", width: "100px" },
          { field: "cost", title: "Cost", width: "120px" }
        ]
    };
    //===========================================================

    var serviceScheduleData = new kendo.data.DataSource({
        data: []
    });

    $scope.serviceScheduleTabGridOptions = {
        toolbar: ["excel"],
        excel: {
            fileName: "ServiceSchedule.xlsx"
        },
        dataSource: serviceScheduleData,
        sortable: {
            mode: "single"
        },
        columns: [
          { field: "taskName", title: "Service Item", width: "120px" },
          { field: "frequency", title: "Frequency", width: "120px" },
          { field: "nextDueInWords", title: "Next Due", width: "120px" },
          { template: "<button class='k-button' ng-click='editMaintItem_v2(this)'>Edit</button>", width: "80px" },
          { template: "<button class='k-button' ng-click='deleteMaintItem_v2(this)'>Delete</button>", width: "80px" }
        ]
    };
    //===========================================================


    $scope.meassureName = '--';
    $scope.meassureUnitName = '--';

    $scope.dev = {
        name: '--',
        readOnly: true
    }

    $scope.vehicleTypes = [];

    $scope.loadDevices = function () {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        Device.getAll({ token: token, noCache: noCache }, function (data) {
            $scope.devices = data;
            $scope.$apply();

            $("#vehiclesList").kendoPanelBar({
                expandMode: "single"
            });

        }, function (err) {
            alert('We could not load your Devices. Please try again or contact Technical Support.');
        });
    }

    $scope.editVehicleInfo = function () {
        if (_.isObject($scope.dev)) {
            if ($scope.dev.readOnly == true) {
                $scope.dev.readOnly = false;
            }
        }
        else {
            alert('Please select a vehicle');
        }
    }

    $scope.saveVehicleInfo = function () {
        //POST the data here
        var token = getTokenCookie('ETTK');

        //Mark changed 
        if ($scope.dev.odometer != $scope.devBackup.odometer) {
            $scope.dev.odometerChanged = true;
        }
        if ($scope.dev.ignitionHours != $scope.devBackup.ignitionHours) {
            $scope.dev.ignitionHoursChanged = true;
        }
        if ($scope.dev.input1Hours != $scope.devBackup.input1Hours) {
            $scope.dev.input1HoursChanged = true;
        }
        if ($scope.dev.input2Hours != $scope.devBackup.input2Hours) {
            $scope.dev.input2HoursChanged = true;
        }
        if ($scope.dev.input3Hours != $scope.devBackup.input3Hours) {
            $scope.dev.input3HoursChanged = true;
        }
        if ($scope.dev.input4Hours != $scope.devBackup.input4Hours) {
            $scope.dev.input4HoursChanged = true;
        }

        var dev = new Device({ token: token });
        dev = $scope.dev;
        dev.$saveDevice({ token: token, deviceId: $scope.dev.id }, function (data) {
            $scope.dev.readOnly = true;
            alert('Information saved');
        }, function (err) {
            var b = 1;
        });

        //Go back to view mode...
        //$scope.dev.readOnly = true;
    }

    $scope.cancelVehicleInfo = function () {
        //$scope.devices[$scope.dev.ind] = angular.copy($scope.devBackup);
        //$scope.dev = angular.copy($scope.devBackup);
        $scope.dev.name = $scope.devBackup.name;
        $scope.dev.typeId = $scope.devBackup.typeId;
        $scope.dev.make = $scope.devBackup.make;
        $scope.dev.model = $scope.devBackup.model;
        $scope.dev.modelYear = $scope.devBackup.modelYear;

        $scope.dev.insuranceCarrier = $scope.devBackup.insuranceCarrier;
        $scope.dev.insurancePolicyNo = $scope.devBackup.insurancePolicyNo;
        $scope.dev.insurancePremium = $scope.devBackup.insurancePremium;
        $scope.dev.insuranceDueOn = $scope.devBackup.insuranceDueOn;

        $scope.dev.odometer = $scope.devBackup.odometer;
        $scope.dev.ignitionHours = $scope.devBackup.ignitionHours;

        $scope.dev.input1Name = $scope.devBackup.input1Name;
        $scope.dev.input1Hours = $scope.devBackup.input1Hours;
        $scope.dev.input2Name = $scope.devBackup.input2Name;
        $scope.dev.input2Hours = $scope.devBackup.input2Hours;
        $scope.dev.input3Name = $scope.devBackup.input3Name;
        $scope.dev.input3Hours = $scope.devBackup.input3Hours;
        $scope.dev.input4Name = $scope.devBackup.input4Name;
        $scope.dev.input4Hours = $scope.devBackup.input4Hours;

        $scope.dev.readOnly = true;
    }

    $scope.loadModule = function () {
        $scope.loadDevices();
    }

    $scope.pickDevice = function (ind, ev) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);

        if (_.isObject($scope.dev)) {
            if ($scope.dev.readOnly == false) {
                var b = confirm("There are unsaved changes.  Would you like an opportunity to save them?");
                if (b == true) {
                    ev.stopPropagation();
                    return;
                }
            }
        }
        //alert('Selected device ' + $scope.devices[ind].name);
        $scope.dev = $scope.devices[ind];
        $scope.dev.ind = ind;
        $scope.dev.readOnly = true;
        $scope.devBackup = angular.copy($scope.dev);
        $scope.$apply();

        //Load the schedules of this device
        MaintSchedule.getByDevice({ token: token, noCache: noCache, deviceId: $scope.dev.id }, function (data) {

            $.each(data.schedules, function (index, obj) {
                obj.nextDueInWords = $scope.calcNextDueInWords(obj);
                obj.isOverdue = $scope.calcIsOverdue(obj);
            });

            $scope.devSchedule = data.schedules;
            $scope.maintMeassures = data.meassures;

            try{
                $('#serviceScheduleTabGrid').data('kendoGrid').dataSource.data(data.schedules);
            }
            catch (err) {
                alert('Error: ' + err.message);
            }

        }, function (err) {
            alert('We could not load the Device Schedule. Please try again or contact Technical Support.');
        });

        //Load the log of this device (history of services done)
        MaintLog.getByDevice({ token: token, noCache: noCache, deviceId: $scope.dev.id }, function (data) {
            $scope.devLog = data.logs;

            try{
                $('#serviceLogTabGrid').data('kendoGrid').dataSource.data(data.logs);
            }
            catch (err) {
                alert('Error: ' + err.message);
            }

        }, function (err) {
            alert('We could not load the Device Log. Please try again or contact Technical Support.');
        });

        return true;
    }
    
    //SCHEDULE ITEM NEW/UPDATE DIALOG
    //===========================================================
    $scope.newServiceItemDlg = function () {
        if (!_.isObject($scope.dev) || !_.isString($scope.dev.id)) {
            alert('Please select a vehicle first');
            return;
        }

        var token = getTokenCookie('ETTK');
        $scope.maintItem = new MaintItem({token: token, deviceId: $scope.dev.id, id: 0});
        $scope.maintItem.isNew = true;
        $scope.maintItem.id = '0';
        $scope.maintItem.nextDue = getNow();
        $scope.maintItem.reminderTimeRefVal = 10;
        $scope.maintItem.reminderTimeRefId = '1';

        $('#dlgScheduleItem').dialog("open");
    }

    $scope.onChangeTasks = function () {
        $scope.thisTask = _.find($scope.maintTasks, function (i) {
            return i.id == $scope.maintItem.taskId;
        });
        $scope.taskId = $scope.thisTask.id;
        $scope.taskName = $scope.thisTask.name;
        $scope.maintItem.meassureId = $scope.thisTask.meassureId;
        $scope.maintItem.meassureValue = $scope.thisTask.value;
        $scope.onChangeMeassures();
    }

    $scope.onChangeTimeRef = function () {
        $scope.thisTimeRef = _.find($scope.timeReferences, function (i) {
            return i.id == $scope.maintItem.repeatEveryTimeRefId;
        });
        $scope.timeRefId = $scope.thisTimeRef.id;
    }

    $scope.onChangeMeassures = function () {
        $scope.thisMeassure = _.find($scope.maintMeassures, function (i) {
            return i.id == $scope.maintItem.meassureId;
        });
        $scope.meassureId = $scope.thisMeassure.id;
        $scope.meassureName = $scope.thisMeassure.name;
        $scope.meassureUnitName = $scope.thisMeassure.unitName;
        $scope.maintItem.reminderMeassureVal = parseInt($scope.maintItem.meassureValue / 10);
        switch ($scope.meassureId) {
            case 1: //miles
                $scope.currMeassureValue = $scope.dev.odometer;
                break;
            case 2: //engine hours
                $scope.currMeassureValue = $scope.dev.ignitionHours;
                break;
            case 3: //days
                $scope.currMeassureValue = 0;
                break;
            case 4: //input 1
                $scope.currMeassureValue = $scope.dev.input1Hours;
                break;
            case 5: //input 2
                $scope.currMeassureValue = $scope.dev.input2Hours;
                break;
            case 6: //input 3
                $scope.currMeassureValue = $scope.dev.input3Hours;
                break;
            case 7: //input 4
                $scope.currMeassureValue = $scope.dev.input4Hours;
                break;
        }

        $scope.maintItem.nextMeassureValue = $scope.currMeassureValue + $scope.maintItem.meassureValue;
        $scope.maintItem.reminderMeassureVal = parseInt($scope.maintItem.meassureValue / 10);

    }

    $scope.onChangeMeassureValue = function () {
        $scope.calcNextMeassureValue();
    }

    $scope.saveMaintItem = function () {
        $scope.saveScheduleItem();
        $scope.closeMaintItemDlg();
    }

    $scope.saveNewMaintItem = function () {
        $scope.saveScheduleItem();

        var token = getTokenCookie('ETTK');
        $scope.maintItem = new MaintItem({ token: token, deviceId: $scope.dev.id, id: 0 });
    }

    $scope.saveScheduleItem_DEPRECATED = function () {
        $scope.maintItem.taskName = $scope.taskName;
        $scope.maintItem.taskValue = $scope.maintItem.meassureValue;
        if (_.isUndefined($scope.maintItem.lastServiceOn)) {
            $scope.maintItem.lastServiceOn = '1/1/1900';
        }

        //The following values will be resolved at the database level (in the store procedure)...
        $scope.calcNextDueDate();
        $scope.maintItem.frequency = ''; //frequencyInWords
        $scope.maintItem.nextDueInWords = '';
        $scope.maintItem.reminder = ''; //reminderInWords
        $scope.calcNextMeassureValue();

        if ($scope.maintItem.isNew == true) {
            $scope.maintItem.id = '0';
        }

        var isNew = $scope.maintItem.isNew;
        var id = $scope.maintItem.id;

        var token = getTokenCookie('ETTK');
        $scope.maintItem.$saveItem({ token: token, deviceId: $scope.dev.id, id: $scope.maintItem.id }, function (data) {
            data.nextDueInWords = $scope.calcNextDueInWords(data);
            data.isOverdue = $scope.calcIsOverdue(data);
            if (isNew == true) {
                $scope.devSchedule.push(data);
            }
            else {
                var ind = -1;
                $.each($scope.devSchedule, function (index, obj) {
                    if (obj.id == id) {
                        ind = index;
                        return;
                    }
                });
                if (ind >= 0) {
                    $scope.devSchedule.splice(ind, 1, data);
                }
                else {
                    $scope.devSchedule.push(data);
                }
            }
            alert('Tasks has been scheduled');
        }, function (err) {
            var b = 1;
        });
    }

    $scope.saveScheduleItem = function () {
        $scope.maintItem.taskName = $scope.taskName;
        $scope.maintItem.taskValue = $scope.maintItem.meassureValue;

        //The following values will be resolved at the database level (in the store procedure)...
        $scope.maintItem.frequency = ''; //frequencyInWords
        $scope.maintItem.nextDueInWords = '';
        $scope.maintItem.reminder = ''; //reminderInWords

        //$scope.maintItem.valueSinceLastService = $scope.maintItem.meassureValue - $scope.maintItem.nextMeassureValue;

        //Next Meassure Value
        //$scope.calcNextMeassureValue();

        if ($scope.maintItem.isNew == true) {
            $scope.maintItem.id = '0';
        }

        var isNew = $scope.maintItem.isNew;
        var id = $scope.maintItem.id;

        var token = getTokenCookie('ETTK');
        $scope.maintItem.$saveItem({ token: token, deviceId: $scope.dev.id, id: $scope.maintItem.id }, function (data) {
            data.nextDueInWords = $scope.calcNextDueInWords(data);
            data.isOverdue = $scope.calcIsOverdue(data);
            if (isNew == true) {
                $scope.devSchedule.push(data);
            }
            else {
                var ind = -1;
                $.each($scope.devSchedule, function (index, obj) {
                    if (obj.id == id) {
                        ind = index;
                        return;
                    }
                });
                if (ind >= 0) {
                    $scope.devSchedule.splice(ind, 1, data);
                }
                else {
                    $scope.devSchedule.push(data);
                }
            }
            alert('Tasks has been scheduled');
        }, function (err) {
            var b = 1;
        });
    }

    $scope.closeMaintItemDlg = function () {
        $scope.maintItem = {};
        $('#dlgScheduleItem').dialog("close");
    }

    $scope.calcNextMeassureValue_DEPRECATED = function () {
        try{
            //Estimate next due
            switch ($scope.maintItem.meassureId) {
                case 1: //Miles
                    $scope.maintItem.nextMeassureValue = $scope.dev.odometer + ($scope.maintItem.meassureValue - ($scope.dev.odometer % $scope.maintItem.meassureValue));
                    break;
                case 2: //Engine Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.ignitionHours + ($scope.maintItem.meassureValue - ($scope.dev.ignitionHours % $scope.maintItem.meassureValue));
                    break;
                case 3: //Days
                    $scope.maintItem.nextMeassureValue = $scope.maintItem.meassureValue;
                    break;
                case 4:// Input 1 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input1Hours  + ($scope.maintItem.meassureValue - ($scope.dev.input1Hours % $scope.maintItem.meassureValue));
                    break;
                case 5:// Input 2 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input2Hours + ($scope.maintItem.meassureValue - ($scope.dev.input2Hours % $scope.maintItem.meassureValue));
                    break;
                case 6:// Input 3 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input3Hours + ($scope.maintItem.meassureValue - ($scope.dev.input3Hours % $scope.maintItem.meassureValue));
                    break;
                case 7:// Input 4 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input4Hours + ($scope.maintItem.meassureValue - ($scope.dev.input4Hours % $scope.maintItem.meassureValue));
                    break;
            }

            $scope.maintItem.reminderMeassureVal = parseInt($scope.maintItem.meassureValue / 10);
        }
        catch (err) {
            $scope.maintItem.nextMeassureValue = 0;
            $scope.maintItem.reminderMeassureVal = 0;
        }

    }

    $scope.calcNextMeassureValue = function () {
        try {
            //Estimate next due
            switch ($scope.maintItem.meassureId) {
                case 1: //Miles
                    $scope.maintItem.nextMeassureValue = $scope.dev.odometer + $scope.maintItem.nextMeassureValue;
                    break;
                case 2: //Engine Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.ignitionHours + $scope.maintItem.nextMeassureValue;
                    break;
                case 3: //Days
                    //$scope.maintItem.nextMeassureValue = $scope.maintItem.meassureValue;
                    break;
                case 4:// Input 1 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input1Hours + $scope.maintItem.nextMeassureValue;
                    break;
                case 5:// Input 2 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input2Hours + $scope.maintItem.nextMeassureValue;
                    break;
                case 6:// Input 3 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input3Hours + $scope.maintItem.nextMeassureValue;
                    break;
                case 7:// Input 4 Hours
                    $scope.maintItem.nextMeassureValue = $scope.dev.input4Hours + $scope.maintItem.nextMeassureValue;
                    break;
            }

        }
        catch (err) {
            $scope.maintItem.nextMeassureValue = 0;
            $scope.maintItem.reminderMeassureVal = 0;
        }

    }

    $scope.calcNextDueInWords_DEPRECATED = function (itm) {
        var nextDue = 'Due on ';
        var val = false;

        meassureName = '';
        if (itm.meassureId > 0) {
            var thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == itm.meassureId;
            });
            meassureName = thisMeassure.name;
        }

        switch (itm.meassureId) {
            case 1: //Miles
                val = itm.nextMeassureValue - $scope.dev.odometer;
                nextDue = nextDue + itm.nextDue + ' or ' + val + ' miles';
                break;
            case 2://Engine Hours
                val = itm.nextMeassureValue - $scope.dev.ignitionHours;
                nextDue = nextDue + itm.nextDue + ' or ' + val + ' engine hours';
                break;
            case 3://Days
                nextDue = nextDue + itm.nextDue + ' or ' + itm.daysUntilDue + ' days';
                break;
            case 4: //Input 1
                val = itm.nextMeassureValue - $scope.dev.input1Hours;
                nextDue = nextDue + itm.nextDue + ' or ' + val + ' ' + meassureName + ' hours';
                break;
            case 5:
                val = itm.nextMeassureValue - $scope.dev.input2Hours;
                nextDue = nextDue + itm.nextDue + ' or ' + val + ' ' + meassureName + ' hours';
                break;
            case 6:
                val = itm.nextMeassureValue - $scope.dev.input3Hours;
                nextDue = nextDue + itm.nextDue + ' or ' + val + ' ' + meassureName + ' hours';
                break;
            case 7:
                val = itm.nextMeassureValue - $scope.dev.input4Hours;
                nextDue = nextDue + itm.nextDue + ' or ' + val + ' ' + meassureName + ' hours';
                break;
        }

        return nextDue;
    }

    $scope.calcNextDueInWords_DEPRECATED2 = function (itm) {
        var nextDue = 'Next due in: ';
        var val = false;

        meassureName = '';
        if (itm.meassureId > 0) {
            var thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == itm.meassureId;
            });
            meassureName = thisMeassure.name;
        }

        switch (itm.meassureId) {
            case 1: //Miles
                val = Math.floor(itm.nextMeassureValue - $scope.dev.odometer);
                nextDue = nextDue + val + ' miles';
                break;
            case 2://Engine Hours
                val = Math.floor(itm.nextMeassureValue - $scope.dev.ignitionHours);
                nextDue = nextDue + val + ' engine hours';
                break;
            case 3://Days
                nextDue = nextDue + itm.daysUntilDue + ' days';
                break;
            case 4: //Input 1
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input1Hours);
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
            case 5: //Input 2
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input2Hours);
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
            case 6: //Input 3
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input3Hours);
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
            case 7: //Input 4
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input4Hours);
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
        }

        return nextDue;
    }

    $scope.calcNextDueInWords = function (itm) {
        var nextDue = 'Next due: ';
        var val = false;

        meassureName = '';
        if (itm.meassureId > 0) {
            var thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == itm.meassureId;
            });
            meassureName = thisMeassure.name;
        }

        val = itm.nextMeassureValue;

        switch (itm.meassureId) {
            case 1: //Miles
                nextDue = nextDue + val + ' miles';
                break;
            case 2://Engine Hours
                nextDue = nextDue + val + ' engine hours';
                break;
            case 3://Days
                nextDue = nextDue + itm.daysUntilDue + ' days';
                break;
            case 4: //Input 1
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
            case 5: //Input 2
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
            case 6: //Input 3
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
            case 7: //Input 4
                nextDue = nextDue + val + ' ' + meassureName + ' hours';
                break;
        }

        return nextDue;
    }

    $scope.calcNextDue_DEPRECATED = function (itm) {
        var val = false;

        if (itm.meassureId > 0) {
            var thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == itm.meassureId;
            });
        }

        switch (itm.meassureId) {
            case 1: //Miles
                val = Math.floor(itm.nextMeassureValue - $scope.dev.odometer);
                break;
            case 2://Engine Hours
                val = Math.floor(itm.nextMeassureValue - $scope.dev.ignitionHours);
                break;
            case 3://Days
                val = itm.daysUntilDue;
                break;
            case 4: //Input 1
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input1Hours);
                break;
            case 5: //Input 2
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input2Hours);
                break;
            case 6: //Input 3
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input3Hours);
                break;
            case 7: //Input 4
                val = Math.floor(itm.nextMeassureValue - $scope.dev.input4Hours);
                break;
        }

        return val;
    }

    $scope.calcNextDue = function (itm) {
        var val = false;

        if (itm.meassureId > 0) {
            var thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == itm.meassureId;
            });
        }

        if (itm.meassureId == 3) {
            val = itm.daysUntilDue;
        }
        else {
            val = itm.nextMeassureValue;
        }
        
        return val;
    }

    $scope.calcNextDueDate_DEPRECATED = function () {
        if (_.isNumber($scope.maintItem.repeatEveryX)) {
            var days = 0;
            if (_.isString($scope.maintItem.repeatEveryTimeRefId)) {
                switch ($scope.maintItem.repeatEveryTimeRefId) {
                    case '1':
                        days = $scope.maintItem.repeatEveryX;
                        break;
                    case '2':
                        days = $scope.maintItem.repeatEveryX * 7;
                        break;
                    case '3':
                        days = $scope.maintItem.repeatEveryX * 30;
                        break;
                    case '4':
                        days = $scope.maintItem.repeatEveryX * 365;
                        break;
                }

                if (_.isNumber(days)) {
                    $scope.maintItem.nextDue = new XDate($scope.maintItem.lastServiceOn).addDays(days).toString('MM/dd/yyyy');
                }
            }
        }
    }

    $scope.calcIsOverdue = function (itm) {
        var isOverdue = false;

        switch (itm.meassureId) {
            case 1:
                if (itm.nextMeassureValue < $scope.dev.odometer) {
                    isOverdue = true;
                }
                break;
            case 2:
                if (itm.nextMeassureValue < $scope.dev.ignitionHours) {
                    isOverdue = true;
                }
                break;
            case 3:
                //This was solved above... 
                break;
            case 4:
                if (itm.nextMeassureValue < $scope.dev.input1Hours) {
                    isOverdue = true;
                }
                break;
            case 5:
                if (itm.nextMeassureValue < $scope.dev.input2Hours) {
                    isOverdue = true;
                }
                break;
            case 6:
                if (itm.nextMeassureValue < $scope.dev.input3Hours) {
                    isOverdue = true;
                }
                break;
            case 7:
                if (itm.nextMeassureValue < $scope.dev.input4Hours) {
                    isOverdue = true;
                }
                break;
        }

        return isOverdue;
    }

    $scope.editMaintItem = function (ind) {
        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        MaintItem.getItem({ token: token, noCache: noCache, deviceId: $scope.dev.id, id: $scope.devSchedule[ind].id }, function (data) {
            $scope.maintItem = data;
            $scope.maintItem.isNew = false;

            $scope.thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == $scope.maintItem.meassureId;
            });
            $scope.meassureId = $scope.maintItem.meassureId;
            $scope.meassureName = $scope.thisMeassure.name;
            $scope.meassureUnitName = $scope.thisMeassure.unitName;
            //$scope.maintItem.nextMeassureValue = $scope.maintItem.meassureValue;
            //$scope.maintItem.reminderMeassureVal = parseInt($scope.maintItem.meassureValue / 10);
            switch ($scope.meassureId) {
                case 1: //miles
                    $scope.currMeassureValue = $scope.dev.odometer;
                    break;
                case 2: //engine hours
                    $scope.currMeassureValue = $scope.dev.ignitionHours;
                    break;
                case 3: //days
                    $scope.currMeassureValue = 0;
                    break;
                case 4: //input 1
                    $scope.currMeassureValue = $scope.dev.input1Hours;
                    break;
                case 5: //input 2
                    $scope.currMeassureValue = $scope.dev.input2Hours;
                    break;
                case 6: //input 3
                    $scope.currMeassureValue = $scope.dev.input3Hours;
                    break;
                case 7: //input 4
                    $scope.currMeassureValue = $scope.dev.input4Hours;
                    break;
            }


            $scope.maintItem.nextMeassureValue = $scope.calcNextDue(data);

            $('#dlgScheduleItem').dialog("open");
        }, function (err) {
            alert('Could not load Schedule Item. Please try again or contact Technical Support');
        });
    }

    $scope.editMaintItem_v2 = function (e) {
        var elem = e.dataItem;
        var id = elem.id;

        var token = getTokenCookie('ETTK');
        var noCache = Math.floor((Math.random() * 100000) + 1);
        MaintItem.getItem({ token: token, noCache: noCache, deviceId: $scope.dev.id, id: id }, function (data) {
            $scope.maintItem = data;
            $scope.maintItem.isNew = false;

            $scope.thisMeassure = _.find($scope.maintMeassures, function (i) {
                return i.id == $scope.maintItem.meassureId;
            });
            $scope.meassureId = $scope.maintItem.meassureId;
            $scope.meassureName = $scope.thisMeassure.name;
            $scope.meassureUnitName = $scope.thisMeassure.unitName;
            //$scope.maintItem.nextMeassureValue = $scope.maintItem.meassureValue;
            //$scope.maintItem.reminderMeassureVal = parseInt($scope.maintItem.meassureValue / 10);
            switch ($scope.meassureId) {
                case 1: //miles
                    $scope.currMeassureValue = $scope.dev.odometer;
                    break;
                case 2: //engine hours
                    $scope.currMeassureValue = $scope.dev.ignitionHours;
                    break;
                case 3: //days
                    $scope.currMeassureValue = 0;
                    break;
                case 4: //input 1
                    $scope.currMeassureValue = $scope.dev.input1Hours;
                    break;
                case 5: //input 2
                    $scope.currMeassureValue = $scope.dev.input2Hours;
                    break;
                case 6: //input 3
                    $scope.currMeassureValue = $scope.dev.input3Hours;
                    break;
                case 7: //input 4
                    $scope.currMeassureValue = $scope.dev.input4Hours;
                    break;
            }


            $scope.maintItem.nextMeassureValue = $scope.calcNextDue(data);

            $('#dlgScheduleItem').dialog("open");
        }, function (err) {
            alert('Could not load Schedule Item. Please try again or contact Technical Support');
        });
    }

    $scope.deleteMaintItem = function (id) {
        if (confirm('Are you sure you want to delete this task?')) {
            // Save it!
            var token = getTokenCookie('ETTK');
            MaintItem.delItem({ token: token, deviceId: $scope.dev.id, id: id }, function (data) {
                var ind = -1;
                $.each($scope.devSchedule, function (index, obj) {
                    if (obj.id == data.transId) {
                        ind = index;
                        return;
                    }
                });
                if (ind >= 0) {
                    $scope.devSchedule.splice(ind, 1);
                }
            }, function (err) {
                alert('Could not delete Schedule Item. Please try again or contact Technical Support');
            });
        } else {
            return;
        }
    }

    $scope.deleteMaintItem_v2 = function (e) {
        var elem = e.dataItem;
        var id = elem.id;

        if (confirm('Are you sure you want to delete this task?')) {
            // Save it!
            var token = getTokenCookie('ETTK');
            MaintItem.delItem({ token: token, deviceId: $scope.dev.id, id: id }, function (data) {
                var ind = -1;
                $.each($scope.devSchedule, function (index, obj) {
                    if (obj.id == data.transId) {
                        ind = index;
                        return;
                    }
                });
                if (ind >= 0) {
                    $scope.devSchedule.splice(ind, 1);
                }

                //Re-Load the schedules of this device
                var noCache = Math.floor((Math.random() * 100000) + 1);
                MaintSchedule.getByDevice({ token: token, noCache: noCache, deviceId: $scope.dev.id }, function (data) {

                    $.each(data.schedules, function (index, obj) {
                        obj.nextDueInWords = $scope.calcNextDueInWords(obj);
                        obj.isOverdue = $scope.calcIsOverdue(obj);
                    });

                    $scope.devSchedule = data.schedules;
                    $scope.maintMeassures = data.meassures;

                    try {
                        $('#serviceScheduleTabGrid').data('kendoGrid').dataSource.data(data.schedules);
                    }
                    catch (err) {
                        alert('Error: ' + err.message);
                    }

                }, function (err) {
                    alert('We could not load the Device Schedule. Please try again or contact Technical Support.');
                });


            }, function (err) {
                alert('Could not delete Schedule Item. Please try again or contact Technical Support');
            });
        } else {
            return;
        }
    }

    //===========================================================
    //COMPLETE MAINTENANCE DIALOG
    //===========================================================

    $scope.completeMaintItem = function (ind) {
        var token = getTokenCookie('ETTK');
        MaintItem.getItem({ token: token, deviceId: $scope.dev.id, id: $scope.devSchedule[ind].id }, function (data) {
            $scope.maintItem = data;
            $scope.maintItem.isNew = false;
            $scope.maintItem.completedOn = getNow();

            $scope.onChangeMeassures();

            $('#dlgCompleteMaintItem').dialog("open");
        }, function (err) {
            alert('Could not load Schedule Item. Please try again or contact Technical Support');
        });
    }

    $scope.saveCompleteMaintItem = function () {
        $scope.saveCompleteItem();
        $scope.closeCompleteMaintItemDlg();
    }

    $scope.saveNewCompleteMaintItem = function () {
        $scope.saveCompleteItem();
        $scope.newCompleteMaintItem();
    }

    $scope.newCompleteMaintItem = function () {
        var token = getTokenCookie('ETTK');
        $scope.maintItem = new MaintItem({ token: token, deviceId: $scope.dev.id, id: 0 });
        if (!$('#dlgCompleteMaintItem').dialog("isOpen")) {
            $('#dlgCompleteMaintItem').dialog("open");
        }
    }

    $scope.saveCompleteItem = function () {
        var token = getTokenCookie('ETTK');
        $scope.maintItem.odometer = $scope.dev.odometer;
        $scope.maintItem.ignitionHours = $scope.dev.ignitionHours;
        $scope.maintItem.input1Hours = $scope.dev.input1Hours;
        $scope.maintItem.input2Hours = $scope.dev.input2Hours;
        $scope.maintItem.input3Hours = $scope.dev.input3Hours;
        $scope.maintItem.input4Hours = $scope.dev.input4Hours;

        var ScheduleId = $scope.maintItem.id;

        $scope.maintItem.$saveCompletedItem({ token: token, deviceId: $scope.dev.id, id: 0 }, function (data) {
            data.nextDueInWords = $scope.calcNextDueInWords(data);
            if (_.isObject($scope.devSchedule)) {
                var ind = -1;
                $.each($scope.devSchedule, function (index, obj) {
                    if (obj.id == ScheduleId) {
                        ind = index;
                        return;
                    }
                });
                if (ind >= 0) {
                    $scope.devSchedule.splice(ind, 1, data);
                }
            }

            alert('Job has been saved');

        }, function (err) {
            var b = 1;
        });

    }

    $scope.closeCompleteMaintItemDlg = function () {
        $scope.maintItem = {};
        $('#dlgCompleteMaintItem').dialog("close");
    }

    //===========================================================


}]);

function onMaintTabSelect(e) {
    try {
        alert('Selected ' + $(e.item).find("> .k-link").text());
    }
    catch (err) {
        alert('onMaintTabSelect: ' + err.description);
    }
}