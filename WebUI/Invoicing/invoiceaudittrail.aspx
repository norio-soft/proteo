<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.InvoiceAuditTrail" Codebehind="InvoiceAuditTrail.aspx.cs" MasterPageFile="~/default_tableless.Master"  AutoEventWireup="True" Title ="Invoice Audit Trail" %>

<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
          function gridDblClick(gridItem) {
            dgInvoices.Select(gridItem, false);
            dgInvoices.Postback();


        }
        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Invoice Audit Trail</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <h2>You can view your invoices and produce a report detailling the invoiced jobs below</h2>
    <input type="hidden" id="hidSelectedInvoices" runat="server" value="" />
    <input type="hidden" id="hidNonPDFInvoiceSelectedCount" runat="server" value="0" />

    <asp:panel ID="pnlError" runat="server" Visible="false">
        <div style="margin-top: 10px;  font-family: Trebuchet MS, Arial, Helvetica;    font-size: 1em;    font-weight: bold;	background-color:#FDE8E9;	border:2px solid #9E0B0E;	padding:2px; 	color: #9E0B0E;">
            <img src="../images/status-red.gif"  align="middle"/><asp:Label ID="lblError" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <asp:panel ID="pnlEmptySelectionError" runat="server" style="display: none">
        <div style="margin-top: 10px;  font-family: Trebuchet MS, Arial, Helvetica;    font-size: 1em;    font-weight: bold;	background-color:#FDE8E9;	border:2px solid #9E0B0E;	padding:2px; 	color: #9E0B0E;">
            <img src="../images/status-red.gif"  align="middle"/><asp:Label ID="lblEmptySelectionError" runat="server"></asp:Label>
        </div>
    </asp:panel>
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            			<input type="button" onclick="loadInvoicesToPrint();" value="Print" style="width:75px;" />
			<asp:Button id="btnPost" runat="server" text="Post To Accounts" 
			OnClientClick="this.disabled=true;__doPostBack('btnPost','');" CausesValidation="False" ></asp:Button>
			<asp:Button id="btnExportToCSV" runat="server" text="Export To CSV" CausesValidation="False"></asp:Button>
            <asp:Button id="btnExportToCSVWithDetail" OnClientClick="return ExportToCsvDetailClick()" runat="server" text="Export To CSV With Detail" CausesValidation="False"></asp:Button>
            <asp:Button id="btnEmailInvoices" runat="server" text="Email" CausesValidation="False"></asp:Button>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
            <asp:Panel DefaultButton="btnFilter" ID="pnlFilter" runat ="server">
	<fieldset>
	<legend>
			<strong>Audit Filtering Options</strong></legend>
	<div align="left">
		<table>
			<tr>
				<td style="width:120px;" class="formCellLabel" valign="top">Client</td>
				<td class="formCellField">
                    <telerik:RadComboBox AllowCustomText="true" ID="cboClient" Overlay="true" height="400px" ZIndex="500"  AutoPostBack="false" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px"  ></telerik:RadComboBox>
                </td>
			</tr>
			<tr>
				<td class="formCellLabel" valign="top">Invoice Type</td>
				<td class="formCellField"><asp:DropDownList id="cboInvoiceType" runat="server"></asp:DropDownList></td>
			</tr>
			<tr>
			    <td class="formCellLabel" valign="top">Date Type</td>
				<td class="formCellField">
				    <asp:RadioButtonList ID="rblDateType" runat="server" RepeatDirection="Horizontal" RepeatColumns="2">
				        <asp:ListItem Text="Create Date" Value="0" />
				        <asp:ListItem Text="Invoice Date" Value="1" />
				    </asp:RadioButtonList>
				</td>
			</tr>
            <tr>
                <td class="formCellLabel" valign="top">Show Cancelled Invoices</td>
                <td class="formCellField">
                    <asp:CheckBox ID="chkShowCancelled" runat="server" Checked="false" />
                </td>
            </tr>
                        <tr>
                <td class="formCellLabel" valign="top">Show Posted Invoices</td>
                <td class="formCellField">
                    <asp:CheckBox ID="chkShowPosted" runat="server" Checked="true" />
                </td>
            </tr>
			<tr>
				<td class="formCellLabel" valign="top">Date between</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteInvoiceStartDate" runat="server" ToolTip="The start date of the invoice search" Width="100px" >
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
			</tr>
			<tr>
				<td class="formCellLabel" valign="top">and</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteInvoiceEndDate" runat="server" ToolTip="The end date of the invoice search" Width="100px" >
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
			</tr>
            <tr>
				    <td class="formCellLabel" valign="top">or Invoice Id</td>
				    <td class="formCellField"><telerik:RadTextBox TextMode="SingleLine" CausesValidation="true" runat="server" ID="txtInvoiceId"></telerik:RadTextBox></td>
				    <td class="formCellField">
                        <asp:CustomValidator id="cfvInvoiceId" ControlToValidate="txtInvoiceId" Runat="server" OnServerValidate="cfvInvoiceID_ServerValidate" Display="Static" ErrorMessage="Please provide a numeric value for Invoice ID" EnableClientScript="False" ValidationGroup="vgFilter"></asp:CustomValidator>
                    </td>
			    </tr>	
			</table>
	</div>
	</fieldset>
            </asp:Panel>
            
    		<div class="buttonbar">
			<asp:Button id="btnFilter" runat="server" text="Filter" CausesValidation="True" width="75" ValidationGroup="vgFilter"></asp:Button>

		</div>
    </div>

	
		<table width="100%">
			<tr>
				<td >
				     <asp:GridView ID="dgInvoices" runat="server" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="100%" visible="true">
                        <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
                        <rowStyle height="20" cssclass="Row" />
                        <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
                        <SelectedRowStyle height="20" cssclass="SelectedRow" />
                        <columns>                            
                            <asp:TemplateField HeaderText=" ">
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelectAll" onclick="javascript:selectAllCheckboxes(this);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <input type="checkbox" id="chkInvoice" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                            <asp:templatefield headertext="Invoice No" SortExpression="InvoiceID">
                            <itemtemplate>
                               <a id="lnkInvoiceNo" runat="server" target="_blank"><%# Eval("InvoiceID").ToString()%></a>
                            </itemtemplate>
                            </asp:templatefield>
                            <asp:boundfield headertext="Client" SortExpression="OrganisationName" datafield="OrganisationName" />
                            <asp:templatefield headerText="Email">
                                <itemtemplate>
                                    <input runat="server" id="chkEmail" type="checkbox" />
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:boundfield headertext="Account" datafield="AccountCode" SortExpression="AccountCode" />
                            <asp:boundfield headertext="Type" datafield="InvoiceType" SortExpression="InvoiceType" />
                            <asp:boundfield headertext="Date" datafield="InvoiceDate" htmlencode="false" DataFormatString="{0:dd/MM/yy}" />
                            <asp:templatefield headertext="Net">
                                <itemtemplate>
                                    <asp:Label id="netCurrencyLabel" runat="server" text=""></asp:Label>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:templatefield headertext="VAT">
                                <itemtemplate>
                                    <asp:Label id="VATCurrencyLabel" runat="server" text=""></asp:Label>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:templatefield headertext="Gross">
                                <itemtemplate>
                                    <asp:Label id="grossCurrencyLabel" runat="server" text=""></asp:Label>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:templatefield headertext="Extra">
                                <itemtemplate>
                                    <asp:Label id="extraCurrencyLabel" runat="server" text=""></asp:Label>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:templatefield headertext="Fuel Surcharge">
                                <itemtemplate>
                                    <asp:Label id="fuelCurrencyLabel" runat="server" text=""></asp:Label>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:boundfield headertext="Client Reference" datafield="ClientReferenceNo" SortExpression="ClientReferenceNo" />
                            <asp:boundfield headertext="Created By" datafield="CreateUserId" />
                            <asp:boundfield headertext="Created Date" datafield="CreateDate" htmlencode="false" DataFormatString="{0:dd/MM/yy}" />
                            <asp:boundfield headertext="Posted" datafield="Posted" />
                            <asp:boundfield headertext="Posted By" datafield="PostedBy" />
                            <asp:boundfield headertext="Transaction Id" datafield="TransactionId" />
                        </columns>
                    </asp:GridView>                
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<uc1:reportviewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%"></uc1:reportviewer>
				</td>
			</tr>
		</table>

<script language="javascript" type="text/javascript">

    var hidSelectedInvoices = document.getElementById('<%=hidSelectedInvoices.ClientID %>');
    var lblEmptySelectionError = document.getElementById('<%=lblEmptySelectionError.ClientID %>');
    var pnlEmptySelectionError = document.getElementById('<%=pnlEmptySelectionError.ClientID %>');
    var hidNonPDFInvoiceSelectedCount = document.getElementById('<%=hidNonPDFInvoiceSelectedCount.ClientID %>');
    
    function loadInvoicesToPrint()
    {
        hideEmptySelectionError();

        var invoiceIdCSV = hidSelectedInvoices.value;
        
        if (invoiceIdCSV.substr(invoiceIdCSV.length-1 , 1) == ",")
            invoiceIdCSV = invoiceIdCSV.substr(0, invoiceIdCSV.length - 1);

        if (invoiceIdCSV.length == "")
        {
            lblEmptySelectionError.innerText = "Can not print: no invoices selected";
            pnlEmptySelectionError.style.display = "block";
        }
        else {
            if (hidNonPDFInvoiceSelectedCount.value > 0) {
                sure = confirm("You have selected some Extra and/or One-Liner type invoices that do not have a PDF attached. It is not currently possible to print these types of invoices from this screen. Please go to one liner screen and update to generate the PDF then try again.");
            }
            else {

                var url = "printmultipleinvoices.aspx?i=" + invoiceIdCSV;
                //var url = "multiaddupdateinvoice.pdfx?ShowReport=true&InvoiceIdCSV=" + invoiceIdCSV;
                window.open(url);
            }
        }
    }
    
    function updateInvoices(el, invoiceId, netAmount, pdflocation)
  	{
  	    hideEmptySelectionError();
  	    
  	    if (el.checked)
  	    {
  	        var sure = false;
  	        if (netAmount == '0.0000')
  	            sure = confirm("This value has zero amount, if this is not correct print the invoice before posting.  Press OK to confirm selection, Cancel to deselect this invoice.");
            else
  	            sure = true;

            if (sure)
            {
                // We've selected this item
                hidSelectedInvoices.value = hidSelectedInvoices.value + invoiceId + ",";

                if(pdflocation == 0)
                    hidNonPDFInvoiceSelectedCount.value = (parseInt(hidNonPDFInvoiceSelectedCount.value) + 1) || 1;
            }
            else
                el.checked = false;
  	    }
  	    else
  	    {
  	        if (hidSelectedInvoices.value.substr(0, (new String(invoiceId).length) + 1) == invoiceId + ",")
			{
			    if (hidSelectedInvoices.value == (invoiceId + ","))
			    {
			        hidSelectedInvoices.value = "";
			    }
			    else
			    {
			        hidSelectedInvoices.value = hidSelectedInvoices.value.substr((new String(invoiceId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedInvoices.value.indexOf("," + invoiceId + ",");
                hidSelectedInvoices.value = hidSelectedInvoices.value.substr(0, location + 1) + hidSelectedInvoices.value.substr(location + ("," + invoiceId + ",").length);
            }

            if (pdflocation == 0)
                hidNonPDFInvoiceSelectedCount.value = (parseInt(hidNonPDFInvoiceSelectedCount.value) - 1) || 0;
  	    }
  	}
  	
  	function hideEmptySelectionError()
    {
  	    lblEmptySelectionError.innerText = "";
        pnlEmptySelectionError.style.display = "none";
    }
    FilterOptionsDisplayHide();


    function selectAllCheckboxes(chkbox) {
        $('table[id*=dgInvoices] input:enabled[id*=chkInvoice]').each(function () {
            this.click();
        });
    }

    function ExportToCsvDetailClick() {

        var canExecute = false;

        $('table[id*=dgInvoices] input:enabled[id*=chkInvoice]').each(function () {
            if (this.checked == 1) {
                canExecute = true;
            }
            else {
                canExecute == false;
                //alert('At least one Invoice must be selected');
            }


        });

        if (canExecute == false) {
            alert('At least one Invoice must be selected');
        }

        return canExecute;

    }

</script>


</asp:Content>
