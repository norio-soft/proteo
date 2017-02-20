var MwfDriverMessaging = function (driverIDs, jobID) {
    var _this = this;
    var _deferred;
    var _$sendMessageDialog;
    var _$spnRunID;
    var _dteDateTime;
    var _cboPoint;
    var _txtNotes;

    if ($.isReady) {
        showSendMessageDialog();
    }
    else {
        $(function () {
            showSendMessageDialog();
        });
    }

    var getPointID = function(pointComboValue) {
        var ids = pointComboValue.split(",");
        return ids.length > 1 ? parseInt(ids[1], 10) : null;
    }

    function findDriverMessagingComponent (selector) {
        var element = $(selector, _$sendMessageDialog).get(0);
        return element ? $find(element.id) : null;
    }

    var sendMessageBlockUI = function () {
        if ($.blockUI) {
            $.blockUI({
                message: '<h1 style="color: #fff">Sending message</h1>',
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#444',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    'border-radius': '10px'
                }
            });
        }
    };

    var sendMessageUnblockUI = function () {
        if ($.unblockUI) {
            $.unblockUI();
        }
    };

    function showSendMessageDialog () {
        _$sendMessageDialog = $('#SendMessageDialog');

        // Initialise dialog
        _$sendMessageDialog.dialog({ autoOpen: false, modal: true, height: 290, width: 426, dialogClass: "dialogWithDropShadow" });

        _$sendMessageDialog
            .off('click', '#btnSendMessage') // Unbind any previously attached handler
            .on('click', '#btnSendMessage', function () {
                if (!Page_ClientValidate('driverMessaging')) {
                    return;
                }

                var messageDateTime = _dteDateTime.get_selectedDate();
                var pointID = getPointID(_cboPoint.get_value());
                var message = _txtNotes.get_value();

                sendMessageBlockUI();

                Orchestrator.WebUI.Services.MwfDriverMessaging.SendMessage(driverIDs, messageDateTime, pointID, jobID, message,
                    function () {
                        sendMessageUnblockUI();
                        _$sendMessageDialog.dialog('close');
                        _deferred.resolve();
                    },
                    function (error) {
                        sendMessageUnblockUI();
                        _$sendMessageDialog.dialog('close');
                        _deferred.reject(error.get_message ? error.get_message() : error);
                    });
            });

        // JQuery workaround to bug that causes table html to display in the combo's input sometimes
        $('[id$=cboDriverMessagingPoint]').on('blur', '.rcbInput', function () {
            var firstTableCellText = null;

            try {
                firstTableCellText = $(this.value).find('td:first').text();
            }
            catch (ex) { }

            if (firstTableCellText) {
                this.value = firstTableCellText;
            }
        });

        _this.sendMessage = function () {
            _deferred = new $.Deferred();

            _$spnRunID = $('#spnDriverMessagingRunID');
            _dteDateTime = findDriverMessagingComponent('[id$=dteDriverMessagingDateTime]');
            _cboPoint = findDriverMessagingComponent('[id$=cboDriverMessagingPoint]');
            _txtNotes = $find('txtDriverMessagingNotes');

            _$spnRunID.html(jobID);
            _dteDateTime.set_selectedDate(null);
            _cboPoint.clearSelection();
            _txtNotes.set_value('');

            Page_ClientValidate('clearValidation');

            var dlg = _$sendMessageDialog.dialog('open');

            return _deferred;
        };

    };

};
