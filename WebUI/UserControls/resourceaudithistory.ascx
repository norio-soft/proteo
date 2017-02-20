<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.resourceAuditHistory" Codebehind="resourceAuditHistory.ascx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<fieldset>
	<legend><strong>Audit History</strong></legend>
		<asp:DataGrid ID="dgAuditHistory" Runat="server" CssClass="DataGridStyle" Width="100%" DataG AutoGenerateColumns="False" AllowSorting="False" AllowPaging="True" PageSize="15" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right">
			<Columns>
				<asp:BoundColumn DataField="Description" HeaderText="Audit Description"></asp:BoundColumn>
				<asp:BoundColumn DataField="UserName" HeaderText="User" ></asp:BoundColumn>
				<asp:BoundColumn DataField="CreateDate" HeaderText="Date" DataFormatString="{0:dd/MM/yy HH:mm}" ></asp:BoundColumn>
			</Columns>
			<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
			<ItemStyle CssClass="DataGridListItem" VerticalAlign="Top"></ItemStyle>
			<HeaderStyle CssClass="DataGridListHead" VerticalAlign="Top"></HeaderStyle>
			<PagerStyle CssClass="DataGridListPagerStyle"></PagerStyle>
		</asp:DataGrid>
</fieldset>