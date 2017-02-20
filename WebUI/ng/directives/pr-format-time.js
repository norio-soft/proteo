'use strict';

angular.module('peApp').directive('prFormatTime', [function () {
    return {
        require: 'ngModel',
        link: function ($scope, element, attrs, model) {
            element.bind('blur', function () {
                if (element.val() > '' && element.val().length > 2) {
                    // Setup our values
                    var input = element.val();
                    var hours = '';
                    var minutes = '';

                    var colonDelimiter = input.indexOf(':');

                    if (colonDelimiter > 0) {
                        // We have delimiters in the string so break up based on that
                        var splitInput = input.split(':');

                        hours = splitInput[0];
                        minutes = splitInput[1];

                    } else {
                        if (input.length < 3) {
                            return;
                        }
                        //If input is length 3 it will format to having a 0 at the front.
                        //e.g. 213 -> 02:13
                        if (input.length == 3) {
                            hours = input.slice(0, 1);
                            minutes = input.slice(1, 3);
                        }
                        else {
                            // We have no delimiters so work it out
                            hours = input.slice(0, 2);
                            minutes = input.slice(2, 4);
                        }
                    }

                    // Check hours for 2 digits if not prefix with 0
                    if (hours.length == 1) {
                        hours = '0' + hours;
                    }
                    // Check minutes for 2 digits if not prefix with 0
                    if (minutes.length == 1) {
                        minutes = '0' + minutes;
                    }
                    var value = hours + ':' + minutes;

                    element.val(value);
                    $scope.$apply(function () {
                        model.$setViewValue(value);
                    });
                }
            });
        }
    };
}]);