<%@ Page Language="C#" AutoEventWireup="true" MasterPageFilE="~/default_tableless.Master" Inherits="Orchestrator.WebUI.Integration.showUnalteredCollectionJobs" Codebehind="showUnalteredCollectionJobs.aspx.cs" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
        function ShowContextMenu(item, column, evt) {
            dgJobs.Select(item);
            GridContextMenu.ShowContextMenu(evt);
            GridContextMenu.ContextData = dgJobs.GetSelectedItems(); ;

            return false;
        }

        function ContextMenuClickHandler(item) {
            var jobId = GridContextMenu.ContextData[0].GetMember("JobId").Value;
            var lastUpdateDate = GridContextMenu.ContextData[0].GetMember("LastUpdateDate").Value;
            var jobStateId = GridContextMenu.ContextData[0].GetMember("JobStateId").Value;

            GridContextMenu.Hide();

            if (item.Text == "Job Details")
                OpenJobDetails(jobId);
            else if (item.Text == "Call In")
                OpenJobManagement(jobId);
            else if (item.Text == "Sub-Contract")
                openSubContractWindow(jobId, lastUpdateDate);
            else if (item.Text == "Change Booked Times") {
                if (jobStateId <= 3)
                    openAlterBookedTimesWindow(jobId, lastUpdateDate);
                else
                    alert("You can not alter the booked times as everything has been called in.");
            }
            else if (item.Text == "Change Planned Times") {
                if (jobStateId <= 3)
                    openAlterPlannedTimesWindow(jobId, lastUpdateDate);
                else
                    alert("You can not alter the planned times as everything has been called in.");
            }
        }

        function openAlterBookedTimesWindow(jobId, lastUpdateDate) {
            var url = "../traffic/changeBookedTimes.aspx?jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            _showModalDialog(url, 500, 320, "Change Booked Times");
        }

        function openAlterPlannedTimesWindow(jobId, lastUpdateDate) {
            var url = "../traffic/changePlannedTimes.aspx?jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            _showModalDialog(url, 800, 320, "Change Planned Times");
        }

        function openSubContractWindow(jobId, lastUpdateDate) {
            var url = "../traffic/SubContract.aspx?jobId=" + jobId + "&lastUpdateDate=" + lastUpdateDate;
            _showModalDialog(url, 400, 320, 'Sub-Contract Job');
        }
        function ContextMenuOnShow(menu) {
            var miSubContractId = "<%=GridContextMenu.ClientID%>_0";
            var miTrunkId = "<%=GridContextMenu.ClientID%>_2";
            var miRemoveTrunkId = "<%=GridContextMenu.ClientID%>_3";
            var miResourceId = "<%=GridContextMenu.ClientID%>_4";
            var miBookedTimesId = "<%=GridContextMenu.ClientID%>_6";
            var miPlannedTimesId = "<%=GridContextMenu.ClientID%>_7";
            var miCommunicateId = "<%=GridContextMenu.ClientID%>_9";
            var miJobDetailsId = "<%=GridContextMenu.ClientID%>_10";
            var miCallInId = "<%=GridContextMenu.ClientID%>_11";

            var miSubContract = top.document.getElementById(miSubContractId);
            var miTrunk = top.document.getElementById(miTrunkId);
            var miRemoveTrunk = top.document.getElementById(miRemoveTrunkId);
            var miResource = top.document.getElementById(miResourceId);
            var miCommunicate = top.document.getElementById(miCommunicateId);
            var miJobDetails = top.document.getElementById(miJobDetailsId);
            var miCallIn = top.document.getElementById(miCallInId);
            var miBookedTimes = top.document.getElementById(miBookedTimesId);
            var miPlannedTimes = top.document.getElementById(miPlannedTimesId);

            // Disable SubContract
            miSubContract.className = "DisabledMenuItem";
            miSubContract.onmouseover = null;
            miSubContract.onmouseout = null;

            //disable Trunk
            miTrunk.className = "DisabledMenuItem";
            miTrunk.onmouseover = null;
            miTrunk.onmouseout = null;

            //disable Remove Trunk
            miRemoveTrunk.className = "DisabledMenuItem";
            miRemoveTrunk.onmouseover = null;
            miRemoveTrunk.onmouseout = null;

            //disable Resource This
            miResource.className = "DisabledMenuItem";
            miResource.onmouseover = null;
            miResource.onmouseout = null;

            //disable Communicate This
            miCommunicate.className = "DisabledMenuItem";
            miCommunicate.onmouseover = null;
            miCommunicate.onmouseout = null;

            //disable Call In
            miCallIn.className = "DisabledMenuItem";
            miCallIn.onmouseover = null;
            miCallIn.onmouseout = null;

            // Disable Planned Times
            miPlannedTimes.className = "DisabledMenuItem";
            miPlannedTimes.onmouseover = null;
            miPlannedTimes.onmouseout = null;
        }
        
	    function ShowPlannerRequests(jobId)
	    {

	        var url = '../Traffic/ListRequestsForJob.aspx?wiz=true&jobId=' + jobId + getCSID();

		    window.open(url, 'plannerRequests', 'width=850,height=400,resizable=no,scrollbars=yes');
	    }
    	
	    function ShowPoint(url, pointId)
	    {
		    var txtPointAddress = document.getElementById('txtPointAddress');
		    var pageUrl = url + "?pointId=" + pointId;

		    var xmlRequest = new XMLHttpRequest();
    		
		    xmlRequest.open("POST", pageUrl, false);
		    xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		    xmlRequest.send(null);
    		
		    var spnPointAddress = document.getElementById('spnPointAddress');

		    if (spnPointAddress != null && divPointAddress != null)
		    {
			    spnPointAddress.innerHTML = xmlRequest.responseText;
			    divPointAddress.style.display = '';
			    divPointAddress.style.position = "absolute";
			    xMousePos = window.event.x;
			    yMousePos = window.event.y;
			    divPointAddress.style.left = xMousePos + 10 + "px";
			    divPointAddress.style.top = yMousePos + 8 + "px";
		    }
	    }
    	
	    function HidePoint()
	    {
		    if (divPointAddress != null)
			    divPointAddress.style.display = 'none';
	    }
    	
	    function OpenJob(jobId)
	    {
	        openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=' + jobId + getCSID(), '600', '400');
	    }
    	
	    function OpenJobDetails(jobId)
	    {
		    openDialogWithScrollbars('../job/job.aspx?wiz=true&jobId=' + jobId,'600','400');
	    }
    	
	    function OpenJobManagement(jobId)
	    {
	        openDialogWithScrollbars('../traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=' + jobId + getCSID(), '600', '400');
	    }	

        //Window Code
        function _showModalDialog(url, width, height, windowTitle) {
            MyClientSideAnchor.WindowHeight = height + "px";
            MyClientSideAnchor.WindowWidth = width + "px";

            MyClientSideAnchor.URL = url;
            MyClientSideAnchor.Title = windowTitle;
            var returnvalue = MyClientSideAnchor.Open();
            if (returnvalue == true) {
                document.all.Form1.submit();
            }
            return true;
        }
         
    </script>
    
    <style>
        .stateBooked  td.DataCell 
        { 
          cursor: default;
          padding: 3px; 
          padding-top: 2px; 
          padding-bottom: 1px; 
          border-bottom: 1px solid #EAE9E1; 
          font-family: verdana; 
          font-size: 11px; 
          background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Booked) %>; 
          
        }

        .statePlanned td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Planned) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateInProgress td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.InProgress) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateCompleted td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Completed) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateBookingInIncomplete td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.BookingInIncomplete) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateBookingInComplete td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.BookingInComplete) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateReadyToInvoice td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.ReadyToInvoice) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateInvoiced td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Invoiced) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateCancelled td.DataCell 
        { 
            height:20px;
            background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Cancelled) %>; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .statePlanned td.DataCell 
        { 
          cursor: default;
          padding: 3px; 
          padding-top: 2px; 
          padding-bottom: 1px; 
          border-bottom: 1px solid #EAE9E1; 
          font-family: verdana; 
          font-size: 11px; 
        } 
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Wisbech Roadways Runs</h1>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Runs for Wisbech Roadways 1 and 4 that have not had their collection dates altered are shown below</h2>
	<div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
		<table style="background-color: white; border:solid 1pt black; " cellpadding="2">
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>
	<cs:webmodalanchor id="MyClientSideAnchor" title="Sub-Contract Job" runat="server" clientsidesupport="true"
		windowwidth="580" windowheight="532" scrolling="false" url="addupdatedriver.aspx" handledevent="onclick"
		linkedcontrolid="dgJobs"></cs:webmodalanchor>
    <ComponentArt:Menu id="GridContextMenu" 
        SiteMapXmlFile="~/traffic/gridMenu.xml"
        ExpandSlide="none"
        ExpandTransition="fade"
        ExpandDelay="250"
        CollapseSlide="none"
        CollapseTransition="fade"
        Orientation="Vertical"
        CssClass="MenuGroup"
        DefaultGroupCssClass="MenuGroup"
        DefaultItemLookID="DefaultItemLook"
        DefaultGroupItemSpacing="1"
        ImagesBaseUrl="~/images/"
        EnableViewState="false"
        ContextMenu="Custom"
        ClientSideOnItemSelect="ContextMenuClickHandler"
        ClientSideOnContextMenuShow="ContextMenuOnShow"
        runat="server">
        <ItemLooks>
            <ComponentArt:ItemLook LookID="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover" LabelPaddingLeft="15" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" />
            <ComponentArt:ItemLook LookID="BreakItem" ImageUrl="break.gif" CssClass="MenuBreak" ImageHeight="1" ImageWidth="100%" />
            <ComponentArt:ItemLook LookId="DisabledItemLook" CssClass="DisabledMenuItem" HoverCssClass="DisabledMenuItem"  LabelPaddingLeft="15" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" />
        </ItemLooks>
    </ComponentArt:Menu>
	<div class="infoPanel">
        Currently the loads coming down automatically from Wisbech Roadways do not have the correct collection times, so they must be altered to match those on SOLAR.
        <br /><br />
        In the next few weeks, this should be resolved, but for now this page shows you all jobs Wisbech Roadways 1 and 4 that have not had their collection times altered.
        <br /><br />
        You can right click on the runs and change the booked times to the correct values.
    </div>
    <div class="whitespacepusher"></div>
    <div class="buttonbar">
        <asp:Button id="btnRefresh" runat="server" Text="Refresh"></asp:Button>
    </div>							
	<table id="tblLegend" runat="server" border="0" cellpadding="2" cellspacing="1" STYLE="border:solid 1pt black;" width="100%">
		<tr height="20">
			<td align="center" width="11%">Booked</td>
			<td align="center" width="11%">Planned</td>
			<td align="center" width="11%">In Progress</td>
			<td align="center" width="11%">Completed</td>
			<td align="center" width="12%">Booking In Incomplete</td>
			<td align="center" width="11%">Booking In Complete</td>
			<td align="center" width="11%">Ready To Invoice</td>
			<td align="center" width="11%">Invoiced</td>
			<td align="center" width="11%">Cancelled</td>
		</tr>
	</table>
	<div class="whitespacepusher"></div>							
    <ComponentArt:Grid id="dgJobs" 
     RunningMode="Client" 
     CssClass="Grid" 
     AllowEditing="False"
     HeaderCssClass="GridHeader" 
     FooterCssClass="GridFooter"

     GroupByTextCssClass="GroupByText"
     GroupingNotificationTextCssClass="GridHeaderText"
     GroupBySortAscendingImageUrl="group_asc.gif"
     GroupBySortDescendingImageUrl="group_desc.gif"
     GroupBySortImageWidth="10"
     GroupBySortImageHeight="10"
     GroupingPageSize = "25"
     ShowHeader="true"
     ShowSearchBox="False"
     SearchOnKeyPress="false"
     PageSize="25" 
     PagerStyle="Slider" 
     PagerTextCssClass="GridFooterText"
     PagerButtonWidth="41"
     PagerButtonHeight="22"
     SliderHeight="20"
     PreExpandOnGroup="false"
     SliderWidth="150" 
     SliderGripWidth="9" 
     SliderPopupOffsetX="20"
     SliderPopupClientTemplateId="SliderTemplate" 
     ImagesBaseUrl="../images/" 
     PagerImagesFolderUrl="../images/pager/"
     TreeLineImagesFolderUrl="../images/lines/" 
     TreeLineImageWidth="22" 
     TreeLineImageHeight="19" 
     Width="100%" runat="server"
     KeyboardEnabled ="true"
     OnContextMenu="ShowContextMenu"
    >
        <Levels>
            <ComponentArt:GridLevel 
             DataKeyField="JobId"
             HeadingCellCssClass="HeadingCell" 
             HeadingRowCssClass="HeadingRow" 
             HeadingTextCssClass="HeadingCellText"
             DataCellCssClass="DataCell" 
             RowCssClass="Row" 
             SelectedRowCssClass="SelectedRow"
             SortAscendingImageUrl="asc.gif" 
             SortDescendingImageUrl="desc.gif" 
             SortImageWidth="10"
             SortImageHeight="10"
             GroupHeadingCssClass="GroupHeading" >
                <Columns>
                    <ComponentArt:GridColumn DataField="JobStateId" Visible="False" />
                    <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="True" HeadingText="Run Id" DataCellServerTemplateId="JobId" Width="60" />
                    <ComponentArt:GridColumn DataField="LoadNo" AllowGrouping="False" AllowSorting="True" HeadingText="Load Number" Width="90" Align="Right" />
                    <ComponentArt:GridColumn DataField="JobState" AllowGrouping="True" AllowSorting="True" HeadingText="State" Width="120" />
                    <ComponentArt:GridColumn DataField="OrganisationName" AllowGrouping="True" AllowSorting="True" HeadingText="Client" Width="200" />
                    <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Collections" DataCellServerTemplateId="Collections" Width="350" />
                    <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Deliveries" DataCellServerTemplateId="Deliveries" Width="350" />
                    <ComponentArt:GridColumn DataField="LastUpdateDate" Visible="false" />
                    <ComponentArt:GridColumn DataField="IssuedPCVs" Visible="False" />
                    <ComponentArt:GridColumn DataField="Requests" Visible="false" />
                </Columns>
                <ConditionalFormats>
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 1" RowCssClass="stateBooked" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 2" RowCssClass="statePlanned" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 3" RowCssClass="stateInProgress" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 4" RowCssClass="stateCompleted" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 5" RowCssClass="stateBookingInIncomplete" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 6" RowCssClass="stateBookingInComplete" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 7" RowCssClass="stateReadyToInvoice" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 8" RowCssClass="stateInvoiced" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 9" RowCssClass="stateCancelled" />
                </ConditionalFormats>
            </ComponentArt:GridLevel>
        </Levels>
        <ServerTemplates>
            <ComponentArt:GridServerTemplate Id="JobId">
                <Template>
                    <table width="60">
                        <tr>
                            <td colspan="3" align="center" align="right">
                                <a id="lnkManageJob" runat="server" title="Manage this job."></a>
                            </td>
                        </tr>
                        <tr>
                            <td width="33%"><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;"></td>
                            <td width="33%"><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand"></td>
                            <td width="33%"><img id="imgHasNewPCVs" runat="server" width="10" height="10" src="../images/yellow_tick.gif" alt="PCVs were issued on this job" style="VERTICAL-ALIGN: -3px;"></td>
                        </tr>
                    </table>
                </Template>
            </ComponentArt:GridServerTemplate>
            <ComponentArt:GridServerTemplate ID="Collections">
                <Template>
                    <asp:Table id="tblCollections" runat="server" Width="350px" CellSpacing="2"></asp:Table>
                </Template>
            </ComponentArt:GridServerTemplate>
            <ComponentArt:GridServerTemplate ID="Deliveries">
                <Template>
                    <asp:Table id="tblDeliveries" runat="server" Width="350px" CellSpacing="2"></asp:Table>
                </Template>
            </ComponentArt:GridServerTemplate>
        </ServerTemplates>
        <ClientTemplates>
            <ComponentArt:ClientTemplate Id="SliderTemplate">
                <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td valign="top" style="padding:5px;">
                            <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                                            <tr>
                                                <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("LoadNo").Value ##</nobr></div></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2"></td>
                                            </tr>
                                        </table>    
                                    </td>
                                </tr>
                            </table>  
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height:14px;background-color:#757598;">
                            <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td style="padding-left:5px;color:white;font-family:verdana;font-size:10px;">
                                        Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgJobs.PageCount ##</b>
                                    </td>
                                    <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                        Job <b>## DataItem.Index + 1 ##</b> of <b>## dgJobs.RecordCount ##</b>
                                    </td>
                                </tr>
                            </table>  
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Grid>				

    
</asp:Content>