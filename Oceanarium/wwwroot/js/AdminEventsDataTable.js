var dataTable;
var selectedUrl;

const filtersState = {
    id: null,
    name: null,
    description: null,
    startDate: null,
    endDate: null,
    startTime: null,
    endTime: null,
    duration: null,
    minPrice: null,
    maxPrice: null,
    ticketsMaxMin: null,
    ticketsMaxMax: null,
    ticketsAvailable: null,
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


//Filters 
$('#filterEventId').on('input', function () {
    updateFilter('id', $(this).val());
});

$('#eventName').on('input', function () {
    updateFilter('name', $(this).val());
});

$('#eventDescription').on('input', function () {
    updateFilter('description', $(this).val());
});

$('#eventStartDate').on('change', function () {
    updateFilter('startDate', $(this).val());
});
$('#eventEndDate').on('change', function () {
    updateFilter('endDate', $(this).val());
});

$('#eventStartTime').on('change', function () {
    updateFilter('startTime', $(this).val());
});

$('#eventEndTime').on('change', function () {
    updateFilter('endTime', $(this).val());
});
$('#eventDuration').on('change', function () {
    updateFilter('duration', $(this).val());
});
$('#eventMinPrice').on('change', function () {
    updateFilter('minPrice', $(this).val());
});
$('#eventMaxPrice').on('change', function () {
    updateFilter('maxPrice', $(this).val());
});
$('#eventTicketsMaxMin').on('change', function () {
    updateFilter('ticketsMaxMin', $(this).val());
});
$('#eventTicketsMaxMax').on('change', function () {
    updateFilter('ticketsMaxMax', $(this).val());
});
$('#eventTicketsAvailable').on('change', function () {
    updateFilter('ticketsAvailable', $(this).val());
});
$('#eventStatus').on('change', function () {
    updateFilter('status', $(this).val());
});


$(document).ready(function () {
    dataTable = loadDataTable();

    $('#applyFilters').on('click', function (e) {
        e.preventDefault();
        const query = getQueryStringFromFilters();
        dataTable.ajax.url('/Admin/Events/Index?handler=AllEvents&' + query).load();
    });
});

function loadDataTable() {
    dataTable = $('#eventTableAdmin').DataTable({
        "ajax": {
            url: '/Admin/Events/Index?handler=AllEvents',
            "type": "GET",
            "dataType": "json",
            dataSrc: function (json) {
                return json.objEventsList;
            }
        },
        "columns": [
            { data: 'id' },
            { data: 'name' },
            { data: 'description' },
            { data: 'startDate' },
            { data: 'endDate' },
            { data: 'price' },
            { data: 'maxTicketsDefault' },
            { data: 'maxTickets' },
            { data: 'status' },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/tickets/Index?eventId=${data}" class="btn btn-outline-secondary">
                                <i class="bi bi-ticket"></i> Tickets manage
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Events/Edit?eventId=${data}" class="btn btn-outline-secondary edit-button">
                                <i class="bi bi-pencil-square"></i>
                                Edit
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Events/Index?handler=DeleteEvent&eventId=${data}" class="btn btn-outline-secondary delete-link">
                                <i class="bi bi-trash"></i>
                                Delete
                            </a>`;
                }
            }
        ],
        order: [[0, 'asc']]
    });

    //Confirmation modal delete
    $('#eventTableAdmin tbody').on('click', 'a.delete-link', function (e) {
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