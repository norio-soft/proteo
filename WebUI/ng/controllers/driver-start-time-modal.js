'use strict';

angular.module('peApp').controller('DriverStartTimeModalCtrl', ['$scope', '$modalInstance', 'driverResourceID', 'driverFullName', 'initialDate', 'apiService', function ($scope, $modalInstance, driverResourceID, driverFullName, initialDate, apiService) {

    //// VARIABLES ////
    $scope.timeTypeCaption = 'Start';

    //// FUNCTIONS ////
    var getDriverStartTime = function (date) {
        var params = { driverResourceID: $scope.driverTime.driverResourceID, date: $scope.driverTime.date };

        apiService.driverStartTime.get(params).$promise.then(
            function (response) {
                $scope.driverTime.time = moment(response.startTime, 'HH:mm:ss').format('HH:mm');
                $scope.driverTime.notes = response.notes;
            },
            function () {
                $scope.driverTime.notes = null;
            });
    };

    $scope.save = function () {
        // Validation
        $scope.$broadcast('show-errors-check-validity');

        if ($scope.driverTimeForm.$invalid) {
            return;
        }

        var params = { driverResourceID: $scope.driverTime.driverResourceID };
        var data = { date: $scope.driverTime.date, startTime: $scope.driverTime.time, notes: $scope.driverTime.notes };

        // Save
        apiService.driverStartTime.set(params, data).$promise.then(function (response) {
            $modalInstance.close($scope.driverTime);
        });
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    // Date picker
    $scope.openDatePicker = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();

        $scope.datePickerOpened = true;
    };

    //// PAGE SETUP ////
    $scope.$watch('driverTime.date', function (oldValue, newValue) {
        getDriverStartTime();
    });

    $scope.driverTime = {
        driverResourceID: driverResourceID,
        date: moment(initialDate).startOf('day').toDate(),
        time: null,
        notes: null
    };

    $scope.driverFullName = driverFullName;

}]);
