$(document).ready(function() {

    $('#tabs').tabs();

});

function showHideDiv(divIdToShowHide, imageIdToChange, imgPath1, imgPath2) {
    // Show or hide content
    var div = document.getElementById(divIdToShowHide);

    if (div != null) {
        if (div.style.display == 'inline') {
            div.style.display = 'none';
        }
        else {
            div.style.display = 'inline';
        }
    }

    // Swap the image displayed
    var img = document.getElementById(imageIdToChange);

    if (img != null) {
        if (img.src.indexOf(imgPath1) > -1) {
            img.src = webserver + "/" + imgPath2;
        }
        else {
            img.src = webserver + "/" + imgPath1;
        }
    }
}

function HandleSelectAll(chk) {
    $(":checkbox[id$=chkExclude]").each(function(chkIndex) { if (this.checked != chk.checked) this.click(); });
}

function HandleSelectAllAdditional(chk) {
    $(":checkbox[id$=chkExcludeAdditional]").each(function(chkIndex) { if (this.checked != chk.checked) this.click(); });
}

function importFile_Success(result) {
    var parts = result.toString().split("|");

    if (parts.length == 1) {
        updateLoadingMessage('Matching to System Orders');
        PageMethods.ValidFileImport(validFileImport_Success, validFileImport_Failure);
    }
    else {
        var errorCodeID = parseInt(parts[0], 10);

        switch (errorCodeID) {
            case 1:
                var errorMessage = "";

                if (parts[1].length > 1);
                errorMessage = " Error Content : " + parts[1];

                alert("There was an error uploading the file." + errorMessage);
                break;
            case 2:
                alert("There was an error uploading the file, please select and try again.");
                break;
            case 3:
                alert("The file has already been imported.");
                break;
            case 4:
                alert("There was an error importing the file as the file does not currently exist on the server.");
                break;
            default:
                break;
        }

        enableProcessing();
        hideLoading();
    }
}

function importFile_Failure(error) {
    fileToUpload = null;
    hideLoading();
    alert(error.get_message());
}

function validFileImport_Success(result) {
    var parts = result.toString().split("|");

    if (parts.length == 1) {
        updateLoadingMessage('Creating Pre Invoice');
        PageMethods.CreatePreInvoice(createPreInvoice_Success, validFileImport_Failure);
    }
    else {
        var errorCodeID = parseInt(parts[0], 10);

        switch (errorCodeID) {
            case 1:
                var errorMessage = "";

                if (parts[1].length > 1);
                errorMessage = " Error Content : " + parts[1];

                alert("There was an error uploading the file." + errorMessage);
                break;
            default:
                break;
        }

        enableProcessing();
        hideLoading();
    }
}

function validFileImport_Failure(error) {
    fileToUpload = null;
    hideLoading();
    alert(error.get_message());
}

function createPreInvoice_Failure(error) {
    fileToUpload = null;
    hideLoading();
    alert(error.get_message());
}

function restoreOriginalValues_Success(result) {
    var parts = result.toString().split("|");

    if (parts.length > 1) {
        var errorCode = parseInt(parts[0], 10);
        var errorMessage = parts[1];

        switch (errorCode) {
            case 1:
                alert(errorMessage);
                break;
            default:
                alert("There was an error when trying to restore the original values. Please reload the page and try again.");
                break;
        }
    }
    else {
        updateLoadingMessage('Removing PreInvoice');
        hdnRemoveButton.click();
    }
}

function approvePreInvoice() {
    showLoading('Creating Invoice');
    PageMethods.CreateInvoice(createInvoice_Success, createInvoice_Failure);
    return false;
}

function createInvoice_Success(result) {
    hideLoading();
    hdnApproveButton.click();
}

function createInvoice_Failure(error) {
    hideLoading();
    alert(error.get_message());
}

function ramPreInvoiceAjaxRequest_OnResponseEnd(sender, eventArgs) {
    $('#tabs').tabs();

    if (unMatchedItemsID.length > 0) {
        var unMatchedItemCount = parseInt($("#" + unMatchedItemsID).text(), 10);
        if (unMatchedItemCount) {
            $('#tabs').tabs({ active: 1 });
            $('#spnUnMatchedTitle').text("Unmatched Items (" + unMatchedItemCount + ")");
        }
    }

    disbleProcessing();
    hideLoading();
}

function btnSubmit_OnClientAdded() {
    if (fileToUpload != null) {
        fileToUpload.submit();
    }
}

function btnRemove_OnClick() {
    showLoading('Restoring Original Values');
    PageMethods.RestoreOriginalValues(restoreOriginalValues_Success, restoreOriginalValues_Failure);
    return false;
}

function restoreOriginalValues_Failure(error) {
    alert(error.get_message());
    hideLoading();
}

function enableProcessing() {
    UploadInvoiceFile.show();
    UploadedFileDetails.hide();
}

function disbleProcessing() {
    UploadInvoiceFile.hide();
    UploadedFileDetails.show();
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

function toggleGroup(img, numberOfRows) {
    //  get a reference to the row and table
    var tr = img.parentNode.parentNode;
    var src = img.src;

    //  do some simple math to determine how many
    //  rows we need to hide/show
    var startIndex = tr.rowIndex + 1;
    var stopIndex = startIndex + parseInt(numberOfRows);

    //  if the img src ends with plus, then we are expanding the
    //  rows.  go ahead and remove the hidden class from the rows
    //  and update the image src
    if (src.endsWith('plus.gif')) {
        for (var i = startIndex; i < stopIndex; i++) {
            Sys.UI.DomElement.removeCssClass(table.rows[i], 'hidden');
        }

        src = src.replace('plus.gif', 'minus.gif');
    }
    else {
        for (var i = startIndex; i < stopIndex; i++) {
            Sys.UI.DomElement.addCssClass(table.rows[i], 'hidden');
        }

        src = src.replace('minus.gif', 'plus.gif');
    }

    //  update the src with the new value
    img.src = src;
}