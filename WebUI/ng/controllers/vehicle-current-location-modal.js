'use strict';

angular.module('peApp').controller('VehicleCurrentLocationModalCtrl', ['$scope', '$modalInstance', '$sce', 'gpsUnitID', function ($scope, $modalInstance, $sce, gpsUnitID) {

    //// VARIABLES ////
    var url = '../GPS/GetCurrentLocation.aspx?uid=' + gpsUnitID;
    $scope.gpsUnitUrl = $sce.trustAsResourceUrl(url);

    //// FUNCTIONS ////
    $scope.close = function () {
        $modalInstance.dismiss('cancel');
    };

    //// PAGE SETUP ////

}]);
