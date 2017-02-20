'use strict';

angular.module('peApp').run([
    '$rootScope', '$state', '$stateParams','$window', '$document', '$timeout', 'authenticationService', 'tokenService', 'apiService',
    function ($rootScope, $state, $stateParams, $window, $document, $timeout, authenticationService, tokenService, apiService) {
        var configureDragAndDrop = function () {
            // Allow handling drag and drop in the same way on jQuery event handlers and regular javascript event handlers
            if ($window.jQuery && (-1 === $window.jQuery.event.props.indexOf('dataTransfer'))) {
                $window.jQuery.event.props.push('dataTransfer');
            }
        }

        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if (toState.data) {
                $rootScope.title = toState.data.pageTitle;
            }

            if (toParams.exttkn && !authenticationService.useExternalTokenProvider) {
                authenticationService.setUpExternalTokenProvider();
            }
            // User doesn't have a WebAPI token,     
            if (!authenticationService.isAuthenticated()) {                 
                // Use some other provider to get our token 
                if (authenticationService.useExternalTokenProvider) {
                    authenticationService.requestExternalToken();
                }
                else {
                    //get one based on the current WebUI Forms Authentication login  (or external token provider)
                    tokenService.getApiToken()
                }

            }

            //Ensure the user has permission to access the defined system portions
            if (toState.systemPortionsAny !== undefined && toState.resolve._canAccessAny === undefined) {
                var systemPortionIDs = toState.systemPortionsAny;

                toState.resolve._canAccessAny = function () {
                    return apiService.account.canAccessAny({ systemPortionIDs: systemPortionIDs }).$promise.then(
                        function (response) {
                            if (!response.canAccess) {
                                $state.transitionTo('accessdenied', null, { location: 'replace' });
                            }
                            else {
                                return response;
                            }
                        }
                    );
                };
            }

            if (toState.systemPortionsAll !== undefined && toState.resolve._canAccessAll === undefined) {
                var systemPortionIDs = toState.systemPortionsAll;

                toState.resolve._canAccessAll = function () {
                    return apiService.account.canAccessAll({ systemPortionIDs: systemPortionIDs }).$promise.then(
                         function (response) {
                             if (!response.canAccess) {
                                 $state.transitionTo('accessdenied', null, { location: 'replace' });
                             }
                             else {
                                 return response;
                             }
                         }
                    );
                };
            }
        });

        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;

        $rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams, error) {
            $state.go('resourceunits');
        });

        configureDragAndDrop();
    }
]);

// Temporary fix for bootstrap ui datepicker format issue with angular 1.3
// See https://github.com/angular-ui/bootstrap/issues/2659
    angular.module('peApp').directive('datepickerPopup', function () {
        return {
            restrict: 'EAC',
            require: 'ngModel',
            link: function (scope, element, attr, controller) {
                //remove the default formatter from the input directive to prevent conflict
                controller.$formatters.shift();
            }
        }
    });

// Temporary fix for window.showModalDialog
    window.showModalDialog = function (arg1, arg2, arg3) {
        var w;
        var h;
        var resizable = "no";
        var scroll = "no";
        var status = "no";

        // get the modal specs
        var mdattrs = arg3.split(";");

        for (var i = 0; i < mdattrs.length; i++) {
            var mdattr = mdattrs[i].split("=");

            var n = mdattr[0];
            var v = mdattr[1];
            if (n) { n = n.trim().toLowerCase(); }
            if (v) { v = v.trim().toLowerCase(); }

            if (n == "dialogheight") {
                h = v.replace("px", "");
            } else if (n == "dialogwidth") {
                w = v.replace("px", "");
            } else if (n == "resizable") {
                resizable = v;
            } else if (n == "scroll") {
                scroll = v;
            } else if (n == "status") {
                status = v;
            }
        }

        var left = window.screenX + (window.outerWidth / 2) - (w / 2);
        var top = window.screenY + (window.outerHeight / 2) - (h / 2);
        var targetWin = window.open(arg1, arg1, 'toolbar=no, location=no, directories=no, status=' + status + ', menubar=no, scrollbars=' + scroll + ', resizable=' + resizable + ', copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        targetWin.focus();
        return false;
    };

    function pausecomp(ms) {
        ms += new Date().getTime();
        while (new Date() < ms) { }
    }
