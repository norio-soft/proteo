<%@ Page Title="Revenue per Vehicle Report" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="RevenuePerVehicleReport.aspx.cs" Inherits="Orchestrator.WebUI.Report.RevenuePerVehicleReport" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>

    <style type="text/css">
        .predefinedDateRange {
            margin-left: 4px;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Revenue per Vehicle Report</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <div class="overlayedFilterIconOff" id="filterOptionsDiv" style="display: none;">
            Show filter options
        </div>
        <div class="overlayedFilterIconOn" id="filterOptionsDivHide">
            Close filter options
        </div>
        <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" />
        <asp:Button ID="btnGenerateCsv" runat="server" Text="Generate CSV" />
        <asp:Button ID="btnGeneratePdf" runat="server" Text="Generate PDF" />
    </div>

    <div class="overlayedFilterBox" id="overlayedClearFilterBox">

        <table>            
            <tr>
                <td class="formCellLabel">Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker ID="dteStartDate" runat="server" Width="100" ToolTip="The earliest date to report on." DateInput-DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic">
                        <img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." />
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvStartDate" runat="server" OnServerValidate="cfvStartDate_ServerValidate" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date." Display="Dynamic">
                        <img src="../images/Error.gif" height="16" width="16" title="The start date must be before the end date." />
                    </asp:CustomValidator>
                    to
                    <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100" ToolTip="The last date to report on." DateInput-DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date." Display="Dynamic">
                        <img src="../images/Error.gif" height="16" width="16" title="Please specify an end date." />
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvEndDate" runat="server" OnServerValidate="cfvEndDate_ServerValidate" ControlToValidate="dteEndDate" ErrorMessage="The end date must be today or earlier." Display="Dynamic">
                        <img src="../images/Error.gif" height="16" width="16" title="The end date must be today or earlier." />
                    </asp:CustomValidator>
                    <a id="lnkToday" class="predefinedDateRange">Today</a>
                    <a id="lnkThisWeek" class="predefinedDateRange">This week</a>
                    <a id="lnkThisMonth" class="predefinedDateRange">This month</a>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Depot</td>
                <td class="formCellField" colspan="2">
                    <telerik:RadComboBox ID="cboDepot" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" Enabled="True"
                        Skin="WindowsXP" Width="355px"
                        OnSelectedIndexChanged="cboDepot_SelectedIndexChanged" AutoPostBack="True">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Vehicle</td>
                <td class="formCellField" colspan="2">
                    <telerik:RadComboBox ID="cboVehicle" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" Enabled="False"
                        Skin="WindowsXP" Width="355px"
                        OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" AutoPostBack="True">
                    </telerik:RadComboBox>
                </td>
            </tr>
        </table>

        <div class="buttonbar">
            <asp:Button ID="btnReport" runat="server" Text="Generate Report" />
        </div>
    </div>

    <telerik:ReportViewer ID="reportViewer" runat="server" Height="800px" 
        ViewMode="PrintPreview" Width="100%" ShowExportGroup="false" />

    <script type="text/javascript">
        function filterOptionsDisplayToggle(show) {
            $('#overlayedClearFilterBox').toggle(show);
            $('#filterOptionsDiv').toggle(!show);
            $('#filterOptionsDivHide').toggle(show);
        }

        $(function () {
            $(':checkbox[id$=chkSelectAllTrafficAreas]').on('click', function () {
                $(':checkbox[id$=cboTrafficAreas]').prop('checked', this.checked);
            });

            $('#filterOptionsDiv').on('click', function () { filterOptionsDisplayToggle(true); });
            $('#filterOptionsDivHide').on('click', function () { filterOptionsDisplayToggle(false); });

            $('#lnkToday').on('click', function () {
                var today = moment().startOf('day').toDate();
                setDates(today, today);
            });

            $('#lnkThisWeek').on('click', function () {
                var weekStartDay = parseInt('<%= this.WeekStartDay %>', 10);
                var weekStartMoment = moment().startOf('day').subtract('days', (7 + moment().day() - weekStartDay) % 7);
                var weekStart = weekStartMoment.toDate();
                var weekEnd = moment().startOf('day').toDate();
                setDates(weekStart, weekEnd);
            });

            $('#lnkThisMonth').on('click', function () {
                var monthStart = moment().startOf('month').toDate();
                var monthEnd = moment().startOf('day').toDate();
                setDates(monthStart, monthEnd);
            });
        });

        function setDates(startDate, endDate) {
            $find('<%= dteStartDate.ClientID %>').set_selectedDate(startDate);
            $find('<%= dteEndDate.ClientID %>').set_selectedDate(endDate);
        }
    </script>
</asp:Content>

