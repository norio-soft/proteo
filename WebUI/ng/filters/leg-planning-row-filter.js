'use strict';

angular.module('peApp').filter('legPlanningRow', ['ListFilterSummary', function (ListFilterSummary) {
    return function (items, filterSettings, filterOnLegs) {
        if (!items || !filterSettings) {
            return null;
        }

        /// === variables === ///
        var driverTypeSummary = new ListFilterSummary(filterSettings.driverType);
        var todayFinishTrafficAreaSummary = new ListFilterSummary(filterSettings.todayFinishTrafficArea);

        /// === functions === ///
        var contains = function (str1, str2) {
            if (!str1 || !str2) return false;
            return str1.toLowerCase().indexOf(str2.toLowerCase()) > -1;
        }

        var isMatch = function (item) {

            

            if (filterSettings.driverName) {
                if (!contains(item.driver.fullName, filterSettings.driverName)) return false;
            }

            if (filterSettings.todayFinishTown) {
                if (!contains(item.driver.todayFinishTown, filterSettings.todayFinishTown)) return false;
            }

            if (filterSettings.vehicleRegistration && item.vehicle) {
                if (!contains(item.vehicle.registrationNumber, filterSettings.vehicleRegistration)) return false;
            }

            if (driverTypeSummary.shouldCheck) {
                if (!_.contains(driverTypeSummary.toCheck, item.driver.driverType)) return false;
            }

            if (todayFinishTrafficAreaSummary.shouldCheck) {
                if (!_.contains(todayFinishTrafficAreaSummary.toCheck, item.driver.todayFinishTrafficArea)) return false;
            }

            if (filterOnLegs !== null) {
                var rowHasLeg = filterOnLegs.some(function (schedulerEvent) {
                    return schedulerEvent.leg !== undefined && item.resourceUnits.some(function (resourceUnit) {
                        return schedulerEvent.y_data_key === resourceUnit.key
                    });
                });

                if (!rowHasLeg) {
                    return false;
                }
            }

            return true;
        };

        /// === filtering === ///
        return _.filter(items, isMatch);
    };
}]);