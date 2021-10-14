$(document).ready(function () {
    $('#accountDatatable').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/Accounts",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "accountNumber", "name": "AccountNumber", "autoWidth": true },
            { "data": "accountName", "name": "Account Name", "autoWidth": true },
            { "data": "description", "name": "Description", "autoWidth": true },
            { "data": "category", "name": "Category", "autoWidth": true },
            { "data": "subCategory", "name": "SubCategory", "autoWidth": true },
            { "data": "balance", "name": "Balance", "autoWidth": true },
            { "data": "createdOn", "name": "CreatedOn", "autoWidth": true },
            { "data": "userName", "name": "Username", "autoWidth": true },
            { "data": "comments", "name": "Comments", "autoWidth": true },
        ]



    });
});