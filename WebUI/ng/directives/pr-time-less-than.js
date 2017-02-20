'use strict';

angular.module('peApp').directive('prTimeLessThan', ['$parse', '$rootScope', function ($parse, $rootScope) {

    function getStartDateTime(startDate, startTime) {
        var justStartDate = moment(startDate).format('DD/MM/YYYY');

        return moment(justStartDate + ' ' + startTime, 'DD/MM/YYYY HH:mm').toDate();
    };

    function getFinishDateTime(finishDate, finishTime) {
        var justFinishDate = moment(finishDate).format('DD/MM/YYYY');

        return moment(justFinishDate + ' ' + finishTime, 'DD/MM/YYYY HH:mm').toDate();
    };

    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {

            var validateTimes = function (viewValue) {
                ctrl.$setValidity('startTimeGreater', true);

                // Setup start and finish dates
                var startDate;
                var finishDate;

                if (attrs.startDate !== undefined) {
                    var parsedStartDate = $parse(attrs.startDate);
                    startDate = new Date(parsedStartDate(scope));
                } else {
                    startDate = moment().toDate();
                }

                if (attrs.finishDate !== undefined) {
                    var parsedFinishDate = $parse(attrs.finishDate);
                    finishDate = new Date(parsedFinishDate(scope));
                } else {
                    finishDate = moment().toDate();
                }

                // Setup start and finish times
                var startTime;
                var finishTime;

                var parsedStartTime = $parse(attrs.prTimeLessThan);
                var parsedStartTimeToDateTime = new Date(parsedStartTime(scope));

                startTime = moment(parsedStartTimeToDateTime).format('HH:mm');


                // Check if we have a date object or the text from the time picker
                var dateTest = Date.parse(viewValue);

                if (!isNaN(dateTest)) {
                    finishTime = moment(viewValue).format('HH:mm');
                }
                else {
                    var formattedFinishDate = moment(finishDate).format('DD/MM/YYYY');
                    finishTime = moment(formattedFinishDate + ' ' + viewValue, 'DD/MM/YYYY hh:mmA').format('HH:mm');
                }

                var startDateTime = getStartDateTime(startDate, startTime);
                var finishDateTime = getFinishDateTime(finishDate, finishTime);

                if (finishDateTime < startDateTime) {
                    ctrl.$setValidity('startTimeGreater', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('startTimeGreater', true);
                    return viewValue;
                }
            }

            ctrl.$parsers.unshift(validateTimes);
            ctrl.$formatters.push(validateTimes);
            scope.$watch(attrs.prTimeLessThan, function () {
                validateTimes(ctrl.$viewValue);
            });

            scope.$watch(attrs.startDate, function () {
                validateTimes(ctrl.$viewValue);
            });

            scope.$watch(attrs.finishDate, function () {
                validateTimes(ctrl.$viewValue);
            });
        }
    };
}]);