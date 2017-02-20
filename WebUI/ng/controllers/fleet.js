
'use strict';

angular.module('peApp').controller('FleetCtrl', [
    '$scope', '$window', 'apiService', 'mapsService', '$interval', '$filter', 'legacyAppWindowService',
    function ($scope, $window, apiService, mapsService, $interval, $filter, legacyAppWindowService) {

        var bottomPanelHeight = 323;
        var sidePanelWidth = 250;


        // ========== Helper Functions ================

        function getNoticeboardData(zoomAndCenter) {

            $scope.layoutState.isLoading = true;

            apiService.gpsPosition.get()
                .$promise.then(function (result) {

                    //result = result.slice(1, 4);

                    $scope.mapData.vehicle.unfilteredVehicles = result.map(function (val) {

                        val.lat = val.latitude; 
                        val.lng = val.longitude;
                        val.id = val.gpsUnitID;
                        val.options = {};

                        if ($scope.isClientPortalUser) {
                            // Don't display the run ID to client users
                            val.currentJobID = null;
                        }

                        val.viewRun = function () {
                            if (val.currentJobID !== null) {
                                legacyAppWindowService.viewRun(val.currentJobID);
                            }
                        };

                        delete val.gpsUnitID;
                        delete val.latitude;
                        delete val.longitude;
                        return val;
                    });

                    if (areFiltering()) {
                        $scope.mapData.vehicle.tableData = $scope.mapData.vehicle.vehicles = getFilteredVehicles();
                    } else {
                        $scope.mapData.vehicle.tableData = $scope.mapData.vehicle.vehicles = $scope.mapData.vehicle.unfilteredVehicles;
                    }

                    if (zoomAndCenter) {
                        $scope.zoomToCoordinates($scope.mapData.vehicle.vehicles);
                    }
                    else if ($scope.mapData.vehicle.trackedPinId) {
                        var trackedPin = getPin($scope.mapData.vehicle.trackedPinId);
                        if (trackedPin) {
                            $scope.zoomToCoordinates([trackedPin]);
                        }
                    }

                    $scope.layoutState.isLoading = false;

                });      
        }

        function getVehicleHistoryData() {
         
            $scope.layoutState.isLoading = true;
            
            var apiParams = {
                gpsUnitId: $scope.mapOptions.vehicleHistory.vehicle.id,
                startDateTime: $scope.mapOptions.vehicleHistory.startDateTime,
                endDateTime: $scope.mapOptions.vehicleHistory.endDateTime
            };

            apiService.gpsPositionHistory.get(apiParams)
                .$promise.then(function (result) {

                    var histories = result.map(mapGPSPosition);

                    $scope.zoomToCoordinates(histories);

                    $scope.mapData.vehicleHistory.tableData = $scope.mapData.vehicleHistory.gpsPositionHistories = histories;
                    $scope.layoutState.isLoading = false;

                });

        }

        function getLocationHistoryData() {
            
            $scope.layoutState.isLoading = true;

            drawRadii();

            var center = {
                lat: $scope.mapOptions.locationHistory.latitude,
                lng: $scope.mapOptions.locationHistory.longitude
            }

            $scope.zoomToCoordinates([center]);


            if ($scope.mapOptions.vehicle.historicalSearch) {
            apiService.gpsPositionLocationHistory.get($scope.mapOptions.locationHistory)
                .$promise.then(function (result) {

                        $scope.mapData.locationHistory.tableData = result.map(function (val) {

                        val.lat = val.latitude;
                            val.lng = val.longitude;
                        val.id = val.gpsUnitID;
                        val.options = {};

                        delete val.gpsUnitID;
                        delete val.latitude;
                        delete val.longitude;
                        return val;
                    });

                    $scope.layoutState.isLoading = false;

                });
        }
        }

        function queryIsClientPortalUser() {
            $scope.isClientPortalUser = false;
            apiService.account.canAccessAll({ systemPortionIDs: [window.enums.systemPortion.clientPortalUser] }).$promise.then(
                function (response) {
                    $scope.isClientPortalUser = response.canAccess;
                });
        }

        function getPin(pinId) {
            return _.find($scope.mapData.vehicle.vehicles, function (pin) {
                return pin.id === pinId;
            });
        }

        function mapGPSPosition(val, index, array) {
            val.lat = val.latitude;
            val.lng = val.longitude;
            val.id = val.gpsPositionHistoryID;

            if (index === 0) {
                val.keyEventType = 'start';
            }
            else if (index === (array.length - 1)) {
                val.keyEventType = 'finish';
            }
            else {
                if (val.reasonCode === 6) {
                    val.keyEventType = 'ignitionOn';
                }
                else if (val.reasonCode === 7) {
                    val.keyEventType = 'ignitionOff';
                }
            }



            var colourIndex = Math.round((index / array.length) * 99);
            var colour = historyColours[colourIndex];
            val.colour = rgbToHex(Math.round(colour._r), Math.round(colour._g), Math.round(colour._b));

            var borderColour = historyBorderColours[colourIndex];
            val.borderColour = rgbToHex(Math.round(borderColour._r), Math.round(borderColour._g), Math.round(borderColour._b));

            delete val.gpsPositionHistoryID;
            delete val.latitude;
            delete val.longitude;
            return val;
        }

        function componentToHex(c) {
            var hex = c.toString(16);
            return hex.length === 1 ? "0" + hex : hex;
        }

        function drawRadii() {
            $scope.mapData.locationHistory.radii = [
             {
                 id: 1,
                 lat: $scope.mapOptions.locationHistory.latitude,
                 lng: $scope.mapOptions.locationHistory.longitude,
                 radius: $scope.mapOptions.locationHistory.radius,
               
             },
              {
                  id: 2,
                  lat: $scope.mapOptions.locationHistory.latitude,
                  lng: $scope.mapOptions.locationHistory.longitude,
                  radius: $scope.mapOptions.locationHistory.radius / 2.0,
              },
               {
                   id: 3,
                   lat: $scope.mapOptions.locationHistory.latitude,
                   lng: $scope.mapOptions.locationHistory.longitude,
                   radius: $scope.mapOptions.locationHistory.radius / 10.0,
               }

            ];
        }

        function rgbToHex(r, g, b) {
            return "#" + componentToHex(r) + componentToHex(g) + componentToHex(b);
        }

        function switchMode() {

            $scope.layoutState.firstTableInit = true;

            switch ($scope.mapOptions.mode) {
                case 'noticeboard':
                    $scope.mapData.locationHistory.radii = [];
                    $scope.mapData.vehicleHistory.gpsPositionHistories = [];
                    getNoticeboardData(true);
                    startAutoRefresh();
                    $scope.noticeBoardButtonCaption = 'Reset';                    
                    break;
                case 'vehicleHistory':
                    stopAutoRefresh();
                    $scope.layoutState.sidePanelOpen = false;
                    $scope.mapData.locationHistory.radii = [];
                    $scope.mapData.vehicle.vehicles = [];
                    getVehicleHistoryData();
                    $scope.noticeBoardButtonCaption = 'Back to Noticeboard';                   
                    break;
                case 'locationHistory':
                    stopAutoRefresh();
                    $scope.layoutState.sidePanelOpen = false;
                    $scope.mapData.vehicleHistory.gpsPositionHistories = [];
                    $scope.mapData.vehicle.vehicles = [];
                    getLocationHistoryData();
                    $scope.noticeBoardButtonCaption = 'Back to Noticeboard';                   
                    break;

            }
        }

        function doSearch(){
            if ($scope.mapOptions.vehicle.filterText) {
                var locationPromise = mapsService.locationQuery($scope.mapSetup.credentials, $scope.mapOptions.vehicle.filterText);
                locationPromise.then(function (locations) {
                    if (locations.length > 0) {
                        var coords = locations[0].location.displayPosition
                        var centre = { lat: coords.latitude, lng: coords.longitude };
                        $scope.zoomToCoordinates([centre]);
                        showLocationHistory(centre);
                    }
                });
            }
            else {
                //no search text, zoom back to default view of all vehicles
                $scope.zoomToCoordinates($scope.mapData.vehicle.vehicles);
            }
        }

        function doSearchOrFilter() {
            if ($scope.mapOptions.vehicle.isMapSearchEnabled) {
                $scope.mapData.vehicle.tableData = $scope.mapData.vehicle.vehicles = $scope.mapData.vehicle.unfilteredVehicles;
                doSearch();
            }
            else {

                if (areFiltering()) {
                    $scope.mapData.vehicle.tableData = $scope.mapData.vehicle.vehicles = getFilteredVehicles();
                } else {
                    $scope.mapData.vehicle.tableData = $scope.mapData.vehicle.vehicles = $scope.mapData.vehicle.unfilteredVehicles;
                }

                // kludge to deal with clearing of vehicle view filter zooming to
                // show all vehices when a single vehicle has been selected in
                // the vehicle view. Selection needs to take precedence over clearing of filter;
                if ($scope.mapOptions.zoomToSelectedRequested) {
                    $scope.mapOptions.zoomToSelectedRequested = false;
                }
                else {
                    $scope.mapData.vehicle.selectedPinId = null;
                    $scope.mapData.vehicle.trackedPinId = null;
                    $scope.zoomToCoordinates($scope.mapData.vehicle.vehicles);
                }
            }
        }
        
        function getFilteredVehicles()
        {
            return $filter('gpsPositionVehicleViewFilter')($filter('gpsPositionFilter')($scope.mapData.vehicle.unfilteredVehicles, $scope.mapOptions.vehicle.filterText), $scope.mapOptions.vehicle.vehicleViewFilter);
            
        }

        function areFiltering() {
            return (($scope.mapOptions.vehicle.filterText ||
                   $scope.mapOptions.vehicle.vehicleViewFilter.length > 0) && !$scope.mapOptions.vehicle.isMapSearchEnabled);
        }
        
        function showLocationHistory(location) {
            $scope.mapOptions.locationHistory = {
                startDateTime: moment().subtract(1, 'days').startOf('day').toDate(),
                endDateTime: moment().toDate(),
                latitude: location.lat,
                longitude: location.lng,
                radius: 10
            };
            if ($scope.mapOptions.mode === 'locationHistory') {
                getLocationHistoryData();
            }
            else {
                $scope.mapOptions.zoom = 11;

                if ($scope.mapOptions.vehicle.historicalSearch)
                $scope.mapOptions.mode = 'locationHistory';
                else 
                    drawRadii();
            }
        }

        // ============ Auto-refresh =============
        var refresher;

        var startAutoRefresh = function () {
            refresher = $interval(function () {
                getNoticeboardData();
            }, 60000);
        };

        var resetAutoRefresh = function () {
            stopAutoRefresh();startAutoRefresh 
            startAutoRefresh();
        };

        var stopAutoRefresh = function () {
            if (angular.isDefined(refresher)) {
                $interval.cancel(refresher);
                refresher = undefined;
            }
        };

        var setMapContainerSize = function () {
            $scope.layoutState.mapContainerHeight = $window.innerHeight - $scope.layoutState.bottomPanelHeight;
            $scope.layoutState.mapContainerWidth = $window.innerWidth - $scope.layoutState.sidePanelWidth;
            $scope.hereMapResize = !$scope.hereMapResize; // toggling this value will cause the Here Map's viewport to be redrawn
        };


        // ========== Scope Functions =============

        $scope.refreshNoticeboard = function () {
            getNoticeboardData();
        };

        $scope.refreshVehicleHistory = function () {
            getVehicleHistoryData();
        };

        $scope.refreshLocationHistory = function () {
            getLocationHistoryData();
        };

        // zooms to a bounding boz specified by the array of coordinates
        // if there is only one point then the map is centred on the map
        // point but zoom is not affected.
        $scope.zoomToCoordinates = function(coordinates) {
            if (coordinates.length > 0) {
                $scope.mapOptions.viewCoordinates = _.map(coordinates, function (coord) { return { lat: coord.lat, lng: coord.lng }; });
            }
        };

        $scope.backToNoticeboard = function () {

            $scope.clearFilter();

            if ($scope.mapOptions.mode === 'noticeboard') {
                // reset the noticeboard
                $scope.mapData.vehicle.selectedPinId = null;
                $scope.mapData.vehicle.trackedPinId = null;
                $scope.zoomToCoordinates($scope.mapData.vehicle.vehicles);
                $scope.layoutState.firstTableInit = true;
                getNoticeboardData();
            }
            else {
                $scope.mapOptions.mode = 'noticeboard';
            }
        };

        $scope.toggleBottomPanel = function () {
            $scope.layoutState.bottomPanelOpen = !$scope.layoutState.bottomPanelOpen;
        };

        $scope.toggleSidePanel = function () {
            $scope.layoutState.sidePanelOpen = !$scope.layoutState.sidePanelOpen;
        };

        $scope.toggleFullScreen = function () {
            $scope.layoutState.isFullscreen = !$scope.layoutState.isFullscreen;
        };

        $scope.clearFilter = function () {
            $scope.mapOptions.vehicle.filterText = "";
            $scope.mapOptions.vehicle.vehicleViewFilter = [];
            $scope.mapData.locationHistory.radii = [];
        };

        $scope.resizeMap = function () {
            setMapContainerSize();
        };


        // ========== Watches =====================

        $scope.$watch('mapOptions.mode', switchMode);

        $scope.$on('showVehicleHistory', function (event, vehicle) {

            $scope.mapOptions.vehicleHistory = {
                vehicle: vehicle,
                startDateTime: moment().startOf('day').toDate(),
                endDateTime: moment().toDate()
            };

            $scope.mapOptions.mode = 'vehicleHistory';
        });

        $scope.$on('showLocationHistory', function (event, location) {
            $scope.mapOptions.vehicle.historicalSearch = true;
            showLocationHistory(location);
        });    

        $scope.$on('mapOptions.vehicle.historicalSearch', function () {
            doSearchOrFilter();
        });

        $scope.$watch('mapOptions.vehicle.filterText', function () {
            doSearchOrFilter();       
        });

        $scope.$watch('mapOptions.vehicle.vehicleViewFilter', function () {
            doSearchOrFilter();
        });

        $scope.$watch('mapOptions.vehicle.isMapSearchEnabled', function () {
            doSearchOrFilter();
        });

        $scope.$watch('layoutState.bottomPanelOpen', function (newValue) {
            $scope.layoutState.bottomPanelHeight = newValue ? bottomPanelHeight : 0;
            setMapContainerSize();
        });

        $scope.$watch('layoutState.sidePanelOpen', function (newValue) {
            $scope.layoutState.sidePanelWidth = newValue ? sidePanelWidth : 0;
            setMapContainerSize();
        });


        // ========== Initialisation ===========

        var historyColourGradient = tinygradient([
                  { color: '#5566d6', pos: 0 },
                  { color: '#3ea568', pos: 1 }
        ]);
        var historyColours = historyColourGradient.rgb(100);
        var historyBorderColourGradient = tinygradient([
                  { color: '#3e467a', pos: 0 },
                  { color: '#3b7753', pos: 1 }
        ]);
        var historyBorderColours = historyBorderColourGradient.rgb(100);

        $scope.layoutState = {
            bottomPanelHeight: bottomPanelHeight,
            sidePanelWidth: 0,
            bottomPanelOpen: true,
            sidePanelOpen: false,
            isFullscreen: false,
            isFirstTableInit: true,
        };

        $scope.mapSetup = {};
        $scope.mapSetup.credentials = { app_id: "j8Wgmhi8ntrfHwEgHiFC", app_code: "JpK4aIcgRI4b6hkgCWJBiQ" }
        $scope.mapOptions = {};
        $scope.mapOptions.center = { latitude: 52.627954, longitude: 1.288239 };
        $scope.mapOptions.zoom = 8;
        $scope.mapOptions.vehicleHistory = {};
        $scope.mapOptions.vehicle = {
            filterText: '',
            isMapSearchEnabled: false,
            historicalSearch: false,
            filterOptions:{
                updateOn: 'change blur'
            },
            vehicleViewFilter: []
        };

        $scope.mapOptions.legend = {
            getLegendItem: function (reasonCode) {
                if (reasonCode === 7 || reasonCode === 3) {
                    return {
                        fillColour: '#f78a62',
                        borderColour: '#b26143'
                    };
                }
                else {
                    return {
                        fillColour: '#6ac990',
                        borderColour: ' #4e966b'
                    };
                }
            }
        };
		
        $scope.mapData = {
            vehicle: {
                selectedPinId: null,
                trackedPinId: null,
                vehicles: [],
                unfilteredVehicles: [],
            },
            vehicleHistory: {
                selectedPinId: null,
                gpsPositionHistories: [],
                tableData: []
            },
            locationHistory: {
                radii: [],
                tableData: []
            },
            vehicleViews: []

        };

        //setting the mode to noticeboard by default
        $scope.mapOptions.mode = 'noticeboard';
        //check for client portal users
        queryIsClientPortalUser();

        setMapContainerSize();

    }
]);
