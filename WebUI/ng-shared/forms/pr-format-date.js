'use strict';

angular.module('proteo.shared.forms').directive('prFormatDate', [function () {
    return {
        require: 'ngModel',
        link: function ($scope, element, attrs, model, ctrl) {
            element.bind('blur', function () {
                if (element.val() > '' && element.val().length > 2) {
                    // Setup our values
                    var input = element.val();
                    var format = 'DD/MM/YY';
                    moment().locale('en');

                    var minimumValidLength = 4;
                    if (input.length < minimumValidLength) {
                        return;
                    }

                    var _date = moment(input, ['DD/MM/YY', 'DD/MM/YYYY', 'DDMMYY', 'DDMMYYYY']);

                    if (!_date.isValid()) {
                        return;
                    }

                    if (element.attr('date-format') !== undefined) {
                        format = element.attr('date-format');
                    }

                    $scope.$evalAsync(function () {
                        model.$setViewValue(_date.format(format));
                        model.$render();
                    });
                }
            });
        }
    };
}]);