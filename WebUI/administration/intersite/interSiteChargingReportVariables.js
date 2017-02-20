'use strict';

var app = angular.module('interSiteReportVariablesApp',
    [
        'proteo.shared.api',
        'proteo.shared.forms'        
    ]);

app.config([
    '$httpProvider', 'ngToastProvider', function ($httpProvider, ngToastProvider) {
        $httpProvider.interceptors.push('authInterceptorService');
        ngToastProvider.configure({
            verticalPosition: 'top',
            horizontalPosition: 'center',
            maxNumber: 3,
            animation: 'slide',
        });
    }

]);

app.controller('InterSiteReportVariablesCtrl', ['$scope', 'apiService', 'ngToast',
    function ($scope, apiService, ngToast) {
        $scope.futureValidFromOpened = false;
        $scope.futureControlAreaValidFromOpened = false;
        $scope.showInterSiteCosts = false;
        $scope.tommorrow = moment().add(1, 'days').toDate();

        var getInterSiteCharges = function () {
            apiService.interSite.getInterSiteCharge().$promise.then(
                function (result) {
                    $scope.currentInterSiteCharge = result.currentInterSiteCharge;
                    if (!$scope.currentInterSiteCharge) {
                        $scope.currentInterSiteCharge = {};
                        $scope.currentInterSiteCharge.miles1 = 0;
                        $scope.currentInterSiteCharge.miles2 = 0;
                        $scope.currentInterSiteCharge.miles3 = 0;
                        $scope.currentInterSiteCharge.miles4 = 0;
                        $scope.currentInterSiteCharge.miles5 = 0;
                        $scope.currentInterSiteCharge.miles6 = 0;
                    }
                    $scope.currentInterSiteCharge.validFrom = moment().format('DD/MM/YY');
                    $scope.futureInterSiteCharge = result.futureInterSiteCharge;
                });
        };


        var getControlAreas = function () {
            apiService.controlArea.get().$promise.then(
                function (result) {
                    $scope.controlAreas = result;
                }
                );
        };

        var getControlAreaInterSiteCosts = function (controlAreaId) {
            apiService.interSite.getControlAreaInterSiteCosts({controlAreaId : controlAreaId}).$promise.then(
                function (result) {
                    $scope.currentControlAreaInterSiteCost = result.currentControlAreaInterSiteCost;
                    if (!$scope.currentControlAreaInterSiteCost) {
                        $scope.currentControlAreaInterSiteCost = {};
                        $scope.currentControlAreaInterSiteCost.driverCostPerHour = 0;
                        $scope.currentControlAreaInterSiteCost.unitCostPerHour = 0;
                        $scope.currentControlAreaInterSiteCost.trailerCostPerHour = 0;
                        $scope.currentControlAreaInterSiteCost.fuelCostPerMile = 0;
                    }
                    $scope.currentControlAreaInterSiteCost.validFrom = moment().format('DD/MM/YY');
                    $scope.futureControlAreaInterSiteCost = result.futureControlAreaInterSiteCost;
                    $scope.showInterSiteCosts = true;
                })
        }

        $scope.openFutureValidFrom = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.futureValidFromOpened = !$scope.futureValidFromOpened;
        };

        $scope.openFutureControlAreaValidFrom = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.futureControlAreaValidFromOpened = !$scope.futureControlAreaValidFromOpened;
        }

        $scope.addCurrentInterSiteCharge = function () {

            $scope.$broadcast('show-errors-check-validity');

            if ($scope.currentInterSiteForm.$invalid)
                return;

            $scope.currentInterSiteCharge.validFrom = moment().toDate();
            apiService.interSite.addCurrentInterSiteCharge($scope.currentInterSiteCharge).$promise.then(
                function (result) {
                    $scope.currentInterSiteCharge = result;
                    $scope.currentInterSiteCharge.validFrom = moment().format('DD/MM/YY');
                    ngToast.success('Inter-site charges have been added');
                }
            )
        };

        $scope.addCurrentControlAreaInterSiteCost = function () {
            $scope.$broadcast('show-errors-check-validity');

            if ($scope.currentControlAreaForm.$invalid)
                return;

            $scope.currentControlAreaInterSiteCost.validFrom = moment().toDate();
            $scope.currentControlAreaInterSiteCost.controlAreaId = $scope.selectedControlArea.controlAreaId;
            apiService.interSite.addCurrentControlAreaInterSiteCost($scope.currentControlAreaInterSiteCost).$promise.then(
                function (result) {
                    $scope.currentControlAreaInterSiteCost = result;
                    $scope.currentControlAreaInterSiteCost.validFrom = moment().format('DD/MM/YY');
                    ngToast.success('Inter-site cost has been added');
                }
            )
        }

        $scope.addFutureInterSiteCharge = function () {

            $scope.$broadcast('show-errors-check-validity');

            if ($scope.futureInterSiteForm.$invalid)
                return;

            apiService.interSite.addFutureInterSiteCharge($scope.futureInterSiteCharge).$promise.then(
                function (result) {
                    $scope.futureInterSiteCharge = result;
                    ngToast.success('Inter-site charges have been updated');
                }
            )
        };

        $scope.addFutureControlAreaInterSiteCost = function () {

            $scope.$broadcast('show-errors-check-validity');

            if ($scope.futureControlAreaInterSiteCost.$invalid)
                return;

            $scope.futureControlAreaInterSiteCost.controlAreaId = $scope.selectedControlArea.controlAreaId;
            apiService.interSite.addFutureControlAreaInterSiteCost($scope.futureControlAreaInterSiteCost).$promise.then(
                function (result) {
                    $scope.futureControlAreaInterSiteCost = result;
                    ngToast.success('Inter-site costs have been updated');
                }
            )
        }

        $scope.populateFutureControlAreaIntersiteCost = function () {
                $scope.futureControlAreaInterSiteCost = {};
                $scope.futureControlAreaInterSiteCost.driverCostPerHour = 0;
                $scope.futureControlAreaInterSiteCost.unitCostPerHour = 0;
                $scope.futureControlAreaInterSiteCost.trailerCostPerHour = 0;
                $scope.futureControlAreaInterSiteCost.fuelCostPerMile = 0;
        }

        $scope.populateFutureInterSiteCharge = function () {
            $scope.futureInterSiteCharge = {};
            $scope.futureInterSiteCharge.miles1 = 0;
            $scope.futureInterSiteCharge.miles2 = 0;
            $scope.futureInterSiteCharge.miles3 = 0;
            $scope.futureInterSiteCharge.miles4 = 0;
            $scope.futureInterSiteCharge.miles5 = 0;
            $scope.futureInterSiteCharge.miles6 = 0;
        }

        $scope.filterChanged = function () {
            getControlAreaInterSiteCosts($scope.selectedControlArea.controlAreaId);
        }

        getControlAreas();
        getInterSiteCharges();
    }]);