function setOutputs(deviceId) {
    try {
        
        if (validateUserAccess(44) == false) {
            alert('You are not authorized to use Telemetry.  Telemetry allows you to Turn ON or OFF things in your vehicle, or to ENABLE or DISABLE the vehicle ignition.  Please contact your System Administrator');
            return;
        }
        $("#dlgTelemetry").dialog({
            autoOpen: false,
            resizable: false,
            height:440,
            width:600,
            modal: true,
            buttons: {
                "Close": function () {
                    $(this).dialog("close");
                }
            }
        });

        var dlgScope = angular.element($("#dlgTelemetry")).scope();
        dlgScope.openDialog(deviceId);
    }
    catch (err) {
        alert('setOutputs:' + err.description);
    }
}
