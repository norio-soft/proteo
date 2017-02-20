'use strict';

angular.module('peApp').filter('gpsPositionFilter', [function () {
    return function (items, freeText) {
        if (!items) {
            return null;
        }

        if (!freeText) {
            return items;
        }

        /// === functions === ///
        var contains = function (str1, str2) {
            if (!str1 || !str2) return false;
            return str1.toLowerCase().indexOf(str2.toLowerCase()) > -1;
        }

        var isMatch = function (item) {
            return (contains(item.regNo, freeText) ||
                    contains(item.firstNames + ' ' + item.lastName, freeText) ||
                    contains(item.locationString, freeText) ||
                    contains(item.reason, freeText))
        };

        /// === filtering === ///
        return _.filter(items, isMatch);
    };
}]);