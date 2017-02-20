<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.DeletedResources" Codebehind="DeletedResources.aspx.cs" MasterPageFile="~/default_tableless.master"   Title="Deleted Resources" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Deleted Resources</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<h2>A list of deleted resources is shown below.</h2>

	<div align="left" id="topPortion">
		<table width="99%" cellspacing="2" cellpadding="0" border="0">
			<tr valign="top">
				<td>
					<P1:PrettyDataGrid id="dgResources" runat="server" RowHighlightColor="255, 255, 128" CssClass="DataGridStyle" GridLines="Both" 
						AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="False" 
						GroupingColumn="ResourceType" GroupCountEnabled="True" AllowGrouping="True" 
						AllowCollapsing="True" StartCollapsed="False" width="100%" FixedHeaders="False">
						<SELECTEDITEMSTYLE CssClass="DataGridListItemSelected"></SELECTEDITEMSTYLE>
						<ALTERNATINGITEMSTYLE CssClass="DataGridListItemAlt"></ALTERNATINGITEMSTYLE>
						<ITEMSTYLE CssClass="DataGridListItem"></ITEMSTYLE>
						<HEADERSTYLE CssClass="DataGridListHead"></HEADERSTYLE>
						<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
						<COLUMNS>
							<asp:BoundColumn HeaderText="Resource Description" DataField="ResourceDescription" ItemStyle-VerticalAlign="Top" SortExpression="ResourceDescription" HeaderStyle-Wrap="False"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="ResourceId"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="ResourceTypeId"></asp:BoundColumn>
							<asp:ButtonColumn HeaderText="Restore" ButtonType="PushButton" Text="Restore" CommandName="Restore"></asp:ButtonColumn>
						</COLUMNS>
						<PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
					</P1:PrettyDataGrid>
				</td>
			</tr>
		</table>
	</div>

</asp:Content>