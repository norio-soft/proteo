<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.CreditNoteAuditTrail" Title="Credit Note Audit Trail" Codebehind="creditnoteaudittrail.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Credit Note Audit Trail</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" EnableViewState="true">
    <script language="javascript" type="text/javascript">
    	
  	function gridDblClick(gridItem)
  	{
  	    dgCreditNotes.Select(gridItem, false);
  	    dgCreditNotes.Postback();
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
    
    <input type="hidden" id="hidSelectedCreditNotes" runat="server" value="" />
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
		<asp:Button id="btnPost" enabled="true" runat="server" text="Post To Accounts" CausesValidation="False" 
		OnClick="btnPost_Click" OnClientClick="this.disabled=true;__doPostBack('ctl00$ContentPlaceHolder1$btnPost','');" ></asp:Button>
		<asp:Button id="btnExportToCSV" runat="server" text="Export To CSV" CausesValidation="False" OnClick="btnExportToCSV_Click"></asp:Button>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
	<fieldset>
	<legend>
			<strong>Audit Filtering Options</strong></legend>
		<table>
			<tr>
				<td class="formCellLabel">Client</td>
				<td class="formCellField">
                    <telerik:RadComboBox AllowCustomText="true" ID="cboClient" Overlay="true" height="400px" ZIndex="500"  AutoPostBack="true" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px"  ></telerik:RadComboBox>
                </td>
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
				<td class="formCellLabel">Date between</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteStartDate" runat="server" ToolTip="The start date of the Credit Note search" Width="100px" >
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
			</tr>
			<tr>
				<td class="formCellLabel">and</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteEndDate" runat="server" ToolTip="The end date of the Credit Note search" Width="100px" >
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
			</tr>	
			</table>
	</fieldset>
    	<div class="buttonbar">
		<asp:Button id="btnFilter" runat="server" text="Filter" CausesValidation="False" width="75" OnClick="btnFilter_Click"></asp:Button>

	</div>
    </div>

	
		<table width="100%">
			<tr>
				<td >
				     <asp:GridView ID="dgCreditNotes" runat="server" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="100%" visible="true">
                        <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
                        <rowStyle height="20" cssclass="Row" />
                        <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
                        <SelectedRowStyle height="20" cssclass="SelectedRow" />
                        <columns>
                            <asp:templatefield headerText=" ">
                                <itemtemplate>
                                    <input type="checkbox" <%#Eval("Posted").ToString()=="No"? "":"disabled"%>  onclick='updateCreditNotes(this, <%# Eval("CreditNoteId").ToString() %>, "<%# Eval("NetAmount").ToString() %>")' />
                                    <a id="lnkViewInvoice" runat="server" target="_blank">View Credit Note</a>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:templatefield headerText=" ">
                                <itemtemplate>
                                    <a id="lnkEditInvoice" runat="server" target="_blank">Edit Credit Note</a>
                                </itemtemplate>
                            </asp:templatefield>
                            <asp:boundfield headertext="Credit Note No" datafield="CreditNoteId" />
                            <asp:boundfield headertext="Client" datafield="OrganisationName" />
                            <asp:boundfield headertext="Credit Note Date" datafield="CreditNoteDate" htmlencode="false" DataFormatString="{0:dd/MM/yy}" />
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
                            <asp:boundfield headertext="Posted" datafield="Posted" />
                            <asp:boundfield headertext="Created By" datafield="CreateUserId" />
                            <asp:boundfield headertext="Created Date" datafield="CreateDate" htmlencode="false" DataFormatString="{0:dd/MM/yy}" />
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

    var hidSelectedCreditNotes = document.getElementById('<%=hidSelectedCreditNotes.ClientID %>');
    var lblEmptySelectionError = document.getElementById('<%=lblEmptySelectionError.ClientID %>');
    var pnlEmptySelectionError = document.getElementById('<%=pnlEmptySelectionError.ClientID %>');
    
    function updateCreditNotes(el, creditNoteId, netAmount)
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
                hidSelectedCreditNotes.value = hidSelectedCreditNotes.value + creditNoteId + ",";
            }
            else
                el.checked = false;
  	    }
  	    else
  	    {
  	        if (hidSelectedCreditNotes.value.substr(0, (new String(creditNoteId).length) + 1) == creditNoteId + ",")
			{
			    if (hidSelectedCreditNotes.value == (creditNoteId + ","))
			    {
			        hidSelectedCreditNotes.value = "";
			    }
			    else
			    {
			        hidSelectedCreditNotes.value = hidSelectedCreditNotes.value.substr((new String(creditNoteId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedCreditNotes.value.indexOf("," + creditNoteId + ",");
                hidSelectedCreditNotes.value = hidSelectedCreditNotes.value.substr(0, location + 1) + hidSelectedCreditNotes.value.substr(location + ("," + creditNoteId + ",").length);
			}
  	    }
  	}
  	
  	function hideEmptySelectionError()
    {
  	    lblEmptySelectionError.innerText = "";
        pnlEmptySelectionError.style.display = "none";
    }
    FilterOptionsDisplayHide();
</script>
</asp:Content>