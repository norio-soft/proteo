
$(document).ready(function () {
    myLayout = $('body').layout({
        north__size: 25,
        north__showOverflowOnHover: true,
        north__resizable: false,
        north__togglerLength_closed: -1,
        north__togglerLength_open: 0,
        north__spacing_open: 0,

        south__size: 20,
        south__resizable: false,
        south__spacing_open: 0,
        south__togglerLength_open: 0,
        south__togglerLength_closed: -1,
        
        onresize: function (name, el, state, options, layoutName) {
            if (name == "center") {
            }
        }
    });

    // configure scheduler
    scheduler.locale.labels.timeline_tab = 'Timeline'
    scheduler.locale.labels.section_custom = 'Section';
    scheduler.config.details_on_create = true;
    scheduler.config.details_on_dblclick = false;
    scheduler.config.xml_date = '%Y-%m-%d %H:%i';
    scheduler.config.show_loading = true;

    scheduler.config.first_hour = 0
    scheduler.config.last_hour = 23
    


});


$(function () {

  
    Orchestrator.WebUI.Services.VehiclePlanning.GetAllResourceUnits(
        function (results) {
            var compiledTemplate = getTemplate('resourceunit');
            //var html = compiledTemplate(ru);
            var sections = $.map(results, function (ru) {
                return {
                    key: ru.ResourceUnitID,
                    label: compiledTemplate(ru)
                };
            });



            scheduler.createTimelineView({
                name: 'timeline',
                x_unit: 'hour',
                x_date: '%H:%i',
                second_scale: {
                    x_unit: 'day',
                    x_date: '%F %d'
                },
                x_step: 1,
                x_size:24,
                x_start: 0,
                x_length: 14,
                y_unit: sections,
                y_property: 'section_id',
                render: 'bar',
                event_dy: 'full'
            });

            // turn off the default creation behaviours
            scheduler.config.dblclick_create = false;
            scheduler.config.drag_create = false;

            var startDate = moment();
            if (moment().day() != 1)
                startDate = moment().day((moment().day() - 1));

            scheduler.init('vehiclePlanningScheduler', startDate.startOf('day').toDate(), 'timeline');

            scheduler.attachEvent('onEventChanged', function (event_id, event_object) {
                showLoading();

                Orchestrator.WebUI.Services.VehiclePlanning.OrderPlanChange(event_object.orderID, event_object.instructionTypeID, event_object.instructionID, event_object.section_id, wcfDateFormatter(event_object.start_date),
                    function () {
                        loadSchedulerData().done(function () { hideLoading(); });
                    },
                    function (error) {
                        alert(error.get_message());
                        loadSchedulerData().done(function () { hideLoading(); });;
                    });
            });

            
            scheduler.attachEvent('onExternalDragIn', function (id, source) {
                var event = scheduler.getEvent(id);
            });

            
            // stop the scheduler showing the add Event form
            scheduler.attachEvent("onEmptyClick", function (date, native_event_object) {
                //any custom logic here
            });

            scheduler.attachEvent("onDblClick", function (date, native_event_object) {
                //any custom logic here
            });

            scheduler.attachEvent("onBeforeLightbox", function (event_id) {
                //any custom logic here
            });

            // handles the rendering of the events on the timeline
            //scheduler.templates.event_class = function (start, end, event) {
               
            //};

            scheduler.attachEvent("onClick", function (eventId, native_event_object) {
                _selectedEvent = eventId;
                $("#dialog-modal").dialog("open");
            });

      
            // handles the rendering of the contents of the tooltip
            scheduler.templates.tooltip_text = function (start, end, event) {
              
            };



        },
        function (error) {
            alert(error.get_message());
        });
});