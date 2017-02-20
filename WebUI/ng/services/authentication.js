'use strict';

angular.module('peApp').service('authenticationService', ['$rootScope', 'ipCookie', '$window', '$q', function ($rootScope, ipCookie, $window, $q) {

    var _this = this;

    this.readCookie = function () {
        if (!this.useExternalTokenProvider && $rootScope.authCookieName && ipCookie($rootScope.authCookieName) === undefined) {
            _this.clearToken();
            return null;
        }

        if (!ipCookie('userData')) {
            return null;
        }

        var test = ipCookie('userData');
        var cookie = angular.fromJson(ipCookie('userData'));

        // if the token is going to expire within the next 30 seconds then don't use it, since we are likely to get a 401 back and then would have to request a new token anyway.
        if (moment().add(30, 'seconds').isAfter(cookie.expiryDateTime)) {
            return null;
        }

        return cookie;
    };

    var writeCookie = function (userData) {
        ipCookie('userData', angular.toJson(userData), { path: '/'} );
    };

    this.storeToken = function (token, expiryDateTime, userID, userName, firstName, lastName) {
        writeCookie({
            accessToken: token,
            expiryDateTime: expiryDateTime,
            userID: userID,
            userName: userName,
            firstName: firstName,
            lastName: lastName
        });
    };

    this.clearToken = function () {
        ipCookie.remove('userData', { path: '/' });
    };

    this.getToken = function () {

        var userData = this.readCookie();
        return userData ? userData.accessToken || null : null;
    };

    this.isAuthenticated = function () {
        return this.getToken() !== null;
    };

    this.getUserID = function () {
        var userData = this.readCookie();
        return userData ? userData.userID : null;
    };

    this.getUserName = function () {
        var userData = this.readCookie();
        return userData ? userData.userName || null : null;
    };

    this.getFullName = function () {
        var userData = this.readCookie();
        return userData ? ((userData.firstName || '') + ' ' + (userData.lastName || '')) : null;
    };

    this.logout = function () {
        var userData = this.readCookie();
        $window.location.href = '/security/login.aspx?lo=1';
    };

    this.redirectToLoginPage = function () {
        $window.location.href = '/security/login.aspx?returnurl=' + encodeURIComponent($window.location.pathname);
    };

    this.useExternalTokenProvider = false;

    this.setUpExternalTokenProvider = function() {
        this.useExternalTokenProvider = true;

        window.addEventListener('message', function (event) {

            if (event.data.message == "token") {
                var expiryDateTime = moment().add(300, 'seconds').toDate();
                _this.storeToken(event.data.token, expiryDateTime);
            }

            if (event.data.message == "logout") {
                _this.clearToken();
            }
        });

    }

    this.requestExternalToken = function () {
        var deferred = $q.defer();
        var data = {message: "requestToken"};
        window.parent.postMessage(data, '*')

        setTimeout(function () {
            if (_this.isAuthenticated()) {
                deferred.resolve(_this.getToken());
            } else {
                deferred.reject();
            }
        }, 100);

        return deferred.promise;
    }


}]);