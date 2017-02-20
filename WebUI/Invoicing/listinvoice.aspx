<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.ListInvoice" Codebehind="ListInvoice.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Find Invoice" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Find Invoice</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script language="javascript" type="text/javascript">
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
    <h2>Find an Invoice by completing any of the information below</h2>
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            		<input type="reset" value="Reset" />
		<asp:Button id="btnReport" runat="server" Text="Generate Report" visible="false"></asp:Button>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
	<fieldset>
		<legend>Filter Options</legend>

		<asp:RadioButtonList id="rdoFilterOptions" runat="server" RepeatDirection="Horizontal" AutoPostBack="True"></asp:RadioButtonList>					
		
        <asp:Panel id="pnlInvoiceId" runat="server">
		    <table cellpadding="0" cellspacing="0">
			    <tr>
				    <td class="formCellLabel" style="width: 125px;"><asp:Label id="lblInvoiceId" runat="server">Invoice Id</asp:Label></td>
				    <td class="formCellField"><telerik:RadTextBox AutoPostBack="true" TextMode="SingleLine" CausesValidation="true" runat="server" ID="txtInvoiceId"></telerik:RadTextBox></td>
				    <td class="formCellField">
                        <asp:CustomValidator id="cfvInvoiceId" ControlToValidate="txtInvoiceId" Runat="server" OnServerValidate="ValidateInvoiceId" Display="Static" ErrorMessage="Please provide a numeric value for Invoice ID" EnableClientScript="False"></asp:CustomValidator>
					    <asp:requiredfieldvalidator id="rfvInvoiceId" runat="server" ControlToValidate="txtInvoiceId" Display="Dynamic" ErrorMessage="Please enter an Invoice Id." text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>">
					        <img src="../images/Error.gif" height='16' width='14' title='Please enter an Invoice Id.' alt='Field is Required' />
                        </asp:requiredfieldvalidator>
                    </td>
			    </tr>
			</table>	
			<div>(Excluding Sub-Contractors/Self Bill Invoice No - use radio buttons to filter the ones you want.)</div>
		</asp:Panel>			
		
        <asp:Panel id="pnlFilter" runat="server" Visible="False">
		    <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="formCellLabel" style="width: 125px;"><asp:Label id="lblClient" runat="server">Client</asp:Label></td>
                    <td class="formCellField"><telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px"></telerik:RadComboBox></td>
                </tr>
                <tr>
                    <td class="formCellLabel"><asp:Label id="lblSubContractor" runat="server">Sub Contractor</asp:Label></td>
                    <td class="formCellField"><telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px"></telerik:RadComboBox></td>
                </tr>
            </table>
            <asp:panel id="pnlPosted" runat="server" visible="false">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="formCellLabel" style="width: 125px;">Include Posted </td>
                        <td class="formCellField">
                            <asp:RadioButtonList id="rdoPosted" runat="server" AutoPostBack="true" repeatdirection="Horizontal" ></asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </asp:panel>
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td class="formCellLabel" style="width: 125px;"><asp:Label id="lblJobState" runat="server">Job State</asp:Label></td>
                    <td class="formCellField"><asp:dropdownlist id="cboJobState" runat="server"></asp:dropdownlist></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Invoice Date From</td>
                    <td class="formCellField"><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
                    <td>
                        <asp:requiredfieldvalidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" Display="Dynamic" ErrorMessage="Please enter a from date." text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>">
					        <img src="../images/Error.gif" height='16' width='14' title='Please enter a from date.' alt='Field is Required' />
                        </asp:requiredfieldvalidator>
                    </td>
                    <td class="formCellLabel">Invoice Date To </td>
                    <td class="formCellField"><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
                    <td>
                        <asp:requiredfieldvalidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" Display="Dynamic" ErrorMessage="Please enter a to date." text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>">
					        <img src="../images/Error.gif" height='16' width='14' title='Please enter a to date.' alt='Field is Required' />
                        </asp:requiredfieldvalidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel"><asp:Label id="lblClientInvoiceNumber" visible="false" runat="server">Client Invoice Number</asp:Label></td>
                    <td class="formCellField"><asp:TextBox id="txtClientInvoiceNumber" runat="server" visible="false"></asp:TextBox></td>
                </tr>
            </table>
        </asp:Panel>

    </fieldset>
        <div class="buttonbar">
		<asp:button id="btnSearch" runat="server" cssclass="Button" text="Search"></asp:button>
	</div>
    </div>

	<div>

		<asp:Panel id="pnlNormalInvoice" runat="server" Visible="False">
            <asp:gridview id="gvInvoices" runat="server" AllowSorting="true" autogeneratecolumns="false" width="100%" enableviewstate="true" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid">
                <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
                <rowStyle  cssclass="Row" />
                <AlternatingRowStyle  backcolor="WhiteSmoke" />
                <SelectedRowStyle  cssclass="SelectedRow" />
                <columns>
                    <asp:templatefield headertext="Invoice No" >
                        <itemtemplate>
                            <a target="_invoice" href="<%# GetInvoiceLinkURL(((Orchestrator.eInvoiceFilterType)Enum.Parse(typeof(Orchestrator.eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", ""))))%><%#Eval("InvoiceId")%>"><%#Eval("InvoiceId")%></a>
                        </itemtemplate>
                    </asp:templatefield>
                    <asp:boundfield datafield="OrganisationName" visible="true" HeaderText="Client" />
                    <asp:boundfield datafield="InvoiceDate" visible="true" HeaderText="Invoice Date" dataformatstring="{0:dd/MM}" htmlencode="false" />
                    <asp:boundfield datafield="CreateUserId" visible="true" HeaderText="Created By" />
                    <asp:boundfield datafield="CreateDate" visible="true" HeaderText="Create Date" dataformatstring="{0:dd/MM}" htmlencode="false" />
                    <asp:templatefield headertext="Posted" itemstyle-width="12">
                        <itemtemplate>
                            <%# Eval("Posted").ToString() == "True" ? "Yes" : "No"%>
                        </itemtemplate>
                    </asp:templatefield>
                      <asp:templatefield headertext="Posted Date" >
                        <itemtemplate>
                            <%# Eval("Posted").ToString() == "True" && Eval("LastUpdateDate") != DBNull.Value ? Eval("LastUpdateDate") : ""%>
                        </itemtemplate>
                    </asp:templatefield>
                    <asp:boundfield datafield="SelfBillInvoiceNumber" visible="true" HeaderText="Client Invoice Number" />
                </columns>
            </asp:gridview>
		</asp:Panel>

		<asp:Panel id="pnlSubContractorInvoice" runat="server" Visible="False">
		    <asp:gridview id="gvSubContratorInvoice" runat="server" AllowSorting="true" autogeneratecolumns="false" width="100%" enableviewstate="true" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid">
                <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
                <rowStyle  cssclass="Row" />
                <AlternatingRowStyle  backcolor="WhiteSmoke" />
                <SelectedRowStyle  cssclass="SelectedRow" />
                <columns>
                     <asp:templatefield headertext="Invoice No" >
                        <itemtemplate>
                            <a target="_invoice" href="<%#Eval("PDFLocation")%>"><%#Eval("InvoiceId")%></a>
                        </itemtemplate>
                    </asp:templatefield>
                    <asp:templatefield headertext="Client Invoice No" >
                        <itemtemplate>
                            <a target="_invoice" href="<%#Eval("PDFLocation")%>"><%#Eval("ClientInvoiceNumber")%></a>
                        </itemtemplate>
                    </asp:templatefield>
                    <asp:boundfield datafield="DateReceived" visible="true" HeaderText="Date ReceivedDate" dataformatstring="{0:dd/MM/yy}" htmlencode="false" />
                    <asp:boundfield datafield="JobIdCSV" visible="true" HeaderText="Jobs Associated" />
                    <asp:boundfield datafield="Contractor" visible="true" HeaderText="Sub Contractor" />
                    <asp:boundfield datafield="InvoiceTotal" visible="true" HeaderText="Invoice Total" dataformatstring="{0:C}" />
                </columns>
            </asp:gridview>
		</asp:Panel>

	</div>

	<asp:Label id="lblNote" runat="server" Visible="False"></asp:Label>
	

	
    <script type="text/javascript" language="javascript">
	    var lastHighlightedRow = "";
	    var lastHighlightedRowColour = "";
	    var lastHighlightedRowClass = "";
    	
	    function HighlightRow(row)
	    {
		    var rowElement;
    		
		    if (lastHighlightedRow != "")
		    {
			    rowElement = document.getElementById(lastHighlightedRow);
			    rowElement.style.backgroundColor = lastHighlightedRowColour;
                rowElement.className = lastHighlightedRowClass;
    			
		    }

		    rowElement = document.getElementById(row);
		    lastHighlightedRow = row;
		    lastHighlightedRowColour = rowElement.style.backgroundColor;
		    lastHighlightedRowClass = rowElement.className;
		    rowElement.style.backgroundColor = "";	
		    rowElement.className = 'SelectRowTrafficSheetLite';
		}
		FilterOptionsDisplayHide();
	</script>

</asp:Content>