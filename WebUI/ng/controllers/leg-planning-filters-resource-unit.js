'use strict';

angular.module('peApp').controller('LegPlanningFiltersResourceUnitCtrl', ['$scope', '$rootScope', '$modal', '$q', 'FilterProperties', 'apiService',
    function ($scope, $rootScope, $modal, $q, FilterProperties, apiService) {
        /// === scope variables === ///

        var filterDefaults = {
            freeText: '',
            driverName: '',
            vehicleRegistration: '',
            todayFinishTrafficArea: {},
            todayFinishTown: '',
            driverType: {},
            orderResourceUnitsBy: 'driver.lastName'

        };

        $scope.filterProperties.resourceUnits = new FilterProperties(
            function () { return $scope.filters.resourceUnits; },
            function(filter) { $scope.filters.resourceUnits = filter; },
            filterDefaults);

        $scope.orderResourceUnitsByFields = [
            { field: 'driver.lastName', displayName: "Driver Last Name"},
            { field: 'vehicle.registrationNumber', displayName: "Vehicle Registration No" },
            { field: 'driver.startTime', displayName: 'Driver Start Time' },
            { field: 'driver.finishTime', displayName: 'Driver Finish Time' },
            { field: 'driver.todayFinishTown', displayName: 'Driver Finish Town' },
            { field: 'driver.todayFinishTrafficArea', displayName: 'Driver Finish Area' },       
        ];

        /// === functions === ///

        var initialiseFilter = function () {
            var getDriverTypes = apiService.driverType.getDriverTypes().$promise;
            var getTrafficAreas = apiService.trafficArea.get().$promise;

            $q.all([
                getDriverTypes,
                getTrafficAreas
            ]).then(function (results) {
                $scope.driverTypes = results[0];
                $scope.trafficAreas = results[1];

                initialiseFilterList($scope.driverTypes, filterDefaults.driverType, 'description', true);
                initialiseFilterList($scope.trafficAreas, filterDefaults.todayFinishTrafficArea, 'description', true);

                $scope.filters.resourceUnits = angular.copy(filterDefaults);

                if (sessionStorage.resourceUnitOrder)
                    $scope.filters.resourceUnits.orderResourceUnitsBy = JSON.parse(sessionStorage.resourceUnitOrder);
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
                    initialiseFilterList($scope.driverTypes, $scope.filters.resourceUnits.driverType, 'description', $scope.isDriverTypeChecked);
                    break;
                case 'todayFinishTrafficArea':
                    initialiseFilterList($scope.trafficAreas, $scope.filters.resourceUnits.todayFinishTrafficArea, 'description', $scope.isTodayFinishTrafficAreaChecked);
                    break;
            }
        };

        /// === page initialization === ///

        initialiseFilter();

    }]);
