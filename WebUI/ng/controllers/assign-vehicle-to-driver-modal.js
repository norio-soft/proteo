'use strict';

angular.module('peApp').controller('AssignVehicleToDriverModalCtrl', ['$scope', '$modalInstance', 'driverResourceID', 'driverFullName', 'resourceUnits', 'legs', 'apiService',
    function ($scope, $modalInstance, driverResourceID, driverFullName, resourceUnits, legs, apiService) {

        //// VARIABLES ////
        $scope.timePickerOptions = {
            timeFormat: 'H:i'
        };


        //// FUNCTIONS ////

        $scope.getVehicles = function (val) {
            return apiService.vehicle.getVehicles({ searchTerm: val }).$promise;
        };

        $scope.save = function () {
            // Validation
            $scope.$broadcast('show-errors-check-validity');

            if ($scope.assignVehicleForm.$invalid) {
                return;
            }

            var params = {
                driverResourceID: $scope.data.driverResourceID,
                vehicleResourceID: $scope.data.vehicle.resourceID,
            };

            var data = {
                resourceUnits: $scope.data.resourceUnits
                    .filter(function (resourceUnit) {
                        return resourceUnit.isSelected;
                    })
                    .map(function (resourceUnit) {
                        var startDate = moment(resourceUnit.startDate).format('DD/MM/YYYY');
                        var startTime =resourceUnit.startTime;

                        var finishDate = moment(resourceUnit.finishDate).format('DD/MM/YYYY');
                        var finishTime = resourceUnit.finishTime;

                        return {
                            startDateTime: moment(startDate + ' ' + startTime, 'DD/MM/YYYY HH:mm').toDate(),
                            finishDateTime: moment(finishDate + ' ' + finishTime, 'DD/MM/YYYY HH:mm').toDate(),
                        };
                    }),

                legs: $scope.data.legs
                    .filter(function (leg) {
                        return leg.isSelected;
                    })
                    .map(function (leg) {
                        return {
                            jobID: leg.jobID,
                            jobLastUpdateDateTime: leg.jobLastUpdateDateTime,
                            startInstructionID: leg.startInstructionID,
                            endInstructionID: leg.endInstructionID,
                        };
                    }),
            };

            // Save
            apiService.driver.assignVehicle(params, data).$promise.then(function (response) {
                $modalInstance.close();
            });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        // Date picker
        $scope.openStartDatePicker = function ($event, resourceUnit) {
            $event.preventDefault();
            $event.stopPropagation();

            closeAllDatePickers();
            resourceUnit.startDateOpen = true;
        };

        $scope.openFinishDatePicker = function ($event, resourceUnit) {
            $event.preventDefault();
            $event.stopPropagation();

            closeAllDatePickers();
            resourceUnit.finishDateOpen = true;
        };

        var closeAllDatePickers = function () {
            $scope.data.resourceUnits.forEach(function (resourceUnit) {
                resourceUnit.startDateOpen = false;
                resourceUnit.finishDateOpen = false;
            });
        };

        $scope.setVehicle = function (vehicle) {
            $scope.data.vehicle = vehicle;
            setVehicleValidity(true);
        };

        $scope.vehicleBlurred = function () {
            setVehicleValidity(true);

            if ($scope.data.vehicle && !$scope.data.vehicle.registrationNumber) {
                $scope.getVehicles($scope.data.vehicle).then(function (result) {
                    // check whether a vehicle has been selected in the interim while the request was in progress (by setVehicle)
                    if ($scope.data.vehicle.registrationNumber) {
                        return;
                    }

                    if (result.length === 1) {
                        $scope.data.vehicle = result[0];
                    }
                    else {
                        setVehicleValidity(false);
                    }
                });
            }
        };

        var setVehicleValidity = function (isValid) {
            $scope.assignVehicleForm.vehicle.$setValidity('invalidVehicle', isValid);
        };


        //// PAGE SETUP ////

        $scope.data = {
            driverResourceID: driverResourceID,
            driverFullName: driverFullName,
            vehicle: null,

            resourceUnits: resourceUnits.map(function (resourceUnit, index) {
                var startDateTime = new Date(resourceUnit.startDateTime);
                var finishDateTime = new Date(resourceUnit.finishDateTime);

                return {
                    startDate: startDateTime,
                    startTime: moment(startDateTime).format('HH:mm'),
                    finishDate: finishDateTime,
                    finishTime: moment(finishDateTime).format('HH:mm'),
                    isSelected: index === 0,
                };
            }),

            legs: legs.map(function (leg) {
                return {
                    jobID: leg.jobID,
                    jobLastUpdateDateTime: leg.jobLastUpdateDateTime,
                    startInstructionID: leg.startInstructionID,
                    endInstructionID: leg.endInstructionID,
                    startPointDescription: leg.startPointDescription,
                    startDateDisplay: leg.startDateDisplay,
                    endPointDescription: leg.endPointDescription,
                    endDateDisplay: leg.endDateDisplay,
                    vehicleRegistration: leg.vehicleRegistration,
                    isSelected: true,
                };
            }),
        };

    }]);