'use strict';

angular.module('peApp').factory('jobUpdateBusinessRulesService', function () {
    var service = {};

    service.getInfringements = function (modelState, includeUnexpectedInfringementDescriptions) {
        var infringements = [];

        for (var errorType in modelState) {
            switch (errorType) {
                case 'Orchestrator.BusinessRules.eBRJob.JobTimingsDoNotFlow':
                    infringements.push('Legs timings do not flow correctly. Please check the dates and times and try again.');
                    break;
                case 'Orchestrator.BusinessRules.eBRJob.JobVersionDoNotMatch':
                case 'Orchestrator.BusinessRules.eBRLeg.JobVersionDoNotMatch':
                    infringements.push('The run has been updated by someone else. Please refresh your screens and try again.');
                    break;
                default:
                    if (includeUnexpectedInfringementDescriptions && modelState[errorType].length > 0) {
                        infringements = infringements.concat(modelState[errorType]);
                    };
                    break;
            }
        }

        return infringements;
    };

    return service;
});