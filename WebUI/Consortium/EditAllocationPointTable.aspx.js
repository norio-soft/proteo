var allocations = new Array();

function addAllocation() {
    if (Page_ClientValidate("vgAllocation")) {
        correctPointText();

        var cboConsortiumMember = $find(pageIDs.cboConsortiumMember);
        var consortiumMemberIdentityID = parseInt(cboConsortiumMember.get_value(), 10);

        var cboPoint = $find(pageIDs.cboPoint);
        var pointID = getPointID(cboPoint.get_selectedItem().get_value());

        if (consortiumMemberIdentityID > 0 && pointID > 0) {
            // Remove any existing allocation to this point from the allocations array and the grid
            removePoint(pointID);

            // Add the new allocation to the array and the grid
            allocations.push({ "ConsortiumMemberIdentityID": consortiumMemberIdentityID, "PointID": pointID });
            $("#trAllocationAdd").before(String.format(
                "<tr id=\"tr{0}\"><td>{1}</td><td>{2}</td><td><a href=\"javascript:removePoint({0});\">remove</a></td></tr>",
                pointID,
                cboConsortiumMember.get_text(),
                cboPoint.get_text()));

            cboPoint.clearSelection();
            var cboPointInput = cboPoint.get_inputDomElement();
            cboPointInput.focus();
            cboPointInput.select();

            // Clear validation errors
            $(Page_Validators).each(function() {
                ValidatorUpdateDisplay(this);
            });
        }
    }
}

function removePoint(pointID) {
    allocations = $.grep(allocations, function(i) { return i.PointID != pointID; });
    $("#tr" + pointID).remove();
}

function correctPointText() {
    window.setTimeout(function() {
        var cboPoint = $find(pageIDs.cboPoint);
        var itemText = cboPoint.get_text();
        var itemVal = cboPoint.get_value();
        
        if (itemText) {
            var tds = $("<div>" + itemText + "</div>").find("td");

            if (tds.length > 0) {
                cboPoint.set_text(tds.html());
                cboPoint.set_value(itemVal);
            }
        }
    }, 0);
}

function cboConsortiumMember_KeyPressing(sender, eventArgs) {
    var keyCode = eventArgs.get_domEvent().keyCode;
    if (!sender.get_dropDownVisible() && keyCode == 13) {
        addAllocation();
    }
}

function cboPoint_DropDownClosed(sender, eventArgs) {
    correctPointText();
}

function cboPoint_KeyPressing(sender, eventArgs) {
    var keyCode = eventArgs.get_domEvent().keyCode;
    if (keyCode == 38 || keyCode == 40) { // Up or Down
        correctPointText();
    }
    else if (!sender.get_dropDownVisible() && keyCode == 13) {
        addAllocation();
    }
}

function cboPoint_Blur(sender, eventArgs) {
    correctPointText();
}

function cvConsortiumMember_Validate(sender, eventArgs) {
    var cboConsortiumMember = $find(pageIDs.cboConsortiumMember);
    var consortiumMemberIdentityID = parseInt(cboConsortiumMember.get_value(), 10);
    eventArgs.IsValid = consortiumMemberIdentityID > 0;
}

function cvPoint_Validate(sender, eventArgs) {
    var cboPoint = $find(pageIDs.cboPoint);
    var pointID = getPointID(cboPoint.get_selectedItem().get_value());
    eventArgs.IsValid = pointID > 0;
}

function getPointID(pointComboValue) {
    var ids = pointComboValue.split(",");
    return ids.length > 1 ? parseInt(ids[1], 10) : null;
}

function writeAllocationsForServer() {
    $("#" + pageIDs.hidAllocations).val(Sys.Serialization.JavaScriptSerializer.serialize(allocations));
}