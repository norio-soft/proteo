'use strict';

angular.module('peApp').directive('ngReallyClick', ['$modal',
    function ($modal) {

        var ModalInstanceCtrl = ['$scope', '$modalInstance', function ($scope, $modalInstance) {
            $scope.ok = function () {
                $modalInstance.close();
            };

            $scope.cancel = function () {
                $modalInstance.dismiss('cancel');
            };
        }];

        return {
            restrict: 'A',
            scope: {
                ngReallyClick: "&",
                item: "="
            },
            link: function (scope, element, attrs) {
                element.bind('click', function () {
                    var message = attrs.ngReallyMessage || "Are you sure ?";
                    var okMessage = attrs.ngReallyOk || "Ok";
                    var cancelMessage = attrs.ngReallyCancel || "Cancel";

                    var modalHtml = '<div class="modal-body">' + message + '</div>';
                    modalHtml += '<div class="modal-footer"><button class="btn btn-primary" ng-click="ok()">' + okMessage + '</button><button class="btn btn-warning" ng-click="cancel()">' + cancelMessage + '</button></div>';

                    var modalInstance = $modal.open({
                        template: modalHtml,
                        controller: ModalInstanceCtrl
                    });

                    modalInstance.result.then(function () {
                        scope.ngReallyClick({ item: scope.item }); //raise an error : $digest already in progress
                    }, function () {
                        //Modal dismissed
                    });
                    //*/

                });

            }
        }
    }
]);

