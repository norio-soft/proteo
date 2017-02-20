'use strict';

/// Simple click directive that doesn't cause memory leaks in the way
/// that ng-click does i.e. it unhooks listeners on destroy and doesn't use
/// jquery.on/off (leaks mystery 12 byte objects to jquery cache)
angular.module('peApp').directive('prSafeClick', ['$parse', '$rootScope', function ($parse, $rootScope) {
    return {
        restrict: 'A',
        scope: {
            'callback': '&prSafeClick',
        },
        link: function (scope, element, attrs) {

            function handler (event) {       
                scope.callback();
            }

            element[0].addEventListener('click', handler);

            scope.$on('$destroy', function () {
                element[0].removeEventListener('click', handler);
            });
        }
    };
}]);