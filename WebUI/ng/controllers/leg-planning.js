'use strict';

angular.module('peApp').controller('LegPlanningCtrl', ['$rootScope', '$scope', '$filter', '$interval', '$window', '$modal', '$q', 'apiService', 'legMenuItemsService', 'jobUpdateBusinessRulesService', 'authenticationService', '$timeout', 'legacyAppWindowService', '$http', 'ipCookie',
function ($rootScope, $scope, $filter, $interval, $window, $modal, $q, apiService, legMenuItemsService, jobUpdateBusinessRulesService, authenticationService, $timeout, legacyAppWindowService, $http, ipCookie) {


    //// VARIABLES ////

    $scope.schedulerEvents = [];
    $scope.filters = {};
    $scope.filterProperties = {};
    $scope.loading = false;
    $scope.planLegFailed = false;
    $scope.planLegErrors = [];
    $scope.contextMenuError = '';
    $scope.dropDownOpen = false;
    $scope.spinnerConfig = { lines: 12, radius: 6, length: 4, width: 2 };
    $scope.driverFinishTowns = [];
    $scope.alerts = [];
    $scope.datePickerOpened = false;

    $scope.onDropDownClick = function () {
        $scope.dropDownOpen = !$scope.dropDownOpen;
    };

    var originalSchedulerRows;
    var originalSchedulerEvents;

    $scope.filters.includeAllResourceUnits = 'onlyMyResourceUnits';
    $scope.filters.resourceUnitFreeTextFilter = '';
    $scope.includeAllResourceUnitsBtnText = 'My Resource Units Only';
    $scope.filters.viewingPlanner = authenticationService.getUserID();
    $scope.userID = authenticationService.getUserID();

    // Initialise scheduler to two hours before now - toDateTime will be set by the scheduler based on its timespan
    $scope.scheduleState = {
        fromDateTime: moment().add(-2, 'hours').toDate(),
        toDateTime: null,
        displayEventTooltips: true
    };

    $scope.datePickerOptions = {
        'starting-day': 1
    };

    $scope.scheduleConfig =
    {
        yMapProperty: 'y_data_key',
        dx: 283,
        dy: 109,
        yEventHeight: 38,
        selectedTimeMode: '24hour',
        timeModes: [
             {
                 name: '6hour',
                 xUnit: 'hour',
                 xDateFormat: '%H:%i',
                 xStep: 1,
                 xSize: 6,
                 xStart: moment().hour()
             },
              {
                  name: '12hour',
                  xUnit: 'hour',
                  xDateFormat: '%H:%i',
                  xStep: 1,
                  xSize: 12,
                  xStart: moment().hour()
              },
            {
                name: '24hour',
                xUnit: 'hour',
                xDateFormat: '%H:%i',
                xStep: 1,
                xSize: 24,
                xStart: 0
            },
            {
                name: '48hour',
                xUnit: 'hour',
                xDateFormat: '%H:%i',
                xStep: 2,
                xSize: 24,
                xStart: 0
            },
            {
                name: '72hour',
                xUnit: 'hour',
                xDateFormat: '%H:%i',
                xStep: 3,
                xSize: 24,
                xStart: 0
            },
            {
                name: 'week',
                xUnit: 'day',
                xDateFormat: '%D %d',
                xStep: 1,
                xSize: 7,
                xStart: 0,
            }
        ]
    };

    $scope.legContextMenuItems = [];
    $scope.travelNotesContextMenuItems = [];

    var legStateColors = {
        1: '#cccccc', // booked
        2: '#ccffcc', // planned
        3: '#99ff99', // in progress
        4: '#add8e6', // completed
    };

    //// FUNCTIONS ////

    var setFiltersIfSet = function () {
        var resourceViewFilters = ipCookie('resourceViewFilters');
        //console.log("GOT COOKIE : " + JSON.stringify(ipCookie('resourceViewFilters')));
        if (resourceViewFilters != null) {
            $scope.filters.includeAllResourceUnits = resourceViewFilters.resourcesFilter;
            $scope.filters.viewingPlanner = resourceViewFilters.planner;
            switch ($scope.filters.includeAllResourceUnits) {
                case 'onlyMyResourceUnits':
                    $scope.includeAllResourceUnitsBtnText = 'My Resource Units Only';
                    break;
                case 'includeAllResourceUnits':
                    $scope.includeAllResourceUnitsBtnText = 'All Resource Units';
                    break;
                case 'anotherPlannersResourceUnits':
                    $scope.includeAllResourceUnitsBtnText = 'Another Planner\'s Resource Units';
                    break;
            }
        }
    }();

    // Return a promise of an array of resource units (grouped where there are multiple resource units for the same driver/vehicle pair)
    var getResourceUnitGroups = function () {
        var params = { fromDateTime: $scope.scheduleState.fromDateTime, toDateTime: $scope.scheduleState.toDateTime, plannerIdentityID: $scope.filters.viewingPlanner };
        return apiService.resourceUnit.getResourceUnitGroups(params).$promise;
    };

    // Return a promise of an array of drivers that have no resource unit for the specified period (but have availability based on their driver type)
    var getUnconnectedDrivers = function () {
        var params = { fromDateTime: $scope.scheduleState.fromDateTime, toDateTime: $scope.scheduleState.toDateTime, plannerIdentityID: $scope.filters.viewingPlanner };
        return apiService.resourceUnit.getUnconnectedDrivers(params).$promise;
    };

    // Return a promise of a summary object containing today's start times for drivers, yesterday's finish times for drivers and outstanding driver requests
    var getTodaySummary = function () {
        return apiService.driver.getTodaySummary().$promise;
    };

    // Return a promise of a summary object containing start times for drivers, yesterday's finish times for drivers and outstanding driver requests
    var getDaySummary = function () {
        if (!$scope.scheduleState.fromDateTime)
            return;

        var params = { date: $scope.scheduleState.fromDateTime };
        return apiService.driver.getDaySummary(params).$promise;
    };

    // Return a promise of an array of planned legs for the current date range
    var getLegs = function () {
        var params = { startDateTime: $scope.scheduleState.fromDateTime, endDateTime: $scope.scheduleState.toDateTime, plannerIdentityID: $scope.filters.viewingPlanner };
        return apiService.legPlan.get(params).$promise;
    };

    // Return a promise of an array of resource schedules (e.g. vehicle maintenance, driver annual leave)
    var getResourceSchedules = function () {
        var params = { fromDateTime: $scope.scheduleState.fromDateTime, toDateTime: $scope.scheduleState.toDateTime, plannerIdentityID: $scope.filters.viewingPlanner };
        return apiService.resourceSchedule.getForPlanning(params).$promise;
    };

    //returns a promise of an array of Debriefs
    var getDriverDebriefs = function () {
        var params = { plannerIdentityId: $scope.filters.viewingPlanner, dateStamp: $scope.scheduleState.fromDateTime }
        return apiService.driverDebriefs.getDriverDebriefs(params).$promise;
    };

    var getData = function (delayLoadingIndicatorUntilDataRetrieved) {
        if (_initializing)
            return;

        //This makes sure the scheduler has been initialized before it gets the data
        if (!$scope.scheduleState.toDateTime || !$scope.scheduleState.fromDateTime)
            return;

        resetAutoRefreshingScheduler();

        var resourceUnitGroupsPromise = getResourceUnitGroups();
        var unconnectedDriversPromise = getUnconnectedDrivers();
        var todaySummaryPromise = getDaySummary();
        var legsPromise = getLegs();
        var resourceSchedulesPromise = getResourceSchedules();
        var driverDebriefsPromise = getDriverDebriefs();

        $scope.loading = !delayLoadingIndicatorUntilDataRetrieved;

        $q.all([
            resourceUnitGroupsPromise,
            unconnectedDriversPromise,
            todaySummaryPromise,
            legsPromise,
            resourceSchedulesPromise,
            driverDebriefsPromise
        ]).then(
            function (results) {
                $scope.loading = true;
                gotData(results[0], results[1], results[2], results[3], results[4], results[5]);
            },
            function (error) {
                $scope.loading = false;
            }
        );
    };

    var gotData = function (resourceUnitGroupsResult, unconnectedDriversResult, todaySummaryResult, legsResult, resourceSchedulesResult, driverDebriefs) {

        // get the finish town and traffic area for the selected schedulerdate
        $scope.driverFinishTowns = [];
        var finishDate = moment($scope.scheduleState.fromDateTime);
        var drivers = _.map(_.uniq(legsResult, false, function (n) { n.driverResourceID; }),
            function (x) { return x.driverResourceID; });

        // add the information to the driver for display on the schedule
        _.each(drivers, function (n) {
            var leg = _.last(_.sortBy(_.filter(_.filter(legsResult, { 'driverResourceID': n }), function (l) {
                return moment(l.endDateTime).isSame(finishDate, 'day');
            }), 'endDateTime'));

            if (leg) {
                $scope.driverFinishTowns.push({ 'driverResourceID': n, 'finishPostTown': leg.endPointPostTown, 'finishTrafficArea': leg.trafficArea })
            }
        });


        // generate the scheduler rows from the returned data and store the unfiltered scheduler rows
        originalSchedulerRows = populateSchedulerRows(resourceUnitGroupsResult, unconnectedDriversResult, todaySummaryResult.driverRequests, todaySummaryResult.StartTimes, todaySummaryResult.yesterdayFinishTimes, driverDebriefs);

        loadSchedulerEvents(legsResult, resourceSchedulesResult);

        $scope.loading = false;
    };

    var loadSchedulerEvents = function (legsResult, resourceSchedulesResult) {
        // generate the scheduler events from the leg and resource schedule data and store the unfiltered events
        originalSchedulerEvents = populateSchedulerEvents(legsResult, resourceSchedulesResult);

        // apply any resource unit and leg filters and set the resource units and scheduler events on the scope
        applyFilters();
    };

    var refreshLegs = function (forceReload) {
        // Refresh only the leg and resource schedule data - this is done following leg planning/resourcing changes or off the back of a context menu action that indicates data has been updated
        var legsPromise = getLegs();
        var resourceSchedulesPromise = getResourceSchedules();
        //  var travelNotesPromise = getDriverTravleNotes();

        $scope.loading = true;

        $q.all([
            legsPromise,
            resourceSchedulesPromise,
   //         travelNotesPromise
        ]).then(
            function (results) {
                loadSchedulerEvents(results[0], results[1], results[2]);

                if (forceReload) {
                    $scope.schedulerEventsForceReload = true;
                }

                $scope.loading = false;
            },
            function (error) {
                $scope.loading = false;
            }
        );
    };

    var populateSchedulerRows = function (resourceUnitGroups, unconnectedDrivers, driverRequests, todayStartTimes, yesterdayFinishTimes, driverDebriefs) {
        var unconnectedDriverRows = unconnectedDrivers.map(function (unconnectedDriver) {
            return {
                key: createSchedulerRowKey(unconnectedDriver.driver.resourceID, null),
                driver: unconnectedDriver.driver,
                vehicle: null,
                resourceUnits: unconnectedDriver.availability,

            };
        });

        var rows = resourceUnitGroups.concat(unconnectedDriverRows);

        if (rows.length == 0) {
            addAlert("There are no drivers for the current filter selection.", true);
        }


        // iterate through all rows
        rows.forEach(function (row) {

            // set up the modals
            row.driverStartTimeModal = function () {
                driverStartTimeModal('', row.driver.resourceID, row.driver.fullName);
            };
            row.driverFinishTimeModal = function () {
                driverFinishTimeModal('', row.driver.resourceID, row.driver.fullName);
            };
            row.viewDriverRequests = function () {
                viewDriverRequests(row.driver);
            };
            row.toggleDriverDebriefFlag = function () {
                toggleDriverDebriefFlag(row.driver);
            };
            if (row.vehicle !== null) {
                row.viewVehicleCurrentLocation = function () {
                    vehicleCurrentLocation('', row.vehicle.gpsUnitID);
                };
            }

            for (var i = 0; i < driverRequests.length; i++) {
                if (row.driver.resourceID === driverRequests[i].resourceID) {
                    row.driver.pendingDriverRequest = true;
                }
            }

            row.driver.driverRequestClass = 'glyphicon glyphicon-registration-mark';
            row.driver.driverDebriefClass = 'glyphicon glyphicon-ok-circle';

            if (row.driver.pendingDriverRequest) {
                row.driver.driverRequestClass += ' glyphicon-red';
            }

            for (var i = 0; i < todayStartTimes.length; i++) {
                if (row.driver.resourceID === todayStartTimes[i].resourceID) {
                    row.driver.startTime = moment(todayStartTimes[i].startTime, 'HH:mm:ss').format('HH:mm');
                    if (todayStartTimes[i].notes) {
                        row.driver.startTimeHasNotes = true;
                        row.driver.startTimeNotes = todayStartTimes[i].notes;
                    }
                }
            };

            if (!row.driver.startTime) {
                row.driver.startTime = '--:--';
            }


            for (var i = 0; i < yesterdayFinishTimes.length; i++) {
                if (row.driver.resourceID === yesterdayFinishTimes[i].resourceID) {
                    row.driver.finishTime = moment(yesterdayFinishTimes[i].finishTime, 'HH:mm:ss').format('HH:mm');
                    if (yesterdayFinishTimes[i].notes)
                        row.driver.finishTimeHasNotes = true;
                }
            };

            if (!row.driver.finishTime)
                row.driver.finishTime = '--:--';


            row.updateDriverTravelNotes = function () {
                apiService.updateTravelNotes.update(row.driver);
            };

            var finishDetails = _.find($scope.driverFinishTowns, function (d) { return d.driverResourceID == row.driver.resourceID; });

            if (finishDetails) {
                row.driver.finishPostTown = finishDetails.finishPostTown;
                row.driver.finishTrafficArea = finishDetails.finishTrafficArea;
            }

            row.driver.currentFinishDate = moment($scope.scheduleState.fromDateTime).format('DD/MM/YY');

            row.assignVehicle = function () {
                assignVehicle(row.driver.resourceID, row.driver.fullName);
            };

            row.driver.driverDebrief = _.find(driverDebriefs, function (debrief) { return debrief.DriverResourceID == row.driver.resourceID; });
            if (row.driver.driverDebrief)
                row.driver.driverDebriefClass += ' glyphicon-red';

            // iterate over resource units
            row.resourceUnits.forEach(function (resourceUnit, resourceUnitGroups, array) {
                resourceUnit.key = row.key;
                resourceUnit.driver = row.driver;
                resourceUnit.vehicle = row.vehicle;

            });
        });

        return rows;
    };

    var populateSchedulerEvents = function (legsResult, resourceSchedulesResult) {
        // Each leg may apply to multiple rows and therefore be represented as multiple scheduler events,
        // for example there may be multiple resource units for a driver, each with a different vehicle.
        var schedulerEvents = [];

        originalSchedulerRows.forEach(function (row) {
            var rowDriverResourceID = row.driver.resourceID;
            var rowVehicleResourceID = row.vehicle === null ? null : row.vehicle.resourceID;

            // generate the scheduler events from the legs
            var legSchedulerEvents = legsResult
                .filter(function (val) {
                    var isDriverMatch = val.driverResourceID === rowDriverResourceID;
                    //var isVehicleMatch = rowVehicleResourceID !== null && val.vehicleResourceID === rowVehicleResourceID;
                    return isDriverMatch; // || isVehicleMatch;
                })
                .map(function (val) {
                    var isForDriver = val.driverResourceID === rowDriverResourceID;
                    var isForVehicle = val.vehicleResourceID === rowVehicleResourceID;

                    var contextMenuItems = legMenuItemsService.getMenuItems(
                        val.endInstructionID,
                        val.legState,
                        val.jobID,
                        val.jobLastUpdateDateTime,
                        val.startDateTime,
                        val.endDateTime,
                        val.driverResourceID,
                        val.driverName,
                        val.vehicleResourceID,
                        val.vehicleRegistration,
                        val.trailerResourceID,
                        val.trailerRef,
                        val.subcontractorIdentityID,
                        val.isMwfAllowed,
                        function () {
                            refreshLegs();
                        },
                        function (error) {
                            $scope.contextMenuError = error;
                        });

                    var startDateTime = moment(val.startDateTime);
                    var endDateTime = moment(val.endDateTime);

                    var isPartial = !isForDriver || !isForVehicle;
                    var partialTooltipText = '';

                    if (val.legState === 2)
                        val.endMwfCommunicationStatus = 'Leg needs communicating.';

                    if (isPartial) {
                        if (isForVehicle)
                            partialTooltipText = 'This leg is only matched to the Vehicle';
                        else if (isForDriver)
                            partialTooltipText = 'This leg is only matched to the Driver';
                    }

                    var isEndBookingWindow = val.endBookedIsAnytime == false && (val.endBookedFrom != val.endBookedTo);

                    return {
                        text: val.jobID,
                        start_date: startDateTime,
                        end_date: endDateTime,
                        color: legStateColors[val.legState] || '',
                        textColor: '#000000',
                        y_data_key: row.key,
                        readonly: val.legState === 4, // completed jobs are read only
                        yEventHeight: $scope.scheduleConfig.yEventHeight,
                        isScheduleEvent: false,
                        leg: {
                            jobID: val.jobID,
                            legStateID: val.legState,
                            deliveryOrderNumber: val.DeliveryOrderNumber,
                            loadNumber: val.LoadNumber,
                            driverResourceID: val.driverResourceID,
                            driverName: val.driverName,
                            vehicleResourceID: val.vehicleResourceID,
                            vehicleRegistration: val.vehicleRegistration,
                            startDateTime: startDateTime,
                            startDateDisplay: startDateTime.format('HH:mm'),
                            endDateDisplay: endDateTime.format('HH:mm'),
                            startInstructionID: val.startInstructionID,
                            endInstructionID: val.endInstructionID,
                            startPointDescription: val.startPointDescription,
                            endPointDescription: val.endPointDescription,
                            startPointPostTown: val.startPointPostTown,
                            endPointPostTown: val.endPointPostTown,
                            trailerRef: val.trailerRef,
                            trafficArea: val.trafficArea,
                            controlArea: val.controlArea,
                            businessType: val.businessType,
                            startMwfCommunicationStatus: val.startMwfCommunicationStatus,
                            endMwfCommunicationStatus: val.endMwfCommunicationStatus,
                            jobLastUpdateDateTime: val.jobLastUpdateDateTime,
                            isPartial: isPartial,
                            isForVehicle: isForVehicle,
                            isForDriver: isForDriver,
                            isSingleLegRun: val.isSingleLegRun,
                            hasStartMwfInst: val.hasStartMwfInst,
                            partialTooltipText: partialTooltipText,
                            endBookedFrom: moment(val.endBookedFrom).format('HH:mm'),
                            endBookedTo: moment(val.endBookedTo).format('HH:mm'),
                            endBookedIsAnytime: val.endBookedIsAnytime,
                            isEndBookingWindow: isEndBookingWindow,
                            customer: val.Customer,

                        },
                        onContextMenuShow: function () {
                            $scope.contextMenuError = '';
                            $scope.legContextMenuItems = contextMenuItems;
                            $scope.scheduleState.displayEventTooltips = false;
                        },
                        onContextMenuClose: function () {
                            $scope.scheduleState.displayEventTooltips = true;
                        },
                        isPartial: function () {
                            if (isPartial)
                                return 'partial-indication';
                            else
                                return 'non-partial-indication';
                        },
                        mwfStatusIcon: function () {
                            if (val.isMwfAllowed && val.legState > 1) {
                                if (val.hasLegGotMwfCommError)
                                    return 'glyphicon glyphicon-phone glyphicon-red';

                                if (val.legState === 2)
                                    return 'glyphicon glyphicon-phone glyphicon-gray';


                                return 'glyphicon glyphicon-phone glyphicon-green';
                            }
                        },
                        openMWFHistory: function () {
                            $modal.open({
                                templateUrl: 'html-partials/mwf-history-modal.html',
                                controller: 'MWFHistoryModalCtrl',
                                resolve: {
                                    startInstructionID: function () {
                                        return val.startInstructionID;
                                    },
                                    endInstructionID: function () {
                                        return val.endInstructionID;
                                    }
                                }
                            });
                        }
                    };
                });


            Array.prototype.push.apply(schedulerEvents, legSchedulerEvents);

            // add scheduler events for any resource schedules
            var resourceScheduleSchedulerEvents = resourceSchedulesResult
                .filter(function (val) {
                    return val.resourceID === rowDriverResourceID || val.resourceID === rowVehicleResourceID;
                })
                .map(function (val) {
                    var startDateTime = moment(val.startDateTime);
                    var endDateTime = moment(val.endDateTime);
                    var resourceScheduleText = val.resourceActivityType;

                    if (val.resourceActivityTypeID == 30) {
                        resourceScheduleText = val.comments;
                    }
                    else {
                        resourceScheduleText += ' - ' + val.comments;
                    }
                    var travelNotesContextMenuItems = legMenuItemsService.getTravelNoteMenuItems(
                          val.resourceScheduleID,
                          val,
                          refreshLegs,
                          function () {
                              refreshLegs();
                          },
                          function (error) {
                              $scope.contextMenuError = error;
                          });

                    return {
                        text: resourceScheduleText,
                        start_date: startDateTime,
                        end_date: endDateTime,
                        color: val.colour ? val.colour : '#6f6e52',
                        isScheduleEvent: true,
                        readonly: false,
                        y_data_key: row.key,
                        resourceSchedule: {
                            startDateDisplay: startDateTime.calendar(),
                            endDateDisplay: endDateTime.calendar(),
                            isForVehicle: val.resourceID === rowVehicleResourceID,
                            id: val.resourceScheduleID,
                            resourceId: val.resourceID,
                            startDateTime: val.startDateTime,
                            endDateTime: val.endDateTime,
                            resourceActivityTypeId: val.resourceActivityTypeID,
                            resourceActivityType: val.resourceActivityType,
                            comments: val.comments,
                            color: val.colour,
                            fullName: row.driver.fullName
                        },
                        onContextMenuShow: function () {
                            $scope.contextMenuError = '';
                            $scope.travelNotesContextMenuItems = travelNotesContextMenuItems;
                            $scope.scheduleState.displayEventTooltips = false;
                        },
                        onContextMenuClose: function () {
                            $scope.scheduleState.displayEventTooltips = true;
                        },
                    };
                });

            Array.prototype.push.apply(schedulerEvents, resourceScheduleSchedulerEvents);

        });

        return schedulerEvents;
    };

    // Set scheduler resource unit unavailability for the date range covered by the scheduler
    var setResourceUnitUnavailability = function (schedulerRows) {
        var unavailability = [];

        angular.forEach(schedulerRows, function (schedulerRow) {
            var currentDay = moment($scope.scheduleState.fromDateTime).startOf('day');
            var toDay;

            var timelineMode = _.find($scope.scheduleConfig.timeModes, function (item) {
                return item.name === $scope.scheduleConfig.selectedTimeMode;
            });

            if (timelineMode.xUnit === 'day' && timelineMode.xSize === 7)
                toDay = moment($scope.scheduleState.toDateTime).subtract(1, 'day').startOf('day');
            else
                toDay = moment($scope.scheduleState.toDateTime).startOf('day');

            while (currentDay <= toDay) {
                // Generate a set of time ranges within the current day corresponding to the schedule row's resource units
                var timeRanges = getSchedulerRowTimeRangesForDay(currentDay.clone().toDate(), schedulerRow.resourceUnits);

                // Merge any overlapping or adjoining time ranges
                var combinedTimeRanges = combineTimeRanges(timeRanges);

                // Invert the available time ranges to get the unavailable time ranges as a flat array of minutes
                var unavailableTimes = generateUnavailabilityTimesFromAvailableTimeRanges(combinedTimeRanges);

                unavailability.push({
                    sectionKey: schedulerRow.key,
                    day: currentDay.weekday(),
                    times: unavailableTimes
                });

                currentDay.add(1, 'days');
            }
        });

        $scope.schedulerUnavailability = unavailability;
    };

    var getSchedulerRowTimeRangesForDay = function (dayStart, schedulerRowResourceUnits) {
        var nextDayStart = moment(dayStart).clone().add(1, 'days').toDate();

        var timeRanges =
            schedulerRowResourceUnits
            .map(function (resourceUnit) {
                var from = moment(resourceUnit.startDateTime).toDate();
                var to = moment(resourceUnit.finishDateTime).toDate();

                return {
                    from: from < dayStart ? dayStart : from,
                    to: to > nextDayStart ? nextDayStart : to,
                };
            })
            .filter(function (timeRange) {
                var validRange = timeRange.to > timeRange.from;
                var intersectsCurrentDay = timeRange.from < nextDayStart && timeRange.to > dayStart;
                return validRange && intersectsCurrentDay;
            })
            .sort(function (a, b) {
                return a.from - b.from;
            });

        return timeRanges;
    };

    var combineTimeRanges = function (timeRanges) {
        // timeRanges is expected to be an array of objects with "from" and "to" javascript Date properties.
        // The time ranges must be pre-sorted in order of from date.

        var combined = [];

        timeRanges.forEach(function (timeRange) {
            var matchIndex = null;

            // Find first adjoining or overlapping time range in combined
            for (var i = 0; i < combined.length; i++) {
                var existing = combined[i];

                if (existing.from <= timeRange.to && existing.to >= timeRange.from) {
                    matchIndex = i;
                    break;
                }
            }

            if (matchIndex === null) {
                combined.push({ from: timeRange.from, to: timeRange.to });
            }
            else {
                var match = combined[matchIndex];

                if (timeRange.from < match.from) {
                    match.from = timeRange.from;
                }

                if (timeRange.to > match.to) {
                    match.to = timeRange.to;
                }
            }
        });

        if (combined.length < timeRanges.length)
            return combineTimeRanges(combined);

        return combined;
    };

    var generateUnavailabilityTimesFromAvailableTimeRanges = function (timeRanges) {
        // timeRanges is expected to be an array of objects with "from" and "to" javascript Date properties.
        // The time ranges must all fall within a single day (midnight to midnight) and must be pre-sorted in order of from date and non-overlapping/adjoining.

        var endOfDayMinutes = 24 * 60;
        var times = [0];

        for (var i = 0; i < timeRanges.length; i++) {
            var from = minutes(timeRanges[i].from);
            var to = minutes(timeRanges[i].to);

            if (to === 0) {
                to = endOfDayMinutes;
            }

            if (i === 0 && from === 0) {
                times.shift();
            }
            else {
                times.push(from);
            }

            if (i < timeRanges.length - 1 || to < endOfDayMinutes) {
                times.push(to);
            }
        }

        if (times.length % 2 === 1) {
            times.push(endOfDayMinutes);
        }

        return times;
    };

    var loadPlannerList = function () {
        var params = { systemPortionID: window.enums.systemPortion.plan };

        apiService.users.getForSystemPortion(params).$promise.then(function (result) {
            var currentUserIndex = -1;

            //Remove current user from list if present
            for (var i = 0; i < result.length; i++) {
                if ($scope.userID === result[i].identityID) {
                    currentUserIndex = i;
                }
            }

            if (currentUserIndex !== -1) {
                result.splice(currentUserIndex, 1);
            }

            $scope.plannerUsers = result;
        });
    };

    var getPlanners = function () {
        var params = { systemPortionID: window.enums.systemPortion.plan };
        return apiService.users.getForSystemPortion(params).$promise;
    };

    $scope.schedulerContextMenu = function (e, driverTravelNote) {
        var _menu = $('#schedulerMenu')
        .show()
        .css(
        {
            position: "absolute",
            left: getMenuPosition(e.clientX, 'width', 'scrollLeft'),
            top: getMenuPosition(e.clientY, 'height', 'scrollTop')

        })
        .off('click')
        .on('click', 'a', function (e) {
            _menu.hide();
            return false;
        });
        return false;
    };
    //setTravelNote(driverTravelNote);

    function getMenuPosition(mouse, direction, scrollDir) {
        var win = $(window)[direction](),
            scroll = $(window)[scrollDir](),
            menu = $('#schedulerMenu')[direction](),
            position = (mouse + scroll);

        // opening menu would pass the side of the page
        if (mouse + menu > win && menu < mouse)
            position -= menu;

        return position;
    };

    $scope.schedulerDoubleClick = function (resourceSchedule) {
        setTravelNote(resourceSchedule);
    };

    $scope.schedulerInternalDrop = function (schedulerEvent) {
        // Save changes
        var key = parseSchedulerRowKey(schedulerEvent.y_data_key);

        // determine which type of event this is (leg or driver travle note)
        if (schedulerEvent.leg) {
            var isSameDriver = schedulerEvent.leg.driverResourceID === key.driverResourceID;
            var isSameVehicle = schedulerEvent.leg.vehicleResourceID === key.vehicleResourceID;
            var setPlannedTimesOnly = isSameDriver && isSameVehicle;
            var isTimeUnchanged = moment(schedulerEvent.start_date).isSame(schedulerEvent.leg.startDateTime);
            var isSingleLegRun = schedulerEvent.leg.isSingleLegRun;

            var displayJobLegsModal = !setPlannedTimesOnly && !isSingleLegRun && !isTimeUnchanged;

            if (displayJobLegsModal) {
                var selectedRow = getSchedulerRowForKey(schedulerEvent.y_data_key);
                var modalPromise = jobLegs(schedulerEvent.leg.jobID, schedulerEvent.leg.endInstructionID, key.driverResourceID, selectedRow.driver.fullName, key.vehicleResourceID, selectedRow.vehicle.registrationNumber, setPlannedTimesOnly, schedulerEvent.leg.jobLastUpdateDateTime);

                modalPromise.then(
                    function () {
                        // Refresh the data on leg plan completion
                        refreshLegs();
                    },
                    function () {
                        // Refresh the data on user cancellation to move the dragged leg back to its original position.
                        // Need to set the forceReload parameter to true in order to force the data to be reloaded, since the pr-scheduler will otherwise think that nothing has changed.
                        refreshLegs(true);
                    });

                return;
            }

            var legPlan = {
                startInstructionID: schedulerEvent.leg.startInstructionID,
                endInstructionID: schedulerEvent.leg.endInstructionID,
                startDateTime: moment(schedulerEvent.start_date),
                endDateTime: moment(schedulerEvent.end_date),
            };

            var params = {
                driverResourceID: key.driverResourceID,
                vehicleResourceID: key.vehicleResourceID,
                setPlannedTimesOnly: setPlannedTimesOnly,
                applyResourceToRelatedLegs: true,
                jobLastUpdateDateTime: schedulerEvent.leg.jobLastUpdateDateTime,
            };

            $scope.planLegFailed = false;
            $scope.loading = true;

            apiService.legPlan.planAndResource(params, [legPlan]).$promise.then(
                function () {
                    $scope.loading = false;
                    getData();
                },
                function (error) {
                    $scope.loading = false;
                    $scope.planLegFailed = true;
                    $scope.planLegErrors = [];

                    // Refresh the data on user cancellation to move the dragged leg back to its original position.
                    // Need to set the forceReload parameter to true in order to force the data to be reloaded, since the pr-scheduler will otherwise think that nothing has changed.
                    refreshLegs(true);

                    if (error.data !== undefined && error.data.ModelState !== undefined) {
                        $scope.planLegErrors = jobUpdateBusinessRulesService.getInfringements(error.data.ModelState, true);
                    }
                });
        }
        if (schedulerEvent.resourceSchedule) {

            var isSameDriver = schedulerEvent.resourceSchedule.resourceId === key.driverResourceID;
            var isTimeUnchanged = (moment(schedulerEvent.start_date).isSame(schedulerEvent.resourceSchedule.startDateTime) && moment(schedulerEvent.end_date).isSame(schedulerEvent.resourceSchedule.endDateTime));
            if (!isSameDriver || !isTimeUnchanged) {
                $scope.planLegFailed = false;
                $scope.loading = true;
                var resourceSchedule = schedulerEvent.resourceSchedule;
                resourceSchedule.resourceId = key.driverResourceID;
                resourceSchedule.startDateTime = $filter('date')(schedulerEvent.start_date, 'yyyy-MM-dd HH:mm:ss');
                resourceSchedule.endDateTime = $filter('date')(schedulerEvent.end_date, 'yyyy-MM-dd HH:mm:ss');
                resourceSchedule.resourceScheduleID = resourceSchedule.id;
                resourceSchedule.colour = resourceSchedule.color;
                apiService.resourceSchedule.update(resourceSchedule).$promise.then(
                    function () {
                        $scope.loading = false;
                        getData();
                    },
                    function (error) {
                        $scope.loading = false;
                        refreshLegs(true);
                        if (error.data !== undefined && error.data.ModelState !== undefined) {
                            $scope.planLegErrors = jobUpdateBusinessRulesService.getInfringements(error.data.ModelState, true);
                        }
                    });
            }
        }
    };

    $scope.schedulerExternalDrop = function (dropData, dropDateTime, schedulerSection, isRowHeaderDrop) {
        var key = parseSchedulerRowKey(schedulerSection);
        var selectedRow = getSchedulerRowForKey(schedulerSection);
        var vehicleRegistration = selectedRow.vehicle === null ? 'no vehicle' : selectedRow.vehicle.registrationNumber;
        var modalPromise = jobLegs(dropData.jobID, dropData.endInstructionID, key.driverResourceID, selectedRow.driver.fullName, key.vehicleResourceID, vehicleRegistration, false, dropData.jobLastUpdateDateTime);

        modalPromise.then(
            function (startDateTime, endDateTime) {
                if (startDateTime > $scope.scheduleState.toDateTime || endDateTime < $scope.scheduleState.fromDateTime) {
                    $scope.scheduleState.fromDateTime = startDateTime;
                }
                else {
                    getData(true);
                }
            },
            function () {
            });
    };

    $scope.dateRangeFormatter = function (dateA, dateB) {
        if ($scope.scheduleConfig.selectedTimeMode === 'week') {
            return moment(dateA).format('ddd D MMMM') + ' - ' + moment(dateB).format('ddd D MMMM');
        }
        else
            return moment(dateA).calendar() + ' to ' + moment(dateB).calendar();
    };


    $scope.schedulerEventDoubleClick = function (schedulerEvent) {
        if (schedulerEvent.leg === undefined || schedulerEvent.isScheduleEvent) {
            return;
        }

        legacyAppWindowService.viewRun(schedulerEvent.text);
    };

    // Auto-refresh
    var refreshingScheduler;

    var startAutoRefreshingScheduler = function () {
        refreshingScheduler = $interval(function () {
            getData(true);
        }, 60000);
    };

    var resetAutoRefreshingScheduler = function () {
        stopAutoRefreshingScheduler();
        startAutoRefreshingScheduler();
    };

    var stopAutoRefreshingScheduler = function () {
        if (angular.isDefined(refreshingScheduler)) {
            $interval.cancel(refreshingScheduler);
            refreshingScheduler = undefined;
        }
    };

    // Date picker popup
    $scope.openDatePicker = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.datePickerOpened = !$scope.datePickerOpened;
    };

    // Driver start time
    var driverStartTimeModal = function (size, driverResourceID, driverFullName) {
        var startTimeModal = $modal.open({
            templateUrl: 'html-partials/driver-time-modal.html',
            controller: 'DriverStartTimeModalCtrl',
            size: size,
            resolve: {
                driverResourceID: function () { return driverResourceID; },
                driverFullName: function () { return driverFullName; },
                initialDate: function () { return moment($scope.scheduleState.fromDateTime); }
            }
        });

        startTimeModal.result.then(function (result) {
            getData(true);
        });
    };

    // Driver finish time
    var driverFinishTimeModal = function (size, driverResourceID, driverFullName) {
        var finishTimeModal = $modal.open({
            templateUrl: 'html-partials/driver-time-modal.html',
            controller: 'DriverFinishTimeModalCtrl',
            size: size,
            resolve: {
                driverResourceID: function () { return driverResourceID; },
                driverFullName: function () { return driverFullName; },
                initialDate: function () { return moment().subtract(1, 'days'); }
            }
        });

        finishTimeModal.result.then(function (result) {
            getData(true);
        });
    };

    // Vehicle Current Location
    var vehicleCurrentLocation = function (size, gpsUnitID) {
        $modal.open({
            templateUrl: 'html-partials/vehicle-current-location-modal.html',
            controller: 'VehicleCurrentLocationModalCtrl',
            size: size,
            resolve: {
                gpsUnitID: function () {
                    return gpsUnitID;
                }
            }
        });
    };

    var assignVehicle = function (driverResourceID, driverFullName) {
        var driverRowKey = createSchedulerRowKey(driverResourceID, null);

        var assignVehicleModal = $modal.open({
            templateUrl: 'html-partials/assign-vehicle-to-driver-modal.html',
            controller: 'AssignVehicleToDriverModalCtrl',
            size: 'lg',

            resolve: {
                driverResourceID: function () { return driverResourceID; },
                driverFullName: function () { return driverFullName; },

                resourceUnits: function () {
                    var schedulerRow = originalSchedulerRows.filter(function (row) {
                        return row.key.split(',')[0] === driverRowKey;
                    })[0];

                    return schedulerRow.resourceUnits;
                },

                legs: function () {
                    var legs = originalSchedulerEvents
                        .filter(function (schedulerEvent) {
                            return schedulerEvent.y_data_key.split(',')[0] === driverRowKey && (schedulerEvent.leg !== undefined && schedulerEvent.leg.legStateID < 3);
                        })
                        .map(function (schedulerEvent) {
                            return schedulerEvent.leg;
                        });

                    return legs;
                },
            }
        });

        assignVehicleModal.result.then(function (result) {
            getData(true);
        });
    };

    var editTravelNote = function (id) {
        console.log(this);
    }

    var setTravelNote = function (resourceSchedule) {
        var driverRowKey = createSchedulerRowKey(resourceSchedule.resourceId, null);

        var addTravelNoteModal = $modal.open({
            templateUrl: 'html-partials/add-edit-resourceschedule-modal.html',
            controller: 'AddEditResourceScheduleModalCtrl',
            size: 'lg',

            resolve: {
                resourceSchedule: function () { return resourceSchedule; }
            }
        });

        addTravelNoteModal.result.then(function (result) {
            getData(true);
        });
    };

    var onResourceUnitUpdated = function () {
        getData();
    };

    // Job legs planned times Modal - returns a promise that is resolved if the user completes the modal and rejected if they cancel
    var jobLegs = function (jobID, legEndInstructionID, driverResourceID, driverFullName, vehicleResourceID, vehicleRegistration, setPlannedTimesOnly, lastUpdateDateTime) {
        var jobLegsModal = $modal.open({
            templateUrl: 'html-partials/job-legs-modal.html',
            controller: 'JobLegsModalCtrl',
            size: 'xlg',
            resolve: {
                jobID: function () { return jobID; },
                legEndInstructionID: function () { return legEndInstructionID; },
                driverResourceID: function () { return driverResourceID; },
                driverFullName: function () { return driverFullName; },
                vehicleResourceID: function () { return vehicleResourceID; },
                vehicleRegistration: function () { return vehicleRegistration; },
                setPlannedTimesOnly: function () { return setPlannedTimesOnly; },
                lastUpdateDateTime: function () { return lastUpdateDateTime; }
            }
        });

        return jobLegsModal.result;
    };

    // Driver request modal
    var viewDriverRequests = function (driver) {

        var viewDriverRequestsModal = $modal.open({
            templateUrl: 'html-partials/view-driver-requests-modal.html',
            controller: 'viewDriverRequestsModalCtrl',
            size: 'lg',
            resolve: {
                driver: function () {
                    return driver;
                },
            }
        });

        viewDriverRequestsModal.result.then(function (hasRequests) {
            getData(true);
        });
    };

    var toggleDriverDebriefFlag = function (driver) {
        if (driver.driverDebrief && driver.driverDebrief.ID > 0)
            apiService.driverDebriefs.remove({ id: driver.driverDebrief.ID }).$promise.then(function () {
                getData(true);
            });
        else {
            var driverDebrief = {
                driverResourceId: driver.resourceID,
                plannerIdentityId: $scope.filters.viewingPlanner,
                dateStamp: $scope.scheduleState.fromDateTime
            }
            apiService.driverDebriefs.create(driverDebrief).$promise.then(function () {
                getData(true);
            });
        }
    }

    var minutes = function (dateTime) {
        return (dateTime.getHours() * 60) + (dateTime.getMinutes());
    };

    $scope.toggleIncludeAllResourceUnits = function () {

        $scope.enableViewingPlannersPlan = false;        

        switch ($scope.filters.includeAllResourceUnits) {
            case 'onlyMyResourceUnits':
                $scope.includeAllResourceUnitsBtnText = 'My Resource Units Only';
                $scope.filters.viewingPlanner = authenticationService.getUserID();
                getData(false);
                break;
            case 'includeAllResourceUnits':
                $scope.includeAllResourceUnitsBtnText = 'All Resource Units';
                $scope.filters.viewingPlanner = null;
                getData(false);
                break;
            case 'anotherPlannersResourceUnits':
                $scope.includeAllResourceUnitsBtnText = 'Another Planner\'s Resource Units';
                $scope.enableViewingPlannersPlan = true;
                break;
        }        
    };

    $scope.resetResourceUnitFilters = function () {
        $scope.filterProperties.resourceUnits.reset();
        sessionStorage.removeItem('resourceUnitOrder');
    };

    $scope.resetResourceUnitFreeTextFilter = function () {
        $scope.filters.resourceUnitFreeTextFilter = ''
    }

    var applyFilters = function () {
        var filteredEvents = originalSchedulerEvents;
        var orderedRows = null;
        var filterOnLegs = $scope.filterProperties.legs && !$scope.filterProperties.legs.isClear();

        if (filterOnLegs) {
            filteredEvents = $filter('legPlanningLeg')(originalSchedulerEvents, $scope.filters.legs);
        }

        var filteredRows = originalSchedulerRows;
        var filterOnResourceUnits = $scope.filterProperties.resourceUnits && !$scope.filterProperties.resourceUnits.isClear();

        if (filterOnResourceUnits || filterOnLegs) {
            filteredRows = $filter('legPlanningRow')(originalSchedulerRows, $scope.filters.resourceUnits, filterOnLegs ? filteredEvents : null);
        }


        if ($scope.filters.resourceUnitFreeTextFilter.length > 0) {
            filteredRows = _.filter(filteredRows, function (item) {
                return (item.driver.fullName.toLowerCase().indexOf($scope.filters.resourceUnitFreeTextFilter) > -1) ||
                        (item.driver.todayFinishTown && item.driver.todayFinishTown.toLowerCase().indexOf($scope.filters.resourceUnitFreeTextFilter) > -1) ||
                        (item.vehicle && item.vehicle.registrationNumber.toLowerCase().indexOf($scope.filters.resourceUnitFreeTextFilter) > -1);
            });

        }

        if ($scope.filters.resourceUnits) {
            orderedRows = $filter('orderBy')(filteredRows, $scope.filters.resourceUnits.orderResourceUnitsBy);
            sessionStorage.resourceUnitOrder = JSON.stringify($scope.filters.resourceUnits.orderResourceUnitsBy);
        }



        $scope.schedulerRows = orderedRows || filteredRows;
        $scope.schedulerEvents = filteredEvents;

        // Set the unavailable (shaded areas) on the scheduler based on the resource units
        setResourceUnitUnavailability($scope.schedulerRows);
    };

    var createSchedulerRowKey = function (driverResourceID, vehicleResourceID) {
        return vehicleResourceID === null ? driverResourceID.toString() : (driverResourceID + ',' + vehicleResourceID);
    };

    var parseSchedulerRowKey = function (key) {
        var resourceIDs = key.split(',');

        return {
            driverResourceID: parseInt(resourceIDs[0], 10),
            vehicleResourceID: resourceIDs.length > 1 ? parseInt(resourceIDs[1], 10) : null,
        };
    };

    var getSchedulerRowForKey = function (key) {
        var rows = $scope.schedulerRows.filter(function (sr) { return sr.key === key; });
        return rows.length > 0 ? rows[0] : null;
    };

    $scope.planLegFailedHide = function () {
        $scope.planLegFailed = false;
        $scope.planLegErrors = [];
    };

    $scope.contextMenuErrorHide = function () {
        $scope.contextMenuError = '';
    };

    $scope.openResourceUnit = function () {
        window.open('/ng/resourceunit');
    };

    $scope.isEndBookingWindow = function (leg) {
        return leg.endBookedFrom == leg.endBookedTo;
    };

    var addAlert = function (message, autoRemove) {
        $scope.alerts.push(message);

        if (autoRemove) {
            $timeout(function () {
                $scope.closeAlert($scope.alerts.indexOf(message));
            }, 4000);
        }
    }

    $scope.closeAlert = function (alertIndex) {
        $scope.alerts.splice(alertIndex, 1);
    };


    //// PAGE SETUP ////

    var _initializing = true;

    $scope.$watch('scheduleState.toDateTime', function (val) {
        if (val !== null) {
            getData();
        }
    });

    $scope.$watch('filters.resourceUnits', function (val) {
        _.defer(function () {
            applyFilters();
            $scope.$digest();
        });
    }, true);

    $scope.$watch('filters.legs', function (val) {
        _.defer(function () {
            applyFilters();
            $scope.$digest();
        });
    }, true);

    $scope.$watch('filters.resourceUnitFreeTextFilter', function (val) {
        _.defer(function () {
            applyFilters();
            $scope.$digest();
        });
    }, true);

    $scope.$watch('filters.viewingPlanner', function (val) {
        var resourceViewFilters =
            {
                resourcesFilter: $scope.filters.includeAllResourceUnits,
                planner: $scope.filters.viewingPlanner
            };
        ipCookie('resourceViewFilters', resourceViewFilters);
        //console.log("STORED COOKIE : " + JSON.stringify(ipCookie('resourceViewFilters')));
        getData();
    }, true);

    loadPlannerList();

    startAutoRefreshingScheduler();

    $scope.refreshScheduler = function () {
        getData();
    }

    //// CLEANUP ////
    $scope.$on('$destroy', function () {
        // Make sure that the interval is destroyed too
        stopAutoRefreshingScheduler();
    });

    var waitForRenderAndDoSomething = function () {
        if ($http.pendingRequests.length > 0) {
            $timeout(waitForRenderAndDoSomething); // Wait for all templates to be loaded
        } else {
            //the code which needs to run after dom rendering
            _initializing = false;
            getData();
        }
    }

    $timeout(waitForRenderAndDoSomething);
}]);
