'use strict';


angular.module('peApp').controller('JobLegsModalCtrl', ['$scope', '$modalInstance', '$q', 'jobID', 'legEndInstructionID', 'driverResourceID', 'driverFullName', 'vehicleResourceID', 'vehicleRegistration', 'setPlannedTimesOnly', 'lastUpdateDateTime', 'apiService', 'jobUpdateBusinessRulesService', 
	function ($scope, $modalInstance, $q, jobID, legEndInstructionID, driverResourceID, driverFullName, vehicleResourceID, vehicleRegistration, setPlannedTimesOnly, lastUpdateDateTime, apiService, jobUpdateBusinessRulesService) {


    //// VARIABLES ////
    $scope.legs = {};

    $scope.driverFullName = driverFullName;
    $scope.vehicleRegistration = vehicleRegistration;
	

    //// FUNCTIONS ////
    var getInstructions = function (jobID, legEndInstructionID) {

        var params = { jobID: jobID, legEndInstructionID: legEndInstructionID };

        apiService.job.getLegs(params).$promise.then(
            function (response) {
                $scope.legs = response.map(function (val) {
                    return {
                        startInstructionID: val.startInstructionID,
                        startInstructionDate: moment(val.startInstructionDateTime, 'YYYY-MM-DDTHH:mm:ss').toDate(),
                        startInstructionTime: moment(val.startInstructionDateTime, 'YYYY-MM-DDTHH:mm:ss').format('HH:mm'),
                        startInstructionLocation: val.startInstructionLocation,

                        endInstructionID: val.endInstructionID,
                        endInstructionDate: moment(val.endInstructionDateTime, 'YYYY-MM-DDTHH:mm:ss').toDate(),
                        endInstructionTime: moment(val.endInstructionDateTime, 'YYYY-MM-DDTHH:mm:ss').format('HH:mm'),
                        endInstructionLocation: val.endInstructionLocation

                    };
                });
            },
            function () {
                // Error
            });
    };

    $scope.openStartDatePicker = function ($event, leg) {
        $event.preventDefault();
        $event.stopPropagation();
        leg.startDateOpen = true;
    };


    $scope.openEndDatePicker = function ($event, leg) {
        $event.preventDefault();
        $event.stopPropagation();
        leg.endDateOpen = true;
    };


	$scope.save = function () {
		//// Validation
		$scope.$broadcast('show-errors-check-validity');

		if ($scope.legsForm.$invalid) {
			return;
		}

		var params = {
			driverResourceID: driverResourceID,
			vehicleResourceID: vehicleResourceID,
			setPlannedTimesOnly: setPlannedTimesOnly,
			jobLastUpdateDateTime: lastUpdateDateTime,
		};

		var legsForPlanning = [];

		angular.forEach($scope.legs, function (leg, key) {
			var startDate = moment(leg.startInstructionDate).format('DD/MM/YYYY');
			var startTime = leg.startInstructionTime;

			var endDate = moment(leg.endInstructionDate).format('DD/MM/YYYY');
			var endTime = leg.endInstructionTime;

			var startInstructionDateTime = moment(startDate + ' ' + startTime, 'DD/MM/YYYY HH:mm').toDate();
			var endInstructionDateTime = moment(endDate + ' ' + endTime, 'DD/MM/YYYY HH:mm').toDate();

			var legPlan = {
				startInstructionID: leg.startInstructionID,
				endInstructionID: leg.endInstructionID,
				startDateTime: startInstructionDateTime,
				endDateTime: endInstructionDateTime,
			};

			legsForPlanning.push(legPlan);
		});

		apiService.legPlan.planAndResource(params, legsForPlanning,
			function (response) {
				// Once all legs are saved close the modal
				$scope.close(legsForPlanning[0].startDateTime, legsForPlanning[legsForPlanning.length - 1].endDateTime);
			}, function (error) {
				$scope.errors = ['Leg planning was unsuccessful'];

				if (error.data !== undefined && error.data.ModelState !== undefined) {
					$scope.errors = jobUpdateBusinessRulesService.getInfringements(error.data.ModelState, true);
				}
				else if (error.status === 500 && error.statusText == 'Internal Server Error') {
					$scope.errors = ['Leg planning was unsuccessful: an unexpected error occurred.'];
				}
			});
	};
	

	$scope.cancel = function () {
		$modalInstance.dismiss('cancel');
	};

	$scope.close = function (startDateTime, endDateTime) {
		$modalInstance.close(startDateTime, endDateTime);
	};

    getInstructions(jobID, legEndInstructionID);

}]);
