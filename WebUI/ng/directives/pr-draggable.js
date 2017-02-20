'use strict';

angular.module('peApp').directive('prDraggable', ['$parse', '$rootScope', function ($parse, $rootScope) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs, ctrl) {

            // set the draggable attr
            element.attr("draggable", false);
            attrs.$observe("prDraggable", function (newValue) {
                element.attr("draggable", newValue);
            });

            // change cursor to hand pointer when mousing over
            element.css('cursor', 'pointer');

            // listen for changes to the drag data
            var dragData = "";
            scope.$watch(attrs.dragData, function (newValue) {
                dragData = newValue;
            });

            // drag start event handler
            element.bind("dragstart", function (e) {
                var sendObj = {data: dragData, dropType: attrs.dragType || "default" }
                var sendData = angular.toJson(sendObj);
 
                e.dataTransfer.setData("text", sendData);

            });

            // drag end event handler
            element.bind("dragend", function (e) {
                var sendType= attrs.dragType || "default";

                if (e.dataTransfer && e.dataTransfer.dropEffect !== "none") {
                    if (attrs.onDropSuccess) {
                        var fn = $parse(attrs.onDropSuccess);
                        scope.$apply(function () {
                            fn(scope, { $event: e });
                        });
                    }
                }
            });

        }
    };
}]);