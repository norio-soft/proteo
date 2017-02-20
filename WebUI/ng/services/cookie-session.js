'use strict';

angular.module('peApp').factory('cookieSessionService', ['$injector', function ($injector) {
    var service = {};

    function getCookieSessionID() {
        if (sessionStorage && !sessionStorage.sessionID) {
            sessionStorage.sessionID = getRandomID(6);
        }
    }


    function getRandomID(length) {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < length; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    }

    service.getCookieSessionIDString = function () {
        return (sessionStorage && sessionStorage.sessionID) ? '&csid=' + sessionStorage.sessionID : '';
    };

    getCookieSessionID();

    return service;
}]);