<%@ Page Language="c#" Inherits="Orchestrator.WebUI.JobDisplay" MasterPageFile="~/WizardMasterPage.master"Title="Run Details" EnableViewState="true" MaintainScrollPositionOnPostback="true" CodeBehind="job.aspx.cs"  %>

<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>

<%@ Register TagPrefix="uc1" TagName="jobstateindicator" Src="~/UserControls/jobStateIndicator.ascx" %>
<%@ Register TagPrefix="uc1" TagName="redeliveryDisplay" Src="~/UserControls/InstructionAttempts.ascx" %>
<%@ Register TagPrefix="uc" TagName="MwfDriverMessaging" Src="~/UserControls/mwf/DriverMessaging.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<%@ Reference Control="~/Job/Wizard/UserControls/DetailsOrderHandling.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <base target="_self" />

    <style type="text/css">
   
        /* Only these styles are used on the Grid */.LegstatePlanned
        {
            height: 20px;
            background-color: #CCFFCC;
            cursor: default;
            border: 0px none !important;
        }
        .LegstateInProgress
        {
            height: 20px;
            background-color: #99FF99;
            cursor: default;
            border: 0px none !important;
        }
        .LegstateCompleted
        {
            height: 20px;
            background-color: #ADD8E6;
            cursor: default;
            border: 0px none !important;
        }
        table.vr
        {
            border-collapse: collapse;
            border-left: 2px solid lightblue;
            border-right: 2px solid lightblue;
        }
    </style>
    
    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/JobDetails.js" type="text/javascript"></script>
    <script language="javascript" src="/script/tooltippopups.js" type="text/javascript"></script>
    
    <script language="javascript" type="text/javascript">
    
        /* New Grid Context Menu Script */
        var _jobID              = -1;
        var _instructionID      = -1;

        var _driver             = "";
        var _driverResourceId   = -1;
        var _regNo              = "";
        var _vehicleResourceId  = -1;
        var _trailerRef         = "";
        var _trailerResourceId  = -1;
        var _legPlannedStart    = "";
        var _legPlannedEnd      = "";
        var _depotCode          = "";
        var _lastUpdateDate     = "";
        var _instructionStateId = -1;
        var _legResourced       = 'false';
        var _legSubbedOut       = 'false';
        var _jobTypeID          = 4;
        var _startInstructionID = -1;
        var _startInstructionStateID = -1;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var _subContractorId  = -1;
        var _pageLoaded = false;
        
        $(window).resize(function() {
            if(_pageLoaded == true)
            {
                var docwidth = $(window).width();
                var docheight = $(window).height();
                
                setCookie("JobDetailWidth",docwidth,365);
                setCookie("JobDetailHeight",docheight,365);
            }
        });
        
        window.setTimeout("sizeFromCookie()", 300);

        function RowContextMenu(sender, eventArgs)
        {
            RowSelected(sender, eventArgs);
                
            var menu = $find("<%=RadMenu1.ClientID %>");
            var evt = eventArgs.get_domEvent();

            var index = eventArgs.get_itemIndexHierarchical();
            document.getElementById("radGridClickedRowIndex").value = index;
            sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);
            menu.show(evt);
            evt.cancelBubble = true;
            evt.returnValue = false;
            
            if (evt.stopPropagation)
            {
                evt.stopPropagation();
                evt.preventDefault();
            }
            
            if (_startInstructionID > 0)
            {
                 var item = menu.findItemByValue("callincollection");
                if(item != null)
                {
                if (_startInstructionStateID == 3)
                    item.enable();
                else
                    item.disable();
                    }
            }
            
            if (_instructionID > 0)
            {
                var item = menu._findItemByValue("callinthis");
                 
                if(item != null)
                {
                    if (_instructionStateId == 3)
                        item.enable();
                    else
                        item.disable();
                }
            }   
        }

        function RowSelected(sender, eventArgs)
        {
            var masterTable = sender.get_masterTableView();
             
            var row = masterTable.get_dataItems()[eventArgs.get_itemIndexHierarchical()];
            var cell = masterTable.getCellByColumnUniqueName(row, "colHiddenKeys");
            var keysArray = $(cell).contents("span[id$=lblHiddenKeys]").text().split("|");
            
            _jobID = keysArray[0];
            _instructionID = keysArray[1];
            _driver = keysArray[2];
            _driverResourceId = keysArray[3];
            _subContractorId = keysArray[4];
            _regNo = keysArray[5];
            _vehicleResourceId = keysArray[6];
            _trailerRef = keysArray[7];
            _trailerResourceId = keysArray[8];
            _legPlannedStart = keysArray[9];
            _legPlannedEnd = keysArray[10];
            _depotCode = keysArray[11];
            _lastUpdateDate = keysArray[12];
            _instructionStateId = keysArray[13];
            _jobTypeID = keysArray[14];
            _startInstructionID = keysArray[15];
            _startInstructionStateID = keysArray[16];
            _allowMwfMessaging = keysArray[17];
        }
         
        function radMenu1_itemClicked(sender, eventArgs)
        {
           var mnuCall = eventArgs.get_item().get_value();
           eventArgs.get_item().get_menu().hide();
            
           switch(mnuCall.toLowerCase())
            {
                case "subcontractleg":
                    openSubContractWindow(_jobID, _lastUpdateDate);    
                    break;
                case "unsubcontractleg":
                    UnSubContractLegPostBack(_instructionID, _instructionStateId, _subContractorId);
                    break;
                case "trunk":
                    openTrunkWindow(_instructionID, _driver, _regNo, _lastUpdateDate); 
                    break;
                case "multitrunk":
                    openMultiTrunkWindow(_jobID, _instructionID, _lastUpdateDate); 
                    break;
                case "removetrunk":
                     if (_instructionStateId != 4)
                         openRemoveTrunkWindow(_jobID ,_instructionID, _lastUpdateDate); 
                     else
                        alert("This leg has already been completed");
                     break;
                case "resourcethis":
                        openResourceWindow(_instructionID, _driver, _driverResourceId, _regNo, _vehicleResourceId, _trailerRef, _trailerResourceId, _legPlannedStart, _legPlannedEnd, _depotCode, _lastUpdateDate, _jobID);
                     break;
                case "changebookedtimes":
                     openAlterBookedTimesWindow(_jobID, _lastUpdateDate);
                     break; 
                case "changeplannedtimes":
                        openAlterPlannedTimesWindow(_jobID, _lastUpdateDate);
                     break;
                case "communicate":
                    if (_instructionStateId == 2)
                        openCommunicateWindow(_instructionID, _driver, _driverResourceId, _subContractorId, _jobID, _lastUpdateDate);
                    else
                        alert("You can only communicate Planned legs");
                    break;
                case "quickcommunicate":
                    if (_instructionStateId == 2)
                        CommunicateThis(_instructionID, _driver, _driverResourceId, _vehicleResourceId, _subContractorId, _jobID);
                    else
                        alert("You can only communicate Planned legs");
                    break;
                case "uncommunicate":
                    if (_instructionStateId == 3)
                        UnCommunicate(_instructionID, _jobID, _lastUpdateDate);
                    else
                        alert("You can only UnCommunicate legs in Progress.");
                    break;
               case "sendmwfmessage":
                   sendMwfMessage(_jobID, _driverResourceId);
                   break;
               case "jobdetails":
                    location.href = location.href;
                    break;
                case "callin":
                    location.href = "../Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + _jobID+ getCSID();
                    break;
                case "giveresourceto":
                    if (_instructionStateId != 4)
                        openGiveResourcesWindow(_instructionID);
                    else
                        alert("This leg has already been completed");
                    break;
                case "showloadorder":
                    openResizableDialogWithScrollbars('../Traffic/LoadOrder.aspx?jobid=' + _jobID, '700', '258');
                    break; 
                case "adddestination":
                    openAddDestination(_jobID, _lastUpdateDate);
                    break;
                case "addmultipledestinations":
                    openAddMultipleDestinations(_jobID, _lastUpdateDate);
                    break;
                case "removedestination":
                    if (_instructionStateId != 4)
                        openRemoveDestination(_jobID, _lastUpdateDate);
                    else
                        alert("This leg has already been completed"); 
                    break;
                case "deliverynote":
                    openDisplayDeliveryListWindow(_jobID);
                    break;
                case "callinthis":
                    location.href = "../Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + _jobID + "&instructionid=" + _instructionID + getCSID();
                    break;
                case "removecallin":
                    RemoveCallIn(_jobID, _instructionID);
                    break;
                case "callincollection":
                    location.href = "../Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + _jobID + "&instructionid=" + _startInstructionID + getCSID();
                    break;
                case "changetrailertype":
                    OpenPlanningCategoryWindow(_jobID);
                    break;
               case "changedepot":
                   openChangeDepot(_jobID, _instructionID);
                   break;
            }
        }

        function radMenu1_showing(sender, eventArgs) {
            var enableMwfMessaging = Boolean.parse(_allowMwfMessaging);
            sender.findItemByValue("sendmwfmessage").set_enabled(enableMwfMessaging);
        }

        function RemoveCallIn(jobID, instructionID)
        {
            PageMethods.RemoveCallIn(jobID, instructionID,success_CallBack, error_CallBack )
        }

        function RemoveCallIn(jobID, instructionID, goodsRefusalCount)
        {
            if(goodsRefusalCount > 0)
            {
                var answer = confirm("There are Goods Refusals associated with the order(s) on this leg, Would you like these to be removed with the call-in?\nPress OK to Delete the Goods Refusals or Press Cancel to keep the Refusals but Remove the Call In");
                if(answer)
                {
                    PageMethods.RemoveCallInWithGoodsRefusals(jobID, instructionID,success_CallBack, error_CallBack )
                }
                else
                {
                    PageMethods.RemoveCallIn(jobID, instructionID,success_CallBack, error_CallBack )
                }
            }
            else
            {
                PageMethods.RemoveCallIn(jobID, instructionID,success_CallBack, error_CallBack )
            }
        }

        function ReDeliver(jobID, instructionID)
        {
            location.href = "/traffic/jobmanagement/Orderbasedredelivery.aspx?wiz=true&jobID=" + jobID + "&instructionID=" + instructionID + "&csid=" + this.CookieSessionID;
        }

        function AlreadyInvoiced(jobID, instructionID)
        {
            alert("This leg (Instruction ID: " + instructionID + ") contains order(s) which are being invoiced or are on Pre-Invoices, please review");
        }

        function success_CallBack(result, context, method)
        {
            if (result)
                location.href = location.href;
        }
        
        function error_CallBack(result, context, method)
        {
        }
        
        // Filter data
        var activeControlAreaId = <%=ControlAreaId %>;
        var activeTrafficAreaIds = "<%=TrafficAreaIds %>";
        var startDate = "<%=StartDate %>";
        var endDate = "<%=EndDate %>";
        var todaysDate = "<%=DateTime.Today.ToString("dd/MM/yy")%>";
        var depotId = "<%=DepotId %>";
        var jobId = "<%=Request.QueryString["jobId"] %>";
        
        function showDemurrageWindow(jobId, instructionId, extraId)
        {
            var qs = "jobId=" + jobId + "&instructionId=" + instructionId + "&extraId=" + extraId;
            <%=dlgExtra.ClientID %>_Open(qs);
        }
        
        function pageLoad()
        {
            if(getCookie("HidedivJobDetails") != null)
            {
                if(getCookie("HidedivJobDetails") == "true")
                    showHideDiv('divJobDetails','imgExpandCollapseJobDetails', 'images/topItem_col.gif','images/topItem_exp.gif');
            }
            
            if(getCookie("HidedivOrderHandling") != null)
            {
                if(getCookie("HidedivOrderHandling") == "true")
                    showHideDiv('divOrderHandling','imgExpandCollapseOrderHandling', 'images/topItem_col.gif','images/topItem_exp.gif');
            }
            
            if(getCookie("HidedivLegs") != null)
            {
                if(getCookie("HidedivLegs") == "true")
                    showHideDiv('divLegs','imgExpandCollapseLegs', 'images/topItem_col.gif','images/topItem_exp.gif');
            }

            sizeFromCookie();
        }
        
        function RefreshPage()
        {
            __doPostBack('', 'Refresh');
        }
        
        function ShowDisplayForSubbyOrNotAssigned()
        {
            return <%= m_subbyName.Length > 0 ? "\"" + m_subbyName + "\"" : "\"Not Assigned\"" %>;
        }
        
        function openPalletHandlingWindow(jobId)
        {
            var qs = "jobId=" + jobId;
            <%=dlgPalletHandling.ClientID %>_Open(qs);
        }
        
        function OpenPlanningCategoryWindow(jobId)
        {
            var qs = "jid=" + jobId;
            <%=dlgTrailerType.ClientID %>_Open(qs);
        }
        
        function OpenUpdateOrderDelivery(InstructionId) {
            var qs = "InstructionId=" + InstructionId;

            <%=dlgUpdateOrderDelivery.ClientID %>_Open(qs);
        }
        
        function sizeFromCookie() 
        {
            if(_pageLoaded == true)
                return;
                
            var currentdocwidth = $(window).width();
            var currentdocheight = $(window).height();
            
            if(getCookie("JobDetailWidth") != null)
            {
                docwidth = getCookie("JobDetailWidth");
                docheight = getCookie("JobDetailHeight");
                resizeBy((docwidth - currentdocwidth),(docheight - currentdocheight));
            }
            
            _pageLoaded = true;
        }

        $(document).ready(function(){
            window.focus();
            sizeFromCookie();
        });
        
        function UnCommunicate(instructionId, jobId, lastUpdateDate)
        {
            PageMethods.UnCommunicate(jobId, instructionId, success_CallBack, UnCommunicate_Failure);
        }
        
        function UnCommunicate_Failure(error)
        {
            alert(error.get_message());
        }
        
        function CommunicateThis(instructionID, driver, driverResourceId, vehicleResourceId, subContractorId, jobID)
        {
            PageMethods.Communicate(instructionID, driver, driverResourceId, vehicleResourceId || -1, subContractorId, jobID, userName, success_CallBack, Communicate_Failure);
        }
        
        function Communicate_Failure(error)
        {
            alert(error.get_message());
        }
        
        function displayRunPalletHandlingAudit()
        {
            var qs = "jID=" + <%=m_jobId %>;
            <%=dlgPHRunAudit.ClientID %>_Open(qs);
        }
        
        function openTrafficAreaWindow(startInstructionId, lastUpdateDate, jobId)
        {
            var qs = "InstructionId=" + startInstructionId + "&LastUpdateDate=" + lastUpdateDate + "&JobId=" + jobId;
            <%=dlgTrafficArea.ClientID %>_Open(qs);
        }

        function searchForCheckIn() 
        {
            var destination = "/POD/outstandingPods.aspx";
            var qs = "?jobID=" + jobId + "&startDate=" + "<%= plannedStartDateString %>" + "&endDate=" + "<%= plannedEndDateString %>";
            window.open(destination + qs);
            return false;
        }
        function openChangeDepot(jobId, instructionID)
        {
            var qs = 'wiz=true&jId=' + jobId + '&iid=' + instructionID;
            <%=dlgChangePlanningDepot.ClientID%>_Open(qs);
        }
        
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <cc1:Dialog ID="dlgPalletHandling" runat="server" URL="/Traffic/JobManagement/AddUpdatePalletHandling.aspx" Width="1115" Height="750" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    <cc1:Dialog ID="dlgExtra" runat="server" Width="400" Height="320" Mode="Modal" URL="/groupage/addupdateextra.aspx" AutoPostBack="true" ReturnValueExpected="true" ></cc1:Dialog>
    <cc1:Dialog ID="dlgFilter" URL="/Traffic/Filters/specifyfilter.aspx" Width="675" Height="650" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgRunHistory" URL="/job/jobHistory.aspx" Width="700" Height="400" AutoPostBack="false" runat="server" ReturnValueExpected="false" Mode="Modal" Resizable="false"></cc1:Dialog>
    <cc1:Dialog ID="dlgTrailerType" URL="/Traffic/changetrailertype.aspx" Width="400" Height="200" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgResourceThis" URL="/Traffic/resourcethis.aspx" Width="1000" Height="710" AutoPostBack="true" Mode="Normal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgTrunk" URL="/Traffic/trunk.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgTrafficArea" URL="/Traffic/settrafficarea.aspx" Width="500" Height="230" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgBookedTimes" URL="/Traffic/changebookedtimes.aspx" Width="800" Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgPlannedTimes" URL="/Traffic/changeplannedtimes.aspx" Width="700" Height="320" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgSubcontract" URL="/Traffic/subcontract.aspx" Width="650" Height="580" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgCommunicate" URL="/Traffic/communicatethis.aspx" Width="500" Height="600" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgRemoveTrunk" URL="/Traffic/removetrunk.aspx" Width="550" Height="358" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgMultiTrunk" URL="/Traffic/multitrunk.aspx" Width="720" Height="450" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgAddDestination" URL="/job/adddestination.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgAddMultiDestination" URL="/job/addmultidestination.aspx" Width="800" Height="600" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgRemoveDestination" URL="/job/removedestination.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgUpdateOrderDelivery" URL="/Groupage/UpdateOrderDelivery.aspx" Width="1200" Height="750" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgAddOrder" runat="server" ReturnValueExpected="true" AutoPostBack="true" URL="/job/addorder.aspx" Height="600" Width="800" Mode="Modal"></cc1:Dialog>
    <cc1:Dialog ID="dlgOrderShuffler" URL="/Groupage/Shuffler.aspx" Width="1230" Height="900" AutoPostBack="false" Mode="Normal" runat="server" ReturnValueExpected="false" Scrollbars="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgPHRunAudit" runat="server" AutoPostBack="false" ReturnValueExpected="false" URL="/Traffic/JobManagement/RunPalletHandlingAudit.aspx" Height="400" Width="1000"></cc1:Dialog>
    <cc1:Dialog ID="dlgUpdateDockets" runat="server" ReturnValueExpected="true" AutoPostBack="true" URL="/job/updatedockets.aspx" Height="600" Width="800" Mode="Modal"></cc1:Dialog>
    <cc1:Dialog ID="dlgChangePlanningDepot" URL="/traffic/changedepot.aspx" Width="550" Height="180" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>    
    <cc1:Dialog ID="dlgMergeRun" runat="server" ReturnValueExpected="true" AutoPostBack="true" URL="/job/mergerun.aspx" Height="600" Width="800" Mode="Modal"></cc1:Dialog>
    <telerik:RadWindowManager ID="rmwJob" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="false" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadCodeBlock runat="server">

    <telerik:RadContextMenu ID="RadMenu1" runat="server" OnClientItemClicked="radMenu1_itemClicked" OnClientShowing="radMenu1_showing" Skin="Black">
        <Items>
            <telerik:RadMenuItem Text="Resource This" Value="resourcethis" />
            <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />            
            <telerik:RadMenuItem Text="Sub-Contract Leg" Value="subcontractleg"  />
            <telerik:RadMenuItem Text="Un-Contract Leg" Value="unsubcontractleg" />
            <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
            <telerik:RadMenuItem Text="Communicate This" Value="communicate" />
            <telerik:RadMenuItem Text="Quick Communicate This" Value="quickcommunicate" Visible="false" />
            <telerik:RadMenuItem Text="Remove Communication" Value="uncommunicate" />
            <telerik:RadMenuItem Text="Send MWF Message" Value="sendmwfmessage" Enabled="false" />
            <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
            <telerik:RadMenuItem Text="Trunk" Value="trunk" />
            <telerik:RadMenuItem Text="Multi-Trunk" Value="multitrunk" />
            <telerik:RadMenuItem Text="Remove Trunk" Value="removetrunk" />
            <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
            <telerik:RadMenuItem Text="Remove Links" Value="removelinks" Visible="false" />
            <telerik:RadMenuItem Text="Change Booked Times" Value="changebookedtimes" />
            <telerik:RadMenuItem Text="Change Planned Times" Value="changeplannedtimes" />
            
            <telerik:RadMenuItem Text="Job Details" Value="JobDetails" Visible="false" />
            <telerik:RadMenuItem Text="Call In" Value="callin" />
            
            <telerik:RadMenuItem Text="Show Load Order" Value="showloadorder" Visible="false" />
            <telerik:RadMenuItem Text="Add Destination" Value="adddestination" />
            <telerik:RadMenuItem Text="Add Multiple Destinations" Value="addmultipledestinations" />
            <telerik:RadMenuItem Text="Remove Destination" Value="removedestination" />
            <telerik:RadMenuItem Text="Give Resource" Value="giveresourceto" Visible="false" />
            <telerik:RadMenuItem Text="Link Job" Value="linkjob" Visible="false" />
            <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />           
            <telerik:RadMenuItem Text="Change Planning Category" Value="changetrailertype" />
            <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />           
            <telerik:RadMenuItem Text="Change Depot" Value="changedepot" />
        </Items>
        <CollapseAnimation Type="none" />
    </telerik:RadContextMenu>

    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    
    <div id="divPointAddress" style="z-index: 5; display: none; background-color: Wheat; padding: 2px 2px 2px 2px;">
        <table style="background-color: white; border: solid 1pt black;" cellpadding="2">
            <tr>
                <td>
                    <span id="spnPointAddress"></span>
                </td>
            </tr>
        </table>
    </div>
    
    <div class="buttonbar" style="margin: 0px;">
        <table border="0" cellpadding="0" cellspacing="2" width="100%">
            <tr>
                <td>
                <input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=' + jobId+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=' + jobId+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=' + jobId+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=' + jobId+ getCSID();" />
                </td>
                <td>
                    <input type="button" id="btnPricing" runat="server" style="width: 75px;"
                        value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=' + jobId+ getCSID();" />
                </td>
                <td width="100%" align="right">
                    <iframe marginheight="0" marginwidth="0" frameborder="no" scrolling="no" width="300px"
                        height="22px" src="/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["jobId"] %>&amp;csid=<%=this.CookieSessionID %>">
                    </iframe>
                </td>
            </tr>
        </table>
    </div>
    
    </telerik:RadCodeBlock>
    
    <div style="height: 4px;"></div>
    
    <table cellpadding="4px" cellspacing="0" width="100%">
        <tr>
            <td style="padding-right: 0px; padding-left: 0px;">
                <table style="background-color: White; border: 1px solid #CCC;" cellpadding="3px" width="100%">
                    <tr>
                        <td class="subTabOuter">
                            <span id="spanEditJob" runat="server">
                                <a id="ancEditJob" runat="server">
                                    <span class="subTabInner">Edit Run</span>
                                </a>
                            </span>
                        </td>
                        <td class="subTabOuter">
                            <a href="javascript:window.location='/traffic/JobManagement.aspx?wiz=true&jobId=' + jobId + getCSID()" >
                                <span class="subTabInner">Manage</span>
                            </a>
                        </td>
                        <td class="subTabOuter" id="tdCancelJob" runat="server">
                            <asp:LinkButton ID="lnkCancelJob" runat="server" CssClass="subTabInner" Text="Cancel Job">Cancel Run</asp:LinkButton>
                            <ajaxToolkit:ConfirmButtonExtender ID="btnCancelJobConfirmation" runat="server" 
                                TargetControlID="lnkCancelJob"
                                ConfirmText="Are you sure you want to cancel this job?"
                                DisplayModalPopupID="mpeCancelJobConfirmation" />
                            <ajaxToolkit:ModalPopupExtender ID="mpeCancelJobConfirmation" runat="server" TargetControlID="lnkCancelJob" PopupControlID="pnlCancelJobConfirmation" OkControlID="ButtonOk" CancelControlID="ButtonCancel" BackgroundCssClass="modalBackground" />
                        </td>
                        <td class="subTabOuter" id="tdCancelJobAndOrders" runat="server">
                            <asp:LinkButton ID="lnkCancelJobAndOrders" runat="server" CssClass="subTabInner" Text="Cancel Run And Orders">Cancel Run And Orders</asp:LinkButton>
                            <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" 
                                TargetControlID="lnkCancelJobAndOrders"
                                ConfirmText="Are you sure you want to cancel this job?"
                                DisplayModalPopupID="mpeCancelJobAmdOrdersConfirmation" />
                                <ajaxToolkit:ModalPopupExtender ID="mpeCancelJobAmdOrdersConfirmation" runat="server" TargetControlID="lnkCancelJobAndOrders" PopupControlID="pnlCancelJobConfirmation" OkControlID="ButtonOk" CancelControlID="ButtonCancel" BackgroundCssClass="modalBackground" />
                        </td>
                        <td class="subTabOuter">
                            <a href="javascript:window.location='/problemAlert/AddUpdateProblemAlert.aspx?wiz=true&jobId=' + jobId" >
                                <span class="subTabInner">Problem Alert</span>
                            </a>
                        </td>
                        <td class="subTabOuter">
                            <a href="" runat="server" id="hypRunHistory" >
                                <span class="subTabInner">Run History</span>
                            </a>
                        </td>
                        <td class="subTabOuter">
                            <radcodeblock runat="server">
                            <a href="javascript:showRoute(<%=m_jobId %>);">
                                <span class="subTabInner">Run Route</span>
                            </a>
                            </radcodeblock>
                        </td>
                        <td class="subTabOuter">
                            <asp:LinkButton ID="lnkCopyRun" CssClass="subTabInner" runat="server" Text="Copy Run"></asp:LinkButton>
                        </td>
                        <td class="subTabOuter">
                            <asp:LinkButton ID="lnkMergeRun" CssClass="subTabInner" runat="server" Text="Merge Run"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="padding-right: 0px; padding-left: 0px;">
                <table style="background-color: White;  border: 1px solid #CCC;" cellpadding="5px" width="100%">
                    <tr>
                        <td>
                            <span style="vertical-align: top;">
                                <img align="middle" id="imgExpandCollapseJobDetails" alt="Expand-Collapse" src="../images/topItem_col.gif"
                                    onclick="javascript: showHideDiv('divJobDetails','imgExpandCollapseJobDetails', 'images/topItem_col.gif','images/topItem_exp.gif');" />&nbsp;<b>Run
                                        Details</b></span>
                            <div id="divJobDetails" style="display: inline;">
                                <table cellpadding="10px" width="100%">
                                    <tr valign="top">
                                        <td style="background-color: whitesmoke; border: 1px solid #CCC;
                                            background-position: bottom; background-repeat: repeat-x;" width="25%">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <b>Run</b>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td width="35%">
                                                        Run Id:
                                                    </td>
                                                    <td>
                                                        <span style="font-weight: bold; font-size: 12px">
                                                            <asp:Label ID="lblJobId" runat="server"></asp:Label></span>
                                                    </td>
                                                </tr>
                                                <tr style="display:none;">
                                                    <td>
                                                        Client
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblClient" runat="server"></asp:Label>
                                                        <asp:HyperLink ID="lnkClient" runat="server" NavigateUrl="javascript:openChangeJobClient();"></asp:HyperLink>
                                                    </td>
                                                </tr>
                                                <tr >
                                                    <td>
                                                        Run State
                                                    </td>
                                                    <td>
                                                        <uc1:jobstateindicator ID="ucJobStateIndicator" runat="server">
                                                        </uc1:jobstateindicator>
                                                        <br />
                                                        <asp:HyperLink ID="hlInvoice" runat="server" Target="_blank" Visible="False"></asp:HyperLink>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr style="display:none;">
                                                    <td>
                                                        Run Type
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblJobType" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                    </td>
                                                    <td valign="top" id="tdTransShippingSheet" runat="server">
                                                        <a href="javascript:showTransShippingSheet(jobId);">Show
                                                            Trans-Shipping Sheet</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Business Type
                                                    </td>
                                                    <td>
                                                        <asp:HyperLink ID="hlBusinessType" runat="server" NavigateUrl="javascript:changeBusinessType()"></asp:HyperLink>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        Nominal Code
                                                    </td>
                                                    <td>
                                                        <asp:HyperLink ID="hlNominalCode" runat="server" NavigateUrl="javascript:changeNominalCode()"></asp:HyperLink>
                                                    </td>
                                                </tr>
                                                <tr id="trRunPHAudit" runat="server">
                                                    <td>
                                                        Pallet Handling
                                                    </td>
                                                    <td>
                                                        <asp:HyperLink ID="hlPalletHandling" runat="server" NavigateUrl="javascript:displayRunPalletHandlingAudit()" Text="Show Run Audit Trail"></asp:HyperLink>
                                                    </td>
                                                </tr>
                                                <tr style="display: none;">
                                                    <td>
                                                        &#160;
                                                    </td>
                                                    <td>
                                                        <telerik:RadAjaxPanel ID="raxSingleInvoice" runat="server">
                                                            <asp:CheckBox ID="chkSingleInvoice" runat="server" Text="Must be invoiced seperately."
                                                                AutoPostBack="true" /></telerik:RadAjaxPanel>
                                                    </td>
                                                </tr>
                                                <!--
							                            <tr id="trCurrentTrafficArea" runat="server">
								                            <td nowrap="nowrap">Current Traffic Area</td>
								                            <td><asp:Label id="lblCurrentTrafficArea" runat="server"></asp:Label></td>
							                            </tr>
							                            -->
                                                <tr style="display:none;">
                                                    <td nowrap="nowrap">
                                                        Stock Movement Run&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblStockMovement" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr id="trShowLoadOrder" runat="server">
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <input type="button" class="buttonClass" value="Show Load Order" onclick="javascript:openResizableDialogWithScrollbars('~/Traffic/LoadOrder.aspx?jobid=<%=Request.QueryString["jobId"] %>', '700', '258')" />
                                                    </td>
                                                </tr>
                                                <tr id="trProducePILs" runat="server">
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnProducePILs" cssclass="buttonClass" runat="server" Text="Produce PILs" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                    <td>
                                                        <img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The run has at least one planner request attached to it, click this icon to review them."
                                                             style="vertical-align: -3px; cursor: pointer;"/>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td style="background-color: whitesmoke; border: 1px solid #CCC;
                                            background-position: bottom; background-repeat: repeat-x;" width="25%">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <b>Run Details</b>
                                                    </td>
                                                </tr>
                                                <tr id="trLoadNumber" runat="server">
                                                    <td width="50%">
                                                        <asp:Label ID="lblLoadNumberText" runat="server"></asp:Label>
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblLoadNumber" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <asp:Repeater ID="repJobReferences" runat="server" Visible="false">
                                                    <HeaderTemplate>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td valign="top" align="left">
                                                                <%# DataBinder.Eval(Container.DataItem, "OrganisationReference.Description") %>
                                                            </td>
                                                            <td valign="top" align="right">
                                                                <%# DataBinder.Eval(Container.DataItem, "Value") %>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <tr id="trReturnReferenceNumber" runat="server" visible="false">
                                                    <td>
                                                        Return Reference Number
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblReturnReferenceNumber" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr id="trNumberOfPallets" runat="server">
                                                    <td>
                                                        Number of Pallets
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblNumberOfPallets" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        Marked for Cancellation
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblMarkedForCancellation" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Revenue (excluding fuel surcharge)
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblTotalRevenue" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Sub-Contractor Cost
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblTotalSubcontractorCost" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        Margin
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblTotalMargin" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        % Margin
                                                    </td>
                                                    <td align="right">
                                                        <asp:Label ID="lblPercentageMargin" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <uc1:redeliveryDisplay ID="RedeliveryDisplay1" runat="server">
                                            </uc1:redeliveryDisplay>
                                        </td>
                                        <td style="background-color: whitesmoke; border: 1px solid #CCC;
                                            background-position: bottom; background-repeat: repeat-x;" width="25%">
                                            <b>Sub Contractor Details</b><br />
                                            <asp:Panel ID="pnlSubContractors" runat="server">
                                                <asp:Repeater ID="repSubContractors" runat="server">
                                                    <HeaderTemplate>
                                                        <table border="0">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td style=" width:60px;">
                                                                Subcontractor:&nbsp;&nbsp;
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblSubContractor" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Invoice No.
                                                            </td>
                                                            <td align="right">
                                                                <a runat="server" id="lnkViewInvoice" target="_blank"></a>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Rate
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblSubContractRate" runat="server" Text=""></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Reference
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblSubcontractReference" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Attended
                                                            </td>
                                                            <td align="right">
                                                                <asp:Label ID="lblIsAttended" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                                <br />
                                                <asp:Button ID="btnUnSubContract" runat="server" Text="Bring Back In House" />
                                                <asp:Label ID="lblSubContracted" runat="server" Text="This run has not been sub contracted."
                                                    Visible="False"></asp:Label>
                                                <br />
                                                <br />
                                                <asp:HyperLink ID="lnkUpdateSubcontractInformation" runat="server" Text="Change Subcontract Information"
                                                    NavigateUrl="javascript:UpdateSubcontractInformation();" Visible="false" />
                                                <br>
                                            </asp:Panel>
                                        </td>
                                        <td style="background-color: whitesmoke; border: 1px solid #CCC;
                                            background-position: bottom; background-repeat: repeat-x;" width="25%">
                                            
                                            <asp:Panel ID="pnlExtra" runat="server" Visible="True">
                                                <b>Run Extras</b><br />
                                                <div style="height: 140px; overflow-y: auto; overflow-x: hidden; width: 100%;">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <input type="button" onclick="javascript:<%=dlgExtra.ClientID %>_Open('jobId=' + jobId);" value="Add Extra" id="btnAddExtra"
                                                                    runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="padding-top: 5px">
                                                                <asp:Label ID="lblNoExtras" runat="server" Text="There are no Extras" Visible="false"></asp:Label>
                                                                <asp:DataGrid ID="dgExtras" runat="server" AllowPaging="False" BorderColor="#999999"
                                                                    BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="2" GridLines="Both"
                                                                    AutoGenerateColumns="False" PagerStyle-HorizontalAlign="Right" PagerStyle-Mode="NumericPages"
                                                                    PageSize="5" ShowFooter="false">
                                                                    <SelectedItemStyle BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SelectedItemStyle>
                                                                    <AlternatingItemStyle ForeColor="Black" BorderStyle="Dotted" BorderColor="Black">
                                                                    </AlternatingItemStyle>
                                                                    <ItemStyle ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ItemStyle>
                                                                    <HeaderStyle BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True" Width="410px"
                                                                        CssClass="PurpleBookHeader1" Height="15"></HeaderStyle>
                                                                    <Columns>
                                                                    <asp:TemplateColumn HeaderText="Type">
                                                                            <ItemTemplate>
                                                                                <a href="javascript:UpdateExtra(<%# DataBinder.Eval(Container.DataItem, "ExtraId") %>)">
                                                                                    <asp:Label ID="lblExtraType" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraType").ToString()) %>'></asp:Label></a>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateColumn>
                                                                        <asp:TemplateColumn Visible="False" HeaderText="Custom Description">
                                                                            <ItemTemplate>
                                                                                <asp:Label Visible="False" ID="lblCustomDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomDescription") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateColumn>
                                                                        <asp:TemplateColumn HeaderText="State">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblExtraState" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraState").ToString()) %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateColumn>
                                                                        <asp:TemplateColumn HeaderText="Contact">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblClientContact" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ClientContact") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateColumn>
                                                                        <asp:BoundColumn HeaderText="Amount" DataField="ExtraAmount" DataFormatString="{0:C}">
                                                                        </asp:BoundColumn>
                                                                    </Columns>
                                                                </asp:DataGrid>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </asp:Panel>
                                            
                                            <asp:Panel ID="pnlManifest" runat="server" Visible="false">
                                                <b>Manifests</b><br />
                                                <!-- used for the production of resource manifests from job details -->
                                                <asp:ListView ID="lvManifests" runat="server">
                                                
                                                    <LayoutTemplate>
                                                        <table>
                                                            <thead>
                                                                <tr>
                                                                    <th>Manifest Name</th><th>Who</th><th></th>
                                                                </tr>
                                                            </thead>
                                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                                            
                                                        </table>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <%#Eval("Description") %>
                                                            </td>
                                                            <td>
                                                                <%#Eval("ResourceName") %>
                                                            </td>
                                                            <td>
                                                                <asp:HyperLink ID="lnkViewManifest" runat="server" Text="View"></asp:HyperLink>&nbsp;
                                                                <a runat="server" id="lnkCreateManifest" class="createManifestLink" visible="false" style="cursor: pointer;">Create</a>
                                                                <asp:LinkButton ID="hidCreateManifestButton" runat="server" CausesValidation="false" style="display: none;" />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="padding-right: 0px; padding-left: 0px;">
                <table style="background-color: White; border: 1px solid #CCC;" cellpadding="5px" width="100%">
                    <tr valign="top" id="trCollectDrop" runat="server">
                        <td width="50%">
                            <fieldset>
                                <legend>Loads</legend>
                                <div style="height: 234px; overflow: auto;">
                                    <asp:Label ID="lblCollections" runat="server"></asp:Label>
                                    <br />
                                </div>
                            </fieldset>
                        </td>
                        <td width="50%">
                            <fieldset>
                                <legend>Drops</legend>
                                <div style="height: 234px; overflow: auto;">
                                    <asp:Label ID="lblDeliveries" runat="server"></asp:Label>
                                    <br />
                                </div>
                            </fieldset>
                        </td>
                    </tr>
                    <tr valign="top" id="trGroupageOrderHandling" runat="server">
                        <td>
                            <span style="vertical-align: top;">
                                <img align="middle" id="imgExpandCollapseOrderHandling" alt="Expand-Collapse" src="../images/topItem_col.gif"
                                    onclick="javascript: showHideDiv('divOrderHandling','imgExpandCollapseOrderHandling', 'images/topItem_col.gif','images/topItem_exp.gif');" />&nbsp;<b>Order
                                        Handling</b></span>
                            <div id="divOrderHandling" style="display: inline;">
                                <asp:PlaceHolder ID="ph_OrderHandling" runat="server"></asp:PlaceHolder>
                                <br />
                                <div class="buttonbar" style="margin: 0px;">
                                    <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                        <tr>
                                            <td>
                                                <input type="button" style="width: 75px" value="Add Order" onclick="javascript:openAddOrder('<%=Request.QueryString["jobId"] %>'); return false;" />
                                                <asp:Button ID="btnTippingSheet" cssclass="buttonClass" runat="server" Text="Tip Sheet(s)" Visible="false" />
                                                <asp:Button ID="btnPodLabels" runat="server" Text="POD Labels" />
                                                <asp:Button ID="btnConfigurePalletHandling" runat="server" Text="Pallet Handling" />
                                                <asp:Button ID="btnLoadingSummarySheet" runat="server" Text="Loading Sheet" />
                                                <input type="button" style="width: 75px;" value="Dockets" onclick="javascript:openUpdateDockets('<%=Request.QueryString["jobId"] %>'); return false;" />
                                                
                
                                                <input type="button" id="Button1" runat="server" style="width: 200px;" value="View Driver Check-In Details" onclick="javascript:searchForCheckIn();" />
                
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="padding-right: 0px; padding-left: 0px;">
                <table style="background-color: White; border: 1px solid #CCC;" cellpadding="5px" width="100%">
                    <tr>
                        <td>
                            <span style="vertical-align: top;">
                                <img align="middle" id="imgExpandCollapseLegs" alt="Expand-Collapse" src="../images/topItem_col.gif"
                                    onclick="javascript: showHideDiv('divLegs','imgExpandCollapseLegs', 'images/topItem_col.gif','images/topItem_exp.gif');" />&nbsp;<a
                                        name="Legs" style="color: Black; text-decoration: none;"><b>Leg Planning</b></a></span>
                            <div id="divLegs" style="display: inline;">
                                <asp:Panel ID="pnlTrafficSheet" runat="server">
                                    <telerik:RadGrid ID="grdTrafficSheet" runat="server" AutoGenerateColumns="false"
                                        EnableAJAX="false" EnableViewState="false" Skin="Orchestrator">
                                        <MasterTableView Name="MasterTable" CommandItemDisplay="None" EditMode="InPlace" >
                                            <Columns>
                                                <telerik:GridTemplateColumn Display="false" UniqueName="colHiddenKeys">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblHiddenKeys" runat="server" /></ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Plan By" HeaderStyle-Width="50">
                                                    <ItemTemplate>
                                                        <a id="hypPlanBy" runat="server"></a>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Start">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblStart" HeaderStyle-Width="75"></asp:Label>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="From">
                                                    <ItemTemplate>
                                                        <b><span runat="server" id="spnCollectionPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx"></span></b>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Finish">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFinish" HeaderStyle-Width="75"></asp:Label>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="To">
                                                    <ItemTemplate>
                                                        <b><span runat="server" id="spnDestinationPoint"  class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx"></span></b>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Driver">
                                                    <ItemTemplate>
                                                        <span runat="server" id="spnDriver"></span>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Vehicle">
                                                    <ItemTemplate>
                                                        <a runat="server" id="hypVehicle"></a>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Trailer">
                                                    <ItemTemplate>
                                                        <a runat="server" id="hypTrailer"></a>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Subby Rate" ItemStyle-HorizontalAlign="Right">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkSubContractRate" runat="server" Text="" CommandName="edit"></asp:LinkButton>
                                                        <asp:TextBox ID="txtSubContractRate" runat="server" Visible="false" Width="50"></asp:TextBox>&nbsp;
                                                        <asp:LinkButton ID="lnkSaveRate" runat="server" Text="Save" CommandName="save" Visible="false"></asp:LinkButton>&nbsp;
                                                        <asp:LinkButton ID="lnkCancel" runat="server" Text="Cancel" CommandName="cancel"
                                                            Visible="false"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings Selecting-AllowRowSelect="true">
                                            <ClientEvents OnRowContextMenu="RowContextMenu" OnRowSelected="RowSelected" ></ClientEvents>
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </asp:Panel>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="padding-right: 0px; padding-left: 0px;">
                <asp:Panel ID="pnlMwfMessages" runat="server" Visible="false">
                    <table style="background-color: White; border: 1px solid #CCC;" cellpadding="5px" width="100%">
                        <tr>
                            <td>
                                <span style="vertical-align: top;">
                                    <img align="middle" id="imgExpandCollapseMwfMessages" alt="Expand-Collapse" src="../images/topItem_col.gif"
                                        onclick="javascript: showHideDiv('divMwfMessages','imgExpandCollapseMwfMessages', 'images/topItem_col.gif','images/topItem_exp.gif');" />&nbsp;
                                    <a name="MwfMessages" style="color: Black; text-decoration: none;">
                                        <b>Driver Messages</b>
                                    </a>
                                </span>
                                <div id="divMwfMessages" style="display: inline;">
                                    <telerik:RadGrid runat="server" ID="grdMwfMessages" AutoGenerateColumns="false" AllowSorting="true">
                                        <MasterTableView AllowPaging="false" DataKeyNames="DriveDateTime, CompleteDateTime" NoMasterRecordsText="No messages have been sent">
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Sent by" />
                                                <telerik:GridBoundColumn DataField="CommunicateDateTime" HeaderText="Date/time sent" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                                <telerik:GridBoundColumn DataField="Location" HeaderText="Point" />
                                                <telerik:GridBoundColumn DataField="ArriveDateTime" HeaderText="Message date/time" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                                <telerik:GridBoundColumn DataField="DriverNames" HeaderText="Driver" />
                                                <telerik:GridBoundColumn DataField="Message" HeaderText="Message" />
                                                <telerik:GridBoundColumn DataField="AcknowledgedDateTime" HeaderText="Acknowledged" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
    
    <asp:Panel ID="pnlCancelJobConfirmation" runat="server" style="width: 400px; background-color: white; border-width: 2px; border-color: black; border-style: solid; padding: 20px; color: Silver; display: none;">
        <h2>Confirmation Needed</h2>
        <p>
            Are you sure you want to cancel this Run?
        </p>
        <div class="buttonBar">
            <asp:Button ID="ButtonOk" runat="server" Text="I am sure" />
            <asp:Button ID="ButtonCancel" runat="server" Text="Do not cancel this run" />
        </div>
    </asp:Panel>

    <uc:MwfDriverMessaging runat="server" ID="mwfDriverMessaging" />

    <telerik:RadCodeBlock runat="server">
    
    <script type="text/javascript">
        function openResourceWindow(InstructionId, Driver, DriverResourceId, RegNo, VehicleResourceId, TrailerRef, TrailerResourceId, legStart, legEnd, DepotCode, lastUpdateDate, jobId) {
             var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&RegNo=" + RegNo + "&VR=" + VehicleResourceId + "&TrailerRef=" + TrailerRef + "&TR=" + TrailerResourceId + "&LS=" + legStart + "&LE=" + legEnd + "&DC=" + DepotCode + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds + "&LastUpdateDate=" + lastUpdateDate + "&jobId=" + jobId + "&depotId=<%=DepotId %>";
             <%=dlgResourceThis.ClientID %>_Open(qs)
        }

        function openTrafficSheetFilterWindow() {
         
        }
        
        function openAddOrder(jobId) {
            var qs = "jid=" + jobId;
            <%=dlgAddOrder.ClientID %>_Open(qs);
        }
        
        function openUpdateDockets(jobID)
        {
            var qs = "jid=" + jobId;
            <%=dlgUpdateDockets.ClientID %>_Open(qs);
        }

        function ClockIn(resourceId) {
            var url = "../Resource/Driver/EnterDriverStartTimes.aspx?resourceId=" + resourceId + "&date=" + todaysDate;
            window.open(url, 'clockin', 'width=500, height=280');
        }

        function openAlterBookedTimesWindow(jobId, lastUpdateDate) {
            var qs = "jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            <%=dlgBookedTimes.ClientID %>_Open(qs)
        }

        function openAlterPlannedTimesWindow(jobId, lastUpdateDate) {
            var qs = "jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            <%=dlgPlannedTimes.ClientID %>_Open(qs);
        }

        function openSubContractWindow(jobId, lastUpdateDate) {
            var qs = "jobId=" + jobId + "&lastUpdateDate=" + lastUpdateDate + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds;
            <%=dlgSubcontract.ClientID %>_Open(qs);
        }

        function UnSubContractLeg(legID, subContractorId) {
            if (subContractorId == null )
                alert("This leg has not been subcontracted.");
            else
                window.open("/traffic/unsubcontractleg.aspx?lID=" + legID + "&returnURL=" + escape(location.href), 'uncontract');
        }

        function UnSubContractLegPostBack(instructionID, instructionStateId, subContractorId) {
            if (subContractorId == null)
            {
                alert("This leg has not been subcontracted." + subContractorId + ".");
            }
            else if(instructionStateId == 3)
            {
                alert("You cannot remove the subcontractor from legs which are in progress and/or communicated.");
            }
            else if(instructionStateId == 4)
            {
                alert("You cannot remove the subcontractor from a completed leg.");
            }
            else
            {
                __doPostBack('uncontractleg', instructionID);
            }
        }  

        function openCommunicateWindow(InstructionID, Driver, DriverResourceId, SubContractorId, JobId, lastUpdateDate) {
            var qs = "iID=" + InstructionID + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&SubbyId=" + SubContractorId + "&jobId=" + JobId + "&LastUpdateDate=" + lastUpdateDate ;
            <%=dlgCommunicate.ClientID %>_Open(qs);
                
        }

        function openTrunkWindow(InstructionId, Driver, RegNo, LastUpdateDate) {
            var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&RegNo=" + RegNo + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgTrunk.ClientID %>_Open(qs);
        }

        function openMultiTrunkWindow(JobId, InstructionId, LastUpdateDate) {
            var qs = "jobID=" + JobId + "&iID=" + InstructionId + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgMultiTrunk.ClientID %>_Open(qs);
        }

        function openRemoveTrunkWindow(JobId, InstructionId, LastUpdateDate) {
            var qs = "jobId=" + JobId + "&iID=" + InstructionId + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgRemoveTrunk.ClientID %>_Open(qs);
        }
        
        function openAddDestination(jobId, LastUpdateDate)
        {
            var qs = 'wiz=true&jobId=' + jobId + '&LastUpdateDate=' + LastUpdateDate;
            <%=dlgAddDestination.ClientID %>_Open(qs);
        }

        function openRemoveDestination(jobId, LastUpdateDate)
        {
            var qs = 'wiz=true&jobId=' + jobId + '&LastUpdateDate=' + LastUpdateDate;
            <%=dlgRemoveDestination.ClientID %>_Open(qs);
        }
        
        function openUpdateTrunkWindow(InstructionId, Driver, RegNo, LastUpdateDate) {
            var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&RegNo=" + RegNo + "&LastUpdateDate=" + LastUpdateDate + "&IsUpdate=true";
            <%=dlgTrunk.ClientID %>_Open(qs);
            return false;
        }
        
        function openAddMultipleDestinations(jobId, LastUpdateDate)
        {
            var qs = 'wiz=true&jobId=' + jobId + '&LastUpdateDate=' + LastUpdateDate;
            <%=dlgAddMultiDestination.ClientID %>_Open(qs);
        }
        
        function sendMwfMessage(jobID, driverID) {
            new MwfDriverMessaging([driverID], jobID).sendMessage()
                .done(function() {
                    alert('The message has been sent.');

                    var url = '/job/job.aspx?wiz=true&jobId=' + jobId+ getCSID();

                    location = url;
                })
                .fail(function (error) {
                    alert(error);
                });
        }

        $(document).ready(function(){
            $('.ShowOrderNotesTooltip').each(function(i, item){
                $(item).qtip({
                            style: {name: 'dark',
                                    width:{min:176}
                            },
                            position: { adjust: { screen: true } },
                            content: {
                                            url:$(item).attr('rel') ,
                                            data: {orderID: $(item).attr('orderID'), isCollection: $(item).attr('isCollection')}, 
                                            method: 'get'
                                         }
                            }
                            
                            
                );
            });
            
            $('.ShowPointTooltip').each(function(i, item){
                $(item).qtip({
                            style: {name: 'dark',
                                    width:{min:176}
                            },
                            position: { adjust: { screen: true } },
                            content: {
                                            url:$(item).attr('rel') ,
                                            data: {pointId: $(item).attr('pointid')}, 
                                            method: 'get'
                                         }
                            }
                            
                            
                );
            });

            $('#createManifestDatePrompt').dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                buttons: {
                    'OK': function() {
                        var createManifestButton = $(this).data('createManifestButton');
                        $(this).dialog('close');

                        if (createManifestButton.length > 0 && createManifestButton[0].click) {
                            var manifestDate = findComponentBySelector('[id$=dteManifestDate]').get_selectedDate();
                            $('[id$=hidManifestDate]').val(manifestDate ? $.datepicker.formatDate('yy-mm-dd', manifestDate) : '');
                            createManifestButton[0].click();
                        }
                    },
                    'Cancel': function() {
                        $(this).dialog('close');
                    }
                }
            });

            $('.createManifestLink').click(function() {
                var createManifestButton = $(this).next('[id$=hidCreateManifestButton]');
                var createManifestDatePrompt = $('#createManifestDatePrompt');
                createManifestDatePrompt.data('createManifestButton', createManifestButton);
                createManifestDatePrompt.dialog('open');
            });
        });
    </script>
    
    </telerik:RadCodeBlock>

    <div id="createManifestDatePrompt" style="display: none;">
        <label for="txtManifestDate">Manifest Date</label>
        <telerik:RadDatePicker ID="dteManifestDate" runat="server" Width="120px" />
    </div>​

    <asp:HiddenField ID="hidManifestDate" runat="server" />
</asp:Content>
