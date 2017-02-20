<%@ Page language="c#" Inherits="Orchestrator.WebUI.Point.viewrates" MasterPageFile="~/default_tableless.master" Codebehind="viewrates.aspx.cs" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>Job Rates</h1>
    <h2>The rates are displayed below</h2>
	<div id="divPointAddress" style="z-index=5;">
		<table>
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>
	<table>
		<tr valign="top">
			<td>
				<fieldset>
					<legend>Collection Point</legend>
					<table>
						<tr>
							<td class="formCellLabel">Client</td>
							<td class="formCellInput">
                                <telerik:RadComboBox ID="cboCollectionClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" OnClientSelectedIndexChanged="CollectionClientSelectedIndexChanged" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Width="355px">
                                </telerik:RadComboBox>
								
							</td>
						</tr>
						<tr>
							<td class="formCellLabel">Point</td>
							<td class="formCellInput">
                                <telerik:RadComboBox ID="cboCollectionPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" OnClientItemsRequesting="CollectionPointRequesting"
                                    ShowMoreResultsBox="true" Width="355px" AutoPostBack="true">
                                </telerik:RadComboBox>
							</td>
						</tr>
						<tr>
							<td class="formCellInput" colspan="2" align="right">
								<asp:Button id="btnClearCollectionPoint" CssClass="buttonClass" runat="server" Text="Clear"></asp:Button>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
			<td width="50%">
				<fieldset>
					<legend>Delivery Point</legend>
					<table>
						<tr>
							<td class="formCellLabel">Client</td>
							<td class="formCellInput">
							
                                <telerik:RadComboBox ID="cboDeliveryClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" OnClientSelectedIndexChanged="DeliveryClientSelectedIndexChanged" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Width="355px">
                                </telerik:RadComboBox>
								
							</td>
						</tr>
						
						<tr>
							<td class="formCellLabel">Point</td>
							<td class="formCellInput">
                                <telerik:RadComboBox ID="cboDeliveryPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" OnClientItemsRequesting="DeliveryPointRequesting"
                                    ShowMoreResultsBox="true" Width="355px" AutoPostBack="true">
                                </telerik:RadComboBox>
								
							</td>
						</tr>
						<tr>
							<td class="formCellInput" colspan="2" align="right">
								<asp:Button id="btnClearDeliveryPoint" CssClass="buttonClass" runat="server" Text="Clear"></asp:Button>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
		<tr>
			<td>
				<fieldset>
					<legend>Client</legend>
					<table>
					    <tr>
					        <td class="formCellLabel">Client</td>
					        <td class="formCellInput">
					            <telerik:RadComboBox ID="cboJobClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Width="355px" AutoPostBack="true">
                                </telerik:RadComboBox>
                            </td>
					    </tr>
					</table>
                    
					<div align="right">
						<asp:Button id="btnClearJobClient" CssClass="buttonClass" runat="server" Text="Clear"></asp:Button>
					</div>
				</fieldset>
			</td>
		</tr>
    </table>
    <P1:PrettyDataGrid id="dgRates" CssClass="DataGridStyle" runat="server" GridLines="Both" 
    AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="True" RowSelectionEnabled="False" 
    GroupCountEnabled="True" AllowGrouping="False" RowHighlightingEnabled="False" 
    AllowCollapsing="True" StartCollapsed="False" width="100%" FixedHeaders="False" AllowPaging="False">
        <SELECTEDITEMSTYLE CssClass="DataGridListItemSelected"></SELECTEDITEMSTYLE>
        <ALTERNATINGITEMSTYLE  CssClass="DataGridListItemAlt" BackColor="#FFFFFF"></ALTERNATINGITEMSTYLE>
        <ITEMSTYLE CssClass="DataGridListItem"></ITEMSTYLE>
        <HEADERSTYLE CssClass="DataGridListHead"></HEADERSTYLE>
        <FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
        <COLUMNS>
            <asp:BoundColumn HeaderText="RateId" DataField="JobRateId" Visible="False"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="Client" DataField="OrganisationName" SortExpression="OrganisationName"></asp:BoundColumn>
            <asp:TemplateColumn HeaderText="Collection Point">
                <ItemTemplate>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="Delivery Point">
                <ItemTemplate>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn HeaderText="Full Load Rate" DataField="FullLoadRate" DataFormatString="{0:C}" SortExpression="FullLoadRate"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="Part Load Rate" DataField="PartLoadRate" DataFormatString="{0:C}" SortExpression="PartLoadRate"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="Multi Drop Rate" DataField="MultiDropRate" DataFormatString="{0:C}" SortExpression="MultiDropRate"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="Start Date" DataField="StartDate" DataFormatString="{0:dd/MM/yy HH:mm}" SortExpression="StartDate"></asp:BoundColumn>
            <asp:BoundColumn HeaderText="End Date" DataField="EndDate" DataFormatString="{0:dd/MM/yy HH:mm}" SortExpression="EndDate"></asp:BoundColumn>
            <asp:ButtonColumn CommandName="End" ButtonType="PushButton" Text="End Rate" ItemStyle-HorizontalAlign="Center"></asp:ButtonColumn>
        </COLUMNS>
    </P1:PrettyDataGrid>
    <div align="right">
    <asp:CheckBox id="chkShowEnded" runat="server" Text="Show Rates that have Ended" Text-Align="Right" AutoPostBack="True"></asp:CheckBox>
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

 <script language="javascript">
     function CollectionClientSelectedIndexChanged(item) {
         var pointCombo = $find("<%=cboCollectionPoint.ClientID %>");
         pointCombo.set_text("");
         pointCombo.requestItems(item.get_value(), false);
     }

     function CollectionPointRequesting(sender, eventArgs) {
         var collectionClientCombo = $find("<%=cboCollectionClient.ClientID %>");

         var context = eventArgs.get_context();
         context["FilterString"] = collectionClientCombo.get_value() + ";" + sender.get_text();
     }

     function DeliveryClientSelectedIndexChanged(item) {
         var pointCombo = $find("<%=cboDeliveryPoint.ClientID %>");
         pointCombo.set_text("");
         pointCombo.requestItems(item.get_value(), false);
     }

     function DeliveryPointRequesting(sender, eventArgs) {
         var deliveryClientCombo = $find("<%=cboDeliveryClient.ClientID %>");

         var context = eventArgs.get_context();
         context["FilterString"] = deliveryClientCombo.get_value() + ";" + sender.get_text();
     } 
    </script>
</asp:Content>