function ramApprovalProcess_RequestStart(sender, eventArgs) {
    showLoading("Retrieving Rows");
}

function ramApprovalProcess_ResponseEnd(sender, eventArgs) {
    hideLoading();
}

function SelectAllItems(itemName) {
    var parents = document.getElementsByName(itemName);
    for (var index = 0; index < parents.length; index++) {
        var inputs = parents[index].getElementsByTagName("input");
        if (inputs.length > 0)
            if (inputs[0].style.display != 'none')
            inputs[0].checked = true;
    }
}

function selectCreatedBy(item) {
    var selected = true;
    for (var i = 0; i < $(item).children().children().children().length; i++)
    {
        if($($(item).children().children().children()[i]).children().length > 0)
        {
            if (!$($(item).children().children().children()[i]).children()[0].checked)
                selected = false;
        }
    }

    $('.chkSelectAllCreatedBy')[0].checked = selected;
}

function HideOption(clientID, substituteID) {
    var item = document.getElementById(clientID);
    if (item != null) {
        if (item.checked) {
            var substituteItem = document.getElementById(substituteID);
            if (substituteItem != null)
                substituteItem.checked = true;
        }

        item.style.display = 'none';
    }
}

function pendingChanges(invoiceRow) {
    // Find the parent row the contains the class rgRow.
    var hidIsDirty = invoiceRow.find('input[id*=hidIsDirty]');
    var lblPendingChanges = invoiceRow.find('span[id*=lblPendingChanges]');
    var rdoDoNothing = invoiceRow.find('input[id*=rdoDoNothing]');
    var rdoReject = invoiceRow.find('input[id*=rdoReject]');
    var rdoApprove = invoiceRow.find('input[id*=rdoApprove]');
    var rdoApproveAndPost = invoiceRow.find('input[id*=rdoApproveAndPost]');

    hidIsDirty.val("True");

    lblPendingChanges.text("Pending Changes...");
    lblPendingChanges.css("display", "");

    rdoDoNothing.css("display", "none");
    rdoReject.css("display", "none");
    rdoApprove.css("display", "none");
    rdoApproveAndPost.css("display", "none");
}

function rdiInvoiceDate_OnClientValueChanged(source, eventArgs) {
    var invoiceDateItem = $('#' + source.get_id());
    var invoiceRow = invoiceDateItem.parent().parent().parent();
    pendingChanges(invoiceRow);
}

function rtClientReference_OnClientValueChanged(source, eventArgs) {
    var invoiceDateItem = $('#' + source.get_id());
    var invoiceRow = invoiceDateItem.parent().parent().parent();
    pendingChanges(invoiceRow);
}

function rtPurchaseOrderReference_OnClientValueChanged(source, eventArgs) {
    var invoiceDateItem = $('#' + source.get_id());
    var invoiceRow = invoiceDateItem.parent().parent().parent();
    pendingChanges(invoiceRow);
}

function cboTaxRate_OnClientSelectedIndexChanged(source) {
    var invoiceDateItem = $('#' + source.id);
    var invoiceRow = invoiceDateItem.parent().parent();
    pendingChanges(invoiceRow);
}

function txtRate_OnClientValueChanged(source) {
    var invoiceDateItemChildRow = $('#' + source.id);
    var hidInvoiceItemIsDirty = invoiceDateItemChildRow.parent().find('input[id*=hidInvoiceItemIsDirty]');
    hidInvoiceItemIsDirty.val("True");

    var invoiceRow = null;
    var invoiceChildRowId = invoiceDateItemChildRow.parent().parent().parent().parent().attr("id");
    var invoiceChildRow = $find(invoiceChildRowId);

    if (invoiceChildRow != null) {
        invoiceRow = invoiceChildRow.get_parentRow();

        var jQueryInvoiceRow = $('#' + invoiceRow.id);
        pendingChanges(jQueryInvoiceRow);
    }
}

function preInvoiceOrder_Remove(source) {

    var setDelete = confirm("Do you want to flag this order for removal?");

    if (setDelete) {

        var invoiceDateItemChildRow = $('#' + source.id);
        var hidInvoiceItemPendingDelete = invoiceDateItemChildRow.parent().find('input[id*=hidInvoiceItemPendingDelete]');
        hidInvoiceItemPendingDelete.val("True");

        setInvoiceItemToPendingDelete(invoiceDateItemChildRow);

        var invoiceRow = null;
        var invoiceChildRowId = invoiceDateItemChildRow.parent().parent().parent().parent().attr("id");
        var invoiceChildRow = $find(invoiceChildRowId);

        if (invoiceChildRow != null) {
            invoiceRow = invoiceChildRow.get_parentRow();

            var jQueryInvoiceRow = $('#' + invoiceRow.id);
            pendingChanges(jQueryInvoiceRow);

            return false;
        }
    }
}

function setInvoiceItemToPendingDelete(item) {

    item.parent().parent().children().eq(2).html('Pending Delete');
    item.parent().parent().children().eq(4).children().eq(1).prop('disabled', true);
    item.parent().parent().children().eq(5).children().first().prop('disabled', true);

};

function gvPreInvoices_OnClientRowSelected(source, eventArgs) {
    var sourceStr = eventArgs._id;

//    // Check the source is the parent table, and if so then do the request.
//    if (sourceStr.search("Detail") == -1) {
//        var preInvoiceId = eventArgs.getDataKeyValue("PreInvoiceID")
//        ramApprovalProcess.ajaxRequest(preInvoiceId);
    //    }

    var preInvoiceId = eventArgs.getDataKeyValue("PreInvoiceID")
    __doPostBack('PreInvoiceSelectedIndexChanged', preInvoiceId);
}

// Block UI
//#region 
function showLoading(messageContent) {
    $.blockUI({
        message: '<div style="margin-left:30px;"><span id="UpdatableMessage">' + messageContent + '</span></div>',
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: '.5',
            color: '#fff'
        }
    });
}

function updateLoadingMessage(messageContent) {
    $('#UpdatableMessage').text(messageContent);
}

function hideLoading() {
    $.unblockUI();
}