'use strict';

// Display a warning icon with a tooltip if a form field is non-valid
angular.module('peApp').directive('prValidator', [function () {
    return {
        restrict: 'E',

        template:
            '<i ' +
            'tooltip="{{ checkInput(formControl.$error, fieldName || \'This field\') }}" ' +
            'tooltip-placement="{{ tooltipPlacement || \'right\'}}" ' +
            'tooltip-trigger="mouseenter" ' +
            'ng-style="{ visibility : (formControl.$touched || (onlyWhenTouched !== \'true\' && onlyWhenTouched !== \'\')) && formControl.$invalid ? \'\' : \'hidden\' }" ' +
            'class="glyphicon glyphicon-warning-sign">',

        scope: {
            'formControl': '=', // the name of the form control for which the validator is being displayed
            'fieldName': '@', // a friendly name for the field which is used in some tooltip messages
            'tooltipPlacement': '@', // default placement is 'right', specify here for other tooltip placement
            'onlyWhenTouched': '@', // add the only-when-touched attribute to the directive with no value (or ="true") to only display the validator once the form control has been touched
        },

        link: function (scope, element, attrs) {
            scope.checkInput = function (errorObject, fieldName) {
                if (!_.isObject(errorObject) || _.isEmpty(errorObject)) {
                    return;
                }

                if (errorObject.date) {
                    return fieldName + ' is not valid';
                }

                if (errorObject.required) {
                    return fieldName + ' is required';
                }

                if (errorObject.isTimeValid) {
                    return fieldName + ' is not valid';
                }

                if (errorObject.startDateGreater) {
                    return 'Finish date must be after start date.';
                }

                if (errorObject.startTimeGreater) {
                    return 'Finish time must be after start time';
                }

                if (errorObject.invalidDriver) {
                    return 'This is an invalid driver';
                }

                if (errorObject.unplannedDriver) {
                    return 'Driver has no assigned Planner';
                }

                if (errorObject.invalidVehicle) {
                    return 'This is an invalid vehicle';
                }

                if(errorObject.finishDateTimeLesser)
                {
                    return 'Finish must be after the start';
                }
            };
        },
    };
}]);