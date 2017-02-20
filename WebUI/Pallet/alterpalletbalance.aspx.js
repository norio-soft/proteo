$(document).ready(function() {
    $('#tabs').tabs();

    if ($('input:radio[id*=rdDeliveryTimedBooking]')[0].checked == true)
        $('tr[id*=trDeliverFrom]').hide();

    if ($('input:radio[id*=rdDeliveryIsAnytime]')[0].checked == true)
        $('tr[id*=trDeliverFrom]').hide();

    if ($('input:radio[id*=rdDeliveryBookingWindow]')[0].checked == true)
        $('tr[id*=trDeliverFrom]').show();
});

// Block UI
//#region 
function showLoading(messageContent) {
    $.blockUI({
        message: '<div style="margin-left:30px;"><span id="UpdatableMessage">' + messageContent + '</span></div>',
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: '.5',
            color: '#fff'
        }
    });
}

function updateLoadingMessage(messageContent) {
    $('#UpdatableMessage').text(messageContent);
}

function hideLoading() {
    $.unblockUI();
}