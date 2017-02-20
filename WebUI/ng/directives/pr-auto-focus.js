'use strict';

angular.module('peApp').directive('prAutoFocus', ['$parse', '$timeout', function ($parse, $timeout) {
    return {
        restrict: 'AC',
        link: function (scope, element, attrs) {
            var value = $parse(attrs.prAutoFocus)(scope);

            if (value === false) {
                return;
            }

            $timeout(function () {
                var el = element.first();
                el.focus();
                el.select();
            }, 50);
        }
    };
}]);
