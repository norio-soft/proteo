
'use strict';

angular.module('peApp').controller('VehicleHistoryTableCtrl', ['$scope', 'DTOptionsBuilder', 'DTColumnDefBuilder', '$timeout' , function ($scope, DTOptionsBuilder, DTColumnDefBuilder, $timeout) {


    // ========== Helper Functions ================

    function loadStateParams(settings, data) {
        // first time the table loads clear the table state, so default ordering applies
        // We always want to see the table order by timestamp initially
        if ($scope.layoutState.firstTableInit) {
            data.order = [[0, 'desc']];
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
        if ($scope.mapOptions.mode === 'vehicleHistory' && $scope.vehicleHistoryDataTable.instance.dataTable) {
            $scope.vehicleHistoryDataTable.instance.dataTable.fnAdjustColumnSizing();
        }
    }

    $scope.getReasonColorStyle = function (reasonCode) {
        return { 'background-color': $scope.mapOptions.legend.getLegendItem(reasonCode).fillColour }
    }

    $scope.selectPin = function (pin) {
        var zoomCoords = [pin];
        $scope.zoomToCoordinates(zoomCoords)
        $scope.mapData.vehicleHistory.selectedPinId = pin.id;
        $scope.mapOptions.zoom = 13;
    }

    // ========== Watches ==================

    $scope.$watch('layoutState.isLoading', function (isLoading) {
        if ($scope.mapOptions.mode === 'vehicleHistory') {
            if (isLoading) {
                $scope.vehicleHistoryDataTable.instance._renderer.options.oLanguage.sEmptyTable = 'Loading data...';
                if ($scope.tableData.length === 0) {
                    $scope.vehicleHistoryDataTable.instance.rerender();
                }
                else {
                    $scope.tableData = []
                }
            }
            else {
                if ($scope.mapData.vehicleHistory.tableData.length === 0) {
                    $scope.vehicleHistoryDataTable.instance._renderer.options.oLanguage.sEmptyTable = 'No vehicle history for the specified date range.';
                    $scope.vehicleHistoryDataTable.instance.rerender();
                }
                else {
                    $scope.tableData = $scope.mapData.vehicleHistory.tableData;
                }
            }
                     
        }
    });


    // ========== Initialisation ===========   



    $scope.vehicleHistoryDataTable = {
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
                "sEmptyTable": "Loading data...",
            }),
        columnDefs: [
            DTColumnDefBuilder.newColumnDef(0).withOption('width', '15%').withOption('iDataSort', 4),
            DTColumnDefBuilder.newColumnDef(1).withOption('width', '15%').withOption('iDataSort', 5),
            DTColumnDefBuilder.newColumnDef(2).withOption('width', '50%'),
            DTColumnDefBuilder.newColumnDef(3).withOption('width', '20%'),
            DTColumnDefBuilder.newColumnDef(4).withOption('width', '0px').notVisible(),
            DTColumnDefBuilder.newColumnDef(5).withOption('width', '0px').notVisible()
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
