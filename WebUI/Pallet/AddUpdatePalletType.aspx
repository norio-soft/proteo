<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Pallet.AddUpdatePalletType" MasterPageFile="~/default_tableless.Master" Codebehind="AddUpdatePalletType.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Configure Pallet Type</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Allows you to control an existing or add a new pallet type</h2>
    <fieldset>
        <legend>Pallet Type Information</legend>
        <table>
            <tr>
                <td class="formCellLabel">Description</td>
                <td class="formCellField">
                    <asp:TextBox id="txtDescription" runat="server" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator id="rfvDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Please supply a description." Display="Dynamic"><img src="../images/error.gif" height="16" width="16" title="Please supply a description." /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Track Pallets By Default</td>
                <td class="formCellField">
                    <asp:CheckBox id="chkTrack" runat="server" Tooltip="When adding a new client any pallet types that are marked will be automatically be marked as trackable for that new client."></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Active (for new clients)</td>
                <td class="formCellField">
                    <asp:CheckBox id="chkForNewClients" runat="server" Tooltip="When adding a new client any pallet types that are marked will be automatically selected for that new client."></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Default Pallet Type (for new clients)</td>
                <td class="formCellField">
                    <asp:CheckBox id="chkIsDefault" runat="server" Tooltip="When adding a new client, the pallet type marked as 'default' will become the client default pallet type."></asp:CheckBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button id="btnAdd" runat="server" Text="Add Pallet Type" />
        <asp:Button id="btnRemoveFromAllClients" runat="server" Text="Remove from all Clients" CausesValidation="False" Visible="false" />
        <asp:Button id="btnReturnToList" runat="server" Text="Return to List" CausesValidation="false" />
    </div>
</asp:content>
