var dataTable;
var selectedUrl;

const filtersState = {
    name: null,
    description: null,
    startDate: null,
    endDate: null,
    startTime: null,
    endTime: null,
    duration: null,
    minPrice: null,
    maxPrice: null,
    ticketsAvailable: null
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
$('#eventTicketsAvailable').on('change', function () {
    updateFilter('ticketsAvailable', $(this).val());
});



$(document).ready(function () {
    dataTable = loadDataTable();

    $('#applyFilters').on('click', function (e) {
        e.preventDefault();
        const query = getQueryStringFromFilters();
        dataTable.ajax.url('/Events?handler=AllEvents&' + query).load();
    });
});

function loadDataTable() {
    dataTable = $('#eventTableClient').DataTable({
        "ajax": {
            url: '/Events?handler=AllEvents',
            "type": "GET",
            "dataType": "json",
            dataSrc: function (json) {
                return json.objEventsList;
            }
        },
        "columns": [
            { data: 'name' },
            { data: 'description' },
            {
                data: 'endDate',
                render: function (data) {
                    if (!data) return '';
                    const date = new Date(data);
                    return date.toLocaleDateString(); // Example: "19.05.2025"
                }
            },
            {
                data: null, // null => all parametrs
                render: function (data) {
                    if (!data.startDate || !data.endDate) return '';
                    const start = new Date(data.startDate);
                    const end = new Date(data.endDate);

                    // Get time (HH:MM)
                    const formatTime = date => date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

                    return `${formatTime(start)} – ${formatTime(end)}`;
                }
            },
            { data: 'price' },
            {
                data: 'price',
                render: function (data) {
                    const lowerPrice = data * 0.5;
                    return lowerPrice;
                }
            },
            {
                data: 'price',
                render: function (data) {
                    const lowerPrice = data * 0.75;
                    return lowerPrice;
                }
            },
            { data: 'maxTickets' },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Tickets?eventId=${data}" class="btn btn-primary">
                    Buy <i class="bi bi-arrow-right-circle"></i>
                    </a>`;
                }
            },
        ],
        order: [[0, 'asc']]
    });

    return dataTable;

}
