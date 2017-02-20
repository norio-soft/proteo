function viewOrderProfile(orderID) {
    var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;
    var wnd = window.open(url, "Order", "width=1080, height=900, resizabled=1, scrollbars=1");
}

function viewJobDetails(jobID) {

    var url = '../job/job.aspx?jobId=' + jobID + getCSID();

    openResizableDialogWithScrollbars(url,'900','600');
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
    if (src.endsWith('topItem_exp.gif')) {
        for (var i = startIndex; i < stopIndex; i++) {
            Sys.UI.DomElement.removeCssClass(table.rows[i], 'hidden');
        }

        src = src.replace('topItem_exp.gif', 'topItem_col.gif');
    }
    else {
        for (var i = startIndex; i < stopIndex; i++) {
            Sys.UI.DomElement.addCssClass(table.rows[i], 'hidden');
        }

        src = src.replace('topItem_col.gif', 'topItem_exp.gif');
    }

    //  update the src with the new value
    img.src = src;
}