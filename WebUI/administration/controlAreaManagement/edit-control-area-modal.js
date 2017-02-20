'use strict';

angular.module('controlAreaManagementApp').controller('EditControlAreaModal', [
    '$scope', 'apiService', 'controlArea', '$modalInstance',
    function ($scope, apiService, controlArea, $modalInstance) {


        //// SCOPE FUNCTIONS ////

        $scope.saveControlArea = function () {

            //Validation
            $scope.$broadcast('show-errors-check-validity');

            if ($scope.controlAreaForm.$invalid || $scope.saving) {
                return;
            }

            if ($scope.controlArea.controlAreaId) {
                $scope.saving = true;
                apiService.controlArea.put({ controlAreaId: $scope.controlArea.controlAreaId }, $scope.controlArea).$promise.then(function () {
                    var result = { controlArea: $scope.controlArea, edit: true };
                    $modalInstance.close(result);
                }).finally(function () {
                    $scope.saving = false;
                });
            }
            else {
                $scope.saving = true;
                apiService.controlArea.post($scope.controlArea).$promise.then(function (response) {
                    var result = { controlArea: response, edit: false };
                    $modalInstance.close(result);
                }).finally(function () {
                    $scope.saving = false;
                });
            }
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
  

        //// PAGE SETUP ////
        $scope.controlArea = controlArea;

    }
]);