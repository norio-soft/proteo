'use strict';

angular.module('peApp').directive('prValidTime', ['$parse', '$rootScope', function ($parse, $rootScope) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {

            var validateTime = function (viewValue) {
                var time24RegEx = new RegExp("^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
                var isValid = true;

                if (ctrl.$viewValue)
                    isValid = time24RegEx.test(ctrl.$viewValue);
               
                if (isValid) {
                    ctrl.$setValidity('isTimeValid', true);
                    return viewValue;
                } else {
                    ctrl.$setValidity('isTimeValid', false);
                    return viewValue;
                }
            }

            ctrl.$parsers.unshift(validateTime);
            ctrl.$formatters.push(validateTime);
            scope.$watch(attrs.prTimeValid, function () {
                validateTime(ctrl.$viewValue);
            });
        }
    };
}]);