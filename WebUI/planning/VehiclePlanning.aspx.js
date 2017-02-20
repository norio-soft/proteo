var myLayout; // a var is required because this page utilizes: myLayout.allowOverflow() method
var _selectedFilter = 0;
var _selectedEvent = -1;


Number.prototype.toCurrencyString = function (lcid) {
    var ret = Math.floor(this).toLocaleString() + (this % 1).toFixed(2).toLocaleString().replace(/^0/, '');
    if (lcid == undefined || lcid == null)
        lcid = 2057;

    var symbol = '£';
    switch (lcid) {
        case 2057: '£';
            break;
        default: '€';
    }
    return symbol + ret;
}


$(document).ready(function () {
    myLayout = $('#container').layout({
        west__showOverflowOnHover: false
    });
    $("#orderstoplan").accordion({ heightStyle: "fill", active:1 });
    $("#filteroptions").buttonset();
    $("input[name=filteroption]").click(function () {
        _selectedFilter = this.value;
        showLoading();
        $.when(loadOrders(_selectedFilter)).done(function () { hideLoading(); });
      
        $("#orderstoplan").accordion("option", "active", 1);
    });
    $(".driverpopup").css( 'cursor', 'help');
    $(".driverpopup").tooltip({ show: false });
    
    $("#dialog-modal").dialog({
        height: 140,
        modal: true,
        autoOpen: false,
        buttons: {
            "Remove from Plan": function () {
                $(this).dialog("close");
                //remove and reload
                removePrePlanEvent(_selectedEvent);
            },
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#showOrder').on("click", function () {
        var evt = scheduler.getEvent(_selectedEvent);
        var orderID = evt.orderID;
        dlgOrder_Open("oid=" + orderID);
    });

});



    
$(function () {

    $("#sortable").sortable();
    $("#sortable").disableSelection();

    scheduler.locale.labels.timeline_tab = 'Timeline'
    scheduler.locale.labels.section_custom = 'Section';
    scheduler.config.details_on_create = true;
    scheduler.config.details_on_dblclick = false;
    scheduler.config.xml_date = '%Y-%m-%d %H:%i';
    scheduler.config.show_loading = true;

    scheduler.config.first_hour = 6
    scheduler.config.last_hour = 23
    //scheduler.setLoadMode("month");


   
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
                x_step: 12,
                x_size: 14,
                x_start: 0,
                x_length: 14,
                y_unit: sections,
                y_property: 'section_id',
                render: 'bar',
                event_dy: 'full'
            });

            //scheduler.config.lightbox.sections = [
            //    { name: 'description', height: 130, map_to: 'text', type: 'textarea', focus: true },
            //    { name: 'custom', height: 23, type: 'select', options: sections, map_to: 'section_id' },
            //    { name: 'time', height: 72, type: 'time', map_to: 'auto' }
            //];

            // turn off the default creation behaviours
            scheduler.config.dblclick_create = false;
            scheduler.config.drag_create = false;

            var f = document.getElementById("dialog-modal");
            scheduler.showLightbox = function (id) {
                var ev = scheduler.getEvent(id);
                scheduler.startLightbox(id, f);
            }

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

            // intialise the Orders
            // init the Sortables
            $(".column").sortable({
                connectWith: $(".column")
                , placeholder: 'widget-placeholder'
                , cursor: 'move'
                //	use a helper-clone that is append to 'body' so is not 'contained' by a pane
                , helper: function (evt, ui) { return $(ui).clone().appendTo('body').show(); }
                , over: function (evt, ui) {
                    var
                        $target_UL = $(ui.placeholder).parent()
                        , targetWidth = $target_UL.width()
                        , helperWidth = ui.helper.width()
                        , padding = parseInt(ui.helper.css('paddingLeft'))
                                    + parseInt(ui.helper.css('paddingRight'))
                                    + parseInt(ui.helper.css('borderLeftWidth'))
                                    + parseInt(ui.helper.css('borderRightWidth'))
                    ;

                    ui.helper
                        .height('auto')
                        .width(targetWidth - padding)
                    ;
                }
            });

            loadOrders(0);
           

            scheduler.attachEvent("onViewChange", function (mode, date) {
                loadSchedulerData(date);
            });

            scheduler.attachEvent('onExternalDragIn', function (id, source) {
                var event = scheduler.getEvent(id);
            });

            var dateFrom = moment().startOf('day').toDate();
            loadSchedulerData(dateFrom);

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
            scheduler.templates.event_class = function (start, end, event) {
                var country = "";
                switch (event.lcid) {
                    case 2067: country = "dhx_cal_event_belgium"; break;
                    case 1029: country = "dhx_cal_event_chech"; break;
                    case 1036: country = "dhx_cal_event_france"; break;
                    case 1031: country = "dhx_cal_event_germany"; break;
                    default: //UK
                        country = "dhx_cal_event_uk";

                }
                var goodstype = "";
                switch (event.goodstypeid) {
                    case 5: goodstype  = "color-green";
                        break;
                    case 6: goodstype = "color-blue";
                        break;
                    case 7:  goodstype = "color-yellow";
                        break;
                }

                return country + " " + goodstype
            };

            scheduler.attachEvent("onClick", function (eventId, native_event_object) {
                _selectedEvent = eventId;
                $("#dialog-modal").dialog("open");
            });
           
            $("#dialog-modal").on("dialogclose", function (event, ui) {
                // reload if necessary
                var dateFrom = moment().startOf('day').toDate();
                loadSchedulerData(dateFrom);
            });
            // handles the rendering of the contents of the tooltip
            scheduler.templates.tooltip_text = function (start, end, event) {
                var format = scheduler.date.date_to_str("%d-%m-%Y");
                return "<b> " + event.customer + "<br/><b>Date</b> " + format(start) + "<br/><b>Collection:</b> " + event.collection + "<br/><b>Delivery:</b>" + event.delivery + "<br/><b>Rate:</b>" + event.rate;
            };


         
        },
        function (error) {
            alert(error.get_message());
        });
});

function showOrder(event) {
    var orderID = $(event).attr("data-orderid");
    dlgOrder_Open("oid=" + orderID);
}

function loadSchedulerData(date) {
    var $deferred = new $.Deferred();
    date = date || scheduler._date;

    var dateFrom = moment(date).startOf('day').add('days', -1).toDate();
    var dateTo = moment(date).startOf('day').add('days', 8).toDate();

    loadSchedules(date);

    Orchestrator.WebUI.Services.VehiclePlanning.GetVehicleOrderPlans(wcfDateFormatter(dateFrom), wcfDateFormatter(dateTo),
        function (results) {
            var schedulerDateFormat = 'YYYY-MM-DD HH:mm';

            var plans = $.map(results, function (plan) {
                return {
                    start_date: moment(plan.ArrivalDateTime).format(schedulerDateFormat),
                    end_date: moment(plan.ArrivalDateTime).add('days', 1).format(schedulerDateFormat),
                    text:GetPlanText(plan),
                    section_id: plan.ResourceUnitID,
                    orderID: plan.OrderID,
                    instructionTypeID: plan.InstructionType,
                    instructionID: plan.InstructionID,
                    lcid: plan.LCID,
                    customer: plan.Customer,
                    collection : plan.Delivery,
                    delivery : plan.Collection,
                    rate : plan.Rate,
                    rateLCID: plan.RateLCID,
                    goodstypeid: plan.GoodsTypeID,

                };
            });

            scheduler.clearAll();
            scheduler.parse(plans, 'json');
 $deferred.resolve();
            $('.dhx_matrix_cell').droppable({
                accept: '.unplanned-order',
                drop: function (event, ui) {
                    var actionData = scheduler.getActionData(event);
                    createPrePlan(ui.helper.context.id, actionData.section, actionData.date);
                },
                //over: function (event, ui) {
                //    $(this).css("background-color", "silver");
                //},
                //out: function (event, ui) {
                //    $(this).css("background-color", "#ffffff");
                //}
            });



           
        },
        function (error) {
            alert(error.get_message());
            $deferred.reject();
        });

    return $deferred;
}

function loadSchedules(date) {
    var $deferred = new $.Deferred();
    date = date || scheduler._date;

    var dateFrom = moment(date).startOf('day').add('days', -1).toDate();
    var dateTo = moment(date).startOf('day').add('days', 8).toDate();

    Orchestrator.WebUI.Services.VehiclePlanning.GetSchedules(wcfDateFormatter(dateFrom), wcfDateFormatter(dateTo),
        function (results) {
           
            for (var i = 0; i < results.length; i++) {
                scheduler.addMarkedTimespan({ 
                    days: results[i].StartDateTime,
                    zones: "fullday",
                    type: "dhx_time_block",
                    sections: {timeline: results[i].ResourceID},
                    css: "red_section",
                    html: results[i].ActivityType
                });
            }
            scheduler.updateView();
            $deferred.resolve();
        },
        function (error) {
            alert(error.get_message());
            $deferred.reject();
        });

    return $deferred;
}

function GetPlanText(plan) {
    var format = scheduler.date.date_to_str("%d-%m-%Y");
    var txt = "";
    if (plan.InstructionType == 1) {
        txt = "<div>COL -" + instructionTypeDisplay(plan.InstructionTypeID) + (plan.InstructionID ? ' - planned' : '') + "</div>"
                + format(plan.ArrivalDateTime) + "<div>" + plan.Collection + "</div>" ;
    }
    else {
        txt = "<div>DEL -" + instructionTypeDisplay(plan.InstructionTypeID) + (plan.InstructionID ? ' - planned' : '') + "</div>"
                + format(plan.ArrivalDateTime) + "<div>" + plan.Delivery + "</div>";
    }

    return txt;
}

function instructionTypeDisplay(instructionTypeID) {
    switch (instructionTypeID) {
        case 1:
            return 'Collection';
            break;
        case 2:
            return 'Delivery';
            break;
        default:
            return '';
            break;
    };
};


function removePrePlanEvent(eventId) {
    var ev = scheduler.getEvent(eventId);
    removePrePlan(ev.orderID);
}

    function createPrePlan(orderID, vehicleResourceID, arrivalDateTime) {
        showLoading();

        Orchestrator.WebUI.Services.VehiclePlanning.OrderPrePlanCreate(orderID, vehicleResourceID, arrivalDateTime,
            function () {
                $.when(loadSchedulerData(), loadOrders(_selectedFilter)).done(function () { hideLoading(); });
            },
            function (error) {
                alert(error.get_message());
            });
    }

    function removePrePlan(orderID) {
        showLoading();

        Orchestrator.WebUI.Services.VehiclePlanning.OrderPrePlanDelete(orderID,
            function () {
                $.when(loadSchedulerData(), loadOrders(_selectedFilter)).done(function () { hideLoading(); });
           
            },
            function (error) {
                alert(error.get_message());
            });
    }

    var wcfDateFormatter = function (date) {
        return '\/Date(' + date.getTime() + '-0000)\/'; //TODO: will this work OK for both BST and GMT?
    }

    function loadOrders(filteroption) {
        var $deferred = new $.Deferred();

        Orchestrator.WebUI.Services.VehiclePlanning.GetOrdersForPrePlanning(filteroption,
            function (results) {
                var compiledTemplate = getTemplate('unplannedorder');
                var html = compiledTemplate({ unplannedOrders: results });
                $('#unplannedOrders').html(html);
                $('#ordersTitle').text("Orders (" + results.length + ")");
                $deferred.resolve();
            },
            function (error) {
                alert(error.get_message());
                $deferred.reject();
            });
       

        return $deferred;
    }

    function showLoading() {
        $.blockUI({
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: '.3',
                color: '#fff'
            }
        });
    }

    function hideLoading() {
        $.unblockUI();
    }

    Handlebars.registerHelper('goodstype', function (object) {
        var ret = "color-green";
        switch (object) {
            case 5: ret = "color-green";
                break;
            case 6: ret = "color-blue";
                break;
            case 7: ret = "color-yellow";
                break;
            default: ret = "color-grey";

        }

        return new Handlebars.SafeString(ret);
        
    });

    Handlebars.registerHelper("formatteddate", function (object) {
        var format = scheduler.date.date_to_str("%d %M");
        return format(object);
    });
