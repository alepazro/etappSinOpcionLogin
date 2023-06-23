

function sendQuickMessage(deviceId, deviceName, driverId) {
    try {
        
        var dlgScope = angular.element($("#dlgQuickMsg")).scope();

        $("#dlgQuickMsg").dialog({
            autoOpen: false,
            resizable: false,
            height: 320,
            width: 500,
            modal: true,
            buttons: {
                "Send Text": function () {
                    dlgScope.quickMsgSend(1);
                },
                "Send Email": function () {
                    dlgScope.quickMsgSend(2);
                },
                "Close": function () {
                    $(this).dialog("close");
                }
            }
        });

        dlgScope.openDialog(deviceId, deviceName, driverId);
    }
    catch (err) {
        alert('sendQuickMessage:' + err.description);
    }
}