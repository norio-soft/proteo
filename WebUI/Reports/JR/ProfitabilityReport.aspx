<%@ Page Title="Profitability Report" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ProfitabilityReport.aspx.cs" Inherits="Orchestrator.WebUI.Reports.JR.ProfitabilityReport" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js"></script>    

    <style type="text/css">
        #depotFilterRow, #vehicleFilterRow, #clientFilterRow, #clientGroupByFilterRow {
            display: none;
        }

        .predefinedDateRange {
            margin-left: 4px;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Profitability Report</h1>
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
            <tr>
                <td class="formCellLabel">Report by</td>
                <td class="formCellField">
                    <asp:RadioButton ID="rbReportByDepot" runat="server" GroupName="rbRepotBy" ClientIDMode="Static" Text="Depot" />
                    <asp:RadioButton ID="rbReportByClient" runat="server" GroupName="rbRepotBy" ClientIDMode="Static" Text="Client" />
                    <asp:CustomValidator ID="cfvReportBy" runat="server" ErrorMessage="Please select whether to report by depot or client." ClientValidationFunction="cfvReportBy_Validate" Display="Dynamic">
                        <img src="/images/Error.gif" height="16" width="16" title="Please select whether to report by depot or client." />
                    </asp:CustomValidator>
                </td>
            </tr>
            <tr id="depotFilterRow">
                <td class="formCellLabel">Depots</td>
                <td class="formCellField">
					<telerik:RadListBox runat="server" ID="lbAvailableDepots" ClientIDMode="Static" Width="300" Height="300"
                        EnableDragAndDrop="true" AllowTransfer="true" TransferToID="lbSelectedDepots" AllowTransferDuplicates="false"
                        MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                        SelectionMode="Multiple" DataValueField="OrganisationLocationID" DataTextField="OrganisationLocationName"
                        AllowReorder="False" TabIndex="3" EnableMarkMatches="True" OnClientTransferred="depotTransferred"/>
                    <span style="padding: 0 5px; line-height: 20px;">Selected Depots</span>
                    <telerik:RadListBox runat="server" ID="lbSelectedDepots" ClientIDMode="Static" Width="300" Height="300" EnableDragAndDrop="true"
                        SelectionMode="Multiple" DataValueField="OrganisationLocationID" DataTextField="OrganisationLocationName"
                        AllowReorder="false" TabIndex="4" EnableMarkMatches="true" OnClientTransferred="depotTransferred" />
                </td>
            </tr>
            <tr id="vehicleFilterRow">
                <td class="formCellLabel">Vehicle</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboVehicle" runat="server" ClientIDMode="Static" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" Overlay="true" CausesValidation="false"  ShowMoreResultsBox="true" EnableVirtualScrolling="true"
                        OnClientItemsRequesting="cboVehicle_ItemsRequesting" OnClientItemsRequested="cboVehicle_ItemsRequested" >
                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetVehicles"/>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr id="clientFilterRow">
                <td class="formCellLabel">Clients</td>
                <td class="formCellField">
					<telerik:RadListBox runat="server" ID="lbAvailableClients" ClientIDMode="Static" Width="300" Height="400"
                        EnableDragAndDrop="true" AllowTransfer="true" TransferToID="lbSelectedClients" AllowTransferDuplicates="false"
                        MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                        SelectionMode="Multiple" DataValueField="IdentityId" DataTextField="OrganisationName"
                        AllowReorder="False" TabIndex="3" EnableMarkMatches="True" OnClientTransferred="clientTransferred"/>
                    <span style="padding: 0 5px; line-height: 20px;">Selected Clients</span>
                    <telerik:RadListBox runat="server" ID="lbSelectedClients" ClientIDMode="Static" Width="300" Height="400" EnableDragAndDrop="true"
                        SelectionMode="Multiple" DataValueField="IdentityId" DataTextField="OrganisationName"
                        AllowReorder="false" TabIndex="4" EnableMarkMatches="true" OnClientTransferred="clientTransferred" />
                </td>
            </tr>
            <tr id="clientGroupByFilterRow">
                <td class="formCellLabel">Group By</td>
                <td class="formCellField">
                    <asp:RadioButton ID="rbGroupByDeliveryPoint" runat="server" GroupName="rbGroupBy" ClientIDMode="Static" Text="Delivery Point" Checked="true" />
                    <asp:RadioButton ID="rbGroupByCollectionPoint" runat="server" GroupName="rbGroupBy" ClientIDMode="Static" Text="Collection Point" />
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

            $('#rbReportByDepot, #rbReportByClient').on('change', displayReportByFilters);
        });

        function pageLoad() {
            displayReportByFilters();
        }

        function setDates(startDate, endDate) {
            $find('<%= dteStartDate.ClientID %>').set_selectedDate(startDate);
            $find('<%= dteEndDate.ClientID %>').set_selectedDate(endDate);
        }

        function cfvReportBy_Validate(sender, eventArgs) {
            eventArgs.IsValid = $('#rbReportByDepot:checked, #rbReportByClient:checked').length > 0;
        }

        function cboVehicle_ItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var depotID = $find('lbSelectedDepots').get_items().getItem(0).get_value();
            context['FilterString'] = depotID;
            context['TopItemText'] = '- all -';
        }

        function cboVehicle_ItemsRequested(sender, eventArgs) {
            hideLoading();
            if (!sender.get_text()) {
                sender.get_items().getItem(0).select();
            }
        }

        function depotTransferred(sender, eventArgs) {
            var reportBySingleDepot = $find('lbSelectedDepots').get_items().get_count() == 1;
            $('#vehicleFilterRow').toggle(reportBySingleDepot);

            if (reportBySingleDepot) {
                showLoading('loading vehicles...');
                var cboVehicle = $find('cboVehicle');
                cboVehicle.clearSelection();
                cboVehicle.set_text('');
                cboVehicle.requestItems();
            }
        }

        function clientTransferred(sender, eventArgs) {
            var reportBySingleClient = $find('lbSelectedClients').get_items().get_count() == 1;
            $('#clientGroupByFilterRow').toggle(reportBySingleClient);
        }

        function displayReportByFilters() {
            var reportByDepot = $('#rbReportByDepot:checked').length > 0;
            $('#depotFilterRow').toggle(reportByDepot);
            var reportBySingleDepot = reportByDepot && $find('lbSelectedDepots').get_items().get_count() == 1;
            $('#vehicleFilterRow').toggle(reportBySingleDepot);
            var reportByClient = $('#rbReportByClient:checked').length > 0;
            $('#clientFilterRow').toggle(reportByClient);
            var reportBySingleClient = reportByClient && $find('lbSelectedClients').get_items().get_count() == 1;
            $('#clientGroupByFilterRow').toggle(reportBySingleClient);
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

