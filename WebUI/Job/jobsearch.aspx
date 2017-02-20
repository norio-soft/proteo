<%@ Reference Page="~/error.aspx" %>

<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.jobSearch" Codebehind="jobSearch.aspx.cs" MasterPageFile="~/default_tableless.Master"   %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Job Search Results</h1>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

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

        td.DataCell > div > table
        {
            white-space: normal;
        }
    </style>
    
    <script language="javascript" type="text/javascript" src="/script/popaddress.js" ></script>
    
    <script language="javascript" type="text/javascript">
        function showCount(DataItem) {
            alert(DataItem.Rows.length);
            return dgJobs.Levels[0].Groups[0].Rows.length;
        }

        function ShowContextMenu(item, column, evt) {
            dgJobs.Select(item);
            GridContextMenu.ShowContextMenu(evt);
            GridContextMenu.ContextData = dgJobs.GetSelectedItems(); ;

            return false;
        }

        function ContextMenuClickHandler(item) {
            var jobId = GridContextMenu.ContextData[0].GetMember("JobId").Value;
            var lastUpdateDate = GridContextMenu.ContextData[0].GetMember("LastUpdateDate").Text;
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
            else if (item.Text == "Show Load Order") {
                openResizableDialogWithScrollbars('../Traffic/LoadOrder.aspx?jobid=' + jobId, '700', '258');
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
            var miLinkJobId = "<%=GridContextMenu.ClientID%>_6";
            var miRemoveLinksId = "<%=GridContextMenu.ClientID%>_7";
            var miBookedTimesId = "<%=GridContextMenu.ClientID%>_9";
            var miPlannedTimesId = "<%=GridContextMenu.ClientID%>_10";
            var miCommunicateId = "<%=GridContextMenu.ClientID%>_11";
            var miJobDetailsId = "<%=GridContextMenu.ClientID%>_12";
            var miCallInId = "<%=GridContextMenu.ClientID%>_13";
            var miGiveResource = "<%=GridContextMenu.ClientID%>_14";

            var miSubContract = top.document.getElementById(miSubContractId);
            var miTrunk = top.document.getElementById(miTrunkId);
            var miRemoveTrunk = top.document.getElementById(miRemoveTrunkId);
            var miResource = top.document.getElementById(miResourceId);
            var miLinkJob = top.document.getElementById(miLinkJobId);
            var miRemoveLinks = top.document.getElementById(miRemoveLinksId);
            var miCommunicate = top.document.getElementById(miCommunicateId);
            var miJobDetails = top.document.getElementById(miJobDetailsId);
            var miCallIn = top.document.getElementById(miCallInId);
            var miBookedTimes = top.document.getElementById(miBookedTimesId);
            var miPlannedTimes = top.document.getElementById(miPlannedTimesId);
            var miGiveResource = top.document.getElementById(miGiveResource);

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

            // disable link job
            miLinkJob.className = "DisabledMenuItem";
            miLinkJob.onmouseover = null;
            miLinkJob.onmouseout = null;

            // disable remove links
            miRemoveLinks.className = "DisabledMenuItem";
            miRemoveLinks.onmouseover = null;
            miRemoveLinks.onmouseout = null;

            //disable Communicate This
            miCommunicate.className = "DisabledMenuItem";
            miCommunicate.onmouseover = null;
            miCommunicate.onmouseout = null;

            //disable Call In
            miCallIn.className = "DisabledMenuItem";
            miCallIn.onmouseover = null;
            miCallIn.onmouseout = null;

            // disable give resource
            miGiveResource.className = "DisabledMenuItem";
            miGiveResource.onmouseover = null;
            miGiveResource.onmouseout = null;
        }
        var lastHighlightedRow = "";
        var lastHighlightedRowColour = "";
        var lastHighlightedRowClass = "";

        function HighlightRow(row) {
            var rowElement;
            //alert(lastHighlightedRow);
            if (lastHighlightedRow != "") {
                if (document.getElementById(lastHighlightedRow) != null)
                    document.getElementById(lastHighlightedRow).style.display = "none";

                lastHighlightedRow = "imgSelectRow_" + row.Id;
            }
            else {
                lastHighlightedRow = "imgSelectRow_" + row.Id;
            }
            //alert(row.Id);

            document.getElementById(lastHighlightedRow).style.display = "";
            return true;
        }

        function GetSelectedRowImage(jobId) {

        }

        // Function to display the column configure box 
        function ColumnDisplayShow() {
            $("#tabs").css({ 'display': 'none' });
            $("#dvColumnDisplay").css({ 'display': 'block' });
        }

        // Function to hide the column configure box 
        function ColumnDisplayHide() {
            $("#tabs").css({ 'display': 'block' });
            $("#dvColumnDisplay").css({ 'display': 'none' });
        }

        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
        }

        // Function to show the filter options overlay box
        function keyDisplayShow() {
            $("#overlayedClearKeyBox").css({ 'display': 'block' });
            $("#keyOptionsDiv").css({ 'display': 'none' });
            $("#keyOptionsDivHide").css({ 'display': 'block' });
        }

        function keyDisplayHide() {
            $("#overlayedClearKeyBox").css({ 'display': 'none' });
            $("#keyOptionsDivHide").css({ 'display': 'none' });
            $("#keyOptionsDiv").css({ 'display': 'block' });
        }
     
</script>
    
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgAddUpdateDriver" URL="addupdatedriver.aspx" Width="580" Height="532" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="false"></cc1:Dialog>

    <div>
	    <div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
		    <table style="background-color: white; border:solid 1pt black; " cellpadding="2">
			    <tr>
				    <td><span id="spnPointAddress"></span></td>
			    </tr>
		    </table>
	    </div>
	
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
    
        <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
		    <div class="overlayedKeyIconOff" id="keyOptionsDiv" onclick="keyDisplayShow()">Show row Key</div>
		    <div class="overlayedKeyIconOn" id="keyOptionsDivHide" onclick="keyDisplayHide()" style="display: none;">Close row Key</div>
	    </div>	
	
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
		    <fieldset>
			    <table border=0>
				    <tr>
					    <td colspan="3">
						    <span style="font-size: 12px;">You searched for "<b><asp:Label id="lblSearchExpression" runat="server"></asp:Label></b>", which returned <b><asp:Label id="lblResultCount" runat="server"></asp:Label></b> results.  Please select the run you are interested in, or if you'd like to reduce the amount of results returned, enter a date range.</span>
					    </td>
				    </tr>
				    <tr>
					    <td class="formCellLabel">Search For</td>
					    <td class="formCellField">
						    <asp:TextBox id="txtSearchFor" runat="server"></asp:TextBox>
						    <asp:RequiredFieldValidator id="rfvSearchFor" runat="server" ControlToValidate="txtSearchFor" ErrorMessage="You must provide a search expression."><img src="../images/Error.gif" height="16" width="16" title="You must provide a search expression." /></asp:RequiredFieldValidator>
					    </td>
				    </tr>
				    <tr>
					    <td class="formCellLabel">Run State</td>
					    <td class="formCellField" valign="top"><asp:CheckBoxList id="chkJobStates" runat="server" RepeatDirection="Horizontal" RepeatColumns="9"></asp:CheckBoxList></td>
				    </tr>
				    <tr>
					    <td class="formCellLabel">Run Search Field</td>
					    <td class="formCellField" valign="top" colspan="2"><asp:DropDownList id="cboJobSearchField" runat="server"></asp:DropDownList></td>
				    </tr>
				    <tr>
					    <td class="formCellLabel">Date Range</td>
					    <td>
						    <table cellspacing="0" cellpadding="0">
							    <tr>
								    <td class="formCellField">Start Date &nbsp; </td>
								    <td class="formCellField"><telerik:RadDatePicker id="dteStartDate" runat="server" ToolTip="The start Date to display on the Traffic Sheet" Width="100px">
                                    <DateInput runat="server"
                                    dateformat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                    </td>
								    <td class="formCellField"> &nbsp; End Date &nbsp; </td>
								    <td class="formCellField"><telerik:RadDatePicker id="dteEndDate" runat="server" ToolTip="The start Date to display on the Traffic Sheet" Width="100px">
                                    <DateInput runat="server"
                                    dateformat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker></td>
							    </tr>
						    </table>
					    </td>
				    </tr>
			    </table>
		    </fieldset>
    						
		    <div class="buttonbar">
		        <asp:button id="btnSearch" runat="server" text="Search"/>
		        <input type="button" id="Button2" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide()" />
		    </div>
	    </div>
	
        <div class="overlayedKeyBox" id="overlayedClearKeyBox" style="display: none;">
            <table id="tblLegend" runat="server" border="0" cellpadding="2" cellspacing="1" STYLE="border:solid 1pt black; margin-bottom: 10px;" width="100%">
                <tr style="height:20px;">
	                <td align="center" width="11%">Booked</td>
	                <td align="center" width="11%">Planned</td>
	                <td align="center" width="11%">In Progress</td>
	                <td align="center" width="11%">Completed</td>
	                <td align="center" width="12%" style="display: none">Booking In Incomplete</td>
	                <td align="center" width="11%" style="display: none">Booking In Complete</td>
	                <td align="center" width="11%">Ready To Invoice</td>
	                <td align="center" width="11%">Invoiced</td>
	                <td align="center" width="11%">Cancelled</td>
                </tr>
            </table>
            <div class="buttonbar">
	            <input type="button" id="Button3" runat="server" value="Close" onclick="keyDisplayHide()" />
	        </div>
        </div>

        <ComponentArt:Grid id="dgJobs" 
            RunningMode="server" 
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
            ShowHeader="false"
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
            LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
            LoadingPanelEnabled="true"
            TreeLineImageWidth="22" 
            TreeLineImageHeight="19" 
            Width="100%" runat="server"
            KeyboardEnabled ="true"
            ClientSideOnSelect="HighlightRow"
            OnContextMenu="ShowContextMenu"
            LoadingPanelPosition="MiddleCenter"
            enableviewstate="false">
            
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
                        GroupHeadingCssClass="GroupHeading" 
                        GroupHeadingClientTemplateId="groupByTemplate">
                        
                        <Columns>
                            <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" HeadingText=" " DataCellClientTemplateId="SelectedRowTemplate" Width="14" />
                            <ComponentArt:GridColumn DataField="JobStateId" Visible="False" />
                            <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="True" HeadingText="Run Id" DataCellServerTemplateId="JobId" Width="60" />
                            <ComponentArt:GridColumn DataField="LoadNo" AllowGrouping="False" AllowSorting="True" HeadingText="Load Number" Width="90" Align="Right" />
                            <ComponentArt:GridColumn DataField="JobState" AllowGrouping="True" AllowSorting="True" HeadingText="State" Width="120" />
                            <ComponentArt:GridColumn DataField="OrganisationName" AllowGrouping="True" AllowSorting="True" HeadingText="Client" Width="200" />
                            <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Collections" DataCellServerTemplateId="Collections" Width="340" />
                            <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Deliveries" DataCellServerTemplateId="Deliveries" Width="350" />
                            <ComponentArt:GridColumn DataField="LastUpdateDate" Visible="false" FormatString="dd/MM/yyyy HH:mm:ss" />
                            <ComponentArt:GridColumn DataField="IssuedPCVs" Visible="False" />
                            <ComponentArt:GridColumn DataField="Requests" Visible="false" />
                            <ComponentArt:GridColumn DataField="ExtraId" Visible="false" />
                            <ComponentArt:GridColumn DataField="ForCancellation" Visible="false" />
                            <ComponentArt:GridColumn DataField="ForCancellationReason" Visible="false" />
                            <ComponentArt:GridColumn DataField="JobType" Visible="false" />
                            <ComponentArt:GridColumn DataField="HasPCVAttached" Visible="false" />
                            <ComponentArt:GridColumn DataField="HasDehireReceipt" Visible="false" />
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
                                    <td colspan="4" align="center" align="right">
                                        <a id="lnkManageJob" runat="server" title="Manage this job."></a>
                                    </td>
                                </tr>
                                <tr>
                                    <td ><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;" /></td>
                                    <td ><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand" /></td>
                                    <td ><img id="imgHasExtras" runat="server" visible="false" width="16" height="16" src="../images/extras_ico.gif" style="VERTICAL-ALIGN: -3px;" /></td>														    
                                    <td ><img id="imgHasNewPCVs" runat="server" width="10" height="10" src="../images/yellow_tick.gif" alt="PCVs were issued on this job" style="VERTICAL-ALIGN: -3px;" /></td>
                                    <td ><img id="imgMarkedForCancellation" runat="server" width="16" height="16" src="../images/ico_critical.gif" style="VERTICAL-ALIGN: -3px;" onclick="javascript:openDialogWithScrollbars('canceljob.aspx?wiz=true','1000','600');" /></td>
                                    <td ><img id="imgHasPCVAttached" runat="server" width="10" height="10" src="/App_Themes/Orchestrator/Img/MasterPage/icon-pcv.png" alt="PCVs are attached to this job" style="VERTICAL-ALIGN: -3px;" /></td>
                                    <td ><img id="imgHasDehireReceipt" runat="server" width="10" height="10" src="/App_Themes/Orchestrator/Img/MasterPage/icon-receipt.png" alt="Dehire Receipt Issued" style="VERTICAL-ALIGN: -3px;" /></td>
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
                    <ComponentArt:ClientTemplate Id="SelectedRowTemplate">
                        <img src="../images/selector.gif" id='imgSelectRow_##DataItem.GetMember("JobId").Value##' style="display:none;" /> 
                    </ComponentArt:ClientTemplate>
                    
                    <ComponentArt:ClientTemplate Id="groupByTemplate">## DataItem.ColumnValue ## (## DataItem.Rows.length ##)</ComponentArt:ClientTemplate>
                
                    <ComponentArt:ClientTemplate Id="SliderTemplate">
                        <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
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
                
                    <ComponentArt:ClientTemplate Id="LoadingFeedbackTemplate">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td style="font-size:10px;font-family:Verdana;">Loading...&nbsp;</td>
                                <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                            </tr>
                        </table>
                    </ComponentArt:ClientTemplate>
                </ClientTemplates>
                
        </ComponentArt:Grid>
	</div>

    <script language="javascript" type="text/javascript">
    <!--
        function Search()
        {
            var txtSearchFor = document.getElementById("<%=txtSearchFor.ClientID %>");
            var dteStartDate = null;
            dteStartDate = eval(document.getElementById("<%=dteStartDate.ClientID %>"));
            var dteEndDate = null;
            dteEndDate = eval(document.getElementById("<%=dteEndDate.ClientID %>"));
            var cboJobSearchField = document.getElementById("<%=cboJobSearchField.ClientID %>");
            var states = "";
            for (i = 0; i < 9; i++) {
                var checkClientId = "<%=chkJobStates.ClientID %>" + "_" + i;
                var chkJobStates = document.getElementById(checkClientId);
                if (chkJobStates.checked)
                {
                    if (states.length > 0)
                        states = states + ",";
                    
                    states = states + (i + 1);
                }
            }
            
            var url = webserver + "/job/jobSearch.aspx?searchString=" + txtSearchFor.value + "&from=" + dteStartDate.value + "&to=" + dteEndDate.value + "&field=" + cboJobSearchField.selectedIndex + "&state=" + states;
            location.href = url;
            return false;
            
        }
        
	    function ShowPlannerRequests(jobId)
	    {
		    window.open('../Traffic/ListRequestsForJob.aspx?wiz=true&jobId=' + jobId, 'plannerRequests', 'width=850,height=400,resizable=no,scrollbars=yes');
	    }
    		
	    function HidePoint()
	    {
		    if (divPointAddress != null)
			    divPointAddress.style.display = 'none';
	    }
    	
	    function OpenJob(jobId)
	    {
	        openResizableDialogWithScrollbars('../traffic/<%=Orchestrator.Globals.Configuration.UseJobManagementLink? "Jobmanagement.aspx" : "job.aspx" %>?wiz=true&jobId=' + jobId + getCSID(), '1200', '900');
	    }
    	
        function OpenJobDetails(jobId) {
            var url = '/job/job.aspx?wiz=true&jobId=' + jobId + getCSID();
	        window.open(url, "JobDetails", "width=1200, height=900, scrollbars=yes, resizable=yes,status=1");
    		
	    }
    	
	    function OpenJobManagement(jobId) {

	        return OpenJob(jobId);
	        //openResizableDialogWithScrollbars('../traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=' + jobId,'600','400');
	    }
    	
	    function ReturnsForInstructionId(show, instructionId)
	    {
    	    
	    }
    	
	    var singleJobId = "<%=m_singleJobId%>";
	    if (singleJobId != "0")
	        OpenJobDetails(singleJobId);
    	
    	
    //-->
    </script>
    
    <script type="text/javascript" language="javascript" defer="defer">
        function pageLoad() {
            $('#<%=txtSearchFor.ClientID %>').focus();        
        }
    </script>
    
    <script type="text/javascript" language="javascript">
            
        //Window Code
        function _showModalDialog(url, width, height, windowTitle)
        {
            MyClientSideAnchor.WindowHeight= height + "px";
            MyClientSideAnchor.WindowWidth= width + "px";
            
            MyClientSideAnchor.URL = url;
            MyClientSideAnchor.Title = windowTitle;
            var returnvalue = MyClientSideAnchor.Open();
            if (returnvalue == true)
            {
                document.all.Form1.submit();
	        }
            return true;	        
        }
        FilterOptionsDisplayHide()    
    </script>

</asp:Content>