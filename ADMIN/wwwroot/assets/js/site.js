$('#BaseProjectSelect').change(function () {
    var dataToSend = {
        MenuSelect: $(this).val()
    };
    $.ajax({
        type: 'POST',
        url: '/Home/ActiveMenuSelect',
        data: dataToSend,
        success: function (result) {
            window.location.href = '/';
        },
        error: function (error) {
            window.location.href = '/';
        }
    });
});

