'use strict';

angular.module('peApp').factory('errorLoggerService', ['$injector', function ($injector) {
    var service = {};

    service.logError = function (ex, stack) {
        if (ex === null) {
            return;
        }

        apiErrorLog(ex, stack);
    };

    var apiErrorLog = _.debounce(function (ex, stack) {
        var apiService = $injector.get('apiService');
        apiService.error.log({ exception: ex, stackTrace: stack });
    }, 1000, true);

    return service;
}]);