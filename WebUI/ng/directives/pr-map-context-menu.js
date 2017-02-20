'use strict';

angular.module('peApp').directive('prMapContextMenu', [function () {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'html-partials/directives/map-context-menu.html',
        scope: {
            pin: '=',
            mapOptions: '='
        },
        link: function (scope, element, attrs) {
           
            //========= Scope Functions ===========

            scope.showVehicleHistory = function () {
                scope.$emit('showLocationHistory', scope.pin)
                scope.pin.closeContextMenu();
            }



        }
    };
}]);



