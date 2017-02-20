'use strict';

angular.module('peApp').factory('ContextMenuService', function () {
    return {
        element: null,
        menuElement: null
    };
}).directive('prScheduler', ['apiService', '$window', '$compile', '$interpolate', '$timeout', '_', 'ContextMenuService', '$document', function (apiService, $window, $compile, $interpolate, $timeout, _, ContextMenuService, $document) {
    return {
        restrict: 'E',
        template: '<div id="scheduler" class="dhx_cal_container pr-scheduler" style="width:100%;">'
                + '<div class="dhx_cal_navline">'
                + '<div class="dhx_cal_date"/>'
                + '</div>'
                + '<div class="responsive-cal-row">'
                + '<div class="pr-cal-date col-lg-3 col-lg-offset-4 col-md-5 col-md-offset-2 col-sm-5 col-xs-7 col-xs-offset-0"><span>{{displayDateRange}}</span></div>'
                + '<div class="col-md-5 col-sm-7  col-xs-5">'
                + '<div class="pr-cal-nav-buttons">'
                + '<div class="btn-group align-right">'
                + '<button id ="showRefreshButton" class="btn btn-group btn-default glyphicon glyphicon-refresh" ng-click="refresh()" ng-disabled="loading" title="Refresh your current view."></button>'
                + '<button id ="showNowButton" class="btn btn-group btn-default" ng-click="moveNow()" title="Move back to current time." >Now</button>'
                + '<button id="btnMoveBackwards" class="btn btn-group btn-default btn-with-dropdown glyphicon glyphicon-chevron-left" ng-click="showBackMenu()" title="Move back.">'
                + '<ul id="moveBackwardsDropdown" class="dropdown-menu dropdown-menu-right {{showMoveBackMenu ? \'open-dropdown-menu\' : \'\'}}" role="menu">'
                + '<li><a ng-click="moveBackwards(-1)" class="pointer dropdown-item" role="menuitem">Back by {{getMenuItemText(1)}}</a></li>'
                + '<li><a ng-click="moveBackwards(-2)" class="pointer dropdown-item" role="menuitem">Back by {{getMenuItemText(2)}}</a></li>'
                + '<li><a ng-click="moveBackwards(-6)" class="pointer dropdown-item" role="menuitem">Back by {{getMenuItemText(6)}}</a></li>'
                + '</ul>'
                +'</button>'
                + '<button id="btnMoveForwards" class="btn btn-group btn-default btn-with-dropdown glyphicon glyphicon-chevron-right" ng-click="showForwardMenu()" title="Move forwards.">'
                + '<ul id="moveForwardsDropdown" class="dropdown-menu dropdown-menu-right {{showMoveForwardMenu ? \'open-dropdown-menu\' : \'\'}}" role="menu">'
                + '<li><a ng-click="moveForwards(1)" class="pointer dropdown-item" role="menuitem">Ahead by {{getMenuItemText(1)}}</a></li>'
                + '<li><a ng-click="moveForwards(2)" class="pointer dropdown-item" role="menuitem">Ahead by {{getMenuItemText(2)}}</a></li>'
                + '<li><a ng-click="moveForwards(6)" class="pointer dropdown-item" role="menuitem">Ahead by {{getMenuItemText(6)}}</a></li>'
                + '</ul>'
                + '</button>'
                + '</div>'
                + '</div>'
                + '<div class="pr-cal-time-mode-control visible-lg visible-md visible-sm hidden-xs" ></div>'
                + '</div>'
                + '</div>'
                + '<div class="dhx_cal_header"></div>'
                + '<div class="dhx_cal_data"></div>'
                + '</div>',
        scope: {
            config: '=',
            timelineFilter: '=',
            yData: '=',
            data: '=',
            forceReloadData: '=',
            unavailability: '=',
            fromDateTime: '=',
            toDateTime: '=',
            onInternalDrop: '&',
            onExternalDrop: '&',
            onColumnToggle: '&',
            allowDropOnRowHeaders: '@',
            onEventDoubleClick: '&',
            conflictDetect: '&',
            dateRangeFormatter: '&',
            dropType: '@',
            loading: '=',
            processingYData: '=',
            processingData: '=',
            testState: '=',
            control: '=',
            displayEventTooltips: '=', // used to switch off the display of tooltips
            showNowButton: '@',
            onRefresh: '&',
            onContextMenu: '&',
            onDoubleClick: '&',
            showContextMenu:'@', 
            dataTarget: '@'
        },
        replace: true,
        transclude: true,
        link: function (scope, element, attrs, ctrl, transclude) {
            var draggedEventID;
            var draggedEventCopy;
            var allowInternalDragDrop = attrs.onInternalDrop;
            var allowExternalDragDrop = attrs.onExternalDrop;
            var handleDoubleClick = attrs.onEventDoubleClick;
            var shouldCheckForConflicts = attrs.conflictDetect;
            var useDateRangeFormatter = attrs.dateRangeFormatter;
            var useColumnHeaders = attrs.useColumnHeaders;
            var emptyCellCountTooltip = attrs.emptyCellCountTooltip;
            var schedulerHeight = attrs.schedulerHeight;
            var useProcessingYData = !!attrs.processingYData;
            var useProcessingData = !!attrs.processingData;
            var allowContextMenu = attrs.onContextMenu;
            var dataElement = element.find('.dhx_cal_data');
            var allowDoubleClick = attrs.onDoubleClick;
            var target = attrs.target;

            var schedulerAttachedEventIDs = [];

            //// === transcluded templates === ////

            var yDataTemplate = null;
            var yDataScopes = {};
            var yDataTemplateDeferredRenderCounts = {};
            var oldYData = [];

            var eventTemplate = null;
            var eventScopes = {};
            var eventDataTemplateDeferredRenderCounts = {};
            var oldEventData = [];
            var oldUnavailability = [];

            var eventTooltipTemplate = null;
            var timeModeControlTemplate = null;

            var duringDrag = false;
            var dropTargets = null;
            
            var columnHeaderBadges = [];

            var showNowButton = (scope.showNowButton === 'true');
            var showContextMenu = (scope.showContextMenu === 'true');

            scope.columnHeaderToggles = [];

            var onRefresh = attrs.onRefresh;

            transclude(function (clone) {
                var getTemplateHtml = function (selector) {
                    return clone.filter(selector).wrap('<div>').parent().html();
                };

                yDataTemplate = getTemplateHtml('#yDataTemplate');
                eventTemplate = getTemplateHtml('#eventTemplate');
                eventTooltipTemplate = getTemplateHtml('#eventTooltipTemplate');
                timeModeControlTemplate = getTemplateHtml('#timeModeControlTemplate');
            });

            //// === watches === ////
            scope.$watchCollection('yData', function (yData) {
                // Only re-render if the collection has actually changed
                if (!yData || angular.equals(oldYData, yData)) {
                    return;
                }

                loadYData(yData);
            });

            scope.$watchCollection('data', function (collection) {
                // Only re-render if the collection has actually changed
                if (duringDrag || !collection || angular.equals(oldEventData, collection)) {
                    return;
                }

                loadData(collection);
            });

            scope.$watch('forceReloadData', function (value) {
                if (value) {
                    loadData(scope.data);
                    scope.forceReloadData = false;
                }
            });

            var loadYData = function (yData) {
                setProcessingYData(true);

                $timeout(function () {
                    // Clear any existing yData child scopes
                    angular.forEach(yDataScopes, function (yDataScope) {
                        yDataScope.$destroy();
                    });
                    yDataScopes = {};
                    yDataTemplateDeferredRenderCounts = {};

                    // take a *copy* of the new collection as the scheduler can subsequently add properties
                    oldYData = angular.copy(yData);
                    scheduler.updateCollection('yData', yData);

                    if (useColumnHeaders) {
                        generateColumnHeader();
                    }

                    setProcessingYData(false);
                }, 10);
            };

            var loadData = function (data) {
                setProcessingData(true);

                $timeout(function () {
                    // Clear any existing event child scopes
                    angular.forEach(eventScopes, function (eventScope) { eventScope.$destroy(); });
                    eventScopes = {};
                    eventDataTemplateDeferredRenderCounts = {};

                    // take a *copy* of the new data as the scheduler can subsequently add properties
                    oldEventData = angular.copy(data);
                    scheduler.clearAll();

                    angular.forEach(data, function (item) {
                        item.originalColor = item.color || '';
                    });

                    scheduler.parse(data, "json");

                    setProcessingData(false);
                }, 10);
            };

            scope.$watchCollection('unavailability', function (unavailability) {

                if (!angular.equals(oldUnavailability, unavailability)) {

                    // take a *copy* of the new collection as the scheduler can subsequently add properties
                    oldUnavailability = angular.copy(unavailability);

                    // Clear any existing marked timespans
                    scheduler.deleteMarkedTimespan();

                    angular.forEach(unavailability, function (item) {
                        scheduler.addMarkedTimespan({
                            days: item.day,
                            zones: item.times,
                            sections: { timeline: item.sectionKey, unit: item.sectionKey },
                            css: 'unavailable'
                        });
                    });

                    scheduler.updateView();
                }
            });

            scope.$watch('timelineFilter', function (filter) {
                scheduler.updateView();
            }, true);


            scope.$watch('fromDateTime', function (newFromDateTime) {
                if (newFromDateTime) {
                    if (newFromDateTime.getTime() !== scheduler.getState().min_date.getTime()) {
                        scope.displayDateRange = scheduler.templates[scheduler._mode + "_date"](scope.fromDateTime, scope.toDateTime, scheduler._mode);
                        scheduler.updateView(newFromDateTime);
                        scope.toDateTime = scheduler.getState().max_date;
                    }
                }
            });

            scope.$watch('loading', function (value) {
                updateLoading();
            }, true);

            scope.$watch('config.selectedTimeMode', function (value) {
                if (value != oldTimelineMode) {

                    var timelineMode = _.find(scope.config.timeModes, function (item) {
                        return item.name === scope.config.selectedTimeMode;
                    });

                    var timeline = scheduler.matrix['timeline'];
                    timeline.x_unit = timelineMode.xUnit;
                    timeline.x_date = timelineMode.xDateFormat,
                    timeline.x_step = timelineMode.xStep;
                    timeline.x_size = timelineMode.xSize;
                    timeline.x_start = timelineMode.xStart;
                    timeline.x_length = timelineMode.xLength || 1;

                    scheduler.templates["timeline_scale_date"] = scheduler.date.date_to_str(timelineMode.xDateFormat);
                    setStartTimeBehaviour(timeline.x_unit);

                    scheduler.setCurrentView();
                    scope.toDateTime = scheduler.getState().max_date;

                    oldTimelineMode = value;
                }
            });

            scope.internalControl = scope.control || {};

            scope.internalControl.resetScheduler = function () {
                var state = scheduler.getState();
                scheduler.setCurrentView(state.date, state.mode);
                generateColumnHeader();
            };

            /// === scheduler event handlers === ///

            // Close the dropdown menu if the user clicks outside of it
            window.onclick = function (event) {
                if (!event.target.matches('.btn-with-dropdown')) {
                    scope.showMoveBackMenu = false;
                    scope.showMoveForwardMenu = false;
                    scope.$apply();
                }
            }

            var attachSchedulerEvent = function (name, handler) {
                schedulerAttachedEventIDs.push(scheduler.attachEvent(name, handler));
            };

            var detachAllSchedulerEvents = function () {
                for (var i = 0; i < schedulerAttachedEventIDs.length; i++) {
                    scheduler.detachEvent(schedulerAttachedEventIDs[i]);
                }

                schedulerAttachedEventIDs = [];
            };

            //Blank event handler added to prevent legs refreshing on onclick.
            attachSchedulerEvent('onClick', function (event) {                
            });

            attachSchedulerEvent('onEventCollision', function (event) {
                if (shouldCheckForConflicts) {
                    event.conflict = true;
                }

                return false;
            });

            attachSchedulerEvent('onViewChange', function (newEvent, newDate) {

                _.defer(function () {
                    scope.$apply(function () {
                        var state = scheduler.getState();

                        if (scope.fromDateTime.getTime() !== state.min_date.getTime()) {
                            scope.fromDateTime = state.min_date;
                            scope.columnHeaderToggles = [];
                        }

                        if (scope.toDateTime.getTime() !== state.max_date.getTime()) {
                            scope.toDateTime = state.max_date;
                            scope.columnHeaderToggles = [];
                        }

                        scope.displayDateRange = scheduler.templates[scheduler._mode + "_date"](scope.fromDateTime, scope.toDateTime, scheduler._mode);
                       
                        if (useColumnHeaders) {
                            generateColumnHeader();
                        }

                    });
                });
            });

            if (useColumnHeaders) {
                attachSchedulerEvent('onXLE', function () {
                    generateColumnHeader();
                });
            }


            if (handleDoubleClick) {
                attachSchedulerEvent('onDblClick', function (id, e) {
                    var schedulerEvent = scheduler.getEvent(id);
                    scope.onEventDoubleClick({ schedulerEvent: schedulerEvent });
                });
            }


            if (allowInternalDragDrop) {

                // Used to get the event in onDragEnd as currently it doesn't pass the ID of the event
                attachSchedulerEvent('onBeforeDrag', function (id, mode, e) {
                    //take a copy of the event
                    draggedEventID = id;
                    duringDrag = true;
                    draggedEventCopy = angular.copy(scheduler.getEvent(draggedEventID));
                    return blockReadOnly(id);
                });

                attachSchedulerEvent('onEventDrag', function (id, mode, e) {
                    return true;
                });

                attachSchedulerEvent('onDragEnd', function () {
                    var schedulerEvent = scheduler.getEvent(draggedEventID);
                    duringDrag = false;
                    // only consider the event a drop if the dragged events properties have changed
                    if (!angular.equals(schedulerEvent, draggedEventCopy)) {
                        scope.onInternalDrop({ schedulerEvent: schedulerEvent });
                        removeDragOverClassForAllCells();
                    }

                });
            }

            if (true)
            {
                attachSchedulerEvent('onLightbox', function (id) {
                    alert(id);

                    return false;
                });

                attachSchedulerEvent('onBeforeLightbox', function (id) {
                    var event = scheduler.getEvent(id);

                    scope.onDoubleClick({
                    resourceSchedule: resourceSchedule
                });

                    return false;
                });
            }


            if (allowDoubleClick)
            {
                attachSchedulerEvent('onCellDblClick', function (x_ind, y_ind, x_val, y_val, e) {
                    var section = scheduler.matrix.timeline.y_unit[scheduler._locate_cell_timeline(e).y];
                    var resourceSchedule = {
                        id: -1,
                        resourceId: section.driver.resourceID,
                        fullName: section.driver.fullName,
                        startDateTime: x_val,
                        endDateTime: null,
                        comments: ''
                    }
                    scope.onDoubleClick({ resourceSchedule: resourceSchedule });
                });

            }

            if (allowExternalDragDrop) {
                dataElement.addClass('allow-external-drag');

                dropTargets = '.dhx_matrix_cell,.dhx_marked_timespan.unavailable';
                if (scope.allowDropOnRowHeaders === 'true') dropTargets += ',.dhx_matrix_scell';

                element.on('dragover.pr-scheduler', dropTargets, function (e) {

                    if (e.preventDefault) {
                        e.preventDefault(); // preventDefault is required on dragover otherwise the drop e doesn't fire.
                    }
                    e.dataTransfer.dropEffect = 'move';

                });

                element.on('dragenter.pr-scheduler', dropTargets, function (e) {
                    removeDragOverClassForAllCells();
                    this.classList.add('external-drag-over');
                });

                element.on('dragleave.pr-scheduler', dropTargets, function (e) {
                    this.classList.remove('external-drag-over');
                });

                element.on('drop.pr-scheduler', function (e) {

                    var dropData = JSON.parse(e.dataTransfer.getData('text'));

                    if (!scope.dropType || dropData.dropType == scope.dropType) {
                        var dropLocation = scheduler.getActionData(e.originalEvent);

                        var dropMessage = { dropData: dropData.data, dropDateTime: dropLocation.date, schedulerSection: dropLocation.section };

                        if (e.target.classList.contains('dhx_matrix_scell')) {
                            dropMessage.isRowHeaderDrop = true;
                        }
                        scope.onExternalDrop(dropMessage);
                        removeDragOverClassForAllCells();

                    }
                });
            }

            if (!showNowButton) {
                element.find('#showNowButton').remove();
            }

            if (!onRefresh)
                element.find('#showRefreshButton').remove();
            

            /// === scheduler functions === ////


            // If the scheduler is being displayed in 'hour' mode then allow the start point to be a specific hour,
            // rather than the default behaviour which is always to start from the beginning of the day.
            var setStartTimeBehaviour = function (xUnit) {
                if (xUnit === 'hour') {
                    scheduler.date.timeline_start = function (date) {
                        return moment(date).startOf('hour').toDate();
                    };
                }
                else {
                    scheduler.date.timeline_start = function (date) {
                        return moment(date).startOf('day').toDate();
                    };
                }
            };

            var timelineMode = _.find(scope.config.timeModes, function (item) {
                    return item.name === scope.config.selectedTimeMode;
            });

            var oldTimelineMode = scope.config.selectedTimeMode;

            scheduler.createTimelineView({
                name: 'timeline',
                x_unit: timelineMode.xUnit, //measuring unit of the X-Axis.
                x_date: timelineMode.xDateFormat, //date format of the X-Axis
                x_step: timelineMode.xStep, //X-Axis step in 'x_unit's
                x_size: timelineMode.xSize, //X-Axis length specified as the total number of 'x_step's
                x_start: timelineMode.xStart, //X-Axis offset in 'x_unit's
                x_length: timelineMode.xLength || 1, //number of 'x_step's that will be scrolled at a time
                y_unit: scheduler.serverList('yData', []),
                y_property: scope.config.yMapProperty, //mapped data property
                render: 'bar', //view mode
                dy: scope.config.dy,
                dx: scope.config.dx,
                section_autoheight: false,
                event_min_dy: scope.config.yEventHeight
            });

            scope.config.selectedTimeMode = timelineMode.name;
            setStartTimeBehaviour(timelineMode.xUnit);
          
 
            scheduler.templates.event_class = function (start, end, event) {
                if (event.color !== '#c8d5e3' && shouldCheckForConflicts) {

                    // Note that collision detection is O(n^2). Simple brute force approach.
                    // Possible to improve to O(n Log n) by building a 2N array of start/end times, sorting them by datetime
                    // and then looking for consecutive starts.
                    scheduler.checkCollision(event);
                    checkForConflicts(event);

                    if (event.conflict) {
                        event.color = 'red';
                        event.conflict = false;
                    } else
                        event.color = '';
                }

                return '';
            };

            var templateInterpolate = function (interpolateFunction, scope) {
                var retVal = interpolateFunction(scope);

                // Facilitate use of ng-attr-* attributes in interpolated templates: workaround for the fact that $interpolate
                // will interpolate the content of an ng-attr-* attribute, but will not strip "ng-attr-" from the attribute name.
                retVal = retVal.replace(/\s+ng-attr-(\w+=".+")/gi, ' $1');

                return retVal;
            };

            if (eventTooltipTemplate) {
                var eventTooltipTemplateInterpolate = $interpolate(eventTooltipTemplate);

                scheduler.templates.tooltip_text = function (start, end, ev) {
                    if (scope.displayEventTooltips === false) {
                        return null;
                    }

                    var eventTooltipScope = scope.$new();
                    angular.extend(eventTooltipScope, ev);
                    
                    var html = templateInterpolate(eventTooltipTemplateInterpolate, eventTooltipScope);
                    eventTooltipScope.$destroy();
                    return html;
                };
            }

            if (eventTemplate) {
                var eventTemplateInterpolate = $interpolate(eventTemplate);
                var eventTemplateCompile = $compile(eventTemplate);

                scheduler.templates.event_bar_text = function (start, end, event) {
                    // Create a child scope with the data for this event and a placeholder for the injected template.
                    // We use a placeholder because the scheduler component requires an html string here, and therefore this can't be properly compiled and data-bound at this point.
                    // We will inject the template in the onScaleAdd event handler.
                    if (eventScopes[event.id] === undefined) {
                        var childScope = scope.$new();
                        angular.extend(childScope, event);
                        eventScopes[event.id] = childScope;
                    }

                    // interpolate the template (i.e. get the html, no interactivity)
                    var placeholderTemplate = templateInterpolate(eventTemplateInterpolate, eventScopes[event.id]);

                    return '<div class="event-placeholder" id="' + event.id + '">' + placeholderTemplate + '</div>';
                };

                attachSchedulerEvent('onScaleAdd', function (unit, key) {

                    var eventPlaceholders = $(unit).find('.event-placeholder');

                    // keep track of the requests to render the template ensuring only final request actually causes a compile
                    if (eventDataTemplateDeferredRenderCounts[key] !== undefined) eventDataTemplateDeferredRenderCounts[key]++;
                    else (eventDataTemplateDeferredRenderCounts[key]) = 0;
                    var thisRenderCount = eventDataTemplateDeferredRenderCounts[key];

                    if (eventPlaceholders.length > 0) {
                        _.defer(function () {
                            if (thisRenderCount != eventDataTemplateDeferredRenderCounts[key]) return;
                            eventPlaceholders.each(function () {
                                // Inject the template bound to the data for this event
                                var eventPlaceholder = $(this);
                                var eventId = eventPlaceholder.attr('id');
                                var childScope = eventScopes[eventId];

                                if (childScope !== undefined) {
                                    // Fixes bug which leaves drag handles on events even when they are set to readonly.
                                    if (childScope.readonly)
                                        eventPlaceholder.parent().find('.dhx_event_resize').remove();
                                        
                                    var compiled = eventTemplateCompile(childScope, function () { });
                                    eventPlaceholder.html(compiled);
                                    childScope.$digest();
                                }
                            });
                        });
                    }
                });
            }
            else {
                scheduler.templates.event_bar_text = function (start, end, event) {
                    var text = event.text;

                    if (parseInt(scheduler.getState().drag_id, 10) === event.id) {
                        text += ', <b>' + scheduler.templates.event_header(start, end, event) + '</b>';
                    }

                    return text;
                };
            }

            if (yDataTemplate) {

                var yDataTemplateInterpolate = $interpolate(yDataTemplate);
                var yDataTemplateCompile = $compile(yDataTemplate);

                scheduler.templates.timeline_scale_label = function (sectionId, sectionLabel, sectionData) {
                    // Create a child scope with the data for this y-axis label and a placeholder for the injected template.
                    // We use a placeholder because the scheduler component requires an html string here, and therefore this can't be properly compiled and data-bound at this point.
                    if (yDataScopes[sectionId] === undefined) {
                        var childScope = scope.$new();
                        angular.extend(childScope, sectionData);
                        yDataScopes[sectionId] = childScope;
                    }

                    // interpolate the template (i.e. get the html, no interactivity)
                    var placeholderTemplate = templateInterpolate(yDataTemplateInterpolate, yDataScopes[sectionId]);

                    return '<div class="y-template-placeholder">' + placeholderTemplate + '</div>';
                };


                attachSchedulerEvent('onScaleAdd', function (unit, key) {


                    // Inject the template bound to the data for this y-axis label
                    var childScope = yDataScopes[key];
                    var placeholder = $(unit).parents('tr').find('.y-template-placeholder');

                    // keep track of the requests to render the template ensuring only final request actually causes a compile
                    if (yDataTemplateDeferredRenderCounts[key] !== undefined) yDataTemplateDeferredRenderCounts[key]++;
                    else (yDataTemplateDeferredRenderCounts[key]) = 0;

                    var lastRenderCount = yDataTemplateDeferredRenderCounts[key];

                    if (childScope !== undefined && placeholder.length > 0) {
                        _.defer(function () {
                            if (lastRenderCount != yDataTemplateDeferredRenderCounts[key]) return;
                            var compiled = yDataTemplateCompile(childScope, function () { });
                            placeholder.html(compiled);
                            childScope.$digest();
                        });
                    }
                });
            }


            if (timeModeControlTemplate) {

                var timeModeControl = $compile(timeModeControlTemplate)(scope);
                var placeholder = $(element).find('.pr-cal-time-mode-control');
                placeholder.html(timeModeControl);
                
            }

            if (attrs.clickHandlers && scope.clickHandlers.length > 0) {
                angular.forEach(scope.clickHandlers, function (clickHandler) {
                    dataElement.on('click', clickHandler.selector, clickHandler.handler);
                });
            }

            if (useDateRangeFormatter) {
                scheduler.templates.timeline_date = function (dateA, dateB) {
                    return scope.dateRangeFormatter({ dateA: dateA, dateB: dateB });
                };
            }

            scheduler.filter_timeline = function (id, event) {
                if (scope.timelineFilter && scope.timelineFilter.length > 0) {
                    var rx = RegExp(scope.timelineFilter, 'i');

                    if (event.text.search(rx) === -1 && !event.readonly) {
                        event.color = '#c8d5e3';
                    } else {
                        event.color = event.originalColor;
                    }
                } else {
                    event.color = event.originalColor;
                }

                return true;
            };

            scope.showMoveBackMenu = false;
            scope.showMoveForwardMenu = false;

            scope.getMenuItemText = function (step) {
                if (scope.config.selectedTimeMode === 'week') {
                    if (scope.config.type === 'resourceUnits')
                        return step + (step === 1 ? ' week' : ' weeks');
                    return step + (step === 1 ? ' day' : ' days');
                }
                else {
                    if (scope.config.selectedTimeMode === '72hour') {
                        return step * 3 + ' hours';
                    }
                    else if (scope.config.selectedTimeMode === '48hour') {
                        return step * 2 + ' hours';
                    }
                    else {
                        return step + (step === 1 ? ' hour' : ' hours');
                    }
                }
            }

            scope.showBackMenu = function () {
                scope.showMoveBackMenu = true;
                scope.showMoveForwardMenu = false;
            }

            scope.showForwardMenu = function () {
                scope.showMoveBackMenu = false;
                scope.showMoveForwardMenu = true;
            }

            scope.moveNow = function () {
                scheduler._click.dhx_cal_today_button();
            };

            scope.moveForwards = function (step) {
                var dummy = 0;
                scheduler._click.dhx_cal_next_button(dummy, step);
            };

            scope.moveBackwards = function (step) {
                var dummy = 0;
                scheduler._click.dhx_cal_next_button(dummy, step);
            };

            scope.refresh = function () {
                scope.onRefresh();
            }

            /// === helper functions === ///

            /// The dthmlx scheduler shrinks the scheduler to fit in the current browser window. 
            /// When the sceduler is particularly large this can mean two  vertical scroll bars
            /// This function sets various elements of the schduler to be as large enough to display 
            /// all the row in its table.
            var resizeSchedulerToFullHeight = function () {
                // table inside the 'dhx_cal_data' div has the "correct" height 
                var cal_data = element[0].getElementsByClassName('dhx_cal_data')[0];
                var dataTable = cal_data.children[0];
                if (dataTable.clientHeight > 0) {
                    // set the heights of the data section and the overall element
                    angular.element(cal_data).height(dataTable.clientHeight + 50);
                    angular.element(element).height(dataTable.clientHeight + 150);
                }
            };

            var checkForConflicts = function (thisEvent) {
                if (!shouldCheckForConflicts) {
                    return;
                }

                var events = scheduler.getEvents();

                var collision = _.some(events, function (event) {
                    return scope.conflictDetect({ event1: thisEvent, event2: event });
                });

                if (collision)
                    thisEvent.conflict = true;
            };

            var removeDragOverClassForAllCells = function () {
                element.find('.dhx_matrix_cell, .dhx_matrix_scell').removeClass('external-drag-over');
            };

            var setProcessingYData = function (value) {
                if (useProcessingYData) {
                    scope.processingYData = value;
                    updateLoading();
                }
            };

            var setProcessingData = function (value) {
                if (useProcessingData) {
                    scope.processingData = value;
                    updateLoading();
                }
            };

            var updateLoading = function () {
                var isLoading = scope.loading || scope.processingYData || scope.processingData;
                dataElement.toggleClass('loading', isLoading);
            };

            var blockReadOnly = function (id) {
                return !id || !scheduler.getEvent(id).readonly;
            };

            var generateColumnHeader = function () {
                var yUnitID = scope.config.yMapProperty;
                var minDate = scheduler.getState().min_date;
                var maxDate = scheduler.getState().max_date;
                var events = scheduler.getEvents(minDate, maxDate);

                if (scope.columnHeaderToggles.length == 0) {
                    var columnSize = $('.dhx_scale_bar').length;
                    while (columnSize--) scope.columnHeaderToggles[columnSize] = false;
                }

                var header = $('.dhx_cal_header').find('.badge');

                if (header.length == 0) {
                    angular.forEach(columnHeaderBadges, function (badges) { badges.unbind('click'); });
                    columnHeaderBadges = [];
                }


                $('.dhx_scale_bar').each(function (index, value) {
                    var date = moment(scheduler.getState().min_date);
                    var dayStart = date.clone().add(index, 'days').toDate();
                    var nextDayStart = date.clone().add(index + 1, 'days').toDate();

                    var yData = scope.yData || [];

                    var emptyCellCount = yData.reduce(function (previous, current, index, array) {
                        var eventsForThisCell = events.filter(function (ev) {
                            return ev[yUnitID] === parseInt(current.key, 10) && ev.start_date < nextDayStart && ev.end_date >= dayStart;
                        });

                        return eventsForThisCell.length > 0 ? previous : previous + 1;
                    }, 0);

                    var badge = $(this).find('.badge');

                    var badgeClass = (scope.columnHeaderToggles[index]) ? 'badge scheduler-empty-cell-count mouse-cursor-click badge-toggle-on' : 'badge scheduler-empty-cell-count mouse-cursor-click';

                    if (badge.length == 0) {

                        badge = $('<span/>', {
                            html: emptyCellCount,
                            title: emptyCellCountTooltip,
                            'class': badgeClass
                        });

                        badge.on('click', function (e) {
                            scope.columnHeaderToggles[index] = !scope.columnHeaderToggles[index];
                            $(this).toggleClass('badge-toggle-on');
                            scope.onColumnToggle({ dayDate: dayStart });
                        });

                        $(this).append(badge).attr('tooltip', emptyCellCountTooltip);

                        columnHeaderBadges.push(badge);
                    }
                    else {
                        badge.html(emptyCellCount);
                        badge.attr('class', badgeClass);
                    }

                });
            };


            /// === scheduler initialization === ///

            scheduler.skin = 'flat';
            scheduler.init(element[0], scope.fromDateTime, 'timeline');
            scheduler.config.mark_now = true;
            scheduler.config.time_step = 15;
            scheduler.config.dblclick_create = false;
            scheduler.config.drag_create = false;
            scheduler.config.collision_limit = 1;

            element.height(schedulerHeight);

            scope.toDateTime = scheduler.getState().max_date;

            // give access to internals if testing
            if (scope.testState) {
                scope.testState.scheduler = scheduler;
            }

            scope.$on('$destroy', function () {
                detachAllSchedulerEvents();
                scheduler.clearAll();
            });
        }
    };
}]);