<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.tabReturns" Codebehind="tabReturns.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>
<%@ Register TagPrefix="uc1" TagName="callInTabStrip" Src="~/UserControls/DriverCallInTabStrip.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Returns</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <link rel="stylesheet" type="text/css" href="/style/styles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newMasterpage.css" />

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/popaddress.js" type="text/javascript"></script>
    
    <script language="javascript" type="text/javascript">
	<!--
	    //moveTo((screen.width - 1220) / 2, (screen.height - 870) / 2);
		//resizeTo(1220, 870);
		window.focus();
		
		var returnUrlFromPopUp = window.location;
	//-->
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">      
                    <div runat="server" id="buttonBar" class="buttonbar" style="text-align:left;">
                        <table width="99%" border="0" cellpadding="0" cellspacing="2">
							<tr>
								<td><input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>    '+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px; display: <%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>" value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td width="100%" align="right"><iframe marginheight="0" marginwidth="0" frameborder="0" scrolling="no" width="360px" height="22px" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'></iframe></td>
							</tr>
						</table>
                    </div>
                    
                    <uc1:callInTabStrip id="CallInTabStrip1" runat="server" SelectedTab="0"></uc1:callInTabStrip>
                    <div style="padding-bottom:10px;"></div>
                    <asp:Panel id="pnlHasInstructionGoods" runat="server">
                        <fieldset>
                            <asp:Label id="lblGoodsCount" runat="server"></asp:Label>
                        </fieldset>
                        <asp:datagrid id="dgGoods" runat="server" CssClass="DataGridStyle" Width="100%" AutoGenerateColumns="False">
                            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
                            <ItemStyle CssClass="DataGridListItem" VerticalAlign="Top"></ItemStyle>
                            <HeaderStyle CssClass="DataGridListHead" VerticalAlign="Top"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn HeaderText="Goods ID">
                                    <ItemTemplate>
                                        <a runat="server" id="hypRefusal"></a>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="RefusalId" HeaderText="Refusal Id" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn DataField="DeliveryOrderNumber" HeaderText="Reference"></asp:BoundColumn>
                                <asp:BoundColumn DataField="RefusalType" HeaderText="Reason Type"></asp:BoundColumn>
                                <asp:BoundColumn DataField="QuantityRefused" HeaderText="Quantity"></asp:BoundColumn>
                                <asp:BoundColumn DataField="RefusalStatus" HeaderText="Status"></asp:BoundColumn>
                                <asp:BoundColumn DataField="RefusalLocation" HeaderText="Location"></asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Store At">
	                                <ItemTemplate>
		                                <asp:Label id="lblStoreAt" runat="server"></asp:Label>
	                                </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Return to">
	                                <ItemTemplate>
		                                <asp:Label id="lblReturnTo" runat="server"></asp:Label>
	                                </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Return on">
                                    <ItemTemplate>
                                        <a href="javascript:OpenJobDetails(<%# DataBinder.Eval(Container.DataItem, "ReturnJobId") %>);"><%# DataBinder.Eval(Container.DataItem, "ReturnJobId") %></a>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:BoundColumn DataField="TimeFrame" HeaderText="Return By" DataFormatString="{0:dd/MM/yy}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="ProductName" HeaderText="Product Name"></asp:BoundColumn>
                                <asp:ButtonColumn Text="Delete" ButtonType="PushButton" CommandName="delete"></asp:ButtonColumn>
                            </Columns>
                            <PagerStyle HorizontalAlign="Right" PageButtonCount="2" CssClass="DataGridListPagerStyle"></PagerStyle>
                        </asp:datagrid>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>

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
    	
	    function OpenJobDetails(jobId)
	    {
	        openDialogWithScrollbars('~/job/job.aspx?wiz=true&jobId=' + jobId+ getCSID(),'600','400');
	    }
    //-->
    </script>
</asp:Content>