'use strict';

angular.module('peApp').filter('resourceUnitDriverType', ['ListFilterSummary', function (ListFilterSummary) {
    return function (items, filterSettings) {

        if (!items || !filterSettings) {
            return null;
        }        

        /// === variables === ///
        var driverTypeSummary = new ListFilterSummary(filterSettings.driverType);

        var isMatch = function (item) {

            if (!filterSettings.includeAllDrivers)
                if (item.plannerIdentityID != filterSettings.plannerId)
                    return false;

            if (driverTypeSummary.shouldCheck) {
                if (!_.contains(driverTypeSummary.toCheck, item.driverType)) return false;
            }


            return true;

        };

        /// === filtering === ///
        return _.filter(items, isMatch);

    }
}]);