<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgencyDriver.aspx.cs" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.Reports.AgencyDriver" Title="Agency Driver Usage" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Agency Driver Usage</h1>
</asp:Content>

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
    <h2>Use this screen to retrieve a report showing agency driver usage for the specified date range.</h2>
    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: block;">Show filter Options</div>
        <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()" style="display: none;">Close filter Options</div>
        <asp:Button ID="btnExport" runat="server" Text="Export" Width="80px" ToolTip="Extract the data in a format you can open in Excel" CausesValidation="true" />

    </div>
    <!--Hidden Filter Options-->
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: none;">
        <fieldset>
            <legend>Agency Drivers</legend>
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
                    <td class="formCellLabel">Depot</td>
                    <td class="formCellInput">
                        <telerik:RadComboBox ID="cboDepot" runat="server" Skin="WindowsXP" Width="300" radcontrolsdir="~/script/RadControls/">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        All Depots

                    </td>
                    <td class="formCellInput">
                        <asp:CheckBox ID="chkAllDepots" runat="server" onchange="chkAllDepotsChanged()" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div id="buttonBar" runat="server" class="buttonBar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="80px" ToolTip="Requery Orchestrator with the dates above" CausesValidation="true" />

        </div>
    </div>

    <%-- Grid --%>
    <telerik:RadGrid runat="server" ID="grdAgencyDriver" AllowPaging="false" AutoGenerateColumns="false" EnableViewState="false">
        <MasterTableView Width="100%" NoMasterRecordsText="No data for the specified date range."
            DataKeyNames="ResourceId"
            ClientDataKeyNames="ResourceId" 
            ItemType="Orchestrator.Repositories.DTOs.DriverAgencyReportRow">
        <Columns>
            <telerik:GridBoundColumn HeaderText="Agency" DataField="AgencyName" UniqueName="AgencyName" HeaderStyle-Width="17%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Driver Name" DataField="FullName" UniqueName="DriverName" HeaderStyle-Width="17%">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Payroll No" DataField="PayrollNo">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Depot" DataField="DepotName" UniqueName="DepotName">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Number of Jobs" DataField="JobCount" UniqueName="JobCount" HeaderStyle-Width="10%">
            </telerik:GridBoundColumn>
        </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <script type="text/javascript">
        chkAllDepotsChanged();

        function chkAllDepotsChanged(sender, eventArgs) {
            var allDepotsChkBox = $(<%=chkAllDepots.ClientID %>)[0];
            var cboDepot = $find("<%=cboDepot.ClientID %>");
            if (allDepotsChkBox.checked) {
                cboDepot.disable();
            }
            else {
                cboDepot.enable();
            }
        }
    </script>

</asp:Content>
