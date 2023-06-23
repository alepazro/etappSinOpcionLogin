function loadCases() {
    try {
        //var data = jsonGET('cases.svc', 'cases', 0, true);

        //$("#casesGrid").kendoGrid({
        //    dataSource: {
        //        data: data,
        //        pageSize: 20
        //    },
        //    height: 550,
        //    groupable: true,
        //    sortable: true,
        //    pageable: {
        //        refresh: true,
        //        pageSizes: true,
        //        buttonCount: 5
        //    },
        //    columns: [
        //        {
        //            command: { text: "Edit", click: editCase },
        //            title: " ",
        //            width: "150px"
        //        },
        //        {
        //        field: "id",
        //        title: "ID",
        //        hidden: true,
        //        width: 240
        //    }, {
        //        field: "companyName",
        //        title: "Company"
        //    }, {
        //        field: "statusName",
        //        title: "Status"
        //    }, {
        //        field: "categoryName",
        //        title: "Category"
        //    }, {
        //        field: "typeName",
        //        title: "Type"
        //    }, {
        //        field: "deviceName",
        //        title: "device"
        //    }, {
        //        field: "assignedToName",
        //        title: "Assigned to"
        //    }, {
        //        field: "subject",
        //        title: "Subject"
        //    }, {
        //        field: "createdOn",
        //        title: "Created On"
        //    }, {
        //        field: "lastUpdatedOn",
        //        title: "Last Activity On"
        //    }
        //    ]
        //});

    }
    catch (err) {

    }
}

function newCase() {
    try {
        window.open('crmCase.html?' + 'id=0', target = "_blank");
    }
    catch (err) {
    }
}

function editCase_deprecated(e) {
    //now this function is in the controller of cases (casesController)
    try {
        e.preventDefault();
        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
        window.open('crmCase.html?' + 'id=' + dataItem.id, target = "_blank");
    }
    catch (err) {

    }
}
