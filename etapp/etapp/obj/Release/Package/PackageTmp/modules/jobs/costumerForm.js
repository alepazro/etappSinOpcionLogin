var customerForm = (function () {
  
    var currentModule = 3; //modulo de creacion de jobs
    var validator = null;
    var location = null;
    var currentcustomer = null;

    function InitModule() {
        try {
            app.currentModule = currentModule;

            $('#btnGeocoding').click(function () {             
                app.findaddress();
            });


            $('#btnSaveCustomer').click(function () {

                if (validator.validate() === false) {
                    // get the errors and write them out to the "errors" html container
                    var errors = validator.errors();
                    $(errors).each(function () {
                        alert('Fields in red are required');
                    });
                }
                else {
                    var current = {};
                    current.name = $('#custName').val();
                    current.street = $('#custAddress').val();
                    current.city = $('#custCity').val();
                    current.state = $('#custState').val();
                    current.contactName = $('#custCN').val();
                    current.postalCode = $('#custPC').val();
                    current.email = $('#custEmail').val();
                    current.phone = $('#custPhone').val();
                    current.country=$('#custCountry').val();
                    current.lat = customerForm.location.lat;
                    current.lng = customerForm.location.lng;
                    
                                       
                    if (currentcustomer == null) {
                        current.id = 0;
                        currentcustomer = current;
                    }
                    else {
                        currentcustomer.name = current.name;
                        currentcustomer.street = current.street;
                        currentcustomer.city = current.city;
                        currentcustomer.state = current.state;
                        currentcustomer.contactName = current.contactName;
                        currentcustomer.postalCode = current.postalCode;
                        currentcustomer.email = current.email;
                        currentcustomer.phone = current.phone;
                        currentcustomer.country = current.country
                        currentcustomer.lat = current.lat;
                        currentcustomer.lng = current.lng;
                    }
                    
                    CustomerSave();





                }
            });


            $('#btnCloseCustomer').click(function () {

                cancel();
            });


            validator = $("#cutomerValidator").kendoValidator().data("kendoValidator");

        } catch (exc) {
            _Show("EXC InitModule " + exc.message);
        }
     
    }

    function Resize() {
        try {

        } catch (exc) {
            _Show("EXC Resize " + exc.message);
        }
    }

    function cancel() {
        app.currentModule = 0;// siempre se debe poner para que las funciones genericas queden en estado inicial
        $("#customerForm").data('kendoWindow').close();
    }

    function CustomerSave() {
        try {
            var flagrun = true;
            if (currentcustomer.id == 0) {
                var customer = _.where(app.dataCustomers, { name: currentcustomer.name })[0];
                if (customer != null && typeof (customer) != 'undefined') {
                    flagrun = false;
                }
            }

            if (flagrun) {
                loading(true);
                var data = JSON.stringify(currentcustomer);

                //_Show("Data Customer -->" + data);
                dbSync('jobs.svc', 'postCustomer', data, "POST", CustomerSave_callback);
            }
            else {
               alter("The new customer entered already exists");
            }

        } catch (exc) {
            _Show("EXC CustomerSave " + exc.message);
        }
    }

    function CustomerSave_callback(rs) {
        try {
           // _Show("Result process Customer -->" + JSON.stringify(rs));
            if (typeof (rs.resultCode) != 'undefined') {
                alert("EXC  JobForm_callback -->" + rs.resultMsg);
                return;
            }

            if (rs.isOk) {
                if (currentcustomer.id == 0)
                    alert("The Customer was created successfully ");
                else
                    alert("The Customer was updated successfully ");

                app.reloadFilters = false;
                LoadCustomers(customerForm.customerrefresh);
            }
            else {

                alert("Error --> " + rs.msg);
            }

        } catch (exc) {
            _Show("EXC CustomerSave_callback " + exc.message);
        }
    }

    //ya recargue la data delos customers
    function customerrefresh() {
        cancel();
        loading(false);
    }

    function SetCostumer(customer) {
        try {
            var lat = 0, lng = 0;
            var city = '', state = '', postalCode = '', country = '', street = '', contactName='',email='',phone='';

            if (customer != null) {
                lat = customer.lat;
                lng = customer.lng;
                city = customer.city;
                state = customer.state;
                postalCode = customer.postalCode;
                street = customer.street;
                country = customer.country;
                contactName = customer.contactName;
                email = customer.email;
                phone = customer.phone;


                currentcustomer = customer;
                $('#custName').val(customer.name);
                $('#custAddress').val(street);
                $('#custCity').val(city);
                $('#custState').val(state);
                $('#custCN').val(contactName);
                $('#custPC').val(postalCode);
                $('#custEmail').val(email);
                $('#custPhone').val(phone);
                $('#custCountry').val(country);
               
                customerForm.location = {
                    address1: street,
                    address2: '',
                    city: city,
                    state: state,
                    postalcode: postalCode,
                    country: country,
                    lat: lat,
                    lng: lng,
                    radius: 0
                };

            }

        } catch (exc) {
            _Show("EXC SetCostumer " + exc.message);
        }
    }

    function SetLocation(location) {
        try {

            $('#custAddress').val(location.address1);
            $('#custCity').val(location.city);
            $('#custState').val(location.state);
            $('#custPC').val(location.postalCode);
            $('#custCountry').val(location.country);

            if (currentcustomer != null) {
                currentcustomer.lat = location.lat;
                currentcustomer.lng = location.lng;
            }

            customerForm.location = {
                address1: location.address1,
                address2: '',
                city: location.city,
                state: location.state,
                postalcode: location.postalCode,
                country: location.country,
                lat: location.lat,
                lng: location.lng,
                radius: 0
            };
         
        }
        catch (exc) {
            _Show("EXC SetLocation " + exc.message);
        }
      
    }

    function getLocation() {
        try {
            return  {
                address1: $('#custAddress').val(),
                address2: '',
                city:$('#custCity').val(),
                state: $('#custState').val(),
                postalcode: $('#custPC').val(),
                country: $('#custCountry').val(),
                lat: 0,
                lng:0,
                radius: 0
            };

        } catch (exc) {
            _Show("EXC getLocation " + exc.message);

        }
        return null;
    }

    return {
        InitModule: function () {
            InitModule();
        },
        resize: function () {
            Resize();
        },
        SetCostumer: function (customer) {
            SetCostumer(customer);
        },
        SetLocation: function (location) {
            SetLocation(location);
        },
        getLocation: function () {
           return getLocation();
        },
        getCustomer: function () {
            return currentcustomer;
        },
        customerrefresh: function () {
            customerrefresh();
        }
    };

    // Pull in jQuery and Underscore
})();

customerForm.InitModule();
app.currentModule = 3;