'use strict';

angular.module('peApp').directive('prHereMapPolylines', [function () {

    return {

        controller: function ($scope) {
          
        },
        template: '<div ng-transclude></div>',
        restrict: 'EA',
        transclude: true,
        scope: {
            defaultOptions: '=?',
            polylines: '=',
            visible: '=',
        },
        require: '^prHereMap',
        link: function (scope, element, attrs, mapCtrl) {

            var mapGroup = new H.map.Group();


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

            function getColorString(color) {
                return 'rgba(' + color.red + ', ' + color.green + ', ' + color.blue + ', ' + color.a + ')';
            }

            function updatePolylines() {
                if (scope.polylines === undefined) {
                    return;
                }

                mapGroup.removeAll();

                // check for polylines that have been added
                scope.polylines.forEach(function (polyline) {
                    var strip = new H.geo.Strip();

                    polyline.points.forEach(function (point) {
                        strip.pushLatLngAlt(point.lat, point.lng);
                    });

                    var options = mergeOptions(polyline.options, scope.defaultOptions);
                    var strokeColor = getColorString(options.strokeColorARGB);

                    var newPolyline = new H.map.Polyline(strip, {
                        data: { polylineId: polyline.id },
                        style: {
                            lineWidth: options.strokeThickness,
                            strokeColor: strokeColor,
                        },
                        zIndex: options.zIndex || 0,
                    });

                    mapGroup.addObject(newPolyline);
                });
            }


            // ============ Watches ===============

            scope.$watch('polylines', updatePolylines, true);

            scope.$watch('visible', function (newValue) {
                if (newValue !== undefined) {
                    mapGroup.setVisibility(newValue);
                }
            });

            scope.$on('$destroy', function () {
                mapGroup.clear();
                mapCtrl.map.entities.remove(mapGroup);
            });


            // ============ Initialization ===============

            mapGroup.setVisibility(false);
            mapCtrl.map.addObject(mapGroup);
            updatePolylines();

        }
    };

}]);


