'use strict';

angular.module('peApp').controller('DriverNoPlannerModalCtrl', ['$scope', '$rootScope', '$modalInstance', 'data',
    function ($scope, $rootScope, $modalInstance, data) {

        $scope.data = data;

        $scope.ok = function () {
            $modalInstance.close('ok');
        };

    }]);