$(document).ready(function () {
    var sum = 0;
    $('.services-list span').each(function () {
        var txt = $(this).text();
        sum += parseInt(txt);
    });
    $('#service-sum').html(sum);
    $('#service-sum-input').val(sum);

    /* -----# WYBIERANIE USŁUG #--------- */
    $('select.uslugi-dropdown').change(function () {
        var selectId = $(this).attr("data-select-id");
        var price = $('option:selected', this).attr('data-price');
        var serviceId = $('option:selected', this).val();

        var divToAppend = '<div class="row align-items-center" style="margin: 15px -15px; background: #fff;"><div class="col-5">' + $('option:selected', this).text() +
            '</div><div class="col-4 data-price"><input type="text" value="' + price + '" data-id="' + serviceId + '"  data-selectid="' + selectId + '"data-text="' + $('option:selected', this).text() + '"></div><div class="col-3"><a href="#" class="btn btn-primary">DODAJ</a></div></div>';
        $(this).parent().siblings('ul.services-list').append(divToAppend);
        
    });

    $('.services-list').on('click', 'a', function (e) {
        e.preventDefault();
        console.log('elo');
        var serviceId = $(this).parent().siblings('.data-price').children('input').attr('data-id');
        var selectId = $(this).parent().siblings('.data-price').children('input').attr('data-selectid');
        var price = $(this).parent().siblings('.data-price').children('input').val();
        var text = $(this).parent().siblings('.data-price').children('input').attr('data-text');

        var toAppend = '<li data-price="' + price + '" data-id="' + serviceId + '">' + text + ' <span>' + price + '</span></li>';
        $(this).parent().parent().parent().append(toAppend);
        appendInputs(selectId);
        
        sum += parseInt(price);
        $('#service-sum').html(sum);
        $('#service-sum-input').val(sum);

        $(this).parent().parent().remove();
    });

    $('.services-list').on('click', 'li', function () {
        var selectId = $(this).parent().attr("data-select-id");
        var price = $(this).find('span').text();
        sum -= parseInt(price);
        $('#service-sum').html(sum);
        $('#service-sum-input').val(sum);
        $(this).remove();
        appendInputs(selectId);

    });
    /* -----# WYBIERANIE USŁUG #--------- */

    $('#addNewOrderItem').click(function (event) {
        event.preventDefault();
        $('.order-item-container.hidden').first().removeClass('hidden');
        if ($('.order-item-container.hidden').first().length === 0) {
            $('#addNewOrderItem').parent().parent().remove();
        }
    });
});

var appendInputs = function (orderItemNumber) {
    var servicesList = $('.services-list.services-list-Id-' + orderItemNumber);
    var servicesInputs = $('.services-inputs-Id-' + orderItemNumber);
    servicesInputs.empty();
    var servicesCount = servicesList.find('li').length;

    for (var i = 0; i < servicesCount; i++) {
        var serviceId = servicesList.find('li').eq(i).attr('data-id');
        var unitPrice = servicesList.find('li').eq(i).attr('data-price');
        servicesInputs.append('<input type="hidden" name="OrderItem.OrderItemServices[' + i + '].ServiceId" value="' + serviceId + '">');
        servicesInputs.append('<input type="hidden" name="OrderItem.OrderItemServices[' + i + '].UnitPrice" value="' + unitPrice + '">');
    }

};
