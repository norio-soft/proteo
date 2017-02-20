'use strict';

angular.module('peApp').filter('duration', function () {
    return function (durationInSeconds) {
        return (durationInSeconds === null || durationInSeconds === undefined) ? null : moment.duration(durationInSeconds, 'seconds').format('hh:mm:ss', { trim: false });
    };
});