'use strict';

angular.module('peApp').directive('prInstructionPin', [function () {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'html-partials/directives/instruction-pin-template.html',
        scope: {
            pin: '=',
            mapOptions: '=',
        },
        link: function (scope, element) {

            var closing = false;
            var displayElement = {};

            if (!scope.pin) {
                return;
            }

            // ========= Helper functions =========

            var keyEventColours = {
                'currentPosition': '#000000',
                'ignitionOff': '#e55824',
            };

            var keyEventZIndex = {
                'currentPosition': 10,
                'ignitionOff': 8,
            };

            var keyEventTemplates = {
                'currentPosition': 'html-partials/directives/start-svg.html',
                'ignitionOff': 'html-partials/directives/ignition-svg.html',
            };

            function setStyles() {
                if (scope.isKeyEvent) {
                    scope.svgStyle = {
                        'fill': keyEventColours[scope.pin.keyEventType],
                        'stroke': keyEventColours[scope.pin.keyEventType],
                    };

                    scope.zIndexStyle = {
                        'z-index': keyEventZIndex[scope.pin.keyEventType],
                        'position': 'absolute',
                        'top' : '-31px'
                    };

                    scope.svgTemplate = keyEventTemplates[scope.pin.keyEventType];
                }
                else {
                    scope.svgStyle = {
                        'fill': scope.pin.colour,
                        'stroke': scope.pin.borderColour,
                        'stroke-width': 1,
                    };

                    scope.zIndexStyle = {
                        'z-index': '5',
                        'position': 'relative',
                        'left': '-' + scope.pin.options.anchor.x + 'px',
                        'top': '-' + scope.pin.options.anchor.y + 'px',
                    };

                    scope.cssStyle = {
                        'background-color': scope.pin.fillColour,
                        'border-color': scope.pin.borderColour,
                    };
                }
            }

            // closing popup programatically can only be triggering the popovers trigger
            // event i.e. clicking on the pin header
            var closePopup = function () {
                if (isPopupOpen()) {
                    _.defer(function () {
                        closing = true;
                        displayElement.trigger('click');
                        closing = false;
                    });
                }
            };

            var isPopupOpen = function () {
                return element.find('.popover').length > 0;
            };

            //========= Scope Functions ===========

            scope.closePopup = closePopup;

            scope.click = function () {
                if (!closing) {
                    // only fire the pinSelected event if the pin header is actully being clicked by the user
                    // not because it's fired to close the popup
                    scope.$emit('pinSelectedEmitUp', { pin: scope.pin, deselect: closePopup });
                }
            };

            scope.dblClick = function () {
                event.stopPropagation();
            };


            // ========= Initialization =========

            scope.pin.options = { width: 29, height: 29, anchor: { x: 10, y: 20 } };
            scope.isKeyEvent = scope.pin.keyEventType !== undefined;
            scope.templateUrl = 'instruction-popup-template.html';
            scope.popupZIndexStyle = { 'z-index': '11', 'position': 'relative' };
            displayElement = element.find('.instruction-pin-display');
            setStyles();

        }
    };
}]);



