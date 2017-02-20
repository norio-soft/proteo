'use strict';

angular.module('peApp').controller('sendMWFMessageModalCtrl', ['$scope', '$modalInstance', 'driverID', 'jobID', 'apiService',
    function ($scope, $modalInstance, driverID, jobID, apiService) {
        //// VARIABLES ////
        $scope.mwfMessage = {
            driverID: 0,
            messageDateTime: '',
            date: new Date(),
            pointID: '',
            message: '',
            heJobID: '',
        };
        $scope.isTimeOpen = false;
        $scope.pointsLoading = false;
        $scope.pointDescription = '';

        //// FUNCTIONS ////
        $scope.close = function () {
            $modalInstance.close();
        };

        $scope.send = function () {

            $scope.$broadcast('show-errors-check-validity');

            if ($scope.sendMwfMessageForm.$invalid) {
                return;
            }


            var messageDateTime = moment($scope.mwfMessage.date);
            if ($scope.mwfMessage.time) {
                messageDateTime = moment(messageDateTime.format('YYYY-MM-DD') + 'T' + $scope.mwfMessage.time);
            }

            $scope.mwfMessage.messageDateTime = messageDateTime;

            apiService.sendMwfInstruction.create($scope.mwfMessage)
            .$promise.then(function (response) {
                $scope.close();
            });
        };

        // Get points from web API
        $scope.getPoints = function (val) {
            $scope.pointsLoading = true;
            return apiService.points.query({ searchLocal: true, searchTerm: val }).$promise.then(function (results) {
                var pointsArray = [];
                $scope.pointsLoading = false;
                angular.forEach(results, function (item) {
                    pointsArray.push(item);
                });

                return pointsArray;
            });
        };

        $scope.setMWFMessagePoint = function (point) {
            $scope.mwfMessage.pointID = point.pointID;
            $scope.pointDescription = point.companyName;
        };

        // Date pickers
        $scope.openDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.dateTimeOpened = !$scope.dateTimeOpened;
        };

        $scope.toggleTime = function () {
            $scope.isTimeOpen = !$scope.isTimeOpen;
        };

        //// PAGE SETUP ////
        $scope.mwfMessage.heJobID = jobID;
        $scope.mwfMessage.driverID = driverID;

    }]);