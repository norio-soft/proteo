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

function validateBatchLabels() {
    var isValid = true;

    //var txtBatches = $('input:text[id*=txtBatchId]').filter(function () { return $(this).attr("value") == "" });
    //for (var i = 0; i < txtBatches.length; i++)
    //{
    //    var currentTxtBatch = $("#" + txtBatches[i].id);
    //    currentTxtBatch.css("background-color", "");
    //}

    // Find all the textboxes on the page that have a value currently set.
    var txtBatches = $('input:text[id*=txtBatchId]').filter(function () { return $(this).attr("value") != "" });

    for (var i = 0; i < txtBatches.length; i++) {
        var localIsValid = validateIndividualTextBox(txtBatches[i]);
        if (!localIsValid)
            isValid = localIsValid;
    }

    displayCreateBatchButtons(isValid);
}

function validateBatchLabel(txtBatch) {
    var isValid = validateIndividualTextBox(txtBatch);
    displayCreateBatchButtons(isValid);
}

function validateIndividualTextBox(txtBatch) {
    var selectedClientId = 0, selectedBatchLabel = "";
    var currentTxtBatch = $("#" + txtBatch.id);
    var txtBatches = null;
    var isValid = true, isIndividualValid = true;

    if (currentTxtBatch.length > 0) {
        selectedClientId = currentTxtBatch.attr("OCustomerIdentityId");
        selectedBatchLabel = currentTxtBatch.attr("OBatchLabel");
    }

    // Check for the selected Order's customer identityid.
    if (selectedClientId > 0) {
        // Get the text boxes new batchlabel.
        var newLabel = currentTxtBatch.val();

        if (newLabel != "") {
            // Check for an existing batch with the same batch label.
            var matchedbatch = jQuery.grep(existingBatchLabels, function (a) { return a[1] == newLabel });

            // If a match has been found and its not for the same client.
            if (matchedbatch.length > 0 && matchedbatch[0][0] != selectedClientId) {
                isIndividualValid = false;
            }

            // Only check this if the previous criteria has already passed.
            if (isIndividualValid) {
                // Check to see if the batch is being used in another textbox for a different client.
                txtBatches = $("input[id*=txtBatchId]");
                var foundMatch = jQuery.grep(txtBatches, function (a) { var b = $("#" + a.id); return b.attr("OCustomerIdentityId") != selectedClientId && b.val() == newLabel });

                // If a match has been found
                if (foundMatch.length > 0) {
                    isIndividualValid = false;
                }
            }
        }

        // Again only check if all of the previous tests have passed.
        if (isIndividualValid) {
            // If not already instanciated.
            if (txtBatches == null)
                txtBatches = $("input[id*=txtBatchId]");

            // Check to see of any other textboxes are still unhappy, if so return them.
            var foundMatch = jQuery.grep(txtBatches, function (a) { var b = $("#" + a.id); return a.id != currentTxtBatch.attr("id") && b.css("background-color") == "red" });

            // If any unhappy textboxes have been found, then we still cannot allow a batch to be created/updated.
            if (foundMatch.length > 0) {
                isValid = false;
            }
        }
        else
            isValid = false;

        if (isIndividualValid) // Remove background colour
            currentTxtBatch.css("background-color", "");
        else // Flag as a textbox that need modifying.
            currentTxtBatch.css("background-color", "red");
    }

    return isValid;
}

function displayCreateBatchButtons(isValid) {
    if (createBatch1 != null && createBatch2 != null) {
        if (isValid) { // Allow batch creation/updating.
            createBatch1.css("display", "");
            createBatch2.css("display", "");
        }
        else { // Prevent batch creation/updating.
            createBatch1.css("display", "none");
            createBatch2.css("display", "none");
        }
    }
}

var clickedItemIndex;
var selected;
var isIE;

function GridCreated(sender, args) {
    selected = new Array(sender.get_masterTableView().get_dataItems().length);
    isIE = navigator.appVersion.indexOf("MSIE") > 0;
}

function RowClick(sender, args) {
    //if (!isIE) {
        var master = sender.get_masterTableView();
        var index = args.get_itemIndexHierarchical();
        selected[index] = !master.get_dataItems()[index].get_selected()
        clickedItemIndex = index;
        master.get_dataItems()[index].set_selected(!master.get_dataItems()[index].get_selected());

        master.get_dataItems()[index]._element.children[0].children[0].checked = !master.get_dataItems()[index]._element.children[0].children[0].checked;
        SelectOrder(master.get_dataItems()[index]._element.children[0].children[0]);
    //    validateBatchLabels();
    //}
}

function RowDeselecting(sender, args) {
    var index = args.get_itemIndexHierarchical();
    if (isIE) {
        if (clickedItemIndex != index && selected[index]) {
            args.set_cancel(true);
        }
    }
    else {
        if (clickedItemIndex != index && selected[index]) {
            args.set_cancel(true);
        }
    }
}

function RowSelecting(sender, args) {
    var index = args.get_itemIndexHierarchical();
    var master = sender.get_masterTableView();

    if (isIE) {
        if (selected[index]) {
            args.set_cancel(true);
            selected[index] = false;
        }
        else {
            var index = args.get_itemIndexHierarchical();
            selected[index] = !master.get_dataItems()[index].get_selected()
        }
        clickedItemIndex = index;
    }
    else {
        if (clickedItemIndex == index && !selected[index]) {
            args.set_cancel(true);
        }
    }

}

function RowMouseOver(sender, args) {
    if (isIE) {
        clickedItemIndex = args.get_itemIndexHierarchical();
    }
}

//#region Select all Orders Action

//#endregion