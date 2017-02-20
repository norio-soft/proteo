<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Vehicle.AvailableVehicles" Codebehind="AvailableVehicles.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" runat="server" Title="Available Vehicle List" SubTitle="A list of available vehicles is shown below." ></uc1:header>
	
<form id="Form1" method="post" runat="server">
	<fieldset>
		<legend><Strong>Available Vehicles</Strong></legend>
		<table width="300" border="0" cellpadding="1" cellspacing="0">
			<tr>
				<td width="75" valign="top">Start Date</td>
				<td width="105"><telerik:RadDateInput id="dteStartDate" runat="server" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
				<td>
					<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify the start date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify the start date."></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" EnableClientScript="False" ControlToValidate="dteStartDate" ErrorMessage="The start date must occur before the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="The start date must occur before the end date."></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td valign="top">End Date</td>
				<td><telerik:RadDateInput id="dteEndDate" runat="server" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
				<td>
					<asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify the end date."></asp:RequiredFieldValidator>
				</td>
			</tr>
		</table>
		<div class="buttonbar">
			<asp:Button id="btnFilter" runat="server" Text="Filter"></asp:Button>
		</div>
	</fieldset>
	<asp:datagrid id="dgVehicles" runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True"
		pagesize="20" width="100%" cellpadding="2" backcolor="White" border="1" cssclass="DataGridStyle"
		PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right">
		<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
		<ItemStyle CssClass="DataGridListItem"></ItemStyle>
		<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
		<Columns>
			<asp:TemplateColumn SortExpression="RegNo" HeaderText="Registration">
				<ItemTemplate>
					<a href='addupdatevehicle.aspx?resourceId=<%# DataBinder.Eval(Container.DataItem,"ResourceId")%>'>
						<%#DataBinder.Eval(Container.DataItem, "RegNo")%>
					</a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="RegularDriver" SortExpression="RegularDriver" HeaderText="Regular Driver"></asp:BoundColumn>
			<asp:BoundColumn DataField="VehicleManufacturer" SortExpression="VehicleManufacturer" HeaderText="Make"></asp:BoundColumn>
			<asp:BoundColumn DataField="ChassisNo" SortExpression="ChassisNo" HeaderText="ChassisNo"></asp:BoundColumn>
			<asp:BoundColumn DataField="MOTExpiry" SortExpression="MOTExpiry" HeaderText="MOT Expires On" DataFormatString="{0:dd MMMM yyyy}"></asp:BoundColumn>
			<asp:BoundColumn DataField="TelephoneNumber" SortExpression="TelephoneNumber" HeaderText="Cab Phone"></asp:BoundColumn>
			<asp:BoundColumn DataField="CurrentVehicleController" SortExpression="CurrentVehicleController" HeaderText="Current Controller"></asp:BoundColumn>
			<asp:BoundColumn DataField="CurrentLocation" SortExpression="CurrentLocation" HeaderText="Current Location"></asp:BoundColumn>
		</Columns>
		<PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
	</asp:datagrid>
</form>	
<uc1:footer id="footer1" runat="server"></uc1:footer>