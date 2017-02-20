'use strict';

angular.module('peApp').controller('ResourceUnitSchedulerCtrl', ['$scope', '$filter', 'apiService', '$rootScope', '$modal',
    function ($scope, $filter, apiService, $rootScope, $modal) {


        /// variables ///
        $scope.scheduleConfig = {

            yMapProperty: 'vehicle_id',
            dx: 150,
            dy: 40,
            type: 'resourceUnits',
            yEventHeight: 38,
            selectedTimeMode: 'week',
            timeModes: [
                {
                    name: 'week',
                    xUnit: 'day',
                    xDateFormat: '%D %d',
                    xStep: 1,
                    xSize: 7,
                    xStart: 0,
                    xLength: 7
                }
            ]
        };



        $scope.resourceUnitContextMenuItems = [];

        $scope.vehicleDaysToFilter = [];

        var orderVehicles = $filter('orderBy');
        $scope.orderVehiclesBy = 'label';


        /// === functions === ///

        $scope.schedulerControl = {};


        $scope.dateRangeFormatter = function (dateA, dateB) {
            var dayBeforeDayB = moment(dateB).subtract(1, "seconds");
            return moment(dateA).format('ddd D MMMM') + ' - ' + dayBeforeDayB.format('ddd D MMMM');
        };


        /// scheduler event handlers ///

        $scope.onInternalDrop = function (schedulerEvent) {

            var resourceUnit = {
                resourceUnitID: schedulerEvent.resource_unit_id,
                driverResourceID: schedulerEvent.driver_id,
                vehicleResourceID: schedulerEvent.vehicle_id,
                activeFrom: schedulerEvent.start_date,
                activeTo: schedulerEvent.end_date
            };

            var params = { allowConflicts: false };

            // Update the existing resource unit
            apiService.resourceUnit.update(params, resourceUnit).$promise.then(function (response) {
                // update any legs that were already planned
                $scope.replanLegs(resourceUnit);
                $scope.getResourceUnits();
            }, function (reason) {
                //409: Conflict.
                if (reason.status === 409) {
                    var confirmationModal = $modal.open({
                        templateUrl: 'html-partials/confirmation-modal.html',
                        controller: 'ConfirmationModalCtrl',
                        resolve: {
                            modalBody: function () {
                                return 'Updating of this resource unit has conflicts, do you wish to proceed?';
                            },
                            confirmationFunction: function () {
                                return resendResourceUnitUpdate;
                            },
                            params: function () {
                                return reason.config.params;
                            },
                            data: function () {
                                return reason.config.data;
                            }
                        }
                    });

                    confirmationModal.result.then(
                        function () {
                        },
                        function () {
                            // if conflict modal was cancelled then reload resource units
                            $scope.getResourceUnits(true);
                        });

                }
            });

        }

        $scope.onExternalDrop = function (dropData, dropDateTime, schedulerSection, isRowHeaderDrop) {

            var resourceUnit = {
                driverResourceID: dropData.resourceID,
                vehicleResourceID: schedulerSection,
                activeFrom: moment(dropDateTime).toDate()
            }

            var params = { useDriverTypeHours: 'true', resourceForWeek: isRowHeaderDrop, allowConflicts: false };

            apiService.resourceUnit.create(params, resourceUnit).$promise.then(function (response) {
                resourceUnit.activeTo = moment(resourceUnit.activeFrom).endOf('day').toDate(); // this is temp as will create a duplicate 
                $scope.replanLegs(resourceUnit);
                $scope.getResourceUnits();
            }, function (reason) {
                //409: Conflict.
                if (reason.status === 409) {
                    var confirmationModal = $modal.open({
                        templateUrl: 'html-partials/confirmation-modal.html',
                        controller: 'ConfirmationModalCtrl',
                        resolve: {
                            modalBody: function () {
                                return 'Creation of this resource unit has conflicts, do you wish to proceed?';
                            },
                            confirmationFunction: function () {
                                return resendResourceUnitCreation;
                            },
                            params: function () {
                                return reason.config.params;
                            },
                            data: function () {
                                return reason.config.data;
                            }
                        }
                    });
                }
            });
        };

        var resendResourceUnitCreation = function (params, resourceUnit) {

            params.allowConflicts = true;

            apiService.resourceUnit.create(params, resourceUnit).$promise.then(function (response) {
                $scope.getResourceUnits();
            });
        };

        var resendResourceUnitUpdate = function (params, resourceUnit) {

            params.allowConflicts = true;

            apiService.resourceUnit.update(params, resourceUnit).$promise.then(function (response) {
                $scope.getResourceUnits();
            });
        };

        $scope.onConflictDetect = function (event1, event2) {
            return (!(event1.end_date < event2.start_date || event1.start_date > event2.end_date) &&
                     event1.driver_id === event2.driver_id &&
                     event1.resource_unit_id !== event2.resource_unit_id);
        }

        $scope.schedulerEventDoubleClick = function (schedulerEvent) {
            if (!schedulerEvent.isResourceSchedule) {
                var resourceUnitToEdit = _.find($scope.resourceUnits, function (ru) { return ru.resourceUnitID == schedulerEvent.resource_unit_id; });
                $scope.editResourceUnit('', resourceUnitToEdit);
            }
        }

        $scope.onColumnToggle = function (dayDate) {
            //The toggles works by taking the date of the column which has been toggled on the scheduler 
            //and adds or removes it from the 'vehicleDaysToFilter' list
            //depending on the state of the toggle. Then filters on the days in the 'vehicleDaysToFilter' list.
            if (_.some($scope.vehicleDaysToFilter, function (listDay) {
                return dayDate.getTime() == listDay.getTime();
            }))
                $scope.vehicleDaysToFilter = _.filter($scope.vehicleDaysToFilter, function (item) { return item.getTime() != dayDate.getTime() });
            else
                $scope.vehicleDaysToFilter.push(dayDate);

            $scope.applyFilters();

            _.defer(function () {
                $scope.$digest();
            });
        };


        /// filtering ///

        $scope.applyFilters = function () {

            $scope.schedulerVehicles = $filter('resourceUnitDriverVehicleText')($scope.originalSchedulerVehicles, $scope.resourceUnits, $scope.filters.driver, $scope.filters.vehicle, $scope.filters.vehicleTypes);

            if ($scope.vehicleDaysToFilter.length > 0)
                $scope.schedulerVehicles = $filter('resourceUnitDayToggle')($scope.schedulerVehicles, $scope.resourceUnits, $scope.vehicleDaysToFilter);

        }

        /// api calls ///

        // returns a promise of an array of "resource schedule" events for the scheduler
        var getResourceSchedules = function () {
            var params = { fromDateTime: $scope.scheduleState.fromDateTime, toDateTime: $scope.scheduleState.toDateTime };

            return apiService.resourceSchedule.getVehicleResourceSchedules(params).$promise.then(function (result) {
                return result.map(function (val) {
                    var startDateTime = moment(val.startDateTime);
                    var endDateTime = moment(val.endDateTime);
                    var resourceScheduleText = val.resourceActivityType;

                    if (val.comments) {
                        resourceScheduleText += ' - ' + val.comments;
                    }

                    var resourceSchedule = {
                        text: resourceScheduleText,
                        start_date: startDateTime,
                        start_date_display: startDateTime.calendar(),
                        end_date: endDateTime,
                        end_date_display: endDateTime.calendar(),
                        color: '#999',
                        vehicle_id: val.resourceID,
                        readonly: true,
                        isResourceUnit: false,
                        isResourceSchedule: true,
                    };

                    return resourceSchedule;
                });
            });
        }

        $scope.replanLegs = function (resourceUnit) {
            var startDateTime = resourceUnit.activeFrom;
            var endDateTime = resourceUnit.activeTo;
            var driverResourceID = resourceUnit.driverResourceID;
            var vehicleResourceID = resourceUnit.vehicleResourceID;
            var driver = null;
            apiService.driver.getDriver({ driverResourceID: resourceUnit.driverResourceID }).$promise.then(function (result) {
                driver = result;

                var plannerIdentityID = driver.plannerIdentityID;

                var params = { startDateTime: startDateTime, endDateTime: endDateTime, plannerIdentityID: plannerIdentityID };
                apiService.legPlan.get(params).$promise.then(function (result) {
                    var legs = result;
                    var filteredLegs = _.filter(legs, function (leg) { return leg.driverResourceID === driverResourceID });

                    if (filteredLegs.length > 0) {
                        var params = {
                            driverResourceID: driverResourceID,
                            vehicleResourceID: vehicleResourceID,
                        };

                        var data = {
                            resourceUnits: [resourceUnit].map(function (resourceUnit) {
                                var startDate = moment(resourceUnit.activeFrom).format('DD/MM/YYYY');
                                var startTime = moment(resourceUnit.activeFrom).format('HH:mm');

                                var finishDate = moment(resourceUnit.activeTo).format('DD/MM/YYYY');
                                var finishTime = moment(resourceUnit.activeTo).format('HH:mm');

                                return {
                                    startDateTime: moment(startDate + ' ' + startTime, 'DD/MM/YYYY HH:mm').toDate(),
                                    finishDateTime: moment(finishDate + ' ' + finishTime, 'DD/MM/YYYY HH:mm').toDate(),
                                };
                            }),
                            legs: filteredLegs.map(function (leg) {
                                return {
                                    jobID: leg.jobID,
                                    jobLastUpdateDateTime: leg.jobLastUpdateDateTime,
                                    startInstructionID: leg.startInstructionID,
                                    endInstructionID: leg.endInstructionID,
                                };
                            })
                        }

                        apiService.driver.assignVehicle(params, data).$promise.then(function (response) {

                        });

                    }
                });

            });
        };

        /// watches ///

        $scope.$watch('scheduleState.toDateTime', function (val) {
            if (val !== null) {
                $scope.getResourceUnits();
                $scope.vehicleDaysToFilter = [];
            }
        });

        $scope.$watchCollection('resourceUnits', function (resourceUnits) {
            if (!resourceUnits) {
                return;
            }

            var resourceUnitEvents = resourceUnits.map(function (val) {
                var startDateTime = moment(val.startDateTime);
                var endDateTime = moment(val.finishDateTime);

                var contextMenuItems = [];

                contextMenuItems.push({
                    caption: 'Edit Resource Unit',
                    action: function () {
                        $scope.editResourceUnit('', val);
                    }
                });

                contextMenuItems.push({
                    caption: 'Delete Resource Unit',
                    action: function () {
                        apiService.resourceUnit.delete({ resourceUnitID: val.resourceUnitID }).$promise.then(function () {
                            $scope.getResourceUnits();
                            $rootScope.$emit("pe-app-resource-units-updated", null);
                        });
                    }
                });

                return {
                    text: val.driver.fullName,
                    start_date: startDateTime,
                    start_date_display: startDateTime.calendar(),
                    end_date: endDateTime,
                    end_date_display: endDateTime.calendar(),
                    vehicle_id: val.vehicle.resourceID,
                    driver_id: val.driver.resourceID,
                    resource_unit_id: val.resourceUnitID,
                    original_vehicle_id: val.vehicle.resourceID,
                    yEventHeight: $scope.scheduleConfig.yEventHeight,
                    isResourceUnit: true,
                    isResourceSchedule: false,
                    onContextMenuShow: function () {
                        $scope.contextMenuError = '';
                        $scope.resourceUnitContextMenuItems = contextMenuItems;
                        $scope.scheduleState.displayEventTooltips = false;
                    },
                    onContextMenuClose: function () {
                        $scope.scheduleState.displayEventTooltips = true;
                    }
                };
            });

            getResourceSchedules().then(function (resourceScheduleEvents) {
                $scope.events = resourceUnitEvents.concat(resourceScheduleEvents);
            });

            $scope.applyFilters();
        });


        $scope.$watch('filters.driver', function (val) {
            $scope.applyFilters()
        });

        $scope.$watch('filters.vehicle', function (val) {
            $scope.applyFilters()
        });
        $scope.$watchCollection('filters.vehicleTypes', function (val) {
            $scope.applyFilters()
        });


        //// PAGE SETUP ////
        apiService.vehicle.getVehicles().$promise.then(function (response) {
            var transformed = response.map(function (vehicle) {
                return {
                    key: vehicle.resourceID,
                    label: vehicle.registrationNumber,
                    vehicleTypeID: vehicle.vehicleTypeID
                };
            });

            $scope.schedulerVehicles = orderVehicles(transformed, $scope.orderVehiclesBy);
            $scope.originalSchedulerVehicles = orderVehicles(transformed, $scope.orderVehiclesBy);

        });

    }]);
