function collectionIsAnytime(rb) {
    var pnlSimple = $(rb).closest('div[id*=pnlSimple]');


    pnlSimple.find('tr[id*=trCollectBy]').hide();

    var dteCollectionFromDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromDate]')[0].id.replace('_text', ''));
    var dteCollectionFromTime = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromTime]')[0].id.replace('_text', ''));
    var dteCollectionByDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByDate]')[0].id.replace('_text', ''));
    var dteCollectionByTime = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByTime]')[0].id.replace('_text', ''));


    if ($(rb).closest('div[id*=pnlOrderInstruction]').find(':hidden[id*=hidView]').val() == "simple") {
        if (rb != null) {
            dteCollectionFromDate.get_dateInput().focus();
            dteCollectionFromDate.get_dateInput().selectAllText();
        }
    }

    dteCollectionByDate.get_dateInput().set_value(dteCollectionFromDate.get_dateInput().get_value());

    pnlSimple.find('input:hidden[id*=hidCollectionTimingMethod]').val('anytime');

    pnlSimple.find('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val(dteCollectionFromTime.get_dateInput().get_value());
    pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val(dteCollectionByTime.get_dateInput().get_value());

    dteCollectionFromTime.get_dateInput().set_value('00:00');

    dteCollectionFromTime.get_dateInput().set_enabled(false);

    dteCollectionByTime.get_dateInput().set_value('23:59');
    dteCollectionByTime.get_dateInput().set_enabled(false);
    setDirty(rb.id);

}

function dteCollectionFromDate_SelectedDateChanged(sender, eventArgs) {
    var pnlSimple = $('#' + sender._clientID).closest('div[id*=pnlSimple]');
    var dteCollectionByDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByDate]')[0].id.replace('_text', ''));
    pnlSimple.find('input:radio[id*=rdCollectionBookingWindow]')[0];

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
    setDirty(sender._clientID);
}

function dteCollectionFromTime_SelectedDateChanged(sender, eventArgs) {
    setDirty(sender._wrapperElementID);
}

function dteCollectionByDate_SelectedDateChanged(sender, eventArgs) {
    var pnlSimple = $('#' + sender._textBoxElement.id).closest('div[id*=pnlSimple]');
    var dteCollectionFromDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromDate]')[0].id.replace('_text', ''));
    if (dteCollectionFromDate != null && sender.get_selectedDate() < dteCollectionFromDate.get_selectedDate() && rdCollectionBookingWindow != null && rdCollectionBookingWindow.prop("checked"))
        dteCollectionFromDate.set_selectedDate(sender.get_selectedDate());
    setDirty(sender._clientID);
}

function collectionBookingWindow(rb) {
    var pnlSimple = $(rb).closest('div[id*=pnlSimple]');
    var dteCollectionFromDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromDate]')[0].id.replace('_text', ''));
    var dteCollectionFromTime = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromTime]')[0].id.replace('_text', ''));
    var dteCollectionByDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByDate]')[0].id.replace('_text', ''));
    var dteCollectionByTime = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByTime]')[0].id.replace('_text', ''));

    pnlSimple.find('tr[id*=trCollectBy]').show();
    if ($(rb).closest('div[id*=pnlOrderInstruction]').find(':hidden[id*=hidView]').val() == "simple") {
        if (rb != null) {
            dteCollectionFromDate.get_dateInput().focus();
            dteCollectionFromDate.get_dateInput().selectAllText();
        }
    }

    var method = pnlSimple.find('input:hidden[id*=hidCollectionTimingMethod]').val();

    pnlSimple.find('input:hidden[id*=hidCollectionTimingMethod]').val('window');

    if (pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
        dteCollectionByDate.get_dateInput().set_value(dteCollectionFromDate.get_dateInput().get_value());
    }

    if (method == 'anytime') {
        if (pnlSimple.find('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
            dteCollectionFromTime.get_dateInput().set_value(pnlSimple.find('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

        } else {
            dteCollectionFromTime.get_dateInput().set_value("08:00");
        }

        if (pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
            dteCollectionByTime.get_dateInput().set_value(pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
        } else {
            dteCollectionByTime.get_dateInput().set_value("17:00");
        }
    }

    dteCollectionFromTime.get_dateInput().set_enabled(true);
    dteCollectionByTime.get_dateInput().set_enabled(true);
    setDirty(rb.id);
}


function collectionTimedBooking(rb) {
    var pnlSimple = $(rb).closest('div[id*=pnlSimple]');
    pnlSimple.find('tr[id*=trCollectBy]').hide();

    var dteCollectionFromDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromDate]')[0].id.replace('_text', ''));
    var dteCollectionFromTime = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionFromTime]')[0].id.replace('_text', ''));
    var dteCollectionByDate = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByDate]')[0].id.replace('_text', ''));
    var dteCollectionByTime = $telerik.findControl(pnlSimple[0], pnlSimple.find('input[id*=dteCollectionByTime]')[0].id.replace('_text', ''));

    if ($(rb).closest('div[id*=pnlOrderInstruction]').find(':hidden[id*=hidView]').val() == "simple") {
        if (rb != null) {
            dteCollectionFromDate.get_dateInput().focus();
            dteCollectionFromDate.get_dateInput().selectAllText();
        }
    }

    var method = pnlSimple.find('input:hidden[id*=hidCollectionTimingMethod]').val();

    pnlSimple.find('input:hidden[id*=hidCollectionTimingMethod]').val('timed');

    if (method == 'window') {
        pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val(dteCollectionByDate.get_dateInput().get_value());
    }

    if (method == 'anytime') {
        if (pnlSimple.find('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
            dteCollectionFromTime.get_dateInput().set_value(pnlSimple.find('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

        } else {
            dteCollectionFromTime.get_dateInput().set_value("08:00");
        }

        if (pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
            dteCollectionByTime.get_dateInput().set_value(pnlSimple.find('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
        } else {
            dteCollectionByTime.get_dateInput().set_value("17:00");
        }
    }
    dteCollectionFromTime.get_dateInput().set_enabled(true);
    dteCollectionByTime.get_dateInput().set_enabled(true);

    setDirty(rb.id);
}


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


var dirtyorderIDs = "";
var dirtyInstructionIDs = "";
function setDirty(e) {
    if (_initFired == true) {
        var orderID = e.srcElement ? $(e.srcElement).closest('tr').attr('orderid') : null;
        var instructionID = typeof (e) == 'string' ? $('#' + e).closest('tr[id$=rowInstruction]').find(':hidden[id*=hidInstructionId]').val() : null;

        // instructions
        if (instructionID) {
            if (dirtyInstructionIDs.length > 0)
                dirtyInstructionIDs += ",";
            if (dirtyInstructionIDs.indexOf(instructionID) < 0)
                dirtyInstructionIDs += instructionID;
            dirtyInstructionIDs = dirtyInstructionIDs.replace(",,", ",");
            if (dirtyInstructionIDs.substr(dirtyInstructionIDs.length - 1, 1) == ",")
                dirtyInstructionIDs = dirtyInstructionIDs.substr(0, dirtyInstructionIDs.length - 1);

            $('input:hidden[id*=hidInstructionIDs]').val(dirtyInstructionIDs);
        }

        // orders
        if (orderID) {
            if (dirtyorderIDs.length > 0)
                dirtyorderIDs += ",";

            if (dirtyorderIDs.indexOf(orderID) < 0)
                dirtyorderIDs += orderID;

            dirtyorderIDs = dirtyorderIDs.replace(",,", ",");
            if (dirtyorderIDs.substr(dirtyorderIDs.length - 1, 1) == ",")
                dirtyorderIDs = dirtyorderIDs.substr(0, dirtyorderIDs.length - 1);

            $(':hidden[id*=hidOrderIDs]').val(dirtyorderIDs);
        }
    }

    return true;
}

var _initFired = false;
function InitialiseOrchestratorPage() {
    window.clearTimeout(timerID);
    if (_initFired)
        return;

    dirtyInstructionIDs = "";
    dirtyorderIDs = "";

    if ($(":hidden[id*=hidSimpleInstructions]").val().length > 0) {
        var instructions = new Array();
        instructions = $(":hidden[id*=hidSimpleInstructions]").val().split(',');
        $.each(instructions, function(index, value) {
            var btn = $(':hidden[id*=hidInstructionId][value=' + value + ']').closest('tr[id$=rowInstruction]').find('input[id*=btnSimple]');
            ShowSimple(btn);
        });
    }

    initialiseDeliveryMatrixRecalculation();

    _initFired = true;

}

function initialiseDeliveryMatrixRecalculation() {
    // Set up collection time recalculation based on delivery matrix where configured
    var firstDropRow = document.querySelector('tr.firstDrop');

    if (firstDropRow == null) {
        return;
    }

    var firstDropPointID = $('[id$=hidPointID]', firstDropRow).val();
    var firstDropDate = findComponentBySelector('[id$=dteCollectionFromDate]', firstDropRow);
    var firstDropTime = findComponentBySelector('[id$=dteCollectionFromTime]', firstDropRow);

    var valueChanged = function () {
        recalculateCollectionTimes(firstDropPointID, firstDropDate, firstDropTime);
    }

    firstDropDate.get_dateInput().add_valueChanged(valueChanged);
    firstDropTime.get_dateInput().add_valueChanged(valueChanged);
}

function recalculateCollectionTimes(firstDropPointID, firstDropDate, firstDropTime) {
    if (!confirm('If you would you like to recalculate the collection times based on the changed delivery time press OK, otherwise press Cancel.')) {
        return;
    }

    var firstDropDate = firstDropDate.get_selectedDate();
    var time = firstDropTime.get_selectedDate();
    firstDropDate.setHours(time.getHours(), time.getMinutes(), time.getSeconds(), time.getMilliseconds());

    showLoading();

    PageMethods.RecalculateCollectionTimes(
        jobID,
        firstDropDate,
        firstDropPointID,
        function (result) {
            hideLoading();

            if (!result.Success) {
                alert('The collection times have not been modified because no matching delivery window could be found.')
            }
            else {
                $.each(result.Times, function (index, calculatedCollectionTime) {
                    var instructionID = calculatedCollectionTime.InstructionID;

                    // Find the row for this instruction
                    var instructionRow = $('tr.instructionRow_' + instructionID);

                    if (instructionRow.length > 0) {
                        var arrivalDateTime = calculatedCollectionTime.PlannedArrivalDateTime;

                        // Set date/time for "Simple" view
                        $('[id$=rdCollectionTimedBooking]', instructionRow).prop('checked', true);
                        findComponentBySelector('[id$=dteCollectionFromDate]', instructionRow).set_selectedDate(arrivalDateTime);
                        findComponentBySelector('[id$=dteCollectionFromTime]', instructionRow).set_selectedDate(arrivalDateTime);
                        findComponentBySelector('[id$=dteCollectionByDate]', instructionRow).set_selectedDate(arrivalDateTime);
                        findComponentBySelector('[id$=dteCollectionByTime]', instructionRow).set_selectedDate(arrivalDateTime);

                        // Set date/time for "Advanced" view
                        var arrivalDate = arrivalDateTime.format('dd/MM/yy');
                        var arrivalTime = arrivalDateTime.format('HH:mm');
                        var rbAdvancedCollectionIsTimed = $('[id$=rblCollectionTimeOptions_1]', instructionRow);
                        rbAdvancedCollectionIsTimed.prop('checked', true);
                        $('[id$=txtCollectAtDate]', instructionRow).val(arrivalDate);
                        $('[id$=txtCollectAtTime]', instructionRow).val(arrivalTime);
                        $('[id$=txtCollectByDate]', instructionRow).val(arrivalDate);
                        $('[id$=txtCollectByTime]', instructionRow).val(arrivalTime);
                        setDirty({ srcElement: rbAdvancedCollectionIsTimed });
                    }
                });
            }
        },
        function (error) {
            hideLoading();

            var msg = 'Error creating delivery window';
            if (error.get_message) {
                msg += ':\n\n' + error.get_message();
            }

            alert(msg);
        });
}

function ShowSimple(sender) {
    $(sender).closest('div[id*=pnlOrderInstruction]').find('div[id*=pnlSimple]').show();
    $(sender).closest('div[id*=pnlOrderInstruction]').find('div[id*=pnlAdvanced]').hide();
    $(sender).closest('div[id*=pnlOrderInstruction]').find(':hidden[id*=hidView]').val('simple');
    return false;
}

function ShowAdvanced(sender) {
    $(sender).closest('div[id*=pnlOrderInstruction]').find('div[id*=pnlSimple]').hide();
    $(sender).closest('div[id*=pnlOrderInstruction]').find('div[id*=pnlAdvanced]').show();
    $(sender).closest('div[id*=pnlOrderInstruction]').find(':hidden[id*=hidView]').val('advanced');
    return false;
}

function viewOrderProfile(orderID) {
    var url = "/Groupage/updateOrder.aspx?wiz=true&oID=" + orderID;
    window.open(url, "Order", "width=1200, height=900, scrollbars=1, resizeable=1");
}

function showLoading() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '10px',
            opacity: '.5',
            color: '#fff'
        }
    });
}

function hideLoading() {
    $.unblockUI();
}
