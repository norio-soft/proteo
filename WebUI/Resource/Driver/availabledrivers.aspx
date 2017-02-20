<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.AvailableDrivers" Codebehind="AvailableDrivers.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" runat="server" Title="Available Driver List" SubTitle="A list of available drivers is shown below." ></uc1:header>

<form id="Form1" method="post" runat="server">
	<fieldset>
		<legend><strong>Available Drivers</strong></legend>
		<table width="300" border="0" cellpadding="1" cellspacing="0">
			<tr>
				<td width="75" valign="top">Start Date</td>
				<td width="105"><telerik:RaddateInput id="dteStartDate" runat="server" dateFormat="dd/MM/yy"></telerik:RaddateInput></td>
				<td>
					<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify the start date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify the start date."></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" EnableClientScript="False" ControlToValidate="dteStartDate" ErrorMessage="The start date must occur before the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="The start date must occur before the end date."></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td valign="top">End Date</td>
				<td><telerik:RaddateInput id="dteEndDate" runat="server" DateFormat="dd/MM/yy"></telerik:RaddateInput></td>
				<td>
					<asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify the end date."></asp:RequiredFieldValidator>
				</td>
			</tr>
		</table>
		<div class="buttonbar">
			<asp:Button id="btnFilter" runat="server" Text="Filter"></asp:Button>
		</div>
	</fieldset>
	<asp:datagrid id="dgDrivers" runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True"
		pagesize="20" width="100%" cellpadding="2" backcolor="white" border="1" cssclass="DataGridStyle"
		PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right">
		<columns>
			<asp:TemplateColumn HeaderText="Full Name" SortExpression="FullName">
				<itemtemplate>
					<a href='addupdatedriver.aspx?identityId=<%# DataBinder.Eval(Container.DataItem,"IdentityId")%>'>
						<%#DataBinder.Eval(Container.DataItem, "FullName")%>
					</a>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="MobilePhone" HeaderText="Mobile No" SortExpression="MobilePhone"></asp:BoundColumn>
			<asp:BoundColumn DataField="HomePhone" HeaderText="Home Phone" SortExpression="HomePhone"></asp:BoundColumn>
			<asp:BoundColumn DataField="PersonalMobile" HeaderText="Personal Mobile" SortExpression="PersonalMobile"></asp:BoundColumn>
			<asp:BoundColumn DataField="AddressLine1" HeaderText="Address" SortExpression="AddressLine1"></asp:BoundColumn>
			<asp:boundcolumn datafield="PostCode" headertext="Post Code" sortexpression="PostCode"></asp:boundcolumn>
			<asp:BoundColumn DataField="CurrentDriverController" SortExpression="CurrentDriverController" HeaderText="Current Controller"></asp:BoundColumn>
			<asp:BoundColumn DataField="CurrentLocation" SortExpression="CurrentLocation" HeaderText="Current Location"></asp:BoundColumn>
		</Columns>
		<pagerstyle cssclass="DataGridListPagerStyle"></pagerstyle>
		<itemstyle cssclass="DataGridListItem"></itemstyle>
		<headerstyle cssclass="DataGridListHead"></headerstyle>
		<alternatingitemstyle cssclass="DataGridListItemAlt"></alternatingitemstyle>
	</asp:datagrid>
</form>
	
<uc1:footer id="footer1" runat="server"></uc1:footer>