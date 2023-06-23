/*****************************************************
* Librerias requeridas
*****************************************************/
var ucaddress = null;
require(['maps'], function (maps) {
    try{
        console.log("address_view, create ucmapsfinder instance");

        ucaddress = usercontrol_address();
        ucaddress.initialize();
    }
    catch (exc) {
        app.excManager("address_view", "require", exc);
    }
},
function (err) {
    console.log("Falla en required de MAPAS");
    console.log('modules: ' + err.requireModules);
});

/*****************************************************
* Constructor Principal
*****************************************************/
function usercontrol_address() {
    var uc;
    try {
        uc = {
            ucmaps: {},
            result: null
        };

        uc.resize = function () { };

        /*-----------------------------------------------
        - Inicializacion del Objeto
        ------------------------------------------------*/
        uc.initialize = function () {
            try {
                app.modules.registerModule(new app.ModuleModel({ container: '#addressmap', key: 'address', mod: uc }));

                uc.ucmaps = usercontrol_maps();
                uc.ucmaps.initialize('mapfinder');

                $('#lbxAddress').change(function () {
                    try{
                        var selected = $.parseJSON($(this).val());
                        uc.ucmaps.loadinfowindow(selected.id);
                    }
                    catch (exc1) {
                        app.excManager("address_view", "lbxAddress.change", exc1);
                    }
                });
            }
            catch (exc) {
                app.excManager("address_view", "initialize", exc);
            }
        };

        /*-----------------------------------------------
        - Busca una direccion en google
        ------------------------------------------------*/
        uc.search_address = function () {
            try{
                this.ucmaps.searchaddress({
                    address: $('#tbxaddress').val(),
                    lat:0,
                    lng: 0,
                    callback: ucaddress.search_address_callback
                });
            }
            catch (exc) {
                app.excManager("address_view", "search_address", exc);
            }
        };

        uc.search_address_callback = function (rs) {
            /*
            rs: [] => {
                address: fullAddress,
                lat: lat,
                lng: lng,
                index: n
            }
            */
            try{
                console.log("Calback en address view con la informacion de puntos: " + rs);
                var template = '';
                $.each(rs, function (index, item) {
                    var optionvalue = JSON.stringify(item).replace('\'', '');
                    //console.log("Option Value: " + optionvalue);
                    var fullAddress = (item.street + " " + item.city + ", " + item.state + " " + item.postalcode + " " + item.country);
                    template += '<option value=\'' + optionvalue + '\'>' + fullAddress + '</option>';
                });
                $('#lbxAddress').html(template);
            }
            catch (exc) {

            }
        };

        /*-----------------------------------------------
        - Unload Module
        ------------------------------------------------*/
        uc.unloadmodule = function () {
            try {
                console.log("Unload Address");
                if (typeof (this.ucmaps) != 'undefined')
                    delete uc.ucmaps;

                delete ucaddress;
                delete usercontrol_address;
            }
            catch (exc) {
                app.excManager("address_view", "unloadmodule", exc);
            }
        };

        /*-----------------------------------------------
        - Funcion llamada cuando se presiona accept en la forma
        ------------------------------------------------*/
        uc.returnselection = function () {
            try{
                var selectedaddress = $.parseJSON($('#lbxAddress').val());
                uc.result = selectedaddress;
                $('#dialog-address').dialog("close");
            }
            catch (exc) {

            }
            return false;
        };
    }
    catch (exc) {
        console.log('usercontrol_locations exc: ' + exc.message);
    }
    return uc;
}