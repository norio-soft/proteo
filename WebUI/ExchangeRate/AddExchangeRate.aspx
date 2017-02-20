<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="AddExchangeRate.aspx.cs" Inherits="Orchestrator.WebUI.ExchangeRate.AddExchangeRate" Title="Untitled Page" %>
<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add Exchange Rate</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="../script/scripts.js" type="text/javascript"></script>    
        <fieldset>
            <table>
                <tr>
                    <td class="formCellLabel">Currency</td>
                    <td class="formCellField"><telerik:RadComboBox ID="rcbCurrency" runat="server" Skin="WindowsXP" AutoPostBack="false" TabIndex="0" /></td>
                    <td class="formCellField"><asp:CustomValidator ID="cfvCurrency" runat="server" ControlToValidate="rcbCurrency" ErrorMessage="" ToolTip="Please check the selected currency" Display="Dynamic" ValidationGroup="grpAddExchangeRate" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Exchange Rate</td>
                    <td class="formCellField"><telerik:RadNumericTextBox ID="rntExchangeRate" runat="server" AutoPostBack="false" MaxValue="5.00000" MinValue="0.00001" MaxLength="7" Width="152px" TabIndex="1" Type="Number" Culture="en-GB" /></td>
                    <td class="formCellField"><asp:RequiredFieldValidator ID="rfvExchangeRate" runat="server" ControlToValidate="rntExchangeRate" ErrorMessage="" ToolTip="Please check the supplied exchange rate" Display="dynamic" ValidationGroup="grpAddExchangeRate" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Effective From</td>
                    <td class="formCellField"><telerik:RadDateInput ID="rdiEffectiveFrom" runat="server" AutoPostBack="false" Width="152px" TabIndex="2" /></td>
                    <td class="formCellField"><asp:RequiredFieldValidator ID="rfvEffectiveFrom" runat="server" ControlToValidate="rdiEffectiveFrom" ErrorMessage="" ToolTip="Effective date cannot be before todays date." Display="Dynamic" ValidationGroup="grpAddExchangeRate" /></td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:button id="btnClose" CausesValidation="false" runat="server" text="Cancel" Width="100" TabIndex="4" />
            <asp:Button ID="btnAdd" runat="server" Text="Add" Width="50px" TabIndex="3" ValidationGroup="grpAddExchangeRate" />
        </div>  
        <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
            <div style="font-size:11px;">
                The Exchange rate was added succesfully.
            </div>                
        </asp:Panel>
        <asp:Panel ID="pnlFailed" runat="server" Visible="false">
            <div style="font-size:11px;">
                There was a problem creating the exchange rate, please try again.
            </div>
        </asp:Panel>

        <script language="javascript" type="text/javascript">
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
            return oWindow;
        }
        </script>

</asp:Content>
