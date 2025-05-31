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

function getQueryStringFromFilters() {
    const filtered = Object.entries(filtersState)
        .filter(([_, v]) => v !== null && v !== '') // Delete empty
        .reduce((acc, [k, v]) => {
            acc[k] = v;
            return acc;
        }, {});

    return $.param(filtered);
}



//Filters
$('#filterOrderId').on('input', function () {
    updateFilter('id', $(this).val());
});

$('#buyerEmail').on('input', function () {
    updateFilter('buyerEmail', $(this).val());
});

$('#filterDateFrom').on('input', function () {
    updateFilter('dateFrom', $(this).val());
});

$('#filterDateTo').on('change', function () {
    updateFilter('dateTo', $(this).val());
});

$('#ticketQuantityFrom').on('change', function () {
    updateFilter('ticketQuantityFrom', $(this).val());
});

$('#ticketQuantityTo').on('change', function () {
    updateFilter('ticketQuantityTo', $(this).val());
});
$('#orderCode').on('change', function () {
    updateFilter('orderCode', $(this).val());
});
$('#orderStatus').on('change', function () {
    updateFilter('orderStatus', $(this).val());
});
$('#totalAmounFrom').on('change', function () {
    updateFilter('totalAmounFrom', $(this).val());
});
$('#totalAmountTo').on('change', function () {
    updateFilter('totalAmountTo', $(this).val());
});
$('#paymentMethod').on('change', function () {
    updateFilter('paymentMethod', $(this).val());
});



$(document).ready(function () {
    dataTable = loadDataTable();

    $('#applyFilters').on('click', function (e) {
        e.preventDefault();
        const query = getQueryStringFromFilters();
        dataTable.ajax.url('/Admin/Orders/Index?handler=AllOrders&' + query).load();
    });
});


function loadDataTable() {
    dataTable = $('#orderTableAdmin').DataTable({
        "ajax": {
            url: '/Admin/Orders/Index?handler=AllOrders',
            "type": "GET",
            "dataType": "json",
            dataSrc: function (json) {
                return json.objOrderList;
            }
        },
        "columns": [
            { data: 'id' },
            { data: 'buyerEmail' },
            { data: 'createdAt' },
            { data: 'totalAmount' },
            { data: 'paymentMethod' },
            { data: 'orderStatus' },
            { data: 'orderCode' },
            {
                className: 'details-control',
                orderable: false,
                data: 'id',
                render: function (data) {
                    return `<button  class="btn btn-outline-secondary details-control" data-order-id="${data}" >
							    <i class="bi bi-ticket"></i>
                                View tickets
							</button>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Tickets/Index?orderId=${data}" class="btn btn-outline-secondary">
                                <i class="bi bi-ticket"></i> Tickets manage
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Orders/Index?handler=SendEmail&orderId=${data}" class="btn btn-outline-secondary email-button">
                                <i class="bi bi-envelope-arrow-up"></i>
								Send email
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Orders/Edit?orderId=${data}" class="btn btn-outline-secondary edit-button">
                                <i class="bi bi-pencil-square"></i>
                                Edit
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Orders/Index?handler=DeleteOrder&orderId=${data}" class="btn btn-outline-secondary delete-link">
                                <i class="bi bi-trash"></i>
                                Delete
                            </a>`;
                }
            }
        ],
        order: [[0, 'asc']]
    });

    //Confirmation modal delete
    $('#orderTableAdmin tbody').on('click', 'a.delete-link', function (e) {
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

    //Confirmation modal send email
    $('#orderTableAdmin tbody').on('click', 'a.email-button', function (e) {
        e.preventDefault();

        selectedUrl = $(this).attr('href');
        var myModal = new bootstrap.Modal(document.getElementById('confirmEmailSendModal'));
        myModal.show();
    });

    $('#confirmEmailSendButton').on('click', function () {
        if (selectedUrl) {
            window.location.href = selectedUrl;
        }
    });

    //Reveal tickets list
    $('#orderTableAdmin tbody').on('click', 'td.details-control button', async function () {
        const tr = $(this).closest('tr');
        const row = dataTable.row(tr);

        const rowData = row.data();
        const orderId = rowData.id; 

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        } else {
            try {
                const tickets = await $.getJSON(`/Admin/Orders/Index?handler=TicketsByOrder&orderId=${orderId}`);
                let html = tickets.length
                    ? tickets.map(t => `
                  <div class="card p-2 mb-2">
                    Ticket id: ${t.id}; Status: ${t.status}; Price: ${t.ticketPrice}; Discount type: ${t.discountType}
                  </div>
                `).join('')
                    : '<p class="m-2">No tickets.</p>';

                row.child(`<div class="tickets-container">${html}</div>`).show();
                tr.addClass('shown');
            } catch {
                row.child('<p class="text-danger m-2">Error loading tickets.</p>').show();
                tr.addClass('shown');
            }
        }
    });

    return dataTable;
}