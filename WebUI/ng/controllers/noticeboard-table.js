
'use strict';

angular.module('peApp').controller('NoticeboardTableCtrl', ['$scope', 'DTOptionsBuilder', 'DTColumnDefBuilder', function ($scope, DTOptionsBuilder, DTColumnDefBuilder) {

    var firstTableInit = true;

    // ========== Helper Functions ================

    function loadStateParams(settings, data) {
        // first time the table loads clear the table state, so default ordering applies
        // We always want to see the table order by timestamp initially
        if ($scope.layoutState.firstTableInit) {         
            data.order = [[4,'desc']];
            $scope.layoutState.firstTableInit = false;
        }
    }

    // ========== Scope Functions =============
    $scope.showVehicleHistory = function ($event, vehicle) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.$emit('showVehicleHistory', vehicle)
    };

    $scope.keyPress = function ($event) {
        if ($event.keyCode == 13)
            $scope.$emit('mapOptions.vehicle.historicalSearch');
    };  

    $scope.setSearchMode = function ($event, searchMode) {
        $event.preventDefault();
        $event.stopPropagation();
        
        if (searchMode == 'noticeboard') {
            $scope.mapOptions.vehicle.isMapSearchEnabled = false;
        }
        else {
            $scope.mapOptions.vehicle.isMapSearchEnabled = true;
        }
        $scope.mapOptions.vehicle.searchMode = searchMode;

        $scope.$emit('doSearchOrFilter');
    };

    $scope.doHistoricalSearch = function () {
        $scope.$emit('mapOptions.vehicle.historicalSearch');
    };

    $scope.stopTracking = function ($event, vehicle) {
        $event.preventDefault();
        $event.stopPropagation();
        
        $scope.mapData.vehicle.trackedPinId = null;
    };

    $scope.toggleTracking = function ($event, vehicle) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.mapData.vehicle.trackedPinId == null ? $scope.mapData.vehicle.trackedPinId = vehicle.id : $scope.mapData.vehicle.trackedPinId = null;
    };

    $scope.resizeTable = function () {
        if ($scope.mapOptions.mode === 'noticeboard' && $scope.noticeboardDataTable.instance.dataTable) {
            $scope.noticeboardDataTable.instance.dataTable.fnAdjustColumnSizing();
        }   
    }

    $scope.toggleFilterMode = function() {
        $scope.mapOptions.vehicle.isMapSearchEnabled = !$scope.mapOptions.vehicle.isMapSearchEnabled
    }

    $scope.getReasonColorStyle = function (reasonCode) {
        return { 'background-color': $scope.mapOptions.legend.getLegendItem(reasonCode).fillColour }
    }

    $scope.selectPin = function (pin) {
        var zoomCoords = [pin];
        $scope.zoomToCoordinates(zoomCoords);
        $scope.mapOptions.zoom = 12;
        $scope.mapData.vehicle.selectedPinId = pin.id;
    }

    // ========== Watches =====================

    // ========== Initialisation ===========   

    $scope.noticeboardDataTable = {
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
                'sEmptyTable':     'No vehicle data available.'
            }),

        columnDefs: [
            DTColumnDefBuilder.newColumnDef(0).withOption('width', '11%'),
            DTColumnDefBuilder.newColumnDef(1).withOption('width', '13%'),
            DTColumnDefBuilder.newColumnDef(2).withOption('width', '9%').withOption('iDataSort', 7),
            DTColumnDefBuilder.newColumnDef(3).withOption('width', '35%'),
            DTColumnDefBuilder.newColumnDef(4).withOption('width', '9%').withOption('iDataSort', 8),
            DTColumnDefBuilder.newColumnDef(5).withOption('width', '13%'),
            DTColumnDefBuilder.newColumnDef(6).withOption('width', '10%'),
            DTColumnDefBuilder.newColumnDef(7).withOption('width', '0px').notVisible(),
            DTColumnDefBuilder.newColumnDef(8).withOption('width', '0px').notVisible()
        ]

    };

    

  }]);
