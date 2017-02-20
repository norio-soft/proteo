'use strict';

angular.module('peApp').controller('AvailableDriversCtrl', ['$scope', '$rootScope', 'apiService', '$modal', 'FilterProperties', '$q', '$filter', 'ipCookie', 'authenticationService',
    function ($scope, $rootScope, apiService, $modal, FilterProperties, $q, $filter, ipCookie, authenticationService) {

        /// VARIABLES ///

        $scope.driverFiltersForm = {
            searchText: '',
            includeAllDrivers: false,
            driverType: {}
        };

        $scope.filterProperties.drivers = new FilterProperties(
            function () { return $scope.filters.drivers; },
            function (filter) { $scope.filters.drivers = filter; },
            $scope.driverFiltersForm);

        var unfilteredDays;
        $scope.driverRequests = [];

        // Drop down
        $scope.status = {
            isopen: false
        };

        $scope.statusMedium = {
            isopen: false
        };

        /// functions ///

        var applyFilters = function () {
            //If unfiltered days is null it means that driver API call has not returned yet. So no point in filtering
            if (unfilteredDays) {
                var filterOnDrivers = $scope.filterProperties.drivers;
                var filteredDays = angular.copy(unfilteredDays);
                if (filterOnDrivers) {
                    _.each(filteredDays, filterDrivers);
                }
                $scope.days = filteredDays;
            }
        };

        var filterDrivers = function (day) {
            switch ($scope.filters.includeDrivers) {
                case 'onlyMyDrivers':
                    $scope.filters.drivers.includeAllDrivers = false;
                    break;
                case 'includeAllDrivers':
                    $scope.filters.drivers.includeAllDrivers = true;
                    break;
                case 'anotherPlannersDrivers':
                    $scope.filters.drivers.includeAllDrivers = false;
                    break;
            }

            $scope.filters.drivers.plannerId = $scope.filters.viewingPlanner;
            day.drivers = $filter('resourceUnitDriverType')(day.drivers, $scope.filters.drivers);            
        };

        var initialiseFilter = function () {
            var getDriverTypes = apiService.driverType.getDriverTypes().$promise;

            $q.all([
                getDriverTypes
            ]).then(function (results) {
                $scope.driverTypes = results[0];

                initialiseFilterList($scope.driverTypes, $scope.driverFiltersForm.driverType, 'description', true);

                $scope.filters.drivers = angular.copy($scope.driverFiltersForm);
            });
        };

        //Used to initialise the filter and also clear it down.
        var initialiseFilterList = function (items, filters, filterOn, valueTo) {
            _.each(items, function (item) {
                filters[item[filterOn]] = valueTo;
            });
        };
        $scope.unTickAll = function (items) {

            switch (items) {
                case 'driverType':
                    initialiseFilterList($scope.driverTypes, $scope.filters.drivers.driverType, 'description', $scope.isDriverTypeChecked);
                    break;
            }
        };

        $scope.toggleDropdown = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
        };

        var createDay = function (i) {
            var newDate = moment($scope.scheduleState.fromDateTime).startOf('day').add(i, 'days');
            return { date: newDate, dayName: newDate.format('dddd'), shortDayName: newDate.format('ddd'), drivers: [] };
        };

        // Driver request modal
        $scope.viewDriverRequests = function (driver) {

            $modal.open({
                templateUrl: 'html-partials/view-driver-requests-modal.html',
                controller: 'viewDriverRequestsModalCtrl',
                size: 'lg',
                resolve: {
                    driver: function () {
                        return driver;
                    },
                }
            });
        };

        var updateDrivers = function () {
            
            if ($scope.filters.includeDrivers === 'includeAllResourceUnits') {
                $scope.filters.viewingPlanner = null;
            }

            var driversParameters = {
                availableDates: $scope.days.map(function (day) { return day.date.toJSON(); }),
                allDrivers: ($scope.filters.includeDrivers != 'onlyMyDrivers')
            };

            var driversPromise = apiService.driver.getDrivers(driversParameters).$promise;
            var driverRequestsPromise = apiService.driverRequests.getDriverRequests().$promise;

            $q.all([driversPromise, driverRequestsPromise]).then(function (results) {
                var daysWithDrivers = results[0];
                $scope.driverRequests = results[1];

                daysWithDrivers.forEach(function (dayWithDrivers) {
                    var day = _.find($scope.days, function (day) {
                        return day.date.isSame(dayWithDrivers.date);
                    });

                    day.drivers = dayWithDrivers.drivers;

                    for (var i = 0; i < $scope.driverRequests.length; i++) {
                        for (var j = 0; j < day.drivers.length; j++) {
                            if (day.drivers[j].resourceID == $scope.driverRequests[i].resourceID) {
                                day.drivers[j].pendingDriverRequest = true;
                            }
                        }
                    }
                });

                // Take a copy to act as the "original" for all subsequent filtering.
                unfilteredDays = $scope.days;
                applyFilters();
            });
        };

        var updateDaysAndDrivers = function () {
            var fromDateTime = $scope.scheduleState.fromDateTime;
            var toDateTime = $scope.scheduleState.toDateTime;

            if (fromDateTime && toDateTime) {
                var dayRange = moment(toDateTime).diff(moment(fromDateTime), 'days'); 
                $scope.days = _.range(0, dayRange).map(createDay);
                updateDrivers();
            }
        }

        $scope.filterAvailableDrivers = function () {
            updateDrivers();
            $scope.status.isopen = false;
            $scope.statusMedium.isopen = false;

            var resourcesFilter;
            switch ($scope.filters.includeDrivers) {
                case 'onlyMyDrivers':
                    resourcesFilter = 'onlyMyResourceUnits';
                    $scope.filters.includeAllResourceUnits = false;
                    $scope.filters.viewingPlanner = authenticationService.getUserID();
                    break;
                case 'includeAllDrivers':
                    resourcesFilter = 'includeAllResourceUnits';
                    $scope.filters.includeAllResourceUnits = true;
                    $scope.filters.viewingPlanner = null;
                    break;
                case 'anotherPlannersDrivers':
                    resourcesFilter = 'anotherPlannersResourceUnits';
                    $scope.filters.includeAllResourceUnits = false;
                    break;
            }

            var resourceViewFilters =
            {
                resourcesFilter: resourcesFilter,
                planner: $scope.filters.viewingPlanner
            };
            ipCookie('resourceViewFilters', resourceViewFilters);
            $scope.getResourceUnits(true);
            //console.log("STORED COOKIE : " + JSON.stringify(ipCookie('resourceViewFilters')));
        };

        /// event handling ///

        // if resource units are updated then update driver availibility
        $rootScope.$on('pe-app-resource-units-updated', function (event, data) {
            updateDrivers();
        });

        /// watches ///

        // if the scheduler date changes then rebuild days and get drivers
        $scope.$watch('scheduleState.toDateTime', function (newDate, oldDate) {
            if (newDate !== null && (!$scope.days || oldDate.toDateString() != newDate.toDateString())) {
                updateDaysAndDrivers();
            }
        });

        $scope.$watch('filters.drivers', function (val) {
            applyFilters();
        }, true);

        var loadPlannerList = function () {
            var params = { systemPortionID: window.enums.systemPortion.plan };

            apiService.users.getForSystemPortion(params).$promise.then(function (result) {
                $scope.plannerUsers = result;
            });
        };

        /// initialization ///
        initialiseFilter();
        loadPlannerList();

    }]);
