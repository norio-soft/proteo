<%@ Register TagPrefix="cc3" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.ListDeliveryAndReturnsLog" Codebehind="ListDeliveryAndReturnsLog.aspx.cs" %>
<uc1:header id="Header1" runat="server" Title="List Delivery And Returns Log" SubTitle="Produced Delivery and Returns Logs"></uc1:header>
<form id="Form1" method="post" runat="server">
	<fieldset style="TEXT-ALIGN:left">
		<legend><strong>Client</strong></legend>
		<table>
			<tr>
				<td>
					Client:</td>
				<td>
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" ShowMoreResultsBox="true" Width="355px">
                    </telerik:RadComboBox>
				</td>
			</tr>
		</table>
	</fieldset>
	<br>
	<fieldset style="TEXT-ALIGN:left">
		<legend>
			<strong>Date From and To</strong></legend>
		<table>
			<tr>
				<td>
					<P>Date From:
					</P>
				</td>
				<td>
					<telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
			</tr>
			<TR>
				<TD>
					<P>Date To:
					</P>
				</TD>
				<TD>
					<telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput>
				</TD>
			</TR>
		</table>
	</fieldset>
	<table width=100%>
	<tr>
	<td align=center>
	<font color=#ff6600><strong>Logs Are Not Produced For Any Particular Client If There Are Not Any Deliveries Or Returns For The Given Time Frame And Frequency</strong></font>
	</td>
	</tr>
	</table>
	<fieldset style="TEXT-ALIGN:left">
	<legend>
			<strong>Logs Produced But NOT Been Sent</strong></legend>
	<table>
		<tr>
			<td align="center">
				<asp:Label id="lblError" cssclass="ControlErrorMessage" visible="True" runat="server" />
			</td>
		</tr>
	</table>
	<asp:Panel id="pnlLogs" runat="server" Visible="True">
		<asp:datagrid id="dgLogs" runat="server" cssclass="DataGridStyle" ShowFooter="False" AutoGenerateColumns="False"
			AllowSorting="False" AllowPaging="False" pagesize="50" width="100%" cellpadding="2" backcolor="White"
			border="1" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" OnPageIndexChanged="dgLogs_Page">
			<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
			<ItemStyle CssClass="DataGridListItem"></ItemStyle>
			<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
			<Columns>
				<asp:HyperLinkColumn Text="Log Id" DataNavigateUrlField="LogId" DataNavigateUrlFormatString="DeliveryAndReturnsLog.aspx?LogId={0}"
							DataTextField="LogId" SortExpression="LogId" visible="true" HeaderText="Log Id"></asp:HyperLinkColumn>
				<asp:BoundColumn DataField="OrganisationName" HeaderText="Client"></asp:BoundColumn>
				<asp:BoundColumn DataField="LogFrequency" HeaderText="Frequency Of Log"></asp:BoundColumn>
				<asp:BoundColumn DataField="DateTimeFrom" HeaderText="Date & Time From"></asp:BoundColumn>
				<asp:BoundColumn DataField="DateTimeTo" HeaderText="Date & Time To"></asp:BoundColumn>
				<asp:BoundColumn HeaderText="State Of Log"></asp:BoundColumn>
				<asp:BoundColumn DataField="IdentityId" HeaderText="Identity Id" visible="False"></asp:BoundColumn>
			</Columns>
			<PagerStyle HorizontalAlign="Right" Position="Bottom" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
		</asp:datagrid>
	</asp:Panel>
		</fieldset>
	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnGenerate" ServerClick="btnGenerate_Click" runat="server" text="Search Log Reports"></nfvc:NoFormValButton>
		<input type="reset" value="Reset">
	</div>
	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
</form>
<uc1:footer id="Footer1" runat="server"></uc1:footer>
