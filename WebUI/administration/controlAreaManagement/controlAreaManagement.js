'use strict';

var app = angular.module('controlAreaManagementApp',
    [
        'proteo.shared.api',
        'proteo.shared.forms'
    ]);


app.config([
    '$httpProvider', function ($httpProvider) {     
        $httpProvider.interceptors.push('authInterceptorService');
    }
]);


app.controller('ControlAreaManagementCtrl', ['$scope', '$modal', 'apiService',
    function ($scope, $modal, apiService) {

        //// VARIABLES ////
        $scope.controlAreas = [];
        $scope.customers = [];
        $scope.selectedControlArea = null;
        $scope.selectedCustomer = null;

        //// HELPERFUNCTIONS ////

        var loadCustomers = function (controlAreaId) {
            apiService.organisation.get({ controlAreaId: controlAreaId, organisationTypeId: 2 }).$promise.then(function (result) {
                $scope.customers = result;

                if ($scope.customers.length > 0) {
                    $scope.customers[0].isSelected = true;
                    $scope.selectedCustomer = $scope.customers[0];
                }
                else {
                    $scope.selectedCustomer = null;
                }

            });
        }

        var loadControlAreas = function (controlAreaId) {
            apiService.controlArea.get({ includeCustomerCount: true }).$promise.then(function (result) {
                $scope.controlAreas = result;

                if ($scope.controlAreas.length > 0) {
                   
                    var toSelect = $scope.controlAreas[0];

                    if (controlAreaId) {
                        var toSelect = _.findWhere($scope.controlAreas, { controlAreaId: controlAreaId });
                    }
                    
                    if (toSelect) {
                        toSelect.isSelected = true;
                        $scope.selectedControlArea = toSelect;
                        loadCustomers($scope.selectedControlArea.controlAreaId);
                    }
                    else {
                        $scope.selectedControlArea = null;
                    }
                

                }
                else {
                    $scope.selectedControlArea = null;
                }

            });
        }

        var openControlAreaModal = function (controlArea) {

            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'edit-control-area-modal.html',
                controller: 'EditControlAreaModal',
                backdrop: 'static',
                windowClass: 'modal-kludge',
                size: 'sm',
                resolve: {
                    controlArea: function () {
                        return controlArea ? _.clone(controlArea) : {};
                    }
                }
            });

            modalInstance.result.then(function (result) {
                loadControlAreas();
            });

        }

        //// SCOPE FUNCTIONS ////

        $scope.addCustomer = function () {
            var modalInstance = $modal.open({
                animation: true,
                templateUrl: 'add-customer-modal.html',
                controller: 'AddCustomerModalCtrl',
                backdrop: 'static',
                windowClass: 'modal-kludge',
                resolve: {
                    controlArea: function () {
                        return $scope.selectedControlArea;
                    }
                }
            });

            modalInstance.result.then(function (result) {
                loadControlAreas($scope.selectedControlArea.controlAreaId);
            });
        };


        $scope.addControlArea = function () {
            openControlAreaModal();
        };

        $scope.editControlArea = function () {
            openControlAreaModal($scope.selectedControlArea);
        };

        $scope.selectControlArea = function (controlArea) {

            if ($scope.selectedControlArea) {
                $scope.selectedControlArea.isSelected = false;
            }

            controlArea.isSelected = true;
            $scope.selectedControlArea = controlArea;

            loadCustomers(controlArea.controlAreaId);

        };

        $scope.selectCustomer = function (customer) {

            if ($scope.selectedCustomer) {
                $scope.selectedCustomer.isSelected = false;
            }

            customer.isSelected = true;
            $scope.selectedCustomer = customer;

        };

        $scope.removeCustomer = function () {
            apiService.controlArea.removeCustomer({ controlAreaId: $scope.selectedControlArea.controlAreaId, organisationId: $scope.selectedCustomer.organisationID }, {}).$promise.then(function () {
                loadControlAreas($scope.selectedControlArea.controlAreaId);
            });
        };

        $scope.removeCustomerDisabled = function () {
            return $scope.selectedCustomer === null;
        };

        //// PAGE SETUP ////

        loadControlAreas();
      

        
    }]);