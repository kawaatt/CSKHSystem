//$("#box-iframe").draggable();

//function draggable() {
//    $("#box-iframe").draggable();
//}

//$(function () {
//    draggable();
//});

// Hàm để tạo số ngẫu nhiên không trùng nhau
function generateUniqueRandomNumbers(count, min, max) {
    if (count > max - min + 1 || max < min) {
        return null; // Không đủ số để chọn hoặc min > max
    }

    const uniqueNumbers = new Set();

    while (uniqueNumbers.size < count) {
        const randomNumber = Math.floor(Math.random() * (max - min + 1)) + min;
        uniqueNumbers.add(randomNumber);
    }

    return Array.from(uniqueNumbers);
}

const randomNumbers = generateUniqueRandomNumbers(4, 90, 100);
// Gán số vào thẻ <p> với class "random-number" và kích hoạt animation
if (randomNumbers) {
    const elements = document.getElementsByClassName("number");

    for (let i = 0; i < elements.length; i++) {
        elements[i].innerText = 0; // Đặt giá trị ban đầu là 0
        elements[i].style.opacity = 1; // Hiển thị số

        // Sử dụng setTimeout để tạo độ trễ giữa các số
        animateNumber(elements[i], randomNumbers[i]);
    }
}
// Số ngẫu nhiên từ 90 đến 100
function checkSpeed(i) {
    const randomNumbers = generateUniqueRandomNumbers(4, 90, 100);
    // Gán số vào thẻ <p> với class "random-number" và kích hoạt animation
    if (randomNumbers) {
        const elements = document.getElementsByClassName("number");

        for (let i = 0; i < elements.length; i++) {
            elements[i].innerText = 0; // Đặt giá trị ban đầu là 0
            elements[i].style.opacity = 1; // Hiển thị số

            // Sử dụng setTimeout để tạo độ trễ giữa các số
            animateNumber(elements[i], randomNumbers[i]);
        }
    }

    function processSvgItems(selector) {
        const svgItems = document.querySelectorAll(selector);
        svgItems.forEach((item, index) => {
            item.style.fill = "#e8e8e8";
            const rotateAngle = (index + 1) * 7;
            let fillColor;

            if (rotateAngle < 300) {
                fillColor = "#FC8B00";
            } else if (rotateAngle > 800) {
                fillColor = "#FC8B00";
            } else {
                fillColor = "#FFFFFF";
            }

            setTimeout(() => {
                item.style.fill = fillColor;
                item.parentElement.style.animation = "all 1s ease-in";
            }, index * 40);
        });
    }

    processSvgItems(".path-svg");
    processSvgItems(".path-svg2");
    processSvgItems(".path-svg3");
    processSvgItems(".path-svg4");
}

// Hàm để thực hiện animation số từ 0 đến giá trị được gán
function animateNumber(element, targetNumber) {
    let currentNumber = 0;

    const interval = setInterval(() => {
        element.innerText = currentNumber;

        if (currentNumber >= targetNumber) {
            clearInterval(interval);
        } else {
            currentNumber += Math.ceil((targetNumber - currentNumber) / 10);
        }
    }, 50); // Mỗi 50ms cập nhật một lần
}

checkSpeed();


$(".form-modal-layout").on("click", function () {
    $(this).parent('.form-modal').hide();
});

$(".wp-close").on("click", function () {
    $(this).closest('.form-modal').hide();
});

$(".btn-check-acc").on("click", function () {
    $('#c-account').val("");
    $('#c-cardholder').val("");
    $('#c-captcha').val("");
    $('.form-check-ticket').show();
});

$("#btnSendNewRequestTicket").on("click", function () {
    var account = localStorage.getItem('Account');
    var cardholder = localStorage.getItem('Cardholder');
    $('#el-image-upload img').remove();
    $('#SendTicketRequest_Account').val(account);
    $('#SendTicketRequest_CardHolder').val(cardholder);
    $('#SendTicketRequest_TicketContent').val('');
    $('.form-check-ticket').hide().fadeOut();
    $('.form-add-new-ticket').show().fadeIn();
});


//$(".btn-check-result").on("click", function () {
//    $('.form-check-result').show();
//});

$(".btn-check-again").on("click", function () {
    $('.form-check-result').hide();
    $('.form-check-ticket').show();
});

$(".btn-check-new").on("click", function () {
    $('.form-check-result').hide();
    $('.form-check-ticket').show();
});

//$(".btnCheckDetailTicket").on("click", function () {
//    alert('abcd');
//});

$(document).on("click", ".btnCheckDetailTicket", function () {
    var dataToSend = {
        TicketID : $(this).data('id')
    };
    $.ajax({
        type: 'POST',
        url: '/Home/GetTicketByID',
        data: dataToSend,
        dataType: 'json',
        success: function (data) {
            if (data.isSuccess == true) {
                var ticketProcessing = `<div class="title-history">HỖ TRỢ - ${data.result.ticketCategory.categoryName}</div>`;
                ticketProcessing += `<div class="wp-form"><div class="wp-history-ct"><div class="history-ct-left">`;

                //Print ticket request info
                updateDate = moment(data.result.requestDate);
                ticketProcessing += `<div class="history-item active">`;
                ticketProcessing += `<div class="datetime">
                                        <span class="date">${updateDate.format('DD/MM/YYYY')}</span>
                                        <span class="time">${updateDate.format('HH:mm:ss')}</span>
                                    </div>
                                    <div class="line-history">
                                        <div class="line"></div>
                                        <div class="icon-history"></div>
                                    </div>
                                    <div class="history-content">
                                        <div class="year">Hệ thống đã tiếp nhận yêu cầu hỗ trợ
                                    </div>`;
                if (data.result.requestContent !== undefined) {
                    ticketProcessing += `<div class="title">Nội dung: ${data.result.requestContent}</div>
                                    </div>`;
                }
                
                ticketProcessing += `</div>`;
                ticketProcessing += `</div></div></div>`;

                //Print ticket process history
                for (var i = 0; i < data.result.ticketHistories.length; i++) {
                    const updateDate = moment(data.result.ticketHistories[i].updateTime);
                    var StatusText = `<div class="year">${data.result.ticketHistories[i].ticketStatusTitle}</div>`;
                    if (data.result.ticketHistories[i].ticketStatusTitle === "Hoàn thành hỗ trợ") {
                        StatusText = `<div class="year" style="color:#00CF5A;" >Hoàn thành hỗ trợ</div>`;
                    }
                    ticketProcessing += `<div class="history-item active">
                                        <div class="datetime">
                                            <span class="date">${updateDate.format('DD/MM/YYYY')}</span>
                                            <span class="time">${updateDate.format('HH:mm:ss')}</span>
                                        </div>
                                        <div class="line-history">
                                            <div class="line"></div>
                                            <div class="icon-history"></div>
                                        </div>
                                        <div class="history-content">
                                            ${StatusText}
                                            <div class="title">${data.result.ticketHistories[i].ticketStatusDescription}</div>
                                        </div>`;
                    ticketProcessing += `</div>`;
                }

                

                $('#result-check').html(ticketProcessing);
                $('.form-check-ticket').hide().fadeOut();
                $('.form-check-result').show().fadeIn();
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Lỗi",
                    text: data.message,
                    confirmButtonColor: "#3085d6",
                    confirmButtonText: "Đồng ý",
                })
            }
        },
        error: function (error) {
            toastr.error(JSON.stringify(error));
        }
    });
});

function checkLength(input) {
    if (input.value.length > 4) {
        input.value = input.value.slice(0, 4);
    }
}

function CheckAccountForClaimReward() {
    var account = $('#c-account').val();
    var cardholder = $('#c-cardholder').val();
    var iCaptcha = $('#c-captcha').val();
    var reCaptcha = $('#recaptcha').text();

    if (cardholder.length !== 4) {
        Swal.fire({
            icon: "error",
            title: "Cập nhật thông tin",
            text: "4 số cuối tài khoản ngân hàng chưa hợp lệ",
            confirmButtonColor: "#3085d6",
            confirmButtonText: "Đồng ý",
        })
        return;
    }

    if (account === "" || cardholder === "" || iCaptcha === "") {
        Swal.fire({
            icon: "error",
            title: "Cập nhật thông tin",
            text: "Quý khách vui lòng điền đầy đủ thông tin",
            confirmButtonColor: "#3085d6",
            confirmButtonText: "Đồng ý",
        })
        return;
    }

    if (iCaptcha.toString() !== reCaptcha.toString()) {
        Swal.fire({
            icon: "error",
            title: "Xác thực thất bại",
            text: "Quý khách vui lòng nhập đúng mã xác minh",
            confirmButtonColor: "#3085d6",
            confirmButtonText: "Đồng ý",
        });
        return;
    }

    var dataToSend = {
        Account: account,
        CardHolder: cardholder
    };

    $.ajax({
        type: 'POST',
        url: '/Home/SendCheckAccount',
        data: dataToSend,
        dataType: 'json',
        success: function (data) {
            var ticketList = data.result;
            if (data.isSuccess == false) {
                Swal.fire({
                    icon: "error",
                    title: "Xác thực thất bại",
                    text: "Thông tin xác thực tài khoản chưa chính xác",
                    confirmButtonColor: "#3085d6",
                    confirmButtonText: "Đồng ý",
                });
                return;
            }
            var htmlDom = ``;
            for (var i = ticketList.length-1; i>=0; i--) {
                const requestDate = new Date(ticketList[i].requestDate);
                requestDate.setSeconds(requestDate.getSeconds() + 1); // Add one second

                const formattedDate = requestDate.toLocaleDateString('en-US', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    second: '2-digit', // Include seconds in the formatted date
                    hour12: false // Use 24-hour format
                });

                const ticketHistories = ticketList[i].ticketHistories;
                let iTicketStatus = 0;
                if (ticketHistories.length!==0) {
                    if (ticketHistories.some(ticketValue => ticketValue.ticketStatusValue === 3)) {
                        iTicketStatus = 2;
                    } else {
                        iTicketStatus = 1;
                    }
                }

                
                if (iTicketStatus === 1) {
                    statusText = `<button class='btnProcessProcessing btnCheckDetailTicket' data-id='${ticketList[i].id}'>Đang xử lý</button>`;
                } else {
                    if (iTicketStatus === 2) {
                        statusText = `<button class='btnProcessSuccess btnCheckDetailTicket' data-id='${ticketList[i].id}'>Hoàn thành</button>`;
                    } else {
                        var statusText = `<button class='btnProcessPending btnCheckDetailTicket' data-id='${ticketList[i].id}'>Chờ xử lý</button>`;
                    }
                }

                htmlDom += `<div class="ticket-request-item">
                                <div class="left-column">
                                    <p>ID - ${ticketList[i].id}</label>
                                    <p>Chủ đề - ${ticketList[i].ticketCategory.categoryName}</label>
                                    <p>Thời gian - ${formattedDate}</label>
                                </div>
                                <div class="right-column">${statusText}</div>
                            </div>`;
                $('.ticket-request-list').html(htmlDom);
            }

            localStorage.setItem("Account", account);
            localStorage.setItem("Cardholder", cardholder);

            $('.form-check-ticket').hide().fadeOut();
            $('.form-check-list-ticket').show().fadeIn();

        },
        error: function (error) {
            toastr.error(JSON.stringify(error));
        }
    });
}


const previews = [];
document.getElementById("fileupload").addEventListener("change", function (e) {
    const files = e.target.files;
    const errors = [];
    let count = 0;
    let htmlString = "";
    const elImage = document.getElementById('el-image-upload');

    // Clear previous previews
    previews.length = 0;

    Array.from(files).forEach(file => {
        const reader = new FileReader();
        if (file && file.type.startsWith("image/") && file.size <= 5000000) {
            // <3MB
            if (count < 2) {
                reader.onloadend = function () {
                    previews.push(reader.result);
                    errors.push("");
                    updateImageDisplay();
                };

                count++;
                reader.readAsDataURL(file);
            } else {
                console.log("File limit exceeded");
                Swal.fire({
                    icon: "error",
                    title: "Thao tác thất bại !",
                    text: "Bạn vui lòng tải lên nhiều nhất là 2 hình ảnh !",
                    showCloseButton: true,
                    cancelButtonText: `<i class="fa fa-thumbs-down"></i>`,
                    confirmButtonText: "Thử lại",
                    footer: '<a href="#" rel="nofollow" target="_blank">Chăm sóc khách hàng 24/7</a>'
                });
            }
        } else {
            errors.push("Vui lòng chọn một ảnh có định dạng hợp lệ và kích thước không quá 5MB.");
        }
    });

    // Check for errors
    if (errors.length === files.length) {
        console.log("Errors: ", errors);
    }

    function updateImageDisplay() {
        htmlString = "";
        for (let i = 0; i < previews.length; i++) {
            htmlString += `<img id="SendTicketRequest_ImageBase64" name="SendTicketRequest_ImageBase64" style="width:auto; height:auto; max-width:400px;max-height:400px;" src="${previews[i]}" />`;
        }

        elImage.innerHTML = htmlString;

        if (elImage.children.length > 0) {
            document.getElementsByClassName('upload')[0].style.display = "none";
        }
    }
});

function SendTicketRequest() {
    
    var dataToSend = {
        account: $('#SendTicketRequest_Account').val(),
        ticketContent: $("#SendTicketRequest_TicketContent").val(),
        cardHolder: $("#SendTicketRequest_CardHolder").val(),
        ticketCategoryID: $("#SendTicketRequest_TicketCategory").val(),
        imageBase64: $("#SendTicketRequest_ImageBase64").attr("src")
    };

    if (dataToSend.ticketContent === "" ||
        dataToSend.account === "" ||
        dataToSend.cardHolder === "") {
        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: "Vui lòng nhập đầy đủ thông tin",
            confirmButtonColor: "#3085d6",
            confirmButtonText: "OK"
        });
        return;
    }
    $("#btnSendNewTicketRequest").text(`ĐANG GỬI...`).attr("disabled", true);

    $.ajax({
        type: 'POST',
        url: '/Home/SendTicketRequest',
        data: dataToSend,
        dataType: 'json',
        success: function (data) {
            $("#btnSendNewTicketRequest").text(`GỬI`).attr("disabled", false);
            if (data.isSuccess) {
                Swal.fire({
                    icon: "success",
                    title: "Thành công",
                    text: "Yêu cầu đã được gửi đến hệ thống.",
                    confirmButtonColor: "#3085d6",
                    confirmButtonText: "OK",
                }).then((result) => {
                    $('.form-check-ticket').hide().fadeOut();
                    $('.form-check-list-ticket').hide().fadeOut();
                    $('.form-add-new-ticket').hide().fadeOut();
                    BackToCheckTicketList($('#SendTicketRequest_Account').val(), $("#SendTicketRequest_CardHolder").val()); 
                });
            } else {
                Swal.fire({
                    icon: "error",
                    title: "Lỗi",
                    text: data.message,
                    confirmButtonColor: "#3085d6",
                    confirmButtonText: "OK",
                }).then((result) => {
                    $('.form-check-ticket').hide().fadeOut();
                    $('.form-check-list-ticket').hide().fadeOut();
                    $('.form-add-new-ticket').hide().fadeOut();
                    BackToCheckTicketList($('#SendTicketRequest_Account').val(), $("#SendTicketRequest_CardHolder").val()); 
                });
            }
        },
        error: function (error) {
            toastr.error(JSON.stringify(error));
        }
    });
}

function BackToCheckTicketList(account, cardholder) {
    
    var dataToSend = {
        Account: account,
        CardHolder: cardholder
    };

    $.ajax({
        type: 'POST',
        url: '/Home/SendCheckAccount',
        data: dataToSend,
        dataType: 'json',
        success: function (data) {
            var ticketList = data.result;
            var htmlDom = ``;
            for (var i = ticketList.length - 1; i >= 0; i--) {
                const requestDate = new Date(ticketList[i].requestDate);
                requestDate.setSeconds(requestDate.getSeconds() + 1); // Add one second

                const formattedDate = requestDate.toLocaleDateString('en-US', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',
                    second: '2-digit', // Include seconds in the formatted date
                    hour12: false // Use 24-hour format
                });

                const ticketHistories = ticketList[i].ticketHistories;
                let isProcessingSuccessful = false;

                for (let j = 0; j < ticketHistories.length; j++) {
                    if (ticketHistories[j].ticketStatusValue === 2) {
                        isProcessingSuccessful = true;
                        break;
                    }
                }

                var statusText = `<button class='btnProcessSuccess btnCheckDetailTicket' data-id='${ticketList[i].id}'>Chi Tiết</button>`;
                if (isProcessingSuccessful === false) {
                    statusText = `<button class='btnProcessPending btnCheckDetailTicket' data-id='${ticketList[i].id}'>Chi Tiết</button>`;
                }

                htmlDom += `<div class="ticket-request-item">
                                <div class="left-column">
                                    <p>ID - ${ticketList[i].id}</label>
                                    <p>Chủ đề - ${ticketList[i].ticketCategory.categoryName}</label>
                                    <p>Thời gian - ${formattedDate}</label>
                                </div>
                                <div class="right-column">${statusText}</div>
                            </div>`;
                $('.ticket-request-list').html(htmlDom);
            }

            localStorage.setItem("Account", account);
            localStorage.setItem("Cardholder", cardholder);

            $('.form-check-ticket').hide().fadeOut();
            $('.form-check-list-ticket').show().fadeIn();

        },
        error: function (error) {
            toastr.error(JSON.stringify(error));
        }
    });
}


