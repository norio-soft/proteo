'use strict';

angular.module('proteo.shared.api').factory('apiService', ['$resource', '$rootScope', function ($resource, $rootScope) {
    var service = {};

    var url = function (relativeUrl) {
        return $rootScope.apiBaseUrl + '/api/' + relativeUrl;
    };

    service.account = $resource(url('account'), null, {
        canAccessAny: {
            method: 'GET',
            url: url('account/canaccessany'),
            params: { systemPortionIDs: '@systemPortionIDs' }
        },
        canAccessAll: {
            method: 'GET',
            url: url('account/canaccessall'),
            params: { systemPortionIDs: '@systemPortionIDs' }
        }
    });

    service.error = $resource(url('error/javascript'), null, {
        log: {
            method: 'POST'
        }
    });

    service.driver = $resource(url('driver'), null, {
        getDrivers: {
            method: 'GET',
            isArray: true,
        },
        getDriver:{
            method: 'GET',
            url:url('/driver/:driverResourceID')
        },
        updateDriver: {
            method: 'PUT',
        },
        getTodaySummary: {
            method: 'GET',
            url: url('driver/todaysummary'),
        },
        getDaySummary: {
            method: 'GET',
            url: url('driver/daysummary'),
        },
        assignVehicle: {
            method: 'PUT',
            url: url('driver/:driverResourceID/assignvehicle/:vehicleResourceID'),
        },
        updateTravelNotes: {
            method: 'PUT',
            url: url('driver/updateTravelNotes')
        },
    });

    service.driverRequests = $resource(url('driver/driverRequest'), null, {
        getDriverRequests: {
            method: 'GET',
            isArray: true
        },
        update: {
            method: 'PUT'
        },
        create: {
            method: 'POST'
        }
    });

    service.driverDebriefs = $resource(url('driver/driverDebrief'), null, {
        getDriverDebriefs: {
            method: 'GET',
            isArray: true
        },
        create: {
            method:'POST'
        },
        remove: {
            method: 'DELETE',
            url:url('driver/driverDebrief/:id')
        }
    });

    service.sendMwfInstruction = $resource(url('driver/sendMWFInstruction'), null, {
        create: {
            method: 'POST'
        }
    });

    service.updateTravelNotes = $resource(url('driver/updateTravelNotes'), null, {
        update: {
            method: 'PUT'
        }
    });

    service.vehicle = $resource(url('vehicle'), null, {
        getVehicles: {
            method: 'GET',
            isArray: true,
        },
        getCurrentPosition: {
            method: 'GET',
            url: url('vehicle/:vehicleResourceID/currentposition'),
        },
    });

    service.assignUsualVehicles = $resource(url('resourceunit/UsualVehicles'), null, {
        assignUsualVehicles: {
            method: 'POST',
            params: { startOfWeek: '@startOfWeek', allDrivers: '@allDrivers' },
            isArray: true
        }
    });

    service.copyLastWeek = $resource(url('resourceunit/CopyLastWeek'), null, {
        copyLastWeek: {
            method: 'POST',
            params: {

                fromDateTime: '@fromDateTime',
                toDateTime: '@toDateTime',
                excludeResourcesStartingBeforeFromDate: '@excludeResourcesStartingBeforeFromDate',
                includeAllResourceUnits: '@includeAllResourceUnits',
                excludeDuplicates: '@excludeDuplicates'
            }
        }
    });

    service.interSite = $resource(url('intersite'), null, {
        getInterSiteCharge: {
            method: 'GET',
            url: url('intersite/interSiteCharge')
        },
        addCurrentInterSiteCharge: {
            method: 'POST',
            url: url('intersite/currentInterSiteCharge')
        },
        addCurrentControlAreaInterSiteCost: {
            method: 'POST',
            url: url('intersite/currentInterSiteCost')
        },
        addFutureControlAreaInterSiteCost: {
            method: 'POST',
            url: url('intersite/futureInterSiteCost')
        },
        addFutureInterSiteCharge: {
            method: 'POST',
            url: url('intersite/futureInterSiteCharge')
        },
        getControlAreaInterSiteCosts: {
            method: 'GET',
            url: url('intersite/interSiteCost/:controlAreaId')
        }
    });

    service.resourceUnit = $resource(url('resourceunit'), null, {
        create: {
            method: 'POST',
            isArray: true,
            transformResponse: function (data, header) {
                $rootScope.$emit("pe-app-resource-units-updated", data);
                return angular.fromJson(data);
            }
        },

        getResourceUnits: {
            method: 'GET',
            isArray: true,
            transformResponse: function (data, header) {
                var deserialized = angular.fromJson(data);

                if (angular.isArray(deserialized)) {
                    angular.forEach(deserialized, function (item, idx) {
                        item.startDateTime = moment(item.startDateTime).toDate();
                        item.finishDateTime = moment(item.finishDateTime).toDate();
                    });
                }

                return deserialized;
            }
        },

        getResourceUnit: {
            method: 'GET',
            transformResponse: function (data, header) {
                var deserialized = angular.fromJson(data);

                if (angular.isArray(deserialized)) {
                    angular.forEach(deserialized, function (item, idx) {
                        item.startDateTimeDisplay = moment(item.startDateTime).format('DD/MM/YYYY HH:mm');
                        item.finishDateTimeDisplay = moment(item.finishDateTime).format('DD/MM/YYYY HH:mm');
                        item.startDate = moment(item.startDateTime).format();
                        item.startTime = moment(item.startDateTime).toDate();
                        item.finishDate = moment(item.finishDateTime).format();
                        item.finishTime = moment(item.finishDateTime).toDate();
                    });
                }

                return deserialized;
            }
        },

        getResourceUnitGroups: {
            url: url('resourceunit/group'),
            method: 'GET',
            isArray: true,
            params: { fromDateTime: '@fromDateTime', toDateTime: '@toDateTime', plannerIdentityID: '@plannerIdentityID' }
        },

        getUnconnectedDrivers: {
            url: url('resourceunit/unconnecteddrivers'),
            method: 'GET',
            isArray: true,
            params: { fromDateTime: '@fromDateTime', toDateTime: '@toDateTime', plannerIdentityID: '@plannerIdentityID' }
        },

        update: {
            method: 'PUT',
            transformResponse: function (data, header) {
                $rootScope.$emit("pe-app-resource-units-updated", data);
                return angular.fromJson(data);
            }
        },
    });

    service.legPlan = $resource(url('leg/plan'), null, {
        get: {
            method: 'GET',
            isArray: true,
            params: { startDateTime: '@startDateTime', endDateTime: '@endDateTime' }
        },
        planAndResource: {
            method: 'PATCH',
            params: { jobLastUpdateDateTime: '@jobLastUpdateDateTime', setPlannedTimesOnly: '@setPlannedTimesOnly' }
        }
    });

    service.legCommunication = $resource(url('leg/communication'), null, {
        quickCommunicate: {
            method: 'POST'
        },
        unCommunicate: {
            method: 'DELETE'
        }
    });

    service.legSubcontract = $resource(url('leg/subcontract'), null, {
        unSubcontract: {
            method: 'DELETE'
        }
    });

    service.resourceSchedule = $resource(url('resourceschedule'), null, {
        getVehicleResourceSchedules: {
            method: 'GET',
            url: url('resourceschedule/vehicle'),
            isArray: true
        },
        getForPlanning: {
            method: 'GET',
            url: url('resourceschedule/plan'),
            isArray: true
        },
        update: {
            method: 'PUT',
        },
        getActivityTypes:
            {
                method: 'GET',
                params:{resourceTypeId: '@resourceTypeId'},
                url: url('resourceschedule/activitytypes'),
                isArray: true
            },
        deleteResourceSchedule: {
            method: 'DELETE',
            url: url('resourceschedule/:id')
        },
    });

    service.driverStartTime = $resource(url('driver/:driverResourceID/starttime'), null, {
        get: {
            method: 'GET',
            params: { date: '@date' }
        },
        set: {
            method: 'PUT'
        }
    });

    service.todaysStartTimeForDrivers = $resource(url('driver/todaystarttimes'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.driverFinishTime = $resource(url('driver/:driverResourceID/finishtime'), null, {
        get: {
            method: 'GET',
            params: { date: '@date' }
        },
        set: {
            method: 'PUT'
        }
    });

    service.yesterdaysFinishTimeForDrivers = $resource(url('driver/yesterdayfinishtimes'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.job = $resource(url('job/:jobID'), null, {
        getInstructions: {
            url: url('job/:jobID/instructions'),
            method: 'GET',
            isArray: true,
        },
        getLegs: {
            url: url('job/:jobID/legs'),
            method: 'GET',
            isArray: true,
        },
        getLegsForRunRoute: {
            url: url('job/:jobID/legs/runroute'),
            method: 'GET',
            isArray: true,
        },
    });

    service.legHistory = $resource(url('leg/:startInstructionID/:endInstructionID/history'), null, {
        getLegHistory: {
            method: 'GET'
        }
    });

    service.driverType = $resource(url('driverType'), null, {
        getDriverTypes: {
            method: 'GET',
            isArray: true
        }
    });

    service.trafficArea = $resource(url('trafficArea'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.organisation = $resource(url('organisation'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.controlArea = $resource(url('controlArea'), null, {
        get: {
            method: 'GET',
            isArray: true
        },
        put: {
            method: 'PUT',
            url: url('controlArea/:controlAreaId'),
        },
        post: {
            method: 'POST',
        },
        addCustomer: {
            method: 'POST',
            url: url('controlArea/:controlAreaId/customer/:organisationId'),
        },
        removeCustomer: {
            method: 'DELETE',
            url: url('controlArea/:controlAreaId/customer/:organisationId'),
        }
    });

    service.businessType = $resource(url('businessType'), null, {
        get: {
            method: 'GET',
            isArray: true
        },

    });

    service.vehicleType = $resource(url('vehicleType'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.mwfCommunicationStatus = $resource(url('mwfCommunicationStatus'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.points = $resource(url('point/:pointId/:searchLocal/:searchTerm'), {
        pointId: '@pointId', searchLocal: '@searchLocal', searchTerm: '@searchTerm'
    });

    service.users = $resource(url('users'), null, {
        getForSystemPortion: {
            method: 'GET',
            isArray: true,
            params: { systemPortionID: '@systemPortionID' },
        },
    });

    service.gpsPosition = $resource(url('gpsPosition'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    service.gpsPositionHistory = $resource(url('gpsPositionHistory/:gpsUnitId'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    })

    service.gpsPositionLocationHistory = $resource(url('gpsPositionHistory'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });
    

    service.resourceViews = $resource(url('resourceView'), null, {
        get: {
            method: 'GET',
            isArray: true
        }
    });

    return service;

}]);
