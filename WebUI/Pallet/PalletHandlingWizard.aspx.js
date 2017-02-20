$(document).ready(function() {
    $('#deHireRow').hide();

    if ($('input:radio[id*=rdCollectionTimedBooking]')[0].checked == true)
        $('tr[id*=trCollectBy]').hide();

    if ($('input:radio[id*=rdCollectionIsAnytime]')[0].checked == true)
        $('tr[id*=trCollectBy]').hide();

    if ($('input:radio[id*=rdCollectionBookingWindow]')[0].checked == true)
        $('tr[id*=trCollectBy]').show();

    if ($('input:radio[id*=rdDeliveryTimedBooking]')[0].checked == true)
        $('tr[id*=trDeliverFrom]').hide();

    if ($('input:radio[id*=rdDeliveryIsAnytime]')[0].checked == true)
        $('tr[id*=trDeliverFrom]').hide();

    if ($('input:radio[id*=rdDeliveryBookingWindow]')[0].checked == true)
        $('tr[id*=trDeliverFrom]').show();
});

function rcbPalletHandlingAction_OnClientSelectedIndexChanged(sender, eventArgs) {
    var deHireRow = $('#deHireRow');

    if (sender.get_value() == dehirePalletTypeID)
        deHireRow.show();
    else
        deHireRow.hide();
}

function displayPalletDelivery()
{
    var divPDContainer = $('#divPDContainer');

    if(divPDContainer.css("display") == "none")
        divPDContainer.show();
    else
        divPDContainer.hide();
}

function createPalletReturnRun()
{
    showLoading('Creating Pallet Return Run');
    PageMethods.CreatePalletDeliveryRun(userName, userID, createPalletReturnRun_Success, createPalletReturnRun_Failure);
    return false;
}

function createPalletReturnRun_Success(result) {
    hideLoading();
    
    var parts = result.toString().split("|");

    if (parts.length > 1) {
        // error
        alert("There was and error");
    }
    else {
        loadRunWindow(parts[0]);
        window.close();
    }
}

function createPalletReturnRun_Failure(error) {
    hideLoading();
    alert(error.get_message());
}

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