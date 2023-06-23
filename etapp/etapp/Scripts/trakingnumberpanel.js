var IDtrakingNumber = 0;


function getDevicescmb() {
    try {
        getBasicList('devices', getDevicesList);
    }
    catch (err) {
    }
}
function getDevicesList(data) {
    
    try {
        var jsonDevices = data;
        $.each(jsonDevices, function (i, value) {
            $('#Device').append($('<option>').text(value.value).attr('value', value.id));
        });
    }
    catch (err) {
    }
}
function savepost() {
    
    var obj = {};
    var device = $('select[name=Device] option').filter(':selected').val();
    var deviceName = $('select[name=Device] option').filter(':selected').text();
    var email = $("#SendTo").val();
    var validateuntil = $("#ValidUntil").val();
    var obervations = $("#Message").val();

    obj.Device = device;
    obj.SendTo = email;
    obj.ValidUntil = validateuntil;
    obj.Message = obervations;
    obj.DeviceName = deviceName;

    var result = postTrakingnumber(obj);

   
    return result;
}
function edittrakingn() {
    clearinputs();
    try {
        

        var bool = true;
        //e.preventDefault();
        //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //window.open('crmViewCustomer.html?' + 'uid=' + dataItem.uniqueKey, target = "_blank");
        $("#trakingnumber tr").click(function () {            
            if (bool) {
                var tr = $(this)[0];
                IDtrakingNumber = tr.cells[2].innerText;
                var Device = tr.cells[3].innerText;
                var TrackingNumber = tr.cells[4].innerText;
                var SendTo = tr.cells[5].innerText;
                var ValidUntil = tr.cells[6].innerText;
                var Message = tr.cells[7].innerText;
                //var Lat = tr.cells[8].innerText;
                //var Lng = tr.cells[9].innerText;
                var expired = 0;
                if (tr.cells[9].innerText == "Active") {
                    expired = 0
                } else {
                    expired = 1;
                };                
                
                $("#spanValidUntil").text(ValidUntil);
                $("#SendTop").val(SendTo);
                $("#Messagep").val(Message);
                $("#status").val(expired);
                //window.open('crmViewCustomer.html?' + 'uid=' + trID, target = "_blank");                
            }
            bool = false

        });
    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}
function saveput() {

    
    var obj = {};
    var email = $("#SendTop").val();
    var validateuntil = $("#ValidUntilp").val(); 
    var obervations = $("#Messagep").val();
    var expired = $('select[name=status] option').filter(':selected').val();
    
    if (expired == 1) {
        validateuntil = new Date();
    }
    if (validateuntil == "") {
        validateuntil = document.getElementById("spanValidUntil").innerText;
    }
    if (validaCamposput(email, validateuntil, obervations))
    {
        
        obj.ID = IDtrakingNumber;
        obj.SendTo = email;
        obj.ValidUntil = validateuntil;
        obj.Message = obervations;
        obj.Flag_Expired = expired;


        var result = putTrakingnumber(obj);


        return result;
    }
   
    
}
function validaCampospost() {

    var nombre = $("#nombre").val();
    var edad = $("#edad").val();
    var direccion = $("#direccion").val();
    //validamos campos
    if ($.trim(nombre) == "") {
        toastr.error("No ha ingresado Nombre", "Aviso!");
        return false;
    }
    if ($.trim(edad) == "") {
        toastr.error("No ha ingresado Edad", "Aviso!");
        return false;
    }

    if (edad < 0) {
        toastr.error("Mínimo permitido 0", "Aviso!");
        return false;
    }
    if ($.trim(direccion) == "") {
        toastr.error("No ha ingresado Dirección", "Aviso!");
        return false;
    }

}
function validaCamposput(SendTop, ValidUntilp, Messagep) { 
    //validamos campos
    if ($.trim(SendTop) == "") {
        toastr.error("required email!");
        return false;
    }
    if ($.trim(ValidUntilp) == "") {
        toastr.error("requires expiration date!");
        return false;
    }   
    if ($.trim(Messagep) == "") {
        toastr.error("requires observation!");
        return false;
    }
    return true;

}
function clearinputs() {
    $("#SendTo").text('');
    $("#ValidUntil").val('');
    $("#Message").val('');

    $("#ValidUntilp").text('');
    $("#SendTop").val('');
    $("#Messagep").val('');
}
function getTrakingnumber() {
    return getTrakingnumberDb();
}