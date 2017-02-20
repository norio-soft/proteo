
<%@ Page language="c#" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.Resource.Future" Codebehind="Future.aspx.cs" Title="Haulier Enterprise - Resource Future" %>

<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>


<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Resource Future</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/popAddress.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="/style/styles.css" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div style="width: 100%;">

        <cs:webmodalwindowhelper id="mwhelper" runat="server" showversion="false"></cs:webmodalwindowhelper>    

	    <div id="divPointAddress" style="z-index=5;">
		    <table style="background-color: white; border-width=1px; border-style=solid;" cellpadding="2">
			    <tr>
				    <td><span id="spnPointAddress"></span></td>
			    </tr>
		    </table>
	    </div>

	    <div align="left" id="topPortion">
		    <table width="99%" cellspacing="2" cellpadding="0" border="0">
			    <tr valign="top">
				    <td>
					    This page shows the runs that <asp:Label id="lblResource" runat="server" Font-Bold="True"></asp:Label> is assigned to but has yet to call in.<br>You can click on the run id to view that runs's details.
                                    					
					    <P1:PrettyDataGrid id="dgBasicJobs" runat="server" RowHighlightColor="255, 255, 128" BorderColor="#999999" 
						BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both" 
						AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="False" 
						GroupingColumn="OrganisationName" GroupCountEnabled="True" AllowGrouping="True"  GroupRowColor="#FDA16F" GroupForeColor="Black" 
						AllowCollapsing="True" StartCollapsed="False" width="100%" FixedHeaders="False">
						<SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
						<ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ALTERNATINGITEMSTYLE>
						<ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ITEMSTYLE>
						<HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True"></HEADERSTYLE>
						<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
						<COLUMNS>
							<asp:TemplateColumn HeaderText="Run Id" ItemStyle-VerticalAlign="Top" SortExpression="JobId" HeaderStyle-Wrap="False">
								<ItemTemplate>
									<table width="100%">
										<tr>
											<td colspan="2" align="center">
												<a href="javascript:openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=<%# DataBinder.Eval(Container.DataItem, "JobId") %>'+ getCSID(),'600','400'); title="Manage this job."><%# DataBinder.Eval(Container.DataItem, "JobId") %></a>
												<input type="hidden" id="hidJobId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "JobId") %>' NAME="hidJobId">
											</td>
										</tr>
										<tr>
											<td width="50%"><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;"></td>
											<td width="50%"><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand"></td>
										</tr>
									</table>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn HeaderText="Load" DataField="LoadNo" SortExpression="LoadNo" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="State" DataField="JobState" SortExpression="JobState" ItemStyle-VerticalAlign="Top" Visible="False"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Type" DataField="JobType" SortExpression="JobType" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="My&nbsp;Run" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center">
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
								</ItemTemplate>
							</asp:TemplateColumn>
						</COLUMNS>
						<PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
					</P1:PrettyDataGrid>
				    </td>
			    </tr>
			    <tr><td>&nbsp;</td></tr>
			    <tr>
				    <td align="center">
					    <div id="divScheduleHolder">
						    <OBJECT id="ctSchedule" codeBase="../CAB/ctSchedule.cab" height="90" width="100%" classid="clsid:2D913FCB-9551-4145-B751-7F927D750297" VIEWASTEXT>
							    <PARAM NAME="_Version" VALUE="458752">
							    <PARAM NAME="_ExtentX" VALUE="25770">
							    <PARAM NAME="_ExtentY" VALUE="12965">
							    <PARAM NAME="_StockProps" VALUE="65">
						    </OBJECT>
					    </div>
					    <input type="hidden" runat="server" id="hidResourceScheduleURL" />
				    </td>
			    </tr>
		    </table>
	    </div>

        <div style="height:0px;">
            <object CLASSID="clsid:5220cb21-c88d-11cf-b347-00aa00a28331" >
	            <param name="LPKPath" value="/CAB/ctSchedule7.LPK" />
	        </object>
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
			        divPointAddress.style.top = yMousePos - 50 + "px";
		        }
	        }
                                    	
	        function ShowPlannerRequests(jobId)
	        {
		        window.location.href = '../Traffic/ListRequestsForJob.aspx?wiz=true&jobId=' + jobId;
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
			        divPointAddress.style.top = yMousePos - 50 + "px";
		        }
	        }
                                    	
	        function HidePoint()
	        {
		        if (divPointAddress != null)
			        divPointAddress.style.display = 'none';
	        }

	        function OpenJob(jobId)
	        {
		        if (window.opener != null)
		            window.opener.location.href = '../Job/job.aspx?jobId=' + jobId + getCSID();
		        else
		            window.location.href = '../Job/job.aspx?jobId=' + jobId + getCSID();
                                    			
		        window.close();
	        }

        //-->
        </script>

        <script for="ctSchedule" event="FirstDraw()" language="javascript" type="text/javascript" >
	        var scheduleControl = document.getElementById("ctSchedule");
                                    	
	        if (scheduleControl != null)
	        {
		        scheduleControl.TipsType = 2;
		        scheduleControl.TipsDelay = 0;
	        }
        </script>

    </div>

    <cs:WebModalWindowHelper ID="WebModalWindowHelper1" runat="server" ShowVersion="false"></cs:WebModalWindowHelper>
            
    <div class="buttonbar">
        <asp:Button ID="btnCancel" runat="server" Text="Close" Width="75" CausesValidation="False" />
    </div>

</asp:Content>