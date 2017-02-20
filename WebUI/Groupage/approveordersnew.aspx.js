// Capture Book In
//#region

function BookIn(el, orderID) {
    try {
        var orderID = $(el).data("orderID");
        var bookedInWith = $('#txtBookedInWith').val();
        var bookedInRefs = $('#txtBookedInReferences').val();
        var _dteBookInFromDate = $(dteBookInFromDate).val();
        var _dteBookInFromTime = $(dteBookInFromTime).val();
        var _dteBookInByFromDate = $(dteBookInByFromDate).val();
        var _dteBookInByFromTime = $(dteBookInByFromTime).val();
        var dateOption = $(':radio[name*=BookInTime]:checked').val();

        PageMethods.BookIn(orderID, userName, bookedInWith, bookedInRefs, dateOption, _dteBookInFromDate, _dteBookInFromTime, _dteBookInByFromDate, _dteBookInByFromTime, BookIn_Success, BookIn_Failure)

        $('#bookInWindow').hide();
        $('#' + $(el).data("el")).attr('src', '/images/tick.gif');
        $('#' + $(el).data("el")).attr('alt', "Booked In by " + userName + " with " + bookedInWith + "; references: " + bookedInRefs);
    }
    catch (e) {
        alert(e);
    }

    hideLoading();
}

function ShowBookIn(el, orderID) {
    //getting height and width of the message box
    var pos = findPos(el);
    var height = $('#bookInWindow').height();
    var width = $('#bookInWindow').width();
    //calculating offset for displaying popup message
    //This was giving back a top over over a 1000px causing it to fly off screen. So just used the left pos for both cause its always around 300px
    leftVal = pos[0] + "px";
    topVal = pos[0] + "px";
    //show the popup message and hide with fading effect
    $('#bookInWindow').css({ left: leftVal, top: topVal }).show();
    $('#txtBookedInWith').focus();
    $('#btnSaveBookIn').data("orderID", orderID);
    $('#btnSaveBookIn').data("el", el.id);
}


function BookIn_Success(result) {
    // change the icon on the book in control

    hideLoading();
}

function BookIn_Failure(error) {
    alert(error.get_message());
    hideLoading();
}


//#endregion

// Set Bookin Required
function SetBookInRequired(el, orderID) {
    try {
        //showLoading();

        PageMethods.SetRequiresBookingIn(orderID, el.id, userName, SetRequiresBookingIn_Success, SetRequiresBookingIn_Failure);
    }
    catch (e) {
        alert(e);
    }
    //hideLoading();
}

function SetRequiresBookingIn_Success(result) {
    // Show the Bookin Icon
    $get(result[1]).innerText = "Del";
    $($get(result[1])).removeAttr("onclick");
    $($get(result[1])).attr("onclick", result[2]);
}

function SetRequiresBookingIn_Failure(error) {
    alert(error.get_message());
}

function RemoveBookInRequired(el, orderID) {
    try {
        //showLoading();

        PageMethods.RemoveRequiresBookingIn(orderID, el.id, userName, RemoveRequiresBookingIn_Success, RemoveRequiresBookingIn_Failure);
    }
    catch (e) {
        alert(e);
    }
    //hideLoading();
}

function RemoveRequiresBookingIn_Success(result) {
    $get(result[1]).innerText = "Set";
    $($get(result[1])).closest('tr').find('img[id*=imgOrderBook]').hide();

    $($get(result[1])).removeAttr("onclick");
    $($get(result[1])).attr("onclick", result[2]);
}

function RemoveRequiresBookingIn_Failure(error) {
    alert(error.get_message());
}




function viewOrderProfile(orderID) {
    var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

    var wnd = window.open(url, "OrderProfile", "Width=1180, height=900, scrollbars=1, resizable=1");
}


// page load/init
//#region
$(document).ready(function () {
    $(':radio[id *= rblCollectionTimeOptions]').bind("click", ChangeCollectionTiming);
    $(':radio[id *= rblDeliveryTimeOptions]').bind("click", ChangeDeliveryTiming);
    $(':radio[name*=BookInTime]').bind("click", ChangeBookInTiming);

    $('textarea[id*=txtDeliveryNotes]').bind("change", setDirty);

    $(':text[id*=txtRate]').bind("change", setDirty);

    $(':text[id*=txtCollectByDate]').bind("change", setDirty);
    $(':text[id*=txtCollectByTime]').bind("change", setDirty);
    $(':text[id*=txtCollectAtDate]').bind("change", setDirty);
    $(':text[id*=txtCollectAtTime]').bind("change", setDirty);

    $(':text[id*=txtDeliverByFromDate]').bind("change", setDirty);
    $(':text[id*=txtDeliverByFromTime]').bind("change", setDirty);
    $(':text[id*=txtDeliverAtFromDate]').bind("change", setDirty);
    $(':text[id*=txtDeliverAtFromTime]').bind("change", setDirty);
});

//#endregion

// wire up time handlers
function ChangeCollectionTiming(e) {
    // Get the collect by date and time controls
    var selectedOption = e.srcElement.value;
    var collSpan = $(e.srcElement).closest('div');
    var collDate = $(collSpan).find('input[id*=txtCollectByDate]');
    var collTime = $(collSpan).find('input[id*=txtCollectByTime]');
    var collectionTime = $(collSpan).find('input[id*=txtCollectAtTime]');
    switch (selectedOption) {
        case "0": // Window
            collDate.show();
            collTime.show();
            collectionTime.prop("disabled", false);
            collectionTime.val(collTime.val());
            break;
        case "1": //Timed
            collDate.hide();
            collTime.hide();
            collectionTime.prop("disabled", false);
            collectionTime.val(collTime.val());
            break;
        case "2": // Anytime
            collDate.hide();
            collTime.hide();
            collectionTime.prop("disabled", true);
            collectionTime.val("Anytime");
            break;

    }

    setDirty(e);

}

function ChangeDeliveryTiming(e) {
    // Get the collect by date and time controls
    var selectedOption = e.srcElement.value;
    var delSpan = $(e.srcElement).closest('div');
    var delDate = $(delSpan).find('input[id*=txtDeliverByFromDate]');
    var delTime = $(delSpan).find('input[id*=txtDeliverByFromTime]');
    var deliveryByTime = $(delSpan).find('input[id*=txtDeliverAtFromTime]');
    switch (selectedOption) {
        case "0": // Window
            delDate.show();
            delTime.show();
            deliveryByTime.prop("disabled", false);
            deliveryByTime.val(delTime.val());
            break;
        case "1": //Timed
            delDate.hide();
            delTime.hide();
            deliveryByTime.prop("disabled", false);
            deliveryByTime.val(delTime.val());
            break;
        case "2": // Anytime
            delDate.hide();
            delTime.hide();
            deliveryByTime.prop("disabled", true);
            deliveryByTime.val("Anytime");
            break;

    }

    setDirty(e);
}

function ChangeBookInTiming(e) {
    // Get the collect by date and time controls
    var selectedOption = e.srcElement.value;
    var delSpan = $(e.srcElement).closest('div');
    var delDate = $(delSpan).find('input[id*=txtBookInByFromDate]');
    var delTime = $(delSpan).find('input[id*=txtBookInByFromTime]');
    var deliveryByTime = $(delSpan).find('input[id*=txtBookInFromTime]');
    switch (selectedOption) {
        case "0": // Window
            delDate.show();
            delTime.show();
            deliveryByTime.prop("disabled", false);
            deliveryByTime.val(delTime.val());
            break;
        case "1": //Timed
            delDate.hide();
            delTime.hide();
            deliveryByTime.prop("disabled", false);
            deliveryByTime.val(delTime.val());
            break;
        case "2": // Anytime
            delDate.hide();
            delTime.hide();
            deliveryByTime.prop("disabled", true);
            deliveryByTime.val("Anytime");
            break;

    }
}

var dirtyorderIDs = "";
function setDirty(e) {
    var spn = $('#spnDirtyOrders');
    var orderID = $(e.srcElement).closest('tr').attr('orderid');

    if (dirtyorderIDs.length > 0)
        dirtyorderIDs += ",";

    if (dirtyorderIDs.indexOf(orderID) < 0)
        dirtyorderIDs += orderID;

    dirtyorderIDs = dirtyorderIDs.replace(",,", ",");
    if (dirtyorderIDs.substr(dirtyorderIDs.length - 1, 1) == ",")
        dirtyorderIDs = dirtyorderIDs.substr(0, dirtyorderIDs.length - 1);

    $(':hidden[id*=hidOrderIDs]').val(dirtyorderIDs);
    var orderCount
    if (dirtyorderIDs.length > 0) {
        orderCount = dirtyorderIDs.split(',').length;
    }
    else {
        orderCount = 0;
    }
    spn.text(orderCount);
    //    UpdateOrder(orderID);

    return true;
}

var rowBackgroundColor;

function UpdateOrder(orderID) {
    var collectFromDate;
    var collectFromTime;
    var collectToDate;
    var collectToTime;
    var deliverFromDate;
    var deliverFromTime;
    var deliverToDate;
    var deliverToTime;
    var rate;
    var deliveryNotes;

    var tr = $('table[id*=grdOrders] tr[orderid=' + orderID + ']')
    // Show the saving
    $('#spnSaving').show();

    // populate the collection date and time values
    // look at the collection option specified

    collectFromDate = tr.find('input[id*=txtCollectAtDate]').val();
    collectFromTime = tr.find(':text[id*=txtCollectAtTime]').val() == "Anytime" ? null : collectFromDate + ' ' + tr.find(':text[id*=txtCollectAtTime]').val();
    collectToDate = tr.find('input[id*=txtCollectByDate]').val();
    collectToTime = tr.find(':text[id*=txtCollectByTime]').val() == "Anytime" ? null : collectToDate + ' ' + tr.find(':text[id*=txtCollectByTime]').val();

    // look at the delivery time option specified

    deliverFromDate = tr.find('input[id*=txtDeliverAtFromDate]').val();
    deliverFromTime = tr.find(':text[id*=txtDeliverAtFromTime]').val() == "Anytime" ? null : deliverFromDate + ' ' + tr.find(':text[id*=txtDeliverAtFromTime]').val();
    deliverToDate = tr.find('input[id*=txtDeliverByFromDate]').val();
    deliverToTime = tr.find(':text[id*=txtDeliverByFromTime]').val() == "Anytime" ? null : deliverToDate + ' ' + tr.find(':text[id*=txtDeliverByFromTime]').val();

    var txtRate = tr.find('input[id*=txtRate]');
    rate = Number.parseInvariant(txtRate.val().replace(txtRate.attr('symbol'), ''));
    deliveryNotes = tr.find('textarea[id*=txtDeliveryNotes]').val();

    PageMethods.UpdateOrder(orderID, collectFromDate, collectFromTime, collectToDate, collectToTime, deliverFromDate, deliverFromTime, deliverToDate, deliverToTime, rate, deliveryNotes, userName, UpdateOrder_Success, UpdateOrder_Error)
}

function UpdateOrder_Success(result) {

    // hide the saving notification
    $('#spnSaving').hide();

    if (result[0] == "True")
        var tr = $('table[id*=grdOrders] tr[orderid=' + result[0] + ']').css("background-color", "#BEFFBA");
}

function UpdateOrder_Error(e) {
    $('#spnSaving').show();
    alert(e.get_message());
}



// Approve order selection
var approveOrderIDs = "";
var palletSpaces = 0;
function ChangeList(e, src) {
    var srcToUse = (e == null) ? src : e.srcElement;

    var gridRow = $(srcToUse).closest('tr');

    var orderID = $(srcToUse).closest('tr').attr('orderid');


    if (src.checked) {
        gridRow.attr("class", "SelectedRow_Orchestrator");
        if (approveOrderIDs.length > 0)
            approveOrderIDs += ",";
        if (approveOrderIDs.indexOf(orderID) < 0)
            approveOrderIDs += orderID;
        else
            return;

        palletSpaces += parseInt($(srcToUse).closest('tr').attr('palletspaces'));
    }
    else {
        gridRow.attr("class", "GridRow_Orchestrator");
        if (approveOrderIDs.indexOf(orderID) >= 0)
            approveOrderIDs = approveOrderIDs.replace(orderID, "");
        else
            return;
        palletSpaces -= parseInt($(srcToUse).closest('tr').attr('palletspaces'));
    }

    approveOrderIDs = approveOrderIDs.replace(",,", ",");
    if (approveOrderIDs.substr(approveOrderIDs.length - 1, 1) == ",")
        approveOrderIDs = approveOrderIDs.substr(0, approveOrderIDs.length - 1);

    $('input:hidden[id$=hidApproveOrderIDs]').val(approveOrderIDs);

    var orderCount
    if (approveOrderIDs.length > 0) {
        orderCount = approveOrderIDs.split(',').length;
    }
    else {
        orderCount = 0;
    }
    var spn = $('#spnOrdersToApprove');
    spn.text(orderCount);

    $('#spnSelectedPallets').text(palletSpaces);
}

function ShowRejectReason() {
    if (approveOrderIDs.length == 0 || approveOrderIDs.split(",").length == 0) {
        alert("You have not selected any orders to reject.");
        return;
    }

    $('#divRejectReason').show();
    $('#divRejectReason').css('top', $(window).height() / 2 - $('#divRejectReason').height() / 2);
    $('#divRejectReason').css('left', $(window).width() / 2 - $('#divRejectReason').width() / 2);

    $(':textarea[id*=txtRejectionReason]').focus();
}

// Block UI
//#region 
function showLoading() {
    $.blockUI({ css: {
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

function hideLoading() {
    $.unblockUI();
}

//#endregion

//#region Select all Orders Action
function selectAllCheckboxes(chk) {


    $('table[id*=grdOrders] input:enabled[id*=chkSelectOrder]').each(function () {
        this.click();
    });
}
//#endregion

function ViewPoint(latitude, longitude, pointName) {
    var url = "/gps/getcurrentlocation.aspx?lat=" + latitude + "&lng=" + longitude + '&desc=' + pointName;
    window.open(url, "", "height=600, width=630, scrollbars=0");
}

function ShowPosition(latitude, longitude) {

}
// Helper for positioning
//#region
function findPos(obj) {
    var curLeft = curTop = 0;
    if (obj.offsetParent) {
        do {
            curLeft += obj.offsetLeft;
            curTop += obj.offsetTop;
        } while (obj = obj.offsetParent);
    }
    return [curLeft, curTop];
}
//#endregion

function cboClient_itemsRequesting(sender, eventArgs) {
    try {
        var context = eventArgs.get_context();
        context["DisplaySuspended"] = true;
    }
    catch (err) { }
}

function orderCheckChange(orderGroupId, checkbox) {
    var checkedStatus = checkbox.checked;
    if (orderGroupId != "") {
        $('table[id*=grdOrders] input[id*=hidOrderGroupId][value=' + orderGroupId + ']').parent().find('input[id*=chkSelectOrder]').each(
           function (index, chk) {
               if (chk.disabled != true) {
                   chk.checked = checkedStatus;
                   ChangeList(null, chk);
               }
           }
        );
    }
}

// Function to show the filter options overlay box
function FilterOptionsDisplayShow() {
    $("#overlayedClearFilterBox").css({ 'display': 'block' });
    $("#filterOptionsDiv").css({ 'display': 'none' });
    $("#filterOptionsDivHide").css({ 'display': 'block' });
}

function FilterOptionsDisplayHide() {
    $("#overlayedClearFilterBox").css({ 'display': 'none' });
    $("#filterOptionsDivHide").css({ 'display': 'none' });
    $("#filterOptionsDiv").css({ 'display': 'block' });
}

function validateRejectionReason(sender, eventArgs) {
    eventArgs.IsValid = $(cboRejectionReason).val() || $(txtRejectionReason).val();
}

function rejectOrders() {
    var hidApproveOrderIDs = $("input:hidden[id$=hidApproveOrderIDs]");

    PageMethods.CanRejectOrders(
        hidApproveOrderIDs.val(),

        function (result) {
            if (result.CanReject) {
                var reject = true;

                if (result.Message) {
                    // We can reject the orders provided the user agrees to the supplied confirmation message
                    reject = confirm(result.Message);
                }

                if (reject) {
                    // Store the IDs of the orders to be rejected
                    hidApproveOrderIDs.val(result.OrdersToReject);
                    // Reject the orders
                    $("input[id$=btnHiddenRejectOrders]").click();
                }
            }
            else {
                // We cannot reject the orders... display the reason to the user
                alert(result.Message);
            }
        },

        function (error) {
            alert(error.get_message());
        });
}
