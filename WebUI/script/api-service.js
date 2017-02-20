// Service to allow calls to Orchestrator.WebAPI methods
// - apiService.get() takes one parameter, the full url, returns a promise
(function () {
    if (!window.location.origin) {
        // polyfill for IE10
        window.location.origin =
            window.location.protocol + "//"
            + window.location.hostname
            + (window.location.port ? ':' + window.location.port : '');
    }

    var bearerToken = null;

    var getBearerToken = function () {
        var deferred = $.Deferred();

        if (bearerToken !== null) {
            deferred.resolve(bearerToken);
        }
        else {
            var getToken = $.ajax({
                method: 'POST',
                url: window.location.origin + '/token',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                data: { 'grant_type': 'forms_authentication_cookie' },
            });

            getToken.done(function (data) {
                bearerToken = data.access_token;
                deferred.resolve(bearerToken);
            });
        }

        return deferred.promise();
    };

    var get = function (url) {
        var deferred = $.Deferred();
        var getToken = getBearerToken();

        getToken.done(function (token) {
            var getData = $.ajax({
                url: url,
                headers: { 'Authorization': 'Bearer ' + token },
            });

            getData.done(function (data) {
                deferred.resolve(data);
            });
        });

        return deferred.promise();
    };

    window.apiService = {
        get: get,
    };
})();
