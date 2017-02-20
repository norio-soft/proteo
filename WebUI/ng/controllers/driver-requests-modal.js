'use strict';

angular.module('peApp').controller('viewDriverRequestsModalCtrl', ['$scope', '$modalInstance','driver','ngTableParams','apiService',
    function ($scope, $modalInstance, driver, ngTableParams, apiService) {
        
        //// VARIABLES ////
        $scope.driverRequests = {};
        $scope.driver = '';
        $scope.tableParams = '';

        $scope.newDriverRequest = {
            driverRequestID: 0,
            resourceID: 0,
            requestText: '',
            appliesUntil: new Date(),
        };

        $scope.createDriverRequestActive = false;

        //// FUNCTIONS ////
        $scope.close = function () {
            $modalInstance.close($scope.driverRequests.length > 0);
        };

        $scope.toggleCreateDriverRequest = function () {
            $scope.createDriverRequestActive = !$scope.createDriverRequestActive;
        };

        $scope.createDriverRequest = function () {
           
            $scope.$broadcast('show-errors-check-validity');

            if ($scope.editDriverRequestsForm.$invalid) {
                return;
            }

            // Do Save
            apiService.driverRequests.create($scope.newDriverRequest)
            .$promise.then(function (response) {
                $scope.getDriverRequests();
                $scope.toggleCreateDriverRequest();
            });
        };

        $scope.updateDriverRequest = function (driverRequest) {
            $scope.$broadcast('show-errors-check-validity');

            if (!driverRequest.requestText) {
                driverRequest.$edit = true;
                return;
            }
            
            if (!driverRequest.appliesUntil || !moment(driverRequest.appliesUntil).isValid()) {
                driverRequest.$edit = true;
                return;
            }

            apiService.driverRequests.update(driverRequest);
        };

        $scope.deleteDriverRequest = function (driverRequestID) {

            var driverRequestPromise = apiService.driverRequests.delete({ driverRequestID: driverRequestID }).$promise;
            driverRequestPromise.then(function (results) {
                $scope.getDriverRequests();
            });
        };

        $scope.getDriverRequests = function () {
            var driverRequestPromise = apiService.driverRequests.getDriverRequests().$promise;
            driverRequestPromise.then(function (results) {

                $scope.driverRequests = results;

                for (var i = $scope.driverRequests.length - 1; i >= 0 ; i--) {
                    if ($scope.driver.resourceID == $scope.driverRequests[i].resourceID) {
                        $scope.driverRequests[i].appliesUntil = new Date($scope.driverRequests[i].appliesUntil);
                    }
                    else {
                        $scope.driverRequests.splice(i, 1);
                    }
                }

                //Enable edit mode
                if (results.length == 0)
                    $scope.createDriverRequestActive = true;

            });
        };

        // Date pickers
        $scope.openAppliesUntilDate = function ($event, driverRequest) {
            $event.preventDefault();
            $event.stopPropagation();
            
            driverRequest.isDatePickerOpened = true;
        };

        $scope.openNewAppliesUntilDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isCreateDatePickerOpened = true;
        };

        //// PAGE SETUP ////
        $scope.driver = driver;
        $scope.newDriverRequest.resourceID = $scope.driver.resourceID;
        $scope.getDriverRequests();

        $scope.tableParams = new ngTableParams({
            count: $scope.driverRequests.length // hides pager
        }, {
            counts: [] // hides page sizes
        });
}]);