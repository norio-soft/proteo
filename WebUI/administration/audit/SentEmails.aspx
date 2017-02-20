<%@ Page Title="Haulier Enterprise" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="SentEmails.aspx.cs" Inherits="Orchestrator.WebUI.administration.audit.SentEmails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

<script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
<script type="text/javascript">
    $(document).ready(function () {

        $('#<%=grdEmails.ClientID %>_ctl00 tbody tr').quicksearch({
            position: 'after',
            attached: '#grdFilterHolder',
            labelText: '',
            inputClass: 'input_Class',
            delay: 100
        });

    });

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
 <style type="text/css">
        .input_Class 
        {
            padding:5px;
            font-size:8pt;
            left: 230px;
            position: absolute;
            top: 115px;
            
        }
        
        
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Sent Emails</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<h2>Below is a list of sent emails</h2>

	<asp:Label id="lblConfirmation" runat="server" visible="false" cssclass="confirmation"></asp:Label>

                    <div class="ToolbarBlueWithTextBox" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <div style="padding-top: 5px"><asp:Label ID="lblQF" runat="server" Text="Quick Filter:" /></div>
            <div id="grdFilterHolder"></div>
            
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
	<fieldset>
		<legend>Filter Options</legend>

		<table id="mainTable" runat="server">
			<tr>
				<td class="formCellLabel">Start Date</td>
				<td class="formCellField">
                    <telerik:RadDatePicker ID="dteStartDate" runat="server" DateInput-DateFormat="dd/MM/yy" Width="8em" />
					<asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please select a start date." Display="Dynamic">
                        <img src="../../images/Error.gif" height="16" width="16" alt="Error" title="Please enter a start date." />
                    </asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">End Date</td>
				<td class="formCellField">
                    <telerik:RadDatePicker ID="dteEndDate" runat="server" DateInput-DateFormat="dd/MM/yy" Width="8em" />
					<asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please select an end date." Display="Dynamic">
                        <img src="../../images/Error.gif" height="16" width="16" title="Please enter an end date." alt="error" />
                    </asp:RequiredFieldValidator>
				</td>
			</tr>
            <tr>
                <td class="formCellLabel">Report Type</td>
                <td class="formCellField"><telerik:RadComboBox runat="server" ID="cboReportType" EmptyMessage="All" Width="150"></telerik:RadComboBox></td>
            </tr>
            <tr>
                <td class="formCellLabel">Email Address</td>
                <td class="formCellField"><telerik:RadTextBox runat="server" ID="txtEmailAddress" EmptyMessage="Email Address or part of"></telerik:RadTextBox>
                </td>
            </tr>
		</table>
	</fieldset>
    	<div class="buttonbar">
		<asp:Button id="btnFilter" width="75px" runat="server" Text="Filter"></asp:Button>
	</div>
    </div>



	
        
		<telerik:RadGrid ID="grdEmails" runat="server" AutoGenerateColumns="false">
			<MasterTableView DataKeyNames="EmailID" NoMasterRecordsText="There are no matching sent emails">
				<Columns>
					<telerik:GridBoundColumn UniqueName="ReportType" DataField="ReportType" HeaderText="Report Type" />
					<telerik:GridBoundColumn UniqueName="Recipient" DataField="Recipient" HeaderText="Recipient(s)" />
					<telerik:GridBoundColumn UniqueName="UserName" DataField="UserName" HeaderText="Sent By" />
					<telerik:GridBoundColumn UniqueName="CreateDate" DataField="CreateDate" HeaderText="Sent" />
					<telerik:GridBoundColumn UniqueName="FirstViewedDateTime" DataField="FirstViewedDateTime" HeaderText="First Viewed" />
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
        <script type="text/javascript">
            FilterOptionsDisplayHide();
        </script>
</asp:Content>
