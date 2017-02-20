'use strict';

angular.module('controlAreaManagementApp').controller('AddCustomerModalCtrl', [
    '$scope', 'apiService', 'controlArea', '$modalInstance',
    function ($scope, apiService, controlArea, $modalInstance) {

        //// VARIABLES ////

        $scope.customers = [];
        $scope.customer = {
            selectedCustomer: null
        };

        //// SCOPE FUNCTIONS ////

        $scope.isAddDisabled = function () {
            return !$scope.customer.selectedCustomer;
        }

        $scope.addCustomer = function () {

            if ($scope.saving) {
                return;
            }

            $scope.saving = true;

            apiService.controlArea.addCustomer({ controlAreaId: $scope.controlArea.controlAreaId, organisationId:  $scope.customer.selectedCustomer.organisationID }, {}).$promise.then(function () {
                $modalInstance.close();
            }).finally(function () {
                $scope.saving = false;
            });

        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
  

        //// PAGE SETUP ////
        $scope.controlArea = controlArea;

        apiService.organisation.get({ controlAreaId: controlArea.controlAreaId, organisationTypeId: 2, excludeControlArea: true }).$promise.then(function (result) {
            $scope.customers = result;
        });
    }
]);