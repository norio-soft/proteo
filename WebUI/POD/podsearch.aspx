<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register Assembly="RadCombobox.Net2" Namespace="Telerik.Web.UI" TagPrefix="radC" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.POD.PODSearch" Codebehind="PODSearch.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<uc1:header id="Header1" title="Customer POD Queries" runat="server" SubTitle="Search for a POD below."></uc1:header>
    <form id="Form1" method="post" runat="server">
			<fieldset>
			<legend><strong>POD Search Criteria<strong></legend>
			<table>
				<tr>
					<td>Client</td>
					<td>
                        <radC:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                            ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                        </radC:RadComboBox>
						
					</td>
				</tr>
				<tr>
					<td>Ticket number</td>
					<td><asp:TextBox id="txtTicketNo" runat="server"></asp:TextBox></td>
				</tr>
				<tr>
					<td>Signature date</td>
					<td><telerik:RadDateInput id="dteSignatureDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
				</tr>			
			</table>	
			</fieldset>
			<br>
			<fieldset>
				<legend><strong>Job Search Criteria</strong></legend>
				<table>
					<tr>
						<td>Load Number</td>
						<td><asp:TextBox id="txtLoadNo" runat="server"></asp:TextBox>
					</tr>
					<tr>
						<td>Customer Reference</td>
						<td><asp:TextBox id="txtCustomerRef" runat="server"></asp:TextBox>
					</tr>
					<tr>
						<td>Destination</td>
						<td><asp:TextBox id="txtDestination" runat="server"></asp:TextBox>
					</tr>
				</table>
			</fieldset>
		<div class="ButtonBar">
			<asp:DataGrid id="dgPODSearchResults" runat="server" AllowSorting="True" cssclass="DataGridStyle" AutoGenerateColumns="False"
						  cellpadding="2" backcolor="White" border="1" AllowPaging="True" PagerStyle-Mode="NumericPages"
						  pagesize="20" OnSortCommand="dgPODSearchResults_SortCommand" OnItemDataBound="dgPODSearchResults_ItemDataBound" OnItemCommand="dgPODSearchResults_ItemCommand" OnPageIndexChanged="dgPODSearchResults_Page">
				<ItemStyle CssClass="DataGridListItem"></ItemStyle>
				<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="PODId" SortExpression="PODId" HeaderText="POD ID" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="TicketNo" SortExpression="TicketNo" HeaderText="Ticket No"></asp:BoundColumn>
					<asp:BoundColumn DataField="SignatureDate" SortExpression="SignatureDate" HeaderText="Signature Date" DataFormatString="{0:dd/MM/yy}"></asp:BoundColumn>
					<asp:BoundColumn DataField="OrganisationName" SortExpression="OrganisationName" HeaderText="Client"></asp:BoundColumn>
					<asp:BoundColumn DataField="LoadNo" SortExpression="LoadNo" HeaderText="Load Number" runat="server"/>
					<asp:BoundColumn DataField="Destination" SortExpression="Destination" HeaderText="Destination" runat="server"/>
					<asp:TemplateColumn HeaderText="View">
						<ItemTemplate>
							<asp:HyperLink id="lnkViewPOD" Text="View" runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:ButtonColumn HeaderText="Download" CommandName="Download" Text="Download" ButtonType="PushButton"></asp:ButtonColumn>
					<asp:TemplateColumn HeaderText="Email/Fax">
						<ItemTemplate>
							<asp:CheckBox id="chkEmailPOD" runat="server" />
							</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn DataField="ScannedFormId" Visible="False"></asp:BoundColumn>
				</Columns>
			</asp:DataGrid>
		</div>	
		<div class="ButtonBar" align="left">
			<nfvc:NoFormValButton id="btnSearch" runat="server" text="Filter"></nfvc:NoFormValButton>
			<nfvc:NoFormValButton id="btnEmailFax" runat="server" text="Email/Fax Selected"></nfvc:NoFormValButton>
		</div>
		<uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
	</form>
<script language="javascript" type="text/javascript">
<!--
	function OpenFormViewer(scannedFormId)
	{
		openResizableDialogWithScrollbars('../scan/viewer.aspx?UseLocal=0&ScannedFormId=' + scannedFormId + '&PageNumber=0&TYPEID=POD', 505, 488, null);
	}
	fucntion viewPOD(scannedFormId)
	{
	    openResizableDialogWithScrollbars('podviewer.aspx?ScannedFormId=' + scannedFormId + '&PageNumber=0', 505, 488, null);
	}
//-->
</script>
<uc1:footer id="Footer1" runat="server"></uc1:footer>