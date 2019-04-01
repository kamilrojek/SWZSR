$(document).ready(function () {
    var sum = 0;
    var selectedDate = new Date();
    var maxDays = 0;
    $('.services-list span').each(function () {
        var txt = $(this).text();
        sum += parseInt(txt);
    });
    $('#service-sum').html(sum);
    $('#service-sum-input').val(sum);

    $('select#data_dost').change(function () {
        selectedDate = $('select#data_dost option:selected').val();
        calcDays();
    });

    /* -----# WYBIERANIE USŁUG #--------- */
    $('select.uslugi-dropdown').change(function () {
        var selectId = $(this).attr("data-select-id");
        var price = $('option:selected', this).attr('data-price');
        var days = $('option:selected', this).attr('data-date');
        var serviceId = $('option:selected', this).val();
        var toAppend = '<li data-date="' + days + '" data-price="' + price + '" data-id="' + serviceId + '">' + $('option:selected', this).text() + ' <span>' + price + '</span></li>';

        $(this).parent().siblings('ul.services-list').append(toAppend);
        appendInputs(selectId);
        calcDays();
        sum += parseInt(price);
        $('#service-sum').html(sum);
        $('#service-sum-input').val(sum);
    });
    $('.services-list').on('click', 'li', function () {
        var selectId = $(this).parent().attr("data-select-id");
        var price = $(this).find('span').text();
        sum -= parseInt(price);
        $('#service-sum').html(sum);
        $('#service-sum-input').val(sum);
        $(this).remove();
        appendInputs(selectId);
        calcDays();
    });
    /* -----# WYBIERANIE USŁUG #--------- */

    var calcDays = function () {
        var maxDays = 0;
        var daysArray = [];
        $('.services-list li').each(function () {
            var days = $(this).attr('data-date');
            daysArray.push(parseInt(days));
        });
        if (daysArray.length < 1) {
            maxDays = 0;
        }
        else {
            maxDays = Math.max(...daysArray);
        }
        refreshDate(selectedDate, maxDays);
    };
    calcDays();
    $('#addNewOrderItem').click(function (event) {
        event.preventDefault();
        $('.order-item-container.hidden').first().removeClass('hidden');
        if ($('.order-item-container.hidden').first().length === 0) {
            $('#addNewOrderItem').parent().parent().remove();
        }
    });
});

var refreshDate = function (startDate, days) {
    startDate = new Date(startDate);
    startDate.setDate(startDate.getDate() + days);
    var dd = startDate.getDate().toString();
    var mm = startDate.getMonth();
    mm = (mm + 1).toString();
    var rr = startDate.getFullYear().toString().substr(-2);
    if (mm.length === 1) {
        mm = "0" + mm;
    }
    if (dd.length === 1) {
        dd = "0" + dd;
    }
    $('#service-date').html('<span>' + dd + '.' + mm + '</span>.' + rr);
    $('#service-date-input').val(dd + '-' + mm + '-' + rr);
};

var appendInputs = function (orderItemNumber) {
    var servicesList = $('.services-list.services-list-Id-' + orderItemNumber);
    var servicesInputs = $('.services-inputs-Id-' + orderItemNumber);
    servicesInputs.empty();
    var servicesCount = servicesList.find('li').length;

    for (var i = 0; i < servicesCount; i++) {
        var serviceId = servicesList.find('li').eq(i).attr('data-id');
        var unitPrice = servicesList.find('li').eq(i).attr('data-price');
        servicesInputs.append('<input type="hidden" name="Order.OrderItems[' + orderItemNumber + '].OrderItemServices[' + i + '].ServiceId" value="' + serviceId + '">');
        servicesInputs.append('<input type="hidden" name="Order.OrderItems[' + orderItemNumber + '].OrderItemServices[' + i + '].UnitPrice" value="' + unitPrice + '">');
    }

};
