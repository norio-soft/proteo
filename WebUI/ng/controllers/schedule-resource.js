'use strict';

angular.module('peApp').controller('ScheduleResourceCtrl', ['$rootScope', '$scope', '$window', 'apiService',
function ($rootScope, $scope, $window, apiService) {

    $scope.data = {};
    $scope.resourceTypes = [];
    $scope.driverTypes = [];
    $scope.depots = [];
    $scope.selectedResourceType = "3";
    $scope.selectedDriverType = "0";
    $scope.selectedDepot = "0";
    $scope.showUnavailableResources = false;

    var viewStartDate = null;
    var viewEndDate = null;

    var onload = function () {
        apiService.scheduleResource.getResourceTypes().$promise.then(function (result) {
            $scope.resourceTypes = result;
        });
        apiService.scheduleResource.getDriverTypes().$promise.then(function (result) {
            $scope.driverTypes = result;
            $scope.driverTypes.unshift({ DriverTypeID: "0", Description: "-- all --" });
        });
        apiService.scheduleResource.getDepots().$promise.then(function (result) {
            $scope.depots = result;
            $scope.depots.unshift({ OrganisationLocationId: "0", OrganisationLocationName: "-- all --" });
        });
    };

    $scope.loadEvents = function (startDateTime, endDateTime) {
        viewStartDate = startDateTime;
        viewEndDate = endDateTime;

        var parameters = {
            resourceTypeId: parseInt($scope.selectedResourceType),
            startDateTime: startDateTime,
            endDateTime: endDateTime,
            driverTypeId: $scope.selectedResourceType === "3" ? parseInt($scope.selectedDriverType) : 0,
            orgLocationId: $scope.selectedResourceType === '2' ? 0 : parseInt($scope.selectedDepot),
            showUnavailable: $scope.showUnavailableResources
        };
        return apiService.scheduleResource.getResourceSchedule(parameters).$promise;
    };

    $scope.loadActivities = function () {
        var activityTypesParams = { resourceTypeId: parseInt($scope.selectedResourceType) };
        return apiService.scheduleResource.getActivityTypes(activityTypesParams).$promise;
    };

    $scope.loadResources = function () {
        var resourcesParams = {
            resourceTypeId: parseInt($scope.selectedResourceType),
            showUnavailable: $scope.showUnavailableResources
        };
        return apiService.scheduleResource.getResources(resourcesParams).$promise;
    };

    $scope.addEvent = function (resourceActivityTypeId, resourceId, instructionId, start_date, end_date, comments) {
        var resourceSchedule = {
            resourceScheduleId: 0,
            resourceActivityTypeId: resourceActivityTypeId,
            resourceId: resourceId,
            instructionId: instructionId,
            start_date: start_date,
            end_date: end_date,
            comments: comments,
            activityDescription: "",
            resourceDescription: ""
        };
        apiService.scheduleResource.createResourceSchedule(resourceSchedule).$promise.then(function () {
            //success
        }, function (err) {
            alert("Failed to add new event");
        });
    };

    $scope.updateEvent = function (resourceScheduleId, resourceActivityTypeId, resourceId, instructionId, start_date, end_date,
        comments) {
        var resourceSchedule = {
            resourceScheduleId: resourceScheduleId,
            resourceActivityTypeId: resourceActivityTypeId,
            resourceId: resourceId,
            instructionId: instructionId,
            start_date: start_date,
            end_date: end_date,
            comments: comments,
            activityDescription: "",
            resourceDescription: ""
        };
        apiService.scheduleResource.updateResourceSchedule(resourceSchedule).$promise.then(function () {
            //success
        }, function (err) {
            alert("Failed to update event");
        });
    };

    $scope.deleteEvent = function (resourceScheduleId) {
        apiService.scheduleResource.deleteResourceSchedule({ id: resourceScheduleId }).$promise.then(function () {
            //success
        }, function (err) {
            alert("Failed to delete the event");
        });
    };

    onload();
}]);