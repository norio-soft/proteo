'use strict';


angular.module('peApp').controller('AddEditResourceScheduleModalCtrl', ['$scope', '$rootScope', '$modalInstance', '$modal', '$timeout', 'apiService', 'resourceSchedule', '$filter',
    function ($scope, $rootScope, $modalInstance, $modal, $timeout, apiService, resourceSchedule, $filter) {

        $scope.TRAVEL_NOTE = 30;
        //// VARIABLES ////
        $scope.colors = [{ key: 'btn-color1', isChecked: false, color: '#6f6e52' }, { key: 'btn-color2', isChecked: false, color: '#f4a341' }, { key: 'btn-color3', isChecked: false, color: '#67c597' }, { key: 'btn-color4', isChecked: false, color: '#62a9d4' }, { key: 'btn-color5', isChecked: false, color: '#b089c9' }];
        $scope.activityTypes = {};
        $scope.startDateOpened = false;
        $scope.finishDateOpened = false;
        $scope.datePickerOptions = {
            'starting-day': 1
        };

        $scope.setColor = function (c) {
            $scope.resourceSchedule.colour = c;
            _.each($scope.colors, function (n, key) {
                if (n.color != c)
                {
                    n.isChecked = false;
                }
                });
        };

        $scope.setActivityColor = function () {
            if (resourceSchedule.resourceActivityTypeID != $scope.TRAVEL_NOTE)
                $scope.setColor('#6f6e52')

        }
        
        $scope.timePickerOptions = {
            timeFormat: 'H:i'
        };

        //// FUNCTIONS ////

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

        // Date pickers
        $scope.openStartDatePicker = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.startDateOpened = !$scope.startDateOpened;
        };

        $scope.openFinishDatePicker = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope.finishDateOpened = !$scope.finishDateOpened;
        };

       
        $scope.save = function () {
            // Do Validation
            $scope.$broadcast('show-errors-check-validity');

            $scope.addEditResourceScheduleForm.finishDate.$setValidity('finishDateTimeLesser', true);
            $scope.addEditResourceScheduleForm.finishTime.$setValidity('finishDateTimeLesser', true);

            var startDateTime = moment(moment($scope.resourceSchedule.startDate).startOf('day').format('DD/MM/YYYY') + ' ' + moment($scope.resourceSchedule.startTime, 'HH:mm').format('HH:mm'), 'DD/MM/YYYY HH:mm').toDate();
            var endDateTime = moment(moment($scope.resourceSchedule.finishDate).startOf('day').format('DD/MM/YYYY') + ' ' + moment($scope.resourceSchedule.finishTime, 'HH:mm').format('HH:mm'), 'DD/MM/YYYY HH:mm').toDate();

            $scope.resourceSchedule.startDateTime = startDateTime;
            $scope.resourceSchedule.endDateTime = endDateTime;

            var resourceScheduleCopy = angular.copy($scope.resourceSchedule);
            resourceScheduleCopy.startDateTime = $filter('date')(startDateTime, 'yyyy-MM-dd HH:mm:ss');
            resourceScheduleCopy.endDateTime = $filter('date')(endDateTime, 'yyyy-MM-dd HH:mm:ss');                 

            if (startDateTime > endDateTime)
            {
                $scope.addEditResourceScheduleForm.finishDate.$setValidity('finishDateTimeLesser', false);
                $scope.addEditResourceScheduleForm.finishTime.$setValidity('finishDateTimeLesser', false);
            }

            if ($scope.addEditResourceScheduleForm.$invalid) {
                $scope.saveWait = false;
                return;
            }

            apiService.resourceSchedule.update(resourceScheduleCopy).$promise.then(function () {
                $modalInstance.close();
            },
            function (error) {
                $modalInstance.close(error);
            });

        };

        var getActivityTypes = function () {
            var params = {resourceTypeId: 3}
            apiService.resourceSchedule.getActivityTypes(params).$promise.then(function (result) {
                $scope.activityTypes = result;
            },
            function (err) {
                $modalInstance.close(error);
            });
        }
      
        //// PAGE SETUP ////
        getActivityTypes();

        if (resourceSchedule.resourceScheduleID > 0) {
            $scope.isEdit = true;
            _.each($scope.colors, function (n, key) {
                if (n.color == resourceSchedule.colour) {
                    n.isChecked = true;
                }
            });
            resourceSchedule.finishDateTime = resourceSchedule.endDateTime;
            
        }
        else {
            $scope.isEdit = false;
            resourceSchedule.finishDateTime = moment(resourceSchedule.startDateTime).add(1, 'h');
            $scope.colors[0].isChecked = true;
            resourceSchedule.resourceActivityTypeID = $scope.TRAVEL_NOTE; // default to travel Note
        }

        resourceSchedule.startDate = moment(resourceSchedule.startDateTime).toDate();
        resourceSchedule.startTime = moment(resourceSchedule.startDateTime).format('HH:mm');
        resourceSchedule.finishTime = moment(resourceSchedule.finishDateTime).format('HH:mm');
        resourceSchedule.finishDate = moment(resourceSchedule.finishDateTime).toDate();

        $scope.resourceSchedule = resourceSchedule;
       
    }]);
