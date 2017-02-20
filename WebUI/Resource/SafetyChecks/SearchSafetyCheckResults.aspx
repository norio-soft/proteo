<%@ Page Language="C#" Inherits="Orchestrator.WebUI.Resource.SafetyChecks.Search" Title="Search Safety Checks" CodeBehind="SearchSafetyCheckResults.aspx.cs" MasterPageFile="~/default_tableless.Master" %>

    <asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
        <h1>Search Safety Checks</h1>
    </asp:Content>

    <asp:Content ContentPlaceHolderID="Header" runat="server">
        <script type="text/javascript">
            function viewResults(driverId, checkedDate) {
                var targetUrl = "/Resource/SafetyChecks/ViewSafetyCheckData.aspx?dID=" + driverId + "&cDate=" + checkedDate;
                openNewPopupWindow("Safety Check Results", targetUrl);
            }

            function openNewPopupWindow(winName, url) {
                var newWindow = window.open(url, winName, 'height=700,width=980,toolbar=no,menu=no,scrollbars=yes');
                if (window.focus) newWindow.focus();
            }
        </script>
    </asp:Content>

    <asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <h2>Search Safety Checks</h2>

    <fieldset>
        <legend>
            <strong>Search Criteria</strong>
        </legend>
        <table>
            <tr>
                <td class="formCellLabel">From Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="txtEffectiveDateFrom" runat="server" dateformat="dd/MM/yy"></telerik:RadDatePicker>
                </td>
                <td class="formCellLabel">To Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="txtEffectiveDateTo" runat="server" dateformat="dd/MM/yy"></telerik:RadDatePicker>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Vehicle</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboVehicle" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" RadControlsDir="~/script/RadControls/" Skin="WindowsXP" Width="300px">
                    </telerik:RadComboBox>
                </td>
                <td class="formCellLabel">Trailer</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboTrailer" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" RadControlsDir="~/script/RadControls/" Skin="WindowsXP" Width="300px">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Driver</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" RadControlsDir="~/script/RadControls/" Skin="WindowsXP" Width="300px">
                    </telerik:RadComboBox>
                </td>
                <td class="formCellLabel">Profile</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboProfile" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" RadControlsDir="~/script/RadControls/" Skin="WindowsXP" Width="300px" DropDownWidth="500px">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Status</td>
                <td colspan="3" class="formCellField">
                    <telerik:RadComboBox ID="cboStatus" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" RadControlsDir="~/script/RadControls/" Skin="WindowsXP" Width="300px">
                    </telerik:RadComboBox>
                </td>
            </tr>
        </table>
    </fieldset>
        
    <div class="buttonBar">
        <asp:Button ID="btnSearch" runat="server" Text="Search" />
        <asp:Button ID="btnExport" runat="server" Text="Export" />
    </div>

    <telerik:RadGrid PageSize="100" EnableViewState="false" ID="grdResults" AutoGenerateColumns="False" runat="server" AllowSorting="True" AllowMultiRowSelection="false">
        <MasterTableView DataKeyNames="DriverId" >
            <Columns>
                <telerik:GridBoundColumn DataField="DriverTitle" HeaderText="Driver" />
                <telerik:GridBoundColumn DataField="CheckedDate" HeaderText="Checked" />
                <telerik:GridBoundColumn DataField="VehicleTitle" HeaderText="Vehicle" />
                <telerik:GridBoundColumn DataField="VehicleSafetyCheckProfileTitle" HeaderText="Vehicle Profile" />
                <telerik:GridBoundColumn DataField="VehicleSafetyCheckResultTerm" HeaderText="Vehicle Status" />
                <telerik:GridBoundColumn DataField="TrailerTitle" HeaderText="Trailer" />
                <telerik:GridBoundColumn DataField="TrailerSafetyCheckProfileTitle" HeaderText="Trailer Profile" />
                <telerik:GridBoundColumn DataField="TrailerSafetyCheckResultTerm" HeaderText="Trailer Status" />
                <telerik:GridTemplateColumn HeaderText="Safety Check Detail" UniqueName="Results" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <a href="javascript:viewResults('<%# (((Orchestrator.Entities.SafetyCheckCombinedResults)(Container.DataItem)).DriverId.ToString()) %>','<%# (((DateTime)((Orchestrator.Entities.SafetyCheckCombinedResults)(Container.DataItem)).CheckedDate).ToString("s")) %>');">
                            View
                        </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

</asp:Content>
