'use strict';

angular.module('peApp').controller('ResourceUnitsCtrl', ['$scope', '$rootScope', '$modal', '$q', 'apiService', 'authenticationService', 'ipCookie',
    function ($scope, $rootScope, $modal, $q, apiService, authenticationService, ipCookie) {


        //// VARIABLES ////

        $scope.enableViewingPlannersPlan = false;

        $scope.filters = {
            driver: '',
            vehicle: '',
            includeDrivers: 'onlyMyDrivers',
            includeAllResourceUnits: false,
            vehicleTypes: {},
            viewingPlanner: authenticationService.getUserID()
        }
        $scope.datePickerOptions = {
            'starting-day': 1
        };
        $scope.datePickerOpened = false;

        $scope.filterProperties = {};
        $scope.isVehicleTypeChecked = true;

        var startOfWeek = function (date) {
            var startOfWeekMonday = moment(date).startOf('isoWeek').clone();

            if (moment(date).subtract(6, 'days').isAfter(startOfWeekMonday)) {
                return startOfWeekMonday.add(7, 'days').toDate();
            }

            return startOfWeekMonday.toDate();
        };

        // Initialise scheduler to the start of the current week - toDateTime will be set by the scheduler based on its timespan
        $scope.scheduleState = {
            fromDateTime: startOfWeek(),
            toDateTime: null,
        };

        // For a date picker: called with a Date parameter selects the week in which that date falls, with no parameter returns the current week start date.
        $scope.scheduleState.week = function (newValue) {
            if (angular.isDefined(newValue)) {
                $scope.scheduleState.fromDateTime = startOfWeek(newValue);
            }

            return $scope.scheduleState.fromDateTime;
        };


        $scope.listViewHeading = 'List View';
        $scope.schedulerViewHeading = getSchedulerHeading();

        $scope.driverFilter = 'driver.lastName';

        $scope.includeAllResourceUnitsBtnText = 'Viewing my resource units';

        //// FUNCTIONS ////

        var loadPlannerList = function () {
            var params = { systemPortionID: window.enums.systemPortion.plan };

            apiService.users.getForSystemPortion(params).$promise.then(function (result) {
                $scope.plannerUsers = result;
            });
        }();

        var setFiltersIfSet = function () {
            var resourceViewFilters = ipCookie('resourceViewFilters');
            //console.log("GOT COOKIE : " + JSON.stringify(ipCookie('resourceViewFilters')));
            if (resourceViewFilters != null) {
                if (resourceViewFilters.resourcesFilter === 'onlyMyResourceUnits')
                    $scope.filters.includeDrivers = 'onlyMyDrivers';
                else if (resourceViewFilters.resourcesFilter === 'includeAllResourceUnits')
                    $scope.filters.includeDrivers = 'includeAllDrivers';
                else if (resourceViewFilters.resourcesFilter === 'anotherPlannersResourceUnits')
                    $scope.filters.includeDrivers = 'anotherPlannersDrivers';
                $scope.filters.viewingPlanner = resourceViewFilters.planner;
            }
        }();

        // Close the dropdown menu if the user clicks outside of it
        window.onclick = function (event) {
            if (!event.target.matches('.btn-dropdown')) {

                var dropdown = document.getElementById('moveBackwardsDropdown');
                if (dropdown) dropdown.classList.remove('show');
                dropdown = document.getElementById('moveForwardsDropdown');
                if (dropdown) dropdown.classList.remove('show');
            }
            else {
                var dropdown;
                var btn = event.target || event.srcElement;
                console.log(btn.id);
                if (btn.id === "btnMoveBackwards") {
                    dropdown = document.getElementById('moveForwardsDropdown');
                }
                else {
                    dropdown = document.getElementById('moveBackwardsDropdown');
                }
                if (dropdown) dropdown.classList.remove('show');
            }
        }

        //Used to initialise the filter and also clear it down.
        var initialiseFilterList = function (items, filters, filterOn, valueTo) {
            _.each(items, function (item) {
                filters[item[filterOn]] = valueTo;
            });
        };
        $scope.unTickAll = function (items) {

            switch (items) {
                case 'vehicleType':
                    initialiseFilterList($scope.vehicleTypes, $scope.filters.vehicleTypes, 'vehicleTypeID', $scope.isVehicleTypeChecked);
                    break;
            }
        };

        function getSchedulerHeading() {
            var heading = '(My Resource Units Only)';
            if ($scope.filters.includeDrivers == 'onlyMyDrivers')
                heading = '(My Resource Units Only)';
            else if ($scope.filters.includeDrivers == 'includeAllDrivers')
                heading = '(All Resource Units)';
            else if ($scope.filters.includeDrivers == 'anotherPlannersDrivers')
                heading = '(Another Planner\'s Resource Units)';
            return 'Scheduler View ' + heading;
        }

        $scope.getResourceUnits = function (forceReload) {
            $scope.listViewHeading = 'List View Loading...';
            $scope.loading = true;

            if (forceReload) {
                $scope.resourceUnits = [];
            }

            // get an half day before and after the current range displayed in the scheduler
            // so we can detect resource conflicts which are off the edge of the scheduler
            var params = {
                fromDateTime: moment($scope.scheduleState.fromDateTime).subtract(12, 'hours').toDate(),
                toDateTime: moment($scope.scheduleState.toDateTime).add(12, 'hours').toDate(),
                includeAllResourceUnits: $scope.filters.includeAllResourceUnits,
                anotherPlannerID: $scope.filters.viewingPlanner
            };
            apiService.resourceUnit.getResourceUnits(params)
                .$promise.then(function (result) {

                    $scope.resourceUnits = result;
                    $scope.listViewHeading = 'List View';
                    $scope.loading = false;
                    $rootScope.$emit("pe-app-resource-units-updated", null);
                });
        };


        $scope.editResourceUnit = function (size, resourceUnit) {

            $modal.open({
                templateUrl: 'html-partials/add-edit-resource-unit-modal.html',
                controller: 'AddEditResourceUnitModalCtrl',
                size: size,
                resolve: {
                    resourceUnit: function () {
                        return resourceUnit;
                    },
                    onResourceUnitEdited: function () {
                        return onResourceUnitEdited;
                    }
                }
            });

        }

        var onResourceUnitEdited = function (resourceUnit) {
            $scope.getResourceUnits();
        };

        $scope.deleteResourceUnit = function (resourceUnitID) {

            apiService.resourceUnit.delete({ resourceUnitID: resourceUnitID }).$promise.then(function () {
                $scope.getResourceUnits();
                $rootScope.$emit("pe-app-resource-units-updated", null);
            });
        };

        $scope.copyResourceUnits = function (allResourceUnits) {
            // start of last week plus 7 days
            var fromDateTime = moment().subtract(7, 'days').startOf('isoweek');
            var toDateTime = moment(fromDateTime).add(7, 'days');

            var params =
                    {
                        fromDateTime: fromDateTime.format('YYYY-MM-DDTHH:mm'),
                        toDateTime: toDateTime.format('YYYY-MM-DDTHH:mm'),
                        excludeDuplicates: true,
                        excludeResourcesStartingBeforeFromDate: true,
                        includeAllResourceUnits: allResourceUnits
                    };

            apiService.copyLastWeek.copyLastWeek(params).$promise.then(function (response) {
                $scope.getResourceUnits();
            });
        };

        $scope.assignUsualVehiclesForWeek = function (allDrivers) {

            apiService.assignUsualVehicles.assignUsualVehicles({ startOfWeek: $scope.scheduleState.fromDateTime, allDrivers: allDrivers }).$promise.then(function (noPlannerDriverList) {

                if (noPlannerDriverList && noPlannerDriverList.length > 0) {

                    var driverNoPlannerModal = $modal.open({
                        templateUrl: 'html-partials/driver-noplanner-modal.html',
                        controller: 'DriverNoPlannerModalCtrl',
                        resolve: {
                            driverList: function () {
                                return noPlannerDriverList;
                            }
                        }
                    });
                }
                else {
                    $scope.getResourceUnits();
                }

            });
        };

        $scope.toggleIncludeAllResourceUnits = function () {
            $scope.getResourceUnits();
        };

        var contains = function (str1, str2) {
            if (!str1 || !str2) return false;
            return str1.toLowerCase().indexOf(str2.toLowerCase()) > -1;
        }

        $scope.listFilter = function (resourceUnit) {

            if (!$scope.filters.driver && !$scope.filters.vehicle)
                return true;

            if ($scope.filters.vehicle && !$scope.filters.driver)
                return contains(resourceUnit.vehicle.registrationNumber, $scope.filters.vehicle);

            if ($scope.filters.driver && !$scope.filters.vehicle)
                return contains(resourceUnit.driver.fullName, $scope.filters.driver);

            if ($scope.filters.driver && $scope.filters.vehicle)
                return (contains(resourceUnit.vehicle.registrationNumber, $scope.filters.vehicle) && contains(resourceUnit.driver.fullName, $scope.filters.driver));
        };

        $scope.openDatePicker = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.datePickerOpened = !$scope.datePickerOpened;
        };

        $scope.openLegPlanning = function () {
            window.open('/ng/legplanning');
        };


        //// WATCHES ///

        // listen for resource units created by other controllers (namely the crate/edit resource controller)
        $rootScope.$on('pe-app-resource-units-created', function (event, data) {
            $scope.getResourceUnits();
            $rootScope.$emit("pe-app-resource-units-updated", null);
        });

        $scope.$watch('filters.includeAllResourceUnits', function (val) {
            $scope.schedulerViewHeading = getSchedulerHeading();
        });

        $scope.$watch('filters.includeDrivers', function (val) {
            $scope.schedulerViewHeading = getSchedulerHeading();
        });


        //// PAGE SETUP ////

        $scope.$watch('scheduleState.toDateTime', function (val) {
            if (val !== null) {
                $scope.getResourceUnits();
            }
        });

        var vehicleTypesPromise = apiService.vehicleType.get().$promise.then(function (result) {
            $scope.vehicleTypes = result;
            $scope.filters.vehicleType = angular.copy($scope.vehicleTypes);
            initialiseFilterList($scope.vehicleTypes, $scope.filters.vehicleTypes, 'vehicleTypeID', $scope.isVehicleTypeChecked);
        });

    }
]);
