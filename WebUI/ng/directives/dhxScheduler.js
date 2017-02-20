'use strict';

angular.module('peApp').directive('dhxScheduler', ["$compile", "$injector", function ( $compile , $injector ) {
    return {
        restrict: 'A',
        scope: false,
        transclude: true,
        template: '<div class="dhx_cal_navline" ng-transclude style="display: block;">' +
                    '</div><div class="dhx_cal_header"></div>' +
                    '<div class="dhx_cal_data"></div>',

        link: function ($scope, $element, $attrs, $controller) {
            //default state of the scheduler
            if (!$scope.scheduler)
                $scope.scheduler = {};
            $scope.scheduler.mode = $scope.scheduler.mode || "month";
            $scope.scheduler.date = $scope.scheduler.date || new Date();

            //watch data collection, reload on changes
            $scope.$watch(function () {
                return $scope.selectedResourceType + $scope.selectedDriverType + $scope.selectedDepot + $scope.showUnavailableResources;
            }, function (dummy) {
                //trigger refresh
                scheduler.setCurrentView($scope.scheduler.date, $scope.scheduler.mode);
            }, true);

            //watch mode and date
            $scope.$watch(function () {
                return $scope.scheduler.mode + $scope.scheduler.date.toString();    
            }, function (nv, ov) {
                var mode = scheduler.getState();
                if (nv.date != mode.date || nv.mode != mode.mode) {
                    scheduler.setCurrentView($scope.scheduler.date, $scope.scheduler.mode);
                }
            }, true);

            //size of scheduler
            $scope.$watch(function () {
                return $element[0].offsetWidth + "." + $element[0].offsetHeight;
            }, function () {
                scheduler.setCurrentView();
            });

            var refreshVisibleEvents = function () {
                if (!isNaN(scheduler.getState().min_date.valueOf())) {
                    var evs = scheduler.getEvents();
                    for (var i = 0; i < evs.length; i++) {
                        scheduler.updateEvent(evs[i].id);
                    }
                }
            };

            scheduler.attachEvent("onLightbox", function (id) {
                angular.element(document.querySelector( '[name="isallday"]' )).attr('ng-model', 'isallday');
                angular.element( document.querySelector( ".dhx_section_time" )
                .getElementsByTagName("select") ).attr('ng-class', '{dhx_time_disable: isallday}').attr('ng-disabled', 'isallday');
                var el = angular.element(document.querySelector( ".dhx_cal_larea" ));    
                $injector = el.injector();
                $injector.invoke(['$compile', function($compile){
                  $compile(el)($scope)
                }]);
                return twerue; 
            });

            scheduler.attachEvent("onViewChange", function (mode, date) {
                if (!isNaN(scheduler.getState().min_date.valueOf())) {
                    var promise = $scope.loadEvents(scheduler.getState().min_date, scheduler.getState().max_date);
                    promise.then(function (result) {
                        scheduler.clearAll();
                        scheduler.parse(result, "json");
                    });
                    var activityPromise = $scope.loadActivities();
                    activityPromise.then(function (result) {
                        scheduler.config.lightbox.sections[1].options = result;
                        refreshVisibleEvents();
                    });

                    var resourcePromise = $scope.loadResources();
                    resourcePromise.then(function (result) {
                        scheduler.config.lightbox.sections[2].options = result;
                        refreshVisibleEvents();
                    });
                }
                
                return true;
            });

            scheduler.attachEvent("onBeforeViewChange", function (old_mode, old_date, mode, date) {
                //console.log(scheduler.getState().min_date, scheduler.getState().max_date, old_mode, old_date, mode, date );
/*
                if (!isNaN(scheduler.getState().min_date.valueOf())) {
                    var promise = $scope.loadEvents(scheduler.getState().min_date, scheduler.getState().max_date);
                    promise.then(function (result) {
                        scheduler.clearAll();
                        scheduler.parse(result, "json");
                    });
                    var activityPromise = $scope.loadActivities();
                    activityPromise.then(function (result) {
                        scheduler.config.lightbox.sections[1].options = result;
                        refreshVisibleEvents();
                    });
                    var resourcePromise = $scope.loadResources();
                    resourcePromise.then(function (result) {
                        scheduler.config.lightbox.sections[2].options = result;
                        refreshVisibleEvents();
                    });
                }
                */
                return true;
            });

            scheduler.attachEvent("onEventAdded", function (id, ev) {
                $scope.addEvent(parseInt(ev.resourceActivityTypeId),
                    parseInt(ev.resourceId),
                    0,
                    ev.start_date,
                    ev.end_date,
                    ev.comments);
            });

            scheduler.attachEvent("onEventChanged", function (id, ev) {
                $scope.updateEvent(ev.resourceScheduleId,
                    parseInt(ev.resourceActivityTypeId),
                    parseInt(ev.resourceId),
                    ev.instructionId,
                    ev.start_date,
                    ev.end_date,
                    ev.comments);
            });

            scheduler.attachEvent("onBeforeEventDelete", function (id, ev) {
                $scope.deleteEvent(ev.resourceScheduleId);
                return true;
            });

            scheduler.attachEvent("onTemplatesReady", function () {
                scheduler.templates.event_bar_text = function (start, end, event) {
                    if (scheduler.config.lightbox.sections[1].options != null && scheduler.config.lightbox.sections[2].options != null) {
                        return scheduler.getLabel("resourceId", parseInt(event.resourceId)) + " - " +
                            scheduler.getLabel("resourceActivityTypeId", parseInt(event.resourceActivityTypeId));
                    }
                    else return event.resourceDescription + " - " + event.activityDescription;
                };

                scheduler.templates.event_text = function (start, end, event) {
                    if (scheduler.config.lightbox.sections[1].options != null && scheduler.config.lightbox.sections[2].options != null) {
                        return scheduler.getLabel("resourceId", parseInt(event.resourceId)) + " - " +
                            scheduler.getLabel("resourceActivityTypeId", parseInt(event.resourceActivityTypeId));
                    }
                    else return event.resourceDescription + " - " + event.activityDescription;
                }

                var format = scheduler.date.date_to_str("%Y-%m-%d %H:%i");
                scheduler.templates.tooltip_text = function (start, end, event) {
                    return "<b>" + event.comments + "</b><br/>From <b>" +
                    format(start) + "</b> to <b>" + format(end) + "</b>";
                };

                scheduler.templates.event_class = function (start, end, event) {
                    return "my_event";
                };
            });

            scheduler.renderEvent = function (container, ev, width, height, header_content, body_content) {
                var container_width = container.style.width; // e.g. "105px"

                // move section
                var html = "<div class='dhx_event_move my_event_move' style='width: " + container_width + "'></div>";

                // container for event contents
                html += "<div class='my_event_body'>";
                html += "<span class='event_date'>";
                // two options here: show only start date for short events or start+end for long
                if ((ev.end_date - ev.start_date) / 60000 > 40) { // if event is longer than 40 minutes
                    html += scheduler.templates.event_header(ev.start_date, ev.end_date, ev);
                    html += "</span><br/>";
                } else {
                    html += scheduler.templates.event_date(ev.start_date) + "</span>";
                }
                // displaying event text
                html += "<span>" + scheduler.templates.event_text(ev.start_date, ev.end_date, ev) + "</span>";
                html += "</div>";

                // resize section
                html += "<div class='dhx_event_resize my_event_resize' style='width: " + container_width + "'></div>";

                container.innerHTML = html;
                return true; // required, true - we've created custom form; false - display default one instead
            };

            //styling for dhtmlx scheduler
            $element.addClass("dhx_cal_container");

            //customizations
            scheduler.locale.labels.section_text = 'Subject';
            scheduler.locale.labels.section_starttime = 'Duration';
            scheduler.locale.labels.section_isallday = 'All Day';
            scheduler.locale.labels.section_activitytype = 'Activity Type';
            scheduler.locale.labels.section_resourcename = 'Resource Name';
            scheduler.config.lightbox.sections = [
                { name: "text", height: 150, map_to: "comments", type: "textarea", focus: true },
                { name: "activitytype", map_to: "resourceActivityTypeId", type: "select" },
                { name: "resourcename", map_to: "resourceId", type: "select" },
                { name: "isallday", map_to: "allDay", type: "checkbox", height: 40 },
                { name: "starttime", height: 72, type: "time", map_to: "auto", time_format: ["%m", "%d", "%Y", "%H:%i"] }
            ];

            //scheduler.config.first_hour = 8;
            //scheduler.config.container_autoresize = true;
            scheduler.config.limit_time_select = true;
            scheduler.config.details_on_create = true;
            scheduler.config.details_on_dblclick = true;
            scheduler.config.server_utc = true;
            scheduler.config.drag_create = false;
            scheduler.config.icons_select = ["icon_details", "icon_delete"];
            scheduler.xy.min_event_height = 25;
            scheduler.config.delay_render = 30;
            console.log(scheduler);

            //init scheduler
            scheduler.init($element[0], $scope.scheduler.date, $scope.scheduler.mode);
          
        }
    }
}]);