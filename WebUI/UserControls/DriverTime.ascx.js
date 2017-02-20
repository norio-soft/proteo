var DriverTime = function(driverID) {
    var _$driverTimeDialog;
    var _dteStartDate;
    var _dteEndDate;
    var _cboDriver;
    var _$grid;
    var _grid;
    var _latestDateStamp;
    var _latestVerifiedDateStamp;

    var initialise = function () {
        _$driverTimeDialog = $('#DriverTimeDialog');
        _dteStartDate = findComponent('[id$=dteDriverTimeStartDate]');
        _dteEndDate = findComponent('[id$=dteDriverTimeEndDate]');
        _cboDriver = findComponent('[id$=cboDriverTimeDriver]');
        _$grid = $('#driverTimeGrid');
        _latestDateStamp = $('#LatestNonVerifiedDateStamp');
        _latestVerifiedDateStamp = $('#LatestVerifiedDateStamp');

        _cboDriver.set_value(driverID);
        _cboDriver.set_text('');
        _latestDateStamp.html('');
        _latestVerifiedDateStamp.html('');
        _$grid.empty();

        _$driverTimeDialog.dialog({ autoOpen: false, modal: true, height: 550, width: 460, dialogClass: "dialogWithDropShadow" });

        _$driverTimeDialog
            .off('click', '#btnDriverTimeRefresh') // Unbind any previously attached handler
            .on('click', '#btnDriverTimeRefresh', function () {
                if (Page_ClientValidate('vgDriverTime')) {
                    _grid.dataSource.read();
                }
            });
    };

    this.show = function () {
        var weekStartDay = parseInt($('#hidDriverTimeWeekStartDay').val(), 10);
        var weekStartMoment = moment().startOf('day').subtract('days', (7 + moment().day() - weekStartDay) % 7);
        var weekStart = weekStartMoment.toDate();
        var weekEnd = weekStartMoment.clone().add('days', 6).toDate();
        _dteStartDate.set_selectedDate(weekStart);
        _dteEndDate.set_selectedDate(weekEnd);

        _cboDriver.set_value(driverID);
        _cboDriver.requestItems('', false);

        configureGrid();
        var dlg = _$driverTimeDialog.dialog('open');

        Page_ClientValidate('clearValidation');
    };

    var configureGrid = function () {
        var columnDefinitions = [{
            field: 'Day',
            title: 'Date',
            format: '{0:dd/MM/yyyy}'
        }, {
            field: 'RestDuration',
            title: 'Break'
        }, {
            field: 'AvailabilityDuration',
            title: 'POA'
        }, {
            field: 'WorkDuration',
            title: 'Other Work'
        }, {
            field: 'DriveDuration',
            title: 'Drive'
        }, {
            field: 'TotalWorkingTime',
            title: 'Total Working Time'
        }, {
            field: 'SpreadoverDuration',
            title: 'Total Spreadover'
        }];

        _grid = _$grid.kendoGrid({
            dataSource: {
                transport: {
                    read: {
                        url: '/api/drivertime'
                    },
                    parameterMap: function (data) {
                        return {
                            fromDate: _dteStartDate.get_selectedDate().format('yyyy-MM-dd'),
                            toDate: _dteEndDate.get_selectedDate().format('yyyy-MM-dd'),
                            driverID: _cboDriver.get_value(),
                            pageSize: data.pageSize,
                            skip: data.skip
                        };
                    }
                },
                schema: {
                    model: {
                        fields: {
                            Day: { type: 'date' }
                        }
                    },
                    data: 'GridData',
                    total: 'RecordCount'
                },
                requestEnd: function (e) {
                    displayLatestDateStamp(e.response);
                    generateFooter(e.response);
                },
                error: function (e) {
                    alert(e.errorThrown || e.status);
                },
                serverPaging: true,
                pageSize: 10
            },
            filterable: false,
            sortable: false,
            scrollable: false,
            pageable: {
                pageSizes: false
            },
            columns: columnDefinitions
        }).data('kendoGrid');

        // Set grid column header tooltips
        _$grid.find('thead tr th[data-field=TotalWorkingTime]').attr('title', 'Total of Other Work and Drive');
        _$grid.find('thead tr th[data-field=SpreadoverDuration]').attr('title', 'Total of Break, POA, Other Work and Drive');
    }

    function generateFooter(response) {
        _$grid.find(".k-grid-footer").remove();

        if (!response || !response.RestDurationTotal) {
            return;
        }

        var restTotal = "<b>" + response.RestDurationTotal + "</b>";
        var availableTotal = "<b>" + response.AvailableTotal + "</b>";
        var workTotal = "<b>" + response.WorkTotal + "</b>";
        var driveTotal = "<b>" + response.DriveTotal + "</b>";
        var spreadoverTotal = "<b>" + response.SpreadoverTotal + "</b>";
        var totalWorkingTimeTotal = "<b>" + response.TotalWorkingTimeTotal + "</b>";
        _$grid.find("table").append("<tfoot class=\"k-grid-footer\"><tr class=\"k-footer-template\"><td><b>Totals:</b></td><td>" + restTotal + "</td><td>" + availableTotal + "</td><td>"+ workTotal +"</td><td>"+driveTotal+"</td><td>"+totalWorkingTimeTotal+"</td><td>"+spreadoverTotal+"</td></tr></tfoot>");
    }

    var findComponent = function (selector) {
        var element = $(selector, _$driverTimeDialog).get(0);
        return element ? $find(element.id) : null;
    };

    var displayLatestDateStamp = function (response) {
        _latestDateStamp.html('');

        if (response) {
            var dateStamp = response.MostRecentDriverDateStamp;
            var mostRecentVerifiedDateStamp = response.MostRecentVerifiedDateStamp

            if (dateStamp) {
                _latestDateStamp.html('latest Non Verified date/time : ' + moment(dateStamp).format('DD/MM/YYYY HH:mm'));
            }
            else if (_cboDriver.get_value()) {
                _latestDateStamp.html('no data exists for this driver');
            }
            else {
                _latestDateStamp.html('please select a driver');
            }

            if (mostRecentVerifiedDateStamp)
            {
                _latestVerifiedDateStamp.html('latest Verified date/time: ' + moment(mostRecentVerifiedDateStamp).format('DD/MM/YYYY HH:mm'))
            }
        }
    }

    initialise();
};

function cboDriverTimeDriver_ItemsRequesting(sender, eventArgs) {
    // Setting ItemsPerRequest to zero ensures all items are retrieved in one request
    eventArgs.get_context()["ItemsPerRequest"] = 0;
}

function cboDriverTimeDriver_ItemsRequested(sender, eventArgs) {
    // Select the combo item that matches the combo's current value.
    var selectedDriverID = sender.get_value();

    if (selectedDriverID) {
        var item = sender.findItemByValue(selectedDriverID);
        item.select();
    }
}
