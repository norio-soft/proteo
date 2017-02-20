'use strict';

// Provides a constructor function for a list filter that is used to simplify applying the filter
angular.module('peApp').factory('ListFilterSummary', [function () {
    var ListFilterSummary = function (filter) {
        this.total = 0;
        this.toCheck = [];

        for (var property in filter) {
            if (filter.hasOwnProperty(property)) {
                this.total++;

                if (filter[property]) {
                    this.toCheck.push(property);
                }
            }
        }

        // only bother checking lists when at least one item has been selected/unselected
        this.shouldCheck = this.toCheck.length !== 0 && this.toCheck.length !== this.total;
    };

    return ListFilterSummary;
}]);
