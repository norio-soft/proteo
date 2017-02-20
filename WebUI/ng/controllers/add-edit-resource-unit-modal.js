'use strict';


angular.module('peApp').controller('AddEditResourceUnitModalCtrl', ['$scope', '$rootScope', '$modalInstance', '$modal', '$timeout', 'resourceUnit', 'onResourceUnitEdited', 'apiService',
    function ($scope, $rootScope, $modalInstance, $modal, $timeout, resourceUnit, onResourceUnitEdited, apiService) {


        //// VARIABLES ////

        var driverTypes = [];

        $scope.defaults = {
            driver: '',
            vehicle: '',
            startDateTime: '',
            startDate: moment({ hour: 9 }).toDate(),
            startTime: '09:00',
            finishDateTime: '',
            finishDate: moment({ hour: 17 }).toDate(),
            finishTime: '17:00',
        };

        $scope.resourceUnit = {};


        $scope.driverHasFocus = false;
        $scope.vehicleHasFocus = false;

        $scope.timePickerOptions = {
            timeFormat: 'H:i'
        };


        $scope.createAnother = false;


        //// FUNCTIONS ////

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        // Date pickers
        $scope.openStartDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.startDateOpened = true;
        };

        $scope.openFinishDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.finishDateOpened = true;
        };

        // Get drivers from API
        $scope.getDrivers = function (val) {
            return apiService.driver.getDrivers({ searchTerm: val, allDrivers: true }).$promise;
        };

        $scope.getVehicles = function (val) {
            return apiService.vehicle.getVehicles({ searchTerm: val }).$promise;
        };

        var getDriverTypes = function () {
            var getDriverTypes = apiService.driverType.getDriverTypes().$promise.then(function (results) {
                driverTypes = results.map(function (result) {

                    var startTime = (result.startTime) ? moment(result.startTime) : null;
                    var finishTime = (result.finishTime) ? moment(result.finishTime) : null;

                    if (startTime) {
                        startTime = moment().hour(startTime.hour()).minute(startTime.minute());
                    }

                    if (finishTime) {
                        finishTime = moment().hour(finishTime.hour()).minute(finishTime.minute());
                        if (finishTime.isBefore(startTime)) {
                            finishTime = finishTime.add(1, 'days');
                        }
                    }

                    return {
                        description: result.description,
                        startTime: startTime,
                        finishTime: finishTime
                    };
                });
            });
        };

        $scope.deleteResourceUnit = function (resourceUnitID) {
            apiService.resourceUnit.delete({ resourceUnitID: resourceUnitID }).$promise.then(function () {
                $modalInstance.dismiss('cancel');
                // let the calling code know the resource unit has been edited
                onResourceUnitEdited();
            });
        };

        var doUpdate = function (newResourceUnit) {
            var params = { allowConflicts: false };
            apiService.resourceUnit.update(params, newResourceUnit)
           .$promise.then(function (response) {
               // update the original resource unit with any modifications
               angular.copy($scope.resourceUnit, resourceUnit);
               $modalInstance.dismiss('cancel');
               // let the calling code know the resource unit has been edited
               onResourceUnitEdited(resourceUnit);
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
               }
           });
        };

        var doCreate = function (newResourceUnit) {
            var params = { allowConflicts: false };

            apiService.resourceUnit.create(params, newResourceUnit)
           .$promise.then(function (response) {

               if ($scope.createAnother) {
                   $scope.resetForm();
                   $rootScope.$emit("pe-app-resource-units-created", null);
               }
               else {
                   $modalInstance.dismiss('cancel');
                   // let the calling code know the resource unit has been edited
                   onResourceUnitEdited(newResourceUnit);
               }
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
                $modalInstance.dismiss('cancel');
                // let the calling code know the resource unit has been edited
                onResourceUnitEdited(resourceUnit);
            });
        };

        var resendResourceUnitUpdate = function (params, resourceUnit) {

            params.allowConflicts = true;

            apiService.resourceUnit.update(params, resourceUnit).$promise.then(function (response) {
                $modalInstance.dismiss('cancel');
                // let the calling code know the resource unit has been edited
                onResourceUnitEdited(resourceUnit);
            });
        };

        $scope.saveResourceUnit = function () {
            // Do Validation
            $scope.$broadcast('show-errors-check-validity');

            $scope.addEditResourceUnitForm.finishtime.$setValidity('finishDateTimeLesser', true);
            $scope.addEditResourceUnitForm.finishdate.$setValidity('finishDateTimeLesser', true);

            $scope.resourceUnit.startDateTime = moment(moment($scope.resourceUnit.startDate).format('YYYY-MM-DD') + 'T' +  $scope.resourceUnit.startTime);
            $scope.resourceUnit.finishDateTime = moment(moment($scope.resourceUnit.finishDate).format('YYYY-MM-DD') + 'T' + $scope.resourceUnit.finishTime);

            if ($scope.resourceUnit.startDateTime > $scope.resourceUnit.finishDateTime) {
                $scope.addEditResourceUnitForm.finishtime.$setValidity('finishDateTimeLesser', false);
                $scope.addEditResourceUnitForm.finishdate.$setValidity('finishDateTimeLesser', false);
            }

            if ($scope.addEditResourceUnitForm.$invalid) {
                $scope.saveWait = false;
                return;
            }

            var newResourceUnit = {
                resourceUnitID: $scope.resourceUnit.resourceUnitID,
                driverResourceID: $scope.resourceUnit.driver.resourceID,
                vehicleResourceID: $scope.resourceUnit.vehicle.resourceID,
                activeFrom: $scope.resourceUnit.startDateTime,
                activeTo: $scope.resourceUnit.finishDateTime
            }

            // Do Save
            if ($scope.isEdit) {
                doUpdate(newResourceUnit);
            }
            else {
                doCreate(newResourceUnit);
            }

        };

        var updateTimesForDriverType = function (driver) {
            var driverType = _.find(driverTypes, function (item) {
                return item.description === driver.driverType;
            });

            if (driverType) {

                if (driverType.startTime) {
                    $scope.resourceUnit.startDate = driverType.startTime.toDate();
                    $scope.resourceUnit.startTime = driverType.startTime.format('HH:mm');
                }
                if (driverType.finishTime) {
                    $scope.resourceUnit.finishDate = driverType.finishTime.toDate();
                    $scope.resourceUnit.finishTime = driverType.finishTime.format('HH:mm');
                }

            }
        };

        $scope.resetForm = function () {
            $scope.addEditResourceUnitForm.$setPristine();
            $scope.resourceUnit = angular.copy($scope.defaults);
            $scope.$broadcast('show-errors-reset');
        };

        $scope.setDriver = function (driver) {
            $scope.resourceUnit.driver = driver;

            if ($scope.resourceUnit.driver.plannerIdentityID == null) {
                $scope.addEditResourceUnitForm.driver.$setValidity('unplannedDriver', false);
            }
            else {
                $scope.addEditResourceUnitForm.driver.$setValidity('unplannedDriver', true);
            }

            setDriverValidity(true);
        };

        $scope.driverBlurred = function () {
            setDriverValidity(true);

            if ($scope.resourceUnit.driver && !$scope.resourceUnit.driver.fullName) {
                $scope.getDrivers($scope.resourceUnit.driver).then(function (result) {
                    $timeout(function () {
                        // check whether a driver has been selected in the interim while the request was in progress (by setDriver)
                        if ($scope.resourceUnit.driver.fullName) {
                            return;
                        }

                        if (result.length === 1) {
                            $scope.resourceUnit.driver = result[0];

                            if ($scope.resourceUnit.driver.plannerIdentityID == null) {
                                $scope.addEditResourceUnitForm.driver.$setValidity('unplannedDriver', false);
                            }
                            else {
                                $scope.addEditResourceUnitForm.driver.$setValidity('unplannedDriver', true);
                            }

                        }
                        else {
                            setDriverValidity(false);
                        }
                    }, 100);
                });
            }
        };

        $scope.setVehicle = function (vehicle) {
            $scope.resourceUnit.vehicle = vehicle;
            setVehicleValidity(true);
        };

        $scope.vehicleBlurred = function () {
            setVehicleValidity(true);

            if ($scope.resourceUnit.vehicle && !$scope.resourceUnit.vehicle.registrationNumber) {
                $scope.getVehicles($scope.resourceUnit.vehicle).then(function (result) {
                    // check whether a vehicle has been selected in the interim while the request was in progress (by setVehicle)
                    if ($scope.resourceUnit.vehicle.registrationNumber) {
                        return;
                    }

                    if (result.length === 1) {
                        $scope.resourceUnit.vehicle = result[0];
                    }
                    else {
                        setVehicleValidity(false);
                    }
                });
            }
        };

        var setDriverValidity = function (isValid) {
            $scope.addEditResourceUnitForm.driver.$setValidity('invalidDriver', isValid);
        };

        var setVehicleValidity = function (isValid) {
            $scope.addEditResourceUnitForm.vehicle.$setValidity('invalidVehicle', isValid);
        };


        //// WATCHES ////

        $scope.$watch('resourceUnit.driver', function (driver) {
            if (!$scope.isEdit && driver && driver.driverType) {
                updateTimesForDriverType(driver);
            }
        });

        //// PAGE SETUP ////

        if (resourceUnit) {
            // make a copy of the resource unit
            $scope.isEdit = true;
            $scope.resourceUnit = angular.copy(resourceUnit);
            $scope.resourceUnit.startDate = $scope.resourceUnit.startDateTime;
            $scope.resourceUnit.finishDate = $scope.resourceUnit.finishDateTime;
            $scope.resourceUnit.startTime = moment($scope.resourceUnit.startDateTime).format('HH:mm');
            $scope.resourceUnit.finishTime = moment($scope.resourceUnit.finishDateTime).format('HH:mm');
            $scope.modalTitle = 'Edit Resource Unit';
        }
        else {
            $scope.isEdit = false;
            $scope.resourceUnit = angular.copy($scope.defaults);
            $scope.modalTitle = 'Create Resource Unit';
        }

        getDriverTypes();


    }]);
