'use strict';


angular.module('peApp').directive('prEnter', ['$parse', '$rootScope', function ($parse, $rootScope) {
    return {
        restrict: 'A',


        link: function (scope, element, attrs) {

            function handler(event) {
                if (event.which === 13) {
                    var target = $(event.currentTarget);

                    // Check for an expanded timepicker - don't call enter function in this state
                    if (target.hasClass('ui-timepicker-input')) {
                        var picker = target.siblings('.ui-timepicker-wrapper').get(0);
                        if (picker !== undefined && picker.offsetHeight > 0 && picker.offsetWidth > 0) {
                            return;
                        }
                    }
                    //This will cause any validation attributes to be fired.
                    target.blur();

                    scope.$apply(function () {
                        scope.$eval(attrs.prEnter);
                    });

                    event.preventDefault();
                }
            }

            element[0].addEventListener('keydown', handler);

            scope.$on('$destroy', function () {
                element[0].removeEventListener('keydown', handler);
            });
        }
    };

}]);