// Scripts for job/Job.aspx
// Moved here to reduce Job.aspx bytes footprint

try {
    moveTo(0, 0);
} catch (err) { }
window.focus();

function ShowPlannerRequests(jobId)
{
    window.open('../Traffic/ListRequestsForJob.aspx?wiz=true&jobId=' + jobId + getCSID(), 'plannerRequests', 'width=850,height=400,resizable=no,scrollbars=yes');
}

//Window Code
function _showModalDialog(url, width, height, windowTitle) 
{
    return _showModalDialog(url, width, height, windowTitle, false);   
}

function openJobDetailsWindow(jobId)
{
	return;
}

//function openTrafficAreaWindow(url)
//{
//    _showModalDialog(url, 500, 221, "Change Traffic Area");
//}

//function AddExtra()
//{
//    var url = "AddExtra.aspx?jobId=" + jobId;
//    _showModalDialog(url, 400,320, 'Add Extra');
//}

function openChangeJobClient()
{
    _showModalDialog('ChangeJobClient.aspx?jobId=' + jobId,'550','458','Change Client');
}

function changeNominalCode(jobID)
{
    var url = "changeNominalCode.aspx";
    url += "?jID=" + jobId;
        
    var wnd = window.radopen("about:blank", "mediumWindow");                                  
    wnd.SetUrl(url);
    wnd.SetTitle("Manage Nominal Code");
}

function changeBusinessType(jobID)
{
    var url = "ChangeBusinessType.aspx";
    url += "?jID=" + jobId;
    
    var wnd = window.radopen("about:blank", "mediumWindow");                                  
    wnd.SetUrl(url);
    wnd.SetTitle("Manage Business Type");
}

//function UpdateExtra(extraId)
//{
//    var url = "AddExtra.aspx?jobId=" + jobId + "&extraId=" + extraId;
//    _showModalDialog(url, 400,320, 'Update Extra');
//}

function UpdateSubcontractInformation()
{
    var url = "../traffic/subcontractlegRates.aspx?JID=" + jobId;
    
    var wnd = window.radopen("about:blank", "largeWindow");                                  
    wnd.SetUrl(url);
    
    wnd.SetTitle("Sub-Contractor Rates");
}

function openUpdateLocation(instructionId)
{
    var url = "updateResourceLocations.aspx?instructionid=" + instructionId;
    _showModalDialog(url, 400, 320, 'Update Resource Locations');
}

function openGiveResourcesWindow(InstructionID)
{
    var url = "../traffic/giveResources.aspx?instructionId=" + InstructionID + "&dr=null&vr=null&tr=null&ca=null&ta=null";
    _showModalDialog(url, 400, 320, 'Give Resources');
}
function OpenJobDetails(jobId)
{
    openDialogWithScrollbars('../job/job.aspx?wiz=true&jobId=' + jobId + getCSID(), '600', '400');
}

function GetLegAction(legStateId, jobId, lastUpdateDate)
{
    if (legStateId == 1)
        return "<a href='javascript:openSubContractWindow(" + jobId + ", " + lastUpdateDate +"'>Sub-Contract</a>";
}

function showDocketWindow(instructionId)
{
    var url = "updatedockets.aspx?instructionId=" + instructionId ;
    _showModalDialog(url, 560,320, 'Update Dockets');
}

function _ShowLoadOrder(instructionID)
{
    var url = "../groupage/loadorder.aspx?instructionID=" + instructionID;
    openDialogWithScrollbars(url, 720, 420);
}

function showRoute(jobId)
{
    var url = "/ng/run/" + jobId + '/route';
    window.open(url, "Legs");
}

function showTransShippingSheet(jobId)
{
    var url = "../Groupage/TransshippingSheet.aspx?JobID=" + jobId;
    _showModalDialog(url, 1000, 700, 'Trans-Shipping Sheet', true);
}

function viewOrder(orderID) {
    var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

    var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
}

function ShowLoadOrder(instructionID)
{
    var url = "../Groupage/LoadOrder.aspx";
    url += "?instructionID=" + instructionID;

    var wnd = window.radopen("about:blank", "largeWindow");                                  
    wnd.SetUrl(url);
    wnd.SetTitle("Load Order"); 
}

function showTrafficAreaWindow(legId, LastUpdateDate, jobId)
{
    var url = "../Traffic/setTrafficArea.aspx?";
    url += "LegId=" + legId;
    url += "&LastUpdateDate=" + LastUpdateDate;
    url += "&JobId=" + jobId;
    
    var wnd = window.radopen("about:blank", "mediumWindow");                                  
    wnd.SetUrl(url);
    wnd.SetTitle("Traffic Area");
}

function openDisplayDeliveryListWindow(jobId)
{
    var orderID = 0;
    var url = "../Groupage/DeliveryNote.aspx?oID=" + orderID + "&JobID=" + jobId;

    var wnd = window.radopen("about:blank", "mediumWindow");                                  
    wnd.SetUrl(url);
    wnd.SetTitle("Delivery List");
}

function showHideDiv(divIdToShowHide, imageIdToChange, imgPath1, imgPath2)
{
    // Show or hide content
    var div = document.getElementById(divIdToShowHide);
    
    if (div != null)
    {
        if (div.style.display == 'inline')
        {
            div.style.display = 'none';
            setCookie(("Hide" + divIdToShowHide),"true",365);
        }
        else
        {
            div.style.display = 'inline';
            setCookie(("Hide" + divIdToShowHide), "false", 365);
        }
    }
    
    // Swap the image displayed
    var img = document.getElementById(imageIdToChange);
    
    if (img != null)
    {
        if (img.src.indexOf(imgPath1) > -1)
        {
            img.src = webserver + "/" + imgPath2;
        }
        else
        {
            img.src = webserver + "/" + imgPath1;
        }
    }
}

function ActionSelector(actionSelector)
{
    var actionSelector = document.getElementById(actionSelector);
    
    if (actionSelector != null)
    {
        var codeToExecute = actionSelector.value;
    }
    
    try{
        eval(codeToExecute);
    }
    catch(e){ // swallow error to prevent error appearing in users face.
    }
}

var postBackInProgress = false;

function removeOrder(orderID)
{
    if (!postBackInProgress)
    {
        var answer = confirm("Are you sure you want to remove Order " + orderID + " ?");
        if (answer)
        {
            // Post back to the server to delete the order.
            postBackInProgress = true;
            __doPostBack('RemoveOrder',orderID);
        }
    }
}

function moveUp(instructionId, prevInstructionId)
{
    __doPostBack('MoveUp',instructionId + ',' + prevInstructionId);
}

function moveDown(instructionId, followingInstructionId)
{
    __doPostBack('MoveDown',instructionId + ',' + followingInstructionId);
}

function merge(instructionId, followingInstructionId)
{
    __doPostBack('Merge',instructionId + ',' + followingInstructionId);
}
