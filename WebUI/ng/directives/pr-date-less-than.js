'use strict';

angular.module('peApp').directive('prDateLessThan', ['$parse', '$rootScope', function ($parse, $rootScope) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, element, attrs, ctrl) {

            var validateDates = function(viewValue) {
                ctrl.$setValidity('startDateGreater', true);

                // format start date
                var parsedStartDate = $parse(attrs.prDateLessThan);
                var newStartDate = new Date(parsedStartDate(scope));
                var startDateFormatted = moment(newStartDate).format('DD/MM/YYYY');
                var startDate = moment(startDateFormatted, 'DD/MM/YYYY').toDate();

                // format finish date
                var finishDateFormatted = moment(viewValue).format('DD/MM/YYYY');
                var finishDate = moment(finishDateFormatted, 'DD/MM/YYYY').toDate();

                if (finishDate < startDate) {
                    ctrl.$setValidity('startDateGreater', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('startDateGreater', true);
                    return viewValue;
                }
            }

            ctrl.$parsers.unshift(validateDates);
            ctrl.$formatters.push(validateDates);
            scope.$watch(attrs.prDateLessThan, function() {
                validateDates(ctrl.$viewValue);
            });
        }
    };
}]);