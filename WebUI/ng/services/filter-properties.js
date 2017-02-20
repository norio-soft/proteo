'use strict';

// Provides a constructor function that can be used to provide useful helper properties/methods for a filter
angular.module('peApp').factory('FilterProperties', [function () {
    var FilterProperties = function (getFilter, setFilter, defaults) {
        this.status = {
            isOpen: false
        };

        this.isResetEnabled = function () {
            // deep compare with defaults
            return !angular.equals(getFilter(), defaults)
        };

        // Is the filter clear (i.e. there is no filter to be applied).  A filter is considered clear if none of the filter's properties cause any filter to need to be applied.
        // If a filter property is a check list then it is considered clear if either all or no items are selected.
        // If a filter property is a string then it is considered clear if it contains no text or its value matches the default value.
        // Note: this method currently assumes that all filter properties are either strings or check lists where that object's properties are the keys to filter on and the
        // values are boolean indicators of whether to filter or not.  If other filter property types are added then this will need to be extended.
        this.isClear = function () {
            var filter = getFilter()
            if (filter === undefined) {
                return true;
            }

            for (var propertyName in filter) {
                if (filter.hasOwnProperty(propertyName)) {
                    var propertyValue = filter[propertyName];

                    if (propertyValue === null) {
                        continue;
                    }

                    if (_.isString(propertyValue)) {
                        if (propertyValue !== '' && propertyValue != defaults[propertyName]) {
                            return false;
                        }
                    }
                    else if (_.isObject(propertyValue)) {
                        // Return false if there is more than one unique value (i.e. both true and false)
                        if (_.uniq(_.values(propertyValue)).length > 1) {
                            return false;
                        }
                    }
                    else {
                        console.log('Unexpected filter property type');
                    }
                }
            }

            return true;
        };

        this.reset = function () {
            var defaultsCopy = angular.copy(defaults);
            setFilter(defaultsCopy);
        };
    };

    return FilterProperties;
}]);
