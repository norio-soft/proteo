'use strict';

// NOTE: This interceptor is used for adding the authorization token to http requests originating from AngularJS
angular.module('proteo.shared.api').factory('authInterceptorService', ['$q', '$injector', '$rootScope', 'authenticationService', function ($q, $injector, $rootScope, authenticationService) {

    var authInterceptorServiceFactory = {};

    var addBearerTokenHeader = function (config, accessToken) {
        config.headers = config.headers || {};
        config.headers.Authorization = 'Bearer ' + accessToken;
    };

    var isApiRequest = function (url) {
        return url.toLowerCase().indexOf($rootScope.apiBaseUrl.toLowerCase()) === 0;
    };

    var request = function (config) {
        // If the request is being made to the api then add an authorization bearer token header
        if (isApiRequest(config.url)) {
            var accessToken = authenticationService.getToken();
            config.preExistingToken = accessToken != null;

            if (config.preExistingToken) {
                addBearerTokenHeader(config, accessToken);
            }
            else {              

                var deferred = $q.defer();
                var tokenService = $injector.get('tokenService');

                var tokenPromise = (authenticationService.useExternalTokenProvider) ?
                                    authenticationService.requestExternalToken() :
                                    tokenService.getApiToken();

                tokenPromise.then(
                        function (accessToken) {
                            addBearerTokenHeader(config, accessToken);
                            deferred.resolve(config);
                        },
                        function () {
                            deferred.reject();
                        });

                    return deferred.promise;

            }
        }

        return config;
    };

    var responseError = function (response) {
        if (response.status === 401 && response.config.preExistingToken && !authenticationService.useExternalTokenProvider) {
            // We have a token for WebAPI and the user is logged in to WebUI (Forms Authentication) but the token has expired.
            // Clear the token and then retry the http request which will cause a new token to be requested based on the current Forms Authentication cookie.
            authenticationService.clearToken();
            var $http = $injector.get('$http');
            return $http(response.config);
        }
        else if (response.status === 403) {
            $injector.get('$state').transitionTo('accessdenied', null, { location: 'replace' });
        }

        return $q.reject(response);
    };

    authInterceptorServiceFactory.request = request;
    authInterceptorServiceFactory.responseError = responseError;

    return authInterceptorServiceFactory;

}]);