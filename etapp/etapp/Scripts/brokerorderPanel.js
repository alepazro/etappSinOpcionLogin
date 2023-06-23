var lastFetchOn = 0;
var qtyPanels = 1;
var devicesPerPanel = 0;
var Broker = {};
var BrokerStop = {};
var deviceid = 0;
var brokerID = 0;

function getBoardbrokers() {
    try {
        
        var token = getTokenCookie('ETTK');
        //var data = 'lastFetchOn=' + escape(lastFetchOn) + '&qtyPanels=' + qtyPanels + '&devicesPerPanel=' + devicesPerPanel;
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/devices?token=' + escape(token) + '&lastFetchOn=' + escape(lastFetchOn),
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: getBrokersDevices,
            error: getBrokersDevicesError,
            async: false
        });
    }
    catch (err) {
        console.log("error==> " + err);
        alert('getBoardDevices: ' + err.description);
    }
}
function getBrokersDevices(data, textStatus, jqXHR) {
    
    let colorstatus = "";
    let speedcolor = "";
    let Workinprogress = "";
    try {
        $("#tbody_brokerlist").empty();
        var row = "";
        if (data != null) {
            if (data.length == 1) {
                if (data[0].isOk == false) {
                    return;
                }
            }
           $.each(data, function (key, value) {
               
               colorstatus = eventColor(value.EventCode);
               speedcolor = speedingColor(value.Speed)
               row += "<tr>";
               row += "<td><button type='button' data-toggle='modal' data-target='#formbroker' onclick=savebroker()>New</button></td>";
               row += "<td><button type='button' data-toggle='modal' data-target='#formtrakingnput' onclick=viewBrokersDevices(" + value.ID +")>Orders</button></td>";
               row += "<td><button type='button' data-toggle='modal' data-target='#formtrakingnput' onclick=deletebroker(0," + value.ID + ")>Finish All</button></td>";
               row += "<td style=display:none>" + value.ID + "</td>";
               row += "<td style=display:none>" + value.DeviceID + "</td>";
               row += "<td style=display:none>" + value.DriverID + "</td>";
               row += "<td style=display:none>" + value.ShortName + "</td>";
               row += "<td style=display:none>" + value.LastUpdatedOn + "</td>";
               row += "<td style=display:none>" + value.GPSStatus + "</td>";
               row += "<td style=display:none>" + value.GPSAge + "</td>";
               row += "<td style=display:none>" + value.DriverPhone + "</td>";
               row += "<td style=display:none>" + value.TextColor + "</td>";
               row += "<td style=display:none>" + value.BgndColor + "</td>";
               row += "<td><img src=" + value.IconURL + " alt='image'></td>";
               row += "<td style=display:none>" + value.IconID + "</td>";
               row += "<td>" + value.Name + "</td>";
               row += "<td style=background-color:" + colorstatus + ">" + value.EventName + "</td>";
               row += "<td>" + value.EventDate + "</td>";
               row += "<td>" + value.FullAddress + "</td>";
               row += "<td>" + value.DriverName + "</td>";
               row += "<td  style=background-color:" + speedcolor + ">" + value.Speed + "</td>";
               row += "<td>" + value.CountBrokers + "</td>";  
               row += "</tr>";





              //row += "<td onclick=showLocationInMap('mapCanvas'," + device.latitude + "," + device.longitude+");>"+ value.address + "</td>";
                

            });
            $("#tbody_brokerlist").append(row)
        }
    }
    catch (err) {
        alert('getBoardDevicesOk: ' + err.description);
    }
}
function setLastFetchOn() {
    try {
        if (lastFetchOn == 0) {
            lastFetchOn = '1/1/2000';
        }
        else {
            var now = new Date();
            var now_utc = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
            lastFetchOn = now_utc.getFullYear().toString() + '-' + pad((now_utc.getMonth() + 1).toString(), 2) + '-' + pad(now_utc.getDate().toString(), 2) + ' ' + now_utc.getHours().toString() + ':' + pad(now_utc.getMinutes().toString(), 2) + ':' + pad(now_utc.getSeconds().toString(), 2);
        }
    }
    catch (err) {
        alert('setLastFetchOn: ' + err.description);
    }
}
function getBrokersDevicesError(jqXHR, textStatus, errorThrown) {
    try {
        var a = 1;
    }
    catch (err) {
        alert('getBoardDevicesError: ' + err.description);
    }
}
function loadbrokerOrder(){
    getBoardbrokers();
}
function savebroker() {
    //clearinputs();
    try {      
        Broker = {};
        var bool = true;
        //e.preventDefault();
        //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //window.open('crmViewCustomer.html?' + 'uid=' + dataItem.uniqueKey, target = "_blank");
        $("#brokerlist tr").click(function () {
            
            if (bool) {
                
                var tr = $(this)[0];
                //var brokerdeviceguid = tr.cells[0].innerText;
                Broker.DeviceID = tr.cells[3].innerText;
                Broker.DriverID = tr.cells[5].innerText;
                Broker.DriverID = tr.cells[5].innerText;
                $("#brokerdevice").text(tr.cells[15].innerText);
                $("#brokerdriver").text(tr.cells[19].innerText == "" ? "Unassigned" : tr.cells[18].innerText);
            }
            bool = false

        });
    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}
function savebroker2() {
    try {      
        let procesado
        //var loadt = $('select[name=type] option').filter(':selected').val();
        
        if (validadteForm()) {        
            Broker.Name = $("#name").val();
            Broker.BrokerNumber = $("#Ordernumber").val();
            Broker.PickupAddress == $("#PickupAddress").val();
            Broker.Pickupdetetime = $("#PickupDatetime").val();
            Broker.DeliveryAddress = $("#address").val();
            Broker.Deliverydatetime = $("#DeliveryDatetime").val();
            Broker.Observaciones = $("#Observations").val();
            Broker.HasEmail = $('#chksendtraking').prop('checked') == true ? true : false;
            Broker.TrackingWasSent = Broker.HasEmail
            Broker.EmailTo = $("#emailto").val();
            Broker.StatusID = 20;
            Broker.HasSms = $('#chksendsms').prop('checked') == true ? true : false;
            Broker.SmsPhone = $("#smsPhone1").val();
            //Job.DeviceName = $("#brokerdevice").innerText;
            //Job.Flat_FromJob = false;
            //Job.Flat_FromBrokerOrder = true;
            Broker.BrokerNumber = Broker.BrokerNumber.replace(/\s+/g, '')  // > "Textodeejemplo"

            
            if (postBroker("new", JSON.stringify(Broker))) {
                clearform();
                toastr.success('job created successfully');
                loadbrokerOrder();
            } else {
                toastr.error('ERROR create job');
            }
        }
       /*
        *Note: This information is retrieved from the search events of the creation form.
        Job.PickupAddresscoordinatesLat = "";
        Job.PickupAddresscoordinatesLng = "";
        Job.DeliveryAddressscoordinatesLat = "";
        Job.DeliveryAddressscoordinatesLng = "";
        */
    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function clearform() {
    $("#name").val('');
    $("#Ordernumber").val('');
    $("#PickupAddress").val('');
    $("#PickupDatetime").val('');
    $("#address").val('');
    $("#DeliveryDatetime").val('');
    $("#Observations").val('');
    

}
function clearformStop() {
    $("#stopaddress").val('');
    $("#StopDatetime").val('');
    $("#StopObservations").val('');
    


}
function validadteForm() {
    if ($("#name").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Broker Name field is empty';
        return false;
    } else if ($("#Ordernumber").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Order Number field is empty';
        return false;
    } else if ($("#PickupAddress").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Pick up Address field is empty';
        return false;
    } else if ($("#PickupDatetime").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Pick up Datetime field is empty';
        return false;
    } else if ($("#address").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Delivery Address field is empty';
        return false;
    } else if ($("#DeliveryDatetime").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Delivery Datetime field is empty';
        return false;
    } 
    else if ($("#DeliveryDatetime").val().length == 0) {
        document.getElementById('msg').innerHTML = 'Delivery Datetime field is empty';
        return false;
    } 
    else {
        if ($('#chksendtraking').prop('checked')) {
            if ($("#emailto").val().length == 0) {
                document.getElementById('msg').innerHTML = 'Email field is empty';
                return false;

            }
            //else if ($("#emailComments").val().length == 0) {
            //    document.getElementById('msg').innerHTML = 'Comments email field is empty';
            //    return false;
            //}
        }
        return true;
    }
}
function viewtraking() {
    if ($('#chksendtraking').prop('checked')) {        
        $("#emailstraking").fadeIn(1000).show();       
        
    } else {
        $('#emailstraking').fadeOut(1000).hide();    
    }
    
}
function viewSms() {
    if ($('#chksendsms').prop('checked')) {
        $("#smsphone").fadeIn(1000).show();
    } else {
        $('#smsphone').fadeOut(1000).hide();
    }

}
function postBroker(method, data) {

    try {
        

        var result = false;
        var token = getTokenCookie('ETTK');
        var url = urlServicio + '/Brokers.svc/' + method + '?token=' + token;
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            data: data,
            dataType: "json",
            processdata: false,
            success: function (data) {
                
                result = data;
            },
            error: function (err) {
                
                console.log(err)
                var a = 1;
            },
            async: false
        });

        return result;

    }
    catch (err) {
        console.log("Error----> " + err);
    }
}
function loadbrokerOrders(brokerid,deviceid) {
    getbrokerOders(brokerid, deviceid);
}
function getbrokerOders(brokerid, deviceid) {
    try {

        var token = getTokenCookie('ETTK');
        //var data = 'lastFetchOn=' + escape(lastFetchOn) + '&qtyPanels=' + qtyPanels + '&devicesPerPanel=' + devicesPerPanel;
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/brokers?token=' + escape(token) + '&BrokerID=' + escape(brokerid) + '&DeviceID=' + escape(deviceid),
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: getBrokersOrdersList,
            error: getBrokersDevicesError,
            async: false
        });
    }
    catch (err) {
        console.log("error==> " + err);
        alert('getBoardDevices: ' + err.description);
    }
}
function getBrokersOrdersList(data, textStatus, jqXHR) {
    
    let colorstatus = "";
    let speedcolor = "";
    let Workinprogress = "";
    try {
        $("#tbody_brokerlist").empty();
        var row = "";
        if (data != null) {
            if (data.length == 1) {
                if (data[0].isOk == false) {
                    return;
                }
            }
            
            if (data.length>0 && deviceid > 0) {
                $("#h1_brokerorder").text('');
                $("#h1_brokerorder").text(data[0].DeviceName);
            } else {
                $("#h1_brokerorder").text('Broker Orders');
            }
            $.each(data, function (key, value) {

                

                row += "<tr>";
                //row += "<td><button type='button' data-toggle='modal' data-target='#formtrakingnput' onclick=alert('Finish')>Detail</button></td>";
                row += "<td><button type='button' onclick=detailsBroker(" + value.ID+")>Detail</button></td>";
                row += "<td><button type='button' onclick=deletebroker(" + value.ID + ",0)>Finish Order</button></td>";
                row += "<td><button type='button' data-toggle='modal' data-target='#formbrokerStop' onclick=saveStop(" + value.ID +")>Add Stop</button></td>";
                //row += "<td><button type='button' onclick=alert('Finish')>Add Stop</button></td>";
                row += "<td><button type='button' data-toggle='modal' data-target='#sendEmails' onclick=sendEmails(" + value.ID+")>Send Email</button></td>";
                row += "<td><button type='button' data-toggle='modal' data-target='#sendsms' onclick=sendSms(" + value.ID + ")>Send SMS</button></td>";
                row += "<td style=display:none>" + value.ID + "</td>";
                //row += "<td>" + value.DeviceName + "</td>";
                row += "<td>" + value.Name + "</td>";
                row += "<td>" + value.BrokerNumber + "</td>";
                row += "<td>" + value.PickupAddress + "</td>";
                row += "<td>" + value.Pickupdetetime + "</td>";
                row += "<td>" + value.DeliveryAddress + "</td>";
                row += "<td>" + value.Deliverydatetime + "</td>";
                //row += "<td>" + value.Observaciones + "</td>";
                //row += "<td>" + value.CreateOn + "</td>";
                //row += "<td>" + value.TrackingWasSent + "</td>";
                //row += "<td>" + value.TrackingWasSentDate + "</td>";
                //row += "<td>" + value.SendTo + "</td>";
                //row += "<td>" + value.DriverName + "</td>";
                //row += "<td>" + value.TrackingNumber + "</td>";
                //row += "<td>" + value.TrackingStatus + "</td>";
                row += "<td>" + value.CountStops + "</td>";
                row += "</tr>";
            });
            $("#tbody_brokerlist").append(row);
        } 
    }
    catch (err) {
        
        alert('getBrokersOrders: ' + err.description);
    }
}
function getdeletebrokerOders(brokerid, deviceid) {
    try {

        var token = getTokenCookie('ETTK');
        //var data = 'lastFetchOn=' + escape(lastFetchOn) + '&qtyPanels=' + qtyPanels + '&devicesPerPanel=' + devicesPerPanel;
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/delete?token=' + escape(token) + '&BrokerID=' + escape(brokerid) + '&DeviceID=' + escape(deviceid),
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: false,
            success: function (data) {
                
                result = data;
            },
            error: function (err) {
                
                console.log(err)
            },
            async: false
        });
        return result;
    }
    catch (err) {
        console.log("error==> " + err);
        alert('getBoardDevices: ' + err.description);
    }
}
function deletebroker(brokerid, deviceid) {
    var alert = "";
    if (brokerid > 0) {
        alert = "Are you sure to delete this order?!";
    }
    if (deviceid) {
        alert = "All orders for this unit will be deleted.You won't be able to revert this!";
    }

    Swal.fire({
        title: 'Are you sure?',
        text: alert,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        
        if (result.isConfirmed) {
            
            var result = getdeletebrokerOders(brokerid, deviceid);
            if (result.isOk) {
                Swal.fire(
                    'Deleted!',
                    'Your Broker Orders has been deleted.',
                    'success'
                )
                if (brokerid > 0) {
                    const queryString = window.location.search;
                    const urlParams = new URLSearchParams(queryString);
                    deviceid = urlParams.get('Deviceid')
                    if (deviceid != undefined && deviceid > 0) {
                        loadbrokerOrders(0, deviceid);

                    } else {
                        loadbrokerOrders(0, 0);
                    }
                } else {
                    if (deviceid > 0) {
                        getBoardbrokers();
                    }
                }
                
            } else {
                toastr.error('an error occurred');
            }
           
        }
    })
    

}
function viewBrokersDevices(deviceID) {
    
    window.open('http://localhost:55328/brokerorders.html?Deviceid=' + deviceID +'','_blank');
}
function detailsBroker(id) {
    
    var result = getbrokerOdersDetail(id, 0);
    var resultStops = getStops(id);
    showStops(resultStops);
    var aux1 = result[0].TrackingWasSent == false ? "Not" : "Yes";
    var aux2 = result[0].TrackingWasSent == false ? "NA" : result[0].SendTo;
    var aux3 = "";
    if (result[0].TrackingWasSent == false) {
        aux3 = "NA";
        

    } else {
        if (result[0].TrackingStatus == false) {
            aux3 ="Active"
        }else{
            aux3 = "Idle"
        }
    }

    if (result.length > 0) {
        
        $("#det_brokerHead").text(result[0].BrokerNumber);
        $("#det_Name").val(result[0].Name);
        $("#det_BrokerNumber").val(result[0].BrokerNumber);
        $("#det_PickupAddress").val(result[0].PickupAddress);
        $("#det_Pickupdetetime").val(result[0].Pickupdetetime);
        $("#det_DeliveryAddress").val(result[0].DeliveryAddress);
        $("#det_Deliverydatetime").val(result[0].Deliverydatetime);
        $("#det_CreateOn").val(result[0].CreateOn);
        $("#det_SendTraking").val(aux1);
        $("#det_SendTo").val(aux2);
        $("#det_DriverName").val(result[0].DriverName);
        $("#det_TrackingStatus").val(aux3);
        $("#det_ViewMap").val(result[0].ViewMap);
        $("#det_Observaciones").val(result[0].Observaciones);
        
        $("#det_ViewMap").attr("href","http://localhost:55328/" +result[0].UrlTraking);
    }
}
function getbrokerOdersDetail(brokerid, deviceid) {
    try {
        
        var result=null;
        var token = getTokenCookie('ETTK');
        //var data = 'lastFetchOn=' + escape(lastFetchOn) + '&qtyPanels=' + qtyPanels + '&devicesPerPanel=' + devicesPerPanel;
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/brokers?token=' + escape(token) + '&BrokerID=' + escape(brokerid) + '&DeviceID=' + escape(deviceid),
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: function (data) {
                
                result= data;
            },
            error: getBrokersDevicesError,
            async: false
        });
        return result;
    }
    catch (err) {
        console.log("error==> " + err);
        alert('getBoardDevices: ' + err.description);
    }
}
function getStops(brokerid) {
    try {

        var result;
        var token = getTokenCookie('ETTK');
        //var data = 'lastFetchOn=' + escape(lastFetchOn) + '&qtyPanels=' + qtyPanels + '&devicesPerPanel=' + devicesPerPanel;
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/stops?token=' + escape(token) + '&BrokerID=' + escape(brokerid),
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: function (data) {

                result = data;
            },
            error: getBrokersDevicesError,
            async: false
        });
        return result;
    }
    catch (err) {
        console.log("error==> " + err);
        alert('getBoardDevices: ' + err.description);
    }
}

function showStops(stops) {
    var row = "";

    $("#tbody_brokerStops").empty();
    $.each(stops, function (key, value) {
        row += "<tr>";
        row += "<td><button type='button' data-toggle='modal' data-target='#formbroker' onclick=deletestop(" + value.ID + "," + value.BrokerOrderID + "," + value.DeviceID +")>X</button></td>";
        row += "<td style=display:none>" + value.ID + "</td>";
        row += "<td style=display:none>" + value.BrokerOrderID + "</td>";
        row += "<td style=display:none>" + value.DeviceID + "</td>";
        row += "<td>" + value.PickupAddress + "</td>";
        row += "<td>" + value.Pickupdetetime + "</td>";
        row += "<td style=display:none>" + value.PickupAddresscoordinatesLat + "</td>";
        row += "<td style=display:none>" + value.PickupAddresscoordinatesLng + "</td>";
        row += "<td>" + value.Observations + "</td>";
        row += "<td style=display:none>" + value.CreateOn + "</td>";
        row += "<td style=display:none>" + value.StatusID + "</td>";        
        row += "</tr>";
    });
    $("#tbody_brokerStops").append(row)



}
function sendEmails(brokerid){
    brokerID = 0;
    brokerID = brokerid;
}
function sendSms(brokerid) {
    brokerID = 0;
    brokerID = brokerid;
}
function postemail() {
    
    var result;
    var token = getTokenCookie('ETTK');
    var email = $("#emails").val();
    var textarea = $("#message-text").val();
    if (email.length == 0) {
        toastr.error('Email field cannot be empty');
        return;
    } 
    if (textarea.length == 0) {
        toastr.error('Observations field cannot be empty');
        return;
    }
    try {        
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/sendemail?token=' + token + '&brokerID=' + brokerID + '&emails=' + email + '&resend='+false+'&observations=' + textarea,
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: function (data) {
                
                if (data.isOk) {
                    toastr.success('Email sent successfully');
                } else {
                    toastr.error('An error occurred in the process');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
                toastr.error('An error occurred in the process');
            },
            async: false
        });
        return result;
    }
    catch (err) {
        
        console.log(err);
    }
}
function saveStop(id){ 
    

    
    $("#stopaddress").val('');
    $("#StopDatetime").val('');
    $("#StopObservations").val('');
    var bool = true;
    BrokerStop = {};
    BrokerStop.BrokerOrderID = id;
    $("#tbody_brokerlist tr").click(function () {
        
        if (bool) {
            
            var tr = $(this)[0];
            BrokerStop.DateFromOrden = tr.cells[8].innerText;
            BrokerStop.DateToOrden = tr.cells[10].innerText;            
        }
        bool = false

    });
    console.log(BrokerStop);
    
}
function stopAdd() {
    

    BrokerStop.Pickupdetetime = $("#StopDatetime").val();
    BrokerStop.Observations = $("#StopObservations").val();
    var fecha1 = new Date(BrokerStop.Date);
    var fecha2 = new Date(BrokerStop.DateFromOrden);
    var fecha3 = new Date(BrokerStop.DateToOrden);

    if (fecha1 < fecha2) {
        
        toastr.error('la fecha de la parada debe ser mayor o igual a la fecha de inicio de la orden.');
        return;
    }
    if (fecha1 > fecha3) {
        
        toastr.error('la fecha de la parada debe ser menor o igual a la fecha de finalizacion de la orden.');
        return;
    }
    if (postBroker("newstop", JSON.stringify(BrokerStop))) {
        clearformStop();
        BrokerStop = {};
        
        toastr.success('Stop created successfully');
        loadbrokerOrders(0, deviceid);
        getStops(BrokerStop.BrokerOrderID)
    } else {
        toastr.error('ERROR create Stop');
    }
    console.log(BrokerStop);
}

function deletestop(id,brokerid,deviceid) {
    
    var result;
    var token = getTokenCookie('ETTK');
    try {
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/deletestop?token='+token+'&StopID='+id,
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: function (data) {
                
                if (data.isOk) {
                    
                    toastr.success('Stop removed successfully');
                    loadbrokerOrders(0, deviceid);
                    showStops(getStops(brokerid));
                } else {
                    toastr.error('An error occurred in the process');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
                toastr.error('An error occurred in the process');
            },
            async: false
        });
        return result;
    }
    catch (err) {
        
        console.log(err);
    }
}
function PostsendSms() {
    //brokersms?token={token}&PBrokerID={PBrokerID}&Pobservations={Pobservations}&PPhoneNumber={PPhoneNumber}
    
    var result;
    var token = getTokenCookie('ETTK');
    var phone = $("#smsPhone").val();    
    var observations = $("#smsMessage").val();

    if (phone.length < 10 || phone == "") {
        toastr.error('Enter Phone');
        return;
    }
    try {
        $.ajax({
            type: "GET",
            url: 'https://localhost:44385/Brokers.svc/brokersms?token=' + token + '&PBrokerID=' + brokerID + '&Pobservations=' + observations + '&PPhoneNumber=' + phone,
            contentType: "application/json; charset=utf-8",
            data: 0,
            dataType: "json",
            processdata: true,
            success: function (data) {
                
                if (data.isOk) {
                    toastr.success('SMS sent successfully');
                    $("#smsPhone").val('');
                    $("#smsMessage").val('');
                } else {
                    toastr.error('An error occurred in the process');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                
                console.log(jqXHR)
                toastr.error('An error occurred in the process');
            },
            async: false
        });
        return result;
    }
    catch (err) {
        
        console.log(err);
    }
}

