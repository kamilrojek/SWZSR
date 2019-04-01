$(document).ready(function () {
    $('.myorder-row div.long-to-short').click(function () {
        $(this).text($(this).text() === 'ROZWIŃ' ? 'ZWIŃ' : 'ROZWIŃ');
        $(this).parent().parent().toggleClass('short');
    });
});