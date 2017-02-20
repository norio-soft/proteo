'use strict';

angular.module('peApp').factory('legacyAppWindowService', ['$sce', '$window', 'cookieSessionService', function ($sce, $window, cookieSessionService) {
    var service = {};

    $window.dialogCallbacks = $window.dialogCallbacks || {};

    service.viewRun = function (jobID) {
        var url = $sce.trustAsResourceUrl(
            '/job/job.aspx'
                + '?wiz=' + 'true'
                + '&jobId=' + jobID
                + cookieSessionService.getCookieSessionIDString()
                + '&rs=' + new Date());

        $window.open(url, '', 'height=1000,width=1090,toolbar=0,menu=0,scrollbars=1').focus();
    };

    service.resourceThis = function (endInstructionID, driverName, driverResourceID, vehicleRegistration, vehicleResourceID, trailerRef, trailerResourceID, startDateTime, endDateTime, jobLastUpdateDateTime, jobID, callback) {
        $window.dialogCallbacks.resourceThis = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        //TODO: pass in and use control area, traffic areas and depot id
        var url = $sce.trustAsResourceUrl(
            '/traffic/resourcethis.aspx'
                + '?iID=' + endInstructionID
                + '&Driver=' + driverName
                + '&DR=' + driverResourceID
                + '&RegNo=' + vehicleRegistration
                + '&VR=' + vehicleResourceID
                + '&TrailerRef=' + trailerRef
                + '&TR=' + trailerResourceID
                + '&LS=' + startDateTime
                + '&LE=' + endDateTime
                + '&DC=' + 'TST'
                + cookieSessionService.getCookieSessionIDString()
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&jobId=' + jobID
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.resourceThis');

        $window.open(url, '', 'width=1000,height=800,left=10,resizable=1,scrollbars=1,menubar=0,toolbar=0,status=0,location=0').focus();
    };

    service.subcontract = function (jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.subcontract = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/subcontract.aspx'
                + '?jobId=' + jobID
                + '&lastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.subcontract');

        $window.showModalDialog(url, null, 'dialogWidth=650px;dialogHeight=580px;resizable=1;scroll=1;status=0');
    };

    service.communicateThis = function (endInstructionID, driverName, driverResourceID, subcontractorIdentityID, jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.communicateThis = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/communicatethis.aspx'
                + '?iID=' + endInstructionID
                + '&Driver=' + driverName
                + '&DR=' + driverResourceID
                + '&SubbyId=' + (subcontractorIdentityID || '')
                + '&jobId=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.communicateThis');

        $window.showModalDialog(url, null, 'dialogWidth=800px;dialogHeight=650px;resizable=1;scroll=1;status=0');
    };

    service.trunk = function (endInstructionID, driverName, vehicleRegistration, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.trunk = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/trunk.aspx'
                + '?iID=' + endInstructionID
                + '&Driver=' + driverName
                + '&RegNo=' + vehicleRegistration
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.trunk');

        $window.showModalDialog(url, null, 'dialogWidth=550px;dialogHeight=380px;resizable=1;scroll=1;status=0');
    };

    service.multiTrunk = function(jobID, endInstructionID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.multiTrunk = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/multitrunk.aspx'
                + '?jobID=' + jobID
                + '&iID=' + endInstructionID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.multiTrunk');

        $window.showModalDialog(url, null, 'dialogWidth=720px;dialogHeight=450px;resizable=1;scroll=1;status=0');
    };

    service.removeTrunk = function (jobID, endInstructionID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.removeTrunk = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/removetrunk.aspx'
                + '?jobID=' + jobID
                + '&iID=' + endInstructionID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.removeTrunk');

        $window.showModalDialog(url, null, 'dialogWidth=550px;dialogHeight=358px;resizable=1;scroll=1;status=0');
    };

    service.changeBookedTimes = function (jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.changeBookedTimes = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/changebookedtimes.aspx'
                + '?jobID=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.changeBookedTimes');

        $window.showModalDialog(url, null, 'dialogWidth=860px;dialogHeight=550px;resizable=1;scroll=1;status=0');
    };

    service.changePlannedTimes = function (jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.changePlannedTimes = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/traffic/changeplannedtimes.aspx'
                + '?jobID=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.changePlannedTimes');

        $window.showModalDialog(url, null, 'dialogWidth=700px;dialogHeight=320px;resizable=1;scroll=1;status=0');
    };

    service.callIn = function (jobID, jobLastUpdateDateTime) {
        var url = $sce.trustAsResourceUrl(
            '/traffic/jobmanagement/drivercallin/callin.aspx'
                + '?jobID=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date());

        $window.showModalDialog(url, null, 'dialogWidth=900px;dialogHeight=600px;resizable=1;scroll=1;status=0');
    };

    service.addDestination = function (jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.addDestination = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/job/adddestination.aspx'
                + '?wiz=' + 'true'
                + '&jobID=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.addDestination');

        $window.showModalDialog(url, null, 'dialogWidth=550px;dialogHeight=380px;resizable=1;scroll=1;status=0');
    };

    service.addMultipleDestinations = function (jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.addMultiDestination = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/job/addmultidestination.aspx'
                + '?wiz=' + 'true'
                + '&jobID=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.addMultiDestination');

        $window.showModalDialog(url, null, 'dialogWidth=800px;dialogHeight=600px;resizable=1;scroll=1;status=0');
    };

    service.removeDestination = function (jobID, jobLastUpdateDateTime, callback) {
        $window.dialogCallbacks.removeDestination = function (source, returnValue) {
            if (returnValue && callback) {
                callback();
            }
        };

        var url = $sce.trustAsResourceUrl(
            '/job/removedestination.aspx'
                + '?wiz=' + 'true'
                + '&jobID=' + jobID
                + '&LastUpdateDate=' + jobLastUpdateDateTime
                + '&rs=' + new Date()
                + '&dcb=' + 'window.dialogCallbacks.removeDestination');

        $window.showModalDialog(url, null, 'dialogWidth=550px;dialogHeight=380px;resizable=1;scroll=1;status=0');
    };

    return service;
}]);