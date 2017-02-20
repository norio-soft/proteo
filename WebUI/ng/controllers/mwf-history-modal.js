'use strict';

angular.module('peApp').controller('MWFHistoryModalCtrl', ['$scope', '$modalInstance', 'startInstructionID','endInstructionID','apiService',
    function ($scope, $modalInstance, startInstructionID, endInstructionID, apiService) {

        //// VARIABLES ////
        $scope.legHistory = {};

        //// FUNCTIONS ////

        var getLegHistory = function (startInstructionID, endInstructionID) {

            var params = { startInstructionID: startInstructionID, endInstructionID: endInstructionID };

            apiService.legHistory.getLegHistory(params).$promise.then(
                function (response) {
                    $scope.legHistory = response;
                },
                function () {
                    // Error
                });
        };

        $scope.close = function () {
            $modalInstance.close();
        };


        //// PAGE SETUP ////

        getLegHistory(startInstructionID, endInstructionID);
    }]);