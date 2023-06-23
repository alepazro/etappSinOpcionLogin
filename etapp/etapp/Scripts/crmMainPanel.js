//https://www.trirand.com/jqgridwiki/doku.php?id=wiki:events
//https://trirand.com/blog/jqgrid/jqgrid.html

var jsonCustomers = false;

function clearCustSearch() {
    try {
        //$('#customerSearch').val('');
        //loadCustomers('');
        $("#tbody_customersGrid").empty();
    }
    catch (err) {
        alert('Error: ' + err.description);
    }
}

function searchCustomers() {
    try {
        
        $("#tbody_customersGrid").empty();
        var search = $('#customerSearch').val();
        var row = "";
        var searchParam = '';
        if (!_.isUndefined(search)) {
            searchParam = 'search=' + search;
        }
        else {
            searchParam = 'search=0';
        }
        
        var data = jsonGET('crm.svc', 'customers', searchParam, true);
        $.each(data, function (key, value) {
            //alert(key + ": " + value.name);
            row += "<tr>";

            /*row += "<td><a href=" + viewCustomer(e) + "class=link - primary>Primary link</a></td>";*/

            row += "<td><button type='button' class='btn btn-light' onclick=viewCustomer()>Edit</button></td>";
            row += "<td style=display:none id=uniqueKey>"+ value.uniqueKey + "</td>";
            row += "<td style=display:none>" + value.id + "</td>";
            row += "<td>" + value.name + "</td>";
            row += "<td>" + value.phone + "</td>";
            row += "<td>" + value.email + "</td>";
            row += "<td>" + value.createdOn + "</td>";
            row += "<td>" + value.salesRep + "</td>";
            row += "<td>" + value.isSuspended + "</td>";
            row += "<td>" + value.totalUnits + "</td>";
            row += "<td>" + value.notInstalled + "</td>";
            row += "<td>" + value.workingUnits + "</td>";
            row += "<td>" + value.notWorkingUnits + "</td>";
            row += "<td style=display:none>" + value.usersList + "</td>";
            row += "</tr>";
        });
        $("#tbody_customersGrid").append(row)
       // $('#customersGrid').data('kendoGrid').dataSource.data(data);

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function loadCustomers(search) {
    try {
        var searchParam = '';
        if (!_.isUndefined(search)) {
            searchParam = 'search=' + search;
        }
        else {
            searchParam = 'search=0';
        }

        var data = []; //jsonGET('crm.svc', 'customers', searchParam, true);

        $("#customersGrid").kendoGrid({
            dataSource: {
                data: data,
                pageSize: 20
            },
            height: 650,
            groupable: true,
            sortable: true,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5
            },
            columns: [
                { command: { text: "Edit", click: viewCustomer }, title: " ", width: "70px" },
                { field: "uniqueKey", title: "uniqueKey", hidden: true },
                { field: "id", title: "ID", hidden: true },
                { field: "name", title: "Name", width: "200px"},
                { field: "phone", title: "Phone", width: "120px" },
                { field: "email", title: "Email", width: "150px" },
                { field: "createdOn", title: "Created On", width: "80px" },
                { field: "salesRep", title: "Sales Rep", width: "80px" },
                { field: "isSuspended", title: "Susp.", width: "50px" },
                { field: "totalUnits", title: "T.U.", width:"50px" },
                { field: "notInstalled", title: "N.I.", width: "50px" },
                { field: "workingUnits", title: "W.U.", width: "50px" },
                { field: "notWorkingUnits", title: "N.W.", width: "50px" },
                { field: "usersList", title: "Users List", hidden: true }
            ]
        });

    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

function viewCustomer(e) {
    try {
        //e.preventDefault();
        //var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        //window.open('crmViewCustomer.html?' + 'uid=' + dataItem.uniqueKey, target = "_blank");
        $("#customersGrid tr").click(function () {
            
            var tr = $(this)[0];
            var trID = tr.cells[1].innerText;
            window.open('crmViewCustomer.html?' + 'uid=' + trID, target = "_blank");
           
        });
    }
    catch (err) {
        alert('Error: ' + err.message);
    }
}

