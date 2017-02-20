<%@ Page Title="Profitability BY Planner Report" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ProfitabilityByPlannerReport.aspx.cs" Inherits="Orchestrator.WebUI.Reports.JR.ProfitabilityByPlannerReport" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js"></script>    

    <style type="text/css">
        #driverFilterRow {
            display: none;
        }

        .predefinedDateRange {
            margin-left: 4px;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Profitability By Planner Report</h1>
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
					<telerik:RadDatePicker ID="dteStartDate" runat="server" width ="100" ToolTip="The earliest date to report on." DateInput-DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic">
                        <img src="/images/Error.gif" height="16" width="16" title="Please specify a start date." />
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date." Display="Dynamic">
                        <img src="/images/Error.gif" height="16" width="16" title="The start date must be before the end date." />
                    </asp:CustomValidator>
                    to
                    <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100" ToolTip="The last date to report on." DateInput-DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date." Display="Dynamic">
                        <img src="/images/Error.gif" height="16" width="16" title="Please specify an end date." />
                    </asp:RequiredFieldValidator>
                    <a id="lnkLastWeek" class="predefinedDateRange">Last week</a>
                    <a id="lnkLastMonth" class="predefinedDateRange">Last month</a>
                </td>
            </tr>
            <tr id="depotFilterRow">
                <td class="formCellLabel">Planners</td>
                <td class="formCellField">
					<telerik:RadListBox runat="server" ID="lbAvailablePlanners" ClientIDMode="Static" Width="300" Height="300"
                        EnableDragAndDrop="true" AllowTransfer="true" TransferToID="lbSelectedPlanners" AllowTransferDuplicates="false"
                        MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                        SelectionMode="Multiple" DataValueField="IdentityID" DataTextField="FullName"
                        AllowReorder="False" TabIndex="3" EnableMarkMatches="True" OnClientTransferred="plannerTransferred"/>
                    <span style="padding: 0 5px; line-height: 20px;">Selected Planners</span>
                    <telerik:RadListBox runat="server" ID="lbSelectedPlanners" ClientIDMode="Static" Width="300" Height="300" EnableDragAndDrop="true"
                        SelectionMode="Multiple" DataValueField="IdentityID" DataTextField="FullName"
                        AllowReorder="false" TabIndex="4" EnableMarkMatches="true" OnClientTransferred="plannerTransferred" />
                </td>
            </tr>
        
        <tr id="driverFilterRow">
                <td class="formCellLabel">Driver</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboDriver" runat="server" ClientIDMode="Static" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="false" Overlay="true" CausesValidation="false"  ShowMoreResultsBox="true" EnableVirtualScrolling="true"
                        OnClientItemsRequesting="cboDriver_ItemsRequesting" OnClientItemsRequested="cboDriver_ItemsRequested" Width="270px" >
                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetDriversForPlanner"/>
                    </telerik:RadComboBox>
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

        // Without this, anything other than 100% Zoom on Chrome will crash the screen on Post-Back
        // This will be fixed in Q3 2014 SP1 and can be removed from this solution when upgraded
        // http://www.telerik.com/forums/system-formatexception-input-string-was-not-in-a-correct-format-thrown-on-chrome-when-browser-is-zoomed-in-out
        Telerik.Web.UI.RadListBox.prototype.saveClientState = function () {
            return "{" +
            "\"isEnabled\":" + this._enabled +
            ",\"logEntries\":" + this._logEntriesJson +
            ",\"selectedIndices\":" + this._selectedIndicesJson +
            ",\"checkedIndices\":" + this._checkedIndicesJson +
            ",\"scrollPosition\":" + Math.round(this._scrollPosition) +
            "}";
        }

        $(function () {
            $(':checkbox[id$=chkSelectAllTrafficAreas]').on('click', function () {
                $(':checkbox[id$=cboTrafficAreas]').prop('checked', this.checked);
            });

            $('#filterOptionsDiv').on('click', function () { filterOptionsDisplayToggle(true); });
            $('#filterOptionsDivHide').on('click', function () { filterOptionsDisplayToggle(false); });

            $('#lnkLastWeek').on('click', function () {
                var weekStartDay = parseInt('<%= this.WeekStartDay %>', 10);
                var weekStartMoment = moment().startOf('day').subtract('days', ((7 + moment().day() - weekStartDay) % 7) + 7);
                var weekStart = weekStartMoment.toDate();
                var weekEnd = weekStartMoment.clone().add('days', 6).toDate();
                setDates(weekStart, weekEnd);
            });

            $('#lnkLastMonth').on('click', function () {
                var monthStart = moment().startOf('month').subtract('months', 1).toDate();
                var monthEnd = moment().startOf('month').subtract('days', 1).toDate();
                setDates(monthStart, monthEnd);
            });

        });

     

        function setDates(startDate, endDate) {
            $find('<%= dteStartDate.ClientID %>').set_selectedDate(startDate);
            $find('<%= dteEndDate.ClientID %>').set_selectedDate(endDate);
        }

        function cfvReportBy_Validate(sender, eventArgs) {
            eventArgs.IsValid = $('#rbReportByDepot:checked, #rbReportByClient:checked').length > 0;
        }

        function cboDriver_ItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var plannerID = $find('lbSelectedPlanners').get_items().getItem(0).get_value();
            context['FilterString'] = plannerID;
            context['TopItemText'] = '- all -';
        }

        function cboDriver_ItemsRequested(sender, eventArgs) {
            hideLoading();
            if (!sender.get_text()) {
                sender.get_items().getItem(0).select();
            }
        }

        function plannerTransferred( sender, eventArgs ) {
            var reportBySinglePlanner = $find('lbSelectedPlanners').get_items().get_count() == 1;
            $( '#driverFilterRow' ).toggle( reportBySinglePlanner );

            if ( reportBySinglePlanner ) {
                showLoading('loading drivers...');
                var cboDriver = $find('cboDriver');
                cboDriver.clearSelection();
                cboDriver.set_text( '' );
                cboDriver.requestItems();
                
            }
        }

     

        function showLoading(msg) {
            $.blockUI({
                message: '<h1 style="color: #fff">' + (msg || 'loading...') + '</h1>',
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#444',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    'border-radius': '10px'
                }
            });
        };

        function hideLoading() {
            $.unblockUI();
        };
    </script>
</asp:Content>

