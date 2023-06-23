var jsonFuelLog = [];
var fuelCardFormatId = 0;

function clearUploadFuelLogArea() {
    $('#dataFuelLogTextAreaDiv').show();
    $('#dataFuelLog').val('');

    $('#dataFuelLogTableDiv').hide();
    $('#dataFuelLogTable').html('');

    jsonFuelLog = [];
}

function loadExpectedColumnsInformation() {
    switch (fuelCardFormatId) {
        case 1:
        case '1':
            $('#dataFuelColumns').html('EQNumber, TransDate, TransTime, Units, UnitPrice, MerchantAddress, MerchantCity, MerchantState, MerchantZip');
            break;
        case 2:
        case '2':
            $('#dataFuelColumns').html('Date, Time, Card#, Driver, Unit, Parent Account, Trans Type, T/S#, Merchant Name, City, ST, DieselGal, DieselAmt, Reefer, Reefer, Dscnt, Net P/G, Rebate, Cash, Other, Invoice');
            break;
        case 3:
        case '3':
            $('#dataFuelColumns').html('Col A through Col K in Excel.  Sample: JANET, 8749113, 7149595, 2/1/2015, 1500, 284122, 50, 7.3, 2.089, 15.25, AM1');
            break;
        case 4:
        case '4':
            $('#dataFuelColumns').html('Card Number, Department, Customer, Vehicle ID, Post Date, Trans Date, Time, Merchant, Merchant Name, Merchant Address, Merchant City, Merchant State, Merchant Zip Code, Driver Name, Driver ID, Driver Department, Current Odometer, Adjusted Odometer, Transaction Description, Product, Units, $Unit Cost, $Fuel Cost, $Non-Fuel Cost, $Gross Cost, $Exempt Tax (-), Trans Fee Type, $Trans Fee (+), $Net Cost (=), $Reported Tax, MPG, $CPM, Exchange Rate');
            break;
        case 5:
        case '5':
            $('#dataFuelColumns').html('ACCOUNT NO, FLEET NO, TRAN_NUMB, TYPE, ENTRY METHOD, AUTH_NUMB, TRAN DATE, POST_DATE, ODOM, JOB_NUMB, CARD_NUMB, CARD DESCRIPTION, CARD_DEPT, ID_NUMB, ID DESCRIPTION, ID_DEPT, MCC_ID, SITE_NUMB, SITE DESCRIPTION, PRODUCT_CODE, PRODUCT_DESC, PRODUCT_GROUP, POSTED_QUANTITY, POSTED_TOTAL_PRICE, EXCEPTIONS');
            break;
    }
}

function loadFuelLogUpload() {
    try{
        switch (fuelCardFormatId) {
            case 1:
            case '1':
                jsonFuelLog = tsvToJson($('#dataFuelLog').val(), true);
                uploadFormat01();
                break;
            case 2:
            case '2':
                jsonFuelLog = tsvToJson($('#dataFuelLog').val(), true);
                uploadFormat02();
                break;
            case 3:
            case '3':
                jsonFuelLog = tsvToJson($('#dataFuelLog').val(), false);
                uploadFormat03();
                break;
            case 4:
            case '4':
                jsonFuelLog = tsvToJson($('#dataFuelLog').val(), true);
                uploadFormat04();
            case 5:
            case '5':
                jsonFuelLog = tsvToJson($('#dataFuelLog').val(), true);
                uploadFormat05();

        }
    }
    catch (err) {

    }
}

function processFuelLogUpload() {
    try {
        $("#waitingSpinner").show();
        //This is done to force the spinning wheel.
        setTimeout(function () {
            var noCache = Math.floor((Math.random() * 100000) + 1);
            var jqXHR = $.ajax({
                type: "GET",
                url: 'etrest.svc/doDummyCall/' + '/' + noCache,
                cache: false,
                data: 0, 
                async: true, // responseText is empty if set to true
                dataType: 'json',
                error: function () {
                    processFuelLogData();
                },
                success: function () {
                    processFuelLogData();
                }
            });
        }, 2000);

    }
    catch (err) {

    }
}

function processFuelLogData() {
    switch (fuelCardFormatId) {
        case 1:
        case '1':
            processFormat01();
            break;
        case 2:
        case '2':
            processFormat02();
            break;
        case 3:
        case '3':
            processFormat03();
            break;
        case 4:
        case '4':
            processFormat04();
            break;
        case 5:
        case '5':
            processFormat05();
            break;
    }
}

function tsvToJson(input, hasHeaders) {
    try {
        var info = input.replace(/['"]/g, '');
        var lines = info.split('\n');
        var firstLine = lines.shift().split('\t');
        var json = [];

        // Helper function to remove quotes
        // and parse numeric values
        var removeQuotes = function (string) {
            string = string.replace(/(['"])/g, "\\$1");
            if (string != '') {
                if (string.length < 10) {
                    if (!isNaN(string)) {
                        string = parseFloat(string);
                    }
                }
            }
            return string;
        };

        var c = 'A';
        if (hasHeaders == false) {
            $.each(firstLine, function (index, item) {
                firstLine[index] = c;
                c = String.fromCharCode(c.charCodeAt() + 1) // Returns the next letter
            });
        }

        $.each(lines, function (index, item) {
            var lineItem = item.split('\t');
            var jsonLineEntry = {};

            $.each(lineItem, function (index, item) {
                jsonLineEntry[firstLine[index]] = removeQuotes(item);
            });
            json.push(jsonLineEntry);

        });

        return json;
    }
    catch (err) {

    }
}

function json2Table() {
    try {
        var i = 0;
        var table = $(document.createElement('table'));
        table.attr('style', 'border-collapse: collapse; border:1px solid #000;padding:5px;spacing:5px;width:90%;')
        $.each(jsonFuelLog, function (key, item) {
            var table_row = $('<tr>');
            table_row.attr('id', 'fuelLogCardRow' + i);
            i += 1;
            table_row.attr('style', 'border:1px solid #000;text-align:left;')
            $.each(item, function (itemKey, itemValue) {
                if (key == 0) {
                    var th = $(document.createElement('th'));
                    th.attr('style', 'border:1px solid #000; text-align:left;font-weight:600;padding:5px;spacing:5px;');
                    th.html(itemKey);
                    table.append(th);
                }
                var td = $(document.createElement('td'));
                td.attr('style', 'border:1px solid #000; text-align:left;padding:5px;spacing:5px;');
                td.html(itemValue);
                table_row.append(td);
            });
            var td = $(document.createElement('td'));
            td.attr('style', 'border:1px solid #000; text-align:left;padding:5px;spacing:5px;');
            td.attr('class', 'fuelLogCardComment')
            td.html('');
            table_row.append(td);

            table.append(table_row);
        });
        $('#dataFuelLogTable').append(table);
        $('#dataFuelLogTableDiv').show();
        $('#dataFuelLogTextAreaDiv').hide();
    }
    catch (err) {

    }
}

//=========================================================================================================
function uploadFormat01() {
    try {
        //Expected columns:
        //EQNumber	TransDate	TransTime	Units	UnitPrice	MerchantAddress	MerchantCity	MerchantState	MerchantZip	
        //SAMPLE RECORD:
        //900023	3/15/2015	07:33:30	31	2.09	22820 N 19TH AVE	PHOENIX	AZ	85027	

        var missingFields = '';
        //Validate the structure of the data:
        if (_.isUndefined(jsonFuelLog[0].EQNumber)) {
            missingFields = 'EQNumber';
        }
        if (_.isUndefined(jsonFuelLog[0].TransDate)) {
            missingFields = 'TransDate';
        }
        if (_.isUndefined(jsonFuelLog[0].TransTime)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'TransTime';
        }
        if (_.isUndefined(jsonFuelLog[0].Units)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'Units';
        }
        if (_.isUndefined(jsonFuelLog[0].UnitPrice)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'UnitPrice';
        }
        if (missingFields.length > 0) {
            alert('The following fields are missing: ' + missingFields + '. Please fix the file and paste it again.')
        }
        else {
            json2Table();
        }


    }
    catch (err) {

    }
}

function processFormat01() {
    try {
        var device = false;
        var date = false;
        var time = false;
        var street = false;
        var city = false;
        var state = false;
        var zip = false;
        var galls = false;
        var price = false;
        var amt = false;

        var data = false;
        var jsonResult = false;

        var i = 0;
        var refreshIntervalId = setInterval(function () {
            if (i == 0) {
                $('#dataFuelLogTotal').html(jsonFuelLog.length);
                $('#dataFuelLogProcessed').html('0');
            }

            if (i < jsonFuelLog.length) {

                var line = jsonFuelLog[i];

                if (!_.isUndefined(line.EQNumber)) {
                    if (!_.isNaN(line.TransDate)) {
                        //Process this record
                        device = line.EQNumber;
                        date = line.TransDate;
                        time = line.TransTime;
                        street = line.MerchantAddress;
                        city = line.MerchantCity;
                        state = line.MerchantState;
                        zip = line.MerchantZip;
                        galls = line.Units;
                        if (!_.isNumber(galls)) {
                            galls = 0;
                        }
                        price = line.UnitPrice;
                        if (!_.isNumber(price)) {
                            price = 0;
                        }
                        amt = galls * price;

                        if (galls > 0) {
                            data = 't=' +
                                getTokenCookie('ETTK') +
                                '&device=' + escape(device) +
                                '&date=' + escape(date) +
                                '&time=' + escape(time) +
                                '&street=' + escape(street) +
                                '&city=' + escape(city) +
                                '&state=' + escape(state) +
                                '&zip=' + zip +
                                '&galls=' + escape(galls) +
                                '&price=' + escape(price) +
                                '&amt=' + escape(amt);
                            jsonResult = dbReadWrite('saveFuelLogUpload', data, false, false);
                            if (!_.isUndefined(jsonResult.error)) {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html(jsonResult.error);
                            }
                            else {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html('OK');
                            }
                        }
                    }
                } // End IF

                $('#dataFuelLogProcessed').html(i + 1);
            } else {
                clearInterval(refreshIntervalId);
                $("#waitingSpinner").hide();
                alert('Information Processed...');
            }
            i++;
        }, 100);
    }
    catch (err) {

    }
}
//=========================================================================================================
function uploadFormat02() {
    try {
        //Expected columns:
        //Date	Time	Card#	Driver	Unit	Parent Account	Trans Type	T/S#	Merchant Name	City	ST	DieselGal	DieselAmt	Reefer	Reefer	Dscnt	Net P/G	Rebate	Cash	Other	Invoice
        //SAMPLE RECORD:
        //2/28/2015	20:03	1	2	6		Purchase	195911	Pilot Corporation LLC	Anthony	TX	68.273	195.19	0	0	0	2.859	0	0	0	195.19
        // The following column lables have to be modified from the original:
        // Driver# to Driver
        // Unit@ to Unit
        // Diesel Gal to DieselGal
        // Diesel Amt to DieselAmt

        var missingFields = '';
        //Validate the structure of the data:
        if (_.isUndefined(jsonFuelLog[0].Date)) {
            missingFields = 'Date';
        }
        if (_.isUndefined(jsonFuelLog[0].Time)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'Time';
        }
        if (_.isUndefined(jsonFuelLog[0].Unit)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'Unit';
        }
        if (_.isUndefined(jsonFuelLog[0].City)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'City';
        }
        if (_.isUndefined(jsonFuelLog[0].ST)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'ST';
        }
        if (_.isUndefined(jsonFuelLog[0].DieselGal)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'DieselGal';
        }
        if (_.isUndefined(jsonFuelLog[0].DieselAmt)) {
            if (missingFields.length > 0) {
                missingFields = missingFields + ', ';
            }
            missingFields = missingFields + 'DieselAmt';
        }

        if (missingFields.length > 0) {
            alert('The following fields are missing: ' + missingFields + '. Please fix the file and paste it again.')
        }
        else {
            json2Table();
        }
    }
    catch (err) {

    }
}

function processFormat02() {
    try {
        var date = false;
        var time = false;
        var driver = false;
        var device = false;
        var city = false;
        var state = false;
        var galls = false;
        var price = false;
        var amt = false;

        var data = false;
        var jsonResult = false;

        var i = 0;
        var refreshIntervalId = setInterval(function () {
            if (i == 0) {
                $('#dataFuelLogTotal').html(jsonFuelLog.length);
                $('#dataFuelLogProcessed').html('0');
            }

            if (i < jsonFuelLog.length) {

                    var line = jsonFuelLog[i];

                    if (!_.isUndefined(line.Date)) {
                        if (!_.isNaN(line.Date)) {
                            //Process this record
                            date = line.Date;
                            time = line.Time;
                            driver = line.Driver;
                            device = line.Unit;
                            city = line.City;
                            state = line.ST;
                            galls = line.DieselGal;
                            if (!_.isNumber(galls)) {
                                galls = 0;
                            }
                            amt = line.DieselAmt;
                            if (!_.isNumber(amt)) {
                                amt = 0;
                            }

                            if (galls > 0) {
                                price = amt / galls;
                                data = 't=' + getTokenCookie('ETTK') + '&device=' + escape(device) + '&driver=' + escape(driver) + '&date=' + escape(date) + '&time=' + escape(time) + '&city=' + escape(city) + '&state=' + escape(state) + '&galls=' + escape(galls) + '&price=' + escape(price) + '&amt=' + escape(amt);
                                jsonResult = dbReadWrite('saveFuelLogUpload', data, false, false);
                                if (!_.isUndefined(jsonResult.error)) {
                                    $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html(jsonResult.error);
                                }
                                else {
                                    $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html('OK');
                                }
                            }

                        }
                    } // End IF

                $('#dataFuelLogProcessed').html(i + 1);
            } else {
                clearInterval(refreshIntervalId);
                $("#waitingSpinner").hide();
                alert('Information Processed...');
            }
            i++
        }, 100);
    }
    catch (err) {

    }
}
//=========================================================================================================
function uploadFormat03() {
    try {
        //Expected columns:
        //QM Transport's file does not have column headers.
        //SAMPLE RECORD:
        //JANET               	8749113	7149595	2/1/2015	1500	284122	50	7.3	2.089	15.25	AM1 

        var missingFields = '';
        //Validate the structure of the data:

        if (missingFields.length > 0) {
            alert('The following fields are missing: ' + missingFields + '. Please fix the file and paste it again.')
        }
        else {
            json2Table();
        }
    }
    catch (err) {

    }
}

function processFormat03() {
    try {
        //UnitName?(A), UnitID?(B), Ignore(C), Date(D), Time(E), Ignore(F), Ignore(G), Galls(H), Price(I), Amt(J), Ignore(K)
        //JANET  8749113	7149595	2/1/2015	1500	284122	50	7.3	2.089	15.25	AM1 

        var date = false;
        var time = false;
        var driver = false;
        var device = false;
        var galls = false;
        var price = false;
        var amt = false;

        var data = false;
        var jsonResult = false;

        var i = 0;
        var refreshIntervalId = setInterval(function () {
            if (i == 0) {
                $('#dataFuelLogTotal').html(jsonFuelLog.length);
                $('#dataFuelLogProcessed').html('0');
            }

            if (i < jsonFuelLog.length) {

                var line = jsonFuelLog[i];

                if (!_.isUndefined(line.H)) {
                    if (!_.isNaN(line.H)) {
                        //Process this record
                        driver = line.A;
                        device = line.B;
                        date = line.D;
                        time = line.E.toString();
                        if (time.length == 3) {
                            time = [line.E.toString().slice(0, 1), ':', line.E.toString().slice(1)].join('');
                        }
                        else {
                            time = [line.E.toString().slice(0, 2), ':', line.E.toString().slice(2)].join('');
                        }
                        galls = line.H;
                        if (!_.isNumber(galls)) {
                            galls = 0;
                        }
                        price = line.I;
                        amt = line.J;
                        if (!_.isNumber(amt)) {
                            amt = 0;
                        }

                        if (galls > 0) {
                            data = 't=' + getTokenCookie('ETTK') + '&device=' + escape(device) + '&driver=' + escape(driver) + '&date=' + escape(date) + '&time=' + escape(time) + '&galls=' + escape(galls) + '&price=' + escape(price) + '&amt=' + escape(amt);
                            jsonResult = dbReadWrite('saveFuelLogUpload', data, false, false);
                            if (!_.isUndefined(jsonResult.error)) {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html(jsonResult.error);
                            }
                            else {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html('OK');
                            }
                        }

                    }
                } // End IF

                $('#dataFuelLogProcessed').html(i + 1);
            } else {
                clearInterval(refreshIntervalId);
                $("#waitingSpinner").hide();
                alert('Information Processed...');
            }
            i++
        }, 100);
    }
    catch (err) {

    }
}
//=========================================================================================================
function uploadFormat04() {
    try {
        //Expected columns:
        //Card Number	Department	Customer Vehicle ID	Post Date	Trans Date	Time	Merchant	Merchant Name	Merchant Address	Merchant City	Merchant State	Merchant Zip Code	Driver Name	Driver ID	Driver Department	Current Odometer	Adjusted Odometer	Transaction Description	Product	Units	$Unit Cost	$Fuel Cost	$Non-Fuel Cost	$Gross Cost	$Exempt Tax (-)	Trans Fee Type	$Trans Fee (+)	$Net Cost (=)	$Reported Tax	MPG	$CPM	Exchange Rate
        //SAMPLE RECORD:
        //0004 - 1	AMBULANC	SQUAD 105	4/9/2015	4/6/2015	5:01:25 PM	UNBRANDED	CASEYS GEN STORE 324	2655 HWY 412 E	SILOAM SPRING	AR	72761-0000	EASON	4034	AMBULANC	185600		Outside Payment Terminal	DSL	9.738	$2.40 	$23.36 	$0.00 	$23.36 	$0.00 	 	$0.00 	$23.36 	$0.00 	0	$0.00 

        var missingFields = '';
        //Validate the structure of the data:
        if (missingFields.length > 0) {
            alert('The following fields are missing: ' + missingFields + '. Please fix the file and paste it again.')
        }
        else {
            json2Table();
            //In this case, if the file comes with first row of titles, we remove it because some columns have special characters that complicate reading it.
        }
    }
    catch (err) {

    }
}

function processFormat04() {
    try {
        //Card Number	Department	Customer Vehicle ID	Post Date	Trans Date	Time	Merchant	Merchant Name	Merchant Address	Merchant City	Merchant State	Merchant Zip Code	Driver Name	Driver ID	Driver Department	
        //Current Odometer	Adjusted Odometer	Transaction Description	Product	Units	$Unit Cost	$Fuel Cost	$Non-Fuel Cost	$Gross Cost	$Exempt Tax (-)	Trans Fee Type	$Trans Fee (+)	$Net Cost (=)	$Reported Tax	MPG	$CPM	Exchange Rate
        //SAMPLE RECORD:
        //0004 - 1	AMBULANC	SQUAD 105	4/9/2015	4/6/2015	5:01:25 PM	UNBRANDED	CASEYS GEN STORE 324	2655 HWY 412 E	SILOAM SPRING	AR	72761-0000	EASON	4034	AMBULANC	185600		Outside Payment Terminal	DSL	9.738	$2.40 	$23.36 	$0.00 	$23.36 	$0.00 	 	$0.00 	$23.36 	$0.00 	0	$0.00 

        var cardNumber = false;
        var merchantName = false;
        var street = false;
        var city = false;
        var state = false;
        var zip = false;
        var date = false;
        var time = false;
        var driver = false;
        var odometer = false;
        var lat = false;
        var lng = false;
        var device = false;
        var galls = false;
        var price = false;
        var amt = false;

        var data = false;
        var jsonResult = false;

        var i = 0;
        var refreshIntervalId = setInterval(function () {
            if (i == 0) {
                $('#dataFuelLogTotal').html(jsonFuelLog.length);
                $('#dataFuelLogProcessed').html('0');
            }

            if (i < jsonFuelLog.length) {

                var line = jsonFuelLog[i];

                if (!_.isUndefined(line['Card Number'])) {
                    if (!_.isNaN(line['Card Number'])) {
                        //Process this record
                        cardNumber = line["Card Number"];
                        merchantName = line["Merchant Name"];
                        street = line["Merchant Address"];
                        city = line["Merchant City"];
                        state = line["Merchant State"];
                        zip = line["Merchant Zip Code"];
                        date = line["Trans Date"];
                        time = line["Time"];
                        driver = line["Driver Name"];
                        odometer = line["Current Odometer"];
                        lat = 0;
                        lng = 0;
                        device = line["Customer Vehicle ID"];
                        galls = line["Units"];
                        price = line["$Unit Cost"];
                        amt = line["$Fuel Cost"];

                        if (galls > 0) {

                            data = 't=' +
                                getTokenCookie('ETTK') +
                                '&device=' + escape(device) +

                                '&cardNumber=' + escape(cardNumber) +
                                '&merchantName=' + escape(merchantName) +

                                '&driver=' + escape(driver) +
                                '&date=' + escape(date) +
                                '&time=' + escape(time) +
                                '&street=' + escape(street) +
                                '&city=' + escape(city) +
                                '&state=' + escape(state) +
                                '&zip=' + zip +

                                '&odometer=' + odometer +
                                '&lat=' + lat +
                                '&lng=' + lng +

                                '&galls=' + escape(galls) +
                                '&price=' + escape(price) +
                                '&amt=' + escape(amt);

                            jsonResult = dbReadWrite('saveFuelLogUpload', data, false, false);
                            if (!_.isUndefined(jsonResult.error)) {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html(jsonResult.error);
                            }
                            else {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html('OK');
                            }
                        }

                    }
                } // End IF

                $('#dataFuelLogProcessed').html(i + 1);
            } else {
                clearInterval(refreshIntervalId);
                $("#waitingSpinner").hide();
                alert('Information Processed...');
            }
            i++
        }, 100);
    }
    catch (err) {

    }
}

//=========================================================================================================
function uploadFormat05() {
    try {
        //Expected columns:
        //ACCOUNT NO, FLEET NO, TRAN_NUMB, TYPE, ENTRY METHOD, AUTH_NUMB, TRAN DATE, POST_DATE, ODOM, JOB_NUMB, CARD_NUMB, CARD DESCRIPTION, CARD_DEPT, ID_NUMB, ID DESCRIPTION, ID_DEPT, MCC_ID, SITE_NUMB, SITE DESCRIPTION, PRODUCT_CODE, PRODUCT_DESC, PRODUCT_GROUP, POSTED_QUANTITY, POSTED_TOTAL_PRICE, EXCEPTIONS
        //SAMPLE RECORD:
        //889963302	2292214	046540	Sale	Manual	F169476	06-01-2015 09:36	06-03-2015 02:45	188106		7088889963302441798	2002 E250COL	COL	4712	Shammuel Jamison	COL		07106552	SUNOCO	2	Unleaded	Fuel Products	26.410	68.64	General Suspension

        var missingFields = '';
        //Validate the structure of the data:
        if (missingFields.length > 0) {
            alert('The following fields are missing: ' + missingFields + '. Please fix the file and paste it again.')
        }
        else {
            json2Table();
            //In this case, if the file comes with first row of titles, we remove it because some columns have special characters that complicate reading it.
        }
    }
    catch (err) {

    }
}

function processFormat05() {
    try {
        //ACCOUNT NO, FLEET NO, TRAN_NUMB, TYPE, ENTRY METHOD, AUTH_NUMB, TRAN DATE, POST_DATE, ODOM, JOB_NUMB, CARD_NUMB, CARD DESCRIPTION, CARD_DEPT, ID_NUMB, ID DESCRIPTION, ID_DEPT, MCC_ID, SITE_NUMB, SITE DESCRIPTION, PRODUCT_CODE, PRODUCT_DESC, PRODUCT_GROUP, POSTED_QUANTITY, POSTED_TOTAL_PRICE, EXCEPTIONS
        //SAMPLE RECORD:
        //889963302	2292214	046540	Sale	Manual	F169476	06-01-2015 09:36	06-03-2015 02:45	188106		7088889963302441798	2002 E250COL	COL	4712	Shammuel Jamison	COL		07106552	SUNOCO	2	Unleaded	Fuel Products	26.410	68.64	General Suspension

        var cardNumber = false;
        var merchantName = false;
        var street = false;
        var city = false;
        var state = false;
        var zip = false;
        var date = false;
        var time = false;
        var driver = false;
        var odometer = false;
        var lat = false;
        var lng = false;
        var device = false;
        var galls = false;
        var price = false;
        var amt = false;

        var data = false;
        var jsonResult = false;

        var i = 0;
        var refreshIntervalId = setInterval(function () {
            if (i == 0) {
                $('#dataFuelLogTotal').html(jsonFuelLog.length);
                $('#dataFuelLogProcessed').html('0');
            }

            if (i < jsonFuelLog.length) {

                var line = jsonFuelLog[i];

                if (!_.isUndefined(line['ACCOUNT NO'])) {
                    if (!_.isNaN(line['ACCOUNT NO'])) {
                        //Process this record
                        cardNumber = line["ACCOUNT NO"];
                        merchantName = line["SITE DESCRIPTION"];
                        street = '';
                        city = '';
                        state = '';
                        zip = '';
                        date = line["TRAN DATE"];
                        time = '';
                        driver = line["ID DESCRIPTION"];
                        odometer = line["ODOM"];
                        lat = 0;
                        lng = 0;
                        device = line["CARD_NUMB"];
                        galls = line["POSTED_QUANTITY"];
                        price = 0;
                        amt = line["POSTED_TOTAL_PRICE"];

                        if (galls > 0) {

                            data = 't=' +
                                getTokenCookie('ETTK') +
                                '&device=' + escape(device) +

                                '&cardNumber=' + escape(cardNumber) +
                                '&merchantName=' + escape(merchantName) +

                                '&driver=' + escape(driver) +
                                '&date=' + escape(date) +
                                '&time=' + escape(time) +
                                '&street=' + escape(street) +
                                '&city=' + escape(city) +
                                '&state=' + escape(state) +
                                '&zip=' + zip +

                                '&odometer=' + odometer +
                                '&lat=' + lat +
                                '&lng=' + lng +

                                '&galls=' + escape(galls) +
                                '&price=' + escape(price) +
                                '&amt=' + escape(amt);

                            jsonResult = dbReadWrite('saveFuelLogUpload', data, false, false);
                            if (!_.isUndefined(jsonResult.error)) {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html(jsonResult.error);
                            }
                            else {
                                $('#fuelLogCardRow' + i).find('.fuelLogCardComment').html('OK');
                            }
                        }

                    }
                } // End IF

                $('#dataFuelLogProcessed').html(i + 1);
            } else {
                clearInterval(refreshIntervalId);
                $("#waitingSpinner").hide();
                alert('Information Processed...');
            }
            i++
        }, 100);
    }
    catch (err) {

    }
}
//=========================================================================================================
