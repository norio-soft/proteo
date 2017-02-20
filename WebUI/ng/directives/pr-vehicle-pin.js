'use strict';

angular.module('peApp').directive('prVehiclePin', ['legacyAppWindowService', function (legacyAppWindowService) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'html-partials/directives/vehicle-pin-template.html',
        scope: {
            pin: '=',
            mapOptions: '=',
        },
        link: function (scope, element, attrs) {

            var vehicleHeader;
            var closing = false;

            var rotateMappings = {
                S: 0,
                SW: 45,
                W: 90,
                NW: 135,
                N: 180,
                NE: 225,
                E: 270,
                SE: 315
            };

            if (!scope.pin) {
                return;
            }

            // ========= Helper functions =========

            function setStyles() {
                var legendItem = scope.mapOptions.legend.getLegendItem(scope.pin.reasonCode);
                scope.svgStyle = {
                    'fill': legendItem.fillColour,
                    'stroke': (scope.pin.isSelected) ? '#171717' : legendItem.borderColour,
                    'stroke-width': 1
                };
                scope.cssStyle = {
                    'background-color': legendItem.fillColour,
                    'border-color': (scope.pin.isSelected) ? '#171717' : legendItem.borderColour,
                };
                scope.zIndexStyle = {
                    'z-index': (scope.pin.isSelected) ? '15' : '5',
                    'position': 'absolute',
                    'left': '-' + scope.pin.options.anchor.x + 'px',
                    'top': '-' + scope.pin.options.anchor.y + 'px',
                };
            }

            function setDirection() {
                if (scope.pin.direction) {
                    if (rotateMappings[scope.pin.direction]) {
                        scope.transform = 'rotate(' + rotateMappings[scope.pin.direction] + ' 55 12) translate(0,9)';
                    }
                }
            }

            function setDriverName() {
                var names = $.grep([scope.pin.firstNames, scope.pin.lastName], Boolean).join(" ");
                scope.driverName = names || '?';
            }

            var closePopup = function () {
                if (isPopupOpen()) {
                    // call customized hide function exposed by pr-tooltip
                    scope.hideTooltip();
                }
            };

            var isPopupOpen = function () {
                return scope.$$childHead.isOpen;
            };

            //========= Scope Functions ===========

            scope.closePopup = closePopup;

            scope.click = function () {
                if (isPopupOpen()) {
                    // only fire the pinSelected event if the vehicle header is actully being clicked by the user
                    // not because it's fired to close the popup
                    scope.pin.isSelected = true;
                    //scope.pin.isTracked = true;
                }
            };

            scope.dblClick = function () {
                event.stopPropagation();
            };

            scope.showHistory = function () {
                scope.$emit('showVehicleHistory', scope.pin);
                closePopup();
            };

            scope.toggleTracking = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                scope.pin.isTracked = !scope.pin.isTracked;
                scope.$emit('pinTrackedEmitUp', scope.pin);
            };

            scope.viewRun = function () {
                if (scope.pin.currentJobID !== null) {
                    legacyAppWindowService.viewRun(scope.pin.currentJobID);
                }
            };

            // ========= Watches =========

            scope.$watch('pin.direction', function () {
                setDirection();    
            });

            scope.$watch('pin.reasonCode', function () {               
                setStyles();
            });

            scope.$watch('[pin.firstNames, pin.lastName]', function () {
                setDriverName();
            });

            scope.$watch('pin.isSelected', function () {

                if (!scope.pin.isSelected) {
                    closePopup();
                }
                else {
                    scope.$emit('pinSelectedEmitUp', scope.pin);
                }
                setStyles();
            });

            scope.$watch('pin.isTracked', function () {
                 scope.$emit('pinTrackedEmitUp', scope.pin);
            });

            // ========= Initialization =========
            scope.pin.options = { height: 57, width: 110, anchor: { x: 55, y: 32 } };
            scope.transform = 'translate(0,9)';
            scope.templateUrl = 'html-partials/directives/vehicle-popup-template.html';
            vehicleHeader = element.find('.vehicle-pin-header');           
            setStyles();
            setDirection();
            setDriverName();


        }
    };
}]);



