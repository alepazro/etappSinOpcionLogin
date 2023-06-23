// generic module that will change the focus of the cursor to the defined element after entering the specified number of characters.
// field - field that is being entered
// limit - number of characters to enter before transferring focus.
// next  - field to transfer focus to when the limit number of characters has been entered.
// evt   - pointer to the keyup event
function autofocus(field, limit, nextField, evt) {
    evt = (evt) ? evt : event;
    var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode :
                    ((evt.which) ? evt.which : 0));
    if (charCode > 31 && field.value.length == limit) {
        $('#' + nextField).focus();
    }
}

function click2call(sourceId) {
    try {
        //var phone = document.getElementById('txtAreaCode').value() + document.getElementById('txtPhone3').value() + document.getElementById('txtPhone4').value();
        var phone = $('#txtAreaCode').val() + $('#txtPhone3').val() + $('#txtPhone4').val();
        if (phone.length != 10) {
            alert("Sorry, the phone number you entered does not have 10 digits! ");
            return 0;
        }
        else {
            var data = 'sourceId=' + escape(sourceId) + '&phone=' + escape(phone);
            callWebMethod('ifByPhone_ClickToCall', data, true, true);
            $('#click2CallBtn').find('img').attr({ src: "../images/CallDoneBtn.png" });
            $('#click2CallBtn').attr("disabled", "disabled");
            alert('Your call is being processed.  Please wait a few seconds...');
        }
    }
    catch (err) {
        alert('click2call: ' + err.description);
    }
}

function callWebMethod(methodName, data, alertFailure, isAsync) {
    var ret = true;
    var jsonObj = false;

    if (isAsync == undefined) {
        isAsync = true;
    }

    if (alertFailure == undefined) {
        alertFailure = true;
    }

    try {
        if (methodName == undefined || methodName == null) {
            alert('Invalid method name');
            ret = false;
        }
        else {
            if (data == undefined || data == null) {
                data = '';
            }

            jQuery.ajax({
                url: 'ETWS.asmx/' + methodName,
                data: data,
                dataType: 'xml',
                type: "POST",
                success: function (xml, textStatus) {
                    if (textStatus == 'success') {
                        if (($("string", xml).text()) == 'failure') {
                            ret = false;
                        }
                        else {
                            jsonObj = eval('(' + $("string", xml).text() + ')');
                            if (jsonObj.result == 'failure') {
                                if (alertFailure == true) {
                                    alert(jsonObj.error);
                                }
                                ret = false;
                            }
                            else {
                                ret = true;
                            }
                        }
                    }
                    else {
                        ret = false;
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //alert(errorThrown);
                    ret = false;
                },
                async: isAsync
            });
        }

        return jsonObj;

    }
    catch (err) {
        alert('callWebMethod: Error calling ' + methodName);
        return false;
    }
}
