<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.addupdateinvoice" Codebehind="addupdateinvoice.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<TITLE ></TITLE>
<uc1:header id="Header1" title="Add/Update Invoice" SubTitle="Please enter the Invoice Details below."
	XMLPath="InvoiceContextMenu.xml" runat="server"></uc1:header>
<form id="Form1" runat="server" style="HEIGHT:100%; WIDTH:100%; OVERFLOW:scroll">
    <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false"></cs:WebModalWindowHelper>
	
	<uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>

	<asp:label id="lblConfirmation" runat="server" text="The new Invoice has been added successfully."
		cssclass="confirmation" visible="false">The new Invoice has been added successfully.</asp:label>
	<!-- Invoice Section -->
	<fieldset>
		<legend>
			<STRONG>Invoice Details</STRONG></legend>
		<table>
			<TR>
				<TD>Invoice No.
				</TD>
				<TD><asp:label id="lblInvoiceNo" runat="server" text="To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)"
						ForeColor="Red">To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)</asp:label></TD>
			</TR>
			<TR>
				<TD>Invoice Type
				</TD>
				<TD><asp:label id="lblInvoiceType" runat="server"></asp:label></TD>
			</TR>
			<tr>
				<td>Invoice Date</td>
				<td>
					<table cellpadding="0" cellspacing="1" border="0">
						<tr>
							<td><telerik:RadDateInput id="dteInvoiceDate" runat="server" ToolTip="The date of the invoice" dateformat="dd/MM/yy" Width="60px" Nullable="False"></telerik:RadDateInput></td>
							<td><asp:RequiredFieldValidator id="rfvInvoiceDate" runat="server" ControlToValidate="dteInvoiceDate" ErrorMessage="Please supply an invoice date."><img src="../images/Error.gif" height="16" width="16" title="Please supply an invoice date." /></asp:RequiredFieldValidator></td>
						</tr>
					</table>
				</td>
			</tr>
            <tr>
                <td>Nominal Code</td>
                <td>
                    <telerik:RadComboBox ID="cboNominalCode" runat="server" Skin="WindowsXP" HighlightTemplatedItems="true" Width="230px">
                        <HeaderTemplate>
                            <table style="width:230px; text-align:left;">
                                <tr><td style="width:80px;">Code</td><td style="width:150px;">Description</td></tr>
                            </table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table>
                                <tr><td style="width:80px;"><%#DataBinder.Eval(Container.DataItem, "NominalCode")%></td><td style="width:150px;"><%#DataBinder.Eval(Container.DataItem, "Description") %></td></tr>
                            </table>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </td>
            </tr>
			<TR>
				<TD>Date Created
				</TD>
				<TD><asp:label id="lblDateCreated" runat="server" text="N/A" ForeColor="Red">N/A</asp:label></TD>
			</TR>
			<TR>
				<TD vAlign="top">Post To Accounts
				</TD>
				<TD><asp:checkbox id="chkPostToExchequer" runat="server" visible="false" Text="Post To Accounts" Enabled="False"></asp:checkbox></TD>
			</TR>
			<TR>
				<TD vAlign="top">Invoice Notes (Internal Use ONLY - Not displayed on report)
				</TD>
				<TD><asp:textbox id="txtInvoiceNotes" runat="server" Height="83px" Width="307px" TextMode="MultiLine"></asp:textbox></TD>
			</TR>
			<div id="divClientSelfBillAmount" runat="server" visible="false">
			<tr>
				<TD vAlign="top"><asp:label id="lblClientInvoiceSelfBillAmount" visible="false" runat="server" text="Customer's Invoice Self Bill Amount"></asp:label>
				</TD>
				<TD>
					<asp:textbox id="txtClientSelfBillAmount" runat="server" visible="false"></asp:textbox>
					<asp:requiredfieldvalidator id="rfvClientSelfBillAmount" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>" ErrorMessage="Please enter an Amount." ControlToValidate="txtClientSelfBillAmount" Display="Dynamic">
								<img src="../images/Error.gif" height='16' width='16' title='Please enter an Amount.'
									alt='Field is Required'></asp:requiredfieldvalidator>
				</TD>
			</tr>
			<tr>
				<TD vAlign="top"><asp:label id="lblClientSelfBillInvoiceNumber" visible="false" runat="server" text="Customer's Invoice Self Bill Number"></asp:label>
				</TD>
				<TD>
					<asp:textbox id="txtClientSelfBillInvoiceNumber" runat="server" visible="false"></asp:textbox>
					<asp:requiredfieldvalidator id="rfvClientSelfBillInvoiceNumber" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>" ErrorMessage="Please enter an Amount." ControlToValidate="txtClientSelfBillInvoiceNumber" Display="Dynamic">
								<img src="../images/Error.gif" height='16' width='16' title='Please enter an Customer Invoice Number.'
									alt='Field is Required'></asp:requiredfieldvalidator>
				</TD>
			</tr>
			</DIV> 
			<tr>
				<TD vAlign="top"><asp:label id="lblRemainder" visible="false" runat="server" text="Create Remainder Job"></asp:label>
				</TD>
				<TD><asp:checkbox id="chkSelfBillRemainder" runat="server" visible="false" AutoPostBack="True" Text="Create Remainder Job"></asp:checkbox></TD>
			</tr>
		</table>
	</fieldset>
	<asp:panel id="pnlSelfRemainder" runat="server" visible="false">
<FIELDSET><LEGEND><STRONG>Self Bill Remainder Section</STRONG></LEGEND>
<TABLE></TABLE>
<P>
<TABLE id=Table2>
  <TR>
    <TD>Amount </TD>
    <TD>
<asp:TextBox id=txtSelfBillAmount runat="server"></asp:TextBox></TD>
    <TD>
<asp:requiredfieldvalidator id=rfvAmount runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>" Display="Dynamic" ControlToValidate="txtSelfBillAmount" ErrorMessage="Please enter an Amount.">
								<img src="../images/Error.gif" height='16' width='16' title='Please enter an Amount.'
									alt='Field is Required'></asp:requiredfieldvalidator></TD></TR>
  <TR>
    <TD>Accepted</TD>
    <TD>
<asp:DropDownList id=cboSelfBillStatus runat="server"></asp:DropDownList></TD>
    <TD>
<asp:requiredfieldvalidator id=rfvAccepted runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>" Display="Dynamic" ControlToValidate="cboSelfBillStatus" ErrorMessage="Please enter an Status.">
								<img src="../images/Error.gif" height='16' width='16' title='Please enter an Status.'
									alt='Field is Required'></asp:requiredfieldvalidator></TD></TR>
  <TR>
    <TD>Whom Accepted/Rejected</TD>
    <TD>
<asp:TextBox id=txtBillName runat="server"></asp:TextBox></TD>
    <TD></TD></TR>
  <TR>
    <TD></TD>
    <TD vAlign=top>
<nfvc:NoFormValButton id=btnAddSelfBill runat="server" text="Add Self Bill Job" Visible="True" ServerClick="btnSelfBillAdd_Click"></nfvc:NoFormValButton>
<asp:Label id=lblJobId runat="server" cssclass="confirmation"></asp:Label></TD>
    <TD></TD></TR></TABLE></P></FIELDSET>
	</asp:panel>
	<asp:Panel id="pnlExtras" runat="server" Visible="False">
	<FIELDSET><LEGEND><STRONG>Extras to be included on this invoice</STRONG></LEGEND>

<TABLE cellSpacing=0 cellPadding=1 width="100%" border=0>
  <TR>
    <TD>
<asp:DataGrid id=dgExtras runat="server" AutoGenerateColumns="False">
							<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
							<ItemStyle CssClass="DataGridListItem"></ItemStyle>
							<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="ExtraId" Visible="False" />
								<asp:BoundColumn HeaderText="Job Id" DataField="JobId" />
								<asp:BoundColumn HeaderText="Extra Type" DataField="ExtraType" />
								<asp:BoundColumn HeaderText="Description" DataField="CustomDescription" />
								<asp:BoundColumn HeaderText="Extra State" DataField="ExtraState" />
								<asp:BoundColumn HeaderText="Extra Amount" DataField="ExtraAmount" DataFormatString="{0:C}" />
								<asp:TemplateColumn HeaderText="Inclusion Description" Visible="False">
									<ItemTemplate>
										<asp:TextBox Visible="False" id="txtInclusionDescription" runat="server" TextMode="MultiLine" />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:DataGrid></TD></TR></TABLE></FIELDSET>
	</asp:Panel>
	<!-- OVERRIDE SECTION -->
	<FIELDSET>
		<P><LEGEND><STRONG>Override Section - (One liner will be displayed on the report with a description if all includes are unticked)</STRONG></LEGEND></P>
		<TABLE id="Table5" visible="false">
			<tr>
				<td>
					<asp:CheckBox id="chkOverride" runat="server" AutoPostBack="True" Text="Override Amounts" TextAlign="Left"></asp:CheckBox>
				</td>
			</tr>
		</TABLE>
		<asp:panel id="pnlOverride" runat="server" Visible="false">
<TABLE id=Table6>
  <TR>
    <TD>
<asp:label id=lblVAT runat="server" text="VAT"></asp:label></TD>
    <TD>
<asp:TextBox id=txtOverrideVAT runat="server"></asp:TextBox></TD>
    <TD>
<asp:requiredfieldvalidator id=rfvOverrideVat runat="server" Display="Dynamic" ControlToValidate="txtOverrideVAT" ErrorMessage="Please enter a VAT Amount." EnableClientScript="True">
							<img src="../images/Error.gif" height="16" width="16" title="Please enter a VAT Amount." /></asp:requiredfieldvalidator></TD></TR>
  <TR>
    <TD>
<asp:label id=lblAmount runat="server" text="Total Amount (NET)"></asp:label></TD>
    <TD>
<asp:TextBox id=txtOverrideNetAmount runat="server"></asp:TextBox></TD>
    <TD>
<asp:requiredfieldvalidator id=rfvOverrideNetAmount runat="server" Display="Dynamic" ControlToValidate="txtOverrideNetAmount" ErrorMessage="Please enter a NET Amount." EnableClientScript="True">
							<img src="../images/Error.gif" height="16" width="16" title="Please enter a NET Amount." /></asp:requiredfieldvalidator></TD></TR>
  <TR>
    <TD>
<asp:label id=lblTotalAmount runat="server" text="Total Amount (GROSS)"></asp:label></TD>
    <TD>
<asp:TextBox id=txtOverrideGrossAmount runat="server"></asp:TextBox></TD>
    <TD>
<asp:requiredfieldvalidator id=rfvOverrideGrossAmount runat="server" Display="Dynamic" ControlToValidate="txtOverrideGrossAmount" ErrorMessage="Please enter a Gross Amount." EnableClientScript="True">
							<img src="../images/Error.gif" height="16" width="16" title="Please enter a Gross Amount." /></asp:requiredfieldvalidator></TD></TR>
  <TR>
    <TD>
<asp:label id=lblReason runat="server" text="Reason (This will be displayed on the report)"></asp:label></TD>
    <TD>
<asp:TextBox id=txtOverrideReason runat="server" TextMode="MultiLine" Width="307px" Height="83px"></asp:TextBox></TD>
    <TD>
<asp:requiredfieldvalidator id=rfvOverrideReason runat="server" Display="Dynamic" ControlToValidate="txtOverrideReason" ErrorMessage="Please enter a reason for the overriding the values of the invoice." EnableClientScript="True">
							<img src="../images/Error.gif" height="16" width="16" title="Please enter a reason for the overrides." /></asp:requiredfieldvalidator></TD></TR></TABLE>
		</asp:panel>
	</FIELDSET>
	<!-- INCLUDE SECTION -->
	<table width=100%>
	<tr>
	<td>
	<FIELDSET>
		<P><LEGEND><STRONG>Include Section</STRONG></LEGEND></P>
		<asp:panel id="pnlIncludes" runat="server" visible="true">
<TABLE width=100%>
  <TR>
    <TD></TD>
    <TD>
<asp:CheckBox id=chkIncludePODs runat="server" Text="PODs" AutoPostBack="True"></asp:CheckBox></TD>
    <TD></TD></TR>
  <TR>
    <TD></TD>
    <TD>
<asp:CheckBox id=chkIncludeReferences runat="server" Text="References" AutoPostBack="False"></asp:CheckBox></TD>
    <TD></TD></TR>
    <TR>
    <TD></TD>
    <TD>
<asp:CheckBox id=chkIncludeDemurrage runat="server" Text="Demurrage Details" ></asp:CheckBox>
    <asp:Label id=lblNoDemurrage runat="server" Visible="False">No Demurrage To Be Added For This Invoice</asp:Label></TD>
    <TD>
    <asp:RadioButtonList id=rdoDemurrageType runat="server" Visible="true" repeatdirection="horizontal"></asp:RadioButtonList></TD>
    <TD>
<asp:Label id=lblAcceptedDemurrage runat="server" Visible="True"></asp:Label></TD></TR>
  
  <TR>
    <TD></TD>
    <TD>
<asp:CheckBox id=chkJobDetails runat="server" Text="Job Details" AutoPostBack="False" Checked="True"></asp:CheckBox></TD>
    <TD></TD></TR>
  <TR>
    <TD></TD>
    <TD>
<asp:CheckBox id=chkExtraDetails runat="server" Text="Extra Details" AutoPostBack="False" Visible="False"></asp:CheckBox>
    <TD></TD></TR>
    
    <tr>
        <td>&nbsp;</td>
        <td><asp:CheckBox id="chkShowInstructionNotes" runat="server" Text="Show Instruction Notes" AutoPostBack="False"></asp:CheckBox></td>
    </tr>
    
  <TR>
    <TD ></TD>
    <TD>
       <asp:CheckBox id=chkIncludeFuelSurcharge runat="server" Text="Fuel Surcharge" AutoPostBack="true"></asp:CheckBox>
        <span id=divFuelSurcharge runat="server" Visible="False">
            <asp:TextBox id=txtFuelSurchargeRate runat="server"></asp:TextBox>% 
            <asp:RangeValidator id=rfvFuelSurchargeRate runat="server" Display="Dynamic" ControlToValidate="txtFuelSurchargeRate" ErrorMessage="Please enter a valid higher than 0% and less than 100%." EnableClientScript="True" Type="Double" MaximumValue="100" MinimumValue="-100"><img src="../images/Error.gif" height="16" width="16" title="Please enter a number between -100 and 100."/></asp:RangeValidator>
            <asp:requiredfieldvalidator id=rfvFuelSurhargeRateRequired runat="server" Display="Dynamic" ControlToValidate="txtFuelSurchargeRate" ErrorMessage="Please enter a fuel surcharge amount." EnableClientScript="True"><img src="../images/Error.gif" height="16" width="16" title="Please enter a fuel surcharge amount." /></asp:requiredfieldvalidator></span>
    </TD>
    <TD >
        <asp:RadioButtonList id=rdoFuelSurchargeType runat="server" Visible="false" repeatdirection="horizontal"></asp:RadioButtonList>
    </TD>
 </TR>
 <tr>
    <td></td>
    <td><asp:checkbox id="chkExtrasPerJob" runat="server" text="Show Job Extras" AutoPostBack="False"></asp:checkbox></td></tr>
</TABLE>
		</asp:panel>
	</FIELDSET>
	</td>
	<td height="100%">
	<!-- Sort Section -->
	<asp:panel id="pnlSort" runat="server" Visible="false" height="100%">
	<FIELDSET>
		<P><LEGEND><strong>Sort Section</strong></LEGEND></P>
		<table width=100%>
			<TR>
				<TD vAlign="top">Sort By
				</TD>
				<TD><asp:radiobuttonlist id="rdoSortType" runat="server" AutoPostBack="True" RepeatColumns="3" repeatDirection="horizontal"></asp:radiobuttonlist></TD>
			</TR>
		</table>
	</FIELDSET>
	</asp:panel>
	</td>
	</tr>
	</table>
	
	<asp:panel id="pnlGroup" runat="server" Visible="false">
<FIELDSET>
<P><LEGEND><STRONG>Group By Section</STRONG></LEGEND></P>
<TABLE id=Table1>
  <TR>
    <TD vAlign=top>Group By </TD>
    <TD>
<asp:RadioButtonList id=rdoGroupBy runat="server" AutoPostBack="True" repeatDirection="horizontal" RepeatColumns="3"></asp:RadioButtonList></TD></TR></TABLE>
<asp:DataGrid id=dgGroup runat="server" cssclass="DataGridStyle" AutoGenerateColumns="False" OnPageIndexChanged="dgGroup_Page" PagerStyle-HorizontalAlign="Right" PagerStyle-Mode="NumericPages" border="1" backcolor="White" cellpadding="2" width="100%" pagesize="50" AllowPaging="True" AllowSorting="True" ShowFooter="True">
				<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
				<ItemStyle CssClass="DataGridListItem"></ItemStyle>
				<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="InvoiceId" HeaderText="Invoice Id">
						<ItemStyle ForeColor="Red"></ItemStyle>
					</asp:BoundColumn>
					<asp:BoundColumn DataField="OrganisationName" HeaderText="Organisation Name"></asp:BoundColumn>
					<asp:BoundColumn DataField="FullAddress" HeaderText="Point Address"></asp:BoundColumn>
					<asp:BoundColumn DataField="JobIdList" HeaderText="Job Id(s)"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="PointName" HeaderText="Point Name"></asp:BoundColumn>
					<asp:BoundColumn Visible="False" DataField="PointId" HeaderText="Point Id"></asp:BoundColumn>
					<asp:ButtonColumn HeaderText="View Invoice" Text="View Invoice" CommandName="View"></asp:ButtonColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
			</asp:DataGrid></FIELDSET>
	</asp:panel>
	<asp:panel id="pnlInvoiceDeleted" runat="server" Visible="False">
<FIELDSET><LEGEND>
<P><STRONG>Is Invoice Deleted</STRONG></P></LEGEND>
<asp:checkbox id=chkDelete runat="server" text="This Invoice is deleted."></asp:checkbox></FIELDSET>
	</asp:panel>
	<br>
	<div class="buttonbar"><nfvc:noformvalbutton id="btnAdd" runat="server" text="Add New Invoice" ServerClick="btnAdd_Click" Visible="False"></nfvc:noformvalbutton><nfvc:noformvalbutton id="btnSendToAccounts" runat="server" text="Post To Accounts" ServerClick="btnSendToAccounts_Click"
			Visible="False"></nfvc:noformvalbutton><nfvc:noformvalbutton id="btnViewInvoice" runat="server" text="View Invoice" ServerClick="btnViewInvoice_Click"></nfvc:noformvalbutton></div>
	<uc1:reportviewer id="reportViewer" runat="server" Visible="False" ViewerHeight="800" ViewerWidth="100%"></uc1:reportviewer>
	<br>
</form>
<uc1:footer id="Footer1" runat="server"></uc1:footer>
