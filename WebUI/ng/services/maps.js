'use strict';

angular.module('peApp').service('mapsService', ['$http', '$q', '_',
    function ($http, $q, _) {

        // Takes an array of waypoints which are lat/lng pairs.
        // Returns a promise of an array of routeLeg objects, each one of which has travelDistance and travelDuration properties and a coordinates property which is an array of lat/lng pairs.
        // Promise will be rejected if no route can be found
        this.calculateRoute = function (credentials, waypoints) {
            var platform = getHereMapsPlatform(credentials);

            var router = platform.getRoutingService();

            var legQueries = [];

            for (var i = 1; i < waypoints.length; i++) {
                legQueries.push({ from: waypoints[i - 1], to: waypoints[i] });
            }

            var legPromises = legQueries.map(function (legQuery) {
                var deferred = $q.defer();

                var onRouteSuccess = function (result) {
                    if (result.response !== undefined && result.response.route !== undefined && result.response.route.length > 0) {
                        deferred.resolve(result.response.route[0]);
                    }
                    else {
                        deferred.reject(result.details);
                    }
                };

                var onRouteError = function (result) {
                    deferred.reject();
                };

                router.calculateRoute({
                    mode: 'fastest;truck',
                    representation: 'display',
                    routeattributes: 'summary,shape',
                    maneuverattributes: 'none',
                    waypoint0: legQuery.from.lat + ',' + legQuery.from.lng,
                    waypoint1: legQuery.to.lat + ',' + legQuery.to.lng
                }, onRouteSuccess, onRouteError);

                return deferred.promise;
            });

            var resultPromise = $q.all(legPromises).then(function (legResults) {
                return legResults.map(function (legResult, index) {
                    var coordinates = legResult.shape.map(function (point) {
                        var parts = point.split(',');
                        return { lat: parts[0], lng: parts[1] };
                    });

                    if (index > 0) {
                        // remove the first co-ordinate for all legs other than the first leg (otherwise the overall route contains duplicate co-ordinates)
                        coordinates.shift();
                    }

                    var leg = legResult.leg[0];

                    return {
                        travelDistance: legResult.summary.distance * 0.000621371, // convert metres to miles
                        travelDuration: legResult.summary.travelTime,
                        coordinates: coordinates,
                    };
                });
            });

            return resultPromise;
        };

        
        this.locationQuery = function (credentials, locationQuery) {
            var platform = getHereMapsPlatform(credentials);

            var deferred = $q.defer();

            var geocoder = platform.getGeocodingService();
            var geocodingParams = {
                searchText: locationQuery,
                jsonattributes: 1
            }

            var onGeocodeSuccess = function (result) {
                if (result.response.view.length > 0) {
                    deferred.resolve(result.response.view[0].result);
                }
                else {
                    deferred.reject();
                }
            };

            var onGeocodeError = function (result) {
                deferred.reject();
            };

            geocoder.geocode(
                geocodingParams,
                onGeocodeSuccess,
                onGeocodeError
            );

            return deferred.promise;
        };

        var getHereMapsPlatform = function (credentials) {
            return new H.service.Platform({
                app_id: credentials.app_id,
                app_code: credentials.app_code,
                useCIT: true,
                useHTTPS: true
            });
        }

        var round = function (number, decimals) {
            return +(Math.round(number + 'e+' + decimals) + 'e-' + decimals);
        };

    }]);