
function toggleDateFields() {
    const isPermanent = document.getElementById('permanentOption').checked;
    const startDate = document.getElementById('newExibition_StartDate');
    const endDate = document.getElementById('newExibition_EndDate');

    startDate.disabled = isPermanent;
    endDate.disabled = isPermanent;
}

document.addEventListener('DOMContentLoaded', function () {
    const permanentOption = document.getElementById('permanentOption');
    const temporaryOption = document.getElementById('temporaryOption');

    permanentOption.addEventListener('change', toggleDateFields);
    temporaryOption.addEventListener('change', toggleDateFields);

    toggleDateFields();
});
