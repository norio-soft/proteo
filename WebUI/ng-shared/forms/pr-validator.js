'use strict';

// Display a warning icon with a tooltip if a form field is non-valid
angular.module('proteo.shared.forms').directive('prValidator', [function () {
    return {
        restrict: 'E',

        template:
            '<i ' +
            'tooltip="{{ checkInput(formControl, fieldName || \'This field\') }}" ' +
            'tooltip-placement="{{ tooltipPlacement || \'right\'}}" ' +
            'tooltip-trigger="mouseenter" ' +
            'ng-style="{ visibility : (validityCheckForced ||formControl.$touched || (onlyWhenTouched !== \'true\' && onlyWhenTouched !== \'\')) && formControl.$invalid ? \'\' : \'hidden\' }" ' +
            'class="glyphicon glyphicon-warning-sign">',

        scope: {
            'formControl': '=', // the name of the form control for which the validator is being displayed
            'controlRuleValue': '=', //value of condition that trigger the error. (For example min length 4 set this property to 4)
            'parentForm': '=?',
            'childFormName': '@?',
            'controlName': '@?',
            'fieldName': '@', // a friendly name for the field which is used in some tooltip messages
            'tooltipPlacement': '@', // default placement is 'right', specify here for other tooltip placement
            'onlyWhenTouched': '@', // add the only-when-touched attribute to the directive with no value (or ="true") to only display the validator once the form control has been touched
            'customMessage': '&', // an optional function that will examine the error object and return a custom validation message string where appropriate
        },


        link: function (scope, element, attrs) {

            if (!scope.formControl) {
                scope.formControl = scope.parentForm[scope.childFormName][scope.controlName];
            }

            scope.validityCheckForced = false;

            scope.$on('show-errors-check-validity', function () {
                scope.validityCheckForced = true;
            });

            scope.checkInput = function (formControl, fieldName) {
                if (formControl === undefined) {
                    return null;
                }

                var errorObject = formControl.$error;

                if (!_.isObject(errorObject) || _.isEmpty(errorObject)) {
                    return null;
                }

                if (errorObject.pattern || errorObject.dateFormat || errorObject.isTimeValid || errorObject.email) {
                    return fieldName + ' is not valid.';
                }

                if (errorObject.required) {
                    return fieldName + ' is required';
                }

                if (errorObject.startDateGreater) {
                    return 'Finish date must be after start date.';
                }

                if (errorObject.startTimeGreater) {
                    return 'Finish time must be after start time';
                }

                if (errorObject.maxlength) {
                    return 'Maximum field length exceeded';
                }
                
                if (errorObject.minlength) {
                    if (scope.controlRuleValue) {
                        return 'Must be longer than ' + scope.controlRuleValue + ' characters.';
                    }
                        
                    return 'Minimum length not met';        

                }

                if (errorObject.passwordsDoNotMatch) {
                    return 'Passwords must match';
                }

                if (errorObject.min) {
                    return 'Value is too small.';
                }

                if (angular.isFunction(scope.customMessage)) {
                    var message = scope.customMessage({ error: errorObject });

                    if (typeof message === 'string') {
                        return message;
                    }
                }

                return null;
            };
        },
    };
}]);
