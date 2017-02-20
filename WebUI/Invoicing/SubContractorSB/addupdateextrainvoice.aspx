<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.SubContractorSB.AddUpdateExtraInvoice" MasterPageFile="~/default_tableless.Master" Codebehind="AddUpdateExtraInvoice.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h1><asp:Label id="Title" runat="server"></asp:Label></h1>
    <h2><asp:Label id="SubTitle" runat="server" ></asp:Label></h2>
    
    <uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
	
	<asp:Label id="lblConfirmation" EnableViewState="False" runat="server" cssclass="confirmation" visible="false" />
	
	<div>
        <telerik:RadAjaxPanel ID="rapInvoiceSummary" runat="server">
	        <fieldset>
		        <legend><strong>Invoice Summary</strong></legend>
		        <table width="100%">
			        <tr>
				        <td class="formCellLabel">Invoice No:</td>
				        <td class="formCellField"><asp:Label id="lblInvoiceNo" runat="server" ForeColor="Red" Text="To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)" /></td>
			        </tr>
			        <tr>
				        <td class="formCellLabel">Sub Contractor:</td>
				        <td class="formCellField"><asp:Label id="lblSubContractor" runat="server" /></td>
			        </tr>
			        <tr id="pnlSubbySelfBill" runat="server" visible="false">
                        <td class="formCellLabel" width="250">Self Bill</td>
                        <td class="formCellField" colspan="2"><asp:CheckBox ID="chkSelfBill" runat="server" Checked="false" AutoPostBack="true" /></td>
                    </tr>
                    <tr id="pnlSubbyInvoiceNo" runat="server" visible="true">
                        <td class="formCellLabel" width="250">Sub-Contractor Invoice No</td>
                        <td class="formCellField" colspan="2"><asp:TextBox ID="txtSubbyInvoiceNo" runat="server" MaxLength="255" ></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Nominal Code</td>
                        <td class="formCellField">
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
			        <tr>
				        <td class="formCellLabel">Invoice Amount (NET)</td>
				        <td class="formCellField"><telerik:RadNumericTextBox id="txtAmountNet" runat="server" ReadOnly="true" Type="Currency"></telerik:RadNumericTextBox></td
			        </tr>
			        <tr>
                        <td class="formCellLabel"><asp:Label ID="lblVATType" runat="server" Text="VAT Type"></asp:Label></td>
                        <td class="formCellField"><asp:DropDownList runat="server" ID="cboVATType" AutoPostBack="true" /></td>
                    </tr>
			        <tr>
				        <td class="formCellLabel">Invoice Amount (Gross)</td>
				        <td class="formCellField">
					        <telerik:RadNumericTextBox id="txtAmountGross" runat="server" ReadOnly="true" Type="Currency"></telerik:RadNumericTextBox>
					        &nbsp;
					        <asp:CheckBox id="chkOverrideAmount" runat="server" AutoPostBack="True" Text="Override" style="Display:none;"/>
				        </td>
			        </tr>
			        <tr>
				        <td class="formCellLabel">Invoice Date</td>
				        <td class="formCellField">
					        <table cellpadding="0" cellspacing="1" border="0">
						        <tr>
							        <td><telerik:RadDateInput ID="rdiInvoiceDate" runat="server" ToolTip="The date of the invoice" DateFormat="dd/MM/yy" Width="60px" /></td>
							        <td><asp:RequiredFieldValidator id="rfvInvoiceDate" runat="server" ControlToValidate="rdiInvoiceDate" ErrorMessage="Please supply an invoice date."><img src="../images/Error.gif" height="16" width="16" title="Please supply an invoice date." /></asp:RequiredFieldValidator></td>
						        </tr>
					        </table>
				        </td>
			        </tr>
			        <tr>
				        <td class="formCellLabel">Date Created</td>
				        <td class="formCellField"><asp:Label id="lblDateCreated" runat="server" text="N/A" ForeColor="Red" /></td>
			        </tr>
			        <tr>
				        <td class="formCellLabel">Post To Accounts</td>
				        <td class="formCellField">
					        <asp:checkbox id="chkPostToExchequer" runat="server" visible="false" Text="Post To Accounts" ReadOnly="True" Enabled="False"></asp:checkbox>
				        </td>
			        </tr>
			        <tr>
				        <td class="formCellLabel">Invoice Details</td>
				        <td class="formCellField"><asp:textbox id="txtInvoiceDetails" runat="server" Height="80px" Width="300px" TextMode="MultiLine"></asp:textbox></td>
			        </tr>
			        <tr>
				        <td class="formCellLabel">Include</td>
				        <td class="formCellField"><asp:checkbox id="chkIncludeExtraDetail" runat="server" Text="Extra details" Checked="True" /></td>
			        </tr>
			        <tr>
				        <td>&nbsp;</td>
				        <td class="formCellField"><asp:checkbox id="chkIncludeInvoiceDetail" runat="server" Text="Invoice details" Checked="False" /></td>
			        </tr>
                </table>
	        </fieldset>
	    </telerik:RadAjaxPanel>
	    <div class="buttonbar">
	        <nfvc:NoFormValButton id="btnChangeExtras" runat="server" text="Change Selected Extras" />
	    </div>
    </div>
						
	<div>
		<asp:Panel id="pnlOverrideAmounts" runat="server" Visible="False">
            <fieldset>
                <legend><strong>Override Amounts</strong></legend>
                <table width="100%">
                    <tr>
                        <td>VAT:</td>
                        <td><telerik:RadNumericTextBox id=txtOverrideAmountVAT runat="server" Type="Currency"></telerik:RadNumericTextBox></td>
                    </tr>
                    <tr>
                        <td>Total Amount (Net):</td>
                        <td><telerik:RadNumericTextBox id=txtOverrideAmountNet runat="server" Type="Currency"></telerik:RadNumericTextBox></td>
                    </tr>
                    <tr>
                        <td>Total Amount (Gross):</td>
                        <td><telerik:RadNumericTextBox id=txtOverrideAmountGross runat="server" Type="Currency"></telerik:RadNumericTextBox></td>
                    </tr>
                    <tr>
                        <td>Reason for override:</td>
                        <td><asp:TextBox id=txtReasonForOverride runat="server" TextMode="MultiLine" Width="300px" Height="80px"></asp:TextBox></td>
                    </tr>
                </table>
            </fieldset>
		</asp:Panel>
	</div>
						
	<div>
		<asp:panel id="pnlInvoiceDeleted" runat="server" Visible="False">
            <fieldset><legend><strong>Is Invoice Deleted</strong></legend>
                <asp:checkbox id="chkDelete" runat="server" text="This Invoice is deleted."></asp:checkbox>
            </fieldset>
		</asp:panel>
	</div>
	
	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnViewInvoice" runat="server" Text="View Invoice" />
		<nfvc:noformvalbutton id="btnSendToAccounts" runat="server" text="Post To Accounts" Visible="False" ServerClick="btnSendToAccounts_Click" onclick="btnSendToAccounts_Click"></nfvc:noformvalbutton>
		<nfvc:NoFormValButton id="btnAddUpdateInvoice" runat="server" Visible="False" Text="Add Invoice" />
	</div>
	
	<uc1:reportviewer id="reportViewer" runat="server" EnableViewState="False" Visible="False" ViewerHeight="800" ViewerWidth="100%"></uc1:reportviewer>

</asp:Content>