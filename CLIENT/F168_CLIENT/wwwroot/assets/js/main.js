$(document).ready(function () {
    $('.icon-close').on('click', function () {
        $('.content-popup').hide();
        showTab('tab1-tab');
        $('.tab-content').addClass('active');
    });

    $('.btn-content').each(function () {
        var _this = $(this);
        _this.on('click', function () {
            var tabIdControl = $(this).data('tab-control');

            $('#show-iframe-open .code-iframe').each(function () {
                var tabContentId = $(this).data('content-tab');

                if (tabIdControl == tabContentId) {

                    $(this).addClass('active');
                    $('#box-iframe').show();
                    $('#btn-tawk-small').show();
                } else {
                    $(this).removeClass('active');
                }
            });
        });
    });

    $(".close-customer").click(function () {
        $("#imageModal").modal('hide');
    });

    $('#btn-tawk-small').hide();
    $('#box-iframe').hide();

    $('#btn-tawk-small').on('click', function () {
        $(this).hide();
        $('#box-iframe').hide();
    });

    $('#draggable .chat-dialog').css('display', 'none');
    $('#draggable .chat-dialog').each(function (index, value) {
        if ($(this).hasClass('active')) {
            $(this).css('display', 'block');
            $(this).parent().hide();
        }
    });

    $('#recaptcha').text(Math.floor(1000 + Math.random() * 9000));
    $('#c-captcha').on('click', function () {
        $('#recaptcha').text(Math.floor(1000 + Math.random() * 9000));
    });
    /*$(document).ready(function () {
        $(document).scroll(function () {
            if ($(document).scrollTop() >= 200) {
                $('.menu-fix-sp').addClass('fixed');
            } 
            else {
                $('.menu-fix-sp').removeClass('fixed');
            }
        });
    });*/
});


