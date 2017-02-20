<%@ Page Title="Driver Time Report" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="DriverTimeReport.aspx.cs" Inherits="Orchestrator.WebUI.Reports.DriverTimeReport" %>

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
    <h1>Driver Time Report</h1>
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
            <tr><td colspan="2" class="formCellLabel"><span><b>Disclaimer: This report is based on non-verified driving time.</b></span><br/></td></tr>
            <tr>
                <td class="formCellLabel">Date</td>
                <td class="formCellField">
					<telerik:RadDatePicker ID="dteStartDate" runat="server" width ="100" ToolTip="The earliest date to report on." DateInput-DateFormat="dd/MM/yy" />
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
                    <a id="lnkToday" class="predefinedDateRange">Today</a>
                    <a id="lnkThisWeek" class="predefinedDateRange">This week</a>
                    <a id="lnkThisMonth" class="predefinedDateRange">This month</a>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Driver</td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboDriver" Filter="Contains" DropDownWidth="300px"></telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Calculated Driver Time</td>
                <td class="formCellField">
                    <telerik:RadNumericTextBox ID="txtHoursGreaterThan" runat="server" Type="Number" Width="40px" Value="0">
                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                    </telerik:RadNumericTextBox>
                    <asp:RadioButton ID="rbDefault" GroupName="calDriverTime" runat="server" Text="No Filter" Checked="True" />
                    <asp:RadioButton ID="rbDrive" GroupName="calDriverTime" runat="server" Text="Drive"/>
                    <asp:RadioButton ID="rbSpreadOver" GroupName="calDriverTime" runat="server" Text="Spreadover" />
                </td>
            </tr>
        </table>

        <div class="buttonbar">
            <asp:Button ID="btnReport" runat="server" Text="Generate Report" />
        </div>
    </div>

    <telerik:ReportViewer ID="reportViewer" runat="server" Height="800px" ViewMode="PrintPreview" Width="100%" ShowExportGroup="false" />

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
                var weekEnd = weekStartMoment.clone().add('days', 6).toDate();
                setDates(weekStart, weekEnd);
            });

            $('#lnkThisMonth').on('click', function () {
                var monthStart = moment().startOf('month').toDate();
                var monthEnd = moment().endOf('month').startOf('day').toDate();
                setDates(monthStart, monthEnd);
            });
        });

        function setDates(startDate, endDate) {
            $find('<%= dteStartDate.ClientID %>').set_selectedDate(startDate);
            $find('<%= dteEndDate.ClientID %>').set_selectedDate(endDate);
        }
    </script>
</asp:Content>

