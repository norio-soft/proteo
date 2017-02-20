var MwfInstructionHistory = function (startInstructionID, endInstructionID) {
    var _this = this;
    var _deferred;
    var _$instructionHistoryDialog;
    var _$spnStartHeader;
    var _$ulStartEventDetails;
    var _$spnStartDeviceIdentifier;
    var _$spnEndHeader;
    var _$ulEndEventDetails;
    var _$spnEndDeviceIdentifier;

    if ($.isReady) {
        showInstructionHistoryDialog();
    }
    else {
        $(function () {
            showInstructionHistoryDialog();
        });
    }

    function showInstructionHistoryDialog() {

        _$spnStartHeader = $('#spnInstructionHistoryStartHeader');
        _$ulStartEventDetails = $('#ulInstructionHistoryStartEventDetails');
        _$spnStartDeviceIdentifier = $('#spnInstructionHistoryStartDeviceIdentifier');
        _$spnEndHeader = $('#spnInstructionHistoryEndHeader');
        _$ulEndEventDetails = $('#ulInstructionHistoryEndEventDetails');
        _$spnEndDeviceIdentifier = $('#spnInstructionHistoryEndDeviceIdentifier');

        _$instructionHistoryDialog = $('#InstructionHistoryDialog');

        // Initialise dialog
        _$instructionHistoryDialog.dialog({ autoOpen: false, modal: true, height: 290, width: 426, dialogClass: 'dialogWithDropShadow' });

        $.get(
            '/api/instructionhistory',
            {
                startInstructionID: startInstructionID,
                endInstructionID: endInstructionID
            },
            function (data) {
                populateHeaders(data);
                populateEventDetails(data);
                populateDeviceIdentifier(data);
            });
    };

    _this.show = function () {
        _deferred = new $.Deferred();

        var dlg = _$instructionHistoryDialog.dialog('open');

        return _deferred;
    };

    var populateHeaders = function (response) {

        if (response.StartInstructionHistory)
            _$spnStartHeader.html(response.StartInstructionHistory.InstructionType + ' - ' + response.StartInstructionHistory.PointDescription);

        if (response.EndInstructionHistory)
            _$spnEndHeader.html(response.EndInstructionHistory.InstructionType + ' - ' + response.EndInstructionHistory.PointDescription);
    };

    var populateEventDetails = function (response) {

        var startList = '';
        var endList = '';
        
        _$ulStartEventDetails.empty();
        _$ulEndEventDetails.empty();

        if (response.StartInstructionHistory) {
            for (i = 0; i < response.StartInstructionHistory.EventDetails.length; i++) {
                startList += '<li><label>' + (new Date(response.StartInstructionHistory.EventDetails[i].TimeofEvent)).format('dd/MM/yyyy HH:mm') + '</label> - ' + response.StartInstructionHistory.EventDetails[i].EventDetails + '</li>';
            }
            _$ulStartEventDetails.append(startList);
        }

        if (response.EndInstructionHistory) {
            for (i = 0; i < response.EndInstructionHistory.EventDetails.length; i++) {
                endList += '<li><label>' + (new Date(response.EndInstructionHistory.EventDetails[i].TimeofEvent)).format('dd/MM/yyyy HH:mm') + '</label> - ' + response.EndInstructionHistory.EventDetails[i].EventDetails + '</li>';
            }
            _$ulEndEventDetails.append(endList);
        }
    };

    var populateDeviceIdentifier = function (response) {
        if (response.StartInstructionHistory)
            if (response.StartInstructionHistory.DeviceIdentifier)
                _$spnStartDeviceIdentifier.html('Device identifier - ' + response.StartInstructionHistory.DeviceIdentifier);

        if (response.EndInstructionHistory)
            if (response.EndInstructionHistory.DeviceIdentifier)
                _$spnEndDeviceIdentifier.html('Device identifier - ' + response.EndInstructionHistory.DeviceIdentifier);
    };

};
