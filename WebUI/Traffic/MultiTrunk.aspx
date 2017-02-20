<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MultiTrunk.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.MultiTrunk" MasterPageFile="~/WizardMasterPage.Master" Title="Manage Multi-Trunks" %>

<%@ Register Src="../UserControls/MultiTrunk.ascx" TagName="MultiTrunk" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Multi-Trunk</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">


</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Select your multi-trunk</h2>

    <table width="100%">
        <tr>
            <td>
                <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <fieldset>
                    <uc1:MultiTrunk ID="ucMultiTrunk" runat="server" />
                </fieldset>
            </td>
        </tr>
    </table>
    
    <div class="buttonbar">
        <asp:Button ID="btnConfirm" runat="server" Text="Trunk" Width="75" OnClick="btnConfirm_Click1" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" OnClick="btnCancel_Click" />
    </div>
    
</asp:Content>