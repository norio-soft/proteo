/////////////////////////
// Page / Load Events
/////////////////////////

window.onload = function() {
    var rdCollectionTimedBooking = $('input:radio[id*=rdCollectionTimedBooking]');
    var rdCollectionIsAnytime = $('input:radio[id*=rdCollectionIsAnytime]');
    var rdCollectionBookingWindow = $('input:radio[id*=rdCollectionBookingWindow]');
    var collectBy = $('tr[id*=trCollectBy]');

    var rdDeliveryTimedBooking = $('input:radio[id*=rdDeliveryTimedBooking]');
    var rdDeliveryIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]');
    var rdDeliveryBookingWindow = $('input:radio[id*=rdDeliveryBookingWindow]');
    var deliverFrom = $('tr[id*=trDeliverFrom]');

    if (rdDeliveryTimedBooking.length > 0 && rdDeliveryIsAnytime.length > 0 && rdDeliveryBookingWindow.length > 0)
        switch (true) {
            case rdDeliveryTimedBooking[0].checked:
                deliverFrom.hide();
                break;

            case rdDeliveryIsAnytime[0].checked:
                $('input[id*=dteDeliveryFromTime]').prop("disabled", true);
                $('input[id*=dteDeliveryByTime]').prop("disabled", true);
                deliverFrom.hide();
                break;

            case rdDeliveryBookingWindow[0].checked:
                deliverFrom.show();
                break;
        }

    if (rdCollectionTimedBooking.length > 0 && rdCollectionIsAnytime.length > 0 && rdCollectionBookingWindow.length > 0)
        switch (true) {
            case rdCollectionTimedBooking[0].checked:
                collectBy.hide();
                break;

            case rdCollectionIsAnytime[0].checked:
                $('input[id*=dteDeliveryFromTime]').prop("disabled", true);
                $('input[id*=dteDeliveryByTime]').prop("disabled", true);
                collectBy.hide();
                break;

            case rdCollectionBookingWindow[0].checked:
                collectBy.show();
                break;
        }
};

function initControls(hid) {
    if (hid.value == "0") {

        // Initial page state
        rdOrderAction_NotCollected.prop('checked', true);
        rdResolutionMethod_AttemptLater.prop('checked', true);

        $('tr[id*=trAttemptedCollection_]').show();
        $('tr[id*=trCharge_]').hide();

        trAttemptedCollection_DeliveryTime.hide();
        divCollectionPoint.hide();
        divDeliveryPoint.hide();
        divCustomReason.hide();

        // Turn off validation for hidden fields
        ValidatorEnable(rfvCustomReason, false);
        ValidatorEnable(rfvClientContact, false);
        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

    } else {

        if (cboRedeliveryReason.value == 'Other') {
            ValidatorEnable(rfvCustomReason, true);
            divCustomReason.show();
        } else {
            ValidatorEnable(rfvCustomReason, false);
            divCustomReason.hide();
        }

        if (chkCollectGoodsElsewhere.prop('checked')) {
            divCollectionPoint.show();
            ValidatorEnable(ucNewCollectionPoint_rfvPoint, true);
        } else {
            divCollectionPoint.hide();
            ValidatorEnable(ucNewCollectionPoint_rfvPoint, false);
        }

        if (chkDeliverGoodsElsewhere.prop('checked')) {
            divDeliveryPoint.show();
            ValidatorEnable(ucNewDeliveryPoint_rfvPoint, true);
        } else {
            divDeliveryPoint.hide();
            ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
        }

        if (chkCharging.prop('checked')) {

            $('tr[id*=trCharge_]').show();
            if (cboExtraState.value != 'Awaiting Response') {
                ValidatorEnable(rfvClientContact, true);
                trCharge_Client.show();
            } else {
                ValidatorEnable(rfvClientContact, false);
                trCharge_Client.hide();
            }
        } else {
            $('tr[id*=trCharge_]').hide();
        }

        if (rdOrderAction_Collected.checked) {

            $('tr[id*=trTurnedAway_]').hide();
            $('tr[id*=trCharge_]').hide();
            rdResolutionMethod_AttemptLater.checked = true;
            rdResolutionMethod_Cancel.checked = false;

        } else {

            if (rdResolutionMethod_AttemptLater.checked) {

                $('tr[id*=trTurnedAway_]').hide();
                trAttemptedCollection_ResolutionMethod.show();
                trAttemptedCollection_Charge.show();
                trAttemptedCollection_Reason.show();
                trAttemptedCollection_ChangeDeliveryDate.show();
                ValidatorEnable(rfvClientContact, false);
                ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

            }

            if (rdResolutionMethod_Cancel.checked) {

                $('tr[id*=trTurnedAway_]').hide();
                $('tr[id*=trCharge_]').hide();
                trAttemptedCollection_ResolutionMethod.show();
                trAttemptedCollection_Reason.show();
                chkCharging.checked = false;
                ValidatorEnable(rfvClientContact, false);
                ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

            }
        }
    }
}


/////////////////////////
// Control Events
/////////////////////////
function chkChangeDeliveryDate_onClick(sender) {
    if ($(sender).prop('checked'))
        trAttemptedCollection_DeliveryTime.show();
    else
        trAttemptedCollection_DeliveryTime.hide();
}

function rdOrderAction_NotCollected_onClick(sender) {
    $('tr[id*=trCollection_]').show();
    $('tr[id*=trAttemptedCollection_]').show();
    $('tr[id*=trCharge_]').hide();

    if (!chkChangeDeliveryDate.prop('checked'))
        trAttemptedCollection_DeliveryTime.hide();

    if (!chkCollectGoodsElsewhere.prop('checked'))
        divCollectionPoint.hide();

    if (!chkDeliverGoodsElsewhere.prop('checked'))
        divDeliveryPoint.hide();

    chkCharging_onClick(chkCharging[0]);
    
    chkCharging.checked = false;
}

function rdOrderAction_Collected_onClick(sender) {
    $('tr[id*=trCollection_]').hide();
    $('tr[id*=trAttemptedCollection_]').hide();
    $('tr[id*=trCharge_]').hide();
    
    rdResolutionMethod_AttemptLater.checked = true;
    rdResolutionMethod_Cancel.checked = false;
}


// Resolution methods
function rdResolutionMethod_Cancel_onClick(sender) {
    $('tr[id*=trAttemptedCollection_]').hide();
    $('tr[id*=trCharge_]').hide();
    chkCharging.checked = false;
    ValidatorEnable(rfvClientContact, false);
    ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
    ValidatorEnable(ucNewCollectionPoint_rfvPoint, false);
}

function rdResolutionMethod_AttemptLater_onClick(sender) {
    $('tr[id*=trAttemptedCollection_]').show();
    $('tr[id*=trCharge_]').hide();

    ValidatorEnable(rfvClientContact, true);

    if (!chkChangeDeliveryDate.prop('checked'))
        trAttemptedCollection_DeliveryTime.hide();

    if (!chkCollectGoodsElsewhere.prop('checked')) {
        ValidatorEnable(ucNewCollectionPoint_rfvPoint, false);
        divCollectionPoint.hide();
    }
    else
        ValidatorEnable(ucNewCollectionPoint_rfvPoint, true);

    if (!chkDeliverGoodsElsewhere.prop('checked')) {
        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
        divDeliveryPoint.hide();
    }
    else
        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, true);

    chkCharging_onClick(chkCharging[0]);
}

// Store/delivery points
function chkDeliverGoodsElsewhere_onClick(sender) {
    if ($(sender).prop('checked')) {
        divDeliveryPoint.show();
        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, true);
    } else {
        divDeliveryPoint.hide();
        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
    }
}

function chkCollectGoodsElsewhere_onClick(sender) {
    if ($(sender).prop('checked')) {
        divCollectionPoint.show();
        ValidatorEnable(ucNewCollectionPoint_rfvPoint, true);
    } else {
        divCollectionPoint.hide();
        ValidatorEnable(ucNewCollectionPoint_rfvPoint, false);
    }
}

function chkCharging_onClick(sender) {
    if ($(sender).prop('checked')) {
        $('tr[id*=trCharge_]').show();
        if (cboExtraState.value != 'Awaiting Response') {
            ValidatorEnable(rfvClientContact, true);
            trCharge_Client.show();
        } else {
            ValidatorEnable(rfvClientContact, false);
            trCharge_Client.hide();
        }
    } else {
        $('tr[id*=trCharge_]').hide();
    }
}

function cboExtraState_onChange(sender) {
    if (cboExtraState.value != 'Awaiting Response') {
        ValidatorEnable(rfvClientContact, true);
        trCharge_Client.show();
    } else {
        ValidatorEnable(rfvClientContact, false);
        trCharge_Client.hide();
    }
}

function cboRedeliveryReason_onChange(sender) {
    if (cboRedeliveryReason.value == 'Other') {
        ValidatorEnable(rfvCustomReason, true);
        divCustomReason.show();
    } else {
        ValidatorEnable(rfvCustomReason, false);
        divCustomReason.hide();
    }
}



/////////////////////////
// Validation
/////////////////////////

function validatePoints(src, args) {

//    args.IsValid = true;
//    var rdResolutionMethod_Redeliver = $('input:radio[id*=rdResolutionMethod_Redeliver]')[0];

//    if (rdResolutionMethod_Redeliver.checked == true) {

//        var chkCrossDockGoods = $('input:checkbox[id*=chkCrossDockGoods]')[0];
//        var chkDeliverGoodsElsewhere = $('input:checkbox[id*=chkDeliverGoodsElsewhere]')[0];
//        args.IsValid = !(chkCrossDockGoods.checked == false && chkDeliverGoodsElsewhere.checked == false);
//        if (!args.IsValid) {
//            var imgPointValidationWarningIcon = $('img[id*=imgPointValidationWarningIcon]')[0];
//            $(imgPointValidationWarningIcon).show();
//        }
//    }
}

function ValidateDepartureAfterArrival(sender, args) {
//    var arrivalDate = $find('<%=txtArrivalDateTime.ClientID %>');
//    var departureDate = $find('<%=txtDepartureDateTime.ClientID %>');

//    args.IsValid = arrivalDate.get_displayValue() <= departureDate.get_displayValue();
}

function ValidateDeliveryAfterDeparture(sender, args) {
    if (dteDeliveryByDate != null && dteDeliveryByTime != null) {
    
        var enteredDateParts = dteDeliveryByDate.value.split("/");

        var departureDateTime = itemDepartureDateTime.get_displayValue();
        var deliveryDate = new Date();

        deliveryDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

        if (dteDeliveryByTime.value.length > 0) {
            var enteredTimeParts = dteDeliveryByTime.value.split(":");
            deliveryDate.setHours(enteredTimeParts[0], enteredTimeParts[1], 0, 0);
        }
        else {
            deliveryDate.setHours(23, 59, 0, 0);
        }

        if (deliveryDate > departureDateTime)
            args.IsValid = true;
        else
            args.IsValid = false;
    }
}

function CV_ClientValidateCollectionDate(source, args) {
    var IsUpdate = $get("<%=hidPageIsUpdate.ClientID%>");

    if (IsUpdate != null && IsUpdate.value != "True") {

        var today = new Date();
        var day_date = today.getDate();
        var month_date = today.getMonth();
        var year_date = today.getFullYear();

        today.setFullYear(year_date, month_date, day_date);

        var enteredDateParts = dteCollectionFromDate.get_displayValue().split("/");

        var enteredDate = new Date();
        enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

        if (enteredDate >= today) {
            args.IsValid = true;

        }
        else {
            if (!_skipDeliveryDateCheck) {
                _skipDeliveryDateCheck = true;
                args.IsValid = false;
                alert("The collection date entered is in the past - Are you sure?");
            }
        }
    }
}

function CV_ClientValidateDeliveryDate(source, args) {
    var today = new Date();
    var day_date = today.getDate();
    var month_date = today.getMonth();
    var year_date = today.getFullYear();

    today.setFullYear(year_date, month_date, day_date);

    var enteredDateParts = dteDeliveryByDate.get_displayValue().split("/");
    var enteredDate = new Date();

    enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

    if (enteredDate >= today) {
        args.IsValid = true;
    }
    else {
        if (!_skipCollectionDateCheck) {
            _skipCollectionDateCheck = true;
            args.IsValid = false;
            alert("The date entered is in the past - Are you sure?");
        }
    }
}

function CV_ClientValidateDeliveryDate2(source, args) {
    var warningDate = new Date();
    var day_date = warningDate.getDate();
    var month_date = warningDate.getMonth();
    var year_date = warningDate.getFullYear();

    warningDate.setFullYear(year_date, month_date, day_date);

    var enteredDateParts = dteDeliveryByDate.get_displayValue().split("/");
    var enteredDate = new Date();

    enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

    if (enteredDate >= warningDate) {
        if (!_skipCollectionDateCheck) {
            _skipCollectionDateCheck = true;

            args.IsValid = true; // do not prevent the order from being created.
            alert("The date entered is far in the future - Are you sure?");
        }
    }
    else {
        args.IsValid = true;
    }
}

function validateCollectionPointDisplay() {
    if (chkCollectGoodsElsewhere.prop('checked')) {
        divCollectionPoint.show();
        ValidatorEnable(ucNewCollectionPoint_rfvPoint, true);
    } else {
        divCollectionPoint.hide();
        ValidatorEnable(ucNewCollectionPoint_rfvPoint, false);
    }
}




/////////////////////////
// Javascript for Data Controls
/////////////////////////

function deliveryTimedBooking(rb) {
    $('tr[id*=trDeliverFrom]').hide();

    var dteDeliveryByDate = $('input[id*=dteDeliveryByDate]');
    dteDeliveryByDate.focus();
    dteDeliveryByDate.select();
    
    var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

    $('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

    if (method == 'window') {
        $('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val(dteDeliveryFromTime.get_value());
    }

    dteDeliveryFromTime.set_value(dteDeliveryFromTime.get_value());

    if (method == 'anytime') {
        if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
            dteDeliveryFromTime.set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
        } else {
            dteDeliveryFromTime.set_value("17:00");
        }

        if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
            dteDeliveryByTime.set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
        } else {
            dteDeliveryByTime.set_value("17:00");
        }
    }

    dteDeliveryFromTime.enable();
    dteDeliveryByTime.enable();
}

function deliveryBookingWindow(rb) {
    $('tr[id*=trDeliverFrom]').show();
    var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
    dteDeliveryFromDate.focus();
    dteDeliveryFromDate.select();

    var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

    $('input:hidden[id*=hidDeliveryTimingMethod]').val('window');

    if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val()) {
        dteDeliveryFromTime.set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
    }

    //if (method == 'anytime') {
        if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
            dteDeliveryFromTime.set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
        } else {
            dteDeliveryFromTime.set_value("08:00");
        }

        if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
            dteDeliveryByTime.set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
        } else {
            dteDeliveryByTime.set_value("17:00");
        }
    //}

    dteDeliveryFromTime.enable();
    dteDeliveryByTime.enable();
}

function deliveryIsAnytime(rd) {
    $('tr[id*=trDeliverFrom]').hide();

    var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
    dteDeliveryFromDate.focus();
    dteDeliveryFromDate.select();

    dteDeliveryFromTime.set_value('00:00');
    dteDeliveryFromTime.disable();

    dteDeliveryByTime.set_value('23:59');
    dteDeliveryByTime.disable();
}

function dteDeliveryByDate_SelectedDateChanged(sender, eventArgs) {
    var updateDate = false;

    if (dteDeliveryFromDate != null) {
        if (sender.get_selectedDate() < dteDeliveryFromDate.get_selectedDate())
            updateDate = true;
        else if (rdDeliveryBookingWindow != null && !rdDeliveryBookingWindow.prop("checked"))
            updateDate = true;
    }

    if (updateDate)
        dteDeliveryFromDate.set_selectedDate(sender.get_selectedDate());
}

function collectionTimedBooking(rb) {

    $('tr[id*=trCollectBy]').hide();

    var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
    if (rb != null) {
        dteCollectionFromDate.focus();
        dteCollectionFromDate.select();
    }

    var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

    $('input:hidden[id*=hidCollectionTimingMethod]').val('timed');

    if (method == 'window') {
        $('input:hidden[id*=hidCollectionByTimeRestoreValue]').val(dteCollectionByTime.get_value());
    }

    dteCollectionByTime.set_value(dteCollectionFromTime.get_value());

    if (method == 'anytime') {
        if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
            dteCollectionFromTime.set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

        } else {
            dteCollectionFromTime.set_value("08:00");
        }

        if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
            dteCollectionByTime.set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
        } else {
            dteCollectionByTime.set_value("17:00");
        }
    }
    
    dteCollectionByTime.enable();
    dteCollectionFromTime.enable();
}

function collectionBookingWindow(rb) {
    $('tr[id*=trCollectBy]').show();
    var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
    if (rb != null) {
        dteCollectionFromDate.focus();
        dteCollectionFromDate.select();
    }

    var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

    $('input:hidden[id*=hidCollectionTimingMethod]').val('window');

    if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
        dteCollectionByTime.set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
    }

    if (method == 'anytime') {
        if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
            dteCollectionFromTime.set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

        } else {
            dteCollectionFromTime.set_value("08:00");
        }

        if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
            dteCollectionByTime.set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
        } else {
            dteCollectionByTime.set_value("17:00");
        }
    }

    dteCollectionByTime.enable();
    dteCollectionFromTime.enable();
}

function collectionIsAnytime(rb) {
    $('tr[id*=trCollectBy]').hide();

    var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
    if (rb != null) {
        dteCollectionFromDate.focus();
        dteCollectionFromDate.select();
    }

    $('input:hidden[id*=hidCollectionTimingMethod]').val('anytime');
    $('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val(dteCollectionFromTime.get_value());
    $('input:hidden[id*=hidCollectionByTimeRestoreValue]').val(dteCollectionByTime.get_value());

    dteCollectionFromTime.set_value('00:00');
    dteCollectionFromTime.disable();

    dteCollectionByTime.set_value('23:59');
    dteCollectionByTime.disable();

}

function dteCollectionFromDate_SelectedDateChanged(sender, eventArgs) {
    var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");
    var updateDate = false;

    if (dteCollectionByDate != null) {
        if (rdCollectionBookingWindow != null && !rdCollectionBookingWindow.prop("checked"))
            updateDate = true;
        else if (sender.get_selectedDate() > dteCollectionByDate.get_selectedDate())
            updateDate = true;
    }

    if (updateDate) {
        dteCollectionByDate.set_selectedDate(sender.get_selectedDate());
    }
}

//if (rdDeliveryTimedBooking != null && rdDeliveryTimedBooking.length > 0 && rdDeliveryTimedBooking[0].checked == true)
//    $('tr[id*=trDeliverFrom]').hide();

//if (rdDeliveryIsAnytime != null && rdDeliveryIsAnytime.length > 0 && rdDeliveryIsAnytime[0].checked == true)
//    $('tr[id*=trDeliverFrom]').hide();

//if (rdDeliveryBookingWindow != null && rdDeliveryBookingWindow.length > 0 && rdDeliveryBookingWindow[0].checked == true)
//    $('tr[id*=trDeliverFrom]').show();





/////////////////////////
// Page Functions and Ajax / Web Methods / UI
/////////////////////////

function ViewOrder(orderID) {
    var url = "/Groupage/ManageOrder.aspx";
    url += "?oID=" + orderID;

    var wnd = window.radopen("about:blank", "largeWindow");
    wnd.SetUrl(url);
    wnd.SetTitle("Add/Update Order");
}

function rapRedelivery_OnRequestStart(sender, eventArgs) {
    showLoading("Updating...");
}

function rapRedelivery_OnResponseEnd(sender, eventArgs) {
    hideLoading();
}

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

function hideLoading() {
    $.unblockUI();
}