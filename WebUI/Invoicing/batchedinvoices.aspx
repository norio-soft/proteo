<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.BatchedInvoices" Codebehind="BatchedInvoices.aspx.cs" %>

<uc1:header id=Header1 title="Batched Invoices" runat="server" ShowLeftMenu="false" SubTitle="The Invoice batches summary are below." XMLPath="InvoicingContextMenu.xml"></uc1:header>
<form id=Form1 method=post runat="server">
	<asp:label id=lblNote runat="server" text="" cssclass="confirmation" visible="false"></asp:label>
	
	<asp:panel id="pnlInvoiceBatch" runat="server" Visible="False">
		<asp:datagrid id="dgInvoiceBatch" runat="server" cssclass="DataGridStyle" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="False" pagesize="20" cellpadding="2" backcolor="White" border="1" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" OnPageIndexChanged="dgInvoiceBatch_Page" width="100%" ShowFooter="True">
			<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
			<ItemStyle CssClass="DataGridListItem"></ItemStyle>
			<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
			<Columns>
				<asp:BoundColumn DataField="BatchId" HeaderText="Batch Id"></asp:BoundColumn>
				<asp:BoundColumn DataField="JobIds" HeaderText="Job Ids"></asp:BoundColumn>
				<asp:TemplateColumn HeaderText="Customer" Visible="True">
					<ItemTemplate>
						<asp:Label id="lblCustomerName" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrganisationName") %>'></asp:Label>
						<input type="hidden" id="hidIdentityId" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "IdentityId")%>' NAME="hidIdentityId">
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn DataField="ChargeAmount" DataFormatString="{0:C}" HeaderText="Amount">
					<HeaderStyle HorizontalAlign="Right"></HeaderStyle>
					<ItemStyle HorizontalAlign="Right"></ItemStyle>
				</asp:BoundColumn>
				<asp:ButtonColumn Text="View" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="" CommandName="View"></asp:ButtonColumn>
				<asp:ButtonColumn Text="Create" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" HeaderText="" CommandName="Create" Visible="False"></asp:ButtonColumn>
			</Columns>
			<PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
		</asp:datagrid>
	</asp:panel>

</FORM> 

<uc1:footer id="Footer1" runat="server"></uc1:footer>
