<%@ Page Language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Invoicing.AddUpdateOneLinerInvoice" CodeBehind="AddUpdateOneLinerInvoice.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Add/Update One Liner Invoice</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<script language="javascript" type="text/javascript">
		<!--
    	function updateAmount(el)
		{
			var cost = new NumberFormat(el.value) 

			el.value = cost.toFormatted();

        }

        function AlphabetOnly(sender, eventArgs) {

            var c = eventArgs.get_keyCode();

            if (
                    (c < 13) ||
                    (c > 13 && c < 32) ||
                    (c > 32 && c < 37) ||
                    (c > 38 && c < 40) ||
                    (c > 41 && c < 43) ||
                    (c > 46 && c < 47) ||
                    (c > 47 && c < 48) ||
                    (c > 90 && c < 92) ||
                    (c > 92 && c < 97) ||
                    (c > 122 && c < 163) ||
                    (c > 163)
                )
                eventArgs.set_cancel(true);
        }
		//-->
	</script>
	
	<asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation" Text="The new Invoice has been added successfully.">The new Invoice has been added successfully.</asp:Label>
	
	<fieldset>
		<legend>Invoice Details</legend>
		<table id="Table1">
			<tr>
				<td class="formCellLabel">Invoice No.</td>
				<td class="formCellField">
					<asp:Label ID="lblInvoiceNo" runat="server" Text="To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)" ForeColor="Red">To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)</asp:Label>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Invoice Type </td>
				<td class="formCellField">
					<asp:Label ID="lblInvoiceType" runat="server"></asp:Label>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Date Created</td>
				<td class="formCellField">
					<asp:Label ID="lblDateCreated" runat="server" Text="N/A" ForeColor="Red">N/A</asp:Label>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Invoice Date</td>
				<td class="formCellField">
					<table>
						<tr>
							<td><telerik:RadDatePicker ID="rdiInvoiceDate" runat="server" ToolTip="The date of the invoice." Width="100px" >
                            <DateInput runat="server"
                            DateFormat="dd/MM/yy">
                            </DateInput>
                            </telerik:RadDatePicker></td>
							<td>
								<asp:RequiredFieldValidator ID="rfvInvoiceDate" runat="server" ControlToValidate="rdiInvoiceDate" ErrorMessage="Please supply an invoice date.">
									<img src="/images/Error.gif" height="16" width="16" title="Please supply an invoice date." alt="" />
								</asp:RequiredFieldValidator>
							</td>
                            <td>                
                                <asp:CustomValidator ID="cvInvoiceDate" runat="server" Enabled="true" ControlToValidate="rdiInvoiceDate"
	                                Display="Dynamic" EnableClientScript="true" ClientValidationFunction="ValidateInvoiceDate"
	                                ErrorMessage="This Date may be Wrong">
                	            </asp:CustomValidator>
                            </td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Posted To Accounts</td>
				<td class="formCellField"><asp:CheckBox ID="chkPostToExchequer" runat="server" Visible="false" Text="Posted To Accounts" Enabled="False"></asp:CheckBox></td>
			</tr>
			<asp:Panel ID="pnlAccoutnCode" runat="server">
				<tr>
					<td  class="formCellLabel"><asp:Label ID="lblClient" Text="Customer" runat="server"></asp:Label></td>
					<td class="formCellField">
						<telerik:RadComboBox ID="cboClient" runat="server" CausesValidation="false" EnableLoadOnDemand="true" ShowMoreResultsBox="true" DataTextField="OrganisationName" DataValueField="IdentityId" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" AutoPostBack="true" />
						<asp:RequiredFieldValidator ID="rfvClient" runat="server" ErrorMessage="Please enter a Client." ControlToValidate="cboClient" Display="Dynamic" EnableClientScript="True">
							<img src="/images/Error.gif" height="16" width="16" title="Please enter a Client." alt="" />
						</asp:RequiredFieldValidator>
					</td>
				</tr>
				<tr>
					<td class="formCellLabel">Account Code</td>
					<td class="formCellField"><asp:Label ID="lblAccountCode" Font-Bold="true" runat="server" /></td>
				</tr>
				<tr>
					<td class="formCellLabel">Nominal Code</td>
					<td class="formCellField">
						<telerik:RadComboBox ID="cboNominalCode" runat="server" HighlightTemplatedItems="true" Width="230px">
							<HeaderTemplate>
								<table style="width: 230px; text-align: left;">
									<tr>
										<td style="width: 80px;">Code</td>
										<td style="width: 150px;">Description</td>
									</tr>
								</table>
							</HeaderTemplate>
							<ItemTemplate>
								<table>
									<tr>
										<td style="width: 80px;">
											<%#DataBinder.Eval(Container.DataItem, "NominalCode")%>
										</td>
										<td style="width: 150px;">
											<%#DataBinder.Eval(Container.DataItem, "Description") %>
										</td>
									</tr>
								</table>
							</ItemTemplate>
						</telerik:RadComboBox>
						<asp:CompareValidator ID="cvNominalCode" runat="server" ErrorMessage="Please enter a Nominal Code." ControlToValidate="cboNominalCode" Display="Dynamic" EnableClientScript="True" ValueToCompare="" Operator="NotEqual">
							<img src="/images/Error.gif" height="16" width="16" title="Please enter a Nominal Code." alt="" />
						</asp:CompareValidator>
						<asp:RequiredFieldValidator ID="rfvNominalCode" runat="server" ErrorMessage="Please enter a Nominal Code." ControlToValidate="cboNominalCode" Display="Dynamic" EnableClientScript="True">
							<img src="/images/Error.gif" height="16" width="16" title="Please enter a Nominal Code." alt="" />
						</asp:RequiredFieldValidator>
					</td>
				</tr>
			</asp:Panel>
			<tr>
				<td class="formCellLabel"><asp:Label ID="lblTotalAmount" runat="server" Text="Total Amount (NET)"></asp:Label></td>
				<td class="formCellField">
					<telerik:RadNumericTextBox ID="txtNETAmount" runat="server" Type="Currency"></telerik:RadNumericTextBox>
					<asp:RequiredFieldValidator ID="rfvNETAmount" runat="server" ErrorMessage="Please enter a NET Amount." ControlToValidate="txtNETAmount" Display="Dynamic" EnableClientScript="True">
						<img src="/images/Error.gif" height="16" width="16" title="Please enter a NET Amount." alt="" />
					</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel"><asp:Label ID="lblVATType" runat="server" Text="VAT Type"></asp:Label></td>
                <td class="formCellField">
                    <asp:DropDownList runat="server" ID="cboVATType" /></td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    <asp:Label ID="lblReason" runat="server" Text="Reason For Invoice"></asp:Label></td>

                <td class="formCellField">
                    <telerik:RadTextBox ID="rtbTxtReason" runat="server" TextMode="MultiLine" Width="414px"  Font-Names="Arial" Font-Size="10pt" Height="83px">

                    </telerik:RadTextBox>

                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter a reason for creating this invoice." ControlToValidate="rtbTxtReason" Display="Dynamic" EnableClientScript="True">
						<img src="/images/Error.gif" height="16" width="16" title="Please enter a reason for creating this invoice." alt="" />
                        </asp:RequiredFieldValidator>
                </td>
            </tr>
			<tr>
				<td class="formCellLabel"></td>
				<td class="formCellField"><asp:CheckBox ID="chkUseHeadedPaper" runat="server" Text="Print on Headed Paper" /></td>
			</tr>
		</table>
	</fieldset>
	
	<asp:Panel ID="pnlInvoiceDeleted" runat="server" Visible="False">
		<fieldset>
			<legend><strong>Is Invoice Deleted</strong></legend>
			<asp:CheckBox ID="chkDelete" runat="server" Text="This Invoice is deleted."></asp:CheckBox>
		</fieldset>
	</asp:Panel>
	
	<telerik:RadAjaxManager ID="raxmanager" runat="server">
		<AjaxSettings>
			<telerik:AjaxSetting AjaxControlID="cboClient">
				<UpdatedControls>
				   <telerik:AjaxUpdatedControl ControlID="lblAccountCode" />
					<telerik:AjaxUpdatedControl ControlID="txtNETAmount" />
				</UpdatedControls>
			</telerik:AjaxSetting>
		</AjaxSettings>
	</telerik:RadAjaxManager>
	
	<div class="buttonbar">
        <table>
            <tr>
                <td>
		            <asp:Button id="btnAdd" runat="server" text="Add New Invoice" Visible="false" onclientclick="this.disabled=true; this.value='Please Wait....'; __doPostBack('ctl00$ContentPlaceHolder1$btnAdd','');" ></asp:Button>
                </td>
                <td>
		            <asp:Button id="btnSendToAccounts" runat="server" text="Post To Accounts" Visible="false"
			            OnClientClick="this.disabled=true;__doPostBack('ctl00$ContentPlaceHolder1$btnSendToAccounts','');" ></asp:Button>
                </td>
                <td>
		            <asp:Button id="btnViewInvoice" runat="server" text="View Invoice" ></asp:Button>    
                </td>
                <td class="formCellLabel">
				    <asp:Label ID="lblEmail" runat="server" Visible="false">Email Address:</asp:Label>
			    </td>
			    <td style="width: 350px"> 				
				    <telerik:RadComboBox ID="cboEmail" runat="server" Skin="Orchestrator" HighlightTemplatedItems="true" Width="350px" Height="100px"
				    OnClientSelectedIndexChanged="HandleEmailChanged" AllowCustomText="true" MarkFirstMatch="true"  Visible="false">
                        <HeaderTemplate>
                            <table style="width:345px; text-align:left;">
                                <tr><th style="width:50%;">Contact Name</th><th style="width:50%;">Contact Detail</th></tr>
                            </table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table style="width:345px; text-align:left;">
                                <tr>
                                    <td style="width:50%;">
                                        <%#DataBinder.Eval(Container.DataItem, "ContactName")%>
                                    </td>
                                    <td style="width:50%;">
                                        <%#DataBinder.Eval(Container.DataItem, "ContactDetail")%>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                
                    <asp:TextBox ID="txtEmail" Runat="server" style="display:none"/>
			    </td>
			    <td>
				    <asp:RequiredFieldValidator id="rfvEmailAddress" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="cboEmail" ValidationGroup="Email" Enabled="false"></asp:RequiredFieldValidator>
				    <asp:RegularExpressionValidator id="revEmailAddress" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtEmail" ValidationGroup="Email" Enabled="false"></asp:RegularExpressionValidator>
			    </td>
			    <td>
				    <nfvc:NoFormValButton id="btnEmailOneLiner" CssClass="buttonClass" runat="server" NoFormValList="rfvFaxNumber,revFaxNumber" Text="Email One Liner" ValidationGroup="Email" Width="140px"  Visible="false"></nfvc:NoFormValButton>
			    </td>
                <td>
                    <asp:Label ID="lblEmailSent" runat="server" Visible="false">Email sent.</asp:Label>
                </td>
            </tr>
        </table>
	</div>

    <iframe id="pdfViewer" runat="server" visible="false" src="" width="100%" height="1000" ></iframe>

    <telerik:RadCodeBlock runat="server">
        <script language="javascript" type="text/javascript">

            function HandleEmailChanged(item) {
                    var txtEmail = document.getElementById("<%=txtEmail.ClientID %>");

                    if (item.Value != "")
                        txtEmail.Text = item.Value;
                    else
                        txtEmail.Text = item.Text;
                }

            function ValidateInvoiceDate(source, args) {

                var dteDateTime = $find("<%= rdiInvoiceDate.ClientID %>");
                var dteDateTimeValue = dteDateTime.get_dateInput().get_selectedDate();

                //  Create Today's date
                var today = new Date();
                var upperBound = today.setDate(today.getDate() + 30);
                var today = new Date();
                var lowerBound = today.setDate(today.getDate() - 30);

                if (dteDateTimeValue > upperBound) {
                    var r = confirm('Warning. The selected Invoice date is more than 30 days in the future.');
                    if (r == true)
                        args.IsValid = true;
                    else {
                        args.IsValid = false;
                    }
                }

                if (dteDateTimeValue < lowerBound) {
                    var r = confirm('Warning. The selected Invoice date is more than 30 days in the past.');
                    if (r == true)
                        args.IsValid = true;
                    else {
                        args.IsValid = false;
                    }
                }
            }

        </script>
    </telerik:RadCodeBlock>
	
</asp:Content>