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


function ChangeList(e, src, orderID, noPallets, orderGroupID, orderGroupGroupedPlanning, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress, latitude, longitude, deliveryDate) {


    if (src.checked) {
        // Add to the list
        if (orders.length > 0)
            orders += ",";
        orders += orderID;
        $(src).closest('tr').toggleClass("SelectedRow_Orchestrator");

        //gridRow.className = "SelectedRow_Orchestrator";

        RowSelected(orderID, noPallets, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress, latitude, longitude, deliveryDate);

        // Is the order part of a group that is grouped planning enabled?
        // Automatically select the other orders in the grid that belong to this group.
        if (orderGroupID > 0 && orderGroupGroupedPlanning) {
            HandleGroupSelection(orderGroupID);
        }
    }
    else {
        // remove from the list
        orders = orders.replace(orderID + ",", "");
        orders = orders.replace("," + orderID, "");
        orders = orders.replace(orderID, "");
        $(src).closest('tr').toggleClass("SelectedRow_Orchestrator");
        //gridRow.className = "GridRow_Orchestrator";

        RowDeSelected(orderID, noPallets, palletSpaces, weight, businessTypeID, businessType);

        // Is the order part of a group that is grouped planning enabled?
        // Prompt to see if the user wishes to uncheck those orders also.
        if (orderGroupID > 0 && orderGroupGroupedPlanning) {
            HandleGroupDeselection(orderGroupID);
        }
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

function RowSelected(orderID, pallets, palletSpaces, weight, collectionPoint, deliveryPoint, dropOrder, businessTypeID, businessType, deliveryCustomerName, deliveryAddress, latitude, longitude, deliveryDate) {

    //if (isUpdating)
    //    return;

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

    // if this is for the palletNetwork then we can order these as it does not matter
    //    if ($('#divBusinessTypes :checkbox[checked=true]').val() == palletNetworkBusinessTypeID) {
    //        $($(newRow).find('input[id=txtOrder]')[0]).val($(newRow)[0].rowIndex);
    //    }

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

        // reindex the drop orders GRD: Now done manually
        //        $(tableID + ' tr').each(function() {
        //            var row = $(this);
        //            $($(row).find('input[id=txtOrder]')[0]).val($(row)[0].rowIndex);
        //        });

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
        //if (parseFloat(businessTypes[arrayIndex][2]) + parseFloat(pallets) > 0)
        businessTypes[arrayIndex][2] = parseFloat(businessTypes[arrayIndex][2]) + parseFloat(pallets);
        //else {
        // remove the business type selection
        //  businessTypes.remove(arrayIndex);
        //}
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


    var orderID = $(this).parent().parent().find('span[id=orderID]').text();
    if (orderID == "") {
        orderID = $(el).closest('table').find('span[id=orderID]').text();
    }
    // just call the click on the deliveries grid this will do the work for us.
    $('table[id *=grdDeliveries] a:contains(' + orderID + ')').parent().parent().parent().find('input[id*=chkOrderID]').click();

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
        //        $(tableID + ' tr').each(function() {
        //            var row = $(this);
        //            $($(row).find('input[id=txtOrder]')[0]).val($(row)[0].rowIndex);
        //        });
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

    $('#columnDisplayAccordion').accordion({ collapsible: false, heightStyle: 'content' });

    // Determine available screen width and size grid to fit this
});


// Function to display the column configure box 
function ColumnDisplayShow() {
    $("#tabs").css({ 'display': 'none' });
    $("#dvColumnDisplay").css({ 'display': 'block' });
}

// Function to hide the column configure box 
function ColumnDisplayHide() {
    $("#tabs").css({ 'display': 'block' });
    $("#dvColumnDisplay").css({ 'display': 'none' });
}

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
            location.href = location.href;
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
                    url: "DeliveriesNew.aspx/ShowSubbyManfest",
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
                    url: "DeliveriesNew.aspx/GenerateAndShowManifest",
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
                url: "DeliveriesNew.aspx/GenerateAndShowPil",
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
                url: "DeliveriesNew.aspx/GenerateAndShowLoadingSheet",
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

        var url = "DeliveriesNew.aspx" + getCSIDSingle();

        location.href = url;
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

function ShowBookIn(el, orderID, deliveryFromDate, deliveryDate) {
    //getting height and width of the message box
    var pos = findPos(el);
    var height = $('#bookInWindow').height();
    var width = $('#bookInWindow').width();
    var delSpan = $('#bookedInTime');
    var delDate = $(delSpan).find('input[id*=txtBookInByFromDate]');
    var delTime = $(delSpan).find('input[id*=txtBookInByFromTime]');
    var deliveryByTime = $(delSpan).find('input[id*=txtBookInFromTime]');
    var dFromDate = new Date(deliveryFromDate);
    var dDate = new Date(deliveryDate);
    height += 10;
    width += 10;
    
    //calculating offset for displaying popup message

    //show the popup message and hide with fading effect
    $('#bookInWindow').css({ left: "100px", top: "200px" }).show();
    $('#txtBookedInWith').val("");
    $('#txtBookedInReferences').val("");
    //Show the same booking in times as shown on the order
    $('input[id*=txtBookInFromDate]').val(dFromDate.getUTCDate() + '/' + (dFromDate.getUTCMonth() +1) + '/' + dFromDate.getUTCFullYear());
    $('input[id*=txtBookInFromTime]').val(('0' + dFromDate.getUTCHours()).slice(-2) + ':' + ('0' + dFromDate.getUTCMinutes()).slice(-2));
    delDate.val(dDate.getUTCDate() + '/' + (dDate.getUTCMonth() +1) + '/' + dDate.getUTCFullYear());
    delTime.val(('0' + dDate.getUTCHours()).slice(-2) + ':' + ('0' + dDate.getUTCMinutes()).slice(-2));
    if (deliveryFromDate == deliveryDate) {
        $('#optTimeT').prop('checked', true);
        delDate.hide();
        delTime.hide();
        deliveryByTime.val(delTime.val());
    }
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

    var url = "deliveriesnew.aspx" + getCSIDSingle();

    location.href = url;

}


function CancelAndResetScreen() {
    // block UI while Processing message
    showLoading();
    
    var url = "deliveriesnew.aspx" + getCSIDSingle();

    location.href = url;

    return;
    
//    try {
//        if (isUpdating)
//            location.href = 'deliveriesnew.aspx';
//        else {
//            // uncheck anything checked on the grid
//            var _orderIDS = orders.split(',');
//            for (var x = 0; x < _orderIDS.length; x++)
//                $('table[id *=grdDeliveries] span[orderID=' + _orderIDS[x] + '] > input').click();

//            //reset the variables
//            orders = "";
//            jobs = "";
//            isUpdating = false;
//            totalWeight = 0;
//            totalPallets = 0;
//            totalSpaces = 0;

//            $("#tabs").tabs({ active: 2 })
//        }
//    }
//    catch (e) {

//    }

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

    var url = "/job/job.aspx?jobId=" + jobID + getCSID();

    var popup = window.open(url);
    window.setTimeout(function () {
        if (!popup || popup.outerHeight == 0) {
            alert('The run details cannot be displayed because your browser is blocking popup windows.  Please add this website to your browser\'s exception list.');
        }
    }, 500);
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
        $('#rbWholeJob').prop("disabled", true);
        $('#rbPerOrder').prop("checked", true);
        txtSubContractRate.disable();
    }
    else {
        $('#trTrunkDate').hide();
        $('#rbWholeJob').prop("disabled", false);
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