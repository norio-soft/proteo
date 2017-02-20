'use strict';

angular.module('peApp').directive('prSpin', function () {
    var augmentOpts = function (color, opts) {
        if (!opts.color) {
            opts.color = color;
        }
    };

    return {
        restrict: 'E',
        transclude: true,
        replace: true,
        template: '<div ng-transclude style="position: relative;"></div>',
        scope: {
            config: '=',
            spinIf: '='
        },
        link: function (scope, element, attrs) {
            var cssColor = element.css('color'),
                stopped = false,
                hideElement = !!scope.config.hideElement,
                spinner;

            augmentOpts(cssColor, scope.config);
            spinner = new Spinner(scope.config);
            element.height(spinner.opts.radius + (2 * spinner.opts.length) + 2);
            spinner.spin(element[0]);

            scope.$watch('config', function (newValue, oldValue) {
                if (newValue === oldValue) {
                    return;
                }

                spinner.stop();
                hideElement = !!newValue.config.hideElement;
                spinner = new Spinner(newValue);

                if (!stopped) {
                    spinner.spin(element[0]);
                }
            }, true);

            if (attrs.hasOwnProperty('spinIf')) {
                scope.$watch('spinIf', function (newValue) {
                    if (newValue) {
                        spinner.spin(element[0]);

                        if (hideElement) {
                            element.css('display', '');
                        }

                        stopped = false
                    } else {
                        spinner.stop();

                        if (hideElement) {
                            element.css('display', 'none');
                        }

                        stopped = true
                    }
                });
            }

            scope.$on('$destroy', function() {
                spinner.stop();
            });
        }
    }
});