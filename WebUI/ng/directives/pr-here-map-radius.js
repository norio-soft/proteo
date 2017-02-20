'use strict';

angular.module('peApp').directive('prHereMapRadius', [function () {

    return {

        controller: function ($scope) {
          
        },
        template: '<div ng-transclude></div>',
        restrict: 'EA',
        transclude: true,
        scope: {
            defaultOptions: '=?',
            radii: '=',
            collectionId: '@'
        },
        require: '^prHereMap',
        link: function (scope, element, attrs, mapCtrl) {
          

            // ============ Helper functions ===============

            function mergeOptions(dstOptions, srcOptions) {
                if (!dstOptions && !srcOptions) {                    
                    return {};
                }

                if (!dstOptions) {
                    return angular.copy(srcOptions);
                }

                if (!dstOptions) {
                    return dstOptions;
                }

                return angular.extend({}, srcOptions, dstOptions);
            }

            function getEntityRadiusArray() {

                var objects = mapCtrl.map.getObjects()
                return  _.filter(objects, function (object) {
                    return (object.entityType == 'radius' && object.collectionId == scope.collectionId) 
                });
            }

            function updateRadii() {
                if (scope.radii === undefined) {
                    return;
                }

                var entityRadii = getEntityRadiusArray();

                // check for radiii that have been deleted
                entityRadii.forEach(function (entityRadius) {

                    var existingRadius = _.findWhere(scope.radii, { id: entityRadius.id });
                    
                    // radius no longer exists in our collection of radii, so delete it
                    if (!existingRadius) {
                        mapCtrl.map.removeObjects([entityRadius]);
                    }
                   
                });

                // check for radii that have been updated or addeed
                scope.radii.forEach(function (radius) {

                    var existingEntityRadius = _.findWhere(entityRadii, { id: radius.id });

                    // corresponding entity radii doesn't exist, so add it
                    if (!existingEntityRadius) {
                        var newRadius = new H.map.Circle(new H.geo.Point(radius.lat, radius.lng), radius.radius * 1609.34);


                        newRadius.id = radius.id;
                        newRadius.entityType = 'radius';
                        newRadius.collectionId = scope.collectionId;
                        
                        mapCtrl.map.addObject(newRadius);
                   }
                   else {
                        existingEntityRadius.setRadius(radius.radius * 1609.34);
                        existingEntityRadius.setCenter(new H.geo.Point(radius.lat, radius.lng));
                   }
                });
            }


            // ============ Watches ===============

            scope.$watch('radii', updateRadii, true);

            scope.$on('$destroy', function () {
                mapCtrl.map.removeObjects(mapCtrl.map.getObjects());
            });

            // ============ Initialization ===============

            updateRadii();

        }
    };

}]);


