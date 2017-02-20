'use strict';

angular.module('peApp').filter('resourceUnitVehicleType', ['ListFilterSummary', function (ListFilterSummary) {
    return function (items, filterSettings) {

        if (!items || !filterSettings) {
            return null;
        }

        /// === variables === ///
        var vehicleTypeSummary = new ListFilterSummary(filterSettings.vehicleType);

        var isMatch = function (item) {

            if (vehicleTypeSummary.shouldCheck) {
                if (!_.contains(vehicleTypeSummary.toCheck, item.vehicleType)) return false;
            }


            return true;

        };

        /// === filtering === ///
        return _.filter(items, isMatch);

    }
}]);