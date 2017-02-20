'use strict';

angular.module('peApp').controller('RunRouteCtrl', ['$scope', '$stateParams', '$timeout', '$window', 'DTOptionsBuilder', 'apiService', 'mapsService',
    function ($scope, $stateParams, $timeout, $window, DTOptionsBuilder, apiService, mapsService) {

        var jobID = $stateParams.jobID;
        var instructions = [];

        var actualRouteColorARGB = { a: 223, red: 255, green: 63, blue: 63 };
        var estimatedRouteColorARGB = { a: 223, red: 255, green: 223, blue: 63 };
        var estimatedRouteHighlightColorARGB = { a: 255, red: 223, green: 255, blue: 0 };

        $scope.alerts = [];
        $scope.nonGeocodedPoints = [];

        var bottomPanelHeight = 260;


        // ========== Functions ================

        var getData = function () {
            $scope.mapData.pins = [];
            $scope.mapData.actualRoute = [];
            $scope.mapData.estimatedRoute = [];

            var legsPromise = apiService.job.getLegsForRunRoute({ jobID: jobID }).$promise;

            legsPromise.then(function (result) {
                var legs = result.map(function (leg, index) {
                    leg.index = index;
                    return leg;
                });

                $scope.runLegs = legs;
                redrawLegDataTable();

                loadInstructions();

                var nonGeocodedPoints = getNonGeocodedPoints();
                $scope.nonGeocodedPoints = nonGeocodedPoints;

                drawActualRoute();

                if (nonGeocodedPoints.length === 0) {
                    drawEstimatedRoute();
                }

                displayIgnitionOffEvents();
                getETAs();

                var pins = instructions.map(function (instruction, index) {
                    return {
                        id: instruction.instructionID,
                        instructionNumber: index + 1,
                        lat: instruction.latitude,
                        lng: instruction.longitude,
                        location: instruction.location,
                        plannedArrivalDateTime: instruction.plannedArrivalDateTime,
                        actualArrivalDateTime: instruction.actualArrivalDateTime,
                    };
                });

                addPins(pins);

                $scope.layoutState.bottomPanelOpen = true;
            },
            function (error) {
                var message = error.status === 404 ? ('Could not find run ' + jobID) : ('Could not load run ' + jobID);
                addAlert(message, true);
            });
        };

        var loadInstructions = function () {
            instructions = $scope.runLegs.map(function (leg) {
                return {
                    instructionID: leg.endInstructionID,
                    pointID: leg.endInstructionPointID,
                    location: leg.endInstructionLocation,
                    latitude: leg.endInstructionLatitude,
                    longitude: leg.endInstructionLongitude,
                    plannedArrivalDateTime: leg.endInstructionPlannedArrivalDateTime,
                    actualArrivalDateTime: leg.endInstructionActualArrivalDateTime,
                };
            });

            var firstLeg = $scope.runLegs[0];

            instructions.unshift({
                instructionID: firstLeg.startInstructionID,
                pointID: firstLeg.startInstructionID,
                location: firstLeg.startInstructionLocation,
                latitude: firstLeg.startInstructionLatitude,
                longitude: firstLeg.startInstructionLongitude,
                plannedArrivalDateTime: firstLeg.startInstructionPlannedArrivalDateTime,
                actualArrivalDateTime: firstLeg.startInstructionActualArrivalDateTime,
            });
        };

        var getNonGeocodedPoints = function () {
            return instructions
                .filter(function (instruction) {
                    return instruction.latitude === 0 || instruction.longitude === 0;
                })
                .map(function (instruction) {
                    return {
                        pointID: instruction.pointID,
                        description: instruction.location,
                    };
                });
        };

        var drawActualRoute = function () {
            var routeLegs = $scope.runLegs.map(function(runLeg, index) {
                return {
                    id: 'a' + index,
                    coordinates: runLeg.dataPoints.map(function (dataPoint) {
                        return {
                            lat: dataPoint.latitude,
                            lng: dataPoint.longitude,
                        };
                    }),
                    endInstructionID: runLeg.endInstructionID,
                };
            });

            $scope.mapData.actualRoute = createRouteCollectionForPolyline(routeLegs, actualRouteColorARGB);
        };

        var displayIgnitionOffEvents = function () {
            var legIgnitionOffEvents = $scope.runLegs.map(function (runLeg) {
                return runLeg.dataPoints.filter(function (dataPoint) {
                    return dataPoint.isIgnitionOff;
                });
            });

            var pins = _.flatten(legIgnitionOffEvents).map(function (ignitionOffEvent, index) {
                return {
                    id: 'io' + index,
                    keyEventType: 'ignitionOff',
                    lat: ignitionOffEvent.latitude,
                    lng: ignitionOffEvent.longitude,
                    ignitionOffDateStamp: ignitionOffEvent.dateStamp,
                };
            });

            addPins(pins);
        };

        var drawEstimatedRoute = function () {
            var lastPointID = null;
            var waypoints = [];
            var instructionToRouteLegMapping = [];

            instructions.forEach(function (instruction) {
                if (instruction.pointID !== lastPointID) {
                    instructionToRouteLegMapping[instruction.instructionID] = waypoints.length - 1;
                    waypoints.push({ lat: instruction.latitude, lng: instruction.longitude });
                    lastPointID = instruction.pointID;
                }
            });

            var routePromise = mapsService.calculateRoute($scope.mapSetup.credentials, waypoints);

            routePromise.then(
                function (routeLegs) {
                    if (routeLegs.length === 0) {
                        return;
                    }

                    routeLegs.forEach(function (routeLeg, index) {
                        routeLeg.id = 'e' + index;
                    });

                    // Set the estimated distance and duration values
                    $scope.runLegs.forEach(function (runLeg) {
                        var routeLeg = routeLegs[instructionToRouteLegMapping[runLeg.endInstructionID]];

                        if (routeLeg !== undefined) {
                            runLeg.estimatedDistance = routeLeg.travelDistance;
                            runLeg.estimatedDuration = routeLeg.travelDuration;

                            routeLeg.endInstructionID = runLeg.endInstructionID;
                        }
                    });

                    // Draw the returned route and zoom to show the whole route.
                    $scope.mapData.estimatedRoute = createRouteCollectionForPolyline(routeLegs, estimatedRouteColorARGB);
                    zoomToRoute(routeLegs);
                },
                function (error) {
                    addAlert('Could not calculate estimated route: ' + error, true);
                    zoomToCoordinates(waypoints);
                });
        };

        var getETAs = function () {
            for (var i = 0; i < $scope.runLegs.length; i++) {
                var leg = $scope.runLegs[i];

                if (leg.endInstructionStateID < 4 && leg.vehicleResourceID !== null) {
                    var currentPositionPromise = apiService.vehicle.getCurrentPosition({ vehicleResourceID: leg.vehicleResourceID }).$promise;

                    currentPositionPromise.then(function (result) {
                        showVehicleCurrentPosition(result.latitude, result.longitude, leg.vehicleRegistration);
                        calculateRouteForETAs(leg.vehicleResourceID, result.latitude, result.longitude);
                    });

                    break;
                }
            }
        };

        var calculateRouteForETAs = function (vehicleResourceID, currentLat, currentLong) {
            //TODO: the truth is I'm transcoding directly here from C# code that I don't know how it works and don't have time to find out - this needs reviewing!
            var lastPointID = null;
            var estimatedLegs = [];
            var waypoints = [ { lat: currentLat, lng: currentLong } ];
            var skipFirstInstructionForCurrentResource = false;

            $scope.runLegs.forEach(function (leg) {
                if (leg.endInstructionStateID < 4) {
                    if (leg.vehicleResourceID === vehicleResourceID && !skipFirstInstructionForCurrentResource) {
                        // Use the vehicle's current position as the start of the leg
                        if (leg.endInstructionPointID !== lastPointID) {
                            estimatedLegs.push(leg);
                            waypoints.push({ lat: leg.endInstructionLatitude, lng: leg.endInstructionLongitude });
                            lastPointID = leg.endInstructionPointID;
                        }

                        skipFirstInstructionForCurrentResource = true;
                    }
                    else {
                        if (leg.startInstructionPointID !== lastPointID) {
                            waypoints.push({ lat: leg.startInstructionLatitude, lng: leg.startInstructionLongitude });
                            lastPointID = leg.startInstructionPointID;
                        }

                        if (leg.endInstructionPointID !== lastPointID) {
                            estimatedLegs.push(leg);
                            waypoints.push({ lat: leg.endInstructionLatitude, lng: leg.endInstructionLongitude });
                            lastPointID = leg.endInstructionPointID;
                        }
                    }
                }
            });

            var routePromise = mapsService.calculateRoute($scope.mapSetup.credentials, waypoints);

            routePromise.then(
                function (routeLegs) {
                    if (routeLegs.length === 0) {
                        return;
                    }

                    var timeInSeconds = 0;

                    for (var i = 0; i < routeLegs.length; i++) {
                        var routeLeg = routeLegs[i];
                        var estimatedLeg = estimatedLegs[i];
                        timeInSeconds += routeLeg.travelDuration;
                        estimatedLeg.eta = moment().add(timeInSeconds, 'seconds').toDate();

                        // I have no idea what the purpose of this next few lines is.  Equivalent code is in the current Silverlight run route and it is needed here in order to generate the same ETAs
                        // that the current screen shows.  My assumption is that it may be intended to be adding in turnaround time, but I think it is actually adding in the planned expected time
                        // of the leg which I am not sure why we would want to do that?  Needs further investigation.
                        // Note that the use of planned arrival vs departure time for the start instruction determined on whether it's the first leg is the way the Silverlight implementation works but this
                        //  is actually because of the way the BusinessEntities/LegPlan.cs code works, see the "internal LegView(LegPlan plan, Entities.Instruction startInstruction)" method, currently line 439.
                        var from = i === 0 ? estimatedLeg.startInstructionPlannedArrivalDateTime : estimatedLeg.startInstructionPlannedDepartureDateTime;
                        var to = estimatedLeg.endInstructionPlannedArrivalDateTime;
                        timeInSeconds += moment.duration(moment(to).diff(from)).asSeconds();
                    }

                    estimatedLegs.forEach(function(estimatedLeg) {
                        $scope.runLegs.forEach(function (runLeg) {
                            if (estimatedLeg.endInstructionID === runLeg.endInstructionID) {
                                runLeg.eta = estimatedLeg.eta;
                                runLeg.isOverdue = moment(runLeg.endInstructionPlannedArrivalDateTime).isBefore(runLeg.eta);
                            }
                        });
                    });
                },
                function (error) {
                    addAlert('Could not calculate ETA: ' + error, true);
                });
        };

        var showVehicleCurrentPosition = function (lat, lng, vehicleRegistration) {
            var pin = {
                id: 'currentPosition',
                keyEventType: 'currentPosition',
                lat: lat,
                lng: lng,
                vehicleRegistration: vehicleRegistration,
            };

            addPins([pin]);
        };

        var addPins = function (pins) {
            Array.prototype.push.apply($scope.mapData.pins, pins);
        };

        // Populate a route collection to draw a coloured polyline
        var createRouteCollectionForPolyline = function (routeLegs, colorARGB) {
            var routeCollection = routeLegs
                .filter(function(routeLeg) {
                    return routeLeg.coordinates.length > 0;
                })
                .map(function (routeLeg) {
                    return {
                        id: routeLeg.id,
                        points: routeLeg.coordinates,
                        options: { strokeColorARGB: colorARGB },
                        legEndInstructionID: routeLeg.endInstructionID,
                    };
                });

            return routeCollection;
        };

        var zoomToRoute = function (routeLegs) {
            var routeLegCoordinates = routeLegs.map(function (routeLeg) {
                return routeLeg.coordinates;
            });

            var routeCoordinates = _.union(_.flatten(routeLegCoordinates));
            zoomToCoordinates(routeCoordinates);
        };

        var zoomToCoordinates = function (coordinates) {
            $scope.mapOptions.viewCoordinates = coordinates;
        };

        var setMapContainerSize = function () {
            $scope.layoutState.mapContainerHeight = $window.innerHeight - $scope.layoutState.bottomPanelHeight;
            $scope.hereMapResize = !$scope.hereMapResize; // toggling this value will cause the Here Map's viewport to be redrawn
        };

        $scope.toggleBottomPanel = function () {
            var openingPanel = !$scope.layoutState.bottomPanelOpen;
            $scope.layoutState.bottomPanelHeight = openingPanel ? bottomPanelHeight : 0;
            $scope.layoutState.bottomPanelOpen = openingPanel;
            setMapContainerSize();
        };

        $scope.selectLeg = function (leg) {
            $scope.mapData.estimatedRoute.forEach(function (route) {
                var isSelected = route.legEndInstructionID === leg.endInstructionID;
                updateMapRouteSelection(route, isSelected);

                if (isSelected) {
                    zoomToCoordinates(route.points);
                }
            });
        };

        var updateMapRouteSelection = function (mapRoute, isSelected) {
            var colorARGB = isSelected ? estimatedRouteHighlightColorARGB : estimatedRouteColorARGB;

            // Replace the mapRoute's options property to trigger the angular watch on polylines in prHereMapPolylines.
            // The watch is only one level deep, so changing only the strokeColorARGB or zIndex property of the options of a polyline will not trigger the watch.
            mapRoute.options = angular.extend({}, mapRoute.options, { strokeColorARGB: colorARGB, zIndex: isSelected ? 1 : 0 });
        };

        $scope.toggleActualRoute = function () {
            $scope.layoutState.actualRouteVisible = !$scope.layoutState.actualRouteVisible;
        };

        $scope.toggleEstimatedRoute = function () {
            $scope.layoutState.estimatedRouteVisible = !$scope.layoutState.estimatedRouteVisible;
        };

        $scope.refresh = function () {
            getData();
        };

        var addAlert = function (message, autoRemove) {
            $scope.alerts.push(message);

            if (autoRemove) {
                $timeout(function () {
                    $scope.closeAlert($scope.alerts.indexOf(message));
                }, 4000);
            }
        };

        $scope.closeAlert = function (alertIndex) {
            $scope.alerts.splice(alertIndex, 1);
        };

        $scope.closePointGeocodeMessage = function () {
            $scope.nonGeocodedPoints = [];
        };

        $scope.updatePoint = function (pointID) {
            var url = '/point/addupdatepoint.aspx?pointid=' + pointID;
            $window.open(url, '_blank', 'width=1000,height=900,menubar=false');
        };

        // Hacky workaround for a problem with the datatable where the column headings don't size correctly with the table body when the datatable scrollY property is set
        var redrawLegDataTable = function () {
            $timeout(function () {
                $scope.legDataTable.instance.DataTable.columns.adjust().draw();
            }, 1000);
        };

        $scope.resizeLegTable = function () {
            if ($scope.legDataTable.instance.dataTable) {
                $scope.legDataTable.instance.dataTable.fnAdjustColumnSizing();
            }
        }

        $scope.resizeMap = function () {
            setMapContainerSize();
        };


        // ========== Initialisation ===========

        $scope.layoutState = {
            bottomPanelHeight: bottomPanelHeight,
            bottomPanelOpen: false,
            actualRouteVisible: true,
            estimatedRouteVisible: true,
        };

        $scope.mapSetup = {};
        $scope.mapSetup.credentials = { app_id: "j8Wgmhi8ntrfHwEgHiFC", app_code: "JpK4aIcgRI4b6hkgCWJBiQ" }
        $scope.mapOptions = {};
        $scope.mapOptions.center = { latitude: 52.627954, longitude: 1.288239 };
        $scope.mapOptions.zoom = 8;

        $scope.mapData = {
            pinOptions: {
                height: 30,
                width: 30
            },
            actualRouteOptions: {
                strokeThickness: 7,
                zIndex: 0,
            },
            estimatedRouteOptions: {
                strokeThickness: 7,
                zIndex: 0,
            },
            pins: [],
            actualRoute: [],
            estimatedRoute: [],
        };

        $scope.legDataTable = {
            instance: {},
            options: DTOptionsBuilder.newOptions()
                .withBootstrap()
                .withOption('searching', false)
                .withOption('paging', false)
                .withOption('scrollY', 150)
                .withOption('info', false),
        };

        getData();

        setMapContainerSize();

    }
]);
