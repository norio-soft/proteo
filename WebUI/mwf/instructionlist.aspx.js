$(function () {
    $('#FilterOptions').accordion({ collapsible: true, heightStyle: 'content', active: false }).show();
    $('#CommunicateButton').click(function () { communicate(); });
});

function getCurrentInstructionIDs() {
    var instructionsGrid = findComponentBySelector('[id$=InstructionsGrid]');

    var instructionIDs = $.map(instructionsGrid.get_masterTableView().get_dataItems(), function (item) {
        return item.getDataKeyValue('ID');
    });

    return instructionIDs;
}

function showSignatureTooltip(element, imageUrl, name, comment, datestr, latitude, longitude) {
    var tooltip = $find(tooltipId);
    tooltip.set_targetControl(element);
    var content = "<div style='margin: 10px;'><h1>Signed by: " + name + "<h1>";
    if (imageUrl != "") {
        content += "<img src=\"" + imageUrl + "\">";
    } else {
        content += "<h1 style='margin: 20px;'>No signature collected</h1>";
    }
    if (comment != "") {
        content += "<h3>Comments:</h3><h3>" + comment + "</h3>";
    }

    if (location != null && location != "")
    {
        var url = "showLocation('/gps/showLocation.aspx?lat=" + latitude + "&lon=" + longitude + "')";
        content += "<a href=\"javascript:" + url + "\">Show Where Signed</a><br/>";
        content += "<b>Date:</b>" + datestr;
        content += "</div>";
    }

    tooltip.set_content(content);
    tooltip.show();
}

function showLocation(url) {
    newwindow = window.open(url, 'name', 'height=550,width=600,toolbar=no,menu=no,scrollbars=no');
    if (window.focus) { newwindow.focus() }
}

function communicate() {
    var instructionIDs = getCurrentInstructionIDs();

    if (instructionIDs.length == 0) {
        alert('There are no instructions to communicate');
        return;
    }

    showLoading('Communicating instructions');

    $.ajax({
        type: "POST",
        url: "/services/job.svc/CommunicateInstructions",
        data: JSON.stringify({ instructionIDs: instructionIDs, resendUnchangedInstructions: false }),
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    })
    .done(function () {
        hideLoading();
        document.aspnetForm.submit();
    })
    .fail(function (error) {
        showLoadingError(error);
    });
}
