'use strict';

angular.module('peApp').factory('legMenuItemsService', ['legacyAppWindowService', 'apiService', '$modal', function (legacyAppWindowService, apiService, $modal) {
    var service = {};

    service.getMenuItems = function (
        endInstructionID,
        legStateID,
        jobID,
        jobLastUpdateDateTime,
        startDateTime,
        endDateTime,
        driverResourceID,
        driverName,
        vehicleResourceID,
        vehicleRegistration,
        trailerResourceID,
        trailerRef,
        subcontractorIdentityID,
        isMwfAllowed,
        onUpdated,
        onError
    ) {
        var contextMenuItems = [];

        var doUpdated = function () {
            if (angular.isFunction(onUpdated)) {
                onUpdated();
            }
        };

        var doError = function () {
            if (angular.isFunction(onError)) {
                onError();
            }
        };

        contextMenuItems.push({
            caption: 'View Run',
            action: function () {
                legacyAppWindowService.viewRun(jobID);
            },
        });

        contextMenuItems.push({
            caption: 'Resource This',
            action: function () {
                legacyAppWindowService.resourceThis(
                    endInstructionID,
                    driverName,
                    driverResourceID,
                    vehicleRegistration,
                    vehicleResourceID,
                    trailerRef,
                    trailerResourceID,
                    startDateTime,
                    endDateTime,
                    jobLastUpdateDateTime,
                    jobID,
                    doUpdated);
            },
        });

        contextMenuItems.push({ isDivider: true });

        contextMenuItems.push({
            caption: 'Subcontract',
            action: function () {
                legacyAppWindowService.subcontract(jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({ isDivider: true });
        
        contextMenuItems.push({
            caption: 'Communicate This',
            isDisabled: legStateID !== 2,
            tooltip: legStateID !== 2 ? 'You can only communicate planned legs' : null,
            action: function () {
                legacyAppWindowService.communicateThis(endInstructionID, driverName, driverResourceID, subcontractorIdentityID, jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Quick Communicate This',
            isDisabled: legStateID !== 2,
            tooltip: legStateID !== 2 ? 'You can only communicate planned legs' : null,
            action: function () {
                apiService.legCommunication.quickCommunicate(
                    null,
                    {
                        jobID: jobID,
                        legEndInstructionID: endInstructionID,
                        driverResourceID: driverResourceID,
                        vehicleResourceID: vehicleResourceID,
                        subcontractorIdentityID: subcontractorIdentityID,
                        comments: 'Communicated from leg planning screen',
                    },
                    function () {
                        doUpdated();
                    },
                    function (error) {
                        doError(error);
                    });
            },
        });

        contextMenuItems.push({
            caption: 'Remove Communication',
            isDisabled: legStateID !== 3,
            tooltip: legStateID !== 3 ? 'You can only uncommunicate legs in progress' : null,
            action: function () {
                apiService.legCommunication.unCommunicate(
                    {
                        jobID: jobID,
                        legEndInstructionID: endInstructionID
                    },
                    function (response) {
                        doUpdated();
                    },
                    function (error) {
                        doError(error);
                    });
            },
        });

        contextMenuItems.push({
            caption: 'Send MWF Message',
            isDisabled: !isMwfAllowed,
            tooltip: !isMwfAllowed ? 'MWF communication is not applicable this leg' : null,
            action: function () {
                if (isMwfAllowed) {
                    $modal.open({
                        templateUrl: 'html-partials/send-mwf-message-modal.html',
                        controller: 'sendMWFMessageModalCtrl',
                        resolve: {
                            driverID: function () {
                                return driverResourceID;
                            },
                            jobID: function () {
                                return jobID;
                            },
                        }
                    });
                }
            },
        });

        contextMenuItems.push({ isDivider: true });

        contextMenuItems.push({
            caption: 'Trunk',
            action: function () {
                legacyAppWindowService.trunk(endInstructionID, driverName, vehicleRegistration, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Multi-Trunk',
            action: function () {
                legacyAppWindowService.multiTrunk(jobID, endInstructionID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Remove Trunk',
            action: function () {
                legacyAppWindowService.removeTrunk(jobID, endInstructionID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({ isDivider: true });

        contextMenuItems.push({
            caption: 'Change Booked Times',
            action: function () {
                legacyAppWindowService.changeBookedTimes(jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Change Planned Times',
            action: function () {
                legacyAppWindowService.changePlannedTimes(jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Call In',
            action: function () {
                legacyAppWindowService.callIn(jobID, jobLastUpdateDateTime);
            },
        });

        contextMenuItems.push({
            caption: 'Add Destination',
            action: function () {
                legacyAppWindowService.addDestination(jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Add Multiple Destinations',
            action: function () {
                legacyAppWindowService.addMultipleDestinations(jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        contextMenuItems.push({
            caption: 'Remove Destination',
            action: function () {
                legacyAppWindowService.removeDestination(jobID, jobLastUpdateDateTime, doUpdated);
            },
        });

        return contextMenuItems;
    };



    service.getTravelNoteMenuItems = function (id, resourceSchedule,refreshMethod, onUpdated, onError) {
        var contextMenuItems = [];
        var doUpdated = function () {
            if (angular.isFunction(onUpdated)) {
                onUpdated();
            }
        };

        var doError = function () {
            if (angular.isFunction(onError)) {
                onError();
            }
        };


        contextMenuItems.push({
            caption: 'Remove',
            action: function () {
                apiService.resourceSchedule.deleteResourceSchedule(
                    {
                        id: id
                    },
                    function (response) {
                        doUpdated();
                    },
                    function (error) {
                        doError(error);
                    });
            },
        });
        contextMenuItems.push({
            caption: 'Edit',
            action: function(){
                var m = $modal.open({
                    templateUrl: 'html-partials/add-edit-resourceschedule-modal.html',
                    controller: 'AddEditResourceScheduleModalCtrl',
                    size: 'lg',
                    resolve: {
                        resourceSchedule: function () { return resourceSchedule; }
                    }
                });

                m.result.then(refreshMethod);
                               
            }
        });
            
        return contextMenuItems;
    }
    return service;
}]);