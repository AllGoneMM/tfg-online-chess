window.onload = function () {
    if ($('#errorMessage').text() === "") {
        $('#errorMessageDiv').removeClass("d-block").addClass("d-none");
    } else {
        $('#errorMessageDiv').removeClass("d-none").addClass("d-block");
    }
};

$(document).ready(function () {
    $('form').on('submit', function (event) {
        var form = $(this);

        if (!form.valid()) {
            // Hide server-side error message div if client-side validation fails
            $('#errorMessageDiv').removeClass("d-block").addClass("d-none");
            $('#errorMessage').text('');
        }
    });
});