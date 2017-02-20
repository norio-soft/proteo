<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.demurragejobs" Codebehind="demurragejobs.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Jobs that have Demurrage" %>
<%@ Reference Page="~/error.aspx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Jobs that have Demurrage</h1></asp:Content> 

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">	
    <h2>A list of jobs that have Demurrage but have not yet been invoiced is shown below.</h2>
    
	<div id="divPointAddress" style="z-index: 5;">
		<table style="background-color: white; border-width: 1px; border-style: solid;" cellpadding="2">
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>

	<P1:PrettyDataGrid id="dgJobs" CssClass="DataGridStyle" runat="server" 
		 CellPadding="6" GridLines="Both" EnableViewState="False"
		AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="False" 
		GroupingColumn="OrganisationName" GroupCountEnabled="True" AllowGrouping="True"  GroupRowColor="whitesmoke" GroupForeColor="Black" 
		AllowCollapsing="True" StartCollapsed="True" width="100%" FixedHeaders="False" RowHighlightingEnabled="False">
		<SELECTEDITEMSTYLE CssClass="DataGridListItemSelected"></SELECTEDITEMSTYLE>
		<ALTERNATINGITEMSTYLE CssClass="DataGridListItemAlt"></ALTERNATINGITEMSTYLE>
		<ITEMSTYLE CssClass="DataGridListItem"></ITEMSTYLE>
		<HEADERSTYLE CssClass="DataGridListHead"></HEADERSTYLE>
		<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
		<COLUMNS>
			<asp:TemplateColumn HeaderText="Job Id" ItemStyle-VerticalAlign="Top" SortExpression="JobId">
				<ItemTemplate>
					<table width="100%">
						<tr>
							<td colspan="2" align="center">
								<a href="javascript:openDialogWithScrollbars('../Traffic/JobManagement.aspx?wiz=true&jobId=<%# DataBinder.Eval(Container.DataItem, "JobId") %>'+ getCSID(),'600','400');" title="Manage this job."><%# DataBinder.Eval(Container.DataItem, "JobId") %></a>
								<input type="hidden" id="hidJobId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "JobId") %>' NAME="hidJobId">
							</td>
						</tr>
						<tr>
							<td width="50%"><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;"></td>
							<td width="50%"><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand"></td>
						</tr>
						<tr>
							<td colspan="2" align="center">
								<a href="" id="lnkEditJob" runat="server">edit&nbsp;job</a>
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn HeaderText="Load" DataField="LoadNo" SortExpression="LoadNo" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
			<asp:BoundColumn HeaderText="State" DataField="JobState" SortExpression="JobState" ItemStyle-VerticalAlign="Top" Visible="False"></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Type" DataField="JobType" SortExpression="JobType" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
			<asp:TemplateColumn HeaderText="My&nbsp;Job" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:CheckBox id="chkOwnership" runat="server" Enabled="False"></asp:CheckBox>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Collect" HeaderStyle-Width="350px" SortExpression="NextCollection" ItemStyle-Width="350px" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:Table id="tblCollections" runat="server" Width="350px" CellSpacing="2"></asp:Table>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Deliver" HeaderStyle-Width="410px" SortExpression="NextDelivery" ItemStyle-Width="410px" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:Table id="tblDeliveries" runat="server" Width="410px" CellSpacing="2"></asp:Table>
					<div style="height:2px;"></div>
					<div align="right">
						<span style="font-weight: bold"><%# DataBinder.Eval(Container.DataItem, "ControlAreaDescription") %></span>
					</div>												
				</ItemTemplate>
			</asp:TemplateColumn>
		</COLUMNS>
		<PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
	</P1:PrettyDataGrid>
	
    <div class="whitespacepusher"></div>
    
	<div class="buttonbar">
		<asp:Button id="btnRefresh" runat="server" Text="Refresh"></asp:Button>
	</div>
	
	<script language="javascript" type="text/javascript">
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
	            xMousePosMax = window.innerWidth + window.pageXOffset;
	            yMousePosMax = window.innerHeight + window.pageYOffset;
	        } else if (document.all) {
	            // When the page scrolls in IE, the event's mouse position 
	            // reflects the position from the top/left of the screen the 
	            // user is looking at. scrollLeft/Top is the amount the user
	            // has scrolled into the page. clientWidth/Height is the height/
	            // width of the current page the user is looking at. So, to be
	            // consistent with Netscape (above), add the scroll offsets to
	            // both so we end up with an absolute value on the page, no 
	            // matter if the user has scrolled or not.
	            xMousePos = window.event.x + document.body.scrollLeft;
	            yMousePos = window.event.y + document.body.scrollTop;
	            xMousePosMax = document.body.clientWidth + document.body.scrollLeft;
	            yMousePosMax = document.body.clientHeight + document.body.scrollTop;
	        } else if (document.getElementById) {
	            // Netscape 6 behaves the same as Netscape 4 in this regard 
	            xMousePos = e.pageX;
	            yMousePos = e.pageY;
	            xMousePosMax = window.innerWidth + window.pageXOffset;
	            yMousePosMax = window.innerHeight + window.pageYOffset;
	        }

	        if (divPointAddress != null && divPointAddress.style.display == '') {
	            divPointAddress.style.display = '';
	            divPointAddress.style.position = "absolute";
	            divPointAddress.style.left = xMousePos + 10 + "px";
	            divPointAddress.style.top = yMousePos + 8 + "px";
	        }
	    }

	    function ShowPoint(url, pointId) {
	        var txtPointAddress = document.getElementById('txtPointAddress');
	        var pageUrl = url + "?pointId=" + pointId;

	        var xmlRequest = new XMLHttpRequest();

	        xmlRequest.open("POST", pageUrl, false);
	        xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	        xmlRequest.send(null);

	        var spnPointAddress = document.getElementById('spnPointAddress');

	        if (spnPointAddress != null && divPointAddress != null) {
	            spnPointAddress.innerHTML = xmlRequest.responseText;
	            divPointAddress.style.display = '';
	            divPointAddress.style.position = "absolute";
	            divPointAddress.style.left = xMousePos + 10 + "px";
	            divPointAddress.style.top = yMousePos + 8 + "px";
	        }
	    }

	    function HidePoint() {
	        if (divPointAddress != null)
	            divPointAddress.style.display = 'none';
	    }
    //-->
    </script>
</asp:Content>