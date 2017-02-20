'use strict';

angular.module('peApp').controller('LegPlanningFiltersLegCtrl', ['$scope', '$rootScope', '$modal', '$q', 'FilterProperties', 'apiService',
    function ($scope, $rootScope, $modal, $q, FilterProperties, apiService) {
        /// === scope variables === ///

        var filterDefaults = {
            freeText: '',
            location: '',
            legState: {},
            trafficArea: {},
            controlArea: {},
            businessType: {},
            mwfCommunicationStatus: {}
        };

        $scope.legStates = [
            { legStateID: 1, description: 'Booked' },
            { legStateID: 2, description: 'Planned' },
            { legStateID: 3, description: 'In Progress' },
            { legStateID: 4, description: 'Complete' }
        ];

        $scope.filterProperties.legs = new FilterProperties(
            function () {
                return $scope.filters.legs;
            },
            function (filter) {
                $scope.filters.legs = filter;
            },
            filterDefaults);

        /// === functions === ///

        var initialiseFilters = function () {
            var getTrafficAreas = apiService.trafficArea.get().$promise;
            var getControlAreas = apiService.controlArea.get().$promise;
            var getBusinessTypes = apiService.businessType.get().$promise;
            var getMwfCommunicationStatuses = apiService.mwfCommunicationStatus.get().$promise;

            $q.all([
                getTrafficAreas,
                getControlAreas,
                getBusinessTypes,
                getMwfCommunicationStatuses
            ]).then(function (results) {
                $scope.trafficAreas = results[0];
                $scope.controlAreas = results[1];
                $scope.businessTypes = results[2];
                $scope.mwfCommunicationStatuses = results[3];

                initialiseFilterList($scope.legStates, filterDefaults.legState, 'legStateID', true);
                initialiseFilterList($scope.trafficAreas, filterDefaults.trafficArea, 'description', true);
                initialiseFilterList($scope.controlAreas, filterDefaults.controlArea, 'description', true);
                initialiseFilterList($scope.businessTypes, filterDefaults.businessType, 'description', true);
                initialiseFilterList($scope.mwfCommunicationStatuses, filterDefaults.mwfCommunicationStatus, 'description', true);

                $scope.filters.legs = angular.copy(filterDefaults);
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
                case 'legState':
                    initialiseFilterList($scope.legStates, $scope.filters.legs.legState, 'legStateID', $scope.isLegStateChecked);
                    break;
                case 'trafficArea':
                    initialiseFilterList($scope.trafficAreas, $scope.filters.legs.trafficArea, 'description', $scope.isTrafficAreaChecked);
                    break;
                case 'controlArea':
                    initialiseFilterList($scope.controlAreas, $scope.filters.legs.controlArea, 'description', $scope.isControlAreaChecked);
                    break;
                case 'businessType':
                    initialiseFilterList($scope.businessTypes, $scope.filters.legs.businessType, 'description', $scope.isBusinessTypeChecked);
                    break;
                case 'mwfCommunicationStatus':
                    initialiseFilterList($scope.mwfCommunicationStatuses, $scope.filters.legs.mwfCommunicationStatus, 'description', $scope.isMwfCommunicationChecked);
                    break;
            }
        };

        /// === page initialization === ///

        initialiseFilters();

    }]);
