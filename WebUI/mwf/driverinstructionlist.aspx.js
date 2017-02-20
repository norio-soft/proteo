$(function () {
    jQuery('#ReassignDriverDialog').dialog({ autoOpen: false, modal: true, dialogClass: "dialogWithDropShadow" });

    $('#RefreshButton').click(refreshPage);

    $('#ReassignButton').click(showReassignDriverDialog);
    $('#ReassignDriverButton').click(checkIfOkToAssignDriver);

    $('#CommunicateButton').click(function () { communicate(false); });
    $('#CommunicateAllButton').click(function () { communicate(true); });
});

function getCurrentInstructionIDs() {
    var instructionsGrid = findComponentBySelector('[id$=InstructionsGrid]');

    var instructionIDs = $.map(instructionsGrid.get_masterTableView().get_dataItems(), function (item) {
        return item.getDataKeyValue('ID');
    });

    return instructionIDs;
}

function refreshPage() {
    var driverPickerControl = findComponentBySelector('[id$=DriverPicker]');
    var driverID = driverPickerControl.get_value() || null;
    location.href = 'driverinstructionlist.aspx' + (driverID ? '?dID=' + driverID : '');
}

function showReassignDriverDialog() {
    var instructionIDs = getCurrentInstructionIDs();

    if (instructionIDs.length == 0) {
        alert('There are no instructions to reassign');
        return;
    }

    var driverPickerControl = findComponentBySelector('[id$=ReassignDriverPicker]');
    var items = driverPickerControl.get_items();
    var firstItem = items.getItem(0);
    firstItem.select();

    $('#ReassignDriverButton')
        .attr('value', 'OK')
        .removeData('overrideAssignmentInstructionConflict');

    $('#AssignDriverWarningDiv').hide();

    $('#ReassignDriverDialog').dialog('open');
}

function checkIfOkToAssignDriver() {
    var driverPickerControl = findComponentBySelector('[id$=ReassignDriverPicker]');
    var reassignToDriverID = driverPickerControl.get_value() || null;

    if (reassignToDriverID == findComponentBySelector('[id$=DriverPicker]').get_value()) {
        alert('The instructions are already assigned to this worker.');
        return;
    }

    var parameters = { instructionIDs: getCurrentInstructionIDs(), driverID: reassignToDriverID };

    var assignInstructions = function () {
        showLoading('Reassigning instructions');

        $.ajax({
            type: "POST",
            url: "/services/job.svc/AssignInstructionsToDriver",
            data: JSON.stringify(parameters),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                hideLoading();
                $('#ReassignDriverDialog').dialog('close');
                refreshPage();
            },
            error: function (error) {
                showLoadingError(error);
            }
        });
    }

    if (!parameters.driverID || $('#ReassignDriverButton').data('overrideAssignmentInstructionConflict')) {
        assignInstructions();
        return;
    }

    showLoading('Checking reassignment');

    // Call web service to check if it is ok to assign the instructions to this driver.
    $.ajax({
        type: "POST",
        url: "/services/job.svc/CheckCanAssignInstructionsToDriver",
        data: JSON.stringify(parameters),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            hideLoading();

            if (result.hasOwnProperty('d')) {
                result = result.d;
            }

            if (result) {
                assignInstructions();
            }
            else {
                $('#AssignDriverWarningLink').attr('href', '/job/driverinstructionlist.aspx?did=' + parameters.driverID);
                $('#AssignDriverWarningDiv').slideDown();

                $('#ReassignDriverButton')
                    .attr('value', 'Proceed with worker assignment')
                    .data('overrideAssignmentInstructionConflict', true);
            }

        },
        error: function (error) {
            showLoadingError(error);
            isOk = false;
        }
    });
}

function reassignDriverPicker_selectedIndexChanged() {
    $('#ReassignDriverButton')
        .attr('value', 'OK')
        .removeData('overrideAssignmentInstructionConflict');

    $('#AssignDriverWarningDiv').hide();
} 

function communicate(resend) {
    var instructionIDs = getCurrentInstructionIDs();

    if (instructionIDs.length == 0) {
        alert('There are no instructions to communicate');
        return;
    }

    if (resend && !confirm('This will resend all instructions to the device, even if they have already been communicated and have not subsequently been changed.  Do you wish to continue?')) {
        return;
    }

    showLoading('Communicating instructions');

    $.ajax({
        type: "POST",
        url: "/services/job.svc/CommunicateInstructions",
        data: JSON.stringify({ instructionIDs: instructionIDs, resendUnchangedInstructions: resend }),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    })
    .done(function () {
        hideLoading();
        refreshPage();
    })
    .fail(function (error) {
        showLoadingError(error);
    });
}
