'use strict';

angular.module('peApp').filter('resourceUnitDayToggle', ['ListFilterSummary', function (ListFilterSummary) {
    return function (vehicles, resourceUnits, days) {

        if (!vehicles || !resourceUnits || !days)
            return null;


        //If no days are ticked then no need to filter 
        if (days.length == 0)
            return vehicles;

        /// === variables === ///

        var isOverLapping = function (day, resourceUnit) {
            var dayStart = moment(day);
            var dayFinish = moment(day).add(1, 'days');

            return !((resourceUnit.startDateTime >= dayFinish) || (resourceUnit.finishDateTime < dayStart))
                
        };

        /// === filtering === ///

        return _.filter(vehicles, function (vehicle) {
            var resourceForThisVehicle = _.filter(resourceUnits, function (resourceUnit) {
                return resourceUnit.vehicle.resourceID == vehicle.key;
            });

            var allDaysWithoutResources = _.every(days, function (day) {
                var hasNoOverlaps = !(_.some(resourceForThisVehicle, function (resourceUnit) {
                    return isOverLapping(day, resourceUnit);
                }));
                return hasNoOverlaps;
            });
            return allDaysWithoutResources;
        });

    }
}]);