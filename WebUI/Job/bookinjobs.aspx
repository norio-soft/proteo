<%@ Reference Page="~/error.aspx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.bookinjobs" Codebehind="bookinjobs.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Jobs that can be Booked In" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<asp:content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
    .RedRow 
    { 
      background-color: #F22FFF; 
      cursor: default;
    }

    .RedRow td.DataCell 
    { 
      padding: 3px; 
      padding-top: 2px; 
      padding-bottom: 1px; 
      border-bottom: 1px solid #EAE9E1; 
      font-family: verdana; 
      font-size: 10px;
      color: red;
    } 
    </style>

    <h1>Jobs that can be Booked In</h1>
    <h2>A list of jobs that can be booked in is shown below.</h2>
	<div id="divPointAddress" style="z-index: 5;">
		<table style="background-color: white; border-width: 1px; border-style: solid;" cellpadding="2">
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>

    <ComponentArt:Grid id="dgJobsToBookIn" 
	    EnableViewState="False"
	    RunningMode="Server" 
	    CssClass="Grid" 
	    AllowPaging="False"
	    AllowSorting="False"
	    ShowHeader="False"
	    ShowFooter="False"
	    GroupBy="OrganisationName ASC"
	    GroupByCssClass="GroupByCell"
	    GroupByTextCssClass="GroupByText"
	    Sort="JobId"
	    PreExpandOnGroup="False"
	    ImagesBaseUrl="~/images/" 
	    PagerImagesFolderUrl="~/images/pager/"
	    TreeLineImagesFolderUrl="~/images/lines/" 
	    TreeLineImageWidth="22" 
	    TreeLineImageHeight="19" 
	    IndentCellWidth="22" 
	    GroupingNotificationTextCssClass="GridHeaderText"
	    GroupBySortAscendingImageUrl="group_asc.gif"
	    GroupBySortDescendingImageUrl="group_desc.gif"
	    GroupBySortImageWidth="10"
	    GroupBySortImageHeight="10"
	    LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
	    LoadingPanelPosition="MiddleCenter"
	    Width="100%" runat="server"
    	
	    >
	    <Levels>
		    <ComponentArt:GridLevel
			    DataKeyField="JobId"
			    ShowTableHeading="false" 
			    ShowSelectorCells="false" 
			    RowCssClass="Row" 
			    ColumnReorderIndicatorImageUrl="reorder.gif"
			    DataCellCssClass="DataCell" 
			    HeadingCellCssClass="HeadingCell" 
			    HeadingCellHoverCssClass="HeadingCellHover" 
			    HeadingCellActiveCssClass="HeadingCellActive" 
			    HeadingRowCssClass="HeadingRow" 
			    HeadingTextCssClass="HeadingCellText"
			    SelectedRowCssClass="SelectedRow"
			    GroupHeadingCssClass="GroupHeading" 
			    SortAscendingImageUrl="asc.gif" 
			    SortDescendingImageUrl="desc.gif" 
			    SortImageWidth="10" 
			    SortImageHeight="19">
			      <ConditionalFormats>
				    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobId').Value=='2294'" RowCssClass="RedRow" />
			    </ConditionalFormats>


			    <Columns>
				    <ComponentArt:GridColumn DataField="JobId" Align="Left" DataCellServerTemplateId="JobIdTemplate" HeadingCellCssClass="FirstHeadingCell" DataCellCssClass="FirstDataCell" AllowGrouping="False" AllowSorting="False" HeadingText="Job Id" Width="100" FixedWidth="True" />
				    <ComponentArt:GridColumn DataField="LoadNo" Align="Left" HeadingText="Load Number" Width="150" FixedWidth="True" />
				    <ComponentArt:GridColumn DataField="JobType" Align="Left" HeadingText="Job Type" Width="150" FixedWidth="True" />
				    <ComponentArt:GridColumn DataField="JobId" Align="Left" DataCellServerTemplateId="CollectionDetailsTemplate" HeadingText="Collection Details" Width="350" FixedWidth="True" />
				    <ComponentArt:GridColumn DataField="JobId" Align="Left" DataCellServerTemplateId="DeliveryDetailsTemplate" HeadingText="Delivery Details" Width="410" FixedWidth="True" />
				    <ComponentArt:GridColumn DataField="OrganisationName" Visible="False" />
			    </Columns>
		    </ComponentArt:GridLevel>
	    </Levels>
	    <ServerTemplates>
		    <ComponentArt:GridServerTemplate id="JobIdTemplate">
			    <Template>
				    <table width="100%">
					    <tr>
						    <td colspan="2" align="center">
							    <a href="javascript:openDialogWithScrollbars('../Traffic/JobManagement.aspx?wiz=true&jobId=<%# Container.DataItem["JobId"] %>'+ getCSID(),'600','400');" title="Manage this job."><%# Container.DataItem["JobId"] %></a>
							    <input type="hidden" id="hidJobId" runat="server" value='<%# Container.DataItem["JobId"] %>' NAME="hidJobId" />
							    <input type="hidden" id="hidJobState" runat="server" value='<%# Container.DataItem["JobId"] %>' NAME="hidJobState" />
							    <input type="hidden" id="hidJobType" runat="server" value='<%# Container.DataItem["JobId"] %>' NAME="hidJobType" />
							    <input type="hidden" id="hidHasRequests" runat="server" value='<%# Container.DataItem["JobId"] %>' NAME="hidHasRequests" />
						    </td>
					    </tr>
					    <tr>
						    <td width="50%"><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;" /></td>
						    <td width="50%"><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand" /></td>
					    </tr>
					    <tr>
						    <td colspan="2" align="center">
							    <a href="" id="lnkEditJob" runat="server">edit&nbsp;job</a>
						    </td>
					    </tr>
				    </table>
			    </Template>
		    </ComponentArt:GridServerTemplate>
    		
		    <ComponentArt:GridServerTemplate id="CollectionDetailsTemplate">
			    <Template>
				    <asp:Table id="tblCollections" runat="server" Width="350px" CellSpacing="2"></asp:Table>
				    <input type="hidden" id="hidJobId" runat="server" value='<%# Container.DataItem["JobId"] %>' NAME="hidJobId">
			    </Template>
		    </ComponentArt:GridServerTemplate>

		    <ComponentArt:GridServerTemplate id="DeliveryDetailsTemplate">
			    <Template>
				    <asp:Table id="tblDeliveries" runat="server" Width="410px" CellSpacing="2"></asp:Table>
				    <div style="height:2px;"></div>
				    <div align="right">
					    <span style="font-weight: bold"></span>
				    </div>												
				    <input type="hidden" id="hidJobId" runat="server" value='<%# Container.DataItem["JobId"] %>' NAME="hidJobId">
			    </Template>
		    </ComponentArt:GridServerTemplate>
	    </ServerTemplates>
    </ComponentArt:Grid>
    
    <div class="whitepacepusher"></div>

    <div class="buttonBar">
	    <asp:Button id="btnRefresh" runat="server" Text="Refresh"></asp:Button>
    </div>

<script language="javascript">
<!--
	// Set Netscape up to run the "captureMousePosition" function whenever
	// the mouse is moved. For Internet Explorer and Netscape 6, you can capture
	// the movement a little easier.
	if (document.layers) { // Netscape
		document.captureEvents(Event.MOUSEMOVE);
		document.onmousemove = captureMousePosition;
	} else if (document.all) { // Internet Explorer
		document.onmousemove = captureMousePosition;
	} else if (document.getElementById) { // Netcsape 6
		document.onmousemove = captureMousePosition;
	}
	// Global variables
	xMousePos = 0; // Horizontal position of the mouse on the screen
	yMousePos = 0; // Vertical position of the mouse on the screen
	xMousePosMax = 0; // Width of the page
	yMousePosMax = 0; // Height of the page
	var divPointAddress = document.getElementById('divPointAddress');
	if (divPointAddress != null)
		divPointAddress.style.display = 'none';

	function captureMousePosition(e) {
		if (document.layers) {
			// When the page scrolls in Netscape, the event's mouse position
			// reflects the absolute position on the screen. innerHight/Width
			// is the position from the top/left of the screen that the user is
			// looking at. pageX/YOffset is the amount that the user has 
			// scrolled into the page. So the values will be in relation to
			// each other as the total offsets into the page, no matter if
			// the user has scrolled or not.
			xMousePos = e.pageX;
			yMousePos = e.pageY;
			xMousePosMax = window.innerWidth+window.pageXOffset;
			yMousePosMax = window.innerHeight+window.pageYOffset;
		} else if (document.all) {
			// When the page scrolls in IE, the event's mouse position 
			// reflects the position from the top/left of the screen the 
			// user is looking at. scrollLeft/Top is the amount the user
			// has scrolled into the page. clientWidth/Height is the height/
			// width of the current page the user is looking at. So, to be
			// consistent with Netscape (above), add the scroll offsets to
			// both so we end up with an absolute value on the page, no 
			// matter if the user has scrolled or not.
			xMousePos = window.event.x+document.body.scrollLeft;
			yMousePos = window.event.y+document.body.scrollTop;
			xMousePosMax = document.body.clientWidth+document.body.scrollLeft;
			yMousePosMax = document.body.clientHeight+document.body.scrollTop;
		} else if (document.getElementById) {
			// Netscape 6 behaves the same as Netscape 4 in this regard 
			xMousePos = e.pageX;
			yMousePos = e.pageY;
			xMousePosMax = window.innerWidth+window.pageXOffset;
			yMousePosMax = window.innerHeight+window.pageYOffset;
		}

		if (divPointAddress != null && divPointAddress.style.display == '')
		{
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
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
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
	}

	function HidePoint()
	{
		if (divPointAddress != null)
			divPointAddress.style.display = 'none';
	}
//-->
</script>
	
</asp:content>