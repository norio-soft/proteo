$(function () {
    $("#PlannedTimes").fixedHeader({
        width: $('#PlannedTimes').width(),
        height: 220
    });
});

function pageLoad() {
    // Set up collection time recalculation based on delivery matrix where configured
    var firstDropRow = document.querySelector('tr.firstDrop');

    if (firstDropRow == null) {
        return;
    }

    var firstDropRowEndPointID = $('[id$=hidEndLegPointID]', firstDropRow).val();
    var firstDropRowEndDate = findComponentBySelector('[id$=dteEDate]', firstDropRow);
    var firstDropRowEndTime = findComponentBySelector('[id$=dteETime]', firstDropRow);

    var valueChanged = function () {
        var e = window.event;
        if (e.preventDefault) {
            e.preventDefault();
        }

        recalculateCollectionTimes(firstDropRowEndPointID, firstDropRowEndDate, firstDropRowEndTime);
    }

    firstDropRowEndDate.add_valueChanged(valueChanged);
    firstDropRowEndTime.add_valueChanged(valueChanged);
}

function recalculateCollectionTimes(firstDropRowEndPointID, firstDropRowEndDate, firstDropRowEndTime) {
    if (!confirm('If you would you like to recalculate the planned collection times based on the changed delivery time press OK, otherwise press Cancel.')) {
        return;
    }

    var firstDropDate = firstDropRowEndDate.get_selectedDate();
    var time = firstDropRowEndTime.get_selectedDate();
    firstDropDate.setHours(time.getHours(), time.getMinutes(), time.getSeconds(), time.getMilliseconds());

    showLoading();

    PageMethods.RecalculateCollectionTimes(
        jobID,
        firstDropDate,
        firstDropRowEndPointID,
        function (result) {
            hideLoading();

            if (!result.Success) {
                alert('The collection times have not been modified because no matching delivery window could be found.')
            }
            else {
                $.each(result.Times, function (index, calculatedCollectionTime) {
                    var instructionID = calculatedCollectionTime.InstructionID;

                    // Find the "legStart" row... this will only exist for the first instruction where there is no leg ending on this instruction
                    var legStart = $('tr.legStart_' + instructionID);

                    if (legStart.length > 0) {
                        var startDateTime = calculatedCollectionTime.PlannedArrivalDateTime;
                        findComponentBySelector('[id$=dteSDate]', legStart).set_selectedDate(startDateTime);
                        findComponentBySelector('[id$=dteSTime]', legStart).set_selectedDate(startDateTime);
                    }
                    else {
                        // Find the leg row ending on this instruction
                        var legRow = $('tr.legRow_' + instructionID);

                        if (legRow.length > 0) {
                            var endDateTime = calculatedCollectionTime.PlannedArrivalDateTime;
                            findComponentBySelector('[id$=dteEDate]', legRow).set_selectedDate(endDateTime);
                            findComponentBySelector('[id$=dteETime]', legRow).set_selectedDate(endDateTime);

                            var nextLegRow = legRow.next();

                            if (nextLegRow.length > 0) {
                                var startDateTime = calculatedCollectionTime.PlannedDepartureDateTime;
                                findComponentBySelector('[id$=dteSDate]', nextLegRow).set_selectedDate(startDateTime);
                                findComponentBySelector('[id$=dteSTime]', nextLegRow).set_selectedDate(startDateTime);
                            }
                        }
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
