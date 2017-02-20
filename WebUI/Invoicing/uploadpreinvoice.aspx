<%@ Reference Page="~/error.aspx" %>

<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.UploadPreInvoice" Codebehind="uploadpreinvoice.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text" %>
<uc1:header id="Header1" title="Upload Pre Invoice" runat="server" XMLPath="InvoicingContextMenu.xml"
	SubTitle="The Upload Pre Invoice Preparation summary is below." ShowLeftMenu="false"></uc1:header>
<FORM id="Form1" method="post" encType="multipart/form-data" runat="server">
<asp:label id="lblConfirmation" runat="server" cssclass="confirmation" Visible="False" ></asp:label>
	<table>
		<TR>
			<TD vAlign="top">
				<P>Client {ONLY for Tesco}</P>
			</TD>
			<TD>
                <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                    MarkFirstMatch="true" ShowMoreResultsBox="true" Width="355px">
                </telerik:RadComboBox> </TD>
			<TD><asp:requiredfieldvalidator id="rfvClient" runat="server" ErrorMessage="Please select/enter a client." ControlToValidate="cboClient"
					Display="Dynamic">
					<img src="../images/Error.gif" height='16' width='16' title='Please enter/select a client.'></asp:requiredfieldvalidator></TD>
		</TR>
		<tr>
			<td>Select file:</td>
			<td><input id="uplTheFile" type="file" name="uplTheFile" runat="server"></td>
			<TD></TD>
		</tr>
	</table>
	<P><span id="txtOutput" style="FONT: 8pt verdana" runat="server"></span></P>
	<asp:datagrid id="dgJobDiscrepancies" runat="server" cssclass="DataGridStyle" OnPageIndexChanged="dgJobDiscrepancies_Page"
		PagerStyle-HorizontalAlign="Right" PagerStyle-Mode="NumericPages" border="1" backcolor="White"
		cellpadding="2" width="100%" pagesize="50" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
		ShowFooter="True">
		<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
		<ItemStyle CssClass="DataGridListItem"></ItemStyle>
		<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
		<Columns>
			<asp:BoundColumn DataField="ShipmentNo" HeaderText="Shipment Number"></asp:BoundColumn>
			<asp:BoundColumn DataField="PreInvoicePONumber" HeaderText="PO Number"></asp:BoundColumn>
			<asp:BoundColumn DataField="PreInvoicePickUpLocation" HeaderText="Pick Up Location"></asp:BoundColumn>
			<asp:BoundColumn DataField="HMSJobCost" HeaderText="Job Cost" DataFormatString="{0:C}">
				<ItemStyle HorizontalAlign="Right"></ItemStyle>
			</asp:BoundColumn>
			<asp:BoundColumn DataField="PreInvoiceJobCost" HeaderText="Pre Invoice Job Cost" DataFormatString="{0:C}">
				<ItemStyle HorizontalAlign="Right"></ItemStyle>
			</asp:BoundColumn>
			<asp:BoundColumn DataField="PreInvoiceApprovedDate" HeaderText="Approved Date" DataFormatString="{0:dd/MM/yy}">
				<ItemStyle HorizontalAlign="Right"></ItemStyle>
			</asp:BoundColumn>
			<asp:BoundColumn DataField="DiscrepancyNote" HeaderText="Discrepancy Note"></asp:BoundColumn>
			<asp:TemplateColumn HeaderText="Edit Job">
				<ItemTemplate>
					<asp:LinkButton id="lnkJobId" runat="server" CausesValidation="False" OnClick="lnkJobId_Click">Edit</asp:LinkButton>
					<input type="hidden" id="hidJobId" runat="server" value='<%#DataBinder.Eval(Container.DataItem, "JobId")%>'>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
		<PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
	</asp:datagrid>
	<P><asp:label id="lblPreInvoiceOverview" runat="server" Font-Bold="True"></asp:label></P>
	<DIV class="buttonbar">
		<asp:button id="btnUpload" runat="server" Text="Upload CSV File"></asp:button>
		<asp:button id="btnReload" runat="server" Text="Reload Previous" Visible="False"></asp:button>
		<asp:Button id="btnSave" runat="server" Text="Save Pre Invoice" Visible="False"></asp:Button>
	</DIV>
	<uc1:footer id="Footer1" runat="server"></uc1:footer></FORM>

