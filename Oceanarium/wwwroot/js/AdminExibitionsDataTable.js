var dataTable;
var selectedUrl;

const filtersState = {
    id: null,
    name: null,
    description: null,
    isPermanent: null,
    startDate: null,
    endDate: null

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
$('#filterExibitionId').on('input', function () {
    updateFilter('id', $(this).val());
});

$('#exibitionName').on('input', function () {
    updateFilter('name', $(this).val());
});

$('#exibitionDescription').on('input', function () {
    updateFilter('description', $(this).val());
});

$('#exibitionIsPermanent').on('change', function () {
    const val = $(this).val();
    updateFilter('isPermanent', val === '' ? null : val); 
});

$('#exibitionStartDate').on('change', function () {
    updateFilter('startDate', $(this).val());
});

$('#exibitionEndDate').on('change', function () {
    updateFilter('endDate', $(this).val());
});


$(document).ready(function () {
    dataTable = loadDataTable();

    $('#applyFilters').on('click', function (e) {
        e.preventDefault();
        const query = getQueryStringFromFilters();
        dataTable.ajax.url('/Admin/Exibitions/Index?handler=AllExibitions&' + query).load();
    });
});

function loadDataTable() {
    dataTable = $('#exibitionTableAdmin').DataTable({
        "ajax": {
            url: '/Admin/Exibitions/Index?handler=AllExibitions',
            "type": "GET",
            "dataType": "json",
            dataSrc: function (json) {
                return json.objExibitionsList;
            }
        },
        "columns": [
            { data: 'id' },
            { data: 'name' },
            { data: 'description' },
            {
                data: 'isPermanent',
                "render": function (data) {
                    return data
                        ? '<i class="bi bi-check-circle-fill text-success"></i>'
                        : '<i class="bi bi-x-circle-fill text-danger"></i>';
                }
            },
            { data: 'startDate' },
            { data: 'endDate' },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Exibitions/Edit?exibitionId=${data}" class="btn btn-outline-secondary edit-button">
                                <i class="bi bi-pencil-square"></i>
                                Edit
                            </a>`;
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a href="/Admin/Exibitions/Index?handler=DeleteExibition&exibitionId=${data}" class="btn btn-outline-secondary delete-link">
                                <i class="bi bi-trash"></i>
                                Delete
                            </a>`;
                }
            }
        ],
        order: [[0, 'asc']]
    });

    //Confirmation modal delete
    $('#exibitionTableAdmin tbody').on('click', 'a.delete-link', function (e) {
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
