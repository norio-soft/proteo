'use strict';

angular.module('peApp').directive('prVehicleHistoryPin', [ function () {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'html-partials/directives/vehicle-history-pin-template.html',
        scope: {
            pin: '=',
            mapOptions: '='
        },
        link: function (scope, element, attrs) {

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

            var keyEventColours = {
                'start': scope.pin.colour,
                'finish': scope.pin.colour,
                'ignitionOn': '#6bc460',
                'ignitionOff': '#e55824'
            }

            var keyEventZIndex = {
                'start': 9,
                'finish': 10,
                'ignitionOn': 7,
                'ignitionOff':8
            }

            var keyEventTemplates= {
                'start': 'html-partials/directives/start-svg.html',
                'finish': 'html-partials/directives/finish-svg.html',
                'ignitionOn': 'html-partials/directives/ignition-svg.html',
                'ignitionOff': 'html-partials/directives/ignition-svg.html'
            }

            function capitalizeFirstLetter(string) {
                return string.charAt(0).toUpperCase() + string.slice(1);
            }

            function setStyles() {
                if (scope.isKeyEvent) {
                    scope.svgStyle = {
                        'fill': keyEventColours[scope.pin.keyEventType],
                        'stroke': keyEventColours[scope.pin.keyEventType]
                    }
                    scope.zIndexStyle = {
                        'z-index': (scope.pin.isSelected) ? '15' : keyEventZIndex[scope.pin.keyEventType],
                        'position': 'absolute',
                        'left': '-' + scope.pin.options.anchor.x + 'px',
                        'top': '-' + scope.pin.options.anchor.y + 'px',
                    }

                    scope.svgTemplate = keyEventTemplates[scope.pin.keyEventType];
                }
                else {
                    scope.svgStyle = {
                        'fill': scope.pin.colour,
                        'stroke': (scope.pin.isSelected) ? '#070707' : scope.pin.borderColour,
                        'stroke-width': 1
                    };

                    scope.zIndexStyle = {
                        'z-index': (scope.pin.isSelected) ? '15' : '5',
                        'position': 'absolute',
                        'left': '-' + scope.pin.options.anchor.x + 'px',
                        'top': '-' + scope.pin.options.anchor.y + 'px',
                    }

                }
            }

            function setDirection() {
                if (scope.pin.direction) {
                    if (rotateMappings[scope.pin.direction]) {
                        scope.transform = 'rotate(' + rotateMappings[scope.pin.direction] + ' 15 15) translate(0,12)';
                    }             
                }
            }

            function setDriverName() {
                var names = $.grep([scope.pin.firstNames, scope.pin.lastName], Boolean).join(" ");
                scope.driverName = names || '?';
            }

            function setOptions() {

                if (scope.pin.keyEventType === 'start' || scope.pin.keyEventType === 'finish') {
                    scope.pin.options = { height: 30, width: 30, anchor: { x: 17.5, y: 35 } };               
                    scope.pin.reason = capitalizeFirstLetter(scope.pin.keyEventType) + ' (' + scope.pin.reason + ')';

                }
                else {
                    scope.pin.options = { height: 30, width: 30, anchor: { x: 15, y: 15 } };
                }

            }

            var closePopup = function () {
                if (isPopupOpen()) {
                    closing = true;
                    // call customized hide function exposed by pr-tooltip
                    scope.hideTooltip();
                    closing = false;
                }
            }

            var isPopupOpen = function () {
                return scope.$$childHead.isOpen;
            }

            //========= Scope Functions ===========

            scope.closePopup = closePopup;

            scope.click = function () {
                if (!closing) {
                    // only fire the pinSelected event if the vehicle header is actully being clicked by the user
                    // not because it's fired to close the popup
                    scope.pin.isSelected = true;
                }
            }

            scope.dblClick = function () {
                event.stopPropagation();
            }         

            //========= Watches ===========

            scope.$watch('pin.isSelected', function () {

                if (!scope.pin.isSelected) {
                    closePopup();
                }
                else {
                    scope.$emit('pinSelectedEmitUp', scope.pin);
                }
                setStyles();
            });

            // ========= Initialization =========
            
            scope.transform = 'translate(0,12)';
            scope.isKeyEvent = scope.pin.keyEventType !== undefined; 
            scope.templateUrl = 'html-partials/directives/vehicle-popup-template.html';
            setOptions();
            setStyles();
            setDirection();
            setDriverName();
            

        }
    };
}]);



