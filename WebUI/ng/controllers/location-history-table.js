
'use strict';

angular.module('peApp').controller('LocationHistoryTableCtrl', ['$scope', 'DTOptionsBuilder', 'DTColumnDefBuilder', function ($scope, DTOptionsBuilder, DTColumnDefBuilder) {


    // ========== Helper Functions ================

    function loadStateParams(settings, data) {
        // first time the table loads clear the table state, so default ordering applies
        // We always want to see the table order by timestamp initially
        if ($scope.layoutState.firstTableInit) {
            data.order = [[1, 'desc']];
            $scope.layoutState.firstTableInit = false;
        }
    }

    // ========== Scope Functions =============

    $scope.openToDatePicker = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.state.isToDatePickerOpened = !$scope.state.isToDatePickerOpened;
        $scope.state.isFromDatePickerOpened = false;
    };

    $scope.openFromDatePicker = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.state.isFromDatePickerOpened = !$scope.state.isFromDatePickerOpened;
        $scope.state.isToDatePickerOpened = false;
    };


    $scope.resizeTable = function () {
        if ($scope.mapOptions.mode === 'locationHistory' && $scope.locationHistoryDataTable.instance.dataTable) {
            $scope.locationHistoryDataTable.instance.dataTable.fnAdjustColumnSizing();
        }
    }

    // ========== Watches ==================

    $scope.$watch('layoutState.isLoading', function (isLoading) {
        if ($scope.mapOptions.mode === 'locationHistory') {
            if (isLoading) {
                $scope.locationHistoryDataTable.instance._renderer.options.oLanguage.sEmptyTable = 'Loading data...';
                if ($scope.tableData.length === 0) {
                    $scope.locationHistoryDataTable.instance.rerender();
                }
                else {
                    $scope.tableData = []
                }
            }
            else {
                if ($scope.mapData.locationHistory.tableData.length === 0) {
                    $scope.locationHistoryDataTable.instance._renderer.options.oLanguage.sEmptyTable = 'No location history for the specified date range and radius.';
                    $scope.locationHistoryDataTable.instance.rerender();
                }
                else {
                    $scope.tableData = $scope.mapData.locationHistory.tableData;
                }
            }

        }
    });


    // ========== Initialisation ===========   

    $scope.locationHistoryDataTable = {
        instance: {},
        options: DTOptionsBuilder.newOptions()
            .withOption('bStateSave', true)
            .withOption('bStateDuration', -1) // use session storage to store state (order, column visibility)
            .withOption('bAutowidth', false)
            .withOption('searching', false)
            .withOption('paging', false)
            .withOption('scrollY', 218)
            .withOption('info', false)
            .withOption('stateLoadParams', loadStateParams)
            .withLanguage({
                "sEmptyTable":     "Loading data..."
            }),
        columnDefs: [
            DTColumnDefBuilder.newColumnDef(0).withOption('width', '30%'),
            DTColumnDefBuilder.newColumnDef(1).withOption('width', '20%').withOption('iDataSort', 3),
            DTColumnDefBuilder.newColumnDef(2).withOption('width', '50%'),
            DTColumnDefBuilder.newColumnDef(3).withOption('width', '0px').notVisible()
        ]
    };

    $scope.state = {
        isToDatePickerOpened: false,
        isFromDatePickerOpened: false
    }

    $scope.timePickerOptions = {
        timeFormat: 'H:i'
    };

    $scope.tableData = [{}];


  }]);
