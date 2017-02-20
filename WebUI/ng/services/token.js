'use strict';

angular.module('peApp').service('tokenService', ['$rootScope', '$http', '$q', 'authenticationService', function ($rootScope, $http, $q, authenticationService) {

    var retrieveNewToken = function () {
        return $http({
            method: 'POST',
            url: $rootScope.peBaseUrl + '/token',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            data: $.param({
                'grant_type': 'forms_authentication_cookie'
            })
        });
    };

    this.getApiToken = function () {
        // Retrieve a WebAPI bearer token based on the current user's WebUI Forms Authentication cookie.
        // If successful then the token is stored, otherwise the user is redirected to the login screen.
        // Returns a promise which is resolved with the new token if one is retrieved successfully.
        var deferred = $q.defer();

        retrieveNewToken().then(
            function (response) {
                var data = response.data;

                if (response.status === 200 && data.access_token) {
                    var expiryDateTime = moment().add(data.expires_in, 'seconds').toDate();
                    authenticationService.storeToken(data.access_token, expiryDateTime, data.user_id, data.user_name, data.first_name, data.last_name);
                    deferred.resolve(data.access_token);
                }
                else {
                    // The user is not logged in via Forms Authentication so we can't get a new token.
                    // Redirect to the login page.
                    authenticationService.redirectToLoginPage();
                    deferred.reject();
                }
            },
            function () {
                authenticationService.redirectToLoginPage();
                deferred.reject();
            }
        
        
        );

        return deferred.promise;
    };

}]);