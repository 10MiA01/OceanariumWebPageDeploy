var dataTable;
var selectedUrl; 

const filtersState = {
    id: null,
    eventId: null,
    orderId: null,
    discountType: null,
    purchaseDate: null,
    status: null
};

function updateFilter(key, value) {
    filtersState[key] = value || null; // If empty => null
}
//Filters on page
function getQueryStringFromFilters() {
    const filtered = Object.entries(filtersState)
        .filter(([_, v]) => v !== null && v !== '') // Delete empty
        .reduce((acc, [k, v]) => {
            acc[k] = v;
            return acc;
        }, {});

    return $.param(filtered);
}

//Url filter
function getQueryParam(name) {
    return new URLSearchParams(window.location.search).get(name);
}
const initialOrderId = getQueryParam('orderId');
if (initialOrderId) {
    updateFilter('orderId', initialOrderId);
    $('#filterOrderId').val(initialOrderId);
}
const initialEventId = getQueryParam('eventId');
if (initialEventId) {
    updateFilter('eventId', initialEventId);
    $('#filterEventId').val(initialEventId);
}


//Filters 
$('#filterTicketId').on('input', function () {
    updateFilter('id', $(this).val());
});

$('#filterEventId').on('input', function () {
    updateFilter('eventId', $(this).val());
});

$('#filterOrderId').on('input', function () {
    updateFilter('orderId', $(this).val());
});

$('#filterDiscountType').on('change', function () {
    updateFilter('discountType', $(this).val());
});

$('#filterPurchaseDate').on('change', function () {
    updateFilter('purchaseDate', $(this).val());
});

$('#filterStatus').on('change', function () {
    updateFilter('status', $(this).val());
});


$(document).ready(function () {
    dataTable = loadDataTable();

    //if url filter
    if (initialOrderId || initialEventId) {
        const query = getQueryStringFromFilters();
        dataTable.ajax.url('/Admin/Tickets/Index?handler=AllTickets&' + query).load();
    }
    //Apply filters button
    $('#applyFilters').on('click', function (e) {
        e.preventDefault();
        const query = getQueryStringFromFilters();
        dataTable.ajax.url('/Admin/Tickets/Index?handler=AllTickets&' + query).load();
    });
});

function loadDataTable() {
    dataTable = $('#ticketTableAdmin').DataTable({
        "ajax": {
            url: '/Admin/Tickets/Index?handler=AllTickets',
            "type": "GET",
            "dataType": "json",
            dataSrc: function (json) {
                return json.objTicketsList;
            }
        },
        "columns": [
            { data: 'id' },
            { data: 'eventId' },
            { data: 'orderId' },
            { data: 'discountType' },
            { data: 'purchaseDate' },
            { data: 'status' },
            { data: 'ticketCode' },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Tickets/Edit?ticketId=${data}" class="btn btn-outline-secondary delete-button">
                                <i class="bi bi-pencil-square"></i>
                                Edit
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Tickets/Index?handler=DeleteTicket&ticketId=${data}" class="btn btn-outline-secondary delete-link">
                                <i class="bi bi-trash"></i>
                                Delete
                            </a>`;
                }
            }
        ],
        order: [[0, 'asc']]
    });

    //Confirmation modal delete
    $('#ticketTableAdmin tbody').on('click', 'a.delete-link', function (e) {
        e.preventDefault();

        selectedUrl = $(this).attr('href');
        var myModal = new bootstrap.Modal(document.getElementById('confirmCancelModal'));
        myModal.show();
    });

    $('#confirmCancelButton').on('click', function () {
        if (selectedUrl) {
            window.location.href = selectedUrl;
        }
    });

    return dataTable; 
}