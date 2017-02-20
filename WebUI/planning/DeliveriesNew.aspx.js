//#region Info Box Setup 

/***************************************************************/
/* Add support for pop-up info boxes.
/***************************************************************/
var map = null;
var delayedOrdersToPlot = null;

//Order Selection and Group Handling
//#region Order Selection and Group Handling

var groupHandlingIsActive = false;

function HandleGroupSelection(orderGroupID) {
    if (groupHandlingIsActive)
        return;

    groupHandlingIsActive = true;

    $(":checkbox[id$=chkOrderID]").each(function (chkIndex) {
        if (this.checked == false && this.parentElement.getAttribute('OrderGroupID') == orderGroupID) {
            this.click();
        }
    });

    groupHandlingIsActive = false;
}

function HandleGroupDeselection(orderGroupID) {
    if (groupHandlingIsActive)
        return;

    groupHandlingIsActive = true;

    $(":checkbox[id$=chkOrderID]").each(function (chkIndex) {
        if (this.checked == true && this.parentElement.getAttribute('OrderGroupID') == orderGroupID) {
            this.click();
        }
    });

    groupHandlingIsActive = false;
}

function ChangeList(src, add, orderID, noPallets, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress, deliveryDate) {


    if (add) {
        // Add to the list
        if (orders.length > 0)
            orders += ",";
        orders += orderID;

        // change the pin to show that this is on the current load

        RowSelected(orderID, noPallets, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress, deliveryDate);

    }
    else {
        // remove from the list
        orders = orders.replace(orderID + ",", "");
        orders = orders.replace("," + orderID, "");
        orders = orders.replace(orderID, "");

        // set this back to blue
        if (src != null) {
            if (src.data.__type == "Orchestrator.WebUI.planning.mpd+OrderLite") {
                setPinIcon(src, src.data);
            }
            else
                src.setOptions({ icon: '/images/pushpingrey.png' });
        }

        RowDeSelected(orderID, noPallets, palletSpaces, weight, businessTypeID, businessType);

        
    }

}
var clickedMousePosLeft = 0;
var clickedMousePosTop = 0;
var _orderID = 0;
function RowContextMenu(sender, eventArgs) {

    var evt = eventArgs.get_domEvent();


    var index = eventArgs.get_itemIndexHierarchical();
    document.getElementById("radGridClickedRowIndex").value = index;

    menu.show(evt);

    evt.cancelBubble = true;
    evt.returnValue = false;

    if (evt.stopPropagation) {
        evt.stopPropagation();
        evt.preventDefault();
    }

    sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);
    _orderID = sender.get_masterTableView().get_dataItems()[index].getDataKeyValue("OrderID");
}

function OnClick(sender, eventArgs) {
    var mnuCall = eventArgs.get_item().get_value();
    eventArgs.get_item().get_menu().hide();

    var businessTypeID = mnuCall;
    var returnURL = location.href;

    if (orders == "" && _orderID != undefined)
        orders = _orderID;

    if (orders.length > 0) {
        if (orders.indexOf(",") == -1 || confirm("This will change the business type for multiple orders, are you sure?")) {
            showLoading();
            PageMethods.UpdateBusinessType(orders, businessTypeID, userName, UpdateBusinessType_Success, UpdateBusinessType_Failure);
        }
    }
    else
        alert("Please select at least 1 order to update.");
}

function UpdateBusinessType_Success(result) {
    btnRefresh.click();
    hideLoading();
}

function UpdateBusinessType_Failure(error) {
    hideLoading();
    alert("There was an error changing the business type of the order, please try again.");
}

var templateRowused = false;
var dropOrder = 0;
var businessTypes = new Array();

var selectedOrders = new Array();
function RowSelected(orderID, pallets, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress, latitude, longitude, deliveryDate) {

    if (selectedOrders.length > 0) {
        //make sure we are not rying to add again
        if (selectedOrders.indexOf(orderID) >= 0) {
            alert("This order is already on the Load");
            return;
        }
    }

    
        selectedOrders.push(orderID);

        totalPallets += parseInt(pallets);
        totalSpaces += parseFloat(palletSpaces);
        totalWeight += parseFloat(weight);

        UpdatepalletCounts(businessTypeID, businessType, pallets);

        var table = $('#tblLoadBuilderOrders');
        var tableID = '#tblLoadBuilderOrders';
        var lastRow = $('#tblLoadBuilderOrders tr:last');
        // Show the LoadBuilder Table (if we are creating a new job)
        if ($('#tabs2').is(":visible")) {
            table = $('#tblExistingOrders');
            tableID = '#tblExistingOrders';
            lastRow = $('#tblExistingOrders tr:last');
        }
        table.show();


        var newRow = null;
        if (templateRowused) // Header and template row
        {
            newRow = lastRow.clone(false).insertAfter(lastRow);
            dropOrder = parseInt($($(lastRow).find('input[id=txtOrder]')).val());
        }
        else {
            newRow = lastRow;
            templateRowused = true;
        }

        $(newRow).css("background-color", "white");
        var img = $($(newRow).find('img[id=imgRemove]'));
        img.unbind();
        img.click(removeOrder);


        // set the values for the table row
        $(newRow.find('span[id*=orderID]')[0]).text(orderID);
        $(newRow.find('span[id*=orderID]')[0]).attr("latitude", latitude);
        $(newRow.find('span[id*=orderID]')[0]).attr("longitude", longitude);
        $(newRow.find('span[id*=collectionPoint]')[0]).text(unescape(collectionPoint));
        $(newRow.find('span[id*=deliveryPoint]')[0]).html(unescape(deliveryPoint) + " <br/> " + deliveryDate);
        $(newRow.find('span[id*=palletSpaces]')[0]).text(palletSpaces);
        $(newRow.find('span[id*=weight]')[0]).text(weight);
        $($(newRow).find('input[id=txtOrder]')[0]).val("");

        if (isUpdating) {
            $($(newRow).find('input[id=txtOrder]')[0]).val("");
            $($(newRow).find('input[id=txtOrder]')[0]).css("disabled", "disabled");
        }

        showTotals(true);
        $('#createDeliveryJob').css("display", "");
    
}

function RowDeSelected(orderID, pallets, palletSpaces, weight, businessTypeID, businessType) {

    // GRD: Removed as you need to be able to remove the new orders that you have selected.
    // if (isUpdating)
    // return;
    var i = selectedOrders.indexOf(orderID);
    selectedOrders.remove(i);

    totalPallets -= parseInt(pallets);
    totalSpaces -= parseFloat(palletSpaces);
    totalWeight -= parseFloat(weight);
    showTotals(true);

    var table = $('#tblLoadBuilderOrders');
    var tableID = '#tblLoadBuilderOrders';
    var lastRow = $('#tblLoadBuilderOrders tr:last');
    // Show the LoadBuilder Table (if we are creating a new job)
    if ($('#tblExistingOrders').is(":visible")) {
        table = $('#tblExistingOrders');
        tableID = '#tblExistingOrders';
        lastRow = $('#tblExistingOrders tr:last');
    }

    // find the loadbuilder row for this order id and remove it
    if ($(tableID + ' tr').length > 2) {
        // remove the row
        $($(tableID + ' tr td span:contains(' + orderID + ')')).parent().parent().remove();

        $('#createDeliveryJob').css("display", "");
    }
    else {
        table.hide();
        templateRowused = false;
        dropOrder = 0;

        // disable the create Delivery Job Button
        $('#createDeliveryJob').css("display", "none");
    }


    UpdatepalletCounts(businessTypeID, businessType, pallets * -1);

}
//#endregion

//#region Totals
function UpdatepalletCounts(businessTypeID, businessType, pallets) {
    var arrayIndex = -1;
    // business type handling
    for (var i = 0; i < businessTypes.length; i++) {
        if (businessTypes[i][0] == businessTypeID) {
            arrayIndex = i;
            break;
        }
    }

    if (arrayIndex < 0) {
        arrayIndex = businessTypes.length;

        businessTypes[arrayIndex] = new Array(3);
        businessTypes[arrayIndex][0] = businessTypeID;
        businessTypes[arrayIndex][1] = businessType;
        businessTypes[arrayIndex][2] = pallets;
    }
    else {
        businessTypes[arrayIndex][2] = parseFloat(businessTypes[arrayIndex][2]) + parseFloat(pallets);
    }

    ShowBusinessTypes();
}

function ShowBusinessTypes() {
    var businessTypeHavingMaxPalletCount = 0;
    var palletCount = 0;
    $('#divBusinessTypes').empty()
    jQuery.each(businessTypes, function() {
        if (parseFloat(this[2]) > palletCount) {
            palletCount = parseFloat(this[2]);
            businessTypeHavingMaxPalletCount = this[0];
        }
        if (this[0] != null)
            $('#divBusinessTypes').append('<input type="checkbox" id="chkBusinessType' + this[0] + '" value="' + this[0] + '"/><label for="chkBusinessType' + this[0] + '">' + this[1] + ' (' + this[2] + ')</label>');
    });
    if ($('#chkBusinessType' + businessTypeHavingMaxPalletCount).length == 1)
        $('#chkBusinessType' + businessTypeHavingMaxPalletCount)[0].checked = true;

    // $('#divBusinessTypes').show();
}

function showTotals(changed) {

    // This now has been extended to show and allow job creation
    // if we have clicked on an order without a jobID we can include in to the load builder

    var __orders = orders.split(',');
    var __count = __orders[0] == "" ? 0 : __orders.length;
    var el = document.getElementById("dvSelected");
    el.innerHTML = "Number of Orders : " + __count;
    el.innerHTML += "<br/>Number of Pallets : " + totalPallets;
    el.innerHTML += "<br/>Number of Pallet Spaces : " + totalSpaces;
    el.innerHTML += "<br/>Total weight: " + totalWeight;
}

//#endregion

//#region Remove order handling

function removeOrder(el) {


    var orderID = parseInt($(this).parent().parent().find('span[id=orderID]').text());
    if (orderID == "") {
        orderID = $(el).closest('table').find('span[id=orderID]').text();
    }
    // just call the click on the deliveries grid this will do the work for us.

    var order = _.findWhere(currentOrders, { OrderID: orderID });

    var pin = null;
    ChangeList(pin, false, order.OrderID, order.Pallets, order.PalletSpaces, order.Weight, order.CollectionPoint, order.DeliveryPoint, dropOrder, order.BusinessTypeID, order.BusinessType, order.DeliveryCustomer, order.DeliveryAddress, order.DeliveryDateTime);

    $('table[id *=grdDeliveries] a:contains(' + orderID + ')').parent().parent().parent().find('input[id*=chkOrderID]').click();
    
    var mapViewOptions = { zoom: map.getZoom(), center: map.getCenter() };

    PlotOrders(currentOrders, mapViewOptions);
}

//#endregion

//#region Create Job UI Handling

// ReOrder the rows 
function reOrderLoadBuilder(txtDropOrder) {
    var tableID = '#tblLoadBuilderOrders';

    if ($(txtDropOrder).val() != $(txtDropOrder).parent().parent()[0].rowIndex) {
        // if we are moving to the end put it after otherwise before
        if ($(txtDropOrder).val() > $(txtDropOrder).parent().parent()[0].rowIndex) {
            $(tableID + ' tr:eq(' + $(txtDropOrder).val() + ')').after($(txtDropOrder).parent().parent());
        }
        else {
            // the row has to be moved
            $(tableID + ' tr:eq(' + $(txtDropOrder).val() + ')').before($(txtDropOrder).parent().parent());
        }
    }
}

function ReOrder() {
    // validate that the drop orders have been completed if not add to the end.
    // if there are gaps??
    var ret = true;
    try {
        $('#tblLoadBuilderOrders tr td input[id=txtOrder]').each(function() {
            var txt = $(this);

            if (txt.val() == "") {
                txt.css("background-color", "red");
                ret = false;
            }
            else
                txt.css("background-color", "white");
        });

        if (ret == false) {
            alert("One or more orders in the load builder do not have a drop order, these have been highlighted in red for you, please correct and click on Re-order.");
            return false;
        }



        // reorder the orders
        $('#tblLoadBuilderOrders tr td input[id=txtOrder]').each(function() {
            reOrderLoadBuilder(this);
        });
    }
    catch (e) {
        alert("There was a problem re-ordering the runs, please check to ensure that there are no gaps in the numbering.");
    }
}

//#endregion

function AddOrder() {
    location.href = "manageorder.aspx";
}

function viewOrderProfile(orderID) {
    var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;
    var randomnumber = Math.floor(Math.random() * 11)
    var wnd = window.open(url, randomnumber, "width=1180, height=900, resizable=1, scrollbars=1");
}

//Document Load/Init Events
//#region Document Load/Init Events

// Function to show the filter options overlay box
function FilterOptionsDisplayShow() {
    $("#overlayedClearFilterBox").css({ 'display': 'block' });
    $("#filterOptionsDiv").css({ 'display': 'none' });
    $("#filterOptionsDivHide").css({ 'display': 'block' });
}

function FilterOptionsDisplayHide() {
    $("#overlayedClearFilterBox").css({ 'display': 'none' });
    $("#filterOptionsDivHide").css({ 'display': 'none' });
    $("#filterOptionsDiv").css({ 'display': 'block' });
}

//#endregion

//Resets the Pop up to the lower right 
//#region Resets the Pop up to the lower right 

function resetBox() {
    $("#overlayedClearDataBox").css({ 'top': '', 'left': '', 'right': '20px', 'bottom': '20px' });
}

// Expands the

function expandBox() {
    $(".loadBuilder").css({ 'height': '490px' });
    $(".loadBuilderInner").css({ 'height': '425px' });
    $(".jobView").css({ 'height': '490px' });
    $(".jobViewInner").css({ 'height': '425px' });
    $(".detractTab").css({ 'display': 'block' });
    $(".expandTab").css({ 'display': 'none' });
}

function detractBox() {
    $(".loadBuilder").css({ 'height': '385px' });
    $(".loadBuilderInner").css({ 'height': '315px' });
    $(".jobView").css({ 'height': '385px' });
    $(".jobViewInner").css({ 'height': '315px' });
    $(".expandTab").css({ 'display': 'block' });
    $(".detractTab").css({ 'display': 'none' });
}

//#endregion

//Create Delivery Job
//#region Create Delivery Job
function CreateDeliveryJob() {
    showLoading();
    var trunkCollectionPointID = -1;
    var trunkCollectionDate = null;
    var trunkCollectionTime = null;
    if (!dteCollectionDate.isEmpty())
        trunkCollectionTime = dteCollectionTime.get_selectedDate();

    var selectedBuisnessTypeID = $('#divBusinessTypes input[id*=chkBusinessType]:checked').val();
    if (selectedBuisnessTypeID == null) {
        selectedBuisnessTypeID = $('#divBusinessTypes input[id*=chkBusinessType]')[0].value;
    }

    // pull this all together to create the delivery job
    if ($('input:checkbox[id$=chkTrunkCollection]')[0].checked) {
        var point = $find('ctl00_ContentPlaceHolder1_ucCollectionPoint_cboPoint');
        var pointParts = $find('ctl00_ContentPlaceHolder1_ucCollectionPoint_cboPoint').get_value().split(',');
        trunkCollectionPointID = pointParts[1];

        if (dteCollectionDate.isEmpty()) {
            alert("Please enter when you plan to collect the orders from the cross-dock location.");

            // redirect back to the correct tab and control
            $("#tabs").tabs({ active: 1 });
            hideLoading();
            return;
        }

        trunkCollectionDate = dteCollectionDate.get_selectedDate();

    }

    var orderIDs = new Array();
    var orderCount = 0;
    //orderIDs = orders.split(',');

    // Get the Orders selected and the order for delivery
    $('#tblLoadBuilderOrders span[id*=orderID]').each(function() {
        orderIDs[orderCount] = $(this).text();
        orderCount++;
    });


    // Determine who is doing the job
    var driverResourceID = -1;
    var vehicleResourceID = -1;
    var trailerResourceID = -1;
    var planningCategoryID = -1;
    var subcontractorIdentityID = null;
    var allocatedToIdentityID = null;
    var subcontractType = -1; // whole job or per order;
    var resourceName = "";
    var subContractRate = -1;
    var TrunkDate = null;

    if ($(':radio[id$=rbOwnResource]')[0].checked) {


        var cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
        driverResourceID = cboDriver.get_value() == "" ? -1 : parseInt(cboDriver.get_value());

        var cboVehicle = $find('ctl00_ContentPlaceHolder1_ucResource_cboVehicle');
        vehicleResourceID = cboVehicle.get_value() == "" ? -1 : parseInt(cboVehicle.get_value());
        var cboTrailer = $find('ctl00_ContentPlaceHolder1_ucResource_cboTrailer');
        trailerResourceID = cboTrailer.get_value() == "" ? -1 : parseInt(cboTrailer.get_value());

        var cboPlanningCategory = $find('ctl00_ContentPlaceHolder1_ucResource_cboPlanningCategory');
        planningCategoryID = cboPlanningCategory.get_value() == "" ? -1 : parseInt(cboPlanningCategory.get_value());

        resourceName = cboDriver.get_text();
    }
    else if ($(':radio[id$=rbSubContractor]')[0].checked) {
        subcontractorIdentityID = cboSubContractor.get_value();

        if (subcontractorIdentityID == "") {
            alert("Please select a subcontractor from the list.");
            hideLoading();
            return;
        }

        resourceName = cboSubContractor.get_text();
        subcontractType = $(':radio[name=SubContractOption]:checked').val();
        subContractRate = txtSubContractRate.get_value();
    }
    else if ($(':radio[id$=rbAllocation]')[0].checked) {
        allocatedToIdentityID = cboAllocatedTo.get_value();

        if (allocatedToIdentityID == "") {
            alert("Please select a value from the allocation list.");
            hideLoading();
            return;
        }
    }

    var loadNo = '';
    if ($('#chkApplyLoadNo')[0].checked) {
        loadNo = $('#txtLoadNo')[0].value;
    }

    var createOrderGroup = $('#chkCreateOrderGroup')[0].checked;
    var createManifest = $('#chkCreateManifest')[0].checked;
    var printPods = false;
    var chkPrintPods = $('input[id*=chkPrintPodLabels]');
    if (chkPrintPods[0] == null) {
        printPods = false;
    } else {
        printPods = chkPrintPods[0].checked;
    }

    var printPils = false;
    var chkPrintPils = $('input[id*=chkPrintPils]');
    if (chkPrintPils[0] == null) {
        printPils = false;
    } else {
        printPils = chkPrintPils[0].checked;
    }
    var showAsCommunicated = $('#chkShowInProgress')[0].checked;

    if (driverResourceID == -1 && subcontractorIdentityID == null && createManifest) {
        alert("You cannot create a manifest without selecting a driver or subcontractor.")
        hideLoading();
        return;
    }

    var manifestDate = null;
    var manifestTitle = null;
    if (createManifest) {
        manifestDate = dteManifestDate.get_selectedDate();
        manifestTitle = resourceName + " - " + dteManifestDate.get_displayValue();
    }


    // Assign a trunk date is this is going via the pallet network
    if (!dteTrunkDate.isEmpty()) {
        TrunkDate = dteTrunkDate.get_selectedDate();
    }

    var multipleRuns = $('#chkMultipleRuns')[0].checked;
    var ordersPerRun = parseInt($('#txtOrdersPerRun')[0].value);
    if (!multipleRuns || isNaN(ordersPerRun))
        ordersPerRun = 9999;
    
    while(orderIDs.length > 0)
    {
        var orderIDsChunk = orderIDs.splice(0, ordersPerRun);
        PageMethods.CreateDeliveryJob(orderIDsChunk, selectedBuisnessTypeID, trunkCollectionPointID, trunkCollectionDate, trunkCollectionTime,
            driverResourceID, vehicleResourceID, trailerResourceID, planningCategoryID, userName,
            createManifest, resourceName, subcontractorIdentityID, subcontractType, subContractRate, showAsCommunicated,
            manifestTitle, manifestDate, printPods, printPils, TrunkDate, createOrderGroup, allocatedToIdentityID, loadNo,
            CreateDeliveryJob_Success, CreateDeliveryJob_Failure);
    }
}

function CreateDeliveryJob_Success(result) {
    try {

        var parts = result.toString().split(",");
        var noRequests = true;

        $(document).ajaxStop(function () {
            window.location.reload();
        });

        if ($('#chkShowJobOnCreation')[0].checked) {
            ViewJob(parts[0]);
        }

        if ($('#chkCreateManifest')[0].checked) {
            noRequests = false;

            var excludeFirstRow = $('#chkExcludeFirstRow')[0].checked;
            var extraRows = $('#txtExtraRows').val();
            var showFullAddress = $('#chkShowFullAddress')[0].checked;
            var usePlannedTimes = $('input:radio[name*=rblOrdering][checked]').val() == 0;
            var rmID = parts[1];

            if (parts.length == 3) // Subby
            {
                $.ajax({
                    type: "POST",
                    url: "mpd.aspx/ShowSubbyManfest",
                    data: "{'resourceManifestID' : " + rmID + ", 'excludeFirstRow' : '" + excludeFirstRow + "', 'usePlannedTimes' : '" + usePlannedTimes + "', 'extraRows' : " + extraRows + ", 'showFullAddress' : '" + showFullAddress + "', 'jobID' : " + parts[0] + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        // Set label text to method's return.
                        ManifestGeneration_Success(msg);
                    },
                    error: function (error) {
                        ManifestGeneration_Failure(error);
                    }
                });
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "mpd.aspx/GenerateAndShowManifest",
                    data: "{'resourceManifestID' : " + rmID + ", 'excludeFirstRow' : '" + excludeFirstRow + "', 'usePlannedTimes' : '" + usePlannedTimes + "', 'extraRows' : " + extraRows + ", 'showFullAddress' : '" + showFullAddress + "', 'jobID' : " + parts[0] + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        // Set label text to method's return.
                        ManifestGeneration_Success(msg);
                    },
                    error: function (error) {
                        ManifestGeneration_Failure(error);
                    }
                });
            }
        }

        if ($('#chkPrintPilLabels')[0].checked) {
            noRequests = false;

            $.ajax({
                type: "POST",
                url: "mpd.aspx/GenerateAndShowPil",
                data: "{'JobId' : " + parts[0] + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    // Set label text to method's return.
                    GenerateAndShowPil_Success(msg);
                },
                error: function (error) {
                    GenerateAndShowPil_Failure(error);
                }
            });
        }

        if ($('#chkCreateLoadingSheet')[0].checked) {
            noRequests = false;
            
            $.ajax({
                type: "POST",
                url: "mpd.aspx/GenerateAndShowLoadingSheet",
                data: "{'jobIDs' : " + parts[0] + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    // Set label text to method's return.
                    LoadingSheet_Success(msg);
                },
                error: function (error) {
                    LoadingSheet_Failure(error);
                }
            });
        }
    }
    catch (e) {
    }

    hideLoading();

    if(noRequests)
        location.href = location.href;
}

function CreateDeliveryJob_Failure(error) {

    hideLoading();
    var message = "Something went wrong when creating the Delivery Run, please try again.";
    message += "\n" + error.get_message();
    alert(message);

}

//#endregion


//#region Update Delivery Job

function UpdateDeliveryJob() {
    var instructionID = null;
    var trunkCollectionPointID = -1
    var trunkCollectionDate = null;
    var trunkCollectionTime = null;

    trunkCollectionDate = dteCollectionDate.get_selectedDate();

    if (!dteCollectionDate.isEmpty())
        trunkCollectionTime = dteCollectionTime.get_selectedDate();

    // We are collecting from a Trunk Point
    if ($('#trCollectionPoint').is(':visible') && $('#chkLoadAndGo')[0].checked == false && $('#tblInstructions input[id*=chkAddToInstruction]:checked').length == 0) {
        var point = $find('ctl00_ContentPlaceHolder1_ucCollectionPoint_cboPoint');
        var pointParts = $find('ctl00_ContentPlaceHolder1_ucCollectionPoint_cboPoint').get_value().split(',');
        trunkCollectionPointID = pointParts[1];

        if (dteCollectionDate.isEmpty()) {
            alert("Please enter when you plan to collect the orders from the cross-dock location.");

            // redirect back to the correct tab and control
            $("#tabs2").tabs({ active: 1 });
            return;
        }

    }

    // Are collecting from an Existing instruction on the job
    if ($('#tblInstructions input[id*=chkAddToInstruction]:checked').length > 0)
        instructionID = $('#tblInstructions input[id*=chkAddToInstruction]:checked').attr("instructionID");

    showLoading();

    var __orders = new Array();
    var ordersList = orders.split(",");
    for (var o = 0; o < ordersList.length; o++) {
        __orders[o] = ordersList[o];
    }

    var jobID = $('#txtJobID').val();

    var updateManifest = $('#chkUpdateManifest')[0].checked;
    var manifestDate = null;
    var manifestTitle = null;

    if (updateManifest) {
        manifestDate = dteManifestDate.get_selectedDate();
        manifestTitle = resourceName + " - " + dteManifestDate.get_displayValue();
    }

    var resourceName = '';

    if ($(':radio[id$=rbOwnResource]')[0].checked) {
        var cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
        resourceName = cboDriver.get_text();
    }
    else if ($(':radio[id$=rbSubContractor]')[0].checked) {
        resourceName = cboSubContractor.get_text();
    }

    PageMethods.UpdateJob(jobID, __orders, trunkCollectionPointID, trunkCollectionDate, trunkCollectionTime, instructionID, userName, userID, updateManifest, manifestDate, manifestTitle, resourceName, UpdateJob_Success, UpdateJob_Failure);
}

function UpdateJob_Success(result) {
    hideLoading();

    var parts = result.toString().split(",");
    var excludeFirstRow = $('#chkExcludeFirstRow')[0].checked;
    var showFullAddress = $('#chkShowFullAddress')[0].checked;
    var extraRows = $('#txtExtraRows').val();
    var usePlannedTimes = $('input:radio[name*=rblOrdering][checked]').val() == 0;

    if (parts.length > 2 && parseInt(parts[2]) > 0) {
        var jobID = parts[0]
        var rmID = parts[2];

        if (parts.length > 3)
            PageMethods.ShowSubbyManfest(rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress, jobID, ManifestGeneration_Success, ManifestGeneration_Failure);
        else
            PageMethods.GenerateAndShowManifest(rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress, jobID, ManifestGeneration_Success, ManifestGeneration_Failure);
    }
    else if (parts.length > 1 && parseInt(parts[1]) == 0) {
        DisplayManifestLinkWindow(jobID, rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress)
    }
    else {
        // no manifest;
        location.href = "mpd.aspx";
    }
}

function UpdateJob_Failure(error) {

    hideLoading();
    alert(error.get_message());
}

//#endregion


//#region Manifest Display

function showManifest(resourceManifestID) {
    if (resourceManifestID > 0)
        window.open("/manifest/viewmanifest.aspx?rmID=" + resourceManifestID + "&excludeFirstLine=true;extraRows=0;usePlannedTimes=false;");
}

//#endregion

//#region Find/Load Existing Job

function FindJob() {
    var JobID = $('#txtJobID').val();
    //$('#tblExistingOrders tr:gt(0)').empty();
    LoadJob(JobID);
}

function LoadJob(jobID) {
    showLoading();
    $('#txtJobID').val(jobID);

    LoadInstructionsForJob(jobID);

    PageMethods.GetOrdersForJob(jobID, GetOrdersForJob_Success, GetOrdersForJob_Failure);
}

function LoadInstructionsForJob(jobID) {
    showLoading();
    $('#spnInstructionsJobID').text(jobID);
    PageMethods.GetInstructionsForJob(jobID, GetInstructionsForJob_Success, GetInstructionsForJob_Failure);
}

function GetInstructionsForJob_Success(result) {

    ShowUpdateJob();
    $('#tblInstructions tr:gt(0)').empty();
    $('#tblInstructions tr:last').after(result);

    if ($('#tabs2').tabs().find("#tabs-4").css("display") == "none")
        $('#tabs2').tabs({ active: 0 });

    $('#lblUpdateManifest').after($('#manifestOptions'));
    $('#manifestOptions').show();

    templateRowused = true;

    isUpdating = true;
    hideLoading();
}

function GetInstructionsForJob_Failure(error) {
    $('#spnInstructionsJobID').text("");
    alert("There was a problem getting the instructions for this run.");
}

function GetOrdersForJob_Success(result) {
    $('#tblExistingOrders').show();
    $('#tblExistingOrders tr:gt(0)').empty();
    $('#tblExistingOrders tr:last').after(result);
    ShowUpdateJob();
    templateRowused = true;

    //bind up the delete order from job functionality
    $('#tblExistingOrders img[id=imgRemove]').each(function() {
        $(this).click(DeleteOrder);

    });
    isUpdating = true;
    hideLoading();

}

function ShowUpdateJob() {
    $('#tabs').hide();
    $("#tabs2").tabs({ collapsible: true, active: 0 });
    $('#tabs2').show();

    // Move the point lookup to this tab collection
    $('#tabs-5').append($('#tabs-2 > div'));
    $('#updateJob').show();
    $('#txtJobID').keypress(function(e) {
        if (e.which == 13) {
            FindJob($('#txtJobID').val());
        }
    });
}

function CancelUpdateJob() {
    $('#tabs2').hide();
    $("#tabs").tabs({ collapsible: true, active: 0 });
    $('#tabs').show();
}

function GetOrdersForJob_Failure(error) {
    alert(error.get_message());
}

//#endregion

// Capture Book In
//#region

function BookIn(el, orderID) {
    try {
        var orderID = $(el).data("orderID");
        var bookedInWith = $('#txtBookedInWith').val();
        var bookedInRefs = $('#txtBookedInReferences').val();
        var _dteBookInFromDate = $(dteBookInFromDate).val();
        var _dteBookInFromTime = $(dteBookInFromTime).val();
        var _dteBookInByFromDate = $(dteBookInByFromDate).val();
        var _dteBookInByFromTime = $(dteBookInByFromTime).val();
        var dateOption = $(':radio[name*=BookInTime]:checked').val();

        PageMethods.BookIn(orderID, userName, bookedInWith, bookedInRefs, dateOption, _dteBookInFromDate, _dteBookInFromTime, _dteBookInByFromDate, _dteBookInByFromTime, BookIn_Success, BookIn_Failure)

        $('#bookInWindow').hide();
        $('#' + $(el).data("el")).attr('src', '/App_Themes/Orchestrator/Img/MasterPage/icon-tick-small.png');
        $('#' + $(el).data("el")).attr('alt', "Booked In by " + userName + " with " + bookedInWith + "; references: " + bookedInRefs);
    }
    catch (e) {
        alert(e);
    }

    hideLoading();
}

function ShowBookIn(el, orderID) {
    //getting height and width of the message box
    var pos = findPos(el);
    var height = $('#bookInWindow').height();
    var width = $('#bookInWindow').width();

    height += 10;
    width += 10;
    
    //calculating offset for displaying popup message

    //show the popup message and hide with fading effect
    $('#bookInWindow').css({ left: "100px", top: "200px" }).show();
    $('#txtBookedInWith').val("");
    $('#txtBookedInReferences').val("");

    var date = new Date();

    $('input[id*=txtBookInFromDate]').val(date.getDate() + '/' + (date.getMonth()+1) + '/' + date.getFullYear());
    $('input[id*=txtBookInFromTime]').val("08:00");
    $('input[id*=txtBookInByFromDate]').val(date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear());
    $('input[id*=txtBookInByFromTime]').val("17:00");
    $('#txtBookedInWith').focus();
    $('#btnSaveBookIn').data("orderID", orderID);
    $('#btnSaveBookIn').data("el", el.id);
}

function BookIn_Success(result) {
    // change the icon on the book in control

    hideLoading();
}

function BookIn_Failure(error) {
    alert(error.get_message());
    hideLoading();
}

function ChangeBookInTiming(e) {
    // Get the collect by date and time controls
    var selectedOption = e.srcElement.value;
    var delSpan = $(e.srcElement).closest('div');
    var delDate = $(delSpan).find('input[id*=txtBookInByFromDate]');
    var delTime = $(delSpan).find('input[id*=txtBookInByFromTime]');
    var deliveryByTime = $(delSpan).find('input[id*=txtBookInFromTime]');
    switch (selectedOption) {
        case "0": // Window
            delDate.show();
            delTime.show();
            deliveryByTime.val(delTime.val());
            break;
        case "1": //Timed
            delDate.hide();
            delTime.hide();
            deliveryByTime.val(delTime.val());
            break;
        case "2": // Anytime
            delDate.hide();
            delTime.hide();
            deliveryByTime.prop("disabled", true);
            deliveryByTime.val("Anytime");
            break;

    }
}
//#endregion

function DeleteOrder(el) {
    var orderID = $(this).parent().parent().find('span[id=orderID]').text();
    var jobID = $(this).parent().parent().find('span[id=orderID]').attr("jobID");

    // remove the order from the list of orders on the grid


    if (confirm("This will REMOVE this order from the run, please click OK to continue.")) {
        showLoading();
        $(this).closest('tr').empty();
        PageMethods.RemoveOrderFromJob(jobID, orderID, userName, userID, RemoveOrderFromJob_Success, RemoveOrderFromJob_Failure);
    }
}

function RemoveOrderFromJob_Success(result) {
    var parts = result.split(",");
    if (parts[1] == "reload")
        location.href = location.href;

    hideLoading();
}

function RemoveOrderFromJob_Failure(error) {
    hideLoading();
    alert("There was a problem removing the order from the run, please try again.");

}

function ResetScreen() {
    showLoading();
    location.href = "mpd.aspx"
}


function CancelAndResetScreen() {
    // block UI while Processing message
    showLoading();
    location.href = 'mpd.aspx';
    return;

    // remove all rows in the orders table;
    hideLoading();

}

function SelectLoadInstruction(el) {
    if (el.checked) {
        $('#chkLoadAndGo')[0].checked = false;

        $(':checkbox[id*=chkAddToInstruction]:checked').each(function() {
            if (this != el)
                this.checked = false;
        });

    }
    else {
        $('#chkLoadAndGo')[0].checked = true;
    }

}

function SelectLoadAndGo(el) {
    if (el.checked) {
        $(':checkbox[id*=chkAddToInstruction]:checked').each(function() {
            this.checked = false;
        });
    }

}

function UpdateInstruction() {
    // determine the selected Instruction
    var instructionID = $('#tblInstructions input[id*=chkAddToInstruction]:checked').attr("instructionID");
    var subTable = $('table [id=subTableOrders][instructionID=' + instructionID + ']');
    subTable.show();

    var _orders = orders.split(',');
    for (var i = 0; i < _orders.length; i++) {
        var row = null;
        // add these details to the instructions
        if ($(subTable).find('tr').length == 1 && templateRowused == false) {
            row = $(subTable).find('tr')[0];
            templateRowuse == true;
        }
        else {
            row = $(subTable).find('tr:last').clone(true).insertAfter($(subTable).find('tr:last'));
        }


        var selectedOrderID = _orders[i];
        var span = $(row).find('span').text(selectedOrderID);

    }

}

function RemoveOrderFromInstruction(el) {
    removeOrder(el);

    if ($(el).closest('table').find('tr').length > 1)
        $(el).closest('tr').empty();
    else {
        $(el).closest('table').hide();
        templateRowused = false;
    }
}

// Block UI
//#region 
function showLoading() {
    $.blockUI({ css: {
        border: 'none',
        padding: '15px',
        backgroundColor: '#000',
        '-webkit-border-radius': '10px',
        '-moz-border-radius': '10px',
        opacity: '.8',
        color: '#fff'
    }
    });
}

function hideLoading() {
    $.unblockUI();

    // Resets the Add Order Button submission check on the page load.
    $('input:hidden[id*=hidCheckSubmit]').val("false");
    $('input[id*=btnCreateDeliveryJob]').val("Create Delivery Run");
}

function ViewGroup(orderGroupId) {
    window.open("orderGroupProfile.aspx?OGID=" + orderGroupId,"", "width=800, height=600, resizable=1, scrollbars=1");
}

//#endregion

function ViewJob(jobID) {
    if (jobID > 0) {

        var url = "/job/job.aspx?jobId=" + jobID + getCSID();

        window.open(url);
    }
}

function ViewOrder(orderId) {
    if (orderId > 0) {
        var url = "/Groupage/ManageOrder.aspx?oID=" + orderId + getCSID();

        window.open(url,"","height=960, width=1180, scrollbars=0");
    }
}

function LoadingSummarySheetSelection(src) {
    var selectedJobID = src.value;
    var dataItems = mtv.get_dataItems();
    var isChecked = src.checked;

    for (var rowIndex = 0; rowIndex < dataItems.length; rowIndex++) {
        try {
            var chkLoadingSheet = null;

            if (dataItems[rowIndex].get_element().getElementsByTagName("INPUT").length > 1)
                chkLoadingSheet = dataItems[rowIndex].get_element().getElementsByTagName("INPUT")[1];

            // If the checkbox has been found, set its checked status to that of the selected check box.
            if (chkLoadingSheet != null && chkLoadingSheet.id != src.id && selectedJobID == chkLoadingSheet.value) {
                chkLoadingSheet.checked = isChecked;
            }
        }
        catch (error) {
        }
    }
}

function ShowPosition(latitude, longitude, description) {
    var url = "/gps/getcurrentlocation.aspx?lat=" + latitude + "&lng=" + longitude + "&desc=" + description;
    window.open(url, "", "height=600, width=630, scrollbars=0");
}

function ManifestDate_OnDateChanged(sender, eventArgs) {
    $('#txtManifestTitle').val(cboDriver.get_text() + " - " + dteManifestDate.get_displayValue());
}

function cboSubContractor_SelectedIndexChanged(sender, eventArgs) {
    $('#txtManifestTitle').val(sender.get_text() + " - " + dteManifestDate.get_displayValue());

    // if this is for the pallet network then show the trunk date field
    if (sender.get_value() == palletNetworkID) {
        $('#trTrunkDate').show();
        $('#rbWholeJob').prop("disabled", true)
        $('#rbPerOrder').prop("checked", true);
        txtSubContractRate.disable();
    }
    else {
        $('#trTrunkDate').hide();
        $('#rbWholeJob').prop("disabled", false)
        txtSubContractRate.enable();
    }
}

// Helper for positioning
//#region

function findPos(obj) {
    var curLeft = curTop = 0;
    if (obj.offsetParent) {
        do {
            curLeft += obj.offsetLeft;
            curTop += obj.offsetTop;
        } while (obj = obj.offsetParent);
    }
    return [curLeft, curTop];
}

//#endregion

function cboClient_itemsRequesting(sender, eventArgs) {
    try {
        var context = eventArgs.get_context();
        context["DisplaySuspended"] = false;
    }
    catch (err) { }
}

function cboAllocatedTo_TextChange(sender, eventArgs) {
    // If the user has typed text that does not match an item in the list then clear the field.
    var item = sender.get_selectedItem();
    var text = sender.get_text();
    if ((item != null && text != item.get_text()) || (item == null && text != "")) {
        sender.clearSelection();
        sender.set_text("");
    }
}

function configurePickFromCrossDock() {
    var isPickFromCrossDock = $('input:checkbox[id$=chkTrunkCollection]').prop('checked');
    $('#trCollectionPoint').toggle(isPickFromCrossDock);
    $('#collectionPointText').text(
        isPickFromCrossDock
        ? "You will be picking up the orders from the cross dock location as specified. This will create a run with 1 collection and 1 or more deliveries"
        : "The orders selected will be picked up from their collection point unless they are on a collection run already, in which case they will be picked up from their collection run drop off point. This will create a run with 1 or more collections and 1 or more deliveries");
}

//#region - Added to make the select all checkboxes work as expected

function onTrafficAreaChecked(chkbox) {
    allInCheckboxListChecked($(":checkbox[id*='cblTrafficAreas']"), $(":checkbox[id*='chkSelectAllTrafficAreas']"));
}

function allInCheckboxListChecked(chkboxlist, chkbox) { 
    var allChecked = true;
    chkboxlist.each(function () {
        if (!this.checked)
            allChecked = false;
    });

    chkbox[0].checked = allChecked;
}

//#endregion




$(document).ready(function () {
    // Select all  traffic areas when select all is clicked.
    $(":checkbox[id*='chkSelectAllTrafficAreas']").click(function () {
        var checked_status = this.checked;
        $(":checkbox[id*='cblTrafficAreas']").each(function () {
            this.checked = checked_status;
        });
    });

    // Set up the loadbuilder UI
    $("#tabs").tabs({ collapsible: true, active: false });

    //Display the pop-up box, but only when the page has been rendered
    $("#overlayedClearDataBox").show();

    // Makes the overlay box dragable, but only when the mouse is over the #dragLI icon
    $("#overlayedClearDataBox").draggable({ handle: '#dragLI' });

    // Set up the cross dock fields now, and also set them to be reconfigured on change of chkTrunkCollection
    configurePickFromCrossDock();
    $('input:checkbox[id$=chkTrunkCollection]').click(configurePickFromCrossDock);

    if (createGroupByDefault == 'True')
        $(":checkbox[id*='chkCreateOrderGroup']")[0].checked = true

    $('#chkCreateManifest').click(function () {
        if (this.checked)
            $('#manifestOptions').show();
        else
            $('#manifestOptions').hide();
    });

    $('#chkUpdateManifest').click(function () {
        if (this.checked)
            $('#manifestOptions').show();
        else
            $('#manifestOptions').hide();
    });

    $('#chkApplyLoadNo').click(function () {
        if (this.checked)
            $('#txtLoadNo').show();
        else
            $('#txtLoadNo').hide();
    });

    $(':radio[id$=rbOwnResource]').click(function () {
        $('#pnlSubContractor').hide();
        $('#pnlOwnresource').show();
        $('#pnlAllocate').hide();
        $('#pnlCreateManifest').show();
    });

    $(':radio[id$=rbSubContractor]').click(function () {
        $('#pnlSubContractor').show();
        $('#pnlOwnresource').hide();
        $('#pnlAllocate').hide();
        $('#pnlCreateManifest').show();
    });

    $(':radio[id$=rbAllocation]').click(function () {
        $('#pnlSubContractor').hide();
        $('#pnlOwnresource').hide();
        $('#pnlAllocate').show();
        $('#pnlCreateManifest').hide();

        if ($(":checkbox[id*='chkCreateManifest']")[0].checked == true) {
            $(":checkbox[id*='chkCreateManifest']")[0].click()
            $('#pnlCreateManifest').hide();
        }
    });

    // Hide the column configure box

    $("#dvColumnDisplay").css({ 'display': 'none' });

    // Changes the command row to Light Blue
    $(".rgCommandRow").addClass("GridLBlueCommandRow");
    $(".GridLBlueCommandRow").removeClass("rgCommandRow");

    $(':radio[name*=BookInTime]').bind("click", ChangeBookInTiming);

    //$("#btnLoadMap").click(LoadDeliveries);

    Array.prototype.remove = function (from, to) {
        var rest = this.slice((to || from) + 1 || this.length);
        this.length = from < 0 ? this.length + from : from;
        return this.push.apply(this, rest);
    };

    $.fn.tableScroll.defaults =
    {
        flush: true, // makes the last thead and tbody column flush with the scrollbar
        width: null, // width of the table (head, body and foot), null defaults to the tables natural width
        height: 100, // height of the scrollable area
        containerClass: 'tablescroll' // the plugin wraps the table in a div with this css class
    };

    // bind up the search for the order on the map.

    $('#orderSearch input').bind('keypress', function (e) {
        
     });

});

$(window).load(function () {

    ModuleLoaded();

});


var pinInfobox      = null;
var minilInfobox    = null;
var clusterInfobox = null;


var clusterLayer;
var locations = new Array();
var clusteredDataProvider;
var ui = null;
var currentInfoBubble = null;
var currentTargetId = null;
var currentOrders = null;

function getClusterDomIcon(numOfPoints)
{
    var domElement = document.createElement('div');
    domElement.className += 'clusterDomIcon';
    domElement.innerHTML = numOfPoints;
    var domIcon = new H.map.DomIcon(domElement);
    return domIcon;
}

function getMapIcon(order)
{
    var imageString = "";

    if (selectedOrders.length > 0) {
        //make sure we are not rying to add again
        if (selectedOrders.indexOf(order.OrderID) >= 0) {
            imageString = "/images/pushpinyellow.png";
        }
    }

    if (imageString.length === 0) {
        switch (order.GoodsTypeID) {
            case 2:
                if (order.DeliveryJobID > 0)
                    imageString = "/images/pushpingreensquare.png";
                else
                    imageString = "/images/pushpingreen.png";
                break;
            case 3:
                if (order.DeliveryJobID > 0)
                    imageString = "/images/pushpinorangesquare.png";
                else
                    imageString = "/images/pushpinorange.png";
                break;
            case 4:
                if (order.DeliveryJobID > 0)
                    imageString = "/images/pushpinbluesquare.png";
                else
                    imageString = "/images/pushpinblue.png";
                break;
            default:
                if (order.DeliveryJobId > 0)
                    imageString = "/images/pushpinpinksquare.png";
                else
                    imageString = "/images/pushpinpink.png";
        }
    }
    
    var domElement = document.createElement('div');
    if (order.CollectionPointID === 2023) {
        domElement.innerHTML = "C";
    }
    domElement.className += "deliveryDomMarker";
    domElement.style.backgroundImage = "url('" + imageString + "')";
    domElement.style.height = "30px";
    domElement.style.width = "30px";
    domElement.style.verticalAlign = "middle";
    domElement.style.lineHeight = "30px";
    domElement.style.textAlign = "center";
    domElement.style.backgroundSize = "contain";
    domElement.style.color = "white";
    domElement.style.fontSize = "medium";
    var icon = new H.map.DomIcon(domElement);

    return icon;
}

var CUSTOM_THEME = {
    getClusterPresentation: function (cluster) {
        var numOrders = 0;
        cluster.forEachDataPoint(function (p) {
            numOrders++;
        })

        var clusterMarker = new H.map.DomMarker(cluster.getPosition(), {
            icon: getClusterDomIcon(numOrders),
            min: cluster.getMinZoom(),
            max: cluster.getMaxZoom()
        })

        clusterMarker.setData(cluster);
        return clusterMarker;
    },

    getNoisePresentation: function (noisePoint) {
        var data = noisePoint.getData();

        var noiseMarker = new H.map.DomMarker(noisePoint.getPosition(), {
            min: noisePoint.getMinZoom(),
            icon: getMapIcon(data)
        });

        noiseMarker.setData(noisePoint);

        return noiseMarker;
    }
};



function ModuleLoaded()
{
    //HERE Maps
    var maptypes = platform.createDefaultLayers();
    var mapContainer = document.getElementById('myMap');
    var startLatLong = new H.geo.Point(50.68287, 13.12242); //View of Europe
    map = new H.Map(mapContainer, maptypes.normal.map, {
        center: startLatLong,
        zoom: 4
    });

    var behaviour = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
    ui = mapsjs.ui.UI.createDefault(map, maptypes);

    clusteredDataProvider = new H.clustering.Provider(locations, {
        theme: CUSTOM_THEME
    });

    clusteredDataProvider.addEventListener('pointerenter', function (event) {
        if (event.target.getData().isCluster()) {
            if (event.target.getId() != currentTargetId) {
                if (currentInfoBubble) {
                    currentInfoBubble.close();
                }
                currentTargetId = event.target.getId();
                displayClusterInfobox(event.target.getData());
            }
        }
        else {
            if (event.target.getId() != currentTargetId) {
                if (currentInfoBubble) {
                    currentInfoBubble.close();
                }
                currentTargetId = event.target.getId();
                displayMiniInfobox(event.target.getData());
            }
                
        }
    }, true);

    var layer = new H.map.layer.ObjectLayer(clusteredDataProvider);

    map.addLayer(layer);
 

    if (delayedOrdersToPlot) PlotOrders(delayedOrdersToPlot);
}

function LoadDeliveries() {

    var startDate = dteStartDate.get_value();
    var endDate = dteEnddate.get_value();
    //get the selected Traffic Areas

    var trafficAreas='';
   
     $(":checkbox[id*='cblTrafficAreas']:checked").each(function()
    {
        if(trafficAreas.length==0)
        {
            trafficAreas = $(this).closest('span')[0].getAttribute('ta');
        }
        else
        {
            trafficAreas += ","+$(this).closest('span')[0].getAttribute('ta');
        }
    });

    var showNotPlanned = $(":checkbox[id*='chkShowNotPlanned']:checked").val() !== undefined ? true : false;
    var showAllApproved = $(":checkbox[id*='chkShowAll']:checked").val() !== undefined ? true : false;
    var businessTypeIDs = getSelectedBusinessTypeIDs().join(',');

    var data = '{StartDate: "' + startDate + '", EndDate: "' + endDate + '", trafficAreas: "' + trafficAreas + '" , businessTypes: "' + businessTypeIDs + '", showNotPlanned:' + showNotPlanned + ', showAllApproved:' + showAllApproved + ', cookieSessionID:"' + getParameterByName('csid') + '"}';
    $.ajax({
        type: "POST",
        url: "mpd.aspx/GetOrdersForMap",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {

            FilterOptionsDisplayHide();

            if (clusteredDataProvider) PlotOrders(msg.d);
            else delayedOrdersToPlot = msg.d;
        },
        error: function (err) {
            alert(err.responseText);
        }
    });
}
function PlotOrders(orders, mapViewOptions) {
    locations =[];
    currentOrders = orders;
    
    var ordersWithInvalidLocations = new Array();


    for (var i = 0; i < orders.length; i++) {
        var order = orders[i];

        var orderPoint = new H.clustering.DataPoint(order.DeliveryLatitude, order.DeliveryLongitude, 1, order);

        
        if (order.DeliveryLatitude == 0)
            ordersWithInvalidLocations.push(order.OrderID);
        else {
            locations.push(orderPoint);
        }
            
        
    }
    
    
    $("#orderCount").html("<b>Number of Orders: " + orders.length + "</b>");
    $('#invalidOrders').html('');

    if (ordersWithInvalidLocations.length > 0)
        $('#invalidOrders').append("<span style='font-weight:bold; color:red'>Orders with Invalid Geocodings   </span>");

    $.each(ordersWithInvalidLocations, function (index, value) {
        $('#invalidOrders').append("<a href='#' onclick='viewOrderProfile(" + value + ")'>" + value + "</a> | ");
    });

    if (locations.length > 0) {
        clusteredDataProvider.setDataPoints(locations);

        if (mapViewOptions) {
            map.setZoom(mapViewOptions.zoom);
            map.setCenter(mapViewOptions.center);
        } else {
            var viewRect = H.geo.Rect.coverPoints(locations)
            map.setViewBounds(viewRect);
        }
        
    }
}

function setPinIcon(orderPin, order) {

    // if this is on the loadbuilder then just show as selected
    if (selectedOrders.length > 0) {
        //make sure we are not rying to add again
        if (selectedOrders.indexOf(order.OrderID) >= 0) {
            orderPin.setOptions({ icon: '/images/pushpinyellow.png' });
            return;
        }
    }


    // set the image based on the goods type (this is going to be customer specific)
    switch (order.GoodsTypeID) {
        case 2: //Ambient
            if (order.DeliveryJobID > 0)
                orderPin.setOptions({ icon: '/images/pushpingreensquare.png' });
            else
                orderPin.setOptions({ icon: '/images/pushpingreen.png' });
            break;
        case 3: //Chilled
            if (order.DeliveryJobID > 0)
                orderPin.setOptions({ icon: '/images/pushpinorangesquare.png' });
            else
                orderPin.setOptions({ icon: '/images/pushpinorange.png' });
            break;
        case 4: //Frozen
            if (order.DeliveryJobID > 0)
                orderPin.setOptions({ icon: '/images/pushpinbluesquare.png' });
            else
                orderPin.setOptions({ icon: '/images/pushpinblue.png' });
            break;
        default:
            if (order.DeliveryJobID > 0)
                orderPin.setOptions({ icon: '/images/pushpinpinksquare.png' });
            else
                orderPin.setOptions({ icon: '/images/pushpinpink.png' });
            break;
    }

    return orderPin;
}

var selectedPin = null;

function displayInfobox(e) {

    hideMiniInfobox(null);

    var o = e.target.data;

    if (o.RunID > 0)
        {
            pinInfobox.setOptions({ actions: null });
        }
        else
        {
            pinInfobox.setOptions({ actions: [{ label: 'Add To Load', eventHandler: addOrderToLoad}] });
        }

    pinInfobox.setLocation(e.target.getLocation());
    pinInfobox.setOptions({ visible: true, title: 'Order ID: ' + o.OrderID, description: o.DeliveryPoint });
    pinInfobox.data = o;
    selectedPin = e.target;
}

function displayClusterInfobox(e) {
    
    var count = 0;
    e.forEachDataPoint(function (d) {
        count++;
    })

    // build up the display for the cluster
    var html = '<b> '+ count +' Orders</b><br/><table class="tblClusterOrders">';
    html += '<thead><tr>'
         + '<td></td>'
         + '<td>Load No</td>'
         + '<td>Collect From</td>'
         + '<td>Collect When</td>'
         + '<td>Pallets</td>'
         + '<td>Deliver To</td>'
         + '<td>Deliver When</td>'
         + '<td>Goods Type</td>'
         + '<td>Notes</td>'
         + '</tr></thead>';


    e.forEachDataPoint(function (d) {
        var order = d.gj.data;
            if (order.DeliveryJobID > 0) {
                html += '<tr class="'+ + '"><td><label for="chkClusterOrderID' + order.OrderID + '"></label>'
                  + '</td><td>' + order.CustomerOrderNumber
                  + '</td><td>' + order.CollectionPoint
                  + '</td><td>' + order.CollectionDate
                  + '</td><td>' + order.Pallets
                  + '</td><td style="overflow: hidden; text-overflow: ellipsis">' + order.DeliveryPoint
                  + '</td><td>' + order.DeliveryDate
                  + '</td><td>' + order.GoodsType
                  + '</td><td>' + order.TrafficNotes;
                html += '</td></tr><tr><td></td><td colspan="8">'
                html += "<span style='color:white; margin-right:5px;'>" + order.DeliveryDriver + "</span>"
                + "<span style='color:white; margin-right:5px;'>" + order.DeliveryVehicle + "</span>"
                + "<span style='margin-right:5px; color:white;'><a href='#' style='color:white;' onclick='ViewJob(" + order.RunID + ")'>Run [" + order.RunID + "]</a></span>"
                + "<span><a href='#' style='color:white;' onclick='showManifest(" + order.ManifestID + ")'>Manifest [" + order.ManifestID + "]</a></span>";
                + '</td></tr>';
            }
            else {
                html += '<tr><td><input type="checkbox" value="' + order.OrderID + '" id="chkClusterOrderID' + order.OrderID + '" onclick="addOrderToLoad(' + order.OrderID + ')" /><label for="chkClusterOrderID' + order.OrderID + '"></label>'
                  + '</td><td>' + order.CustomerOrderNumber
                  + '</td><td>' + order.CollectionPoint
                  + '</td><td>' + order.CollectionDate
                  + '</td><td>' + order.Pallets
                  + '</td><td style="overflow: hidden; text-overflow: ellipsis">' + order.DeliveryPoint
                  + '</td><td>' + order.DeliveryDate
                  + '</td><td>' + order.GoodsType
                  + '</td><td>' + order.TrafficNotes;
                html += '</td></tr>'
            }
        })


    
    html += '</table>';

    currentInfoBubble = new H.ui.InfoBubble(e.getPosition(), {
        content: html
    });

    currentInfoBubble.addClass('hereMapsClusterInfoBubble');
    currentInfoBubble.addEventListener('statechange', function (e) {
        if (e.target.j == "closed") {
            currentTargetId = null;
        }
    })
    ui.addBubble(currentInfoBubble);

    var offset = 0;
    offset = $('#tblClusterOrders').height();

    selectedPin = e.target;

}

function displayMiniInfobox(e){
    var order = e.gj.data;

    var content = "<p>Customer: " + order.Customer + "</p>";
    content += "<p>Load No: " + order.CustomerOrderNumber + "</p>";
    content += "<p>Collect From: " + order.CollectionPoint+ "</p>";
    content += "<p>Delivery Point: " + order.DeliveryPoint + "</p>";
    content += "<p>Address: " + order.DeliveryAddressLines  + " </p>";
    content += "<p>Deliver When: " + order.DeliveryDate + "</p>";
    content += "<p>Pallets: " + order.Pallets + "</p>";
    content += "<p>Goods Type: " + order.GoodsType + "</p>";
    content += "<p>Traffic Notes:" + order.TrafficNotes + "</p>";
    if (order.DeliveryJobID > 0) {
        content += "<p>Delivery Driver: " + order.DeliveryDriver + "</p>";
        content += "<p>Delivery Vehicle:" + order.DeliveryVehicle + "</p>";
        content += "<p><a href='#' style='color:white;' onclick='ViewJob(" + order.RunID + ")'>Run [" + order.RunID + "]</a></p>";
        content += "<p><a href='#' style='color:white;' onclick='showManifest(" + order.ManifestID + ")'>Manifest [" + order.ManifestID + "]</a></p>";
    }
    
    content += "<br/><a class=\"mapsInfoBubbleLink\" style='color:white;' onclick=addOrderToLoad(" + order.OrderID + ")>Add To Load</a> <a class=\"mapsInfoBubbleLink\" style='color:white; float:right' onclick=ViewOrder(" + order.OrderID + ")>Order ID:" + order.OrderID + "</a>";
    
    currentInfoBubble = new H.ui.InfoBubble(e.getPosition(), {
        content: content
    });
    currentInfoBubble.addClass('hereMapsInfoBubble');
    currentInfoBubble.addEventListener('statechange', function (e) {
        if (e.target.j == "closed") {
            currentTargetId = null;
        }
    })
    ui.addBubble(currentInfoBubble);
}

function hideMiniInfobox(e){
    minilInfobox.setOptions({visible:false});
}

function hideInfobox(e) {
    pinInfobox.setOptions({ visible: false });
    clusterInfobox.setOptions({ visible: false });
    selectedPin = null;
}

function InfoboxHandler() {
    viewOrderProfile(pinInfobox.data.OrderID);
}
    
function addOrderToLoad(e) {
    var mapViewOptions = { zoom: map.getZoom(), center: map.getCenter() };

    PlotOrders(currentOrders, mapViewOptions);

    var order = _.findWhere(currentOrders, { OrderID: e });

    ChangeList(selectedPin, true, order.OrderID, order.Pallets, order.PalletSpaces, order.Weight, order.CollectionPoint, order.DeliveryPoint, dropOrder, order.BusinessTypeID, order.BusinessType, order.DeliveryCustomer, order.DeliveryAddress, order.DeliveryDateTime);
}

if (typeof String.prototype.startsWith != 'function') {
  // see below for better implementation!
  String.prototype.startsWith = function (str){
    return this.indexOf(str) == 0;
  };
}


function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function Find() {
    // gte the search criteria
    var searchText = $('#txtMapSearch').val();
    var pinToShow = null;

    var cLayer = clusterLayer.GetLayer();
    // find and show the order or the cluter that this is in.
    for (var i = 0; i < cLayer.getLength(); i++) {
        if (cLayer.get(i).data.__type == "Orchestrator.WebUI.planning.mpd+OrderLite") {
            if (cLayer.get(i).data.OrderID.toString().startsWith(searchText) || cLayer.get(i).data.CustomerOrderNubber.toString().startsWith(searchText)) {
                pinToShow = cLayer.get(i);
                break;
            }
        }
        else {
            for (var x = 0; x < cLayer.get(i).data.length; x++) {
                if (cLayer.get(i).data[x].OrderID.toString().startsWith(searchText) || cLayer.get(i).data[x].CustomerOrderNumber.toString().startsWith(searchText)) {
                    pinToShow = cLayer.get(i);
                    break;
                }
            }
        }
    }

    if (pinToShow != null) {
        // show this on the map
        var options = map.getOptions();
        options.center = pinToShow.getLocation();
        options.zoom = 14;

        map.setView(options);
        $('#txtMapSearch').val("");
    }
}
