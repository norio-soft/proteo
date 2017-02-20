<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.AddUpdateExtraInvoice" MasterPageFile="~/default_tableless.Master" Codebehind="AddUpdateExtraInvoice.aspx.cs" EnableEventValidation="false" AutoEventWireup="True" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1><asp:Label id="Title" runat="server"></asp:Label></h1>
    <h2><asp:Label id="SubTitle" runat="server" ></asp:Label></h2>
    
    <uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>

	<asp:Label id="lblConfirmation" EnableViewState="False" runat="server" cssclass="confirmation" visible="false" />

    <div>
        <fieldset>
		    <legend><strong>Invoice Summary</strong></legend>
		    <table width="100%">
			    <tr>
				    <td>Invoice No:</td>
				    <td><asp:Label id="lblInvoiceNo" runat="server" ForeColor="Red" Text="To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)" /></td>
			    </tr>
			    <tr>
				    <td>Customer:</td>
				    <td><asp:Label id="lblClient" runat="server" /></td>
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
                        <asp:CustomValidator ID="cvNominalCode" runat="server" ControlToValidate="cboNominalCode" OnServerValidate="cvNominalCode_Validate" ErrorMessage="Please Select a Nominal Code.">
                            <img src="../images/error.gif" alt="Please Select a Nominal Code." />
                        </asp:CustomValidator>
                    </td>
                </tr>
			    <tr>
				    <td>Invoice Amount (NET)</td>
				    <td><telerik:RadNumericTextBox id="txtAmountNet" runat="server" ReadOnly="true" Type="Currency"></telerik:RadNumericTextBox></td>
			    </tr>
			    <tr>
                    <td><asp:Label ID="lblVATType" runat="server" Text="VAT Type"></asp:Label></td>
                    <td><asp:DropDownList runat="server" ID="cboVATType" AutoPostBack="true" /></td>
                </tr>
			    <tr>
				    <td>Invoice Amount (Gross)</td>
				    <td>
					    <telerik:RadNumericTextBox id="txtAmountGross" runat="server" ReadOnly="true" Type="Currency"></telerik:RadNumericTextBox>
					    &nbsp;
					    <asp:CheckBox id="chkOverrideAmount" runat="server" AutoPostBack="True" Text="Override" style="Display:none;"/>
				    </td>
			    </tr>
			    <tr>
				    <td>Invoice Date</td>
				    <td>
					    <table cellpadding="0" cellspacing="1" border="0">
						    <tr>
						        <td><telerik:RadDateInput ID="rdiInvoiceDate" runat="server" ToolTip="The date of the invoice." DateFormat="dd/MM/yy" Width="60px" /> </td>
							    <td><asp:RequiredFieldValidator id="rfvInvoiceDate" runat="server" ControlToValidate="rdiInvoiceDate" ErrorMessage="Please supply an invoice date."><img src="../images/Error.gif" height="16" width="16" title="Please supply an invoice date." /></asp:RequiredFieldValidator></td>
						    </tr>
					    </table>
				    </td>
			    </tr>
			    <tr>
				    <td>Date Created</td>
				    <td><asp:Label id="lblDateCreated" runat="server" text="N/A" ForeColor="Red" /></td>
			    </tr>
			    <tr>
				    <td>Post To Accounts</td>
				    <td>
					    <asp:checkbox id="chkPostToExchequer" runat="server" visible="false" Text="Post To Accounts" ReadOnly="True" Enabled="False"></asp:checkbox>
				    </td>
			    </tr>
			    <tr>
				    <td>Invoice Details</td>
				    <td><asp:textbox id="txtInvoiceDetails" runat="server" Height="80px" Width="300px" TextMode="MultiLine"></asp:textbox></td>
			    </tr>
			    <tr>
				    <td>Include</td>
				    <td><asp:checkbox id="chkIncludeExtraDetail" runat="server" Text="Extra details" Checked="True" /></td>
			    </tr>
			    <tr>
				    <td>&nbsp;</td>
				    <td><asp:checkbox id="chkIncludeInvoiceDetail" runat="server" Text="Invoice details" Checked="True" /></td>
			    </tr>
            </table>
	    </fieldset>
		<div class="buttonbar">
		    <nfvc:NoFormValButton id="btnChangeExtras" runat="server" text="Change Selected Extras" />
		</div>
    </div>
    
    <div>
        <asp:Panel id="pnlOverrideAmounts" runat="server" Visible="false">
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
            <fieldset>
                <legend><strong>Is Invoice Deleted</strong></legend>
                <asp:checkbox id="chkDelete" runat="server" text="This Invoice is deleted."></asp:checkbox>
            </fieldset>
		</asp:panel>
	</div>
	
	<div class="buttonbar">
	    <asp:Button id="btnViewInvoice" runat="server" text="View Invoice" ></asp:Button>
		<asp:Button id="btnSendToAccounts" runat="server" text="Post To Accounts" Visible="false" 
			OnClientClick="this.disabled=true;__doPostBack('ctl00$ContentPlaceHolder1$btnSendToAccounts','');" ></asp:Button>
        <asp:Button id="btnAddUpdateInvoice" runat="server" text="Add Invoice" Visible="false" ></asp:Button>
	</div>
	
	<uc1:reportviewer id="reportViewer" runat="server" EnableViewState="False" Visible="False" ViewerHeight="800" ViewerWidth="100%"></uc1:reportviewer>

    <telerik:RadAjaxManager ID="ramInvoiceSummary" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboVATType" >
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="txtAmountGross" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>