<%@ Reference Page="~/error.aspx" %>

<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.tsheet" Codebehind="tsheet.aspx.cs" MasterPageFile="~/default_tableless.Master" Title="Daily Traffic Sheet" %>

<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">

    <style type="text/css">
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

    <script language="javascript" type="text/javascript">
        function ShowContextMenu(item, column, evt) {
            dgBasicJobs.Select(item);
            GridContextMenu.ShowContextMenu(evt);
            GridContextMenu.ContextData = dgBasicJobs.GetSelectedItems(); ;

            return false;
        }

        function ContextMenuClickHandler(item) {
            var jobId = GridContextMenu.ContextData[0].GetMember("JobId").Value;
            var lastUpdateDate = GridContextMenu.ContextData[0].GetMember("LastUpdateDate").Value;
            var jobStateId = GridContextMenu.ContextData[0].GetMember("JobStateId").Value;

            GridContextMenu.Hide();

            if (item.Text == "Run Details")
                OpenJobDetails(jobId);
            else if (item.Text == "Call In")
                OpenJobManagement(jobId);
            else if (item.Text == "Sub-Contract") {
                if (jobStateId > 1)
                    alert("You cannot sub contract this run as it is already in progress.");
                else
                    openSubContractWindow(jobId, lastUpdateDate);
            }
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

        function onGridDoubleClick(gi) {
            var jobId = gi.GetMember("JobId").Value;
            openJobDetailsWindow(jobId);
        }

        function ContextMenuOnShow(menu) {

        }
   </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Run Sheet</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
    <cc1:Dialog ID="dlgFilter" URL="/Traffic/Filters/jobsheetfilter.aspx" Width="700" Height="680" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true"></cc1:Dialog>
 
    <div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
	    <table style="background-color: white; border:solid 1pt black; " cellpadding="2">
		    <tr>
			    <td><span id="spnPointAddress"></span></td>
		    </tr>
	    </table>
    </div>
	
    <div class="buttonBar">
       <input type="button" value="Change Filter" runat="server" onclick="javascript:openTrafficSheetFilterWindow()" />
    </div>
    
    <div>
        <asp:Label id="lblJobCount" runat="server" ></asp:Label> for Period <b><%=m_trafficSheetFilter.FilterStartDate.ToString("dd/MM/yy HH:mm")%></b> to <b><%=m_trafficSheetFilter.FilterEnddate.ToString("dd/MM/yy HH:mm")%></b>
    </div>

	<div align="center" id="topPortion">
		<table width="100%" cellspacing="0" cellpadding="0" border="0" >
			<tr valign="top">
				<td>
					<uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
					<asp:CheckBox id="chkIgnoreInstructionTimings" runat="server" Visible="False" Text="Ignore run timings?" TextAlign="Right"></asp:CheckBox>
				</td>
			</tr>
			<tr><td>&nbsp;</td></tr>
			<tr>
				<td>
				    <table cellpadding="0" cellspacing="0" width="100%">
				        <tr>
				            <td><img src="../images/stateLegend.gif" /></td>
				            <td align="right" width="100%"><asp:imagebutton id="imbRefresh" runat="server" imageurl="../images/btnrefresh.gif"></asp:imagebutton></td>
				        </tr>
				    </table>
				</td>
			</tr>
			<tr>
				<td align="center">
					<table width="100%" border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td valign="top" style="padding-top: 10px; text-align:left;">
								<asp:Label id="lblError" runat="server"></asp:Label>
								<ComponentArt:Grid id="dgBasicJobs" 
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
                                    ShowSearchBox="True"
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
                                    ImagesBaseUrl="~/images/" 
                                    PagerImagesFolderUrl="~/images/pager/"
                                    TreeLineImagesFolderUrl="~/images/lines/" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    Width="100%" runat="server"
                                    KeyboardEnabled ="true"
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
                                                <ComponentArt:GridColumn DataField="JobState" AllowGrouping="True" AllowSorting="True" HeadingText="Run State" />
                                                <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="True" HeadingText="Run Id" DataCellServerTemplateId="JobId" Width="60" />
                                                <ComponentArt:GridColumn DataField="OrganisationName" Visible="true" HeadingText="Client" Width="100"/>
                                                <ComponentArt:GridColumn DataField="Load Number" AllowGrouping="False" AllowSorting="True" HeadingText="Load Number" Width="90" Align="Right" />
                                                <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Collections" IsSearchable="True" DataCellServerTemplateId="Collections" Width="350" />
                                                <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Deliveries" IsSearchable="True" DataCellServerTemplateId="Deliveries" Width="350" />
                                                <ComponentArt:GridColumn DataField="ChargeAmount" Visible="true" FormatString="C" Width="100"/>
                                                <ComponentArt:GridColumn DataField="LastUpdateDate" Visible="False" />
                                                <ComponentArt:GridColumn DataField="IssuedPCVs" Visible="False" />
                                                <ComponentArt:GridColumn DataField="Requests" Visible="false" />
                                                <ComponentArt:GridColumn DataField="HasPCVAttached" Visible="false" />
                                                <ComponentArt:GridColumn DataField="HasDehireReceipt" Visible="false" />
                                                <ComponentArt:GridColumn DataField="HasExtra" Visible="false" />
                                                <componentart:GridColumn DataField="HasPalletHandling" Visible="false" />
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
											    <table width="70">
												    <tr valign="top">
													    <td colspan="7" align="center" align="right">
        												    <a id="lnkManageJob" runat="server" title="Manage this run."></a>
													    </td>
												    </tr>
												    <tr>
													    <td><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;" /></td>
													    <td><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The run has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand" /></td>
													    <td><img id="imgHasNewPCVs" runat="server" width="10" height="10" src="../images/yellow_tick.gif" alt="PCVs were issued on this run" style="VERTICAL-ALIGN: -3px;" /></td>
													    <td><img id="imgHasExtra" runat="server"  src="../images/ico_extra.png" alt="There are extras attached to orders on this run" style="vertical-align:-3px;" /></td>
													    <td><img id="imgHasPCVAttached" runat="server" width="10" height="10" src="/App_Themes/Orchestrator/Img/MasterPage/icon-pcv.png" alt="PCVs are attached to this run" style="VERTICAL-ALIGN: -3px;" /></td>
                                                        <td><img id="imgHasDehireReceipt" runat="server" width="10" height="10" src="/App_Themes/Orchestrator/Img/MasterPage/icon-receipt.png" alt="Dehire Receipt Issued" style="VERTICAL-ALIGN: -3px;" /></td>
                                                        <td><img id="imgHadPalletHandling" runat="server" width="16" height="16" src="~/App_Themes/Orchestrator/Img/MasterPage/icon-pallet-small.png" alt="Pallet Handling on this run" style="vertical-align: -3px;" /></td>
												    </tr>
											    </table>
                                            </Template>
                                        </ComponentArt:GridServerTemplate>

                                        <ComponentArt:GridServerTemplate ID="Collections" Width="350">
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
                                                    <td valign="top" style="padding:5px; width:100%;">
                                                        <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                                                            <tr>
                                                                <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("Load Number").Value ##</nobr></div></td>
                                                            </tr>
                                                            <tr>
                                                                <td colspan="2"></td>
                                                            </tr>
                                                        </table>    
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2" style="height:14px;background-color:#757598;">
                                                        <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td style="padding-left:5px;color:white;font-family:verdana;font-size:10px;">
                                                                Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgBasicJobs.PageCount ##</b>
                                                                </td>
                                                                <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                                                Run <b>## DataItem.Index + 1 ##</b> of <b>## dgBasicJobs.RecordCount ##</b>
                                                                </td>
                                                            </tr>
                                                        </table>  
                                                    </td>
                                                </tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                                </ComponentArt:Grid>
								<div style="height:10px;"></div>
								<asp:Button id="btnAddPlannerRequest" runat="server" Text="Add Planner Request" visible="false"></asp:Button>&nbsp;<asp:Button id="btnUpdateOwnership" runat="server" Text="Update My runs" visible="false"></asp:Button>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	
	<div align="center" id="inProgress" style="display: none">
		<table width="99%" cellspacing="2" cellpadding="0" border="0">
			<tr valign="top">
				<td align="center">
					<h1>Please wait...</h1>
				</td>
			</tr>
		</table>
	</div>

<script language="javascript" type="text/javascript">
<!--
    var nIndex = 1;
	var storedClientId = "<%=m_clientId%>";
	var storedTownId = "<%=m_townId%>";
	var storedPointId = "<%=m_pointId%>";
	
	var storedSelectPointClientId = "<%=m_selectPointClientId%>";
	var storedSelectPointTownId = "<%=m_selectPointTownId%>";
	var storedSelectPointPointId = "<%=m_selectPointPointId%>";
	var storedSelectPointTownName = "<%=m_selectPointTownName%>"
	
	function ClockIn(resourceId, date)
	{
		window.location.href = '../Resource/Driver/EnterDriverStartTimes.aspx?resourceId=' + resourceId + '&date=' + date;
	}
	
	function ShowFuture(resourceId, resourceTypeId, fromDate)
	{
		window.open('../Resource/Future.aspx?wiz=true&resourceId=' + resourceId + '&resourceTypeId=' + resourceTypeId + '&fromDateTime=' + fromDate, 'resourceFuture', 'width=1050,height=400,resizable=no,scrollbars=yes');
	}
	
	function ShowDriverRequests(resourceId, fromDate)
	{
		window.open('../Resource/Driver/DriverRequests.aspx?wiz=true&resourceId=' + resourceId + '&fromDateTime=' + fromDate, 'resourceRequests', 'width=550,height=400,resizable=no,scrollbars=yes');
	}
	
	function ShowPlannerRequests(jobId)
	{
		window.open('ListRequestsForJob.aspx?wiz=true&jobId=' + jobId, 'plannerRequests', 'width=850,height=400,resizable=no,scrollbars=yes');
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

	function SelectPointClientStateClient()
	{
		var state = new Object();
		
		state["ClientId"] = storedSelectPointClientId;
		
		return state;
	}

	function SelectPointClientStateTown()
	{
		var state = new Object();
		
		state["TownId"]		= storedSelectPointTownId;
		state["TownName"]	= storedSelectPointTownName;

		return state;
	}

	function SelectPointClientStatePoint()
	{
		var state = new Object();

		state["ClientId"]	= storedSelectPointClientId;
		state["TownId"]		= storedSelectPointTownId;
		state["TownName"]	= storedSelectPointTownName;
		state["PointId"]	= storedSelectPointPointId;

		return state;
	}

	function StoreSelectPointClient(Value, Text, SelectionType)
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedSelectPointClientId = Value;
		}
	}			

	function StoreSelectPointTown(Value, Text, SelectionType) 
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedSelectPointTownId = Value;
			storedSelectPointTownName = Text
		}
	}

	function StoreSelectPointPoint(Value, Text, SelectionType) 
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedSelectPointPointId = Value;
		}
		}
	
	function HideTop()
	{
		if (typeof(Page_ClientValidate) == 'function')
		{
			if (Page_ClientValidate())
			{
				var topPortion = document.getElementById("topPortion");
				var inProgress = document.getElementById("inProgress");
				
				if (topPortion != null && inProgress != null)
				{
					topPortion.style.display = "none";
					inProgress.style.display = "";
				}
			}
		}
		else
		{
			var topPortion = document.getElementById("topPortion");
			var inProgress = document.getElementById("inProgress");
			
			if (topPortion != null && inProgress != null)
			{
				topPortion.style.display = "none";
				inProgress.style.display = "";
			}
		}
	}
	
	function FilterResources(pointId, legId)
	{
		var hidPointId = document.getElementById("hidPointId");
		var hidLegId = document.getElementById("hidLegId");
		var btnFilterResourcesForPoint = document.getElementById("btnFilterResourcesForPoint");
		
		if (hidPointId != null && pointId > 0 && hidLegId != null && legId > 0 && btnFilterResourcesForPoint != null)
		{
			hidPointId.value = pointId;
			hidLegId.value = legId;
			
			btnFilterResourcesForPoint.click();
		}
	}
	
	function SelectResource(resourceId, resourceTypeId, resourceName)
	{
		var hidResourceId = document.getElementById("hidResourceId");
		var hidResourceTypeId = document.getElementById("hidResourceTypeId");
		var btnAssign = document.getElementById("btnAssign");
		
		if (btnAssign != null && hidResourceId != null && hidResourceTypeId != null)
		{			
			if (confirm("Assign " + resourceName + " to this run?"))
			{
				hidResourceId.value = resourceId;
				hidResourceTypeId.value = resourceTypeId;
				
				HideTop();
				
				btnAssign.click();
			}
		}
	}

	function GetResource(scheduleName, nIndex)
	{
		var scheduleControl = document.getElementById(scheduleName);

		if (scheduleControl != null)
		{
			var resourceTypeId = scheduleControl.CellText(nIndex, 3);
			var resourceId = scheduleControl.CellText(nIndex, 2);
			var resourceName = scheduleControl.CellText(nIndex, 1);
			
			SelectResource(resourceId, resourceTypeId, resourceName);
		}
	}

	function GetScheduleData(scheduleControl, urlHolder, scheduleHolder, resourceType)
	{
		var scheduleControl = document.getElementById(scheduleControl);
		var success = false;
		
		if (scheduleControl != null)
		{
			var urlControl = document.getElementById(urlHolder);
			
			if (urlControl != null)
			{
				var url = urlControl.value;
				
				if (url != "")
				{
					success = scheduleControl.ReadFile(url + '&resourceType=' + resourceType, 0);
				}
			}
		}
		
		if (document.getElementById(scheduleHolder) != null)
		{
			if (success)
			{
				document.getElementById(scheduleHolder).style.display = "inline";
			}
			else
			{
				document.getElementById(scheduleHolder).style.display = "none";
			}
		}
	}
//-->
</script>
<script for="ctDriverSchedule" event="FirstDraw()" language="javascript">
	var scheduleControl = document.getElementById("ctDriverSchedule");
	
	if (scheduleControl != null)
	{
		scheduleControl.TipsType = 2;
		scheduleControl.TipsDelay = 0;
	}
</script>
<script for="ctVehicleSchedule" event="FirstDraw()" language="javascript">
	var scheduleControl = document.getElementById("ctVehicleSchedule");
	
	if (scheduleControl != null)
	{
		scheduleControl.TipsType = 2;
		scheduleControl.TipsDelay = 0;
	}
</script>
<script for="ctTrailerSchedule" event="FirstDraw()" language="javascript">
	var scheduleControl = document.getElementById("ctTrailerSchedule");
	
	if (scheduleControl != null)
	{
		scheduleControl.TipsType = 2;
		scheduleControl.TipsDelay = 0;
	}
</script>
<script for="ctDriverSchedule" event="ListDblClick(nIndex)" language="javascript">
	GetResource("ctDriverSchedule", nIndex);
</script>
<script for="ctVehicleSchedule" event="ListDblClick(nIndex)" language="javascript">
	GetResource("ctVehicleSchedule", nIndex);
</script>
<script for="ctTrailerSchedule" event="ListDblClick(nIndex)" language="javascript">
	GetResource("ctTrailerSchedule", nIndex);
</script>
<script type="text/javascript" language="javascript">
        // Filter data
        var activeControlAreaId = <%=m_trafficSheetFilter.ControlAreaId %>;
        var activeTrafficAreaIds = "<%=TrafficAreas %>";
        //Window Code
       
        
        function openTrafficSheetFilterWindow()
        {
            var qs= "";
            <%=dlgFilter.ClientID %>_Open(qs);
        }
        
         function openSubContractWindow(jobId, lastUpdateDate)
        {
            var qs = "SubContract.aspx?jobId=" + jobId + "&lastUpdateDate=" + lastUpdateDate + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds;
            window.open(qs, 'SubContract', 'height=320, width=400');
            
            
        }
</script>

</asp:Content>