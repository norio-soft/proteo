'use strict';

angular.module('peApp').directive('ngTransform', function () {
        return function (scope, element, attrs) {
            scope.$watch(attrs.ngTransform, function (value) {
                element.attr('transform', value);
            });
        };
    })