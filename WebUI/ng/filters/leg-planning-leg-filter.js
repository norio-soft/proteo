'use strict';

angular.module('peApp').filter('legPlanningLeg', ['ListFilterSummary', function (ListFilterSummary) {
    return function (items, filterSettings) {
        if (!items || !filterSettings) {
            return null;
        }

        /// === variables === ///
        var legStateSummary = new ListFilterSummary(filterSettings.legState);
        var trafficAreaSummary = new ListFilterSummary(filterSettings.trafficArea);
        var controlAreaSummary = new ListFilterSummary(filterSettings.controlArea);
        var businessTypeSummary = new ListFilterSummary(filterSettings.businessType);
        var mwfCommunicationStatusSummary = new ListFilterSummary(filterSettings.mwfCommunicationStatus);

        /// === functions === ///
        var contains = function (str1, str2) {
            if (!str1 || !str2) return false;
            return str1.toLowerCase().indexOf(str2.toLowerCase()) > -1;
        }

        var isMatch = function (item) {
            if (item.leg === undefined) {
                // item is not a leg so should not be filtered out
                return true;
            }

            if (filterSettings.freeText) {
                if (!(contains(item.leg.startPointDescription, filterSettings.freeText) ||
                    contains(item.leg.endPointDescription, filterSettings.freeText) ||
                    contains(item.leg.jobID.toString(), filterSettings.freeText))) return false;
            }

            if (filterSettings.location) {
                if (!(contains(item.leg.startPointDescription, filterSettings.location) || contains(item.leg.endPointDescription, filterSettings.location))) return false;
            }

            if (legStateSummary.shouldCheck) {
                if (!_.contains(legStateSummary.toCheck, String(item.leg.legStateID))) return false;
            }

            if (trafficAreaSummary.shouldCheck) {
                if (!_.contains(trafficAreaSummary.toCheck, item.leg.trafficArea)) return false;
            }

            if (controlAreaSummary.shouldCheck) {
                if (!_.contains(controlAreaSummary.toCheck, item.leg.controlArea)) return false;
            }

            if (businessTypeSummary.shouldCheck) {
                if (!_.contains(businessTypeSummary.toCheck, item.leg.businessType)) return false;
            }

            if (mwfCommunicationStatusSummary.shouldCheck) {
                if (!_.contains(mwfCommunicationStatusSummary.toCheck, item.leg.endMwfCommunicationStatus)) return false;
            }

            return true;
        };

        /// === filtering === ///
        return _.filter(items, isMatch);
    };
}]);