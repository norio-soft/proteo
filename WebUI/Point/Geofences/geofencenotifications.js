
var userLight =
{
    IdentityId: null, 
    UserName :  null ,
    Recipient : null, 
    TypeId : null, 
    NotificationId : null
}

var notificationUsers = new Array();

$(document).ready(function () {
    pointID = $('#ctl00_ContentPlaceHolder1_lblPointID').text();
    $("#tabs").tabs({ selected: 0 });

    $('#lnkActiveTimes').click(ShowWAP);

});
function SetRow(setOn, row) {
    for (var i = 0; i < 24; i++) {
        SetCell($get(_wapeditprefix + row + "_" + i + ".0"), setOn);
        SetCell($get(_wapeditprefix + row + "_" + i + ".5"), setOn)
    }
    return false;
}
function SetRowAll(setOn) {
    for (var i = 0; i < 7; i++) {
        SetRow(setOn, i);
    }
    return false;
}

function SetCell(sender, active) {
    if (active) {
        sender.className = 'active1';
    } else {
        sender.className = 'inactive';
    }
}

function ToggleCell(sender) {
    if (sender.className == 'inactive') {
        sender.className = 'active1';
    } else {
        sender.className = 'inactive';
    }
}

var isdragging = false;
function MouseDown(sender, eventArgs) {
    dragRangeStart = this;
    isdragging = true;
}

function MouseUp(sender, eventArgs) {
    if (dragRangeStart == null) return;
    dragRangeEnd = this;
    isdragging = false;
    //UpdateUI();
    $get('tempfocus').focus();
}

var lastCellId = "";
function MouseMove(sender, eventArgs) {
    //if (dragRangeStart == null) return;
    if (isdragging) {

        var cell = sender.target;
        if (cell.id != lastCellId) {
            if (cell.className == "inactive" || cell.className == "")
                cell.className = "active1";
            else
                cell.className = "inactive";

            lastCellId = cell.id;
        }
        dragRangeEnd = this;
        UpdateUI();
    }
}

var dragRangeStart, dragRangeEnd;
var isdragging = false;

function MouseClick(sender, eventArgs) {
    UpdateUI();
}

function UpdateUI() {
    if (isdragging) return;
    var startIndex = _wapeditprefix.length + 1 + 1;
    var start = new Number(dragRangeStart.id.substring(startIndex));
    var end = new Number(dragRangeEnd.id.substring(startIndex));
    var preamble = dragRangeStart.id.substring(0, startIndex);

    var initialStatusOff = dragRangeStart.className == 'inactive';

    var i = 0;
    if (end > start) {
        i = end;
        while (i >= start) {
            var j = new String(i);
            if (j.indexOf('.') < 0) j = j + ".0";
            var cell = $get(preamble + j);
            if (isdragging) {
                if (cell.className == "inactive" || cell.className == "")
                    cell.className = "active1";
                else
                    cell.className = "inactive";
            }
            else {
                SetCell(cell, initialStatusOff);
            }
            i -= 0.5;
        }
    } else {
        i = start;
        while (i <= end) {
            var j = new String(i);
            if (j.indexOf('.') < 0) j = j + ".0";
            var cell = $get(preamble + j);
            SetCell(cell, initialStatusOff);
            i += 0.5;
        }
    }

}


function Init() {
    for (var i = 2; i < tbl.rows.length; i++) {
        var cols = tbl.rows[i].cells.length;
        for (var j = 0; j < cols; j++) {
            var ele = tbl.rows[i].cells[j];
            if (ele.id.startsWith(_wapeditprefix)) {
                $addHandler(ele, 'mousedown', MouseDown);
                $addHandler(ele, 'mouseup', MouseUp);
                $addHandler(ele, 'mousemove', MouseMove);
                $addHandler(ele, 'click', MouseClick);
            }
        }
    }
}

function SaveWAP() {
    var retVal = false;

    var currentBinary = "";
    var currentBinary2 = "";
    hidden.value = '';
    // we need to read this in backwards for this to work
    for (var i = 2; i < tbl.rows.length; i++) {
        var cols = tbl.rows[i].cells.length;
        currentBinary = "";
        currentBinary2 = "";
        for (var j = cols-1; j >= 0; j--) {
            var ele = tbl.rows[i].cells[j];
            // we just need to convert this to binary and store the value, simples
            if (ele.id.startsWith(_wapeditprefix) && ele.className == 'active1') {
                if (j % 2 == 0)
                    currentBinary += "1";
                else
                    currentBinary2 += "1";
                //hidden.value += ele.id;
                //hidden.value += "|";
            }
            else if (ele.id.startsWith(_wapeditprefix) && ele.className == 'inactive') {
                if (j % 2 == 0)
                    currentBinary += "0";
                else
                    currentBinary2 += "0";
            }
        }
        if (currentBinary.length > 0 && currentBinary2.length > 0) {
            hidden.value += parseInt(currentBinary, 2) + "." + parseInt(currentBinary2, 2);
            hidden.value += "|";
        }
    }

    if (hidden.value.length == 0) {
        radalert('Please set when the notification is active, click the Set the active Times link.', 330, 100, 'Set Active Times', fnAlertCallback);
        retVal = false;
    }
    else
        retVal = true;

    return retVal;
}

function fnAlertCallback(arg) {
    return false;
}

function ShowWAP() {
    $find('ctl00_ContentPlaceHolder1_dlgWeekActivePeriod').show();
    return false;
}

function CloseWAP() {
    $find('ctl00_ContentPlaceHolder1_dlgWeekActivePeriod').close();
    return false;
}