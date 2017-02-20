<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true"
    CodeBehind="AddFuelSurcharge.aspx.cs" Inherits="Orchestrator.WebUI.FuelSurcharge.AddFuelSurcharge" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add Fuel Surcharge Rate</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script src="../script/scripts.js" type="text/javascript"></script>

    <fieldset>
        <table>
            <tr>
                <td class="formCellLabel">
                    Fuel Surcharge Rate
                </td>
                <td class="formCellField">
                    <telerik:RadNumericTextBox ID="rntFuelSurchargeRate" runat="server" AutoPostBack="false"
                        MaxValue="100.00" MinValue="-100.00" MaxLength="5" Width="50px" TabIndex="1" Culture="en-GB" />&nbsp;%
                </td>
                <td class="formCellField">
                    <asp:RequiredFieldValidator ID="rfvFuelSurchargeRate" runat="server" ControlToValidate="rntFuelSurchargeRate"
                        ErrorMessage="" ToolTip="Please check the rate" Display="dynamic" ValidationGroup="grpAddFuelSurcharge">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Effective From
                </td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="rdiEffectiveFrom" runat="server" AutoPostBack="false" Width="152px"
                        TabIndex="2" />
                </td>
                <td class="formCellField">
                    <asp:RequiredFieldValidator ID="rfvEffectiveFrom" runat="server" ControlToValidate="rdiEffectiveFrom"
                        ErrorMessage="Date is too far in the past." ToolTip="" Display="Dynamic" ValidationGroup="grpAddFuelSurcharge">*</asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button ID="btnClose" CausesValidation="false" runat="server" Text="Cancel" Width="100"
            TabIndex="4" OnClientClick="javascript:window.close();" />
        <asp:Button ID="btnAdd" runat="server" Text="Add" Width="50px" TabIndex="3" ValidationGroup="grpAddFuelSurcharge" />
    </div>
    <asp:Panel ID="pnlFailed" runat="server" Visible="false">
        <div style="font-size: 11px;">
            There was a problem creating the Fuel Surcharge rate, please try again.
        </div>
    </asp:Panel>
</asp:Content>
