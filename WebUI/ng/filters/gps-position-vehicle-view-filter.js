'use strict';

angular.module('peApp').filter('gpsPositionVehicleViewFilter', [function () {
    return function (items, vehicleViewItems) {    

        if (!items) {
            return null;
        }

        if (!vehicleViewItems || vehicleViewItems.length == 0) {
            return items;
        }

        /// === functions === ///      
        var flatten = function(nodes) {
            nodes.forEach(function (node) {
                flattened.push(node);
                if (node.children) {
                    flatten(node.children);
                }
            });
        }


        var isMatch = function (item) {
            return _.find(flattened, function (node) {
                return item.resourceID == node.id;
            }) !== undefined;
        };

        /// === filtering === ///

        // flatten the hierarchy
        var flattened = [];
        flatten(vehicleViewItems);


        return _.filter(items, isMatch);
    };
}]);