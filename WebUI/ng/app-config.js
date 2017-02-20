'use strict';

angular.module('peApp').config([
    '$provide', '$stateProvider', '$urlRouterProvider', '$locationProvider', '$httpProvider', 'cfpLoadingBarProvider', '$compileProvider', function ($provide, $stateProvider, $urlRouterProvider, $locationProvider, $httpProvider, cfpLoadingBarProvider, $compileProvider) {
        $locationProvider.html5Mode(true);
        $urlRouterProvider.otherwise('/resourceunits');

        // performance increase, plus stops memory leaks
        // re-enable with angular.reloadWithDebugInfo(); in console#
        // see https://docs.angularjs.org/guide/production
        $compileProvider.debugInfoEnabled(false);

        // Temporary workaround fix for not being able to add state resolves in stateChangeStart
        // See https://github.com/angular-ui/ui-router/issues/1165
        $stateProvider.decorator('path', function (state, parentFn) {
            if (state.self.resolve === undefined) {
                state.self.resolve = {};
                state.resolve = state.self.resolve;
            }
            return parentFn(state);
        });

        window.enums = {
            systemPortion: { 
                addEditRun: 1,
                generalUsage: 2,
                plan: 3,
                communicate: 4,
                takeCallIn: 5,
                attachPOD: 6,
                price: 7,
                processCancellation: 8,
                addEditRate: 9,
                invoicing: 10,
                attachPCVToRun: 11,
                registerClientUser: 12,
                scanPOD: 13,
                searchForPOD: 14,
                configureSchedule: 15,
                addEditResource: 16,
                addEditPoint: 17,
                addEditOrganisation: 18,
                kpi: 19,
                controlAreaManagement: 20,
                systemUsage: 21,
                addEditUser: 22,
                palletBalanceManagement: 23,
                addEditOrder: 24,
                approveOrder: 25,
                addEditCreditManagement: 26,
                gpsProfileManagement: 27,
                addEditResourceUnit: 29,
                importMessageManagement: 30,
                clientPortalUser: 31
            }
        };

        $stateProvider
            .state('accessdenied', {
                url: '/accessdenied',
                templateUrl: 'html-partials/access-denied.html'
            })
            .state('resourceunits', {
                url: '/resourceunits',
                controller: 'ResourceUnitsCtrl',
                templateUrl: 'html-partials/resource-units.html',
                systemPortionsAll: [window.enums.systemPortion.addEditResourceUnit, window.enums.systemPortion.generalUsage],
                data: {
                    pageTitle: 'Resource Units'
                }
            })
            .state('legplanning', {
                url: '/legplanning',
                controller: 'LegPlanningCtrl',
                templateUrl: 'html-partials/leg-planning.html?d=14072016120000',
                systemPortionsAll: [window.enums.systemPortion.plan, window.enums.systemPortion.communicate, window.enums.systemPortion.generalUsage],
                data: {
                    pageTitle: 'Leg Planning'
                }
            })
            .state('fleet', {
                url: '/fleet?exttkn',
                controller: 'FleetCtrl',
                templateUrl: 'html-partials/fleet.html',
                systemPortionsAny: [window.enums.systemPortion.generalUsage, window.enums.systemPortion.clientPortalUser],
                data: {
                    pageTitle: 'Fleet View'
                }
            })
            .state('runroute', {
                url: '/run/{jobID:[0-9]+}/route',
                controller: 'RunRouteCtrl',
                templateUrl: 'html-partials/run-route.html',
                systemPortionsAll: [window.enums.systemPortion.generalUsage],
                data: {
                    pageTitle: 'Run Route'
                }
            })
            .state('scheduleresource', {
                url: '/scheduleresource',
                controller: 'ScheduleResourceCtrl',
                templateUrl: 'html-partials/schedule-resource.html',
                systemPortionsAll: [window.enums.systemPortion.generalUsage, window.enums.systemPortion.plan],
                data: {
                    pageTitle: 'Schedule Resources'
                }
            });

        $provide.decorator('$exceptionHandler', ['$delegate', 'errorLoggerService', function ($delegate, errorLoggerService) {
            return function (exception, cause) {
                $delegate(exception, cause);
                errorLoggerService.logError(exception.message, exception.stack);
            };




        }]);



        $httpProvider.interceptors.push('authInterceptorService');
        cfpLoadingBarProvider.includeSpinner = false;

       
    }
]);

moment.locale('en', {
    calendar: {
        lastDay: '[Yesterday] HH:mm',
        sameDay: '[Today] HH:mm',
        nextDay: '[Tomorrow] HH:mm',
        lastWeek: 'ddd DD/MM/YYYY HH:mm',
        nextWeek: 'ddd DD/MM/YYYY HH:mm',
        sameElse: 'ddd DD/MM/YYYY HH:mm'
    }
});

