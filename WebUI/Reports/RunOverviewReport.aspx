<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="RunOverviewReport.aspx.cs" Inherits="Orchestrator.WebUI.Reports.RunOverviewReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
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
        $(":checkbox[id*='chkSelectAllTrafficAreas']").click(function () {
            var checked_status = this.checked;
            $(":checkbox[id*='cboTrafficAreas']").each(function () {
                this.checked = checked_status;

            });
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Run Overview Report</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()"
            style="display: none;">
            Show filter Options</div>
        <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">
            Close filter Options</div>
    </div>
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
    <fieldset>
        <table>
            <tr>
                <td class="formCellLabel">
                    Traffic Area(s)
                </td>
                <td class="formCellField checkboxListField" colspan="4" style="width: 550px !important;">
                    <div runat="server" id="divTrafficAreas">
                        <asp:CheckBox runat="server" ID="chkSelectAllTrafficAreas" Text="Select all Traffic Areas" Checked="True" />
                        <asp:CheckBoxList ID="cblTrafficAreas" runat="server" DataValueField="TrafficAreaId"
                            DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="5" />
                    </div>
                </td>
            </tr>
            <tr>
                    <td class="formCellLabel">Run State</td>
                    <td class="formCellField">
                        <asp:CheckBoxList id="cblJobStates" runat="server" RepeatDirection="Horizontal" RepeatColumns="5" DataValueField="key" DataTextField="value"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                <td class="formCellLabel">Booked Start Date</td>
                <td class="formCellField">
					<telerik:RadDatePicker id="dteStartDate" runat="server" width ="100" ToolTip="The earliest date to report on.">
                    <DateInput ID="DateInput1" runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>	
				</td>
                <td class="formCellField">
                <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." /></asp:RequiredFieldValidator>
                <asp:CustomValidator id="cfvStartDate" runat="server" OnServerValidate="cfvStartDate_ServerValidate" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date."><img src="../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
                </td>
                </tr>
                <tr>
                <td class="formCellLabel">Booked End Date</td>
                <td class="formCellField">
                <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100" ToolTip="The last date to report on.">
                <DateInput ID="DateInput2" runat ="server" DateFormat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker>
                </td>
                <td class="formCellLabel">
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify an end date." /></asp:RequiredFieldValidator>
                </td>
                </tr>
        </table>
        </fieldset>
        <div class="buttonbar">
        <asp:Button id="btnReport" runat="server" Text="Generate Report" />
        </div>
    </div>
    <telerik:ReportViewer ID="rptvRunOverview" runat="server" Height="800px" ViewMode="PrintPreview"
        Width="100%">
    </telerik:ReportViewer>
       <script type="text/javascript">
           FilterOptionsDisplayHide();
    </script>
</asp:Content>

