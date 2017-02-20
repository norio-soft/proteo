'use strict';

angular.module('peApp').controller('ConfirmationModalCtrl', ['$scope', '$rootScope', '$modalInstance', 'modalBody', 'confirmationFunction', 'params', 'data',
    function ($scope, $rootScope, $modalInstance, modalBody, confirmationFunction, params, data) {

        $scope.confirmationBody = modalBody;


        $scope.ok = function () {
            $modalInstance.close('ok');
            confirmationFunction(params, data);
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };
    }]);