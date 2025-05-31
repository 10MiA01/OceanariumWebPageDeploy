//Cancellation functions

document.addEventListener('DOMContentLoaded', function () {
    let selectedForm = null;

    let cancelButtons = document.querySelectorAll('.cancel-button');
    if (cancelButtons.length > 0) {
        cancelButtons.forEach(button => {
            button.addEventListener('click', function () {
                selectedForm = this.closest('form');
                var myModal = new bootstrap.Modal(document.getElementById('confirmCancelModal'));
                myModal.show();
            });
        });
    }

    let confirmCancelButton = document.getElementById('confirmCancelButton');
    if (confirmCancelButton) {
        confirmCancelButton.addEventListener('click', function () {
            if (selectedForm) {
                selectedForm.submit(); 
            }
        });
    }
});
