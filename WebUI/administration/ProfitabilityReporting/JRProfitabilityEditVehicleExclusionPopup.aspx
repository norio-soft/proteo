<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JRProfitabilityEditVehicleExclusionPopup.aspx.cs" Inherits="Orchestrator.WebUI.administration.ProfitabilityReporting.JRProfitabilityEditVehicleExclusionPopup" MasterPageFile="~/WizardMasterPage.master" Title="Manage Vehicle Exclusion" %>
<%@ Import Namespace="Orchestrator.Models" %>

<asp:content id="Titlebar1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    Manage Vehicle Exclusion for <%# VehicleRegNo %>
</asp:content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="padding-top:20px">
        <h2>Manage Profitability Report Exclusion for Vehicle <%# VehicleRegNo %></h2>

        <table>
            <tr>
                <td class="formCellLabel">Exclusion Type</td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboVehicleExclusionType" Width="250" DataValueField="Key" DataTextField="Value" />
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Exclusion From Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="dtVariableExclusionFromDate" runat="server" Width="100"><DateInput ID="DateInput1" runat="server" dateformat="dd/MM/yy" ></DateInput></telerik:RadDatePicker>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Exclusion To Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="dtVariableExclusionToDate" runat="server" Width="100"><DateInput ID="DateInput2" runat="server" dateformat="dd/MM/yy" ></DateInput></telerik:RadDatePicker>
                </td>
            </tr>
        </table>
    </div>
    
    <div  style="padding-top:20px">
    <div class="buttonBar">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" /> &nbsp;
        <asp:Button ID="btnSave" runat="server" Text="Save" />
    </div>
   </div>        

    <div style="padding-top:20px">
        <asp:Label id="ErrorMessage" runat="server" CssClass="errorMessage"></asp:Label>
    </div>

</asp:Content>