<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Trailer.AvailableTrailers" Codebehind="AvailableTrailers.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" runat="server" Title="Available Trailer List" SubTitle="A list of available trailers is shown below." ></uc1:header>
	
<form id="Form1" method="post" runat="server">
	<fieldset>
		<legend>Available Trailers</legend>
		<table width="300" border="0" cellpadding="1" cellspacing="0">
			<tr>
				<td width="75" valign="top"><b>Start Date</b></td>
				<td width="105"><telerik:RadDateInput id="dteStartDate" runat="server" dateFormat="dd/MM/yy"></telerik:RadDateInput></td>
				<td>
					<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify the start date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify the start date."></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" EnableClientScript="False" ControlToValidate="dteStartDate" ErrorMessage="The start date must occur before the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="The start date must occur before the end date."></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td valign="top"><b>End Date</b></td>
				<td><telerik:RadDateInput id="dteEndDate" runat="server" dateFormat="dd/MM/yy"></telerik:RadDateInput></td>
				<td>
					<asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify the end date."></asp:RequiredFieldValidator>
				</td>
			</tr>
		</table>
		<div class="buttonbar">
			<asp:Button id="btnFilter" runat="server" Text="Filter"></asp:Button>
		</div>
	</fieldset>
	<asp:datagrid id="dgTrailers" runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True"
		pagesize="20" width="100%" cellpadding="2" backcolor="white" border="1" cssclass="DataGridStyle"
		PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right">
		<Columns>
			<asp:TemplateColumn HeaderText="Trailer Ref" SortExpression="TrailerRef">
				<ItemTemplate>
					<a href='addupdatetrailer.aspx?resourceId=<%# DataBinder.Eval(Container.DataItem,"ResourceId")%>'>
						<%#DataBinder.Eval(Container.DataItem, "TrailerRef")%>
					</a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="Manufacturer" HeaderText="Manufacturer" Visible="False" SortExpression="Manufacturer"></asp:BoundColumn>
			<asp:BoundColumn DataField="TrailerType" HeaderText="Trailer Type" Visible="False" SortExpression="TrailerType"></asp:BoundColumn>
			<asp:BoundColumn DataField="TrailerDescription" HeaderText="Trailer Description" SortExpression="TrailerDescription"></asp:BoundColumn>
			<asp:BoundColumn DataField="CurrentLocation" HeaderText="Current Location" SortExpression="CurrentLocation"></asp:BoundColumn>
		</Columns>
		<pagerstyle cssclass="DataGridListPagerStyle"></pagerstyle>
		<itemstyle cssclass="DataGridListItem"></itemstyle>
		<headerstyle cssclass="DataGridListHead"></headerstyle>
		<alternatingitemstyle cssclass="DataGridListItemAlt"></alternatingitemstyle>
	</asp:datagrid>
</form>
	
<uc1:footer id="footer1" runat="server"></uc1:footer>
