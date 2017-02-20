<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleRevenueReportExclusions.aspx.cs" Inherits="Orchestrator.WebUI.administration.VehicleRevenueReportExclusions.VehicleRevenueReportExclusions" MasterPageFile="~/default_tableless.Master" Title="Vehicle Revenue Report Exclusions" EnableEventValidation="false" %>

<%@ Import Namespace="Orchestrator.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Vehicle Revenue Report Exclusions</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <h2>Vehicle Revenue Report Exclusions</h2>

    <div>
        <table>
            <tr>
                <td class="formCellLabel">Vehicle</td>
                <td class="formCellField" colspan="2">
                    <telerik:RadComboBox ID="cboVehicle" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" 
                        Skin="WindowsXP" Width="355px"
                        OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" AutoPostBack="True">
                    </telerik:RadComboBox>
                </td>
            </tr>
        </table>

        <div class="buttonbar">
            <asp:Button ID="btnAddNewExclusion" runat="server" Text="Add New Vehicle Exclusion" OnClick="addNewExclusion_Click" />
        </div>
    </div>

    <telerik:RadGrid ID="grdVehicleRevenueReportExclusions" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" EnableAJAX="True" Width="800">
        <MasterTableView DataKeyNames="VehicleResourceId">
            <Columns>
                <telerik:GridBoundColumn HeaderText="Vehicle ID" DataField="VehicleResourceId" HeaderStyle-Width="50px" Visible="false" />
                <telerik:GridBoundColumn HeaderText="Vehicle Name" DataField="VehicleDisplayName" HeaderStyle-Width="50px" />
                <telerik:GridButtonColumn HeaderText="Action" HeaderStyle-Width="20px" ItemStyle-HorizontalAlign="Center" ButtonType="PushButton" CommandName="remove" Text="Remove" ConfirmText="Are you sure you want to remove this vehicle exclusion?"></telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

</asp:Content>
