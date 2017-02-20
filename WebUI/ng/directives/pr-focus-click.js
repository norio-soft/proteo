'use strict';

angular.module('peApp').directive('prFocusClick', [function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            function clickHandler() {
                element.focus();
                scope.$digest();
            }

            element.on('click', clickHandler);

            scope.$on('$destroy', function () {
                element.off('click', clickHandler)
            });
        }
    };
}]);