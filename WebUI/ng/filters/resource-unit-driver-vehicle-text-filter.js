'use strict';

angular.module('peApp').filter('resourceUnitDriverVehicleText', ['ListFilterSummary', '$filter', function (ListFilterSummary, $filter) {
    return function (vehicles, resourceUnits, driverQuery, vehicleQuery, vehicleTypesQuery) {

        if (!vehicles || !resourceUnits)
            return null;

        // If no vehicle type is unchecked then there is no need to filter on vehicle type
        var filterOnVehicleType = _.some(vehicleTypesQuery, function (value) { return value === false });

        if (driverQuery == '' && vehicleQuery == '' && !filterOnVehicleType)
            return vehicles;

        /// === variables === ///
        var isMatch = function(item, query){
            if (!item || !query) return false;
            return item.toLowerCase().indexOf(query.toLowerCase()) > -1;
        }

        /// === filtering === ///
        var filteredVehicles = $filter('filter')(vehicles, { label: vehicleQuery });

        if (filterOnVehicleType) {
            var vehicleTypes = _.map(vehicleTypesQuery, function (value, key) { if (value === true) { return parseInt(key); } });
            filteredVehicles = filteredVehicles.filter(function (vehicle) {
                return vehicleTypes.indexOf(vehicle.vehicleTypeID) > -1;
            });
        }

        if (driverQuery != '')
            filteredVehicles = filteredVehicles.filter(function (vehicle) {
                var resourceUnitsForThisVehicle = resourceUnits.filter(function (resourceUnit) {
                    return resourceUnit.vehicle.resourceID.toString() === vehicle.key;
                });

                var filteredResourceUnits = resourceUnitsForThisVehicle.filter(function (resourceUnit) {
                    return isMatch(resourceUnit.driver.fullName, driverQuery);
                });

                return filteredResourceUnits.length !== 0
            });

        return filteredVehicles;

    }
}]);