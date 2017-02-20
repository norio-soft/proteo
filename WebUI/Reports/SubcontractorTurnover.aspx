<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubcontractorTurnover.aspx.cs" Inherits="Orchestrator.WebUI.Reports.SubcontractorTurnover" MasterPageFile="~/default_tableless.master" Title="Subcontractor Turnover Per Month" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Subcontractor Costs per Month</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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

        $(document).ready(function () {
            /*stops the table having a black background when viewed in a small window/screen*/
            var width = $(".rgMasterTable").width();
            $(".masterpagelite_layoutContainer").css("min-width", width + 20 + "px");
        });
   
  
</script>
    <h2>Subcontractor costs per month for the specified date range.</h2>
                 <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
             <asp:Button ID="btnExport" runat="server" Text="Export" Width="80px" ToolTip="Extract the data in a format you can open in Excel" CausesValidation="true" />

	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">   
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">Date From</td>
                <td class="formCellInput">
                    <telerik:RadDatePicker ID="dteStartDate" runat="server" Width="100px">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="Please enter a valid start date" ControlToValidate="dteStartDate" Display="Dynamic"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid start date" /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Date To</td>
                <td class="formCellInput">
                    <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100px">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ErrorMessage="Please enter a valid end date" ControlToValidate="dteEndDate" Display="Dynamic"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid end date" /></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvEndAfterStartDate" runat="server" ErrorMessage="Please ensure the end date is after the start date" ControlToValidate="dteEndDate" Display="Dynamic" EnableClientScript="false"><img src="/images/Error.gif" height="16" width="16" alt="Please ensure the end date is after the start date" /></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td class="formCellInput">
                    <asp:CheckBox ID="chkIncludeTotalsPerClient" runat="server" Text="Include Subcontractor Totals" Checked="true" />
                    <asp:CheckBox ID="chkIncludeTotalsPerMonth" runat="server" Text="Include Month Totals" Checked="true" />
                </td>
            </tr>
       </table>        
    </fieldset>
        <div id="buttonBar" runat="server" class="buttonBar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="80px" ToolTip="Requery Orchestrator with the dates above" CausesValidation="true" />
       
    </div>
    </div>

    
    <telerik:RadGrid runat="server" ID="grdClientTurnover" AllowPaging="false" AutoGenerateColumns="false" EnableViewState="false">
        <MasterTableView Width="100%">
            <RowIndicatorColumn Display="true"></RowIndicatorColumn>
        </MasterTableView>
        <ClientSettings />
    </telerik:RadGrid>
        <script language="javascript" type="text/javascript">
            FilterOptionsDisplayHide();
</script>
</asp:Content>