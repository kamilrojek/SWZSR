$(document).ready(function () {
    $('ul.orderListAdmin > li a.more').click(function () {
        $(this).parent().parent().parent().toggleClass('active');
    });
});