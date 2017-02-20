
'use strict';

angular.module('peApp').directive('prHereMap', ['$compile', '$timeout', function ($compile, $timeout) {

    return {
        template: '<div id="transcluded">' +
                    '</div>',
        restrict: 'EA',
        priority: 10,
        transclude: true,
        scope: {
            credentials: '=',
            mapOptions: '=?',
            resize: '=', // used to trigger redraw - the value is arbitrary, but any change to the value will result in a call to the Here Maps viewport resize function
            testState: '=',
            animate: '@'
        },
        controller: ['$scope', '$element', function ($scope, $element) {
            // ============== initialisation ==================
            // DO initialisation in controller so children of controller have access to map
            this.platform = new H.service.Platform({
                app_id: $scope.credentials.app_id,
                app_code: $scope.credentials.app_code
            })

            this.mapTypes = this.platform.createDefaultLayers();
            this.defaultLatLng = new H.geo.Point($scope.mapOptions.center.latitude, $scope.mapOptions.center.longitude);

            this.map = new H.Map($element[0], this.mapTypes.normal.map, {
                center: this.defaultLatLng,
                zoom: $scope.mapOptions.zoom
            });

            this.behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(this.map));
            this.ui = H.ui.UI.createDefault(this.map, this.mapTypes);
            this.ui.getControl('mapsettings').setAlignment('top-left');

            this.mapOptions = $scope.mapOptions;
            $scope.map = this.map;

        }],
        compile: function (tElem, tAttrs) {

            return function link(scope, element, attrs, mapCtrl, transclude) {

                // =============== transclude =====================

                var contextMenuPinScope;
                var contextMenuPinElement;
                var transcludedSection;
                var animate = scope.animate !== 'false';

                transclude(function injectLinkedClone(clone) {
                    // Grab the context menu template and wrap it up as a push-pin directive we'll compile and link later
                    var contextMenuInnerTemplate = clone.filter('#context-menu-template');
                    contextMenuInnerTemplate.detach();
                    var contextMenu = contextMenuInnerTemplate.wrap('<div id="pin-template" class="context-menu" ng-mouseout="pin.closeContextMenu($event)" style="position: absolute; left: -60px; top: -15px; height: 30px; width: 120px;"></div>').parent();
                    contextMenuPinElement = contextMenu.wrap('<pr-here-map-push-pins pins="contextMenus" collection-id="contextMenus" live-update="true" default-options="contextMenuOptions"></pr-here-map-push-pins>').parent();
                    // transclude everything else as usual
                    var otherPushPins = clone.not('#context-menu-template');
                    transcludedSection = element.find('#transcluded');
                    transcludedSection.append(otherPushPins);
                });

                // ============== functions ======================

                function safeDigest() {
                    if (!scope.$$phase && !scope.$root.$$phase) {
                        scope.$digest();
                    }
                    else {
                        _.defer(function () {
                            scope.$digest();
                        });
                    }
                }

                function closeContextMenu($event) {
                    if (!$event || !$.contains(event.currentTarget, $event.toElement)) {
                        contextMenuPinScope.contextMenus = [];
                    }
                }

                function rightClickHandler(e) {

                    var loc = scope.map.screenToGeo(e.viewportX, e.viewportY);
                    var contextMenu = { id: 1, lat: loc.lat, lng: loc.lng, closeContextMenu: closeContextMenu };

                    contextMenuPinScope.contextMenus = [];
                    contextMenuPinScope.contextMenus = [contextMenu];
                    contextMenuPinScope.$digest();
                }

                function mapViewChangeHandler() {
                    scope.mapOptions.zoom = scope.map.getZoom();
                    safeDigest();
                };

                // ============== maps event handlers =============

                scope.map.addEventListener('mapviewchangeend', mapViewChangeHandler);

                scope.map.addEventListener('contextmenu', rightClickHandler);

                // ============== watches ==========================

                scope.$watch('resize', function () {

                    $timeout(function () {
                        var viewport = scope.map.getViewPort();
                        viewport.resize();
                    });

                }, true);

                scope.$watch('mapOptions.center', function (center) {
                    var newCenter = new H.geo.Point(center.latitude, center.longitude)
                    scope.map.setCenter(newCenter, animate);
                });

                scope.$watch('mapOptions.zoom', function (zoom) {
                    scope.map.setZoom(zoom, animate);
                });

                scope.$watchCollection('mapOptions.viewCoordinates', function (viewCoordinates) {
                    if (viewCoordinates && viewCoordinates.length > 0) {
                        var locations = viewCoordinates.map(function (coordinate) {
                            return new H.geo.Point(coordinate.lat, coordinate.lng);
                        });

                        if (locations.length === 1) {

                            scope.map.setCenter(locations[0], animate);

                        }
                        else {
                            var boundingRect = H.geo.Rect.coverPoints(locations);
                            scope.map.setViewBounds(boundingRect, animate);
                        }

                        // Reset to an empty array, so the zoom can be triggered again by setting the same co-ordinates if desired.
                        scope.mapOptions.viewCoordinates = [];
                    }
                });


                scope.$on('$destroy', function () {

                    mapCtrl.map.dispose();
                    if (contextMenuPinScope) {
                        contextMenuPinScope.$destroy();
                    }
                });

                // ============== initialization ==========================

                // build the context menu pins
                if (contextMenuPinElement) {
                    contextMenuPinScope = scope.$new();
                    contextMenuPinScope.contextMenus = [];
                    contextMenuPinScope.contextMenuOptions = { height: 30, width: 120, anchor: { x: 60, y: 15 } };
                    transcludedSection.append(contextMenuPinElement);
                    //need to reinject the controller on the element for the "require" tag to work on the lower level directives
                    contextMenuPinElement.data('$prHereMapController', element.data('$prHereMapController'));
                    $compile(contextMenuPinElement)(contextMenuPinScope);
                    safeDigest();
                }

                if (scope.testState) {
                    scope.testState.map = mapCtrl.map;
                }
            }
        }
    }

}]);

