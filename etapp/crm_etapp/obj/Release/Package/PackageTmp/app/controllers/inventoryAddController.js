etApp.controller('inventoryAddController', ['$scope', '$location', 'AddInventory', function ($scope, $location, AddInventory) {

    $scope.qtyProcessed = 0;
    $scope.rawData = [];
    $scope.device = {};
    $scope.deviceTypeId = 0;

    $scope.loadInventoryRawData = function () {
        $scope.rawData = tsvToJson($('#addInventoryRawData').val(), true);

        //Validate uploaded data
        var missingFields = '';
        //Validate the structure of the data:
        if (_.isUndefined($scope.rawData[0]["SerialNumber"])) {
            missingFields = 'SerialNumber';
        }
        if (_.isUndefined($scope.rawData[0]["IMEI"])) {
            missingFields = 'IMEI';
        }
        if (_.isUndefined($scope.rawData[0]["CARRIER/SIMTYPE/MVNO"])) {
            missingFields = 'CARRIER/SIMTYPE/MVNO';
        }
        if (_.isUndefined($scope.rawData[0]["SIM/ESN/MEID"])) {
            missingFields = 'SIM/ESN/MEID';
        }
        if (_.isUndefined($scope.rawData[0]["MSISDN/MDN"])) {
            missingFields = 'MSISDN/MDN';
        }
        if (missingFields.length > 0) {
            alert('The following fields are missing: ' + missingFields + '. Please fix the file and paste it again.')
        }
        else {
            json2Table($scope.rawData, 'dataNewInventoryTable');
            $('#addInventoryRawDataDiv').hide();
            $('#dataAddInventoryTableDiv').show();
        }

    }

    $scope.clearInventoryRawData = function () {
        $('#addInventoryRawDataDiv').show();
        $('#addInventoryRawData').val('');

        $('#dataAddInventoryTableDiv').hide();
        $('#dataNewInventoryTable').html('');

        $scope.rawData = [];

    }

    $scope.processUpload = function () {

        if ($scope.deviceTypeId == 0) {
            alert('Please select a device type');
            return;
        }

        $scope.serialNumber = false;
        $scope.imei = false;
        $scope.simNo = false;
        $scope.simPhone = false;
        $scope.carrier = false;

        $scope.qtyProcessed = 0;
        var refreshIntervalId = setInterval(function () {

            var isOk = true;

            if ($scope.qtyProcessed < $scope.rawData.length) {
                var line = $scope.rawData[$scope.qtyProcessed];

                if (!_.isUndefined(line["SerialNumber"])) {
                    if (!_.isNaN(line["SerialNumber"])) {

                        var token = getCookie('ETCRMTK');
                        var newInventory = new AddInventory({ token: getCookie('ETCRMTK') });
                        newInventory.deviceTypeId = $scope.deviceTypeId;
                        newInventory.serialNumber = line["SerialNumber"];
                        newInventory.carrier = line["CARRIER/SIMTYPE/MVNO"];
                        newInventory.imei = line["IMEI"];
                        newInventory.simNo = line["SIM/ESN/MEID"];
                        newInventory.simPhone = line["MSISDN/MDN"];

                        if (newInventory.simPhone.toString().length != 10) {
                            isOk = false;
                            $('#uploadedFileRow' + $scope.qtyProcessed).find('.uploadedFileRowComment').html('Invalid MSISDN/MDN (Phone Number).  Must be 10 digits only. No other characters.');
                        }

                        if (isOk == true) {
                            newInventory.$saveNewInventory({ token: token }, function (data) {
                                if (data.isOk == true) {
                                    $('#uploadedFileRow' + $scope.qtyProcessed).find('.uploadedFileRowComment').html('OK');
                                }
                                else {
                                    $('#uploadedFileRow' + $scope.qtyProcessed).find('.uploadedFileRowComment').html(data.msg);
                                }
                            });
                        }
                    }
                }
            }
            else {
                clearInterval(refreshIntervalId);
                alert('Information Processed...');
            }
            $scope.qtyProcessed++;
        }, 100);
    }

}]);