//=============================================================================
// 
//      Javascript Functions that are the same as the deliveries screen
//
//============================================================================= 

//Order Selection and Group Handling
function HandleGroupSelection(orderGroupID) {
    if (groupHandlingIsActive) {
        return;
    }
    groupHandlingIsActive = true;

    var dataItems = mtv.get_dataItems();

    for (var rowIndex = 0; rowIndex < dataItems.length; rowIndex++) {
        try {
            if (dataItems[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID) {
                var chkOrderID = dataItems[rowIndex].get_element().childNodes[1].childNodes[0];
                // If the checkbox has been found, and is not selected - tick the checkbox.
                if (chkOrderID !== null && !$(chkOrderID).children()[0].checked) {
                    $(chkOrderID).children()[0].click();

                }
            }
        }
        catch (error) {
        }
    }

    groupHandlingIsActive = false;
}

function HandleGroupDeselection(orderGroupID) {
    if (groupHandlingIsActive)
        return;

    groupHandlingIsActive = true;

    var dataItems = mtv.get_dataItems();

    for (var rowIndex = 0; rowIndex < dataItems.length; rowIndex++) {
        try {
            if (dataItems[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID) {
                var chkOrderID = dataItems[rowIndex].get_element().childNodes[1].childNodes[0];
                // If the checkbox has been found, and is selected - untick the checkbox.
                if (chkOrderID != null && $(chkOrderID).children()[0].checked)
                    $(chkOrderID).children()[0].click();
            }
        }
        catch (error) {
        }
    }

    groupHandlingIsActive = false;
}

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

    if (refusals == "" && _orderID != undefined)
        refusals = _orderID;

    if (refusals.length > 0) {
        if (refusals.indexOf(",") == -1 || confirm("This will change the business type for multiple orders, are you sure?")) {
            showLoading();
            PageMethods.UpdateBusinessType(refusals, businessTypeID, userName, UpdateBusinessType_Success, UpdateBusinessType_Failure);
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
}

function showTotals(changed) {
    // This now has been extended to show and allow job creation
    // if we have clicked on an order without a jobID we can include in to the load builder

    var __orders = refusals.split(',');
    var __count = __orders[0] == "" ? 0 : __orders.length;
    var el = document.getElementById("dvSelected");
    el.innerHTML = "<div>Number of Refused Goods : " + __count + "</div>";
}

//#endregion

//#region Remove order handling

function removeOrder(el) {
    var orderID = $(this).parent().parent().find('span[id=orderID]').text();
    if (orderID == "") {
        orderID = $(el).closest('table').find('span[id=orderID]').text();
    }
    // just call the click on the orders grid this will do the work for us.
    $('table[id *=grdRefusals] a:contains(' + orderID + ')').parent().parent().parent().find('input[id*=chkOrderID]').click();

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
    var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
}

//Document Load/Init Events
//#region Document Load/Init Events

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
    $(".loadBuilder").css({ 'height': '355px' });
    $(".loadBuilderInner").css({ 'height': '290px' });
    $(".jobView").css({ 'height': '355px' });
    $(".jobViewInner").css({ 'height': '290px' });
    $(".expandTab").css({ 'display': 'block' });
    $(".detractTab").css({ 'display': 'none' });
}

//#endregion

//Create Delivery Job
//#region Create Delivery Job        
function CreateReturns_Success(result) {
    try {
        var parts = result.toString().split("|");
        var runid = 0;

        if ($('#chkShowJobOnCreation')[0].checked) {
            runid = parts[0].split(":")
            ViewJob(runid[1]);
        }

        if (parts.length > 1) {
            // we have a job and the manifest
            var excludeFirstRow = $('#chkExcludeFirstRow')[0].checked;
            var extraRows = $('#txtExtraRows').val();
            var showFullAddress = $('#chkShowFullAddress')[0].checked;
            var usePlannedTimes = $('input:radio[name*=rblOrdering][checked]').val() == 0;
            var rmID = parts[1];

            if (parts.length == 3) {
                // Subby
                PageMethods.ShowSubbyManfest(rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress, runid[1], ManifestGeneration_Success, ManifestGeneration_Failure);
            }
            else
                PageMethods.GenerateAndShowManifest(rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress, runid[1], ManifestGeneration_Success, ManifestGeneration_Failure);
        }
        else if ($('#chkCreateLoadingSheet')[0].checked)
            PageMethods.GenerateAndShowLoadingSheet(runid[1], LoadingSheet_Success, LoadingSheet_Failure);
        else
            hidPostBackOnSuccess.click();        
            //location.href = location.href;
    }
    catch (e) {
    }
    hideLoading();
}

function CreateReturns_Failure(error) {
    hideLoading();
    alert("Something went wrong when creating the Return, please try again.\n" + error.get_message());
}

//#endregion


//#region Update Collection Job
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
        var url = "collectionsnew.aspx" + getCSIDSingle();

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
        window.open("/manifest/viewmanifest.aspx?rmID=" + resourceManifestID + "&excludeFirstLine=false;extraRows=0;usePlannedTimes=false;");
}

//#endregion

//#region Find/Load Existing Job

function FindJob() {
    var JobID = $('#txtJobID').val();
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

function BookIn_Success(result) {
    // change the icon on the book in control
    hideLoading();
}

function BookIn_Failure(error) {
    alert(error.get_message());
    hideLoading();
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
        hidPostBackOnSuccess.click();

    hideLoading();
}

function RemoveOrderFromJob_Failure(error) {
    hideLoading();
    alert("There was a problem removing the order from the run, please try again.");
}

function ResetScreen() {
    showLoading();
    location.href = location.href;
}

function CancelAndResetScreen() {
    // block UI while Processing message
    showLoading();
    try {
        if (isUpdating)
            location.href = location.href;
        else {
            // uncheck anything checked on the grid
            var _orderIDS = refusals.split(',');
            for (var x = 0; x < _orderIDS.length; x++)
                $('table[id *=grdRefusals] span[orderID=' + _orderIDS[x] + '] > input').click();

            //reset the variables
            refusals = "";
            jobs = "";
            isUpdating = false;
            totalWeight = 0;
            totalPallets = 0;
            totalSpaces = 0;

            CancelUpdateJob();
        }
    }
    catch (e) {

    }

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

    var _orders = refusals.split(',');
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
}

//#endregion

function ViewJob(jobID) {

    var url = "/job/job.aspx?jobId=" + jobID + getCSID();

    window.open(url);
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

function ManifestDate_OnDateChanged(sender, eventArgs) {
    $('#txtManifestTitle').val(cboDriver.get_text() + " - " + dteManifestDate.get_displayValue());
}

function cboSubContractor_SelectedIndexChanged(sender, eventArgs) {
    $('#txtManifestTitle').val(sender.get_text() + " - " + dteManifestDate.get_displayValue());
}

//#endregion

//=============================================================================
// 
//  Javascript Functions that exist in the deliveries screen but are different
//
//=============================================================================

function ChangeList(e, src, refusalID, collectionPoint, deliveryPoint, businessTypeID, businessType, collectionCustomerName, collectionAddress, deliveryPointId) {
    if (src.checked) {

        // Add to the list
        if (refusals.length > 0)
            refusals += ",";

        refusals += refusalID;

        $(src).closest('tr').toggleClass("SelectedRow_Orchestrator");

        RowSelected(refusalID, collectionPoint, deliveryPoint, businessTypeID, businessType, collectionCustomerName, collectionAddress);
    }
    else {
        // remove from the list
        refusals = refusals.replace(refusalID + ",", "");
        refusals = refusals.replace("," + refusalID, "");
        refusals = refusals.replace(refusalID, "");
        
        $(src).closest('tr').toggleClass("SelectedRow_Orchestrator");

        RowDeSelected(refusalID, businessTypeID, businessType);
    }
}

function RowSelected(refusalID, collectionPoint, deliveryPoint, businessTypeID, businessType, collectionCustomerName, collectionAddress) {
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
        //dropOrder = parseInt($($(lastRow).find('input[id=txtOrder]')).val());
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
    $(newRow.find('span[id*=refusalID]')[0]).text(refusalID);
    $(newRow.find('span[id*=collectionPoint]')[0]).html(unescape(collectionCustomerName) + " <br/> " + collectionAddress);
    $(newRow.find('span[id*=deliveryPoint]')[0]).text(unescape(deliveryPoint));

    if (isUpdating) {
        $($(newRow).find('input[id=txtOrder]')[0]).val("");
        $($(newRow).find('input[id=txtOrder]')[0]).css("disabled", "disabled");
    }

    showTotals(true);
    $('#createCollectionJob').css("display", "");
}

function RowDeSelected(refusalID, businessTypeID, businessType) {
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
        $($(tableID + ' tr td span:contains(' + refusalID + ')')).parent().parent().remove();
        $('#createCollectionJob').css("display", "");
    }
    else {
        table.hide();
        templateRowused = false;
        dropOrder = 0;

        // disable the create Collection Job Button
        $('#createCollectionJob').css("display", "none");
    }
}

$(document).ready(function() {
    $(":checkbox[id*='chkSelectAllTrafficAreas']").click(function() {
        var checked_status = this.checked;
        $(":checkbox[id*='cblTrafficAreas']").each(function() {
            this.checked = checked_status;

        });
    });
    // Set up the loadbuilder UI
    $("#tabs").tabs({ collapsible: true, active: false });

    //Display the pop-up box, but only when the page has been rendered
    $("#overlayedClearDataBox").show();

    // Makes the overlay box dragable, but only when the mouse is over the #dragLI icon
    $("#overlayedClearDataBox").draggable({ handle: '#dragLI' });

    $('#chkDeliveryPoint').click(function() {
        if (this.checked) {
            $('#deliveryPointText').text("You will be delivering the orders to the cross dock location as specified. This will create a run with 1 or more collections and 1 delivery");
            $('#trDeliveryPoint').show();
        }
        else {
            $('#deliveryPointText').text("The orders selected will be to their Delivery points. This will create a run with 1 or more collections and 1 or more deliveries");
            $('#trDeliveryPoint').hide();
        }
    });

    $('#chkCreateRun').click(function() {
        if (this.checked)
            $('#runOptions').show();
        else
            $('#runOptions').hide();
    });

    $('#chkLeaveGoods').click(function() {
        if (this.checked)
            $('#dvCrossDockLocation').show();
        else
            $('#dvCrossDockLocation').hide();
    });

    $('#chkCreateManifest').click(function() {
        if (this.checked)
            $('#manifestOptions').show();
        else
            $('#manifestOptions').hide();
    });

    $('#chkUpdateManifest').click(function() {
        if (this.checked)
            $('#manifestOptions').show();
        else
            $('#manifestOptions').hide();
    });

    $(':radio[name=rdoresource]').click(function() {
        if (this.value == "1") {
            $('#pnlSubContractor').show();
            $('#pnlOwnresource').hide();
        }
        else {
            $('#pnlSubContractor').hide();
            $('#pnlOwnresource').show();
        }
    });

    // Hide the column configure box
    $("#dvColumnDisplay").css({ 'display': 'none' });

    // Changes the command row to green
    $(".rgCommandRow").addClass("GridGreenCommandRow");
    $(".GridGreenCommandRow").removeClass("rgCommandRow");
});

function CreateReturns() {
    showLoading();

    var refusalIDs = new Array();
    var createRun = $('#chkCreateRun')[0].checked;
    var leaveGoods = $('#chkLeaveGoods')[0].checked;

    var resourceName = "";
    var subcontractType = -1; // whole job or per order;
    var subContractRate = -1;
    var collectionDetailsSet = true, leaveDetailsSet = true, deliverDetailsSet = true;
    var createManifest = false, showAsCommunicated = false, collectionIsAnytime = false, leaveIsAnytime = false, deliveryIsAnytime = false;
    var leavePointID = -1, changeDeliveryPointID = -1, refusalCount = 0, driverResourceID = -1, vehicleResourceID = -1, trailerResourceID = -1, planningCategoryID = -1;
    var leaveFromDate = null, leaveFromTime = null, leaveByDate = null, leaveByTime = null;
    var collectionFromDate = null, collectionFromTime = null, collectionByDate = null, collectionByTime = null;
    var changeDeliveryFromDate = null, changeDeliveryFromTime = null, changeDeliveryByDate = null, changeDeliveryByTime = null, subcontractorIdentityID = null, manifestDate = null, manifestTitle = null;

    var selectedBuisnessTypeID = rcbBusinessType.get_value();
    
    if (selectedBuisnessTypeID == null) {
        selectedBuisnessTypeID = 1; // currently Full Load.
    }

    // Check collection details
    if (dteCollectionFromDate.isEmpty())
        collectionDetailsSet = false;
    else
        collectionFromDate = dteCollectionFromDate.get_selectedDate();

    if (dteCollectionFromTime.isEmpty())
        collectionDetailsSet = false;
    else
        collectionFromTime = dteCollectionFromTime.get_selectedDate();

    if (dteCollectionByDate.isEmpty())
        collectionDetailsSet = false;
    else
        collectionByDate = dteCollectionByDate.get_selectedDate();

    if (dteCollectionByTime.isEmpty())
        collectionDetailsSet = false;
    else
        collectionByTime = dteCollectionByTime.get_selectedDate();

    collectionIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]')[0].checked;
    if (collectionIsAnytime == null || collectionIsAnytime == undefined)
        collectionDetailsSet = false;

    if (!collectionDetailsSet) {
        alert("Please make sure a collection date and time is specified.");
        // redirect back to the correct tab and control
        $("#tabs").tabs({ active: 1 });
        hideLoading();
        return;
    }

    // pull this all together to create the delivery job
    deliverDetailsSet = $('#chkDeliveryPoint')[0].checked;
    if (deliverDetailsSet) {
        var point = $find('ctl00_ContentPlaceHolder1_ucDeliveryPoint_cboPoint');
        var pointParts = $find('ctl00_ContentPlaceHolder1_ucDeliveryPoint_cboPoint').get_value().split(',');
        changeDeliveryPointID = pointParts[1];

        if (dteDeliveryByDate.isEmpty()) {
            alert("Please enter when you plan to return the goods to the selected location.");

            // redirect back to the correct tab and control
            $("#tabs").tabs({ active: 1 });
            hideLoading();
            return;
        }
        else {
            changeDeliveryFromDate = dteDeliveryFromDate.get_selectedDate();
            changeDeliveryByDate = dteDeliveryByDate.get_selectedDate();
        }

        if (dteDeliveryByTime.isEmpty()) {
            alert("Please enter when you plan to return the goods to the selected location.");

            // redirect back to the correct tab and control
            $("#tabs").tabs({ active: 1 });
            hideLoading();
            return;
        }
        else {
            changeDeliveryFromTime = dteDeliveryFromTime.get_selectedDate();
            changeDeliveryByTime = dteDeliveryByTime.get_selectedDate();
        }

        deliveryIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]')[0].checked;
        if (deliveryIsAnytime == null || deliveryIsAnytime == undefined)
            deliveryIsAnytime = false;

         if (changeDeliveryPointID == null) {
            alert("Please enter the location where you plan to return the goods.");

            // redirect back to the correct tab and control
            $("#tabs").tabs({ active: 1 });
            hideLoading();
            return;
        }
    }

    // Get the Orders selected and the order for delivery
    $('#tblLoadBuilderOrders span[id*=refusalID]').each(function() {
        refusalIDs[refusalCount] = $(this).text();
        refusalCount++;
    });

    if (createRun) {

        if (leaveGoods) {
            var pointParts = $find('ctl00_ContentPlaceHolder1_ucLeaveLocation_cboPoint').get_value().split(',');
            leavePointID = pointParts[1];

            if (leavePointID == null) {
                alert("Please enter the location where you plan to leave the goods.");

                // redirect back to the correct tab and control
                $("#tabs").tabs({ active: 2 });
                hideLoading();
                return;
            }

            // Check collection details
            if (dteLeaveFromDate.isEmpty())
                leaveDetailsSet = false;
            else
                leaveFromDate = dteLeaveFromDate.get_selectedDate();

            if (dteLeaveFromTime.isEmpty())
                leaveDetailsSet = false;
            else
                leaveFromTime = dteLeaveFromTime.get_selectedDate();

            if (dteLeaveByDate.isEmpty())
                leaveDetailsSet = false;
            else
                leaveByDate = dteLeaveByDate.get_selectedDate();

            if (dteLeaveByTime.isEmpty())
                leaveDetailsSet = false;
            else
                leaveByTime = dteLeaveByTime.get_selectedDate();

            leaveIsAnytime = $('input:radio[id*=rdLeaveIsAnytime]')[0].checked;
            if (leaveIsAnytime == null || leaveIsAnytime == undefined)
                leaveDetailsSet = false;

            if (!leaveDetailsSet) {
                alert("Please make sure a leave date and time is specified.");
                // redirect back to the correct tab and control
                $("#tabs").tabs({ active: 2 });
                hideLoading();
                return;
            }
        }

        // Determine who is doing the job
        if ($(':radio[id=rbOwnResource]')[0].checked) {
            var cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
            driverResourceID = cboDriver.get_value() == "" ? -1 : parseInt(cboDriver.get_value());

            var cboVehicle = $find('ctl00_ContentPlaceHolder1_ucResource_cboVehicle');
            vehicleResourceID = cboVehicle.get_value() == "" ? -1 : parseInt(cboVehicle.get_value());

            var cboTrailer = $find('ctl00_ContentPlaceHolder1_ucResource_cboTrailer');
            trailerResourceID = cboTrailer.get_value() == "" ? -1 : parseInt(cboTrailer.get_value());

            resourceName = cboDriver.get_text();
        }
        else if ($(':radio[id=rbSubContractor]')[0].checked) {
            subcontractorIdentityID = cboSubContractor.get_value();
            resourceName = cboSubContractor.get_text();
            subcontractType = $(':radio[name=SubContractOption]:checked').val();
            subContractRate = txtSubContractRate.get_value();
        }

        createManifest = $('#chkCreateManifest')[0].checked;
        showAsCommunicated = $('#chkShowInProgress')[0].checked;

        if (driverResourceID == -1 && subcontractorIdentityID == null && createManifest) {
            alert("You cannot create a manifest without selecting a driver or subcontractor.")
            $("#tabs").tabs({ active: 2 });
            hideLoading();
            return;
        }
        
        if (createManifest) {
            manifestDate = dteManifestDate.get_selectedDate();
            manifestTitle = resourceName + " - " + dteManifestDate.get_displayValue();
        }
    }

    PageMethods.CreateReturn(refusalIDs, selectedBuisnessTypeID, deliverDetailsSet, changeDeliveryPointID, changeDeliveryFromDate, changeDeliveryFromTime, changeDeliveryByDate, changeDeliveryByTime, deliveryIsAnytime, driverResourceID, vehicleResourceID, trailerResourceID, planningCategoryID, userName, userID, createManifest, resourceName, subcontractorIdentityID, subcontractType, subContractRate, showAsCommunicated, manifestTitle, manifestDate, createRun, collectionFromDate, collectionFromTime, collectionByDate, collectionByTime, collectionIsAnytime, leaveGoods, leavePointID, leaveFromDate, leaveFromTime, leaveByDate, leaveByTime, leaveIsAnytime, CreateReturns_Success, CreateReturns_Failure);
}

function UpdateCollectionJob() {
    var instructionID = null;
    var trunkDeliveryPointID = -1
    var trunkDeliveryDate = null;
    var trunkDeliveryTime = null;

    if (!dteDeliveryTime.isEmpty())
        trunkDeliveryTime = dteDeliveryTime.get_selectedDate();

    // pull this all together for the update
    if ($('#trDeliveryPoint').is(':visible') && $('#chkLoadAndGo')[0].checked == false && $('#tblInstructions input[id*=chkAddToInstruction]:checked').length == 0) {
        var point = $find('ctl00_ContentPlaceHolder1_ucDeliveryPoint_cboPoint');
        var pointParts = $find('ctl00_ContentPlaceHolder1_ucDeliveryPoint_cboPoint').get_value().split(',');
        trunkDeliveryPointID = pointParts[1];

        if (dteDeliveryDate.isEmpty()) {
            alert("Please enter when you plan to deliver the orders to the cross-dock location.");

            // redirect back to the correct tab and control
            $("#tabs2").tabs({ active: 1 });
            return;
        }
        trunkDeliveryDate = dteDeliveryDate.get_selectedDate();
    }

    // Are collecting from an Existing instruction on the job
    if ($('#tblInstructions input[id*=chkAddToInstruction]:checked').length > 0)
        instructionID = $('#tblInstructions input[id*=chkAddToInstruction]:checked').attr("instructionID");

    showLoading();

    var __orders = new Array();
    var ordersList = refusals.split(",");
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

    if ($(':radio[id=rbOwnResource]')[0].checked) {
        var cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
        resourceName = cboDriver.get_text();
    }
    else if ($(':radio[id=rbSubContractor]')[0].checked) {
        resourceName = cboSubContractor.get_text();
    }

    PageMethods.UpdateJob(jobID, __orders, trunkDeliveryPointID, trunkDeliveryDate, trunkDeliveryTime, instructionID, userName, userID, updateManifest, manifestDate, manifestTitle, resourceName, UpdateJob_Success, UpdateJob_Failure);
}

function BookIn(el, orderID) {
    try {
        showLoading();

        PageMethods.BookIn(orderID, userName, BookIn_Success, BookIn_Failure)

        $(el).attr('src', '/images/tick.gif');
    }
    catch (e) {
        alert(e);
    }

    hideLoading();
}

function ShowPosition(latitude, longitude) {
    var url = "/gps/getcurrentlocation.aspx?lat=" + latitude + "&lng=" + longitude;
    window.open(url, "", "height=600, width=630, scrollbars=0");
}

//=============================================================================
// 
//       Javascript Functions that only exist in the collections screen
//
//=============================================================================

function HandleDeliveryJobSelection(orderID, deliveryPointId, deliveryPoint) {
    if (ordersSelectedOnDeliveryJob.length > 0)
        ordersSelectedOnDeliveryJob += ",";

    ordersSelectedOnDeliveryJob += orderID;

    $('#chkDeliveryPoint')[0].checked = true;
    $('#chkDeliveryPoint')[0].disabled = true;
    $('#trDeliveryPoint').show();

    if (ordersOnDeliveryJobCrossDockPointIds.length > 0)
        ordersOnDeliveryJobCrossDockPointIds += ",";

    ordersOnDeliveryJobCrossDockPointIds += deliveryPointId + "|" + deliveryPoint;

    var existingDeliveryPoint = ordersOnDeliveryJobCrossDockPointIds.split(',');
    var crossDockPointId = -1;
    var crossDockDescription = "";
    var crossDockDetails = "";
    var multipleCrossDockPoints = false;

    if (existingDeliveryPoint != null && existingDeliveryPoint.length > 0)
        crossDockDetails = existingDeliveryPoint[0];

    for (i = 0; i < existingDeliveryPoint.length; i++)
        if (crossDockDetails != existingDeliveryPoint[i]) {
        multipleCrossDockPoints = true;
        break;
    }

    var point = $find('ctl00_ContentPlaceHolder1_ucDeliveryPoint_cboPoint');

    if (multipleCrossDockPoints) {
        point.set_text(defaultGroupageCollectionRunDeliveryPointDescription);
        point.set_value("''," + defaultGroupageCollectionRunDeliveryPointId);

        $('#deliveryPointText').text("You will be delivering the orders to the cross dock location specified. The cross dock location is currently set to the default location as multiple ordrs have different cross dock locations. This will create a run with 1 or more collections and 1 delivery");
    }
    else {
        crossDockDetails = crossDockDetails.split('|');

        if (crossDockDetails.length > 0) {
            crossDockPointId = crossDockDetails[0]; //Id
            crossDockDescription = crossDockDetails[1]; //desc
        }

        point.set_text(crossDockDescription);
        point.set_value("''," + crossDockPointId);

        $('#deliveryPointText').text("You will be delivering the orders to the cross dock location specified. This will create a run with 1 or more collections and 1 delivery");
    }
}

function HandleDeliveryJobDeselection(orderID, deliveryPointId, deliveryPoint) {
    // remove from the list
    ordersSelectedOnDeliveryJob = ordersSelectedOnDeliveryJob.replace(orderID + ",", "");
    ordersSelectedOnDeliveryJob = ordersSelectedOnDeliveryJob.replace("," + orderID, "");
    ordersSelectedOnDeliveryJob = ordersSelectedOnDeliveryJob.replace(orderID, "");

    if (ordersSelectedOnDeliveryJob.length == 0)
        $('#chkDeliveryPoint')[0].disabled = false;

    var entryToRemove = deliveryPointId + "|" + deliveryPoint;
    var entries = ordersOnDeliveryJobCrossDockPointIds.split(',');
    var newList = "";
    var entryRemoved = false;

    // this removes just one entry from the list as there maybe duplicates.
    for (j = 0; j < entries.length; j++) {
        if (entryToRemove == entries[j] && entryRemoved == false)
            entryRemoved = true;
        else {
            if (newList.length > 0)
                newList += ","

            newList += entries[j]
        }
    }

    // remove from the list
    ordersOnDeliveryJobCrossDockPointIds = newList;

    var crossDockPointId = -1;
    var crossDockDescription = "";
    var multipleCrossDockPoints = false;
    var crossDockDetails = "";

    if (ordersOnDeliveryJobCrossDockPointIds.length > 0) {
        var existingDeliveryPoint = ordersOnDeliveryJobCrossDockPointIds.split(',');

        if (existingDeliveryPoint != null && existingDeliveryPoint.length > 0) {
            crossDockDetails = existingDeliveryPoint[0];
        }

        for (i = 0; i < existingDeliveryPoint.length; i++)
            if (crossDockDetails != existingDeliveryPoint[i]) {
            multipleCrossDockPoints = true;
            break;
        }
    }
    else
        multipleCrossDockPoints = true;

    var point = $find('ctl00_ContentPlaceHolder1_ucDeliveryPoint_cboPoint');

    if (multipleCrossDockPoints) {
        point.set_text(defaultGroupageCollectionRunDeliveryPointDescription);
        point.set_value("''," + defaultGroupageCollectionRunDeliveryPointId);

        $('#deliveryPointText').text("You will be delivering the orders to the cross dock location specified. The cross dock location is currently set to the default location as multiple ordrs have different cross dock locations. This will create a run with 1 or more collections and 1 delivery");
    }
    else {
        crossDockDetails = crossDockDetails.split('|');

        if (crossDockDetails.length > 0) {
            crossDockPointId = crossDockDetails[0]; //Id
            crossDockDescription = crossDockDetails[1]; //desc
        }

        point.set_text(crossDockDescription);
        point.set_value("''," + crossDockPointId);
        $('#deliveryPointText').text("You will be delivering the orders to the cross dock location specified. This will create a run with 1 or more collections and 1 delivery");
    }
}

function reOrderExistingJobLoadBuilder(txtDropOrder) {
    // determine the index
    if ($(txtDropOrder).val() != $(txtDropOrder).parent().parent().rowIndex) {
        // the row has to be moved
        var row = $(txtDropOrder).parent().parent().before($('#tblExistingOrders tr:eq(' + $(txtDropOrder).val() + ')'));
    }

}